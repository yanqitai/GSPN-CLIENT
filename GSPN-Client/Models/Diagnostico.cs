using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1.Models
{
    public class Diagnostico
    {
        public int consul { get; set; }
        public Guid guid { get; set; }
        public dynamic dynamicGD { get; set; }
        public string imei { get; set; }

        public override string ToString()
        {
            return "\nGD: " + consul;
        }
    }
}
