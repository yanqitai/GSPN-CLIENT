using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApp1.ConsoleUI;
using WindowsFormsApp1.Helpers;
using WindowsFormsApp1.Models;

namespace WindowsFormsApp1.Helpers
{
    public class AtCommand
    {
        //serial port def
        SerialPort sp;
        Spinner spinner;
        
        public AtCommand()
        {
            sp = new SerialPort();
            spinner = new Spinner();
        }

        public async Task<string> ExecuteCommand(String command, String comPort)
        {
            try
            {
                String result = "";

                sp = new SerialPort(comPort, 115200);

                sp.DtrEnable = true;
                sp.Open();

                sp.Write(command);

                WriteConsole.WriteInLine("Aguardando ", 1, 1);
                spinner.Start();

                while (sp.BytesToRead <= sp.ReceivedBytesThreshold)
                {

                }

                spinner.Stop();

                var data2 = await sp.SerialReadLineAsync().ConfigureAwait(true);

                sp.Close();

                if (data2 == null) new Exception("Erro ao aguardar retorno de aplicativo");
                return data2;
            }
            catch (Exception er)
            {
                Console.WriteLine("\nOcorreu um erro inesperado ao solicitar data via serial\n");
                sp.Close();
                return er.Message;
            }
            finally
            {
                if(sp.IsOpen)
                    sp.Close();
            }
        }
    }
}
