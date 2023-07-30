using Microsoft.AspNetCore.Http;
using MoviesAPI.Controllers;
using MoviesAPI.DTOs;
using MoviesAPI.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesAPI.Tests.PruebasUnitarias
{
	[TestClass]
	public class PeliculasControllerTests: BasePruebas
	{
		[TestMethod]
		private string CrearDataPrueba()
		{
			var databaseName = Guid.NewGuid().ToString();
			var context = ConstruirContext(databaseName);
			var genero = new Genero() { Nombre = "genre 1" };

			var peliculas = new List<Pelicula>()
			{
				new Pelicula(){Titulo = "Pelicula 1", FechaEstreno = new DateTime(2024, 1,1), EnCines = false},
				new Pelicula(){Titulo = "No estrenada", FechaEstreno = DateTime.Today.AddDays(1), EnCines = false},
				new Pelicula(){Titulo = "Pelicula en Cines", FechaEstreno = DateTime.Today.AddDays(-1), EnCines = true}
			};

			var peliculaConGenero = new Pelicula()
			{
				Titulo = "Pelicula con Genero",
				FechaEstreno = new DateTime(2010, 1, 1),
				EnCines = false
			};
			peliculas.Add(peliculaConGenero);

			context.Add(genero);
			context.AddRange(peliculas);
			context.SaveChanges();

			var peliculaGenero = new PeliculasGeneros() { GeneroId = genero.Id, PeliculaId = peliculaConGenero.Id };
			context.Add(peliculaGenero);
			context.SaveChanges();

			return databaseName;
		}

		[TestMethod]
		public async Task FiltrarPorTitulo()
		{
			// Preparacion
			var nombreBD = CrearDataPrueba();
			var mapper = ConfigurarAutoMapper();
			var contexto = ConstruirContext(nombreBD);

			// Prueba
			var controller = new PeliculasController(contexto, mapper, null, null);
			controller.ControllerContext.HttpContext = new DefaultHttpContext();

			var tituloPelicula = "Pelicula 1";

			var filtroDTO = new FiltroPeliculaDTO()
			{
				Titulo = tituloPelicula,
				CantidadRegistrosPorPagina = 10
			};

			// Verificacion
			var respuesta = await controller.Filtrar(filtroDTO);
			var peliculas = respuesta.Value;
			Assert.AreEqual(1, peliculas.Count);
			Assert.AreEqual(tituloPelicula, peliculas[0].Titulo);
		}
	}
}
