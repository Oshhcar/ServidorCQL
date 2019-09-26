using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GramaticasCQL.Parsers.CQL.ast.entorno;
using GramaticasCQL.Parsers.CQL.ast.expresion;

namespace GramaticasCQL.Parsers.CQL.ast.instruccion
{
    class Asignacion : Instruccion
    {
        public Asignacion(Expresion target, Expresion expr, int linea, int columna) : base(linea, columna)
        {
            Target = target;
            Expr = expr;
        }

        public Expresion Target { get; set; }
        public Expresion Expr { get; set; }

        public override object Ejecutar(Entorno e, bool funcion, bool ciclo, bool sw, bool tc, LinkedList<Salida> log, LinkedList<Error> errores)
        {
            Object valExpr = Expr.GetValor(e, log, errores);

            if (valExpr != null)
            {
                if (valExpr is Throw)
                    return valExpr;

                if (Target is Identificador iden)
                {
                    Simbolo sim = Target.GetSimbolo(e);

                    if (sim != null)
                    {
                        if (sim.Tipo.Equals(Expr.Tipo))
                        {
                            sim.Valor = valExpr;
                            return null;
                        }
                        else
                        {
                            if (sim.Tipo.IsCollection() && Expr.Tipo.IsCollection() && iden.IsId2)
                            {
                                if (sim.Tipo.EqualsCollection(Expr.Tipo))
                                {
                                    if (valExpr is Collection collection)
                                    {
                                        sim.Tipo.Clave = collection.Tipo.Clave;
                                        sim.Tipo.Valor = collection.Tipo.Valor;
                                        sim.Valor = collection;
                                        return null;
                                    }
                                }

                            }
                            else
                            {
                                Casteo cast = new Casteo(sim.Tipo, new Literal(Expr.Tipo, valExpr, 0, 0), 0, 0)
                                {
                                    Mostrar = false
                                };
                                valExpr = cast.GetValor(e, log, errores);

                                if (valExpr != null)
                                {
                                    if (valExpr is Throw)
                                        return valExpr;

                                    sim.Valor = valExpr;
                                    return null;
                                }
                            }
                        }

                        errores.AddLast(new Error("Semántico", "El valor no corresponde al tipo de la variable.", Linea, Columna));
                        return null;
                    }
                    if (iden.IsId2)
                        errores.AddLast(new Error("Semántico", "No se ha declarado una variable con el id: " + Target.GetId() + ".", Linea, Columna));
                    else
                        return new Throw("ColumnException", Linea, Columna);

                }
                else if (Target is AtributoRef atributo)
                {
                    atributo.GetObjeto = true;

                    object obj = atributo.GetValor(e, log, errores);

                    if (obj != null)
                    {
                        if (obj is Throw)
                            return obj;

                        Simbolo sim = (Simbolo)obj;

                        if (sim.Tipo.Equals(Expr.Tipo))
                        {
                            sim.Valor = valExpr;
                            return null;
                        }
                        else
                        {
                            Casteo cast = new Casteo(sim.Tipo, new Literal(Expr.Tipo, valExpr, 0, 0), 0, 0)
                            {
                                Mostrar = false
                            };
                            valExpr = cast.GetValor(e, log, errores);

                            if (valExpr != null)
                            {
                                if (valExpr is Throw)
                                    return valExpr;

                                sim.Valor = valExpr;
                                return null;
                            }
                        }

                        errores.AddLast(new Error("Semántico", "El valor no corresponde al tipo de la variable.", Linea, Columna));
                        return null;
                    }
                    errores.AddLast(new Error("Semántico", "No se ha declarado una variable con el id: " + Target.GetId() + ".", Linea, Columna));
                }
                else if (Target is Acceso acceso)
                {
                    acceso.GetCollection = true;

                    object obj = acceso.GetValor(e, log, errores);
                    
                    if (obj != null)
                    {
                        if (obj is Throw)
                            return obj;

                        CollectionValue collection = (CollectionValue)obj;

                        if (acceso.Tipo.Equals(Expr.Tipo))
                        {
                            collection.Valor = valExpr;
                            return null;
                        }
                        else
                        {
                            Casteo cast = new Casteo(acceso.Tipo, new Literal(Expr.Tipo, valExpr, 0, 0), 0, 0)
                            {
                                Mostrar = false
                            };
                            valExpr = cast.GetValor(e, log, errores);

                            if (valExpr != null)
                            {
                                if (valExpr is Throw)
                                    return valExpr;

                                collection.Valor = valExpr;
                                return null;
                            }
                        }

                        errores.AddLast(new Error("Semántico", "El valor no corresponde al tipo de la variable.", Linea, Columna));
                        return null;
                    }
                    errores.AddLast(new Error("Semántico", "No se ha declarado una variable con el id: " + Target.GetId() + ".", Linea, Columna));

                }
            }
            return null;
        }
    }
}
