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
    class FuncionCall : Expresion
    {
        public FuncionCall(string id, int linea, int columna) : base(linea, columna)
        {
            Id = id;
            Parametro = null;
            IsExpresion = true;
        }

        public FuncionCall(string id, LinkedList<Expresion> parametro, int linea, int columna) : base(linea, columna)
        {
            Id = id;
            Parametro = parametro;
            IsExpresion = true;
        }

        public string Id { get; set; }
        public LinkedList<Expresion> Parametro { get; set; }
        public bool IsExpresion { get; set; }

        public override object GetValor(Entorno e, LinkedList<Salida> log, LinkedList<Error> errores)
        {
            string firma = Id;
            Entorno local = new Entorno(e.Global);

            LinkedList<Literal> parametros = new LinkedList<Literal>();

            if (Id.ToLower().Equals("today"))
            {
                if(Parametro != null)
                    errores.AddLast(new Error("Semántico", "La función nativa today no necesita parámetros.", Linea, Columna));

                Tipo = new Tipo(Type.DATE);
                DateTime date = DateTime.Now;
                return new Date(date.Year+"-"+date.Month+"-"+date.Day);
            }
            else if (Id.ToLower().Equals("now"))
            {
                if (Parametro != null)
                    errores.AddLast(new Error("Semántico", "La función nativa now no necesita parámetros.", Linea, Columna));

                Tipo = new Tipo(Type.TIME);
                DateTime date = DateTime.Now;
                return new Time(date.Hour + ":" + date.Minute + ":" + date.Second);
            }

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
                        parametros.AddLast(new Literal(expr.Tipo, valExpr, 0, 0));
                        continue;
                    }
                    return null;
                }
            }

            Simbolo sim = e.GetFuncion(firma);

            if (sim != null)
            {
                Funcion fun = (Funcion)sim.Valor;

                if (Parametro != null)
                {
                    for (int i = 0; i < parametros.Count(); i++)
                    {
                        local.Add(new Simbolo(parametros.ElementAt(i).Tipo, Rol.VARIABLE, fun.Parametro.ElementAt(i).Id, parametros.ElementAt(i).Valor));
                    }
                }
                object obj = fun.Bloque.Ejecutar(local, true, false, false, false, log, errores);

                if (obj != null)
                {
                    if(obj is Return ret)
                    {
                        if (!ret.Error)
                        {
                            if (ret.Valores == null)
                            {
                                if (ret.Valor != null)
                                {
                                    Tipo = ret.Valor.Tipo;

                                    if (Tipo.IsNull())
                                    {
                                        if (sim.Tipo.IsNullable())
                                        {
                                            return ret.Valor.Valor;
                                        }
                                            
                                    }

                                    if (Tipo.Equals(sim.Tipo))
                                    {
                                        return ret.Valor.Valor;
                                    }
                                    else
                                    {
                                        if (sim.Tipo.IsCollection() && ret.Valor.Tipo.IsCollection())
                                        {
                                            if (sim.Tipo.EqualsCollection(ret.Valor.Tipo))
                                            {
                                                return ret.Valor.Valor;
                                            }

                                        }
                                        else
                                        {
                                            Casteo cast = new Casteo(sim.Tipo, ret.Valor, 0, 0)
                                            {
                                                Mostrar = false
                                            };

                                            object valCast = cast.GetValor(e, log, errores);

                                            if (valCast != null)
                                            {
                                                if (!(valCast is Throw))
                                                {
                                                    Tipo = cast.Tipo;
                                                    return valCast;
                                                }
                                            }
                                        }

                                        errores.AddLast(new Error("Semántico", "Se esperaba un valor en return de tipo: " + sim.Tipo.Type.ToString() + ".", ret.Linea, ret.Columna));
                                        return null;
                                    }
                                }
                                else
                                    errores.AddLast(new Error("Semántico", "Se esperaba un valor en return.", ret.Linea, ret.Columna));
                            }
                            else
                                errores.AddLast(new Error("Semántico", "No se esperaba una lista de expresiones en return.", ret.Linea, ret.Columna));
                        }
                        return null;
                    }

                    if (obj is Throw)
                        return obj;
                }

                if(IsExpresion)
                    errores.AddLast(new Error("Semántico", "Se esperaba un return en funcion " + Id +".", Linea, Columna));
                return null;
            }

            errores.AddLast(new Error("Semántico", "No se ha definico una funcion con la firma: " + firma.ToLower() + ".", Linea, Columna));
            return null;
        }
    }
}
