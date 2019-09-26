using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GramaticasCQL.Parsers.CQL.ast.entorno;
using GramaticasCQL.Parsers.CQL.ast.instruccion;
using Type = GramaticasCQL.Parsers.CQL.ast.entorno.Type;

namespace GramaticasCQL.Parsers.CQL.ast.expresion
{
    class Where : Expresion
    {
        public Where(Expresion expr, int linea, int columna) : base(linea, columna)
        {
            Expr = expr;
        }

        public Where(Expresion expr, LinkedList<Expresion> inExpr, int linea, int columna) : base(linea, columna)
        {
            Expr = expr;
            InExpr = inExpr;
        }

        public Expresion Expr { get; set; }
        public LinkedList<Expresion> InExpr { get; set; }

        public override object GetValor(Entorno e, LinkedList<Salida> log, LinkedList<Error> errores)
        {
            object valExpr = Expr.GetValor(e, log, errores);

            if (valExpr != null)
            {
                if (valExpr is Throw)
                    return valExpr;

                if (InExpr == null)
                {
                    Tipo = Expr.Tipo;
                    return valExpr;
                }
                else
                {
                    Tipo = new Tipo(Type.BOOLEAN);

                    foreach (Expresion exprActual in InExpr)
                    {
                        object valExprActual = exprActual.GetValor(e, log, errores);

                        if (valExprActual != null)
                        {
                            if (valExprActual is Throw)
                                return valExprActual;

                            if (exprActual.Tipo.IsCollection())
                            {
                                Collection collection = (Collection)valExprActual;
                                if (Expr.Tipo.Equals(collection.Tipo.Valor))
                                {
                                    foreach (CollectionValue value in collection.Valores)
                                    {
                                        if (valExpr.Equals(value.Valor))
                                            return true;
                                    }
                                }
                            }
                            else
                            {
                                if(Expr.Tipo.Equals(exprActual.Tipo))
                                    if(valExpr.Equals(valExprActual))
                                        return true;
                            }
                        }
                    }
                    return false;
                }
            }
            return null;
        }
    }
}
