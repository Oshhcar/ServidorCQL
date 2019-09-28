using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GramaticasCQL.Parsers.CQL.ast.entorno;

namespace GramaticasCQL.Parsers.LUP.ast.sentencia
{
    public class Login : Sentencia
    {
        public Login(string nombre, string pass, int linea, int columna) : base(linea, columna)
        {
            Nombre = nombre;
            Pass = pass;
        }

        public string Nombre { get; set; }
        public string Pass { get; set; }

        public override object Ejecutar(MasterBD master, LinkedList<Salida> log, LinkedList<Salida> respuesta, LinkedList<Error> errores)
        {
            if (!Nombre.Equals("") && !Pass.Equals(""))
            {
                Usuario user = master.GetUsuario(Nombre);

                if (user != null)
                {
                    if (user.Password.Equals(Pass))
                    {
                        respuesta.AddLast(new Salida(1, "[+LOGIN]\n\t[SUCCESS]\n[-LOGIN]"));
                        return null;
                    }
                }
            }

            respuesta.AddLast(new Salida(1, "[+LOGIN]\n\t[FAIL]\n[-LOGIN]"));
            return null;
        }
    }
}