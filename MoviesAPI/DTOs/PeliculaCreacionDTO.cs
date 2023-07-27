﻿using MoviesAPI.Validaciones;
using System.ComponentModel.DataAnnotations;

namespace MoviesAPI.DTOs
{
	public class PeliculaCreacionDTO : PeliculaPatchDTO
	{
		[PesoArchivoValidacion(PesoMaximoEnMegaBytes: 4)]
		[TipoArchivoValidacion(grupoTipoArchivo: GrupoTipoArchivo.Imagen)]
		public IFormFile Poster { get; set; }
	}
}
