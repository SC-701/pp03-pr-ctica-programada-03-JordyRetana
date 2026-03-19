using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Abstracciones.Modelos
{
    public class VehiculoBase
    {
        [RegularExpression(@"^(\d{3}-[A-Za-z]{3}|[A-Za-z]{3}-\d{3})$",
        ErrorMessage = "El formato de la placa debe ser ###-ABC o ABC-### (Ej: 123-ABC o ABC-123)")]
        public string Placa { get; set; } = string.Empty;

        [Required(ErrorMessage = "La propiedad color es requerida")]
        [StringLength(40, ErrorMessage = "La propiedad color debe ser mayor o igual a 4 caracteres y menor o igual a 40", MinimumLength = 4)]
        [DisplayName("Color")]
        public string Color { get; set; } = string.Empty;

        [Required(ErrorMessage = "La propiedad año es requerida")]
        [Range(1900, 2100, ErrorMessage = "El año debe estar entre 1900 y 2100")]
        [DisplayName("Año")]
        public int Anio { get; set; }

        [Required(ErrorMessage = "La propiedad precio es requerida")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0")]
        [DisplayName("Precio")]
        public decimal Precio { get; set; }

        [Required(ErrorMessage = "La propiedad correo es requerida")]
        [EmailAddress(ErrorMessage = "Ingrese un correo válido")]
        [DisplayName("Correo del propietario")]
        public string CorreoPropietario { get; set; } = string.Empty;

        [Required(ErrorMessage = "La propiedad telefono es requerida")]
        [Phone(ErrorMessage = "Ingrese un teléfono válido")]
        [DisplayName("Teléfono del propietario")]
        public string TelefonoPropietario { get; set; } = string.Empty;
    }

    public class VehiculoRequest : VehiculoBase
    {
        [Required(ErrorMessage = "El modelo es requerido")]
        public Guid IdModelo { get; set; }
    }

    public class VehiculoResponse : VehiculoBase
    {
        public Guid Id { get; set; }

        public string? Modelo { get; set; }
        public string? Marca { get; set; }
    }

    public class VehiculoDetalle : VehiculoResponse
    {
        public bool RevisionValida { get; set; }
        public bool RegistroValido { get; set; }
    }
}