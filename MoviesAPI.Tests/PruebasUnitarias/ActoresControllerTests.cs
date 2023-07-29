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
	public class ActoresControllerTests : BasePruebas
	{
		[TestMethod]
		public async Task ObtenerActoresPaginados()
		{
			// Preparacion
			var nombreBD = Guid.NewGuid().ToString();
			var contexto = ConstruirContext(nombreBD);
			var mapper = ConfigurarAutoMapper();

			contexto.Actores.Add(new Actor() { Nombre = "Actor 1" });
			contexto.Actores.Add(new Actor() { Nombre = "Actor 2" });
			contexto.Actores.Add(new Actor() { Nombre = "Actor 3" });
			await contexto.SaveChangesAsync();

			var contexto2 = ConstruirContext(nombreBD);

			// Prueba
			var controller = new ActoresController(contexto2, mapper, null);
			controller.ControllerContext.HttpContext = new DefaultHttpContext();
			var pagina1 = await controller.Get(new PaginacionDTO()
			{
				Pagina = 1,
				CantidadRegistrosPorPagina = 2
			});

			// Verificacion
			var actoresPagina1 = pagina1.Value;
			Assert.AreEqual(2, actoresPagina1.Count);


			controller.ControllerContext.HttpContext = new DefaultHttpContext();
			var pagina2 = await controller.Get(new PaginacionDTO()
			{
				Pagina = 2,
				CantidadRegistrosPorPagina = 2
			});
			var actoresPagina2 = pagina2.Value;
			Assert.AreEqual(1, actoresPagina2.Count);


			controller.ControllerContext.HttpContext = new DefaultHttpContext();
			var pagina3 = await controller.Get(new PaginacionDTO()
			{
				Pagina = 3,
				CantidadRegistrosPorPagina = 2
			});
			var actoresPagina3 = pagina3.Value;
			Assert.AreEqual(0, actoresPagina3.Count);
		}
	}
}
