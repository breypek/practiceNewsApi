using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using webApiNews.Models;
using webApiNews.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace webApiNews.Controllers
{
    [Route("api/v1/[controller]")]

    [ApiController]
    public class newsApiController : ControllerBase
    {
        // GET: api/<newsApiController>
        private readonly IConfiguration _configuration;

       public newsApiController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<newsApiController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        [SwaggerOperation(Summary = "Obtener noticias", Description = "En esta api se obtienen las noticias realacionadas con la ciudad escogida")]
        [HttpGet("listaNoticias")]
        [Produces("application/json")]
        public async Task<Object> GetNews(string pais)
        {

            
            HttpClient client = new HttpClient();
            service_news service = new service_news(client); //se intancia el servicio de news
            Object respuesta;

            DataTable res =  service.codPais(pais, _configuration);//se busca el codigo del pais pasando el nombre
            string codigo = "", nombrePais="";

            if (res.Rows.Count == 0)
            {
                respuesta = new Resultado();
                return respuesta;
            }

            codigo = res.Rows[0]["codigo"].ToString();
            nombrePais = res.Rows[0]["nombre"].ToString();

          
            //validar que cuand no encuentra un valor valido devuelve otro json con error, por eso esta entrando al catch
            try {

                service.insertar_pais_historial(codigo, nombrePais, _configuration);
                respuesta = await service.listNews(codigo); //aqui se hace la llamada a la api externa con el servicio

            }
            catch (Exception e)
            {
                Console.WriteLine("Error Controlado-------" + e.ToString());
                respuesta = new Resultado();
            }



            return respuesta;



        }



        [SwaggerOperation(Summary = "Obtener Historial", Description = "En esta api se obtiene el historial de busquedas de paises")]
        [HttpGet("historialBusqueda")]
        [Produces("application/json")]
        public IActionResult GetHistory()
        {

            HttpClient client = new HttpClient();
            service_news service = new service_news(client); //se intancia el servicio de news
       

            var res = service.listar_historial(_configuration);//se busca el codio del pais pasando el nombre

            return StatusCode(200, res);

        }




        // POST api/<newsApiController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<newsApiController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<newsApiController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
