using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1.Models
{
    public class Cliente
    {
        public String Nome { get; set; }
        public String Sobrenome { get; set; }
        public String Telefone { get; set; }
        public String Celular { get; set; }
        public String Sexo { get; set; }
        public String Cpf { get; set; }
        public String Id { get; set; }
        public String AddrNumber { get; set; }
        public int Contatar { get; set; }

        public override string ToString()
        {
            return "\nCliente: " 
                + this.Nome + " | " 
                + this.Cpf + " | "
                + (this.Telefone != null ? this.Telefone + " | " : "")
                + (this.Celular != null ? this.Celular + " | " : "");

            var fieldValues = this.GetType().GetProperties().Select(x => x.Name).ToList();

            List<String> a = fieldValues.Select(m => m + " : " + this.GetType().GetProperty(m).GetValue(this, null)).ToList();

            return base.ToString();
        }
    }
}
