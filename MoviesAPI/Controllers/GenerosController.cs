using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesAPI.DTOs;
using MoviesAPI.Entidades;

namespace MoviesAPI.Controllers
{
	[ApiController]
	[Route("api/generos")]
	public class GenerosController : CustomBaseController
	{
		private readonly ApplicationDbContext context;
		private readonly IMapper mapper;

		public GenerosController(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
        {
			this.context = context;
			this.mapper = mapper;
		}

		[HttpGet]
		public async Task<ActionResult<List<GeneroDTO>>> Get()
		{
			return await Get<Genero, GeneroDTO>();
		}

		[HttpGet("{id:int}", Name = "obtenerGenero")]
		public async Task<ActionResult<GeneroDTO>> Get(int id)
		{
			return await Get<Genero, GeneroDTO>(id);
		}

		[HttpPost]
		public async Task<ActionResult> Post([FromBody] GeneroCreacionDTO generoCreacionDTO)
		{
			return await Post<GeneroCreacionDTO, Genero, GeneroDTO>(generoCreacionDTO, nombreRuta: "obtenerGenero");
		}

		[HttpPut("{id:int}")]
		public async Task<ActionResult> Put(int id, [FromBody] GeneroCreacionDTO generoCreacionDTO)
		{
			return await Put<GeneroCreacionDTO, Genero>(id, generoCreacionDTO);
		}

		[HttpDelete("{id:int}")]
		public async Task<ActionResult> Delete(int id)
		{
			var existe = await context.Generos.AnyAsync(x => x.Id == id);

			if (!existe)
			{
				return NotFound();
			}

			context.Remove(new Genero { Id = id });
			await context.SaveChangesAsync();

			return NoContent();
		}
    }
}
