using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsFormsApp1.Models;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Net;
using WindowsFormsApp1.Helpers;
using Newtonsoft.Json;
using WindowsFormsApp1.Api;
using System.Threading;
using WindowsFormsApp1.Class;
using System.Text.RegularExpressions;

namespace WindowsFormsApp1.Services
{
    public class GSPNApi
    {
        public Usuario usuario;

        public GSPNApi(Usuario user)
        {
            usuario = user;
        }

        public async Task GetUserData()
        {
            try
            {
                dynamic httpContent = await WebRequestHelper.PostRequestAsync(
                    "http://biz6.samsungcsportal.com/sessis/New.jsp", "");

                String content = httpContent.result.Replace("\"", "\'");
                String[] dados = {
                    "BUKRS",
                    "PROCESS_TYPE",
                    "LANGU",
                    "IV_JOBFLAG",
                    "SYMPTOM1_CODE",
                    "STATUS",
                    "GD_ASC_CODE",
                    "IV_GSPN_ID",
                    "IV_COMPANY"};

                IDictionary<string, object> dic = new Dictionary<string, object>();

                string id = "";
                dados.ToList().ForEach(x =>
               {
                   RegexOptions options = RegexOptions.Multiline;
                   foreach (Match m in Regex.Matches(content, x + ".*=\\s*\'([^\']*)\'", options))
                   {
                       if (m.Groups[1].ToString() != null)
                       {
                           dic.Add(x, m.Groups[1].ToString());
                           return;
                       }
                   }
               });

                Usuario u = ObjectDictionaryMapper<Usuario>.GetObject(dic);

                u.IdAccess = usuario.IdAccess;
                usuario = u;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task GetCookieByIdAccess()
        {
            try
            {
                dynamic httpContent = await WebRequestHelper.PostRequestAsync(
                    "http://biz6.samsungcsportal.com/gspn/operate.do",
                    "cmd=PortalLoginUserCmd&loginAsId=&loginAsCorpCode=&helpReqNo=&" +
                    "userId=" + usuario.IdAccess +
                    "&companyCode=&dateFormat=MM.dd.yyyy&timeFormat=a_hh%40mm%40ss&listLineNo=10&" +
                    "language=ptl&timeZone=America%2FBuenos_Aires");

                String content = httpContent.result;

                //seta o cookie container
                WebRequestHelper.Cookies = httpContent.cookies;
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        public async Task<Registro> GetRegistroById(String ticket)
        {
            try
            {
                dynamic httpContent = await WebRequestHelper.PostRequestAsync(
                    "http://biz6.samsungcsportal.com/gspn/operate.do",
                    "IV_JOBFLAG=3" +
                    "&IV_TR_NO=" + ticket +
                    "&cmd=ZifGspnConsulCreateCmd");

                dynamic data = JObject.Parse(httpContent.result);
                if (data == null || data.EV_RET_MSG != "Success.") return null;

                Registro rg = new Registro()
                {
                    Id = data.eS_TICKET_INFO.object_id,
                    Aparelho = new Aparelho() { Modelo = data.eS_TICKET_INFO.model_code, RN = data.eS_TICKET_INFO.serial_no, Imei = data.eS_TICKET_INFO.imei },
                    Cliente = await GetClienteById(data.eS_TICKET_INFO.consumer),
                    Reason = data.eS_TICKET_INFO.reason,
                    Symptom_Code = data.eS_TICKET_INFO.symptom2_code
                };

                if (data.eT_TEXT.ToObject<List<dynamic>>().Count == 0) return rg;

                rg.MsgDiagnostico = data.eT_TEXT.ToObject<List<dynamic>>()[0].tdline;

                return rg;
            }
            catch (Exception e)
            {
                Console.WriteLine("Não foi possivel carregar o registro : " + e.Message);
                return null;
            }
        }

        public async Task<List<Registro>> GetListRegistroByDate(DateTime date)
        {
            try
            {
                List<Registro> rg = new List<Registro>();

                dynamic httpContent = await WebRequestHelper.PostRequestAsync(
                    "http://biz6.samsungcsportal.com/gspn/operate.do", "numPerPage=&currPage=&"
                    + "IV_COMPANY=" + usuario.IV_COMPANY
                    + "&IV_EDATE=" + date.Date.ToString("yyyyMMdd")
                    + "&IV_JOBFLAG=1&IV_OBJECT_ID=&"
                    + "IV_ORG_CODE=" + usuario.GD_ASC_CODE
                    + "&IV_PARTNER=" + usuario.IV_GSPN_ID
                    + "&IV_REF_SO_NO="
                    + "&IV_SDATE=" + date.Date.ToString("yyyyMMdd")
                    + "&IV_STATUS="
                    + "&IV_SYMPTOM=&IV_REASON=&IV_IMEI=&IV_CONSUL=&IV_GSPNID=&cmd=ZifGspnConsulListCmd");

                dynamic dataRegistroDynamic = JObject.Parse(httpContent.result);

                if (dataRegistroDynamic == null || dataRegistroDynamic.EV_RET_MSG != "Success.") return null;

                foreach (var data in dataRegistroDynamic.eT_DETAIL)
                {
                    rg.Add(new Registro()
                    {
                        Id = data.object_id,
                        DiagnosticGD = new Diagnostico { consul = data.consul},
                        Aparelho = new Aparelho() { Modelo = data.zzmodel },
                        Cliente = new Cliente() { Id = data.zzconsumer, Nome = data.consumer_name }
                    });
                }

                return rg;
            }
            catch (Exception e)
            {
                new Exception();
                return null;
            }
        }

        public async Task<Aparelho> GetModelData(Aparelho ap)
        {
            try
            {
                dynamic httpContent = await WebRequestHelper.PostRequestAsync(
                    "http://biz6.samsungcsportal.com/gspn/operate.do",
                    "cmd=ServiceOrderModelSearchCmd&MODEL=" + ap.Modelo + "&ASC_CODE=0001768769");

                String content = httpContent.result;
                dynamic dataclientesDynamic = JObject.Parse(content);

                foreach (var data in dataclientesDynamic.etModelInfoList)
                {
                    ap.Model_Desc = data.model_desc;
                    ap.Local_Svc_Prod_Desc = data.local_svc_prod_desc;
                    ap.Local_Svc_Prod = data.local_svc_prod;
                    ap.Hq_Svc_Prod = data.hq_svc_prod;
                    ap.Prod_Category = data.prod_category;
                }

                return ap;
            }
            catch
            {
                new Exception();
            }

            return null;
        }

        public async Task<Cliente> GetClienteById(Object id, Cliente c = null)
        {
            try
            {
                dynamic httpContentJson = await WebRequestHelper.PostRequestAsync(
                    "http://biz6.samsungcsportal.com/gspn/operate.do",
                    "BP_NO=" + id + "&cmd=SVCPopCustomerDetailCmd");

                String json = httpContentJson.result;
                dynamic dataDynamic = JObject.Parse(json);

                if (dataDynamic.dataLists.ToObject<List<dynamic>>().Count == 0) return null;

                return await ClienteFormatter(dataDynamic, c);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        protected async Task<Cliente> ClienteFormatter(dynamic DataDynamic, Cliente c = null)
        {
            try
            {
                Cliente cliente = new Cliente();

                foreach (var data in DataDynamic.dataLists)
                {
                    cliente.Nome = data.NAME_FIRST;
                    cliente.Sobrenome = data.NAME_LAST;
                    cliente.Id = data.CONSUMER;
                    cliente.Cpf = data.UNIQUE_ID;
                    cliente.Telefone = data.HOME_PHONE;
                    cliente.Celular = data.MOBILE_PHONE;
                    cliente.Sexo = data.GENDER;
                    cliente.AddrNumber = data.ADDRNUMBER;
                    cliente.Contatar = data.CONTACT_FLAG;
                }

                if (c == null) return cliente;

                Cliente cValidado = ClienteClass.ValidaCliente(cliente, c);

                if (cliente != cValidado) await SetUserEdit(cValidado);

                return cValidado;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public async Task<Cliente> GetClienteByCPF(String cpf, Cliente c = null)
        {
            try
            {
                dynamic httpContent2 = await WebRequestHelper.PostRequestAsync(
                    "http://biz6.samsungcsportal.com/gspn/operate.do",
                    "cmd=SVCPopCustomerSearchCmd&numPerPage=10&currPage=0&BP_NO=&bpKind=&"
                    + "IV_DIVISION=X&country=BR&Sequence=&firstName=&lastName=&phone="
                    + "&uniqueID=" + cpf
                    + "&bpno=&D_TITLE=&D_NAME_FIRST=&D_NAME_LAST="
                    + "&D_CONSUMER=&D_GENDER=&D_UNIQUE_ID=&D_PREFERENCE_CHANNEL=&D_HOME_PHONE="
                    + "&D_OFFICE_PHONE=&D_OFFICE_PHONE_EXT=&D_MOBILE_PHONE=&D_FAX=&D_EMAIL="
                    + "&D_CONTACT_FLAG=&D_STREET1=&D_STREET2=&D_STREET3=&D_DISTRICT=&"
                    + "D_CITY=&D_CITY_CODE=&D_REGION_CODE=&D_REGION=&D_COUNTRY=&D_POST_CODE=");

                //dynamic httpContent = await WebRequestHelper.PostRequestAsync(
                //"http://biz6.samsungcsportal.com/gspn/operate.do",
                //"uniqueID=" + cpf + "&cmd=SVCPopCustomerDetailCmd");

                if (httpContent2 == null) return null;

                dynamic dataDynamic = JsonConvert.DeserializeObject(httpContent2.result);

                if (dataDynamic.dataLists.ToObject<List<dynamic>>().Count == 0) return null;
                var r = await GetClienteById(dataDynamic.dataLists[0].CONSUMER);

                return r;

                //return await ClienteFormatter(dataDynamic, c);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public async Task<bool> SetUserEdit(Cliente cliente)
        {
            Console.WriteLine("Editando usuário");

            return true;
        }

        public async Task<Cliente> RegisterNewCliente(Cliente cliente)
        {
            if (cliente.Sexo == null) cliente.Sexo = await IBGEApi.GetSexoIBGE(cliente.Nome);
            try
            {
                dynamic httpContent = await WebRequestHelper.PostRequestAsync(
                    "http://biz6.samsungcsportal.com/gspn/operate.do",
                    "cmd=SVCPopCustomerCreateCmd&numPerPage=10&currPage=0&BP_TYPE=C001&TITLE=000" + (cliente.Sexo == "F" ? "1" : "2") +
                    "&NAME_FIRST=" + cliente.Nome +
                    "&NAME_LAST=" + cliente.Sobrenome +
                    "&GENDER=" + cliente.Sexo +
                    "&UNIQUE_ID=" + cliente.Cpf +
                    "&PREFERENCE_CHANNEL=" + cliente.Sexo +
                    "&HOME_PHONE=" + cliente.Telefone +
                    "&OFFICE_PHONE=&OFFICE_PHONE_EXT=" +
                    "&MOBILE_PHONE=" + cliente.Celular +
                    "&FAX=&EMAIL=&CONTACT_FLAG=" + cliente.Contatar +
                    "&STREET1=&STREET2=&STREET3=&DISTRICT=&CITY=&CITY_CODE=&REGION=Acre&REGION_CODE=AC&COUNTRY=BR&POST_CODE=");

                Cliente c = await GetClienteByCPF(cliente.Cpf, cliente);
                if (c == null) return null;
                return c;
            }
            catch (Exception e)
            {
                Console.WriteLine("Erro ao cliar cliente : " + e.Message);
                return null;
            }
        }

        public async Task<Registro> NewRegister(Registro registro)
        {
            Console.WriteLine("\n\nPrecione qualquer tecla para confirmar: ");
            Console.ReadKey();

            dynamic r;

            try
            {
                dynamic httpContent = await WebRequestHelper.PostRequestAsync(
                    "http://biz6.samsungcsportal.com/gspn/operate.do",
                    "cmd=ZifGspnConsulCreateCmd"
                    + "&BUKRS=" + usuario.BUKRS
                    + "&LANGU=" + usuario.LANGU
                    + "&OBJECT_ID="
                    + "&ASC_CODE=0001768769"
                    + "&EMPLOYEE="
                    + "&CUSTOMER="
                    + "&PURCHASE_DATE=&"
                    + "MODEL_CODE=" + registro.Aparelho.Modelo
                    + "&TR_TYPE="
                    + "&PROCESS_TYPE=" + usuario.PROCESS_TYPE
                    + "&SYMPTOM1_CODE=" + usuario.SYMPTOM1_CODE
                    + "&SYMPTOM2_CODE=" + registro.Symptom_Code
                    + "&SYMPTOM3_CODE=01"
                    + "&RESOLUTION=01"
                    + "&STATUS=" + usuario.STATUS
                    + "&REASON=" + registro.Reason
                    + "&PRODUCT_CODE=HHP"
                    + "&DETAIL_FAULT=" + WebUtility.UrlEncode("SES : 	| " + registro.MsgDiagnostico + "}{")
                    + "&IV_JOBFLAG=1"
                    + "&ORG_CODE=" + usuario.GD_ASC_CODE
                    + "&IV_TR_NO=&"
                    + "IV_COMPANY=" + usuario.BUKRS
                    + "&IV_GSPN_ID=" + usuario.IV_GSPN_ID
                    + "&EXT_USER=" + usuario.IV_GSPN_ID
                    + "&CC_CODE=" + usuario.GD_ASC_CODE
                    + "&IV_CC_CODE=" + usuario.GD_ASC_CODE
                    + "&CP_ASC_CODE=0001768769"
                    + "&SERVICE_DATE="
                    + "&SERVICE_TIME="
                    + "&CURR_STATUS="
                    + "&objectID=&"
                    + "IV_DATE=" + DateTime.Now.Date.ToString("yyyyMMdd")
                    + "&DIA_SKU="
                    + "&DIA_SW_VERSION=" + registro.DiagnosticGD.dynamicGD.DIA_SW_VERSION
                    + "&DIA_VERSION=" + registro.DiagnosticGD.dynamicGD.DIA_VERSION
                    + "&DIA_DATE=" + registro.DiagnosticGD.dynamicGD.DIA_DATE
                    + "&DIA_TIME=" + registro.DiagnosticGD.dynamicGD.DIA_TIME
                    + "&DIA_VERSION_CHECK=" + registro.DiagnosticGD.dynamicGD.DIA_VERSION_CHECK
                    + "&DIA_RESULT=" + registro.DiagnosticGD.dynamicGD.DIA_RESULT
                    + "&DIA_ERROR=" + registro.DiagnosticGD.dynamicGD.DIA_ERROR
                    + "&DIA_RESULT_CODE=" + registro.DiagnosticGD.dynamicGD.DIA_RESULT_CODE
                    + "&DIA_CHECK_FLAG=Y"
                    + "&SES_FLAG=S"
                    + "&IV_TEST_END_TIME=" + registro.DiagnosticGD.dynamicGD.IV_TEST_END_TIME
                    + "&IV_BASIC_MODEL_GD=" + registro.DiagnosticGD.dynamicGD.IV_BASIC_MODEL_GD
                    + "&IV_IMEI_GD=" + registro.DiagnosticGD.dynamicGD.IV_IMEI_GD
                    + "&IV_SN_GD=" + registro.DiagnosticGD.dynamicGD.IV_SN_GD
                    + "&IV_UN_GD=" + registro.DiagnosticGD.dynamicGD.IV_UN_GD
                    + "&IV_OCTA_CELL_ID_GD=" + registro.DiagnosticGD.dynamicGD.IV_OCTA_CELL_ID_GD
                    + "&IV_C_VER_GD=" + registro.DiagnosticGD.dynamicGD.IV_C_VER_GD
                    + "&IV_BUYER_CODE_GD=" + registro.DiagnosticGD.dynamicGD.IV_BUYER_CODE_GD
                    + "&IV_ROOTED_GD=" + registro.DiagnosticGD.dynamicGD.IV_ROOTED_GD
                    + "&IV_CAM_FRONT=" + registro.DiagnosticGD.dynamicGD.IV_CAM_FRONT
                    + "&IV_CAM_REAR=" + registro.DiagnosticGD.dynamicGD.IV_CAM_REAR
                    + "&currPage="
                    + "&numPerPage="
                    + "&Serno="
                    + "&ASCCODE="
                    + "&TICKET_COMPANY=" + usuario.BUKRS
                    + "&SERVICE_COMPANY=" + usuario.BUKRS
                    + "&enterCreationDateTime="
                    + "&DIA_SESS_ID=" + registro.DiagnosticGD.guid.ToString()
                    + "&GD_RESULT_TYPE=TA"
                    + "&regionCode="
                    + "&cityFlag=N"
                    + "&postalFlag=N"
                    + "&D_NAME_FIRST=" + registro.Cliente.Nome
                    + "&D_NAME_LAST=" + registro.Cliente.Sobrenome
                    + "&CONSUMER=" + registro.Cliente.Id
                    + "&ADDRNUMBER=" + registro.Cliente.AddrNumber
                    + "&TEL=" + (registro.Cliente.Telefone.Count() == 10 ? registro.Cliente.Telefone : registro.Cliente.Celular)
                    + "&CUST_CONFIRM_DT=" + DateTime.Now.Date.ToString("dd.MM.yyyy")
                    + "&CUST_CONFIRM_TM=" + WebUtility.UrlEncode(DateTime.Now.ToString("HH:mm:ss"))
                    + "&MODEL=" + registro.Aparelho.Modelo
                    + "&VERSION="
                    + "&model_desc=" + registro.Aparelho.Model_Desc
                    + "&prod_category_desc="
                    + "&prod_category=" + registro.Aparelho.Prod_Category
                    + "&sub_category_desc="
                    + "&local_svc_prod_desc=" + registro.Aparelho.Local_Svc_Prod_Desc
                    + "&SVC_PRCD=" + registro.Aparelho.Local_Svc_Prod
                    + "&HQ_SVC_PRCD=" + registro.Aparelho.Hq_Svc_Prod
                    + "&SOLD_COUNTRY_CODE="
                    + "&SOLD_COUNTRY="
                    + "&SERIAL_NO= " + registro.Aparelho.RN
                    + "&IMEI=" + registro.Aparelho.Imei
                    + "&SERIAL_GMES=&"
                    + "SYMPTOM_CAT2=" + registro.Symptom_Code
                    + "&statusReason=" + registro.Reason
                    + "&attach_doc_type=ATT01");

                if (httpContent == null) new Exception("Erro ao setar registro");

                r = JsonConvert.DeserializeObject(httpContent.result);

                if (r.success == true)
                {
                    Console.WriteLine("------------------- REGISTRO SETADO -----------------");
                    return registro;
                }
                else
                {
                    new Exception("Erro ao setar registro");
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return null;
        }
    }
}