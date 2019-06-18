using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApp1.Api;
using WindowsFormsApp1.ConsoleUI;
using WindowsFormsApp1.Helpers;
using WindowsFormsApp1.Models;
using WindowsFormsApp1.Services;
using WindowsFormsApp1.Views;

namespace WindowsFormsApp1.Class
{
    public class AparelhoClass
    {
        Spinner spinner;
        GSPNApi WebApi;
        GalaxyDiagnosticApi GDApi;

        public AparelhoClass()
        {
            WebApi = Main.ApiGSPN;
            spinner = new Spinner();
        }

        public async Task StartDiagnostico()
        {
            GDApi = new GalaxyDiagnosticApi();

            List<Aparelho> aparelhos = (await SearchAparelhoSerial(false,false));
            if (aparelhos == null) return;

            Guid g = Guid.NewGuid();

            //var a = await GalaxyDiagnosticApi.CheckDiagnosticModel(aparelho);
            //var a = await WebApi.GetManufactureInfo("6dba7eea-6ef5-0880-6632-82ca8fbb7f0m");

            ProcessHelper.StartProcess("gdlauncher2:OpenForm?GD_REGION_CODE=AM" +
                "&GD_COMP_CODE=C820" +
                "&GD_ASC_CODE=6082028015" +
                "&GD_USER_ID=GRAVATAISHOP18" +
                "&GD_PROC_TYPE=TA" +
                "&GD_SESS_ID=" + g.ToString() +
                "&GD_SERIAL_NO=" + aparelhos[0].RN +
                "&GD_IMEI=" + aparelhos[0].Imei +
                "&GD_LATEST_VER=;;" +
                "&GD_SW_VER=99" +
                "&GD_SES_FLAG=S" +
                "&GD_TR_NO=" +
                "&GD_BASE_URL=http://gspngd6.samsungcsportal.com/" +
                "&GD_LANG_SAP=P" +
                "&GD_LANG_WEB=ptl" +
                "&GD_CUST_SYMP=FAL");

            var diags = await GalaxyDiagnosticService.LoadDiagnostics();
            diags.Add(new Diagnostico() { guid = g, imei = aparelhos[0].Imei });

            GalaxyDiagnosticService.SaveFile(diags);
        }

        public static async Task<dynamic> DiagnosticResult(Aparelho ap, String guid)
        {
            //get the result
            String result = await GalaxyDiagnosticApi.GetDiagnosticResult(guid, ap);
            //parse the result
            dynamic content = await GalaxyDiagnosticService.DiagnosticFormatter(result.ToString());
            return content;

            //var a = await GDApi.GetManufactureInfo(guid);
        }

        //the openData = false not open txt file
        public static async Task<List<Aparelho>> SearchAparelhoSerial(bool openData = true, bool saveData = true)
        {
            String[] sp = SerialPort.GetPortNames();

            Func<Task<bool>> taskSearchDevice = async () =>
                {
                    sp = SerialPort.GetPortNames();
                    var spinner = new Spinner();

                    WriteConsole.WriteInLine("Pesquisando ", 1, 1);
                    spinner.Start();
                        
                    int count = 0;
                    while (sp.Count() == 0)
                    {
                        sp = SerialPort.GetPortNames();
                        count++;
                        await Task.Delay(1000);
                        if (count >= 5)
                        {
                            spinner.Stop();
                            Console.Clear();
                            return false;
                        }
                    }

                    spinner.Stop();

                    return true;
                };

            if (!await taskSearchDevice()) return null;

            var aps = await CallDeviceInfo(sp);
            if (aps == null) return null;

            aps.ForEach( ap =>
            {

                Console.WriteLine("\n" + ap.ToString() + "\n");

                if (!saveData) return;

                FileHelper.SaveFile("dataSerial.txt",
                    Environment.NewLine + "\n-----------------------------"
                    + Environment.NewLine + ap.Modelo
                    + Environment.NewLine + ap.RN
                    + Environment.NewLine + ap.Imei, false);
            });

            if (!openData || !saveData) return aps;

            await ProcessHelper.CloseProcessByTitle("notepad", "dataSerial ");
            ProcessHelper.StartProcess("notepad.exe", "database/dataSerial.txt",
                Screen.PrimaryScreen.Bounds.Width / 20,
                Screen.PrimaryScreen.Bounds.Height / 8);

            return aps;
        }

        private static async Task<List<Aparelho>> CallDeviceInfo(String[] sp)
        {
            try
            {
                List<Aparelho> aps = new List<Aparelho>();

                AtCommand atc = new AtCommand();

                await sp.ToList().ForEachAsync(async x =>
                {
                    Console.Clear();
                    //String valor1 = await atc.ExecuteCommand("ATZ\r\n", x);
                    String valor2 = await atc.ExecuteCommand("AT+DEVCONINFO\r\n", x);
                    //String valor3 = await atc.ExecuteCommand("AT+SVCIFPGM=1,1", x);

                    Aparelho ap = AparelhoService.DataInfoParser(valor2, x);
                    if (ap.Modelo == null) return;

                    aps.Add(ap);
                });

                return aps;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }
    }
}
