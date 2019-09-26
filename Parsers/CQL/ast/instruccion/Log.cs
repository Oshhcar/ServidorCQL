using GramaticasCQL.Parsers.CQL.ast.entorno;
using GramaticasCQL.Parsers.CQL.ast.expresion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GramaticasCQL.Parsers.CQL.ast.instruccion
{
    class Log : Instruccion
    {
        public Log(Expresion expr, int linea, int columna) : base(linea, columna)
        {
            Expr = expr;
        }

        public Expresion Expr { get; set; }

        public override object Ejecutar(Entorno e, bool funcion, bool ciclo, bool sw, bool tc, LinkedList<Salida> log, LinkedList<Error> errores)
        {
            object valorExpr = Expr.GetValor(e, log, errores);

            if (valorExpr is Throw)
                return valorExpr;

            if (valorExpr != null)
                log.AddLast(new Salida(1, valorExpr.ToString()));

            e.Master.Recorrer();
            //e.Recorrer();
            return null;
        }
    }
}
