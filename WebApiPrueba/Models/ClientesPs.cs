using System;
using System.ComponentModel.DataAnnotations; // Necesario para [Key] y [StringLength]
using System.ComponentModel.DataAnnotations.Schema; // Necesario para [Column] y [DatabaseGenerated]

namespace WebApiPrueba.Models
{


    public class ClientesPs
    {
        /// <summary>
        /// Identificador único autoincremental del cliente.
        /// Es la clave primaria de la tabla.
        /// </summary>
        [Key] // Indica que esta propiedad es la clave primaria de la entidad.
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Especifica que la base de datos genera el valor (IDENTITY en SQL).
        public int Id { get; set; }

        /// <summary>
        /// Número de identificación único del cliente (ej. cédula, NIT).
        /// </summary>
        [Required] // Hace que este campo sea obligatorio.
        [StringLength(50)] // Limita la longitud máxima de la cadena a 50 caracteres, igual que NVARCHAR(50).
        public string NumId { get; set; } = string.Empty; // Se inicializa para evitar posibles nulls.

        /// <summary>
        /// Nombres del cliente.
        /// </summary>
        [Required] // Hace que este campo sea obligatorio.
        [StringLength(200)] // Limita la longitud máxima de la cadena a 200 caracteres.
        public string Nombres { get; set; } = string.Empty;

        /// <summary>
        /// Apellidos del cliente.
        /// </summary>
        [Required] // Hace que este campo sea obligatorio.
        [StringLength(200)] // Limita la longitud máxima de la cadena a 200 caracteres.
        public string Apellidos { get; set; } = string.Empty;

        /// <summary>
        /// Dirección de correo electrónico del cliente. Puede ser nulo.
        /// </summary>
        [StringLength(300)] // Limita la longitud máxima de la cadena a 300 caracteres.
        public string? Correo { get; set; } // 'string?' indica que esta propiedad puede ser nula.

        /// <summary>
        /// Fecha y hora en que se creó el registro del cliente.
        /// </summary>
        [Required] // Este campo es obligatorio.
        public DateTime FechaCreacion { get; set; } = DateTime.Now; // Se inicializa con la fecha y hora actual del servidor.

        /// <summary>
        /// Última fecha y hora en que se modificó el registro del cliente. Puede ser nulo.
        /// </summary>
        public DateTime? FechaModificacion { get; set; } // 'DateTime?' indica que esta propiedad puede ser nula.

        /// <summary>
        /// Indicador de eliminación lógica del registro (Soft Delete).
        /// True (1) para activo, False (0) para eliminado.
        /// </summary>
        [Required] // Este campo es obligatorio.
        public bool Estado { get; set; } = true; // Se inicializa como true (activo) por defecto.
    }
}
