using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GramaticasCQL.Parsers.CQL.ast.entorno;

namespace GramaticasCQL.Parsers.LUP.ast.sentencia
{
    public class Logout : Sentencia
    {
        public Logout(string nombre, int linea, int columna) : base(linea, columna)
        {
            Nombre = nombre;
        }

        public string Nombre { get; set; }

        public override object Ejecutar(MasterBD master, LinkedList<Salida> log, LinkedList<Salida> respuesta, LinkedList<Error> errores)
        {

            if (!Nombre.Equals(""))
            {
                Usuario user = master.GetUsuario(Nombre.Trim());

                if (user != null)
                {
                    respuesta.AddLast(new Salida(1, "[+LOGOUT]\n\t[SUCCESS]\n[-LOGOUT]"));
                    return null;
                }
            }

            respuesta.AddLast(new Salida(1, "[+LOGOUT]\n\t[FAIL]\n[-LOGOUT]"));
            return null;
        }
    }
}