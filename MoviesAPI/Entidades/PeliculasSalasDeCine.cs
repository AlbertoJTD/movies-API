namespace MoviesAPI.Entidades
{
	public class PeliculasSalasDeCine
	{
        public int PeliculaID { get; set; }
        public int SalaDeCineId { get; set; }
        public Pelicula Pelicula { get; set; }
        public SalaDeCine SalaDeCine { get; set; }
    }
}
