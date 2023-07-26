using Microsoft.EntityFrameworkCore;
using MoviesAPI.Entidades;

namespace MoviesAPI
{
	public class ApplicationDbContext: DbContext
	{
		public ApplicationDbContext(DbContextOptions options) : base(options)
		{
		}

        public DbSet<Genero> Generos { get; set; }
		public DbSet<Actor> Actores { get; set; }
    }
}
