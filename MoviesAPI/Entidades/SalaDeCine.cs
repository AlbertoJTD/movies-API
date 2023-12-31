﻿using NetTopologySuite.Geometries;
using System.ComponentModel.DataAnnotations;

namespace MoviesAPI.Entidades
{
	public class SalaDeCine: IId
	{
        public int Id { get; set; }

        [Required]
        [StringLength(120)]
        public string Nombre { get; set; }

        public List<PeliculasSalasDeCine> PeliculasSalasDeCine { get; set; }

        public Point Ubicacion { get; set; }
    }
}
