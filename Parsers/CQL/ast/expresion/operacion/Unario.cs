using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GramaticasCQL.Parsers.CQL.ast.entorno;
using GramaticasCQL.Parsers.CQL.ast.instruccion;
using Type = GramaticasCQL.Parsers.CQL.ast.entorno.Type;

namespace GramaticasCQL.Parsers.CQL.ast.expresion.operacion
{
    class Unario : Operacion
    {
        public Unario(Expresion op1, Operador op, int linea, int columna) : base(op1, null, op, linea, columna) { }

        public override object GetValor(Entorno e, LinkedList<Salida> log, LinkedList<Error> errores)
        {
            if (Op == Operador.AUMENTO || Op == Operador.DECREMENTO)
            {
                Simbolo sim = Op1.GetSimbolo(e);

                if (sim != null)
                {
                    if (sim.Tipo.IsNumeric())
                    {
                        Operador op = Op == Operador.AUMENTO ? Operador.SUMA : Operador.RESTA;
                        Aritmetica arit = new Aritmetica(new Literal(sim.Tipo, sim.Valor, Linea, Columna), new Literal(new Tipo(Type.INT), 1, Linea, Columna), op, Linea, Columna);

                        object valAnt = sim.Valor;
                        object valArit = arit.GetValor(e, log, errores);

                        if (valArit != null)
                        {
                            if (valArit is Throw)
                                return valArit;

                            sim.Valor = valArit;
                            Tipo = sim.Tipo;
                            return valAnt;
                        }
                        return null;
                    }
                    errores.AddLast(new Error("Semántico", "El operando no es numérico.", Linea, Columna));
                    return null;
                }

                if (Op1.GetId() != null)
                {
                    errores.AddLast(new Error("Semántico", "No hay una variabla declarada con el id: " + Op1.GetId() + ".", Linea, Columna));
                }
                else
                {
                    errores.AddLast(new Error("Semántico", "El operando no es una variable.", Linea, Columna));
                }
            }
            else
            {
                Object valOp1 = Op1.GetValor(e, log, errores);

                if (valOp1 != null)
                {
                    if (valOp1 is Throw)
                        return valOp1;

                    if (Op1.Tipo.IsNumeric())
                    {
                        object factor = Op == Operador.SUMA ? 1 : -1;
                        Aritmetica arit = new Aritmetica(new Literal(Op1.Tipo, valOp1, Linea, Columna), new Literal(new Tipo(Type.INT), factor, Linea, Columna), Operador.MULTIPLICACION, Linea, Columna);
                        object valor = arit.GetValor(e, log, errores);
                        Tipo = arit.Tipo;
                        return valor;
                    }
                    errores.AddLast(new Error("Semántico", "El operando no es numérico.", Linea, Columna));
                }
            }
            return null;
        }
    }
}
