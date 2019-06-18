using CefSharp;
using CefSharp.OffScreen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WindowsFormsApp1.ConsoleUI;
using WindowsFormsApp1.Helpers;

namespace WindowsFormsApp1.Api
{
    public class GSPNLoginApi
    {
        private static string IdAccess;
        protected string login;
        protected string pass;

        ChromiumWebBrowser wb;
        TaskCompletionSource<String> tcsDocument = null;

        protected String getIDAccessByHtml(String htmlContent)
        {
            String ID = null;

            String regex = @"(\.formValueEncrypt3)\('(.*?)',";

            RegexOptions options = RegexOptions.Multiline;
            foreach (Match m in Regex.Matches(htmlContent, regex, options))
            {
                ID = m.Groups[2].ToString();
            }

            return ID;
        }

        public async Task<String> NewLogin(String login, String pass)
        {
            try
            {
                if (WebRequestHelper.CheckNet() == false) return null;

                WriteConsole.WriteInLine("Logando ", 1, 1);

                this.login = login;
                this.pass = pass;

                if ((IdAccess = await LoginAsync("http://gspn6.samsungcsportal.com/")) != null)
                {
                    return IdAccess;
                }

                return null;
            }
            catch
            {
                throw;
            }
        }

        private void FrameLogin(object s, FrameLoadEndEventArgs e)
        {
            //seta os dados para fazer login
            if (e.Url.ToString().Contains("http://gspn6.samsungcsportal.com/"))
            {
                Console.WriteLine(e.Url.ToString());
                try
                {
                    wb.ExecuteScriptAsync("document.getElementsByName('LOGIN_ID')[0].value = '" + login + "'");
                    wb.ExecuteScriptAsync("document.getElementsByName('LOGIN_PWD')[0].value = '" + pass + "'");
                    wb.ExecuteScriptAsync("javascript:login()");
                }
                catch (Exception ex)
                {
                    return;
                }
            }
        }

        private async void FrameDataAsync(object s, FrameLoadEndEventArgs e)
        {
            if (e.Url.ToString().Contains("basis/login.do"))
            {
                wb.Stop();

                var result = await Cef.GetGlobalCookieManager().VisitAllCookiesAsync();
                var cookies = new System.Net.CookieContainer();

                result.ToList().ForEach(x => cookies.Add(new System.Net.Cookie(x.Name, x.Value, x.Path, x.Domain)));

                //seta o cookie container;
                WebRequestHelper.Cookies = cookies;

                dynamic httpContent = await WebRequestHelper.PostRequestAsync(
                    "http://gspn6.samsungcsportal.com/basis/common/getChangedTime.jsp",
                    "menuId=MAIN_04&urlType=TOP&varUrl=%2Fbasis%2FmenuServlet.do%3Fmethod%3DgetMenuUrl&varTarget=body");
                if (httpContent == null) return;

                //retorna o id de acesso
                var code = getIDAccessByHtml(httpContent.result);
                if (code == null)
                {
                    Cef.GetGlobalCookieManager().Dispose();
                    Console.WriteLine("Erro de conexão no login. Precione qualquer tecla para acesso offline: ");
                    return;
                }

                tcsDocument.TrySetResult(code);
                wb = null;
                return;
            }
        }

        private async void FrameChangePasswordAsync(object s, FrameLoadEndEventArgs e)
        {
            //Envia a requisição para idAccesse ou changeLogin
            if (e.Url.ToString().Contains("basis/login.do"))
            {
                wb.Stop();

                String result = await wb.GetBrowser().MainFrame.GetSourceAsync();
                if (result.Contains("Password Change"))
                {
                    Console.WriteLine("Servidor requer mudança na senha visite o site gspn.");

                    Console.ReadKey();

                    //tcsDocument.TrySetResult(null);
                    //wb.Stop();
                    //return;

                    Console.WriteLine("Servidor requer mudança na senha: ");

                    Console.WriteLine("Senha antiga: ");
                    String senhaAtual = "1wv!2R!5Ya";

                    Console.WriteLine("Nova senha: ");
                    String novaSenha01 = "1wv!2R!5Yz";

                    Console.WriteLine("Confirme nova senha: ");
                    String novaSenha02 = "1wv!2R!5Yz";

                    wb.ExecuteScriptAsync("document.getElementsByName('oldPwd')[0].value = '" + senhaAtual + "'");
                    wb.ExecuteScriptAsync("document.getElementsByName('newPwd')[0].value = '" + novaSenha01 + "'");
                    wb.ExecuteScriptAsync("document.getElementsByName('retypePwd')[0].value = '" + novaSenha02 + "'");
                    wb.ExecuteScriptAsync("javascript:save()");

                    Console.WriteLine("Senha mudada com sucesso.");

                    Console.ReadKey();

                    tcsDocument.TrySetResult(null);
                    wb = null;
                }
            }
            return;
        }

        protected async Task<String> LoginAsync(String url)
        {
            try
            {
                tcsDocument = null;

                Cef.Initialize(new CefSettings { LogSeverity = LogSeverity.Disable, CachePath = "MyCachePath", PersistSessionCookies = false });
                wb = new ChromiumWebBrowser(url);
                wb.JsDialogHandler = new JsHandler();

                //verificar se é necesario a mudança de senha
                wb.FrameLoadEnd += FrameChangePasswordAsync;

                //Envia a requisição para idAccesse ou changeLogin
                wb.FrameLoadEnd += FrameDataAsync;

                //Faz o login
                wb.FrameLoadEnd += FrameLogin;

                tcsDocument = new TaskCompletionSource<String>();

                String cookie = await tcsDocument.Task;
                wb.Stop();
                wb.StopFinding(true);
                wb = null;
                return cookie;
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }

    public class JsHandler : IJsDialogHandler
    {
        public bool OnBeforeUnloadDialog(IWebBrowser chromiumWebBrowser, IBrowser browser, string messageText, bool isReload, IJsDialogCallback callback)
        {
            throw new NotImplementedException();
        }

        public void OnDialogClosed(IWebBrowser chromiumWebBrowser, IBrowser browser)
        {
            throw new NotImplementedException();
        }

        public bool OnJSAlert(IWebBrowser browser, string url, string message)
        {
            return true;
        }

        public bool OnJSConfirm(IWebBrowser browser, string url, string message, out bool retval)
        {
            retval = true;
            return true;
        }

        public bool OnJSDialog(IWebBrowser browserControl, IBrowser browser, string originUrl, CefJsDialogType dialogType, string messageText, string defaultPromptText, IJsDialogCallback callback, ref bool suppressMessage)
        {
            callback.Continue(true);
            return true;
        }

        public bool OnJSPrompt(IWebBrowser browser, string url, string message, string defaultValue, out bool retval, out string result)
        {
            retval = true;
            result = "";
            return true;
        }

        public void OnResetDialogState(IWebBrowser chromiumWebBrowser, IBrowser browser)
        {
            return;
        }
    }
}