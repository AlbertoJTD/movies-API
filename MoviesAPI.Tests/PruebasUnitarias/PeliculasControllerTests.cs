﻿using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
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
				new Pelicula(){Titulo = "Pelicula 1", FechaEstreno = new DateTime(2020, 1,1), EnCines = false},
				new Pelicula(){Titulo = "No estrenada", FechaEstreno = DateTime.Today.AddDays(10), EnCines = false},
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

		[TestMethod]
		public async Task FitrarEnCines()
		{
			// Preparacion
			var nombreBD = CrearDataPrueba();
			var mapper = ConfigurarAutoMapper();
			var contexto = ConstruirContext(nombreBD);

			// Prueba
			var controller = new PeliculasController(contexto, mapper, null, null);
			controller.ControllerContext.HttpContext = new DefaultHttpContext();

			var filtroDTO = new FiltroPeliculaDTO()
			{
				EnCines = true
			};

			// Verificacion
			var respuesta = await controller.Filtrar(filtroDTO);
			var peliculas = respuesta.Value;
			Assert.AreEqual(1, peliculas.Count);
			Assert.AreEqual("Pelicula en Cines", peliculas[0].Titulo);
		}

		[TestMethod]
		public async Task FiltrarProximosEstrenos()
		{
			// Preparacion
			var nombreBD = CrearDataPrueba();
			var mapper = ConfigurarAutoMapper();
			var contexto = ConstruirContext(nombreBD);

			// Prueba
			var controller = new PeliculasController(contexto, mapper, null, null);
			controller.ControllerContext.HttpContext = new DefaultHttpContext();

			var filtroDTO = new FiltroPeliculaDTO()
			{
				ProximosEstrenos = true
			};

			// Verificacion
			var respuesta = await controller.Filtrar(filtroDTO);
			var peliculas = respuesta.Value;
			Assert.AreEqual(1, peliculas.Count);
			Assert.AreEqual("No estrenada", peliculas[0].Titulo);
		}

		[TestMethod]
		public async Task FiltrarPorGenero()
		{
			// Preparacion
			var nombreBD = CrearDataPrueba();
			var mapper = ConfigurarAutoMapper();
			var contexto = ConstruirContext(nombreBD);

			// Prueba
			var controller = new PeliculasController(contexto, mapper, null, null);
			controller.ControllerContext.HttpContext = new DefaultHttpContext();

			var generoId = contexto.Generos.Select(x => x.Id).First();

			var filtroDTO = new FiltroPeliculaDTO()
			{
				GeneroId = generoId
			};

			// Verificacion
			var respuesta = await controller.Filtrar(filtroDTO);
			var peliculas = respuesta.Value;
			Assert.AreEqual(1, peliculas.Count);
			Assert.AreEqual("Pelicula con Genero", peliculas[0].Titulo);
		}

		[TestMethod]
		public async Task FiltrarOrdenarTituloAscendente()
		{
			// Preparacion
			var nombreBD = CrearDataPrueba();
			var mapper = ConfigurarAutoMapper();
			var contexto = ConstruirContext(nombreBD);

			// Prueba
			var controller = new PeliculasController(contexto, mapper, null, null);
			controller.ControllerContext.HttpContext = new DefaultHttpContext();

			var filtroDTO = new FiltroPeliculaDTO()
			{
				CampoOrdenar = "titulo",
				OrdenAscendente = true
			};

			var respuesta = await controller.Filtrar(filtroDTO);
			var peliculas = respuesta.Value;

			var contexto2 = ConstruirContext(nombreBD);
			var peliculasDB = contexto2.Peliculas.OrderBy(x => x.Titulo).ToList();

			// Verificacion
			Assert.AreEqual(peliculasDB.Count, peliculas.Count);

			for (int i = 0; i < peliculasDB.Count; i++)
			{
				var peliculaDelControlador = peliculas[i];
				var peliculaDB = peliculasDB[i];

				Assert.AreEqual(peliculaDB.Id, peliculaDelControlador.Id);
			}
		}

		[TestMethod]
		public async Task FiltrarTituloDescendente()
		{
			// Preparacion
			var nombreBD = CrearDataPrueba();
			var mapper = ConfigurarAutoMapper();
			var contexto = ConstruirContext(nombreBD);

			// Prueba
			var controller = new PeliculasController(contexto, mapper, null, null);
			controller.ControllerContext.HttpContext = new DefaultHttpContext();

			var filtroDTO = new FiltroPeliculaDTO()
			{
				CampoOrdenar = "titulo",
				OrdenAscendente = false
			};

			var respuesta = await controller.Filtrar(filtroDTO);
			var peliculas = respuesta.Value;

			var contexto2 = ConstruirContext(nombreBD);
			var peliculasDB = contexto2.Peliculas.OrderByDescending(x => x.Titulo).ToList();

			// Verificacion
			Assert.AreEqual(peliculasDB.Count, peliculas.Count);

			for (int i = 0; i < peliculasDB.Count; i++)
			{
				var peliculaDelControlador = peliculas[i];
				var peliculaDB = peliculasDB[i];

				Assert.AreEqual(peliculaDB.Id, peliculaDelControlador.Id);
			}
		}

		[TestMethod]
		public async Task FiltrarPorCampoIncorrecto_DevuelvePeliculas()
		{
			// Preparacion
			var nombreBD = CrearDataPrueba();
			var mapper = ConfigurarAutoMapper();
			var contexto = ConstruirContext(nombreBD);

			// Prueba
			var mock = new Mock<ILogger<PeliculasController>>();

			var controller = new PeliculasController(contexto, mapper, null, mock.Object);
			controller.ControllerContext.HttpContext = new DefaultHttpContext();

			var filtroDTO = new FiltroPeliculaDTO()
			{
				CampoOrdenar = "abc",
				OrdenAscendente = true
			};

			// Verificacion
			var respuesta = await controller.Filtrar(filtroDTO);
			var peliculas = respuesta.Value;

			var contexto2 = ConstruirContext(nombreBD);
			var peliculasDB = contexto2.Peliculas.ToList();

			Assert.AreEqual(peliculasDB.Count, peliculas.Count);
			Assert.AreEqual(1, mock.Invocations.Count);
		}
	}
}
