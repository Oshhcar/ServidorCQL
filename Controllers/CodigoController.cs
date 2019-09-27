using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web;
using System.Web.Http;
using GramaticasCQL.Models;
using GramaticasCQL.Parsers;

namespace GramaticasCQL.Controllers
{
    public class CodigoController : ApiController
    {

        //POST api/Codigo
        public IHttpActionResult Post([FromBody]Codigo codigo)
        {
            if (ModelState.IsValid)
            {
                BaseDatos.PathDatos = HttpContext.Current.Server;//.MapPath("/Files/baseDatos.chison");
                BaseDatos.Entrada = codigo.Contenido;

                ThreadStart thread = new ThreadStart(BaseDatos.Ejecutar);
                Thread t = new Thread(thread, 1000000000);
                t.IsBackground = false;
                t.Start();

                while (t.IsAlive)
                { }

                string salida = "";

                foreach (Salida s in BaseDatos.Log)
                {
                    salida += s.Contenido;
                }

                return Ok(salida);

            }
            return BadRequest(ModelState);
        }
    }
}
