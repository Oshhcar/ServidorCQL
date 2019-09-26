using GramaticasCQL.Parsers.CQL.ast.entorno;
using GramaticasCQL.Parsers.CQL.ast.expresion;
using GramaticasCQL.Parsers.CQL.ast.instruccion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GramaticasCQL.Parsers.CHISON.ast
{
    class Atributo : Instruccion
    {
        public Atributo(Cadena id, Expresion valor, int linea, int columna) : base(linea, columna)
        {
            Id = id;
            Valor = valor;
        }

        public Cadena Id { get; set; }
        public Expresion Valor { get; set; }

        public override object Ejecutar(Entorno e, bool funcion, bool ciclo, bool sw, bool tc, LinkedList<Salida> log, LinkedList<Error> errores)
        {
            return null;
        }
    }
}
