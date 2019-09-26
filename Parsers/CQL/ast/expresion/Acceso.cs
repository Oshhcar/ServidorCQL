using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GramaticasCQL.Parsers.CQL.ast.entorno;
using GramaticasCQL.Parsers.CQL.ast.instruccion;

namespace GramaticasCQL.Parsers.CQL.ast.expresion
{
    class Acceso : Expresion
    {
        public Acceso(Expresion target, Expresion expr, int linea, int columna) : base(linea, columna)
        {
            Target = target;
            Expr = expr;
            GetCollection = false;
        }

        public Expresion Target { get; set; }
        public Expresion Expr { get; set; }
        public bool GetCollection { get; set; }

        public override object GetValor(Entorno e, LinkedList<Salida> log, LinkedList<Error> errores)
        {
            object valExpr = Expr.GetValor(e, log, errores);

            if (valExpr != null)
            {
                if (valExpr is Throw)
                    return valExpr;

                object valTarget = Target.GetValor(e, log, errores);

                if (valTarget != null)
                {
                    if (valTarget is Throw)
                        return valTarget;

                    if (Target.Tipo.IsMap() || Target.Tipo.IsList() || Target.Tipo.IsSet())
                    {
                        if (!(valTarget is Null))
                        {
                            Collection collection = (Collection)valTarget;

                            if (collection.Tipo.Clave.Equals(Expr.Tipo))
                            {
                                object valor = GetCollection ? collection.GetCollection(valExpr.ToString()) : collection.Get(valExpr.ToString());
                                if (valor != null)
                                {
                                    Tipo = collection.Tipo.Valor;
                                    return valor;
                                }
                                else
                                    return new Throw("IndexOutException", Linea, Columna);
                                //errores.AddLast(new Error("Semántico", "No existe un valor en la posición: " + valExpr.ToString() + " del " + Target.Tipo.Type.ToString() + ".", Linea, Columna));
                            }
                            else
                            {
                                Casteo cast = new Casteo(collection.Tipo.Clave, new Literal(Expr.Tipo, valExpr, 0, 0), 0, 0)
                                {
                                    Mostrar = false
                                };
                                valExpr = cast.GetValor(e, log, errores);

                                if (valExpr != null)
                                {
                                    if (valExpr is Throw)
                                        return valExpr;

                                    object valor = GetCollection ? collection.GetCollection(valExpr.ToString()) : collection.Get(valExpr.ToString());
                                    if (valor != null)
                                    {
                                        Tipo = collection.Tipo.Valor;
                                        return valor;
                                    }
                                    else
                                        return new Throw("IndexOutException", Linea, Columna);
                                }

                                errores.AddLast(new Error("Semántico", "El tipo de la clave no coincide con la clave de la Collection.", Linea, Columna));
                                return null;
                            }
                        }
                        else
                            return new Throw("NullPointerException", Linea, Columna);
                            //errores.AddLast(new Error("Semántico", "El " + Target.Tipo.Type.ToString() + " no ha sido inicializado.", Linea, Columna));
                    }
                    else
                        errores.AddLast(new Error("Semántico", "La variable debe ser de tipo Map, List o Set.", Linea, Columna));
                }
            }

            return null;
        }

        public override string GetId()
        {
            return Target.GetId();
        }
    }
}
