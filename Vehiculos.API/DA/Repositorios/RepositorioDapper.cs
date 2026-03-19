using Abstracciones.Interfaces.DA;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace DA.Repositorios
{
    public class RepositorioDapper : IRepositorioDapper
    {
        private readonly IConfiguration _configuracion;

        public RepositorioDapper(IConfiguration configuracion)
        {
            _configuracion = configuracion;
        }

        public SqlConnection ObtenerRepositorio()
        {
            return new SqlConnection(_configuracion.GetConnectionString("BD"));
        }
    }
}