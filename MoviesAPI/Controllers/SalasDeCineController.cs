using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesAPI.DTOs;
using MoviesAPI.Entidades;
using NetTopologySuite.Geometries;

namespace MoviesAPI.Controllers
{
    [ApiController]
    [Route("api/SalasDeCine")]
	public class SalasDeCineController: CustomBaseController
	{
		private readonly ApplicationDbContext context;
		private readonly GeometryFactory geometryFactory;

		public SalasDeCineController(ApplicationDbContext context, IMapper mapper, GeometryFactory geometryFactory): base(context, mapper)
        {
			this.context = context;
			this.geometryFactory = geometryFactory;
		}

        [HttpGet]
        public async Task<ActionResult<List<SalaDeCineDTO>>> Get()
        {
            return await Get<SalaDeCine, SalaDeCineDTO>();
        }

        [HttpGet("{id:int}", Name = "obtenerSalaDeCine")]
        public async Task<ActionResult<SalaDeCineDTO>> Get(int id)
        {
            return await Get<SalaDeCine, SalaDeCineDTO>(id);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] SalaDeCineCreacionDTO salaDeCineCreacionDTO)
        {
            return await Post<SalaDeCineCreacionDTO, SalaDeCine, SalaDeCineDTO>(salaDeCineCreacionDTO, "obtenerSalaDeCine");
        }

        [HttpPut("{id:int}")]
		public async Task<ActionResult> Put(int id, [FromBody] SalaDeCineCreacionDTO salaDeCineCreacionDTO)
		{
			return await Put<SalaDeCineCreacionDTO, SalaDeCine>(id, salaDeCineCreacionDTO);
		}

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            return await Delete<SalaDeCine>(id);
        }

        [HttpGet("CinesCercanos")]
        public async Task<ActionResult<List<SalaDeCineCercanoDTO>>> Cercanos([FromQuery] SalaDeCineCercanoFiltroDTO filtroCine)
        {
            var ubicacionUsuario = geometryFactory.CreatePoint(new Coordinate(filtroCine.Longitud, filtroCine.Latitud));

            var salasDeCine = await context.SalasDeCine.OrderBy(x => x.Ubicacion.Distance(ubicacionUsuario))
                                                       .Where(x => x.Ubicacion.IsWithinDistance(ubicacionUsuario, filtroCine.DistanciaEnKms * 1000))
                                                       .Select(x => new SalaDeCineCercanoDTO
                                                       {
                                                           Id = x.Id,
                                                           Nombre = x.Nombre,
                                                           Latitud = x.Ubicacion.Y,
                                                           Longitud = x.Ubicacion.X,
                                                           DistanciaEnMetros = Math.Round(x.Ubicacion.Distance(ubicacionUsuario))
                                                       }).ToListAsync();

            return salasDeCine;
        }
	}
}
