using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1.Models
{
    public class Usuario
    {
        public String BUKRS { get; set; }
        public String PROCESS_TYPE { get; set; }
        public String LANGU { get; set; }
        public String SYMPTOM1_CODE { get; set; }
        public String IV_JOBFLAG { get; set; }
        public String GD_ASC_CODE { get; set; }
        public String STATUS { get; set; }
        public String IV_GSPN_ID { get; set; }
        public String IV_COMPANY { get; set; }
        public int Contatar { get; set; }

        public String IdAccess;

        public Usuario(String id)
        {
            IdAccess = id;
        }

        public Usuario()
        {

        }
    }
}
