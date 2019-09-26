using GramaticasCQL.Parsers.CQL.ast.entorno;
using GramaticasCQL.Parsers.CQL.ast.expresion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GramaticasCQL.Parsers.CQL.ast.instruccion
{
    class Return : Instruccion
    {
        public Return(LinkedList<Expresion> exprs, int linea, int columna) : base(linea, columna)
        {
            Exprs = exprs;
            Error = false;
        }

        public Return(int linea, int columna) : base(linea, columna)
        {
            Exprs = null;
            Error = false;
        }

        public LinkedList<Expresion> Exprs { get; set; }
        public LinkedList<Literal> Valores { get; set; }
        public Literal Valor { get; set; }

        public bool Error { get; set; }

        public override object Ejecutar(Entorno e, bool funcion, bool ciclo, bool sw, bool tc, LinkedList<Salida> log, LinkedList<Error> errores)
        {
            if (Exprs != null)
            {
                if (Exprs.Count() == 1)
                {
                    Expresion expr = Exprs.ElementAt(0);
                    object valExpr = expr.GetValor(e, log, errores);
                    if (valExpr != null)
                    {
                        if (valExpr is Throw)
                            return valExpr;

                        Valor = new Literal(expr.Tipo, valExpr, Linea, Columna);
                        return this;
                    }
                    Error = true;
                }
                else
                {
                    Valores = new LinkedList<Literal>();
                    foreach (Expresion expr in Exprs)
                    {
                        object valExpr = expr.GetValor(e, log, errores);
                        if (valExpr != null)
                        {
                            if (valExpr is Throw)
                                return valExpr;

                            Valores.AddLast(new Literal(expr.Tipo, valExpr, Linea, Columna));
                            continue;
                        }
                        Error = true;
                    }
                }
            }

            return this;
        }
    }
}
