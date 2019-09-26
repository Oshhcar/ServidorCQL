using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GramaticasCQL.Parsers.CQL.ast.entorno;
using GramaticasCQL.Parsers.CQL.ast.instruccion;

namespace GramaticasCQL.Parsers.CQL.ast.expresion
{
    class Casteo : Expresion
    {
        public Casteo(Tipo tipo, Expresion expr, int linea, int columna) : base(linea, columna)
        {
            Tipo = tipo;
            Expr = expr;
            Mostrar = true;
        }

        public Expresion Expr { get; set; }
        public bool Mostrar { get; set; }

        public override object GetValor(Entorno e, LinkedList<Salida> log, LinkedList<Error> errores)
        {
            object valExpr = Expr.GetValor(e, log, errores);

            if (valExpr != null)
            {
                if (valExpr is Throw)
                    return valExpr;

                if (Expr.Tipo.IsNull())
                {
                    if (Mostrar)
                        return new Throw("NullPointerException", Linea, Columna);
                    return null;
                }

                if (Tipo.IsString())
                {
                    if (Expr.Tipo.IsString() || Expr.Tipo.IsInt() || Expr.Tipo.IsCounter() || Expr.Tipo.IsDouble() || Expr.Tipo.IsTime() || Expr.Tipo.IsDate() || Expr.Tipo.IsCollection())
                    {
                        if (valExpr is Null)
                        {
                            return valExpr;
                            /*
                            if (Mostrar)
                                return new Throw("NullPointerException", Linea, Columna);
                            //errores.AddLast(new Error("Semántico", "El String no ha sido inicializado.", Linea, Columna));
                            //return valExpr;
                            return null;*/
                        }
                        else
                        {
                            Cadena cad = new Cadena
                            {
                                Valor = valExpr.ToString()
                            };
                            return cad;
                        }
                    }
                }
                else if (Tipo.IsInt())
                {
                    if (Expr.Tipo.IsInt() || Expr.Tipo.IsCounter())
                    {
                        return valExpr;
                    }
                    else if (Expr.Tipo.IsDouble())
                    {
                        try
                        {
                            return Convert.ToInt32(Math.Truncate(Convert.ToDouble(valExpr)));
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Exception Casteo: " + ex.Message.ToString());
                        }
                    }
                    else if (Expr.Tipo.IsString())
                    {
                        if (!(valExpr is Null))
                        {
                            if (int.TryParse(valExpr.ToString(), out int i))
                            {
                                return i;
                            }
                        }
                        else
                        {
                            /*
                            if (Mostrar)
                                return new Throw("NullPointerException", Linea, Columna);
                            //errores.AddLast(new Error("Semántico", "El String no ha sido inicializado.", Linea, Columna));
                            */
                        }

                    }
                }
                else if (Tipo.IsDouble())
                {
                    if (Expr.Tipo.IsDouble())
                    {
                        return valExpr;
                    }
                    else if (Expr.Tipo.IsInt() || Expr.Tipo.IsCounter())
                    {
                        return Convert.ToDouble(valExpr);
                    }
                    else if (Expr.Tipo.IsString())
                    {
                        if (!(valExpr is Null))
                        {
                            if (double.TryParse(valExpr.ToString(), out double d))
                            {
                                return d;
                            }
                        }
                        else
                        {
                            /*
                            if (Mostrar)
                                return new Throw("NullPointerException", Linea, Columna);
                            // errores.AddLast(new Error("Semántico", "El String no ha sido inicializado.", Linea, Columna));
                            */
                        }

                    }
                }
                else if (Tipo.IsCounter())
                {
                    if (Expr.Tipo.IsInt() || Expr.Tipo.IsCounter())
                    {
                        return valExpr;
                    }
                    else if (Expr.Tipo.IsDouble())
                    {
                        try
                        {
                            return Convert.ToInt32(Math.Truncate(Convert.ToDouble(valExpr)));
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Exception Casteo: " + ex.Message.ToString());
                        }
                    }
                    else if (Expr.Tipo.IsString())
                    {
                        if (!(valExpr is Null))
                        {
                            if (int.TryParse(valExpr.ToString(), out int i))
                            {
                                return i;
                            }
                        }
                        else
                        {
                            /*
                            if (Mostrar)
                                return new Throw("NullPointerException", Linea, Columna);
                            //errores.AddLast(new Error("Semántico", "El String no ha sido inicializado.", Linea, Columna));
                            */
                        }

                    }
                }
                else if (Tipo.IsDate())
                {
                    if (Expr.Tipo.IsDate())
                    {
                        return valExpr;
                    }
                    else if (Expr.Tipo.IsString())
                    {
                        if (!(valExpr is Null))
                        {
                            Date date = new Date(valExpr.ToString());

                            if (date.Correcto)
                                return date;
                        }
                        else
                        {
                            return valExpr;
                            /*
                            if (Mostrar)
                                return new Throw("NullPointerException", Linea, Columna);
                            //errores.AddLast(new Error("Semántico", "El String no ha sido inicializado.", Linea, Columna));
                            */
                        }
                    }
                }
                else if (Tipo.IsTime())
                {
                    if (Expr.Tipo.IsTime())
                    {
                        return valExpr;
                    }
                    else if (Expr.Tipo.IsString())
                    {
                        if (!(valExpr is Null))
                        {
                            Time time = new Time(valExpr.ToString());

                            if (time.Correcto)
                                return time;
                        }
                        else
                        {
                            return valExpr;
                            /*
                            if (Mostrar)
                                return new Throw("NullPointerException", Linea, Columna);
                            //errores.AddLast(new Error("Semántico", "El String no ha sido inicializado.", Linea, Columna));
                            //return valExpr;
                            return null;*/

                        }
                    }
                }
                else if (Tipo.IsCollection())
                {
                    if (Expr.Tipo.IsCollection())
                    {
                        if(Tipo.EqualsCollection(Expr.Tipo))
                        {
                            if (!(valExpr is Null))
                            {
                                Collection collection = (Collection)valExpr;

                                if (collection.Castear(Tipo, e, log, errores))
                                    return collection;
                            }
                            else
                            {
                                return valExpr;
                                /*
                                if (Mostrar)
                                    return new Throw("NullPointerException", Linea, Columna);
                                return null;*/
                            }
                        }
                    }
                }

                if(Mostrar)
                    errores.AddLast(new Error("Semántico", "No se pudo castear la expresión a " + Tipo.Type.ToString() +".", Linea, Columna));
            }

            Tipo = null;
            return null;
        }
    }
}
