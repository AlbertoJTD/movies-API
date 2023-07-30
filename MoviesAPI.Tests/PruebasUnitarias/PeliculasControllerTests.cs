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
	}
}
