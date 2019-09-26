using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GramaticasCQL.Parsers.CQL.ast.entorno;
using GramaticasCQL.Parsers.CQL.ast.expresion;

namespace GramaticasCQL.Parsers.CQL.ast.instruccion.ddl
{
    class Actualizar : Instruccion
    {
        public Actualizar(string id, LinkedList<Instruccion> asignaciones, int linea, int columna) : base(linea, columna)
        {
            Id = id;
            Asignaciones = asignaciones;
        }

        public Actualizar(string id, LinkedList<Instruccion> asignaciones, Where where, int linea, int columna) : base(linea, columna)
        {
            Id = id;
            Asignaciones = asignaciones;
            Where = where;
        }

        public string Id { get; set; }
        public LinkedList<Instruccion> Asignaciones { get; set; }
        public Where Where { get; set; }
        public bool Correcto { get; set; }

        public override object Ejecutar(Entorno e, bool funcion, bool ciclo, bool sw, bool tc, LinkedList<Salida> log, LinkedList<Error> errores)
        {
            BD actual = e.Master.Actual;

            if (actual != null)
            {
                Simbolo sim = actual.GetTabla(Id);

                if (sim != null)
                {
                    Tabla tabla = (Tabla)sim.Valor;

                    foreach (Entorno ent in tabla.Datos)
                    {
                        e.Master.EntornoActual = ent;

                        if (Where != null)
                        {
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
                        }

                        foreach (Instruccion asigna in Asignaciones)
                        {
                            object obj = asigna.Ejecutar(e, funcion, ciclo, sw, tc, log, errores);

                            if (obj is Throw)
                                return obj;
                        }
                    }
                    e.Master.EntornoActual = null;
                    Correcto = true;
                }
                else
                    return new Throw("TableDontExists", Linea, Columna);
                    //errores.AddLast(new Error("Semántico", "No existe una Tabla con el id: " + Id + " en la base de datos.", Linea, Columna));
            }
            else
                return new Throw("UseBDException", Linea, Columna);
                //errores.AddLast(new Error("Semántico", "No se ha seleccionado una base de datos, no se pudo Actualizar.", Linea, Columna));

            return null;
        }
    }
}
