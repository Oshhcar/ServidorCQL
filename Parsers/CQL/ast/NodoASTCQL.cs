using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GramaticasCQL.Parsers.CQL.ast
{
    class NodoASTCQL
    {
        public NodoASTCQL(int linea, int columna)
        {
            Linea = linea;
            Columna = columna;
        }

        public int Linea { get; set; }
        public int Columna { get; set; }
    }
}
