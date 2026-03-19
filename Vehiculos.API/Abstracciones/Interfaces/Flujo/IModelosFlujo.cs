using Abstracciones.Modelos;
using System.Reflection;

namespace Abstracciones.Interfaces.Flujo
{
    public interface IModeloFlujo
    {
        Task<IEnumerable<Modelo>> Obtener(Guid IdMarca);
    }
}
