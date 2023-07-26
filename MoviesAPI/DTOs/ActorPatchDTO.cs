using System.ComponentModel.DataAnnotations;

namespace MoviesAPI.DTOs
{
	public class ActorPatchDTO
	{
		[Required]
		[StringLength(120)]
		public string Nombre { get; set; }

		public DateTime FechaNacimiento { get; set; }
	}
}
