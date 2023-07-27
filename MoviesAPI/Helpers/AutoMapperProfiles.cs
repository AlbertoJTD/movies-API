using AutoMapper;
using MoviesAPI.DTOs;
using MoviesAPI.Entidades;

namespace MoviesAPI.Helpers
{
	public class AutoMapperProfiles : Profile
	{
        public AutoMapperProfiles()
        {
            CreateMap<Genero, GeneroDTO>().ReverseMap();
			CreateMap<GeneroCreacionDTO, Genero>().ReverseMap();

			CreateMap<Actor, ActorDTO>().ReverseMap();
			CreateMap<ActorCreacionDTO, Actor>().ReverseMap().ForMember(x => x.Foto, options => options.Ignore());
			CreateMap<ActorPatchDTO, Actor>().ReverseMap();

			CreateMap<PeliculaDTO, Pelicula>().ReverseMap();
			CreateMap<PeliculaCreacionDTO, Pelicula>().ReverseMap().ForMember(x => x.Poster, options => options.Ignore());
			CreateMap<PeliculaPatchDTO, Pelicula>().ReverseMap();

		}
	}
}
