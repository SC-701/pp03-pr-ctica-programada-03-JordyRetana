using Abstracciones.Interfaces.API;
using Abstracciones.Interfaces.Flujo;
using Abstracciones.Modelos;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MarcaController : ControllerBase, IMarcaController
    {
        private IMarcaFlujo _marcaFlujo;
        private ILogger<MarcaController> _logger;

        public MarcaController(IMarcaFlujo marcaFLujo, ILogger<MarcaController> logger)
        {
            _marcaFlujo = marcaFLujo;
            _logger = logger;
        }
        #region Operaciones
        [HttpGet]
        public async Task<IActionResult> Obtener()
        {
            var resultado = await _marcaFlujo.Obtener();
            if (!resultado.Any())
                return NoContent();
            return Ok(resultado);
        }
        #endregion Operaciones

    }
}
