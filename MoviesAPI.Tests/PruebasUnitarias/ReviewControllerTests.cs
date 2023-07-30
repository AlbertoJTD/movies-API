using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesAPI.Tests.PruebasUnitarias
{
	[TestClass]
	public class ReviewControllerTests: BasePruebas
	{
		private void CrearPeliculas(string nombreDB)
		{
			var contexto = ConstruirContext(nombreDB);
			contexto.Peliculas.Add(new Pelicula() { Titulo = "pelicula 1" });
			contexto.SaveChanges();
		}
	}
}
