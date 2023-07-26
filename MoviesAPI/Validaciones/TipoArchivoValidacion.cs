using System.ComponentModel.DataAnnotations;

namespace MoviesAPI.Validaciones
{
	public class TipoArchivoValidacion : ValidationAttribute
	{
		private readonly string[] tiposArchivosValidos;

		public TipoArchivoValidacion(string[] tiposArchviosValidos)
        {
			this.tiposArchivosValidos = tiposArchviosValidos;
		}

        public TipoArchivoValidacion(GrupoTipoArchivo grupoTipoArchivo)
        {
			if (grupoTipoArchivo == GrupoTipoArchivo.Imagen)
			{
				tiposArchivosValidos = new string[] { "image/jpg", "image/jpeg", "image/png", "image/gif" };
			}
        }

		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			if (value == null)
			{
				return ValidationResult.Success;
			}

			IFormFile formFile = value as IFormFile;
			if (formFile == null)
			{
				return ValidationResult.Success;
			}

			if (!tiposArchivosValidos.Contains(formFile.ContentType))
			{
				return new ValidationResult($"El tipo de archivo debe ser uno de los siguientes: {string.Join(", ", tiposArchivosValidos)}");
			}

			return ValidationResult.Success;
		}
	}
}
