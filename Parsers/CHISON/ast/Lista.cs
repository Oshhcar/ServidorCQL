using GramaticasCQL.Parsers.CQL.ast.entorno;
using GramaticasCQL.Parsers.CQL.ast.expresion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Type = GramaticasCQL.Parsers.CQL.ast.entorno.Type;

namespace GramaticasCQL.Parsers.CHISON.ast
{
    class Lista : Expresion
    {
        public Lista(LinkedList<Expresion> valores, int linea, int columna) : base(linea, columna)
        {
            Valores = valores;
        }

        public LinkedList<Expresion> Valores { get; set; }

        public override object GetValor(Entorno e, LinkedList<Salida> log, LinkedList<Error> errores)
        {
            Tipo = new Tipo(Type.LIST);
            return Valores;
        }
    }
}
