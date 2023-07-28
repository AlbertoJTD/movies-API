using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MoviesAPI.Entidades;
using NetTopologySuite.Geometries;
using NetTopologySuite;
using System.Security.Claims;

namespace MoviesAPI
{
	public class ApplicationDbContext: IdentityDbContext
	{
		public ApplicationDbContext(DbContextOptions options) : base(options)
		{
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<PeliculasActores>().HasKey(x => new { x.ActorId, x.PeliculaId });
			modelBuilder.Entity<PeliculasGeneros>().HasKey(x => new { x.GeneroId, x.PeliculaId });

			modelBuilder.Entity<PeliculasSalasDeCine>().HasKey(x => new { x.PeliculaID, x.SalaDeCineId });

			SeedData(modelBuilder);
			base.OnModelCreating(modelBuilder);
		}

		private void SeedData(ModelBuilder modelBuilder)
		{

			var rolAdminId = "9aae0b6d-d50c-4d0a-9b90-2a6873e3845d";
			var usuarioAdminId = "5673b8cf-12de-44f6-92ad-fae4a77932ad";

			var rolAdmin = new IdentityRole()
			{
				Id = rolAdminId,
				Name = "Admin",
				NormalizedName = "Admin"
			};

			var passwordHasher = new PasswordHasher<IdentityUser>();

			var username = "alberth@example.com";

			var usuarioAdmin = new IdentityUser()
			{
				Id = usuarioAdminId,
				UserName = username,
				NormalizedUserName = username,
				Email = username,
				NormalizedEmail = username,
				PasswordHash = passwordHasher.HashPassword(null, "Aa123456!")
			};

			//modelBuilder.Entity<IdentityUser>().HasData(usuarioAdmin);
			//modelBuilder.Entity<IdentityRole>().HasData(rolAdmin);
			//modelBuilder.Entity<IdentityUserClaim<string>>().HasData(new IdentityUserClaim<string>()
			//{
			//	Id = 1,
			//	ClaimType = ClaimTypes.Role,
			//	UserId = usuarioAdminId,
			//	ClaimValue = "Admin"
			//});
		}

		public DbSet<Genero> Generos { get; set; }
		public DbSet<Actor> Actores { get; set; }
		public DbSet<Pelicula> Peliculas { get; set; }
		public DbSet<PeliculasActores> PeliculasActores { get; set; }
		public DbSet<PeliculasGeneros> PeliculasGeneros { get; set; }
		public DbSet<SalaDeCine> SalasDeCine { get; set; }
		public DbSet<PeliculasSalasDeCine> PeliculasSalasDeCine { get; set; }
		public DbSet<Review> Reviews { get; set; }
	}
}
