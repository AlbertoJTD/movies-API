using System.ComponentModel.DataAnnotations;

namespace MoviesAPI.DTOs
{
	public class SalaDeCineCreacionDTO
	{
		[Required]
		[StringLength(120)]
		public string Nombre { get; set; }
    }
}
