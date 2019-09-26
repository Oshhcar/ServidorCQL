using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GramaticasCQL.Parsers.CQL.ast.entorno;
using GramaticasCQL.Parsers.CQL.ast.instruccion;

namespace GramaticasCQL.Parsers.CQL.ast.expresion
{
    class Ternario : Expresion
    {
        public Ternario(Expresion expr, Expresion v, Expresion f, int linea, int columna) : base(linea, columna)
        {
            Expr = expr;
            V = v;
            F = f;
        }

        public Expresion Expr { get; set; }
        public Expresion V { get; set; }
        public Expresion F { get; set; }

        public override object GetValor(Entorno e, LinkedList<Salida> log, LinkedList<Error> errores)
        {
            object valExpr = Expr.GetValor(e, log, errores);

            if (valExpr != null)
            {
                if (valExpr is Throw)
                    return valExpr;

                if (Expr.Tipo.IsBoolean())
                {
                    if ((Boolean)valExpr)
                    {
                        object valV = V.GetValor(e, log, errores);

                        if (valV != null)
                        {
                            if (valV is Throw)
                                return valV;

                            Tipo = V.Tipo;
                            return valV;
                        }
                    }
                    else
                    {
                        object valF = F.GetValor(e, log, errores);

                        if (valF != null)
                        {
                            if (valF is Throw)
                                return valF;

                            Tipo = F.Tipo;
                            return valF;
                        }
                    }
                }
                else
                { 
                    errores.AddLast(new Error("Semántico", "Se esperaba un booleano en expresion ternario.", Linea, Columna));
                }
            }
            return null;
        }
    }
}
