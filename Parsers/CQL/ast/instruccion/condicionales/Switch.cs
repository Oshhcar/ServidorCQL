using GramaticasCQL.Parsers.CQL.ast.entorno;
using GramaticasCQL.Parsers.CQL.ast.expresion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GramaticasCQL.Parsers.CQL.ast.instruccion.condicionales
{
    class Switch : Instruccion
    {
        public Switch(Expresion expr, LinkedList<Case> cases, int linea, int columna) : base(linea, columna)
        {
            Expr = expr;
            Cases = cases;
        }

        public Expresion Expr { get; set; }
        public LinkedList<Case> Cases { get; set; }

        public override object Ejecutar(Entorno e, bool funcion, bool ciclo, bool sw, bool tc, LinkedList<Salida> log, LinkedList<Error> errores)
        {
            object valExp = Expr.GetValor(e, log, errores);

            if (valExp != null)
            {
                if (valExp is Throw)
                    return valExp;

                bool continuar = false;
                Expresion exprSwitch = new Literal(Expr.Tipo, valExp, Linea, Columna);

                foreach(Case caso in Cases)
                {
                    caso.ExprSwitch = exprSwitch;
                    caso.Continuar = continuar;

                    object obj = caso.Ejecutar(e, funcion, ciclo, true, tc, log, errores);
                    continuar = caso.Continuar;

                    if (obj != null)
                    {
                        if (obj is Break)
                            return null;//continuar = false;
                        else
                            return obj;
                    }
                }
            }
            return null;
        }
    }
}
