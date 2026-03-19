using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abstracciones.Modelos
{
    public class Revision
    {
        public string Placa { get; set; }
        public IEnumerable<Falta> Faltas { get; set; }
        public string Periodo { get; set; }
        public string Resultado { get; set; }
    }
}
