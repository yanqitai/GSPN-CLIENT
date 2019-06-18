using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1.Models
{
    public class Aparelho
    {
        public String Modelo { get; set; }
        public String RN { get; set; }
        public String Imei { get; set; }
        public String Model_Desc { get; set; }
        public String Local_Svc_Prod_Desc { get; set; }
        public String Local_Svc_Prod { get; set; }
        public String Hq_Svc_Prod { get; set; }
        public String Prod_Category { get; set; }

        public override string ToString()
        {
            var a  = "\nAparelho: " + Modelo + " | " +
            RN + " | " +
            Imei;

            return "\nModelo: " + Modelo + 
                "\nimei: | " + Imei +
                "\nNúmero de Série | " + RN;
        }
    }
}
