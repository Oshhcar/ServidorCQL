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
    class Relacional : Operacion
    {
        public Relacional(Expresion op1, Expresion op2, Operador op, int linea, int columna) : base(op1, op2, op, linea, columna) { }

        public override object GetValor(Entorno e, LinkedList<Salida> log, LinkedList<Error> errores)
        {
            object valOp1 = Op1.GetValor(e, log, errores);
            object valOp2 = Op2.GetValor(e, log, errores);

            if (valOp1 != null && valOp2 != null)
            {
                if (valOp1 is Throw)
                    return valOp1;

                if (valOp2 is Throw)
                    return valOp2;

                Tipo = new Tipo(Type.BOOLEAN);

                if (Op1.Tipo.IsNumeric() && Op2.Tipo.IsNumeric())
                {
                    double op1 = Convert.ToDouble(valOp1);
                    double op2 = Convert.ToDouble(valOp2);

                    switch (Op)
                    {
                        case Operador.MAYORQUE:
                            return op1 > op2;
                        case Operador.MENORQUE:
                            return op1 < op2;
                        case Operador.IGUAL:
                            return op1 == op2;
                        case Operador.MENORIGUAL:
                            return op1 <= op2;
                        case Operador.MAYORIGUAL:
                            return op1 >= op2;
                        case Operador.DIFERENTE:
                            return op1 != op2;
                    }
                }
                else if (Op1.Tipo.IsDate() && Op2.Tipo.IsDate())
                {
                    Date op1 = (Date)valOp1;
                    Date op2 = (Date)valOp2;

                    int res = op1.CompareTo(op2);

                    switch (Op)
                    {
                        case Operador.MAYORQUE:
                            return res == 1;
                        case Operador.MENORQUE:
                            return res == -1;
                        case Operador.IGUAL:
                            return res == 0;
                        case Operador.MENORIGUAL:
                            return res == -1 || res == 0;
                        case Operador.MAYORIGUAL:
                            return res == 1 || res == 0;
                        case Operador.DIFERENTE:
                            return !(res == 0);
                    }

                }
                else if (Op1.Tipo.IsTime() && Op2.Tipo.IsTime())
                {
                    Time op1 = (Time)valOp1;
                    Time op2 = (Time)valOp2;

                    int res = op1.CompareTo(op2);

                    switch (Op)
                    {
                        case Operador.MAYORQUE:
                            return res == 1;
                        case Operador.MENORQUE:
                            return res == -1;
                        case Operador.IGUAL:
                            return res == 0;
                        case Operador.MENORIGUAL:
                            return res == -1 || res == 0;
                        case Operador.MAYORIGUAL:
                            return res == 1 || res == 0;
                        case Operador.DIFERENTE:
                            return !(res == 0);
                    }
                }
                else if ((Op1.Tipo.IsBoolean() && Op2.Tipo.IsBoolean()) || (Op1.Tipo.IsString() && Op2.Tipo.IsString()))
                {
                    if (Op == Operador.IGUAL)
                        return valOp1.ToString().Equals(valOp2.ToString());
                    else if (Op == Operador.DIFERENTE)
                        return !valOp1.ToString().Equals(valOp2.ToString());

                    errores.AddLast(new Error("Semántico", "Con " + Op1.Tipo.Type + " solo se puede operar el igual y diferente.", Linea, Columna));
                    return null;
                }
                else if (Op1.Tipo.IsNull() || Op2.Tipo.IsNull())
                {
                    if (Op1.Tipo.IsNull() && Op2.Tipo.IsNullable())
                    {
                        if (Op == Operador.IGUAL)
                            return valOp2 is Null;
                        else if (Op == Operador.DIFERENTE)
                            return !(valOp2 is Null);

                        errores.AddLast(new Error("Semántico", "Con Null solo se puede operar el igual y diferente.", Linea, Columna));
                        return null;
                    }
                    else if (Op2.Tipo.IsNull() && Op1.Tipo.IsNullable())
                    {
                        if (Op == Operador.IGUAL)
                            return valOp1 is Null;
                        else if (Op == Operador.DIFERENTE)
                            return !(valOp1 is Null);

                        errores.AddLast(new Error("Semántico", "Con Null solo se puede operar el igual y diferente.", Linea, Columna));
                        return null;
                    }
                }

                errores.AddLast(new Error("Semántico", "No se pudo realizar la operación relacional.", Linea, Columna));
            }

            return null;
        }
    }
}
