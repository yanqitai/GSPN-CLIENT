using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsFormsApp1.Models;
using WindowsFormsApp1.Services;
using WindowsFormsApp1.Helpers;
using WindowsFormsApp1.Views;
using WindowsFormsApp1.ConsoleHelpers;
using WindowsFormsApp1.ConsoleUI;
using System.Dynamic;
using System.Text.RegularExpressions;

namespace WindowsFormsApp1.Class
{
    public class RegistroClass
    {
        List<Registro> registros;
        List<Registro> pendentes;
        List<Diagnostico> diagnosticos;

        Spinner spinner;
        GSPNApi WebApi;

        public RegistroClass()
        {
            WebApi = Main.ApiGSPN;
            spinner = new Spinner();
        }

        //pesquisa a quantidade de registros em uma data
        public async Task GetRegisterByDate()
        {
            try
            {
                DateTime date = await new DateSelectionConsole().RunConsoleDateSelection();

                WriteConsole.WriteInLine("Pesquisando ", Console.CursorTop + 1, 1);
                List<Registro> aps = await WebApi.GetListRegistroByDate(date);
                if (aps == null) return;

                Console.Clear();

                //escreve o resultado da pesquisa na tela
                aps.ForEach(x =>
                {
                    Console.Write(x.Cliente.Nome + x.Aparelho.Modelo + " " + (x.DiagnosticGD.consul != 1 ? "N" : "Y"));

                    WriteConsole.Separator('/');
                });

                //await aps.ForEachAsync(x =>
                //{
                //    try
                //    {
                //        x.ToString();

                //        //var ap = await WebApi.GetRegistroById(x.Id);

                //        //Console.Write(ap.ToString());

                //        WriteConsole.Separator('/', 1, 1);
                //    }
                //    catch (Exception e)
                //    {
                //        Console.WriteLine(e.Message);
                //    }
                //});
                var aa = aps.Select(x => x.DiagnosticGD.consul != 0);

                Console.WriteLine("\n" + (aps == null ? 0 : aps.Count()) + " registros e "
                    + (aps.Where(x => x.DiagnosticGD.consul == 1).ToList().Count() == 0 ? 0 : aps.Where(x => x.DiagnosticGD.consul == 1).ToList().Count())
                    + " diagnosticos no dia " + date.Date.ToString("dd/MM/yyyy"));
            }
            catch(Exception e)
            {

            }
        }

        protected async Task<dynamic> GetDiagnosticoGD(Aparelho ap)
        {
            Diagnostico dg;
            if ((dg = diagnosticos.ToList().Find(d => d.imei == ap.Imei)) != null)
            {
                return await AparelhoClass.DiagnosticResult(ap, dg.guid.ToString());
            }
            else
            {
                 return new NullExpandoObject();
            }
        }

        protected Registro GetDiagnosticoByConsole(Registro registro)
        {
            Console.WriteLine(
                "\n------------ Nome: " + registro.Cliente.Nome +
                " Modelo: " + registro.Aparelho.Modelo);

            RegistroData.Sintomas.ToList().ForEach(x => Console.WriteLine(RegistroData.Sintomas.ToList().IndexOf(x) + " - " + x.Value));

            Console.WriteLine("Sintoma: ");
            var value = Console.ReadLine();

            if (value == "00")
            {
                RegistroData.SD.ToList().ForEach(x => Console.WriteLine("\n" + " - " + x.Key + " : " + x.Value.Item1 + " | " + x.Value.Item2 + " | " + x.Value.Item3));

                Console.WriteLine("Value: ");
                registro.Type = int.Parse(Console.ReadLine());
                return GetDiagnosticoByType(registro);
            }

            registro.Symptom_Code = RegistroData.Sintomas.ElementAt(int.Parse(value)).Key;

            RegistroData.Diagnosticos.ToList().ForEach(x => Console.WriteLine(RegistroData.Diagnosticos.ToList().IndexOf(x) + " - " + x.Value));
            Console.WriteLine("Diagnostico: ");
            registro.Reason = RegistroData.Diagnosticos.ElementAt(int.Parse(Console.ReadLine())).Key;

            Console.WriteLine("Observação: ");
            registro.MsgDiagnostico = Console.ReadLine();

            return registro;
        }

        protected Registro GetDiagnosticoByType(Registro registro)
        {
            registro.Symptom_Code = RegistroData.SD[registro.Type.ToString()].Item1;
            registro.Reason = RegistroData.SD[registro.Type.ToString()].Item2;
            registro.MsgDiagnostico = RegistroData.SD[registro.Type.ToString()].Item3;

            return registro;
        }

        public async Task SetRegister()
        {
            Console.Clear();
            try
            {
                diagnosticos = await GalaxyDiagnosticService.LoadDiagnostics();
                registros = GSPNDataService.LoadDataRegistros();
                pendentes = await GSPNDataService.LoadDataPendentes();

                //Diagnostico[] diagnosticos = JsonConvert.DeserializeObject<Diagnostico[]>(FileHelper.OpenFile("diagnosticosPendentes.txt"));

                WriteConsole.Separator('-');

                List<Registro> pendenteForEach = (List<Registro>)pendentes.Select(b => b).ToList();


                await pendenteForEach.ForEachAsync(async x =>
                {
                    try
                    {
                        Cliente c = await WebApi.GetClienteByCPF(x.Cliente.Cpf, x.Cliente);
                        if (c == null) c = await WebApi.RegisterNewCliente(x.Cliente);
                        if (c == null) return;

                        x.Aparelho = await WebApi.GetModelData(x.Aparelho);
                        x.Cliente = c;

                        if (x.Cliente == null || x.Aparelho == null) { Console.WriteLine("Erro em construir formulario"); return; };

                        if (x.Reason == null)
                            x = RegistroData.SD.ContainsKey(x.Type.ToString()) ? GetDiagnosticoByType(x) : GetDiagnosticoByConsole(x);

                        x.DiagnosticGD.dynamicGD = await GetDiagnosticoGD(x.Aparelho);

                        x.Data = DateTime.Now.Date.ToString("MM-dd-yyyy");
                        Console.Write(x.ToString());

                        if (registros == null) registros = new List<Registro>();

                        //se o aparelho ja foi registrado no dia ele fica em espera
                        if (registros.Any(y => x.Data == y.Data && x.Aparelho.RN == y.Aparelho.RN))
                            {
                                Console.WriteLine("NOVO REGISTRO PENDENTE");
                                return;
                            };

                        Registro r = await WebApi.NewRegister(x);
                        if (r == null) return;

                        registros.Add(r);
                        pendentes.RemoveAll(a => a.Aparelho.Imei == x.Aparelho.Imei);

                        //salva o registro setado em registrados.txt
                        GSPNDataService.SaveData(registros, pendentes);
                            Console.WriteLine("Salvo com Sucesso");
                    }
                    catch (Exception e)
                    {
                        Console.Write(e.Message);
                    }
                });

                Console.WriteLine("\nFim - Registros setados\n");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
