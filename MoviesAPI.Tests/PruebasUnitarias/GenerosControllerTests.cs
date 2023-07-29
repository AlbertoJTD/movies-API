using MoviesAPI.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesAPI.Tests.PruebasUnitarias
{
	[TestClass]
	public class GenerosControllerTests: BasePruebas
	{
		[TestMethod]
		public async Task ObtenerTodosLosGeneros()
		{
			// Preparacion
			var nombreBD = Guid.NewGuid().ToString();
			var contexto = ConstruirContext(nombreBD);
			var mapper = ConfigurarAutoMapper();

			contexto.Generos.Add(new Entidades.Genero() { Nombre = "Genero 1" });
			contexto.Generos.Add(new Entidades.Genero() { Nombre = "Genero 2" });
			await contexto.SaveChangesAsync();

			var contexto2 = ConstruirContext(nombreBD);

			// Prueba
			var controller = new GenerosController(contexto2, mapper);
			var respuesta = await controller.Get();

			// Verificacion
			var generos = respuesta.Value;

			Assert.AreEqual(2, generos.Count);
		}
	}
}
