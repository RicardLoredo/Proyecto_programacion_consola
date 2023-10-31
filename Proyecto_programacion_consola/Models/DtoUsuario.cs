using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_programacion_consola.Models
{
    public class DtoUsuario
    {
        public string Usuario { get; set; }

        public string Contraseña { get; set; }

        public string Error { get; set; }

        public string Token { get; set; }
    }
}
