using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
	public class GenerosControllerTests: BasePruebas
	{
		[TestMethod]
		public async Task ObtenerTodosLosGeneros()
		{
			// Preparacion
			var nombreBD = Guid.NewGuid().ToString();
			var contexto = ConstruirContext(nombreBD);
			var mapper = ConfigurarAutoMapper();

			contexto.Generos.Add(new Genero() { Nombre = "Genero 1" });
			contexto.Generos.Add(new Genero() { Nombre = "Genero 2" });
			await contexto.SaveChangesAsync();

			var contexto2 = ConstruirContext(nombreBD);

			// Prueba
			var controller = new GenerosController(contexto2, mapper);
			var respuesta = await controller.Get();

			// Verificacion
			var generos = respuesta.Value;

			Assert.AreEqual(2, generos.Count);
		}

		[TestMethod]
		public async Task ObtenerGeneroPorIdNoExistente()
		{
			// Preparacion
			var nombreBD = Guid.NewGuid().ToString();
			var contexto = ConstruirContext(nombreBD);
			var mapper = ConfigurarAutoMapper();

			// Prueba
			var controller = new GenerosController(contexto, mapper);
			var respuesta = await controller.Get(1);

			// Verificacion
			var resultado = respuesta.Result as StatusCodeResult;
			Assert.AreEqual(404, resultado.StatusCode);
		}

		[TestMethod]
		public async Task ObtenerGeneroPorId()
		{
			// Preparacion
			var nombreBD = Guid.NewGuid().ToString();
			var contexto = ConstruirContext(nombreBD);
			var mapper = ConfigurarAutoMapper();

			contexto.Generos.Add(new Genero() { Nombre = "Genero 1" });
			contexto.Generos.Add(new Genero() { Nombre = "Genero 2" });
			await contexto.SaveChangesAsync();

			var contexto2 = ConstruirContext(nombreBD);

			// Prueba
			var controller = new GenerosController(contexto2, mapper);

			var generoId = 1;
			var respuesta = await controller.Get(generoId);

			// Verificacion
			var resultado = respuesta.Value;
			Assert.AreEqual(generoId, resultado.Id);
		}

		[TestMethod]
		public async Task CrearGenero()
		{
			// Preparacion
			var nombreBD = Guid.NewGuid().ToString();
			var contexto = ConstruirContext(nombreBD);
			var mapper = ConfigurarAutoMapper();

			var nuevoGenero = new GeneroCreacionDTO() { Nombre = "nuevo género" };

			// Prueba
			var controller = new GenerosController(contexto, mapper);
			var respuesta = await controller.Post(nuevoGenero);

			// Verificacion
			var resultado = respuesta as CreatedAtRouteResult;
			Assert.IsNotNull(resultado);

			var contexto2 = ConstruirContext(nombreBD);
			var cantidad = await contexto2.Generos.CountAsync();
			Assert.AreEqual(1, cantidad);
		}

		[TestMethod]
		public async Task ActualizarGenero()
		{
			// Preparacion
			var nombreBD = Guid.NewGuid().ToString();
			var contexto = ConstruirContext(nombreBD);
			var mapper = ConfigurarAutoMapper();

			contexto.Generos.Add(new Genero()
			{
				Nombre = "Genero 1"
			});
			await contexto.SaveChangesAsync();

			// Prueba
			var contexto2 = ConstruirContext(nombreBD);
			var controller = new GenerosController(contexto2, mapper);
			var generoCreacionDTO = new GeneroCreacionDTO() { Nombre = "Nuevo nombre" };

			var generoId = 1;
			var respuesta = await controller.Put(generoId, generoCreacionDTO);

			// Verificacion
			var resultado = respuesta as StatusCodeResult;
			Assert.AreEqual(204, resultado.StatusCode);

			var contexto3 = ConstruirContext(nombreBD);
			var existe = await contexto3.Generos.AnyAsync(x => x.Nombre == "Nuevo nombre");
			Assert.IsTrue(existe);
		}

	}
}
