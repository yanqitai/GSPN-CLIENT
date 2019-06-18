using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsFormsApp1.Helpers;
using WindowsFormsApp1.Models;

namespace WindowsFormsApp1.Services
{
    public class GalaxyDiagnosticService
    {
        public static void SaveFile(List<Diagnostico> diagnostics)
        {
            try
            {
                if (diagnostics != null)
                    FileHelper.SaveFile("diagnosticosPendentes.txt", JsonConvert.SerializeObject(diagnostics));
            }
            catch (Exception e)
            {
                Console.WriteLine("Save error in diagnosticosPendentes.txt");
            }
        }

        public async static Task<List<Diagnostico>> LoadDiagnostics()
        {
            List<Diagnostico> registrados = new List<Diagnostico>();

            try
            {
                registrados.AddRange(JsonConvert.DeserializeObject<List<Diagnostico>>(FileHelper.OpenFile("diagnosticosPendentes.txt")));
            }
            catch
            {
                Console.WriteLine("json syntax error in diagnosticos.txt");
            }

            return registrados;
        }

        public static async Task<dynamic> DiagnosticFormatter(String strDiagnosticResult)
        {
            try
            {
                var arrDiagnosticResult = strDiagnosticResult.Split(';');

                dynamic MyDynamic = new NullExpandoObject();

                var diaResult = "";
                var diaVersion = "";
                var diaSerialNo = "";
                var diaTotalTime = "";
                var diaRunDate = "";
                var diaResultCode = "";
                var diaMobildeVersion = "";
                var diaCheckVersion = "";

                for (var i = 0; i < arrDiagnosticResult.Length; i++)
                {
                    if (DiaGetFieldName(arrDiagnosticResult[i]) == "TR")
                    {
                        diaResult = MobileGetRealData(arrDiagnosticResult[i]);
                        continue;
                    }
                    else if (DiaGetFieldName(arrDiagnosticResult[i]) == "PCVER")
                    {
                        diaVersion = MobileGetRealData(arrDiagnosticResult[i]);
                        continue;
                    }
                    else if (DiaGetFieldName(arrDiagnosticResult[i]) == "SN")
                    {
                        diaSerialNo = MobileGetRealData(arrDiagnosticResult[i]);
                        continue;
                    }
                    else if (DiaGetFieldName(arrDiagnosticResult[i]) == "TotalTime")
                    {
                        diaTotalTime = MobileGetRealData(arrDiagnosticResult[i]);
                        continue;
                    }
                    else if (DiaGetFieldName(arrDiagnosticResult[i]) == "TestTime")
                    {
                        diaRunDate = MobileGetRealData(arrDiagnosticResult[i]);
                        continue;
                    }
                    else if (DiaGetFieldName(arrDiagnosticResult[i]) == "PR")
                    {
                        diaResultCode = MobileGetRealData(arrDiagnosticResult[i]);
                        continue;
                    }
                    else if (DiaGetFieldName(arrDiagnosticResult[i]) == "VER")
                    {
                        diaMobildeVersion = MobileGetRealData(arrDiagnosticResult[i]);
                        continue;
                    }
                    else if (DiaGetFieldName(arrDiagnosticResult[i]) == "CHKVER")
                    {
                        diaCheckVersion = MobileGetRealData(arrDiagnosticResult[i]);
                        continue;
                    }
                    else if (DiaGetFieldName(arrDiagnosticResult[i]) == "MODEL_GD")
                    {
                        MyDynamic.IV_BASIC_MODEL_GD = MobileGetRealData(arrDiagnosticResult[i]);
                        continue;
                    }
                    else if (DiaGetFieldName(arrDiagnosticResult[i]) == "IMEI_GD")
                    {
                        MyDynamic.IV_IMEI_GD = MobileGetRealData(arrDiagnosticResult[i]);
                        continue;
                    }
                    else if (DiaGetFieldName(arrDiagnosticResult[i]) == "SN_GD")
                    {
                        MyDynamic.IV_SN_GD = MobileGetRealData(arrDiagnosticResult[i]);
                        continue;
                    }
                    else if (DiaGetFieldName(arrDiagnosticResult[i]) == "UN_GD")
                    {
                        MyDynamic.IV_UN_GD = MobileGetRealData(arrDiagnosticResult[i]);
                        continue;
                    }
                    else if (DiaGetFieldName(arrDiagnosticResult[i]) == "OCTA_CELL_ID_GD")
                    {
                        MyDynamic.IV_OCTA_CELL_ID_GD = MobileGetRealData(arrDiagnosticResult[i]);
                        continue;
                    }
                    else if (DiaGetFieldName(arrDiagnosticResult[i]) == "C_VER_GD")
                    {
                        MyDynamic.IV_C_VER_GD = MobileGetRealData(arrDiagnosticResult[i]);
                        continue;
                    }
                    else if (DiaGetFieldName(arrDiagnosticResult[i]) == "BUYER_CODE_GD")
                    {
                        MyDynamic.IV_BUYER_CODE_GD = MobileGetRealData(arrDiagnosticResult[i]);
                        continue;
                    }
                    else if (DiaGetFieldName(arrDiagnosticResult[i]) == "ROOTED_GD")
                    {
                        MyDynamic.IV_ROOTED_GD = MobileGetRealData(arrDiagnosticResult[i]);
                        continue;
                    }
                    else if (DiaGetFieldName(arrDiagnosticResult[i]) == "CAM_FRONT")
                    {
                        MyDynamic.IV_CAM_FRONT = MobileGetRealData(arrDiagnosticResult[i]);
                        continue;
                    }
                    else if (DiaGetFieldName(arrDiagnosticResult[i]) == "CAM_REAR")
                    {
                        MyDynamic.IV_CAM_REAR = MobileGetRealData(arrDiagnosticResult[i]);
                        continue;
                    }
                }

                if (diaResult == null || diaResult == "")
                {
                    // TR결과가 없는것도 시스템 오류
                    MyDynamic.DIA_RESULT = "NONE";
                }

                if (diaResult.IndexOf("ERROR") > -1)
                {
                    MyDynamic.DIA_RESULT = "FAIL";
                    MyDynamic.DIA_ERROR = diaResult.Replace("ERROR,", "");
                }
                else
                {
                    MyDynamic.DIA_RESULT = diaResult;
                }

                MyDynamic.DIA_DATE = changeDateFormat(diaRunDate);
                MyDynamic.DIA_TIME = diaTotalTime;
                MyDynamic.DIA_VERSION = diaVersion;
                MyDynamic.DIA_RESULT_CODE = diaResultCode;
                MyDynamic.DIA_SW_VERSION = diaMobildeVersion;
                MyDynamic.DIA_VERSION_CHECK = diaCheckVersion;
                if (diaRunDate == null || diaRunDate == "")
                {
                    diaRunDate = "0000.00.00 0:00 am";
                }
                MyDynamic.IV_TEST_END_TIME = diaRunDate;

                return MyDynamic;

            }
            catch (Exception e)
            {
                new Exception("Erro ao particionar resultado do diagnóstico");
                return null;
            }
        }

        private static string changeDateFormat(String dateString)
        {
            var date = dateString.Split(' ')[0];
            if (date == null || date == "" || date.Split('.').Length != 3)
            {
                return "";
            }
            var year = date.Split('.')[0];
            var month = date.Split('.')[1];
            var day = date.Split('.')[2];
            if (month.Length < 2) month = "0" + month;
            if (day.Length < 2) day = "0" + day;
            //var gubun = compFormatCheck("MM.dd.yyyy");
            var format = "MM.dd.yyyy".Split('.');
            date = "";
            for (var i = 0; i < format.Length; i++)
            {
                if (i != 0) date += '.';
                if (format[i].ToUpper().IndexOf("Y") > -1)
                {
                    date += year;
                }
                else if (format[i].ToUpper().IndexOf("M") > -1)
                {
                    date += month;
                }
                else if (format[i].ToUpper().IndexOf("D") > -1)
                {
                    date += day;
                }
            }
            return date;
        }



        private static string DiaGetFieldName(string data)
        {
            var startIndex = data.IndexOf("(");
            return data.Substring(0, startIndex);
        }

        private static string MobileGetRealData(string data)
        {
            var startIndex = data.IndexOf("(");
            var endIndex = data.IndexOf(")") - 1;

            return data.Substring(startIndex + 1, endIndex - startIndex);
        }
    }
}
