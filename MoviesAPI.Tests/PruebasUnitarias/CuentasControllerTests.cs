using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using MoviesAPI.Controllers;
using MoviesAPI.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesAPI.Tests.PruebasUnitarias
{
	[TestClass]
	public class CuentasControllerTests: BasePruebas
	{
		[TestMethod]
		public async Task CrearUsuario()
		{
			// Preparacion
			var nombreBD = Guid.NewGuid().ToString();
			var cuentasController = ConstruirCuentasController(nombreBD);
			var userInfo = new UserInfo() { Email = "ejemplo@example.com", Password = "Aa123456!" };

			// Prueba
			await cuentasController.CreateUser(userInfo);
			var context2 = ConstruirContext(nombreBD);

			// Verificacion
			var numeroUsuario = await context2.Users.CountAsync();
			Assert.AreEqual(1, numeroUsuario);
		}

		private CuentasController ConstruirCuentasController(string nombreBD)
		{
			var context = ConstruirContext(nombreBD);
			var myUserStore = new UserStore<IdentityUser>(context);
			var userManager = BuildUserManager(myUserStore);
			var mapper = ConfigurarAutoMapper();

			var httpContext = new DefaultHttpContext();
			MockAuth(httpContext);
			var signInManager = SetupSignInManager(userManager, httpContext);

			var miConfiguracion = new Dictionary<string, string>
			{
				{"JWT:key", "73717aea4ab447da998a7c047786dbd5" }
			};

			var configuration = new ConfigurationBuilder().AddInMemoryCollection(miConfiguracion).Build();

			return new CuentasController(userManager, signInManager, configuration, context, mapper);
		}

		// Source: https://github.com/dotnet/aspnetcore/blob/master/src/Identity/test/Shared/MockHelpers.cs
		// Source: https://github.com/dotnet/aspnetcore/blob/master/src/Identity/test/Identity.Test/SignInManagerTest.cs
		// Some code was modified to be adapted to our project.
		private UserManager<TUser> BuildUserManager<TUser>(IUserStore<TUser> store = null) where TUser : class
		{
			store = store ?? new Mock<IUserStore<TUser>>().Object;
			var options = new Mock<IOptions<IdentityOptions>>();
			var idOptions = new IdentityOptions();
			idOptions.Lockout.AllowedForNewUsers = false;

			options.Setup(o => o.Value).Returns(idOptions);

			var userValidators = new List<IUserValidator<TUser>>();

			var validator = new Mock<IUserValidator<TUser>>();
			userValidators.Add(validator.Object);
			var pwdValidators = new List<PasswordValidator<TUser>>();
			pwdValidators.Add(new PasswordValidator<TUser>());

			var userManager = new UserManager<TUser>(store, options.Object, new PasswordHasher<TUser>(),
				userValidators, pwdValidators, new UpperInvariantLookupNormalizer(),
				new IdentityErrorDescriber(), null,
				new Mock<ILogger<UserManager<TUser>>>().Object);

			validator.Setup(v => v.ValidateAsync(userManager, It.IsAny<TUser>()))
				.Returns(Task.FromResult(IdentityResult.Success)).Verifiable();

			return userManager;
		}

		private static SignInManager<TUser> SetupSignInManager<TUser>(UserManager<TUser> manager,
			HttpContext context, ILogger logger = null, IdentityOptions identityOptions = null,
			IAuthenticationSchemeProvider schemeProvider = null) where TUser : class
		{
			var contextAccessor = new Mock<IHttpContextAccessor>();
			contextAccessor.Setup(a => a.HttpContext).Returns(context);
			identityOptions = identityOptions ?? new IdentityOptions();
			var options = new Mock<IOptions<IdentityOptions>>();
			options.Setup(a => a.Value).Returns(identityOptions);
			var claimsFactory = new UserClaimsPrincipalFactory<TUser>(manager, options.Object);
			schemeProvider = schemeProvider ?? new Mock<IAuthenticationSchemeProvider>().Object;
			var sm = new SignInManager<TUser>(manager, contextAccessor.Object, claimsFactory, options.Object, null, schemeProvider, new DefaultUserConfirmation<TUser>());
			sm.Logger = logger ?? (new Mock<ILogger<SignInManager<TUser>>>()).Object;
			return sm;
		}

		private Mock<IAuthenticationService> MockAuth(HttpContext context)
		{
			var auth = new Mock<IAuthenticationService>();
			context.RequestServices = new ServiceCollection().AddSingleton(auth.Object).BuildServiceProvider();
			return auth;
		}
	}
}
