using GramaticasCQL.Parsers.CQL.ast.entorno;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GramaticasCQL.Parsers.CQL.ast.instruccion.ddl
{
    class TablaModificar : Instruccion
    {
        public TablaModificar(string id, LinkedList<Simbolo> simbolos, int linea, int columna) : base(linea, columna)
        {
            Id = id;
            Simbolos = simbolos;
        }

        public TablaModificar(string id, LinkedList<string> columnas, int linea, int columna) : base(linea, columna)
        {
            Id = id;
            Columnas = columnas;
        }

        public string Id { get; set; }
        public LinkedList<Simbolo> Simbolos { get; set; }
        public LinkedList<string> Columnas { get; set; }

        public override object Ejecutar(Entorno e, bool funcion, bool ciclo, bool sw, bool tc, LinkedList<Salida> log, LinkedList<Error> errores)
        {
            BD actual = e.Master.Actual;

            if (actual != null)
            {
                Simbolo sim = actual.GetTabla(Id);

                if (sim != null)
                {
                    Tabla tabla = (Tabla)sim.Valor;

                    if (Simbolos != null)
                    {
                        foreach (Simbolo columna in Simbolos)
                        {
                            columna.Rol = Rol.COLUMNA;

                            if (tabla.Cabecera.GetCualquiera(columna.Id) != null)
                            {
                                errores.AddLast(new Error("Semántico", "Ya existe una columna con el id: " + columna.Id + " en la Tabla.", Linea, Columna));
                                continue;
                            }
                            tabla.Add(columna);
                        }
                    }
                    else
                    {
                        foreach (string columna in Columnas)
                        {
                            int drop = tabla.Drop(columna);

                            if (drop != 1)
                            {
                                if(drop == 2)
                                    errores.AddLast(new Error("Semántico", "No se puede eliminar la columna con el id: " + columna + " porque es llave primaria.", Linea, Columna));
                                else
                                    errores.AddLast(new Error("Semántico", "No existe una columna con el id: " + columna + " en la Tabla.", Linea, Columna));
                            }
                        }
                    }
                }
                else
                    return new Throw("TableDontExists", Linea, Columna);
                    //errores.AddLast(new Error("Semántico", "No existe una Tabla con el id: " + Id + " en la base de datos.", Linea, Columna));
            }
            else
                return new Throw("UseBDException", Linea, Columna);
                //errores.AddLast(new Error("Semántico", "No se ha seleccionado una base de datos, no se pudo modificar la Tabla.", Linea, Columna));

            return null;
        }
    }
}
