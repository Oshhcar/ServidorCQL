using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GramaticasCQL.Parsers.CQL.ast.entorno;
using GramaticasCQL.Parsers.CQL.ast.instruccion;

namespace GramaticasCQL.Parsers.CQL.ast.expresion
{
    class ObjetoDisplay : Expresion
    {
        public ObjetoDisplay(string id, LinkedList<Expresion> atributos, int linea, int columna) : base(linea, columna)
        {
            Id = id;
            Atributos = atributos;
        }

        public string Id { get; set; }
        public LinkedList<Expresion> Atributos { get; set; }

        public override object GetValor(Entorno e, LinkedList<Salida> log, LinkedList<Error> errores)
        {
            BD actual = e.Master.Actual;

            if (actual != null)
            {
                Simbolo sim = actual.GetUserType(Id);
                if (sim != null)
                {
                    if (Atributos.Count() == ((Entorno)sim.Valor).Simbolos.Count())
                    {
                        LinkedList<Simbolo> sims = new LinkedList<Simbolo>();
                        for (int i = 0; i < Atributos.Count(); i++)
                        {
                            Simbolo s = ((Entorno)sim.Valor).Simbolos.ElementAt(i);
                            Expresion expr = Atributos.ElementAt(i);
                            object valExpr = expr.GetValor(e, log, errores);

                            if (valExpr != null)
                            {
                                if (valExpr is Throw)
                                    return valExpr;

                                if (s.Tipo.Equals(expr.Tipo))
                                {
                                    sims.AddLast(new Simbolo(s.Tipo, Rol.ATRIBUTO, s.Id, valExpr));
                                    continue;
                                }
                                else
                                {
                                    if (s.Tipo.IsCollection() && expr.Tipo.IsCollection())
                                    {
                                        if (s.Tipo.EqualsCollection(expr.Tipo))
                                        {/*verificar mas tipos*/
                                            sims.AddLast(new Simbolo(s.Tipo, Rol.ATRIBUTO, s.Id, valExpr));
                                            continue;
                                        }
                                    }
                                    else
                                    {
                                        Casteo cast = new Casteo(s.Tipo, new Literal(expr.Tipo, valExpr, 0, 0), 0, 0)
                                        {
                                            Mostrar = false
                                        };
                                        valExpr = cast.GetValor(e, log, errores);

                                        if (valExpr != null)
                                        {
                                            if (valExpr is Throw)
                                                return valExpr;

                                            sims.AddLast(new Simbolo(s.Tipo, Rol.ATRIBUTO, s.Id, valExpr));
                                            continue;
                                        }
                                    }
                                }

                            }

                            errores.AddLast(new Error("Semántico", "Los valores no coinciden con el User Type.", Linea, Columna));
                            return null;
                        }
                        Tipo = new Tipo(Id.ToLower());
                        return new Objeto(Id.ToLower(), new Entorno(null, sims));
                    }
                    else
                        errores.AddLast(new Error("Semántico", "Los valores no coinciden con el User Type.", Linea, Columna));
                }
                else
                    errores.AddLast(new Error("Semántico", "No existe un User Type con el id: " + Id + " en la base de datos.", Linea, Columna));
            }
            else
                errores.AddLast(new Error("Semántico", "No se ha seleccionado una base de datos, no se pudo buscar el User Type.", Linea, Columna));

            return null;
        }
    }
}
