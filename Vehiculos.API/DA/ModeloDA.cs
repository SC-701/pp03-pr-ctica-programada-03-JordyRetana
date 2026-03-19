using Abstracciones.Interfaces.DA;
using Abstracciones.Modelos;
using Microsoft.Data.SqlClient;
using Dapper;

namespace DA
{
    public class ModeloDA : IModeloDA
    {
        private IRepositorioDapper _repositorioDapper;
        private SqlConnection _sqlConnection;


        public ModeloDA(IRepositorioDapper repositorioDapper)
        {
            _repositorioDapper = repositorioDapper;
            _sqlConnection = _repositorioDapper.ObtenerRepositorio();
        }

        #region Operaciones

        public async Task<IEnumerable<Modelo>> Obtener(Guid IdMarca)
        {
            string query = @"ObtenerModelos";
            var resultadoConsulta = await _sqlConnection.QueryAsync<Modelo>(query,
                new { IdMarca = IdMarca });
            return resultadoConsulta;
        }
        #endregion

    }
}