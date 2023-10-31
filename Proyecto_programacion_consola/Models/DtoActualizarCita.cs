using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_programacion_consola.Models
{
    public class DtoActualizarCita
    {
        public int IdCitas { get; set; }

        public string Motivo_consulta { get; set; }

        public DateTime Fecha_consulta { get; set; }

        public string Hora { get; set; }

        public bool Activo { get; set; }

        public string Error { get; set; }
    }
}

