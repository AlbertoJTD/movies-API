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
			CreateMap<ActorCreacionDTO, Actor>().ReverseMap();
		}
	}
}
