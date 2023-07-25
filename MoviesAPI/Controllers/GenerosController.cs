using Microsoft.AspNetCore.Mvc;

namespace MoviesAPI.Controllers
{
	[ApiController]
	[Route("api/generos")]
	public class GenerosController : ControllerBase
	{
		private readonly ApplicationDbContext context;

		public GenerosController(ApplicationDbContext context)
        {
			this.context = context;
		}
    }
}
