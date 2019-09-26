using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GramaticasCQL.Parsers.CQL.ast.entorno;
using GramaticasCQL.Parsers.CQL.ast.expresion;

namespace GramaticasCQL.Parsers.CQL.ast.instruccion
{
    class Declaracion : Instruccion
    {
        public Declaracion(Tipo tipo, LinkedList<Expresion> target, Expresion expr,  int linea, int columna) : base(linea, columna)
        {
            Tipo = tipo;
            Target = target;
            Expr = expr;
        }

        public Tipo Tipo { get; set; }
        public LinkedList<Expresion> Target { get; set; }
        public Expresion Expr { get; set; }

        public override object Ejecutar(Entorno e, bool funcion, bool ciclo, bool sw, bool tc, LinkedList<Salida> log, LinkedList<Error> errores)
        {
            if (Tipo.IsObject())
            {
                BD actual = e.Master.Actual;

                if (actual != null)
                {
                    if (actual.GetUserType(Tipo.Objeto) == null)
                    {
                        errores.AddLast(new Error("Semántico", "No existe un User Type con el id: " + Tipo.Objeto + " en la base de datos.", Linea, Columna));
                        return null;
                    }
                }
                else
                {
                    errores.AddLast(new Error("Semántico", "No se ha seleccionado una base de datos, no se pudo buscar el User Type.", Linea, Columna));
                    return null;
                }
            }

            foreach (Expresion target in Target)
            {
                object valorExpr = null;

                if (Target.Last.Value.Equals(target))
                {
                    if (Expr != null)
                    {
                        valorExpr = Expr.GetValor(e, log, errores);

                        if (valorExpr == null)
                            return null;

                        if (valorExpr is Throw)
                            return valorExpr;

                        if (!Tipo.Equals(Expr.Tipo))
                        {
                            Casteo cast = new Casteo(Tipo, new Literal(Expr.Tipo, valorExpr, 0, 0), 0, 0)
                            {
                                Mostrar = false
                            };

                            valorExpr = cast.GetValor(e, log, errores);

                            if (valorExpr == null)
                            {
                                if (valorExpr is Throw)
                                    return valorExpr;

                                errores.AddLast(new Error("Semántico", "El valor no corresponde al tipo declarado.", Linea, Columna));
                                return null;
                            }
                        }

                        if (valorExpr is Collection collection)
                        {
                            Tipo.Clave = collection.Tipo.Clave;
                            Tipo.Valor = collection.Tipo.Valor;
                        }
                    }
                }

                if (target is Identificador id)
                {
                    if (id.IsId2)
                    {
                        Simbolo sim = e.GetLocal(id.Id);

                        if (sim != null)
                        {
                            return new Throw("ObjectAlreadyExists", Linea, Columna);
                            //errores.AddLast(new Error("Semántico", "Ya se ha declarado una variable con el id: " + id.Id + ".", Linea, Columna));
                            //continue;
                        }

                        sim = new Simbolo(Tipo, Rol.VARIABLE, id.Id.ToLower(), valorExpr);
                        e.Add(sim);
                    }
                    else
                    {
                        errores.AddLast(new Error("Semántico", "No se puede declarar una variable sin el @ al inicio.", Linea, Columna));
                        continue;
                    }
                }
                else
                {
                    errores.AddLast(new Error("Semántico", "Solo se pueden declarar variables.", Linea, Columna));
                }
            }


            return null;
        }
    }
}
