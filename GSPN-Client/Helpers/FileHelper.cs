using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1.Helpers
{
    public class FileHelper
    {
        public static void SaveFile(String nome, String conteudo, bool overwriter= true)
        {
            if (!overwriter)
            {
                File.AppendAllText("Database/" + nome, conteudo, Encoding.UTF8);
            }
            else
            {
                File.WriteAllText("Database/" + nome, conteudo, Encoding.UTF8);
            }
        }

        public static String OpenFile(String nome, String caminho = "Database/")
        {   
            string path = caminho + nome;

            FileInfo file = new FileInfo(path);
            if (!file.Exists)
            {
                file.Directory.Create();
                file.Create().Dispose();
            }

            string text = File.ReadAllText(path);
            Console.WriteLine(nome + " Aberto");
            return text;
        }
    }
}
