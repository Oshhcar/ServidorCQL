using GramaticasCQL.Parsers;
using GramaticasCQL.Parsers.CQL;
using GramaticasCQL.Parsers.CQL.ast;
using GramaticasCQL.Parsers.CQL.ast.entorno;
using GramaticasCQL.Parsers.LUP;
using GramaticasCQL.Parsers.LUP.ast;
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
        public static LinkedList<Salida> Respuesta = new LinkedList<Salida>();
        public static HttpServerUtility PathDatos;

        public static void Ejecutar()
        {
            Log.Clear();
            Errores.Clear();
            Respuesta.Clear();

            if (Master.GetUsuario("admin") == null)
            {
                Master.AddUsuario("admin", "admin");
            }

            AnalizadorLUP analizador = new AnalizadorLUP();

            if (analizador.AnalizarEntrada(Entrada))
            {
                ASTLUP ast = (ASTLUP)analizador.GenerarArbol(analizador.Raiz.Root);

                if (ast != null)
                {
                    ast.Ejecutar(Log, Errores, Respuesta, Master);
                }
                else
                    Respuesta.AddLast(new Salida(1, "[+MESSAGE]\n\tError en Paquete LUP.\n[-MESSAGE]"));
            }
            else
                Respuesta.AddLast(new Salida(1,"[+MESSAGE]\n\tError en Paquete LUP.\n[-MESSAGE]"));

            //Agregar error en archivo a errores

            /*
            AnalizadorCQL analizador = new AnalizadorCQL();

            if (analizador.AnalizarEntrada(Entrada))
            {
                ASTCQL ast = (ASTCQL)analizador.GenerarArbol(analizador.Raiz.Root);

                if (ast != null)
                {
                    ast.Ejecutar(Log, Errores, Master);
                }
            }
            */
        }
    }
}