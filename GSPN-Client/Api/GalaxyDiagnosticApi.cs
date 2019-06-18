using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsFormsApp1.Class;
using WindowsFormsApp1.Helpers;
using WindowsFormsApp1.Models;

namespace WindowsFormsApp1.Api
{
    public class GalaxyDiagnosticApi
    {
        //retorna o resultado do diagnostico teste
        public async Task<String> GetManufactureInfo(String id)
        {
            try
            {
                dynamic httpContent = await WebRequestHelper.PostRequestAsync(
                    "http://biz6.samsungcsportal.com/gspn/operate.do",
                    "cmd=GDSerialSearchCmd&GD_SESS_ID=" + id);

                dynamic dataDynamic = JsonConvert.DeserializeObject(httpContent.result);
                return dataDynamic.ToString();

            }
            catch
            {
                new Exception();
                return null;
            }
        }

        //verifica se o diagnostico existe
        public static async Task<bool> CheckDiagnosticModel(Aparelho aparelho)
        {
            try
            {
                dynamic httpContent = await WebRequestHelper.PostRequestAsync(
                    "http://biz6.samsungcsportal.com/gspn/operate.do",
                    "cmd=CheckDiagnosticModelCmd&BUKRS=C820"
                    + "&LANGU=ptl&OBJECT_ID=&ASC_CODE=0001768769&"
                    + "EMPLOYEE=&CUSTOMER=&PURCHASE_DATE=&"
                    + "MODEL_CODE=" + aparelho.Modelo
                    + "IV_COMPANY=C820"
                    + "&IV_GSPN_ID=GRAVATAISHOP18"
                    + "&EXT_USER=GRAVATAISHOP18"
                    + "&CC_CODE=6082028015&IV_CC_CODE=6082028015"
                    + "&CP_ASC_CODE=0001768769&SERVICE_DATE=&"
                    + "IV_DATE=" + "20190408"
                    + "&MODEL=" + aparelho.Modelo
                    + "&SERIAL_NO= " + aparelho.RN
                    + "&IMEI=" + aparelho.Imei);

                dynamic dataDynamic = JsonConvert.DeserializeObject(httpContent.result);

                if (dataDynamic.modelCheck != "Y") return false;

                return true;
            }
            catch
            {
                new Exception();
                return false;
            }
        }

        //retorna o resultado do diagnostico
        public static async Task<String> GetDiagnosticResult(String id, Aparelho aparelho)
        {
            try
            {
                dynamic httpContent = await WebRequestHelper.PostRequestAsync(
                    "http://biz6.samsungcsportal.com/gspn/operate.do",
                    "cmd=GDResultMtSearchCmd&GD_SESS_ID=" + id +
                    "&SERIAL_NO=" + aparelho.RN +
                    "&IMEI=" + aparelho.Imei +
                    "&TR_NO=" + "");

                dynamic dataDynamic = JsonConvert.DeserializeObject(httpContent.result);

                if (dataDynamic.dList.Count == 0) return null;

                return dataDynamic.dList[0].ToString();
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}
