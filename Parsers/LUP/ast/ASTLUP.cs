using GramaticasCQL.Parsers.CQL.ast.entorno;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GramaticasCQL.Parsers.LUP.ast
{
    public class ASTLUP
    {
        public ASTLUP(LinkedList<NodoLUP> sentencias)
        {
            Sentencias = sentencias;
        }

        public LinkedList<NodoLUP> Sentencias { get; set; }

        public void Ejecutar(LinkedList<Salida> log, LinkedList<Error> errores, LinkedList<Salida> respuesta, MasterBD master)
        {

            foreach (NodoLUP nodo in Sentencias)
            {
                if (nodo is Sentencia sentencia)
                {
                    sentencia.Ejecutar(master, log, respuesta, errores);
                }
            }

        }
    }
}