using GramaticasCQL.Parsers.CQL.ast.expresion;
using GramaticasCQL.Parsers.CQL.ast.instruccion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GramaticasCQL.Parsers.CQL.ast.entorno
{
    class Funcion
    {
        public Funcion(LinkedList<Identificador> parametro, Bloque bloque)
        {
            Parametro = parametro;
            Bloque = bloque;
        }

        public LinkedList<Identificador> Parametro { get; set; }
        public Bloque Bloque { get; set; }
    }
}
