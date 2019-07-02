using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApp1.Api;
using WindowsFormsApp1.Helpers;

namespace WindowsFormsApp1
{
    public partial class Login
    {
        public Login()
        {
        }

        public static async Task<String> Start()
        {
            try
            {
                dynamic login = JsonConvert.DeserializeObject<NullExpandoObject>(FileHelper.OpenFile("login.json"));

                if (login == null|| string.IsNullOrEmpty(login.user) || string.IsNullOrEmpty(login.pass))
                {
                    await StartLoginFile();

                    throw new Exception("Email ou senha nao informados");
                }

                string id = await new GSPNLoginApi().NewLogin(login.user, login.pass);

                //Th.Invoke((MethodInvoker)delegate
                //{
                //    Th.Hide();
                //});

                if (id == null)
                    throw new Exception("Email ou senha incorretos");
                return id;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static async Task StartLoginFile()
        {
            Console.Clear();

            await ProcessHelper.CloseProcessByTitle("notepad", "Login ");
            ProcessHelper.StartProcess("notepad.exe", "database/login.json",
                Screen.PrimaryScreen.Bounds.Width / 30 * 14,
                Screen.PrimaryScreen.Bounds.Height / 8);

            Console.WriteLine();
        }
    }
}


