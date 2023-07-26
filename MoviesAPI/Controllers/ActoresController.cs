using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesAPI.DTOs;
using MoviesAPI.Entidades;

namespace MoviesAPI.Controllers
{
	[ApiController]
	[Route("api/actores")]
	public class ActoresController : ControllerBase
	{
		private readonly ApplicationDbContext context;
		private readonly IMapper mapper;

		public ActoresController(ApplicationDbContext context, IMapper mapper)
        {
			this.context = context;
			this.mapper = mapper;
		}

		[HttpGet]
		public async Task<ActionResult<List<ActorDTO>>> Get()
		{
			var entidades = await context.Actores.ToListAsync();

			return mapper.Map<List<ActorDTO>>(entidades);
		}

		[HttpGet("{id:int}", Name = "obtenerActor")]
		public async Task<ActionResult<ActorDTO>> Get(int id)
		{
			var entidad = await context.Actores.FirstOrDefaultAsync(x => x.Id == id);

			if (entidad == null)
			{
				return NotFound();
			}

			return mapper.Map<ActorDTO>(entidad);
		}

		[HttpPost]
		public async Task<ActionResult> Post([FromBody] ActorCreacionDTO actorCreacionDTO)
		{
			var entidad = mapper.Map<Actor>(actorCreacionDTO);
			context.Add(entidad);
			await context.SaveChangesAsync();

			var actorDTO = mapper.Map<ActorDTO>(entidad);
			return new CreatedAtRouteResult("obtenerActor", new { id = entidad.Id }, actorDTO);
		}

		[HttpPut("{id:int}")]
		public async Task<ActionResult> Put(int id, [FromBody] GeneroCreacionDTO actorCreacionDTO)
		{
			var entidad = mapper.Map<Actor>(actorCreacionDTO);
			entidad.Id = id;

			context.Entry(entidad).State = EntityState.Modified;
			await context.SaveChangesAsync();
			return NoContent();
		}

		[HttpDelete("{id:int}")]
		public async Task<ActionResult> Delete(int id)
		{
			var existe = await context.Actores.AnyAsync(x => x.Id == id);

			if (!existe)
			{
				return NotFound();
			}

			context.Remove(new Actor { Id = id });
			await context.SaveChangesAsync();

			return NoContent();
		}
	}
}
