using Abstracciones.Modelos;
using System.Reflection;

namespace Abstracciones.Interfaces.DA
{
    public interface IModeloDA
    {
        Task<IEnumerable<Modelo>> Obtener(Guid IdMarca);
    }
}
