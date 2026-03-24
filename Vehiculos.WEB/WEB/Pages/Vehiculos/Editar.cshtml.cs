using Abstracciones.Interfaces.Reglas;
using Abstracciones.Modelos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace Web.Pages.Vehiculos
{
    public class EditarModel : PageModel
    {
        private readonly IConfiguracion _configuracion;

        [BindProperty]
        public VehiculoResponse vehiculo { get; set; } = default!;

        public VehiculoRequest vehiculoRequest { get; set; } = default!;

        [BindProperty]
        public List<SelectListItem> marcas { get; set; } = new();

        [BindProperty]
        public List<SelectListItem> modelos { get; set; } = new();

        [BindProperty]
        public Guid marcaSeleccionada { get; set; }

        [BindProperty]
        public Guid modeloSeleccionado { get; set; }

        public EditarModel(IConfiguracion configuracion)
        {
            _configuracion = configuracion;
        }

        public async Task<ActionResult> OnGet(Guid? id)
        {
            if (id == null || id == Guid.Empty)
                return NotFound();

            string endpoint = _configuracion.ObtenerMetodo("ApiEndPoints", "ObtenerVehiculo");
            var cliente = new HttpClient();

            var solicitud = new HttpRequestMessage(HttpMethod.Get, string.Format(endpoint, id));
            var respuesta = await cliente.SendAsync(solicitud);

            if (!respuesta.IsSuccessStatusCode)
                return NotFound();

            await ObtenerMarcasAsync();

            var resultado = await respuesta.Content.ReadAsStringAsync();
            var opciones = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            vehiculo = JsonSerializer.Deserialize<VehiculoResponse>(resultado, opciones)!;

            if (vehiculo == null)
                return NotFound();

            var marcaActual = marcas.FirstOrDefault(m => m.Text == vehiculo.Marca);
            if (marcaActual != null && Guid.TryParse(marcaActual.Value, out Guid marcaId))
            {
                marcaSeleccionada = marcaId;
            }

            var listaModelos = await ObtenerModelosAsync(marcaSeleccionada);
            modelos = listaModelos.Select(a => new SelectListItem
            {
                Value = a.Id.ToString(),
                Text = a.Nombre,
                Selected = a.Nombre == vehiculo.Modelo
            }).ToList();

            var modeloActual = modelos.FirstOrDefault(m => m.Text == vehiculo.Modelo);
            if (modeloActual != null && Guid.TryParse(modeloActual.Value, out Guid modeloId))
            {
                modeloSeleccionado = modeloId;
            }

            return Page();
        }

        public async Task<ActionResult> OnPost()
        {
            if (vehiculo == null || vehiculo.Id == Guid.Empty)
                return NotFound();

            await ObtenerMarcasAsync();

            if (marcaSeleccionada != Guid.Empty)
            {
                var listaModelos = await ObtenerModelosAsync(marcaSeleccionada);
                modelos = listaModelos.Select(a => new SelectListItem
                {
                    Value = a.Id.ToString(),
                    Text = a.Nombre,
                    Selected = a.Id == modeloSeleccionado
                }).ToList();
            }
            else
            {
                modelos = new List<SelectListItem>();
            }

            if (modeloSeleccionado == Guid.Empty)
            {
                ModelState.AddModelError(string.Empty, "Debe seleccionar un modelo válido.");
                return Page();
            }

            if (!ModelState.IsValid)
                return Page();

            string endpoint = _configuracion.ObtenerMetodo("ApiEndPoints", "EditarVehiculo");
            var cliente = new HttpClient();

            var request = new VehiculoRequest
            {
                IdModelo = modeloSeleccionado,
                Anio = vehiculo.Anio,
                Color = vehiculo.Color,
                CorreoPropietario = vehiculo.CorreoPropietario,
                Placa = vehiculo.Placa,
                Precio = vehiculo.Precio,
                TelefonoPropietario = vehiculo.TelefonoPropietario
            };

            var respuesta = await cliente.PutAsJsonAsync(
                string.Format(endpoint, vehiculo.Id.ToString()),
                request
            );

            if (!respuesta.IsSuccessStatusCode)
            {
                var contenidoError = await respuesta.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, $"Error al actualizar el vehículo: {contenidoError}");
                return Page();
            }

            return RedirectToPage("./Index");
        }

        private async Task ObtenerMarcasAsync()
        {
            string endpoint = _configuracion.ObtenerMetodo("ApiEndPoints", "ObtenerMarcas");
            var cliente = new HttpClient();
            var solicitud = new HttpRequestMessage(HttpMethod.Get, endpoint);

            var respuesta = await cliente.SendAsync(solicitud);
            respuesta.EnsureSuccessStatusCode();

            if (respuesta.StatusCode == HttpStatusCode.OK)
            {
                var resultado = await respuesta.Content.ReadAsStringAsync();
                var opciones = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var resultadoDeserializado = JsonSerializer.Deserialize<List<Marca>>(resultado, opciones) ?? new List<Marca>();

                marcas = resultadoDeserializado.Select(a => new SelectListItem
                {
                    Value = a.Id.ToString(),
                    Text = a.Nombre,
                    Selected = a.Id == marcaSeleccionada
                }).ToList();
            }
        }

        public async Task<JsonResult> OnGetObtenerModelos(Guid marcaId)
        {
            var listaModelos = await ObtenerModelosAsync(marcaId);
            return new JsonResult(listaModelos);
        }

        private async Task<List<Modelo>> ObtenerModelosAsync(Guid marcaId)
        {
            if (marcaId == Guid.Empty)
                return new List<Modelo>();

            string endpoint = _configuracion.ObtenerMetodo("ApiEndPoints", "ObtenerModelos");
            var cliente = new HttpClient();
            var solicitud = new HttpRequestMessage(HttpMethod.Get, string.Format(endpoint, marcaId));

            var respuesta = await cliente.SendAsync(solicitud);
            respuesta.EnsureSuccessStatusCode();

            if (respuesta.StatusCode == HttpStatusCode.OK)
            {
                var resultado = await respuesta.Content.ReadAsStringAsync();
                var opciones = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                return JsonSerializer.Deserialize<List<Modelo>>(resultado, opciones) ?? new List<Modelo>();
            }

            return new List<Modelo>();
        }
    }
}