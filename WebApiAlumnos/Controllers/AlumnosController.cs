using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAlumnos.Entidades;
using WebApiAlumnos.Filtros;
using WebApiAlumnos.Services;

namespace WebApiAlumnos.Controllers
{
    [ApiController]
    [Route("api/alumnos")] // api/alumnos2 //http://localhost:9090/api/alumnos
    //[Authorize]
    public class AlumnosController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IService service;
        private readonly ServiceTransient serviceTransient;
        private readonly ServiceScoped serviceScoped;
        private readonly ServiceSingleton serviceSingleton;
        private readonly ILogger<AlumnosController> logger;
        private readonly IWebHostEnvironment env;
        private readonly string nuevosRegistros = "nuevosRegistros.txt";
        private readonly string registrosConsultados = "registrosConsultados.txt";
        
        public AlumnosController(ApplicationDbContext context, IService service,
            ServiceTransient serviceTransient, ServiceScoped serviceScoped,
            ServiceSingleton serviceSingleton, ILogger<AlumnosController> logger,
            IWebHostEnvironment env)
        {
            this.dbContext = context;
            this.service = service;
            this.serviceTransient = serviceTransient;
            this.serviceScoped = serviceScoped;
            this.serviceSingleton = serviceSingleton;
            this.logger = logger;
            this.env = env;
        }

        [HttpGet("GUID")]
        [ResponseCache(Duration = 10)]
        [ServiceFilter(typeof(FiltroDeAccion))]
        public ActionResult ObtenerGuid()
        {
            throw new NotImplementedException();
            logger.LogInformation("Durante la ejecucion");
            return Ok(new
            {
                AlumnosControllerTransient = serviceTransient.guid,
                ServiceA_Transient = service.GetTransient(),
                AlumnosControllerScoped = serviceScoped.guid,
                ServiceA_Scoped = service.GetScoped(),
                AlumnosControllerSingleton = serviceSingleton.guid,
                ServiceA_Singleton = service.GetSingleton()
            });
        }

        [HttpGet] 
        [HttpGet("listado")] 
        [HttpGet("/listado")]
        //[ResponseCache(Duration = 15)]
        //[Authorize]
        //[ServiceFilter(typeof(FiltroDeAccion))]
        public async Task<ActionResult<List<Alumno>>> GetAlumnos()
        {
            //* Niveles de logs
            // Critical
            // Error
            // Warning
            // Information
            // Debug
            // Trace
            // *//
            throw new NotImplementedException();
            logger.LogInformation("Se obtiene el listado de alumnos");
            logger.LogWarning("Mensaje de prueba warning");
            service.EjecutarJob();
            return await dbContext.Alumnos.Include(x => x.clases).ToListAsync();
        }

        [HttpGet("primero")] //api/alumnos/primero?
        public async Task<ActionResult<Alumno>> PrimerAlumno()
        {
            return await dbContext.Alumnos.FirstOrDefaultAsync();
        }

        [HttpGet("primero2")] //api/alumnos/primero
        public ActionResult<Alumno> PrimerAlumnoD()
        {
            return new Alumno { Nombre = "Gustavo"};
        }

        // api/alumnos?nombre=...

        [HttpGet("{param?}")] //Se puede usar ? para que no sea obligatorio el parametro /{param=Gustavo}  getAlumno/{id:int}/
        public async Task<ActionResult<Alumno>> Get(int id, string param)
        {
            var alumno = await dbContext.Alumnos.FirstOrDefaultAsync(x => x.Id == id);

    

            if (alumno == null)
            {
                return NotFound();
            }

            return alumno;
        }

        [HttpGet("obtenerAlumno/{nombre}")]
        public async Task<ActionResult<Alumno>> Get([FromRoute] string nombre)
        {
            var alumno = await dbContext.Alumnos.FirstOrDefaultAsync(x => x.Nombre.Contains(nombre));

            if (alumno == null)
            {
                logger.LogError("No se encuentra el alumno. ");
                return NotFound();
            }
            
            var ruta = $@"{env.ContentRootPath}\wwwroot\{registrosConsultados}";
            using (StreamWriter writer = new StreamWriter(ruta, append: true)) { writer.WriteLine(alumno.Id + " " + alumno.Nombre); }

            return alumno;
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] Alumno alumno)
        {
            //Ejemplo para validar desde el controlador con la BD con ayuda del dbContext

            var existeAlumnoMismoNombre = await dbContext.Alumnos.AnyAsync(x => x.Nombre == alumno.Nombre);

            if (existeAlumnoMismoNombre)
            {
                return BadRequest("Ya existe un autor con el nombre");
            }

            
            dbContext.Add(alumno);
            await dbContext.SaveChangesAsync();

         //   var ruta = $@"{env.ContentRootPath}\wwwroot\{nuevosRegistros}";
          //  using (StreamWriter writer = new StreamWriter(ruta, append: true)) { writer.WriteLine(alumno.Id + " " + alumno.Nombre); }

            return Ok();
        }

        [HttpPut("{id:int}")] // api/alumnos/1
        public async Task<ActionResult> Put(Alumno alumno, int id)
        {
            var exist = await dbContext.Alumnos.AnyAsync(x => x.Id == id);
            if (!exist)
            {
                return NotFound();
            }

            if (alumno.Id != id)
            {
                return BadRequest("El id del alumno no coincide con el establecido en la url.");
            }

            dbContext.Update(alumno);
            await dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var exist = await dbContext.Alumnos.AnyAsync(x => x.Id == id);
            if (!exist)
            {
                 return NotFound("El Recurso no fue encontrado.");
            }

            dbContext.Remove(new Alumno()
            {
                Id = id
            });
            await dbContext.SaveChangesAsync();
            return Ok();
        }
    }
}
