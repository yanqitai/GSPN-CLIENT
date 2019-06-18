using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WindowsFormsApp1.ConsoleUI;

namespace WindowsFormsApp1.Helpers
{
    public class WebRequestHelper
    {
        public static CookieContainer Cookies = new CookieContainer();

        [System.Runtime.InteropServices.DllImport("wininet.dll")]
        private extern static bool InternetGetConnectedState(out int Description, int ReservedValue);

        public static bool CheckNet()
        {
            int desc;
            return InternetGetConnectedState(out desc, 0);
        }

        public static bool CheckForInternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                using (client.OpenRead("http://clients3.google.com/generate_204"))
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public static async Task<Object> PostRequestAsync(string url, string data, CookieContainer cookie = null)
        {
            var spinner = new Spinner();

            try
            {
                spinner.Start();

                HttpClientHandler handler = new HttpClientHandler();
                handler.CookieContainer = Cookies;

                using (var client = new HttpClient(handler))
                {
                    StringContent content = new StringContent(data, Encoding.UTF8, "application/x-www-form-urlencoded");

                    HttpResponseMessage responseMessage = null;

                    try
                    {
                        responseMessage = await client.PostAsync(url, content);

                        if (responseMessage.StatusCode == HttpStatusCode.OK)
                        {
                            IEnumerable<Cookie> responseCookies = Cookies.GetCookies(new Uri(url)).Cast<Cookie>();

                            dynamic dResult = new NullExpandoObject();

                            dResult.result = responseMessage.Content.ReadAsStringAsync().Result;
                            dResult.cookies = new CookieContainer();

                            responseCookies.ToList().ForEach(x => dResult.cookies.Add(x));

                            return dResult;
                        }

                    }
                    catch (Exception ex)
                    {
                        if (responseMessage == null)
                        {
                            responseMessage = new HttpResponseMessage();
                        }
                        responseMessage.StatusCode = HttpStatusCode.InternalServerError;
                        responseMessage.ReasonPhrase = string.Format("RestHttpClient.SendRequest failed: {0}", ex);
                    }
                    return null;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Erro de Conexão");
                throw e;
            }
            finally
            {
                spinner.Stop();
            }
        }

        public static async Task<String> GetRequestAsync(String url)
        {
            try
            {
                var client = new HttpClient();

                client.BaseAddress = new Uri(url);

                var a = await client.GetAsync("");
                String b = await a.Content.ReadAsStringAsync();

                return b;

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw e;
            }
        }
    }
}