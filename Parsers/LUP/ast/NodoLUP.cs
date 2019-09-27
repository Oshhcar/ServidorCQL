using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GramaticasCQL.Parsers.LUP.ast
{
    public class NodoLUP
    {
        public NodoLUP(int linea, int columna)
        {
            Linea = linea;
            Columna = columna;
        }

        public int Linea { get; set; }
        public int Columna { get; set; }
    }
}