using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Data.SqlClient;
using System.Net.Http;
using System.Text.Json;
using webApiNews.Models;

namespace webApiNews.Services
{
    public class service_news
    {

        private HttpClient client;
        private string url_base = "http://api.mediastack.com/v1/news";
        private string apikey = "9c6d974929ca3916c851a29039868950";

       


        public service_news(HttpClient httpClient)
        {
            client = httpClient;
        }


        public  async Task<Resultado> listNews(string codigo)
        {
            
            string uri = url_base +  "?access_key=" + apikey + "&countries=" + codigo;

            Console.WriteLine(uri);
            var response = await client.GetStringAsync(uri);

            var result = JsonConvert.DeserializeObject<Resultado>(response);


            return result;



        }

       
        public DataTable codPais(string pais, IConfiguration _configuration)
        {
            string cadenaconexion = _configuration.GetSection("StringConnections").GetSection("DefaultConnection").Value;
            DataTable dt = new DataTable();
            Console.WriteLine(cadenaconexion);
            string queryString = $"SELECT TOP 1 iso2 as codigo,nombre FROM info_paises WHERE nombre LIKE '%{pais}%' OR name LIKE '%{pais}%'";
            
            using (SqlConnection con = new SqlConnection(cadenaconexion))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(queryString, con);
                SqlDataAdapter data = new SqlDataAdapter();
                data.SelectCommand = cmd;
                data.Fill(dt);
            }
           
            return dt;
        }



        public void insertar_pais_historial(string codigo, string pais, IConfiguration _configuration)
        {
            
            string cadenaconexion = _configuration.GetSection("StringConnections").GetSection("DefaultConnection").Value;
           
            //tengo que validar que si el codigo existe no lo inserte otra vez
           string cadena= "INSERT INTO historial_busquedas (codigo, nombre)  VALUES ( @codigo, @nombre)";

            using (SqlConnection con = new SqlConnection(cadenaconexion))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(cadena, con);
                cmd.Parameters.AddWithValue("@codigo",  codigo);
                cmd.Parameters.AddWithValue("@nombre",  pais);
                cmd.ExecuteNonQuery();
       
            }

            
        }



        public Object listar_historial( IConfiguration _configuration)
        {

            string cadenaconexion = _configuration.GetSection("StringConnections").GetSection("DefaultConnection").Value;
            
            DataTable dt = new DataTable();
           

            string queryString = "SELECT id, codigo, nombre from historial_busquedas";

            using (SqlConnection con = new SqlConnection(cadenaconexion))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(queryString, con);
                SqlDataAdapter data = new SqlDataAdapter();
                data.SelectCommand = cmd;
                data.Fill(dt);
            }

            var respuesta =
           from order in dt.AsEnumerable()
           select new
           {
               id =
                   order.Field<dynamic>("id") == null ? 0 : order.Field<dynamic>("id"),
               codigo =
                   order.Field<dynamic>("codigo") == null ? 0 : order.Field<dynamic>("codigo"),
               nombre =
                   order.Field<dynamic>("nombre") == null ? 0 : order.Field<dynamic>("nombre")
              
           };


            return respuesta;
        }




  }
}
