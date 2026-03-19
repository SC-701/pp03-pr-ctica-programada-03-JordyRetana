using Abstracciones.Modelos;
using System.Text.RegularExpressions;

namespace Abstracciones.Interfaces.DA
{
    public interface IMarcaDA
    {
        Task<IEnumerable<Marca>> Obtener();
    }
}
