using Abstracciones.Interfaces.DA;
using Abstracciones.Modelos;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace DA
{
    public class MarcaDA : IMarcaDA
    {
        private readonly IRepositorioDapper _repositorioDapper;

        public MarcaDA(IRepositorioDapper repositorioDapper)
        {
            _repositorioDapper = repositorioDapper;
        }

        #region Operaciones
        public async Task<IEnumerable<Marca>> Obtener()
        {
            const string sp = "ObtenerMarcas";

            using SqlConnection sql = _repositorioDapper.ObtenerRepositorio();

            var resultadoConsulta = await sql.QueryAsync<Marca>(
                sp,
                commandType: CommandType.StoredProcedure
            );

            return resultadoConsulta;
        }
        #endregion
    }
}