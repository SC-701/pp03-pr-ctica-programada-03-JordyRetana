using Abstracciones.Interfaces.Reglas;
using Abstracciones.Modelos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net;
using System.Text.Json;

namespace Web.Pages.Vehiculos
{

    public class EditarModel : PageModel
    {
        private IConfiguracion _configuracion;
        [BindProperty]
        public VehiculoResponse vehiculo { get; set; } = default!;
        public VehiculoRequest vehiculoRequest { get; set; } = default!;
        [BindProperty]
        public List<SelectListItem> marcas { get; set; } = default!;
        [BindProperty]
        public List<SelectListItem> modelos { get; set; } = default!;
        [BindProperty]
        public Guid marcaSeleccionada { get; set; } = default!;
        [BindProperty]
        public Guid modeloSeleccionado { get; set; } = default!;
        public EditarModel(IConfiguracion configuracion)
        {
            _configuracion = configuracion;
        }
        public async Task<ActionResult> OnGet(Guid? id)
        {
            if (id == null)
                return NotFound();
            string endpoint = _configuracion.ObtenerMetodo("ApiEndPoints", "ObtenerVehiculo");
            var cliente = new HttpClient();

            var solicitud = new HttpRequestMessage(HttpMethod.Get, string.Format(endpoint, id));
            var respuesta = await cliente.SendAsync(solicitud);
            respuesta.EnsureSuccessStatusCode();
            if (respuesta.StatusCode == HttpStatusCode.OK)
            {
                await ObtenerMarcasAsync();
                var resultado = await respuesta.Content.ReadAsStringAsync();
                var opciones = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                vehiculo = JsonSerializer.Deserialize<VehiculoResponse>(resultado, opciones);
                if (vehiculo != null)
                {
                    marcaSeleccionada = Guid.Parse(marcas.Where(m => m.Text == vehiculo.Marca).FirstOrDefault().Value);
                    modelos = (await ObtenerModelosAsync(marcaSeleccionada)).Select(a =>
                        new SelectListItem
                        {
                            Value = a.Id.ToString(),
                            Text = a.Nombre.ToString(),
                            Selected = a.Nombre == vehiculo.Modelo
                        }).ToList();
                    modeloSeleccionado = Guid.Parse(modelos.Where(m => m.Text == vehiculo.Modelo).FirstOrDefault().Value);
                }

            }
            return Page();
        }
        public async Task<ActionResult> OnPost()
        {
            if (vehiculo.Id == Guid.Empty)
                return NotFound();

            if (!ModelState.IsValid)
                return Page();

            string endpoint = _configuracion.ObtenerMetodo("ApiEndPoints", "EditarVehiculo");
            var cliente = new HttpClient();

            var respuesta = await cliente.PutAsJsonAsync<VehiculoRequest>(string.Format(endpoint, vehiculo.Id.ToString()), new VehiculoRequest { IdModelo = modeloSeleccionado, Anio = vehiculo.Anio, Color = vehiculo.Color, CorreoPropietario = vehiculo.CorreoPropietario, Placa = vehiculo.Placa, Precio = vehiculo.Precio, TelefonoPropietario = vehiculo.TelefonoPropietario });
            respuesta.EnsureSuccessStatusCode();
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
                var resultadoDeserializado = JsonSerializer.Deserialize<List<Marca>>(resultado, opciones);
                marcas = resultadoDeserializado.Select(a =>
                                  new SelectListItem
                                  {
                                      Value = a.Id.ToString(),
                                      Text = a.Nombre.ToString()
                                  }).ToList();
            }
        }
        public async Task<JsonResult> OnGetObtenerModelos(Guid marcaId)
        {
            var modelos = await ObtenerModelosAsync(marcaId);
            return new JsonResult(modelos);
        }

        private async Task<List<Modelo>> ObtenerModelosAsync(Guid marcaId)
        {
            string endpoint = _configuracion.ObtenerMetodo("ApiEndPoints", "ObtenerModelos");
            var cliente = new HttpClient();
            var solicitud = new HttpRequestMessage(HttpMethod.Get, string.Format(endpoint, marcaId));

            var respuesta = await cliente.SendAsync(solicitud);
            respuesta.EnsureSuccessStatusCode();
            if (respuesta.StatusCode == HttpStatusCode.OK)
            {
                var resultado = await respuesta.Content.ReadAsStringAsync();
                var opciones = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                return JsonSerializer.Deserialize<List<Modelo>>(resultado, opciones);
            }
            return new List<Modelo>();
        }
    }
}
