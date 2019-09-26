using GramaticasCQL.Models;
using GramaticasCQL.Parsers;
using GramaticasCQL.Parsers.CQL.ast.entorno;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web.Http;

namespace GramaticasCQL.Controllers
{
    public class ValuesController : ApiController
    {

        // GET api/values
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        public string Get(int id)
        {
            BaseDatos.Entrada = "int ackermann(int @m, int @n) {\nif (@m == 0) {\nreturn (@n + 1);\n} else if (@m > 0 && @n == 0) {\nreturn ackermann(@m - 1, 1);\n} else {\nreturn ackermann(@m - 1, ackermann(@m, @n - 1));\n}\n}\nLog(ackermann(3,"+id+"));";

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

            return salida;
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
