using Abstracciones.Interfaces.Reglas;
using Abstracciones.Modelos;
using Microsoft.Extensions.Configuration;
using System.Net;

namespace Reglas
{
    public class Configuracion : IConfiguracion
    {
        private IConfiguration _configuracion;

        public Configuracion(IConfiguration configuracion)
        {
            _configuracion = configuracion;
        }

        public string ObtenerMetodo(string seccion, string nombre)
        {
            var UrlBase = ObtenerUrlBase(seccion);
            var Metodo = _configuracion.GetSection(seccion).Get<APIEndPoint>().Metodos.Where(m => m.Nombre == nombre).FirstOrDefault().Valor;
            return $"{UrlBase}{Metodo}";
        }

        public string ObtenerValor(string llave)
        {
            return _configuracion.GetSection(llave).Value;
        }

        private string ObtenerUrlBase(string seccion)
        {
            return _configuracion.GetSection(seccion).Get<APIEndPoint>().UrlBase;
        }
    }
}
