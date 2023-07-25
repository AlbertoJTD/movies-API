﻿using System.ComponentModel.DataAnnotations;

namespace MoviesAPI.DTOs
{
	public class GeneroDTO
	{
        public int Id { get; set; }

		[Required]
		[StringLength(50)]
		public string Nombre { get; set; }
    }
}
