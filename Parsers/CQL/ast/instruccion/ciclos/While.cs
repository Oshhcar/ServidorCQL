using GramaticasCQL.Parsers.CQL.ast.entorno;
using GramaticasCQL.Parsers.CQL.ast.expresion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GramaticasCQL.Parsers.CQL.ast.instruccion.ciclos
{
    class While : Instruccion
    {
        public While(Expresion expr, Bloque bloque, int linea, int columna) : base(linea, columna)
        {
            Expr = expr;
            Bloque = bloque;
        }

        public Expresion Expr { get; set; }
        public Bloque Bloque { get; set; }
        public override object Ejecutar(Entorno e, bool funcion, bool ciclo, bool sw, bool tc, LinkedList<Salida> log, LinkedList<Error> errores)
        {
            object valExpr = Expr.GetValor(e, log, errores);

            if(valExpr != null)
            {
                if (valExpr is Throw)
                    return valExpr;

                if (Expr.Tipo.IsBoolean())
                {
                    bool condicion = (bool)valExpr;

                    while (condicion)
                    {
                        object obj = Bloque.Ejecutar(e, funcion, true, sw, tc, log, errores);

                        if (obj is Break)
                            break;
                        else if (obj is Return)
                            return obj;
                        else if (obj is Throw)
                            return obj;

                        valExpr = Expr.GetValor(e, log, errores);

                        if (valExpr != null)
                        {
                            if (valExpr is Throw)
                                return valExpr;

                            if (Expr.Tipo.IsBoolean())
                            {
                                condicion = (bool)valExpr;
                                continue;
                            }
                            errores.AddLast(new Error("Semántico", "Se esperaba un booleano en condicion while.", Linea, Columna));
                        }
                        break;
                    }
                    return null;
                }

                errores.AddLast(new Error("Semántico", "Se esperaba un booleano en condicion while.", Linea, Columna));
            }

            return null;
        }
    }
}
