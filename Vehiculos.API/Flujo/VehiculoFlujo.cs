using Abstracciones.Interfaces.DA;
using Abstracciones.Interfaces.Flujo;
using Abstracciones.Interfaces.Reglas;
using Abstracciones.Modelos;

namespace Flujo
{
    public class VehiculoFlujo : IVehiculoFlujo
    {
        private IVehiculoDA _vehiculoDA;
        private IRegistroReglas _registroReglas;
        private IRevisionReglas _revisionReglas;

        public VehiculoFlujo(IVehiculoDA vehiculoDA, IRevisionReglas revisionReglas, IRegistroReglas registroReglas)
        {
            _vehiculoDA = vehiculoDA;
            _revisionReglas = revisionReglas;
            _registroReglas = registroReglas;
        }

        public async Task<Guid> Agregar(VehiculoRequest vehiculo)
        {
            return await _vehiculoDA.Agregar(vehiculo);
        }

        public async Task<Guid> Editar(Guid Id, VehiculoRequest vehiculo)
        {
            return await _vehiculoDA.Editar(Id, vehiculo);
        }

        public async Task<Guid> Eliminar(Guid Id)
        {
            return await _vehiculoDA.Eliminar(Id);
        }

        public async Task<IEnumerable<VehiculoResponse>> Obtener()
        {
            return await _vehiculoDA.Obtener();
        }

        public async Task<VehiculoDetalle> Obtener(Guid Id)
        {
            var vehiculo = await _vehiculoDA.Obtener(Id);
            vehiculo.RevisionValida = await _revisionReglas.RevisionEsValida(vehiculo.Placa);
            vehiculo.RegistroValido = await _registroReglas.VehiculoEstaRegistrado(vehiculo.Placa, vehiculo.CorreoPropietario);
            return vehiculo;
        }
    }
}
