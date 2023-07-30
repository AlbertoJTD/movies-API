using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using MoviesAPI.Controllers;
using MoviesAPI.DTOs;
using MoviesAPI.Entidades;
using MoviesAPI.Servicios;
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

		[TestMethod]
		public async Task CrearActorSinFoto()
		{
			// Preparacion
			var nombreBD = Guid.NewGuid().ToString();
			var contexto = ConstruirContext(nombreBD);
			var mapper = ConfigurarAutoMapper();

			var actor = new ActorCreacionDTO()
			{
				Nombre = "Alberth",
				FechaNacimiento = DateTime.Now
			};

			var mock = new Mock<IAlmacenadorArchivos>();
			mock.Setup(x => x.GuardarArchivo(null, null, null, null)).Returns(Task.FromResult("url"));

			// Prueba
			var controller = new ActoresController(contexto, mapper, mock.Object);
			var respuesta = await controller.Post(actor);

			// Verificacion
			var resultado = respuesta as CreatedAtRouteResult;
			Assert.AreEqual(201, resultado.StatusCode);

			var contexto2 = ConstruirContext(nombreBD);
			var listado = await contexto2.Actores.ToListAsync();
			Assert.AreEqual(1, listado.Count);
			Assert.IsNull(listado[0].Foto);

			Assert.AreEqual(0, mock.Invocations.Count);
		}

		[TestMethod]
		public async Task CrearActorConFoto()
		{
			// Preparacion
			var nombreBD = Guid.NewGuid().ToString();
			var contexto = ConstruirContext(nombreBD);
			var mapper = ConfigurarAutoMapper();

			var content = Encoding.UTF8.GetBytes("Imagen de prueba");
			var archivo = new FormFile(new MemoryStream(content), 0, content.Length, "Data", "imagen.jpg");
			archivo.Headers = new HeaderDictionary();
			archivo.ContentType = "image/jpg";

			var actor = new ActorCreacionDTO()
			{
				Nombre = "Alberth",
				FechaNacimiento = DateTime.Now,
				Foto = archivo
			};

			var mock = new Mock<IAlmacenadorArchivos>();
			mock.Setup(x => x.GuardarArchivo(content, ".jpg", "actores", archivo.ContentType)).Returns(Task.FromResult("url"));

			// Prueba
			var controller = new ActoresController(contexto, mapper, mock.Object);
			var respuesta = await controller.Post(actor);

			// Verificacion
			var resultado = respuesta as CreatedAtRouteResult;
			Assert.AreEqual(201, resultado.StatusCode);

			var contexto2 = ConstruirContext(nombreBD);
			var listado = await contexto2.Actores.ToListAsync();
			Assert.AreEqual(1, listado.Count);
			Assert.AreEqual("url", listado[0].Foto);
			Assert.AreEqual(1, mock.Invocations.Count);
		}
	}
}
