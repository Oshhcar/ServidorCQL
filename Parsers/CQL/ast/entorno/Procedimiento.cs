using GramaticasCQL.Parsers.CQL.ast.expresion;
using GramaticasCQL.Parsers.CQL.ast.instruccion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GramaticasCQL.Parsers.CQL.ast.entorno
{
    class Procedimiento
    {
        public Procedimiento(LinkedList<Identificador> parametro, LinkedList<Identificador> retorno, Bloque bloque)
        {
            Parametro = parametro;
            Retorno = retorno;
            Bloque = bloque;
        }

        public LinkedList<Identificador> Parametro { get; set; }
        public LinkedList<Identificador> Retorno { get; set; }
        public Bloque Bloque { get; set; }
    }
}
