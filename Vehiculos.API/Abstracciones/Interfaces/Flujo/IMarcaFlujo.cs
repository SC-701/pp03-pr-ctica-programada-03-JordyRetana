using Abstracciones.Modelos;
using System.Text.RegularExpressions;

namespace Abstracciones.Interfaces.Flujo
{
    public interface IMarcaFlujo
    {
        Task<IEnumerable<Marca>> Obtener();
    }
}
