﻿using System.ComponentModel.DataAnnotations;

namespace MoviesAPI.Entidades
{
	public class Genero
	{
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Nombre { get; set; }

		public List<PeliculasGeneros> PeliculasGeneros { get; set; }
	}
}
