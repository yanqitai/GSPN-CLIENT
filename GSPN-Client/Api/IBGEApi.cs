using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsFormsApp1.Helpers;
using WindowsFormsApp1.Services;

namespace WindowsFormsApp1.Api
{
    public class IBGEApi
    {
        public async static Task<String> GetSexoIBGE(String nome)
        {
            try
            {
            String mm = await WebRequestHelper.GetRequestAsync("https://servicodados.ibge.gov.br/api/v2/censos/nomes/" + nome + "?sexo=m");
            String ff = await WebRequestHelper.GetRequestAsync("https://servicodados.ibge.gov.br/api/v2/censos/nomes/" + nome + "?sexo=f");

            dynamic resultMO = JsonConvert.DeserializeObject(mm);
            dynamic resultFO = JsonConvert.DeserializeObject(ff);

            if (resultMO == null || resultMO.Count == 0) return "F";
            if (resultFO == null || resultFO.Count == 0) return "M";

            List<dynamic> m = resultMO.First.res.ToObject<List<dynamic>>();
            List<dynamic> f = resultFO.First.res.ToObject<List<dynamic>>();

            return m.Last().frequencia >= f.Last().frequencia ? "M" : "F";

            }
            catch (Exception e)
            {
                Console.WriteLine("Error IBGEApi\n\nEscreve o sexo para " + nome + " (M/F): ");
                //throw;
                return Console.ReadLine();
            }
       }
    }
}
