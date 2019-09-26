using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GramaticasCQL.Parsers.CQL.ast.entorno;
using GramaticasCQL.Parsers.CQL.ast.expresion;

namespace GramaticasCQL.Parsers.CQL.ast.instruccion.ddl
{
    class Eliminar : Instruccion
    {
        public Eliminar(string id, int linea, int columna) : base(linea, columna)
        {
            Id = id;
        }

        public Eliminar(string id, Where where, int linea, int columna) : base(linea, columna)
        {
            Id = id;
            Where = where;
        }

        public Eliminar(Expresion target, string id, int linea, int columna) : base(linea, columna)
        {
            Target = target;
            Id = id;
        }

        public Eliminar(Expresion target, string id, Where where, int linea, int columna) : base(linea, columna)
        {
            Target = target;
            Id = id;
            Where = where;
        }


        public string Id { get; set; }
        public Where Where { get; set; }
        public bool Correcto { get; set; }
        public Expresion Target { get; set; }

        public override object Ejecutar(Entorno e, bool funcion, bool ciclo, bool sw, bool tc, LinkedList<Salida> log, LinkedList<Error> errores)
        {
            BD actual = e.Master.Actual;

            if (actual != null)
            {
                Simbolo sim = actual.GetTabla(Id);

                if (sim != null)
                {
                    Tabla tabla = (Tabla)sim.Valor;

                    if (Where == null)
                    {
                        tabla.Datos.Clear();
                    }
                    else
                    {
                        LinkedList<Entorno> delete = new LinkedList<Entorno>();

                        foreach (Entorno ent in tabla.Datos)
                        {
                            e.Master.EntornoActual = ent;

                            object valWhere = Where.GetValor(e, log, errores);
                            if (valWhere != null)
                            {
                                if (valWhere is Throw)
                                    return valWhere;

                                if (Where.Tipo.IsBoolean())
                                {
                                    if (!(bool)valWhere)
                                        continue;
                                }
                                else
                                {
                                    errores.AddLast(new Error("Semántico", "Cláusula Where debe ser booleana.", Linea, Columna));
                                    return null;
                                }
                            }
                            else
                                return null;

                            if (Target == null)
                                delete.AddLast(ent);
                            else
                            {
                                if (Target is Acceso acc)
                                {
                                    object valorTarget = acc.Target.GetValor(e, log, errores);

                                    if (valorTarget != null)
                                    {
                                        if (valorTarget is Throw)
                                            return valorTarget;

                                        if (acc.Target.Tipo.IsCollection())
                                        {
                                            if (valorTarget is Null)
                                                return new Throw("NullPointerException", Linea, Columna);
                                            else
                                            {
                                                Collection collection = (Collection)valorTarget;

                                                object valExpr = acc.Expr.GetValor(e, log, errores);

                                                if (valExpr != null)
                                                {
                                                    if (valExpr is Throw)
                                                        return valExpr;

                                                    if (!collection.Tipo.Clave.Equals(acc.Expr.Tipo))
                                                    {
                                                        Casteo cast = new Casteo(collection.Tipo.Clave, new Literal(acc.Expr.Tipo, valExpr, 0, 0), 0, 0)
                                                        {
                                                            Mostrar = false
                                                        };
                                                        valExpr = cast.GetValor(e, log, errores);

                                                        if (valExpr == null)
                                                        {
                                                            errores.AddLast(new Error("Semántico", "El tipo de la clave no coincide con el declarado en el Collection: " + collection.Tipo.Clave.Type.ToString() + ".", Linea, Columna));
                                                            continue;
                                                        }
                                                        else
                                                        {
                                                            if (valExpr is Throw)
                                                                return valExpr;
                                                        }

                                                    }

                                                    if (acc.Target.Tipo.IsList())
                                                    {
                                                        if (!collection.RemoveList(valExpr))
                                                            return new Throw("IndexOutException", Linea, Columna);
                                                    }
                                                    else if (acc.Target.Tipo.IsSet())
                                                    {
                                                        if (collection.Remove(valExpr))
                                                        {
                                                            collection.Ordenar();
                                                        }
                                                        else
                                                            return new Throw("IndexOutException", Linea, Columna);
                                                    }
                                                    else
                                                    {
                                                        if (!collection.Remove(valExpr))
                                                            errores.AddLast(new Error("Semántico", "No existe un valor con la clave: " + valExpr.ToString() + " en Map.", Linea, Columna));

                                                    }
                                                }
                                            }
                                        }
                                        errores.AddLast(new Error("Semántico", "La variable debe ser de tipo Map, List o Set.", Linea, Columna));

                                    }
                                }
                            }
                        }

                        e.Master.EntornoActual = null;

                        foreach (Entorno ent in delete)
                        {
                            tabla.Datos.Remove(ent);
                        }
                    }
                    Correcto = true;
                    return null;
                }
                else
                    return new Throw("TableDontExists", Linea, Columna);
                    //errores.AddLast(new Error("Semántico", "No existe una Tabla con el id: " + Id + " en la base de datos.", Linea, Columna));
            }
            else
                return new Throw("UseBDException", Linea, Columna);
                //errores.AddLast(new Error("Semántico", "No se ha seleccionado una base de datos, no se pudo Eliminar.", Linea, Columna));
        }
    }
}
