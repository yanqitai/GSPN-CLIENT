using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsFormsApp1.Models;

namespace WindowsFormsApp1.Services
{
    public class AparelhoService
    {
        public static async Task serialPorts()
        {
            String[] sp = SerialPort.GetPortNames();

            bool succeeded = false;
            while (sp.Count() == 0)
            {
                Console.Write("-");
                sp = SerialPort.GetPortNames();
                await Task.Delay(1000);
            }
            return;
        }

        public static string DataInfoItemParser(string data)
        {
            string[] strArray = data.Split(new char[] { '(' });
            return strArray[1].Remove(strArray[1].Length - 1);
        }

        public static Aparelho DataInfoParser(string data, string comPort)
        {
            Aparelho ap = new Aparelho();
            string[] strArray = data.Split(new char[] { ';' });

            strArray.ToList().ForEach(x =>
            {
                if (x.Contains("MN("))
                {
                    ap.Modelo = DataInfoItemParser(x);
                }
                else if (x.Contains("UN("))
                {
                    String un = DataInfoItemParser(x);
                }
                else if (x.Contains("IMEI("))
                {
                    ap.Imei = DataInfoItemParser(x);
                }
                else if (x.Contains("SN("))
                {
                    ap.RN = DataInfoItemParser(x);
                }
            });

            return ap;
        }
    }
}
