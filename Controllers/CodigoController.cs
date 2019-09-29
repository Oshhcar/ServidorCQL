using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using GramaticasCQL.Models;
using GramaticasCQL.Parsers;

namespace GramaticasCQL.Controllers
{
    [EnableCors(origins: "*", headers:"*", methods:"*")]
    public class CodigoController : ApiController
    {

        //POST api/Codigo HttpResponseMessage
        public IHttpActionResult Post([FromBody]Codigo codigo)
        {
            if (ModelState.IsValid)
            {
                BaseDatos.PathDatos = HttpContext.Current.Server;//.MapPath("/Files/baseDatos.chison");
                BaseDatos.Entrada = codigo.Contenido;

                ThreadStart thread = new ThreadStart(BaseDatos.Ejecutar);
                Thread t = new Thread(thread, 1000000000)
                {
                    IsBackground = false
                };
                t.Start();

                while (t.IsAlive)
                { }

                string salida = "";

                foreach (Salida s in BaseDatos.Respuesta)
                {
                    salida += s.Contenido +"\n";
                }

                foreach (Salida s in BaseDatos.Log)
                {
                    if (s.Tipo == 1)
                        salida += "[+MESSAGE]\n\t$" + s.Contenido + "$\n[-MESSAGE]\n";
                    else
                        salida += "[+DATA]\n$" + s.Contenido + "$\n[-DATA]\n";
                }

                foreach (Error e in BaseDatos.Errores)
                {
                    salida += "[+ERROR]\n";
                    salida += "\t[+LINE]\n\t\t$" + e.Linea + "$\n\t[-LINE]\n";
                    salida += "\t[+COLUMN]\n\t\t$" + e.Columna + "$\n\t[-COLUMN]\n";
                    salida += "\t[+TYPE]\n\t\t$" + e.Valor + "$\n\t[-TYPE]\n";
                    salida += "\t[+DESC]\n\t\t$" + e.Descripcion + "$\n\t[-DESC]\n";
                    salida += "[-ERROR]\n";
                }

                //HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, "value");
                //response.Content = new StringContent(salida, Encoding.Unicode);

                return Ok(salida);

            }
            return BadRequest(ModelState);
            //return Request.CreateResponse(HttpStatusCode.NotFound, "Error!"); ;
        }
    }
}
