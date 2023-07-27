using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesAPI.DTOs;
using MoviesAPI.Entidades;
using MoviesAPI.Servicios;

namespace MoviesAPI.Controllers
{
	[ApiController]
	[Route("api/peliculas")]
	public class PeliculasController: ControllerBase
	{
		private readonly ApplicationDbContext context;
		private readonly IMapper mapper;
		private readonly IAlmacenadorArchivos almacenadorArchivos;
		private readonly string contenedor = "peliculas";

		public PeliculasController(ApplicationDbContext context, IMapper mapper, IAlmacenadorArchivos almacenadorArchivos)
        {
			this.context = context;
			this.mapper = mapper;
			this.almacenadorArchivos = almacenadorArchivos;
		}

		[HttpGet()]
		public async Task<ActionResult<List<PeliculaDTO>>> Get()
		{
			var peliculas = await context.Peliculas.ToListAsync();
			return mapper.Map<List<PeliculaDTO>>(peliculas);
		}

		[HttpGet("{id:int}", Name = "obtenerPelicula")]
		public async Task<ActionResult<PeliculaDTO>> Get(int id)
		{
			var pelicula = await context.Peliculas.FirstOrDefaultAsync(x => x.Id == id);
			if (pelicula == null)
			{
				return NotFound();
			}

			var dto = mapper.Map<PeliculaDTO>(pelicula);
			return dto;
		}

		[HttpPost]
		public async Task<ActionResult> Post([FromForm] PeliculaCreacionDTO peliculaCreacionDTO)
		{
			var pelicula = mapper.Map<Pelicula>(peliculaCreacionDTO);

			if (peliculaCreacionDTO.Poster != null)
			{
				using (var memoryStream = new MemoryStream())
				{
					await peliculaCreacionDTO.Poster.CopyToAsync(memoryStream);
					var contenido = memoryStream.ToArray();
					var extension = Path.GetExtension(peliculaCreacionDTO.Poster.FileName);

					pelicula.Poster = await almacenadorArchivos.GuardarArchivo(contenido, extension, contenedor,
																				peliculaCreacionDTO.Poster.ContentType);
				}
			}

			context.Add(pelicula);
			await context.SaveChangesAsync();

			var peliculaDTO = mapper.Map<PeliculaDTO>(pelicula);

			return new CreatedAtRouteResult("obtenerPelicula", new { id = pelicula.Id }, peliculaDTO);
		}

		[HttpPut("{id:int}")]
		public async Task<ActionResult> Put(int id, [FromBody] PeliculaCreacionDTO peliculaCreacionDTO)
		{
			var pelicula = await context.Peliculas.FirstOrDefaultAsync(x => x.Id == id);
			if (pelicula == null)
			{
				return NotFound();
			}

			if (peliculaCreacionDTO.Poster != null)
			{
				using (var memoryStream = new MemoryStream())
				{
					await peliculaCreacionDTO.Poster.CopyToAsync(memoryStream);
					var contenido = memoryStream.ToArray();
					var extension = Path.GetExtension(peliculaCreacionDTO.Poster.FileName);

					pelicula.Poster = await almacenadorArchivos.EditarArchivo(contenido, extension, contenedor, pelicula.Poster,
																				peliculaCreacionDTO.Poster.ContentType);
				}
			}

			pelicula = mapper.Map(peliculaCreacionDTO, pelicula);

			await context.SaveChangesAsync();
			return NoContent();
		}

		[HttpDelete("{id:int}")]
		public async Task<ActionResult> Delete(int id)
		{
			var existePelicula = await context.Peliculas.AnyAsync(x => x.Id == id);

			if (!existePelicula)
			{
				return NotFound();
			}

			context.Remove(new Pelicula { Id = id });
			await context.SaveChangesAsync();

			return NoContent();
		}

		[HttpPatch("{id:int}")]
		public async Task<ActionResult> Patch(int id, [FromBody] JsonPatchDocument<PeliculaPatchDTO> patchDocument)
		{
			if (patchDocument == null)
			{
				return BadRequest();
			}

			var entidadDB = await context.Peliculas.FirstOrDefaultAsync(x => x.Id == id);
			if (entidadDB == null)
			{
				return NotFound();
			}

			var entidadDTO = mapper.Map<PeliculaPatchDTO>(entidadDB);
			patchDocument.ApplyTo(entidadDTO, ModelState);

			var esValido = TryValidateModel(entidadDTO);

			if (!esValido)
			{
				return BadRequest(ModelState);
			}

			mapper.Map(entidadDTO, entidadDB);
			await context.SaveChangesAsync();

			return NoContent();
		}
	}
}
