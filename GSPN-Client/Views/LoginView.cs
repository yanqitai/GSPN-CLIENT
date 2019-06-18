using System;
using System.Threading.Tasks;
using WindowsFormsApp1.Api;

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
                String id;
                id = await new GSPNLoginApi().NewLogin("GRAVATAISHOP2018", "1wv!2R!5Yz");

                //Th.Invoke((MethodInvoker)delegate
                //{
                //    Th.Hide();
                //});

                return id == null ? null : id;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}


