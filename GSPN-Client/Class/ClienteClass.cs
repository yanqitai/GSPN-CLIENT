using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsFormsApp1.Models;
using WindowsFormsApp1.Services;
using WindowsFormsApp1.Views;

namespace WindowsFormsApp1.Class
{
    public class ClienteClass
    {
        GSPNApi WebApi;

        public ClienteClass()
        {
            WebApi = Main.ApiGSPN;
        }

        public async Task<Cliente> SearchCliente(String cpf)
        {
            Cliente c = await WebApi.GetClienteByCPF(cpf);
            if (c == null) return null;
            
            return c;
        }

        public static Cliente ValidaCliente(Cliente cliente, Cliente c)
        {
            cliente.Telefone = c.Telefone == null ? (cliente.Telefone.Length == 11 ? cliente.Telefone : null) : c.Telefone;
            cliente.Celular = c.Celular == null ? (cliente.Celular.Length == 10 ? cliente.Celular : null) : c.Celular;
            cliente.Sexo = cliente.Sexo != "M" || cliente.Sexo != "F" ? (c.Sexo != "M" || c.Sexo != "F" ? "M" : c.Sexo) : cliente.Sexo;
            cliente.Contatar = cliente.Contatar == 3 ? c.Contatar : cliente.Contatar;

            return cliente;
        }
    }
}
