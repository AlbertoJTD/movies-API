using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

namespace MoviesAPI.Helpers
{
	public class TypeBinder : IModelBinder
	{
		public Task BindModelAsync(ModelBindingContext bindingContext)
		{
			var nombrePropiedad = bindingContext.ModelName;
			var proveedorDeValores = bindingContext.ValueProvider.GetValue(nombrePropiedad);

			if (proveedorDeValores == ValueProviderResult.None)
			{
				return Task.CompletedTask;
			}

			try
			{
				var valorDeserializado = JsonConvert.DeserializeObject<List<int>>(proveedorDeValores.FirstValue);
				bindingContext.Result = ModelBindingResult.Success(valorDeserializado);
			}
			catch (Exception)
			{
				bindingContext.ModelState.TryAddModelError(nombrePropiedad, "Valor invalido para tipo List<int>");
			}

			return Task.CompletedTask;
		}
	}
}
