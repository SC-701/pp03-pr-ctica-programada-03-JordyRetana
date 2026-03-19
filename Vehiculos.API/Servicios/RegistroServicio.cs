using Abstracciones.Interfaces.Reglas;
using Abstracciones.Interfaces.Servicios;
using Abstracciones.Modelos;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Servicios
{
    public class RegistroServicio : IRegistroServicio
    {
        private readonly IConfiguracion _configuracion;
        private readonly IHttpClientFactory _httpClient;
        private readonly ILogger<RegistroServicio> _logger;

        public RegistroServicio(IConfiguracion configuracion, IHttpClientFactory httpClient, ILogger<RegistroServicio> logger)
        {
            _configuracion = configuracion;
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<Propietario?> Obtener(string placa)
        {
            try
            {
                var endpoint = _configuracion.ObtenerMetodo("ApiEndPointsRegistro", "ObtenerRegistro");

                var servicioRegistro = _httpClient.CreateClient("ServicioRegistro");
                var respuesta = await servicioRegistro.GetAsync(endpoint);
                respuesta.EnsureSuccessStatusCode();
                var json = await respuesta.Content.ReadAsStringAsync();
                var opciones = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var lista = JsonSerializer.Deserialize<List<Propietario>>(json, opciones) ?? new List<Propietario>();
                return lista.FirstOrDefault(x =>
                    !string.IsNullOrWhiteSpace(x.Placa) &&
                    string.Equals(x.Placa, placa, StringComparison.OrdinalIgnoreCase));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al consultar el registro");
                return null;
            }
        }
    }
}