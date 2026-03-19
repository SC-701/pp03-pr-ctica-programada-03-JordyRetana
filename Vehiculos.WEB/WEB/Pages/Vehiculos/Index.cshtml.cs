using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Abstracciones.Interfaces.Reglas;
using System.Net;
using Abstracciones.Modelos;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;

namespace Web.Pages.Vehiculos
{

    public class IndexModel : PageModel
    {
        private IConfiguracion _configuracion;
        public IList<VehiculoResponse> vehiculos { get; set; } = default!;
        public IndexModel(IConfiguracion configuracion)
        {
            _configuracion = configuracion;
        }

        public async Task OnGet()
        {
            string endpoint = _configuracion.ObtenerMetodo("ApiEndPoints", "ObtenerVehiculos");
            var cliente = new HttpClient();
            var solicitud = new HttpRequestMessage(HttpMethod.Get, endpoint);

            var respuesta = await cliente.SendAsync(solicitud);
            respuesta.EnsureSuccessStatusCode();
            if (respuesta.StatusCode == HttpStatusCode.OK)
            {
                var resultado = await respuesta.Content.ReadAsStringAsync();
                var opciones = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                vehiculos = JsonSerializer.Deserialize<List<VehiculoResponse>>(resultado, opciones);
            }
        }
    }
}
