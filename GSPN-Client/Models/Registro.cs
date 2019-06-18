using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1.Models
{
    public class Registro
    {
        public Aparelho Aparelho { get; set; }
        public Cliente Cliente { get; set; }
        public Diagnostico DiagnosticGD {get; set;}

        public String Data { get; set; }
        public int Type { get; set; }
        public String Id { get; set; }

        public String Symptom_Code { get; set; }
        public String Reason { get; set; }
        public String MsgDiagnostico { get; set; }

        public Registro()
        {
            if (DiagnosticGD == null) DiagnosticGD = new Diagnostico();
        }

        public override string ToString()
        {
            return Cliente.ToString() + Aparelho.ToString() + DiagnosticGD + "\n" + "Diagnostico: " + MsgDiagnostico;
        }
    }
}
