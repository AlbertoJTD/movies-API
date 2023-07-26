﻿using System.ComponentModel.DataAnnotations;

namespace MoviesAPI.DTOs
{
	public class ActorCreacionDTO
	{
		[Required]
		[StringLength(120)]
		public string Nombre { get; set; }

		public DateTime FechaNacimiento { get; set; }

        public IFormFile Foto { get; set; }
    }
}
