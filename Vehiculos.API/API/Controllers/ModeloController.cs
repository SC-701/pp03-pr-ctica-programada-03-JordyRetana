using Abstracciones.Interfaces.API;
using Abstracciones.Interfaces.Flujo;
using Abstracciones.Modelos;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModeloController : ControllerBase, IModeloController
    {
        private IModeloFlujo _modeloFlujo;
        private ILogger<ModeloController> _logger;

        public ModeloController(IModeloFlujo modeloFLujo, ILogger<ModeloController> logger)
        {
            _modeloFlujo = modeloFLujo;
            _logger = logger;
        }
        #region Operaciones
        [HttpGet("{IdMarca}")]
        public async Task<IActionResult> Obtener(Guid IdMarca)
        {
            var resultado = await _modeloFlujo.Obtener(IdMarca);
            if (!resultado.Any())
                return NoContent();
            return Ok(resultado);
        }
        #endregion Operaciones

    }
}
