﻿using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using MoviesAPI.Helpers;
using MoviesAPI.Servicios;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using System.Text.Json.Serialization;

namespace MoviesAPI
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		public void ConfigureServices(IServiceCollection services)
		{
			services.AddAutoMapper(typeof(Startup));

			services.AddTransient<IAlmacenadorArchivos, AlmacenadorArchivosLocal>();
			services.AddHttpContextAccessor();

			services.AddSingleton<GeometryFactory>(NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326));
			services.AddSingleton(provider =>
				new MapperConfiguration(config =>
				{
					var geometryFactory = provider.GetRequiredService<GeometryFactory>();
					config.AddProfile(new AutoMapperProfiles(geometryFactory));
				}).CreateMapper()
			);

			services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
																		  sqlServerOptions => sqlServerOptions.UseNetTopologySuite()));

			services.AddControllers().AddNewtonsoftJson();
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();
			app.UseRouting();

			app.UseAuthorization();
			app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
		}
	}
}
