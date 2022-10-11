using WebApiAlumnos.Validaciones;

namespace WebApiAlumnos.Entidades
{
    public class Clase
    {
        public int Id { get; set; }

        [PrimeraLetraMayuscula]
        public string Nombre { get; set; }

        public string Semestre { get; set; }

        public int AlumnoId { get; set; }
        public Alumno Alumno { get; set;}
    }
}
