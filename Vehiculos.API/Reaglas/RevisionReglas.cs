using Abstracciones.Interfaces.Reglas;
using Abstracciones.Interfaces.Servicios;
using Abstracciones.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reglas
{
    public class RevisionReglas : IRevisionReglas
    {
        private readonly IRevisionServicio _revisionServicio;
        private readonly IConfiguracion _configuracion;

        public RevisionReglas(IRevisionServicio revisionServicio, IConfiguracion configuracion)
        {
            this._revisionServicio = revisionServicio;
            _configuracion = configuracion;
        }

        public async Task<bool> RevisionEsValida(string placa)
        {
            var resultadoRevision = await _revisionServicio.Obtener(placa);

            if (resultadoRevision != null && resultadoRevision != null && ValidarEstado(resultadoRevision) && ValidarPeriodo(resultadoRevision))
                return true;
            return false;
        }
        private bool ValidarEstado(Revision resultadoRevision)
        {
            string estadoRevisionValida = _configuracion.ObtenerValor("EstadoRevisionSatisfactorio");
            return resultadoRevision.Resultado == estadoRevisionValida;
        }

        private static bool ValidarPeriodo(Revision resultadoRevision)
        {
            string periodoactual = ObtenerPeriodoActual();
            return resultadoRevision.Periodo == periodoactual;
        }

        private static string ObtenerPeriodoActual()
        {
            var periodoactual = $"{DateTime.Now.Month}-{DateTime.Now.Year}";
            return periodoactual;
        }
    }
}
