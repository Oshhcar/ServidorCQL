using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GramaticasCQL.Parsers.CQL.ast.entorno;
using GramaticasCQL.Parsers.CQL.ast.instruccion;

namespace GramaticasCQL.Parsers.CQL.ast.expresion
{
    class Call : Expresion
    {
        public Call(string id, int linea, int columna) : base(linea, columna)
        {
            Id = id;
            Parametro = null;
        }

        public Call(string id, LinkedList<Expresion> parametro, int linea, int columna) : base(linea, columna)
        {
            Id = id;
            Parametro = parametro;
        }

        public string Id { get; set; }
        public LinkedList<Expresion> Parametro { get; set; }
        public LinkedList<Literal> Parametros { get; set; }
        public Simbolo Simbolo { get; set; }

        public object GetSimbolo(Entorno e, LinkedList<Salida> log, LinkedList<Error> errores)
        {
            BD actual = e.Master.Actual;

            if (actual != null)
            {
                string firma = Id;

                Parametros = new LinkedList<Literal>();

                if (Parametro != null)
                {
                    foreach (Expresion expr in Parametro)
                    {
                        object valExpr = expr.GetValor(e, log, errores);
                        if (valExpr != null)
                        {
                            if (valExpr is Throw)
                                return valExpr;

                            firma += "-" + expr.Tipo.Type.ToString();
                            Parametros.AddLast(new Literal(expr.Tipo, valExpr, 0, 0));
                            continue;
                        }
                        return null;
                    }
                }

                Simbolo = actual.GetProcedimiento(firma);

                if(Simbolo == null)
                    errores.AddLast(new Error("Semántico", "No se ha guardado un Procedimiento con la firma: " + firma.ToLower() + ".", Linea, Columna));

                return Simbolo;
            }
            else
                errores.AddLast(new Error("Semántico", "No se ha seleccionado una base de datos, no se pudo guardar el Procedimiento.", Linea, Columna));

            return null;
        }

        public override object GetValor(Entorno e, LinkedList<Salida> log, LinkedList<Error> errores)
        {
            Entorno local = new Entorno(e.Global); /*********/

            Simbolo sim;

            if (Simbolo != null)
                sim = Simbolo;
            else
            {
                 object obj =  GetSimbolo(e, log, errores);

                if (obj is Throw)
                    return obj;

                sim = (Simbolo)obj;
            }

            if (sim != null)
            {
                Procedimiento proc = (Procedimiento)sim.Valor;

                if (Parametro != null)
                {
                    for (int i = 0; i < Parametros.Count(); i++)
                    {
                        local.Add(new Simbolo(Parametros.ElementAt(i).Tipo, Rol.VARIABLE, proc.Parametro.ElementAt(i).Id, Parametros.ElementAt(i).Valor));
                    }
                }

                object obj = proc.Bloque.Ejecutar(local, true, false, false, false, log, errores);

                if (obj != null)
                {
                    if (obj is Return ret)
                    {
                        if (!ret.Error)
                        {
                            if (proc.Retorno != null)
                            {
                                if (ret.Valor != null)
                                {
                                    if (proc.Retorno.Count() == 1)
                                    {
                                        if (ret.Valor.Tipo.IsNull())
                                        {
                                            if (proc.Retorno.ElementAt(0).Tipo.IsNullable())
                                            {
                                                LinkedList<Literal> valores = new LinkedList<Literal>();
                                                valores.AddLast(ret.Valor);
                                                return valores;
                                            }
                                        }

                                        if (ret.Valor.Tipo.Equals(proc.Retorno.ElementAt(0).Tipo))
                                        {
                                            LinkedList<Literal> valores = new LinkedList<Literal>();
                                            valores.AddLast(ret.Valor);
                                            return valores;
                                        }
                                        else
                                        {
                                            /*Agregar collections*/
                                            if (ret.Valor.Tipo.IsCollection() && proc.Retorno.ElementAt(0).Tipo.IsCollection())
                                            {
                                                if (ret.Valor.Tipo.EqualsCollection(proc.Retorno.ElementAt(0).Tipo))
                                                {
                                                    LinkedList<Literal> valores = new LinkedList<Literal>();
                                                    valores.AddLast(ret.Valor);
                                                    return valores;
                                                }

                                            }
                                            else
                                            {
                                                Casteo cast = new Casteo(proc.Retorno.ElementAt(0).Tipo, ret.Valor, 0, 0)
                                                {
                                                    Mostrar = false
                                                };

                                                object valExpr = cast.GetValor(e, log, errores);

                                                if (valExpr != null)
                                                {
                                                    if (!(valExpr is Throw))
                                                    {
                                                        LinkedList<Literal> valores = new LinkedList<Literal>();
                                                        valores.AddLast(new Literal(cast.Tipo, valExpr, 0, 0));
                                                        return valores;
                                                    }
                                                }
                                            }

                                            errores.AddLast(new Error("Semántico", "Los Tipos de los valores del Return no coinciden con los del Procedimiento.", ret.Linea, ret.Columna));
                                            return null;
                                        }
                                    }
                                    else
                                        return new Throw("NumberReturnsException", Linea, Columna);
                                        //errores.AddLast(new Error("Semántico", "Los valores del Return no coinciden con los del Procedimiento.", ret.Linea, ret.Columna));
                                }
                                else if (ret.Valores != null)
                                {
                                    if (proc.Retorno.Count() == ret.Valores.Count())
                                    {
                                        for (int i = 0; i < proc.Retorno.Count(); i++)
                                        {
                                            if (!proc.Retorno.ElementAt(i).Tipo.Equals(ret.Valores.ElementAt(i).Tipo))
                                            {
                                                Casteo cast = new Casteo(proc.Retorno.ElementAt(0).Tipo, ret.Valores.ElementAt(i), 0, 0)
                                                {
                                                    Mostrar = false
                                                };

                                                object valExpr = cast.GetValor(e, log, errores);

                                                if (valExpr != null)
                                                {
                                                    ret.Valores.ElementAt(i).Tipo = cast.Tipo;
                                                    ret.Valores.ElementAt(i).Valor = valExpr;
                                                }
                                                else
                                                {
                                                    errores.AddLast(new Error("Semántico", "Los Tipos de los valores del Return no coinciden con los del Procedimiento.", ret.Linea, ret.Columna));
                                                    return null;
                                                }
                                            }
                                        }
                                        return ret.Valores;
                                    }
                                    else
                                        return new Throw("NumberReturnsException", Linea, Columna);
                                    //errores.AddLast(new Error("Semántico", "Los valores del Return no coinciden con los del Procedimiento.", ret.Linea, ret.Columna));
                                }
                                else
                                    return new Throw("NumberReturnsException", Linea, Columna);
                                //errores.AddLast(new Error("Semántico", "Se esperaba valor en Return.", ret.Linea, ret.Columna));
                            }
                            else
                            {
                                if (ret.Valor != null || ret.Valores != null)
                                    errores.AddLast(new Error("Semántico", "No se esperaba valor en Return.", ret.Linea, ret.Columna));
                            }
                        }
                        return null;
                    }

                    if (obj is Throw)
                        return obj;
                }

                if(proc.Retorno != null)
                    if(proc.Retorno.Count()>0)
                        errores.AddLast(new Error("Semántico", "Se esperaba Return en Procedimiento.", Linea, Columna));

                return null;
            }

            return null;
        }
    }
}
