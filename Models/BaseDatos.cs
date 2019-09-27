using GramaticasCQL.Parsers;
using GramaticasCQL.Parsers.CQL;
using GramaticasCQL.Parsers.CQL.ast;
using GramaticasCQL.Parsers.CQL.ast.entorno;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GramaticasCQL.Models
{
    public class BaseDatos
    {
        public static string Entrada = "";
        public static LinkedList<Salida> Log = new LinkedList<Salida>();
        public static LinkedList<Error> Errores = new LinkedList<Error>();
        public static MasterBD Master = new MasterBD();
        public static HttpServerUtility PathDatos;

        public static void Ejecutar()
        {
            Log.Clear();
            Errores.Clear();

            if (Master.GetUsuario("admin") == null)
            {
                Master.AddUsuario("admin", "admin");
            }

            AnalizadorCQL analizador = new AnalizadorCQL();

            if (analizador.AnalizarEntrada(Entrada))
            {
                ASTCQL ast = (ASTCQL)analizador.GenerarArbol(analizador.Raiz.Root);

                if (ast != null)
                {
                    ast.Ejecutar(Log, Errores, Master);
                }
            }
        }
    }
}