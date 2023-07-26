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

		[HttpGet("{id:int")]
		public async Task<ActionResult>
    }
}
