using GramaticasCQL.Parsers.CQL.ast.entorno;
using GramaticasCQL.Parsers.CQL.ast.expresion;
using GramaticasCQL.Parsers.CQL.ast.expresion.operacion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GramaticasCQL.Parsers.CQL.ast.instruccion
{
    class AsignacionOperacion : Instruccion
    {
        public AsignacionOperacion(Expresion target, Operador op, Expresion expr, int linea, int columna) : base(linea, columna)
        {
            Target = target;
            Op = op;
            Expr = expr;
        }

        public Expresion Target { get; set; }
        public Operador Op { get; set; }
        public Expresion Expr { get; set; }

        public override object Ejecutar(Entorno e, bool funcion, bool ciclo, bool sw, bool tc, LinkedList<Salida> log, LinkedList<Error> errores)
        {
            object valExpr = Expr.GetValor(e, log, errores);

            if (valExpr != null)
            {
                if (valExpr is Throw)
                    return valExpr;

                Simbolo sim = Target.GetSimbolo(e);

                if (sim != null)
                {
                    Aritmetica arit = new Aritmetica(new Literal(sim.Tipo, sim.Valor, Linea, Columna), new Literal(Expr.Tipo, valExpr, Linea, Columna), Op, Linea, Columna);
                    object valArit = arit.GetValor(e, log, errores);

                    if (valArit != null)
                    {
                        if (valArit is Throw)
                            return valArit;

                        sim.Valor = valArit;
                    }

                    return null;
                }
                errores.AddLast(new Error("Semántico", "No hay una variabla declarada con el id: " + Target.GetId() + ".", Linea, Columna));
            }
            return null;
        }
    }
}
