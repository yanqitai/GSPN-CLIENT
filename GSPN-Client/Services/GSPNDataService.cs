using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WindowsFormsApp1.Models;
using WindowsFormsApp1.Helpers;
using WindowsFormsApp1.Api;
using System.IO;
using WindowsFormsApp1.Services;

namespace WindowsFormsApp1.Services
{
    public class GSPNDataService
    {
        public async static Task<List<Registro>> LoadDataRegitrados()
        {
            List<Registro> registrados = new List<Registro>();

            try
            {
                registrados.AddRange(JsonConvert.DeserializeObject<List<Registro>>(FileHelper.OpenFile("registrados.txt")));
            }
            catch
            {
                Console.WriteLine("json syntax error in registrados.txt");
            }

            return registrados;
        }

        public async static Task<List<Registro>> LoadDataPendentes()
        {
            List<Registro> pendentes = new List<Registro>();

            try
            {
                pendentes.AddRange(JsonConvert.DeserializeObject<List<Registro>>(FileHelper.OpenFile("pendentes.txt")));

                Console.WriteLine("\nRegistros pendentes: " + pendentes.Count());
            }
            catch
            {
                Console.WriteLine("json syntax error in pendentes.txt");
            }

            try
            {
                List<Registro> registrosData = await GetPendentes();
                pendentes.AddRange(registrosData);
            }
            catch
            {
                Console.WriteLine("json syntax error in data.txt");
            }

            RegistroData.SetValues();
            Console.WriteLine("Total de registro pendentes: " + pendentes.Count());

            FileHelper.SaveFile("pendentes.txt", JsonConvert.SerializeObject(pendentes));

            pendentes.ForEach(x => Console.Write(x.ToString()));
            return pendentes;
        }

        public async static Task<List<Registro>> LoadDataRegitrados()
        {
            List<Registro> registrados = new List<Registro>();

            try
            {
                registrados = JsonConvert.DeserializeObject<List<Registro>>(FileHelper.OpenFile("registrados.txt"));

                if(registrados == null) Console.WriteLine("registrados.txt value is null");
                registrados.AddRange(registrados);
            }
            catch(Exception e)
            {
                Console.WriteLine("json syntax error in registrados.txt");
            }

            return registrados;
        }

        public static void SaveData(List<Registro> registros, List<Registro> pendentes)
        {
            try
            {
                if (registros != null)
                    FileHelper.SaveFile("registrados.txt", JsonConvert.SerializeObject(registros));
            }
            catch (Exception e)
            {
                Console.WriteLine("Save error in registros.txt");
            }

            try
            {
                if (pendentes != null)
                    FileHelper.SaveFile("pendentes.txt", JsonConvert.SerializeObject(pendentes));
            }
            catch
            {
                Console.WriteLine("Save error in pendentes.txt");
            }
        }

        private async static Task<List<Registro>> GetPendentes()
        {
            try
            {
                var pendentes = new List<Registro>();
                string file = FileHelper.OpenFile("data.txt");

                List<String> fileList = Regex.Split(file, "--").ToList();
                fileList.RemoveAll(item => item == "");

                await fileList.ForEachAsync(async y =>
                {
                    try
                    {
                        List<String> x = Regex.Split(y, "\r\n").ToList();

                        //remove as linhas vazias
                        while (x.Contains("-")) x.Remove("-");
                        //remove as linhas vazias
                        while (x.Contains("")) x.Remove("");
                        if (x == null || x.Count == 0) return;

                        Cliente cliente = await getClientByString(x);
                        if (cliente == null) return;

                        List<Aparelho> aparelhos = getAparelhoListByString(y);
                        if (aparelhos.Count == 0) return;

                        List<String> types = Regex.Matches(y, @"([0-9])+/").Cast<Match>().Select(match => match.Groups.Cast<Group>().ToList().Last().Value).ToList();

                        foreach (Aparelho aparelho in aparelhos)
                        {
                            Registro registro = new Registro();
                            registro.Cliente = cliente;
                            registro.Aparelho = aparelho;
                            registro.Type = int.Parse(aparelhos.FindIndex(j => j == aparelho) <= types.Count() - 1 ? types[aparelhos.FindIndex(j => j == aparelho)].ToString() : "0");

                            pendentes.Add(registro);
                        }

                        file = file.Replace(y, "");

                    }
                    catch (Exception e)
                    {

                    }
                });

                Console.WriteLine("Novos registros pendentes: " + pendentes.Count());
                Console.WriteLine("Falhados: " + (getAparelhoListByString(string.Join("", fileList.ToArray())).Count() - pendentes.Count()));

                File.WriteAllText("Databse/data.txt", file.Replace(@"\r\n", Environment.NewLine));

                return pendentes;
            }
            catch (Exception e)
            {
                if (e.Source != null)
                    Console.WriteLine("IOException source: {0}", e.Source);

            }
            return null;
        }

        private static List<Aparelho> getAparelhoListByString(String x)
        {
            //List<dynamic> ap = new System.Dynamic.ExpandoObject();
            var aparelho = new List<Aparelho>();

            //aparelhos.Add(new Aparelho(modelo = modelo[], rn = rn[], imei = imei[]));

            List<String> modelos = Regex.Matches(x, @"[A-Z]*-(?:\d+[A-Z]|[A-Z]+\d)[A-Z\d]*").Cast<Match>().Select(m => m.Value).ToList();
            List<String> rns = Regex.Matches(x, @"((^[A-Z]+[A-Za-z0-9]*).)$", RegexOptions.Multiline).Cast<Match>().Select(m => m.Value).ToList();
            List<String> imeis = Regex.Matches(x, @"[\d]{15}").Cast<Match>().Select(m => m.Value).ToList();

            if (Convert.ToDouble(modelos.Count + rns.Count + imeis.Count) / 3 != modelos.Count)
            {
                Console.WriteLine("sintax error in Aparelho");
                return aparelho;
            }

            var results = modelos.ZipThree(rns, imeis, (m, r, i) => new { Modelo = m, RN = r, IMEI = i });

            foreach (var mri in results)
            {
                Aparelho ob = new Aparelho();

                ob.Modelo = mri.Modelo.ToString();
                ob.RN = mri.RN.Replace("\r", string.Empty).ToString();
                ob.Imei = mri.IMEI.ToString();

                aparelho.Add(ob);
            }

            return aparelho;
        }

        private static async Task<Cliente> getClientByString(List<String> x)
        {

            Cliente cliente = new Cliente();

            //verifica se a primeira linha é um modelo de aparelho, se sim cliente não existe
            if (x[0] == Regex.Match(x[0], @"[A-Z]*-(?:\d+[A-Z]|[A-Z]+\d)[A-Z\d]*").Value)
            {
                return null;
            }

            //verifica se a primeira linha é cpf
            if (Regex.Match(x[0], @"[0-9]{11}").Value == x[0])
            {
                cliente.Cpf = x[0];
                return cliente;
            }


            if ((cliente.Contatar = x[0].Contains("/1") ? 2 : 1) == 1)
            {
                x[0] = x[0].Replace("/1"," ");
            }

            if (Regex.Split(x[0], " ").Count() == 1) return null;

            cliente.Nome = Regex.Split(x[0], " ")[0];
            cliente.Sobrenome = x[0].Replace(Regex.Split(x[0], " ")[0] + " ", "");
            cliente.Cpf = x[1];
            cliente.Telefone = x[2].Length == 10 ? x[2] : "";
            cliente.Celular = x[2].Length == 11 ? x[2] : "";
            cliente.Sexo = await IBGEApi.GetSexoIBGE(Regex.Split(x[0], " ")[0]);
            //cliente.Sexo = x[3] == "M" ? "Masculino"  : "Feminino";
            return cliente;
        }
    }
}
