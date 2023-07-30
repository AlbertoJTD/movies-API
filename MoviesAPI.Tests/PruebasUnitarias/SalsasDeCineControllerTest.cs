using MoviesAPI.Controllers;
using MoviesAPI.DTOs;
using MoviesAPI.Entidades;
using NetTopologySuite.Geometries;
using NetTopologySuite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesAPI.Tests.PruebasUnitarias
{
	[TestClass]
	public class SalsasDeCineControllerTest: BasePruebas
	{
		[TestMethod]
		public async Task ObtenerSalasDeCineMenorOIgualA5KM()
		{
			// Preparacion
			var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
			using (var context = LocalDbDatabaseInitializer.GetDbContextLocalDb(false))
			{
				var salasDeCine = new List<SalaDeCine>()
				{
					new SalaDeCine{ Nombre = "Sala 1", Ubicacion = geometryFactory.CreatePoint(new Coordinate(-69.9388777, 18.4839233)) }
				};

				context.AddRange(salasDeCine);
				await context.SaveChangesAsync();
			}

			// Prueba
			var filtro = new SalaDeCineCercanoFiltroDTO()
			{
				DistanciaEnKms = 5,
				Latitud = 18.481139,
				Longitud = -69.938950
			};

			// Verificacion
			using (var context = LocalDbDatabaseInitializer.GetDbContextLocalDb(false))
			{
				var mapper = ConfigurarAutoMapper();
				var controller = new SalasDeCineController(context, mapper, geometryFactory);
				var respuesta = await controller.Cercanos(filtro);
				var valor = respuesta.Value;
				Assert.AreEqual(1, valor.Count);
			}
		}
	}
}
