using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApp1.Models;
using WindowsFormsApp1.Class;
using WindowsFormsApp1.Helpers;
using WindowsFormsApp1.Services;
using System.Diagnostics;
using WindowsFormsApp1.ConsoleHelpers;
using WindowsFormsApp1.ConsoleUI;
using System.IO.Ports;
using System.IO;
using WindowsFormsApp1.Api;
using System.Threading;
using Newtonsoft.Json;

namespace WindowsFormsApp1.Views
{
    public class Main
    {
        public static GSPNApi ApiGSPN;
        static ManualResetEvent resetEvent = new ManualResetEvent(false);

        public Main()
        {
            InitConsole().Wait();
            resetEvent.WaitOne();
        }

        public async Task InitConsole()
        {
            try
            {
                //StartLogin();
                //Inicia a api criando o usuario e iniciando o login

                String IdAccess = await Login.Start();

                if (IdAccess != null)
                {
                    ApiGSPN = new GSPNApi(new Usuario(IdAccess));

                    //inicia o cookie com id de acesso do usuario
                    await ApiGSPN.GetCookieByIdAccess();
                    await ApiGSPN.GetUserData();
                }

                Console.Clear();

                List<ConsoleMenuItem> items = new List<ConsoleMenuItem>();

                items.Add(new ConsoleMenuItem("Pesquisar aparelho by serial port", async () => await StartATAparelhoView()));
                items.Add(new ConsoleMenuItem("Ver Numero de Registros de dia", async () => { await StartCountCadastroClass(); }));
                items.Add(new ConsoleMenuItem("Procurar cliente", () => StartClientClass()));
                items.Add(new ConsoleMenuItem("Set Registros", () => StartCadastroClass()));
                items.Add(new ConsoleMenuItem("Visualizar data serial", () => StartDataSerialFile()));
                items.Add(new ConsoleMenuItem("Visualizar data", () => StartDataFile()));
                items.Add(new ConsoleMenuItem("Start Diagnostico", () => StartDiagnostico()));
                items.Add(new ConsoleMenuItem("Sair", async () => await EndMenu()));

                var menu = new MenuConsole(version.versionCode.ToString() + "IdAccess: " + (ApiGSPN == null ? "Offline" : IdAccess), items);
                menu.RunConsoleMenu();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static async Task StartDiagnostico()
        {
            await new AparelhoClass().StartDiagnostico();
        }

        public static void StartLogin()
        {
            var m_Thread = new Thread(() => Application.Run(new Form()));
            m_Thread.Start();
        }

        public static async Task StartClientClass()
        {
            Console.Clear();

            Console.WriteLine("Digite o cpf: ");

            Cliente c = await new ClienteClass().SearchCliente(Console.ReadLine());
            if (c != null) Console.WriteLine(c.ToString());

            Console.WriteLine();
        }

        public static async Task StartCadastroClass()
        {
            Console.Clear();
            await new RegistroClass().SetRegister();
            Console.WriteLine();
        }

        public static async Task StartDataFile()
        {
            Console.Clear();

            if (!new FileInfo("database/data.txt").Exists) FileHelper.SaveFile("data.txt","");

            await ProcessHelper.CloseProcessByTitle("notepad", "data ");
            ProcessHelper.StartProcess("notepad.exe", "database/data.txt",
                (Screen.PrimaryScreen.Bounds.Width / 30),
                Screen.PrimaryScreen.Bounds.Height / 8);

            Console.WriteLine();
        }

        public static async Task StartDataSerialFile()
        {
            Console.Clear();

            if (!new FileInfo("database/data.txt").Exists) FileHelper.SaveFile("dataSerial.txt", "");

            await ProcessHelper.CloseProcessByTitle("notepad", "dataSerial ");
            ProcessHelper.StartProcess("notepad.exe", "database/dataSerial.txt",
                Screen.PrimaryScreen.Bounds.Width / 30 * 14,
                Screen.PrimaryScreen.Bounds.Height / 8);

            Console.WriteLine();
        }

        public static async Task StartCountCadastroClass()
        {
            Console.Clear();
            await new RegistroClass().GetRegisterByDate();
            Console.WriteLine();
        }

        public static async Task StartATAparelhoView()
        {
            Console.Clear();

            List<Aparelho> aps = await AparelhoClass.SearchAparelhoSerial();
            if (aps == null) return;

            Console.WriteLine();
        }

        public static async Task EndMenu()
        {
            Console.Clear();
            return;
        }
    }
}
