using GramaticasCQL.Parsers.CQL.ast.entorno;
using GramaticasCQL.Parsers.CQL.ast.expresion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GramaticasCQL.Parsers.CQL.ast.instruccion
{
    class AsignacionCall : Instruccion
    {
        public AsignacionCall(LinkedList<Expresion> target, Call call, int linea, int columna) : base(linea, columna)
        {
            Target = target;
            Call = call;
        }

        public LinkedList<Expresion> Target { get; set; }
        public Call Call { get; set; }

        public override object Ejecutar(Entorno e, bool funcion, bool ciclo, bool sw, bool tc, LinkedList<Salida> log, LinkedList<Error> errores)
        {
            object obj = Call.GetSimbolo(e, log, errores);

            if (obj == null)
                return null;

            if (obj is Throw)
                return obj;

            Simbolo sim = (Simbolo)obj;

            Procedimiento proc = (Procedimiento)sim.Valor;

            if (proc.Retorno != null)
            {
                if (proc.Retorno.Count() == Target.Count())
                {
                    obj = Call.GetValor(e, log, errores);

                    if (obj != null)
                    {
                        if (obj is Throw)
                            return obj;

                        LinkedList<Literal> valores = (LinkedList<Literal>)obj;

                        for (int i = 0; i < Target.Count(); i++)
                        {
                            Expresion target = Target.ElementAt(i);
                            Literal valor = valores.ElementAt(i);

                            if (target is Identificador iden)
                            {
                                Simbolo simIden = iden.GetSimbolo(e);

                                if (simIden != null)
                                {
                                    if (valor.Tipo.IsNull())
                                    {
                                        if (simIden.Tipo.IsNullable())
                                        {
                                            simIden.Valor = valor.Valor;
                                            continue;
                                        }
                                    }

                                    if (simIden.Tipo.Equals(valor.Tipo))
                                    {
                                        simIden.Valor = valor.Valor;
                                    }
                                    else
                                    {
                                        Casteo cast = new Casteo(simIden.Tipo, valor, 0, 0)
                                        {
                                            Mostrar = false
                                        };

                                        object valExpr = cast.GetValor(e, log, errores);

                                        if (valExpr != null)
                                        {
                                            if (valExpr is Throw)
                                                return valExpr;

                                            simIden.Valor = valExpr;
                                        }
                                        else
                                            errores.AddLast(new Error("Semántico", "El tipo del Return no coincide con el de la variable: " + target.GetId() + ".", Linea, Columna));
                                    }
                                }
                                else
                                    errores.AddLast(new Error("Semántico", "No se ha declarado una variable con el id: " + target.GetId() + ".", Linea, Columna));
                            }
                            else
                                errores.AddLast(new Error("Semántico", "Solo se pueden capturar el Return en variables.", Linea, Columna));
                        }
                    }
                }
                else
                    errores.AddLast(new Error("Semántico", "Los valores que retorna el Procedimiento no coinciden con la asignación.", Linea, Columna));
            }
            else
                errores.AddLast(new Error("Semántico", "El procedimiento no retorna valores.", Linea, Columna));

            return null;
        }
    }
}
