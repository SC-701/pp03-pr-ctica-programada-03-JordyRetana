using Abstracciones.Interfaces.Reglas;
using Abstracciones.Modelos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net;
using System.Text.Json;

namespace Web.Pages.Vehiculos
{
    public class AgregarModel : PageModel
    {
        private readonly IConfiguracion _configuracion;

        [BindProperty]
        public VehiculoRequest vehiculo { get; set; } = new VehiculoRequest();

        [BindProperty]
        public List<SelectListItem> marcas { get; set; } = new List<SelectListItem>();

        [BindProperty]
        public List<SelectListItem> modelos { get; set; } = new List<SelectListItem>();

        [BindProperty]
        public Guid marcaSeleccionada { get; set; }

        private static readonly JsonSerializerOptions _jsonOptions =
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        public AgregarModel(IConfiguracion configuracion)
        {
            _configuracion = configuracion;
        }

        public async Task<IActionResult> OnGet()
        {
            await ObtenerMarcasAsync();
            modelos = new List<SelectListItem>(); // vacío al inicio
            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            // 1) Normaliza placa (evita espacios/minúsculas)
            vehiculo.Placa = (vehiculo.Placa ?? "").Trim().ToUpperInvariant();

            // 2) Validación extra (muy importante)
            if (vehiculo.IdModelo == Guid.Empty)
            {
                ModelState.AddModelError("vehiculo.IdModelo", "Debe seleccionar un modelo.");
            }

            // 3) Si ModelState inválido, recarga combos y devuelve la página
            if (!ModelState.IsValid)
            {
                await ObtenerMarcasAsync();

                // Si el usuario escogió marca, recarga modelos
                if (marcaSeleccionada != Guid.Empty)
                {
                    var listaModelos = await ObtenerModelosAsync(marcaSeleccionada);
                    modelos = listaModelos.Select(m => new SelectListItem
                    {
                        Value = m.Id.ToString(),
                        Text = m.Nombre
                    }).ToList();
                }
                else
                {
                    modelos = new List<SelectListItem>();
                }

                return Page();
            }

            // 4) Llamada a API
            string endpoint = _configuracion.ObtenerMetodo("ApiEndPoints", "AgregarVehiculo");
            using var cliente = new HttpClient();

            var respuesta = await cliente.PostAsJsonAsync(endpoint, vehiculo);

            // 5) NO explotar: manejar 400 y mostrar el detalle
            if (!respuesta.IsSuccessStatusCode)
            {
                var detalle = await respuesta.Content.ReadAsStringAsync();

                // Mensaje visible en asp-validation-summary="All"
                ModelState.AddModelError(string.Empty,
                    $"Error API ({(int)respuesta.StatusCode}): {detalle}");

                // recargar combos para que la vista no se rompa
                await ObtenerMarcasAsync();
                if (marcaSeleccionada != Guid.Empty)
                {
                    var listaModelos = await ObtenerModelosAsync(marcaSeleccionada);
                    modelos = listaModelos.Select(m => new SelectListItem
                    {
                        Value = m.Id.ToString(),
                        Text = m.Nombre
                    }).ToList();
                }

                return Page();
            }

            return RedirectToPage("./Index");
        }

        private async Task ObtenerMarcasAsync()
        {
            string endpoint = _configuracion.ObtenerMetodo("ApiEndPoints", "ObtenerMarcas");
            using var cliente = new HttpClient();

            var respuesta = await cliente.GetAsync(endpoint);
            respuesta.EnsureSuccessStatusCode();

            var resultado = await respuesta.Content.ReadAsStringAsync();
            var resultadoDeserializado = JsonSerializer.Deserialize<List<Marca>>(resultado, _jsonOptions) ?? new List<Marca>();

            marcas = resultadoDeserializado.Select(a => new SelectListItem
            {
                Value = a.Id.ToString(),
                Text = a.Nombre
            }).ToList();
        }

        public async Task<JsonResult> OnGetObtenerModelos(Guid marcaId)
        {
            var lista = await ObtenerModelosAsync(marcaId);
            return new JsonResult(lista.Select(m => new { id = m.Id, nombre = m.Nombre }));
        }

        private async Task<List<Modelo>> ObtenerModelosAsync(Guid marcaId)
        {
            string endpoint = _configuracion.ObtenerMetodo("ApiEndPoints", "ObtenerModelos");
            using var cliente = new HttpClient();

            var url = string.Format(endpoint, marcaId);
            var respuesta = await cliente.GetAsync(url);
            respuesta.EnsureSuccessStatusCode();

            var resultado = await respuesta.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<Modelo>>(resultado, _jsonOptions) ?? new List<Modelo>();
        }
    }
}