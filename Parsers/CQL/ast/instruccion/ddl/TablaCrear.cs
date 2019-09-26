using GramaticasCQL.Parsers.CQL.ast.entorno;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GramaticasCQL.Parsers.CQL.ast.instruccion.ddl
{
    class TablaCrear : Instruccion
    {
        public TablaCrear(string id, LinkedList<Simbolo> simbolos, LinkedList<string> primary, int linea, int columna) : base(linea, columna)
        {
            Id = id;
            Simbolos = simbolos;
            Primary = primary;
            IfNotExist = false;
        }

        public TablaCrear() : base(0, 0)
        {
            Simbolos = new LinkedList<Simbolo>();
            IfNotExist = false;
        }

        public string Id { get; set; }
        public LinkedList<Simbolo> Simbolos { get; set; }
        public LinkedList<string> Primary { get; set; }
        public bool IfNotExist { get; set; }

        public override object Ejecutar(Entorno e, bool funcion, bool ciclo, bool sw, bool tc, LinkedList<Salida> log, LinkedList<Error> errores)
        {
            BD actual = e.Master.Actual;
            if (actual != null)
            {
                if (actual.GetTabla(Id) == null)
                {
                    if (Primary != null)
                    {
                        foreach (string primary in Primary)
                        {
                            bool band = false;
                            foreach (Simbolo sim in Simbolos)
                            {
                                if (primary.Equals(sim.Id))
                                {
                                    sim.Rol = Rol.PRIMARY;
                                    band = true;
                                    break;
                                }
                            }
                            if(!band)
                                errores.AddLast(new Error("Semántico", "No existe una Columna con el id: " + primary + " en la tabla.", Linea, Columna));
                        }
                    }

                    Tabla tabla = new Tabla();

                    /*Validacion Counter*/
                    bool counter = false;
                    bool another = false;

                    foreach (Simbolo sim in Simbolos)
                    {
                        if (sim.Rol == Rol.PRIMARY)
                        {
                            if (sim.Tipo.IsCounter())
                                counter = true;
                            else
                                another = true;
                        }

                        if (tabla.Cabecera.GetCualquiera(sim.Id) != null)
                        {
                            errores.AddLast(new Error("Semántico", "Ya existe una columna con el id: " + sim.Id + " en la Tabla.", Linea, Columna));
                            continue;
                        }
                        tabla.Cabecera.Add(sim);
                    }

                    if (counter)
                    {
                        if (another)
                        {
                            errores.AddLast(new Error("Semántico", "Todos las columnas primary key deben ser tipo counter.", Linea, Columna));
                            return null;
                        }
                    }

                    if(tabla.Cabecera.Simbolos.Count() > 0)
                        actual.Add(new Simbolo(Rol.TABLA, Id.ToLower(), tabla));
                    else
                        errores.AddLast(new Error("Semántico", "No puede crear una Tabla sin Columnas.", Linea, Columna));

                    return null;
                }
                else
                {
                    if (!IfNotExist)
                        return new Throw("TableAlreadyExists", Linea, Columna);
                        //errores.AddLast(new Error("Semántico", "Ya existe una Tabla con el id: " + Id + " en la base de datos.", Linea, Columna));
                }
            }
            else
                return new Throw("UseBDException", Linea, Columna);
                //errores.AddLast(new Error("Semántico", "No se ha seleccionado una base de datos, no se pudo guardar la Tabla.", Linea, Columna));

            return null;
        }
    }
}
