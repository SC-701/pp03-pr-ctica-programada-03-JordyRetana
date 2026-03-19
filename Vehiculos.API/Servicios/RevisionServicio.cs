using Abstracciones.Interfaces.Reglas;
using Abstracciones.Interfaces.Servicios;
using Abstracciones.Modelos;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Servicios
{
    public class RevisionServicio : IRevisionServicio
    {
        private readonly IConfiguracion _configuracion;
        private readonly IHttpClientFactory _httpClient;
        private readonly ILogger<RevisionServicio> _logger;

        public RevisionServicio(IConfiguracion configuracion, IHttpClientFactory httpClient, ILogger<RevisionServicio> logger)
        {
            _configuracion = configuracion;
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<Revision?> Obtener(string placa)
        {
            try
            {
                var endpoint = _configuracion.ObtenerMetodo("ApiEndPointsRevision", "ObtenerRevision");
                var servicioRevision = _httpClient.CreateClient("ServicioRevision");
                var respuesta = await servicioRevision.GetAsync(endpoint);
                respuesta.EnsureSuccessStatusCode();
                var json = await respuesta.Content.ReadAsStringAsync();
                var opciones = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var lista = JsonSerializer.Deserialize<List<Revision>>(json, opciones) ?? new List<Revision>();
                return lista.FirstOrDefault(x =>
                    !string.IsNullOrWhiteSpace(x.Placa) &&
                    string.Equals(x.Placa, placa, StringComparison.OrdinalIgnoreCase));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al consultar la revision");
                return null;
            }
        }
    }
}