using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WindowsFormsApp1.Api;
using WindowsFormsApp1.Helpers;

namespace WindowsFormsApp1.Models
{
    public class RegistroData
    {
        public static Dictionary<String, String> Sintomas;
        public static Dictionary<String, String> Diagnosticos;
        public static Dictionary<String, Tuple<String, String, String>> SD;

        public static void SetValues()
        {
            SD = new Dictionary<String,Tuple<String, String, String>>(){
                {"1", new Tuple<String,String,String>("11","DTBU","Foi feito as primeiras configurações do aparelho")},
                {"2", new Tuple<String,String,String>("11","TOSR","Feito a transferência para um novo dispositivo")},
                {"3", new Tuple<String,String,String>("11","SMUP","Atualização de software")},
                {"4", new Tuple<String,String,String>("11","EDCU","Explicação sobre operação do aparelho")},
                {"5", new Tuple<String,String,String>("11","ISLO","Informado o local da assistência para solução do problema")}};

            Sintomas = new Dictionary<String,String>(){
                {"01", "Energia"},
                {"02", "Bateria"},
                {"03", "Carga"},
                {"04", "Som"},
                {"05", "Tela"},
                {"06", "Touch"},
                {"07", "Botão"},
                {"08", "Câmera"},
                {"09", "Ligação"},
                {"10", "Operação/Performance"},
                {"11", "Software"},
                {"12", "Conexão, Wi-Fi, BT, Infravermelho"},
                {"13", "Sensores"},
                {"14", "SMS"},
                {"15", "Vibração"},
                {"16","Outros"}};

            Diagnosticos = new Dictionary<String,String>(){
                {"ISLO", "Informação do local da assistência"},
                {"TOSR", "Transferência para abertura de OS"},
                {"SMUP", "Atualização de software (SMART)"},
                {"RREX", "Explicação sobre o reparo executado"},
                {"DTBU", "Transferência de dados/Backup"},
                {"FARE", "Reset ao modo de fábrica"},
                {"CWOR", "Outro motivo"},
                {"APIR", "Instalação/Remoção de aplicativo"},
                {"EDCU", "Explicação ao consumidor"}};
        }
    }
}