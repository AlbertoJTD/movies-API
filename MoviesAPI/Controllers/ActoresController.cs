﻿using AutoMapper;
using Azure;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesAPI.DTOs;
using MoviesAPI.Entidades;
using MoviesAPI.Helpers;
using MoviesAPI.Servicios;

namespace MoviesAPI.Controllers
{
	[ApiController]
	[Route("api/actores")]
	public class ActoresController : CustomBaseController
	{
		private readonly ApplicationDbContext context;
		private readonly IMapper mapper;
		private readonly IAlmacenadorArchivos almacenadorArchivos;
		private readonly string contenedor = "actores";

		public ActoresController(ApplicationDbContext context, IMapper mapper, IAlmacenadorArchivos almacenadorArchivos): base(context, mapper)
        {
			this.context = context;
			this.mapper = mapper;
			this.almacenadorArchivos = almacenadorArchivos;
		}

		[HttpGet]
		public async Task<ActionResult<List<ActorDTO>>> Get([FromQuery] PaginacionDTO paginacionDTO)
		{
			return await Get<Actor, ActorDTO>(paginacionDTO);
		}

		[HttpGet("{id:int}", Name = "obtenerActor")]
		public async Task<ActionResult<ActorDTO>> Get(int id)
		{
			return await Get<Actor, ActorDTO>(id);
		}

		[HttpPost]
		public async Task<ActionResult> Post([FromForm] ActorCreacionDTO actorCreacionDTO)
		{
			var entidad = mapper.Map<Actor>(actorCreacionDTO);

			if (actorCreacionDTO.Foto != null)
			{
				using (var memoryStream = new MemoryStream())
				{
					await actorCreacionDTO.Foto.CopyToAsync(memoryStream);
					var contenido = memoryStream.ToArray();
					var extension = Path.GetExtension(actorCreacionDTO.Foto.FileName);

					entidad.Foto = await almacenadorArchivos.GuardarArchivo(contenido, extension, contenedor, actorCreacionDTO.Foto.ContentType);
				}
			}

			context.Add(entidad);
			await context.SaveChangesAsync();

			var actorDTO = mapper.Map<ActorDTO>(entidad);
			return new CreatedAtRouteResult("obtenerActor", new { id = entidad.Id }, actorDTO);
		}

		[HttpPut("{id:int}")]
		public async Task<ActionResult> Put(int id, [FromForm] ActorCreacionDTO actorCreacionDTO)
		{
			//var entidad = mapper.Map<Actor>(actorCreacionDTO);
			//entidad.Id = id;

			//context.Entry(entidad).State = EntityState.Modified;

			var actorDB = await context.Actores.FirstOrDefaultAsync(x => x.Id == id);
			if (actorDB == null)
			{
				return NotFound();
			}

			if (actorCreacionDTO.Foto != null)
			{
				using (var memoryStream = new MemoryStream())
				{
					await actorCreacionDTO.Foto.CopyToAsync(memoryStream);
					var contenido = memoryStream.ToArray();
					var extension = Path.GetExtension(actorCreacionDTO.Foto.FileName);

					actorDB.Foto = await almacenadorArchivos.EditarArchivo(contenido, extension, contenedor, actorDB.Foto, actorCreacionDTO.Foto.ContentType);
				}
			}

			actorDB = mapper.Map(actorCreacionDTO, actorDB);

			await context.SaveChangesAsync();
			return NoContent();
		}

		[HttpDelete("{id:int}")]
		public async Task<ActionResult> Delete(int id)
		{
			return await Delete<Actor>(id);
		}

		[HttpPatch("{id:int}")]
		public async Task<ActionResult> Patch(int id, [FromBody] JsonPatchDocument<ActorPatchDTO> patchDocument)
		{
			return await Patch<Actor, ActorPatchDTO>(id, patchDocument);
		}
	}
}
