using GramaticasCQL.Parsers.CQL.ast.entorno;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GramaticasCQL.Parsers.LUP.ast
{
    public abstract class Sentencia : NodoLUP
    {
        public Sentencia(int linea, int columna) : base(linea, columna) { }

        public abstract Object Ejecutar(MasterBD master, LinkedList<Salida> log, LinkedList<Salida> respuesta, LinkedList<Error> errores);
    }
}