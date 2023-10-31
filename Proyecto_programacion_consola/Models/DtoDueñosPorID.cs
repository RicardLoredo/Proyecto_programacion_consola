using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_programacion_consola.Models
{
    public class DtoDueñosPorID
    {
        public int IdDueño { get; set; }

        public string Nombre { get; set; }
        public string Apellido_paterno { get; set; }

        public string Apellido_materno { get; set; }

        public string Telefono { get; set; }

        public string Direccion { get; set; }
    }
}
