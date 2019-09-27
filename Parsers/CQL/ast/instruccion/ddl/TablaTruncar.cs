using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GramaticasCQL.Parsers.CQL.ast.entorno;

namespace GramaticasCQL.Parsers.CQL.ast.instruccion.ddl
{
    public class TablaTruncar : Instruccion
    {
        public TablaTruncar(string id, int linea, int columna) : base(linea, columna)
        {
            Id = id;
        }

        public string Id { get; set; }

        public override object Ejecutar(Entorno e, bool funcion, bool ciclo, bool sw, bool tc, LinkedList<Salida> log, LinkedList<Error> errores)
        {
            BD actual = e.Master.Actual;

            if (actual != null)
            {
                if (e.Master.UsuarioActual != null)
                {
                    if (e.Master.UsuarioActual.GetPermiso(actual.Id))
                    {
                        if (!actual.TruncateTabla(Id))
                        {
                            return new Throw("TableDontExists", Linea, Columna);
                            //errores.AddLast(new Error("Semántico", "No existe una Tabla con el id: " + Id + " en la base de datos.", Linea, Columna));
                        }
                    }
                    else
                        errores.AddLast(new Error("Semántico", "El Usuario no tiene permisos sobre: " + actual.Id + ".", Linea, Columna));
                }
                else
                    errores.AddLast(new Error("Semántico", "No hay un Usuario logeado.", Linea, Columna));
            }
            else
                return new Throw("UseBDException", Linea, Columna);
                //errores.AddLast(new Error("Semántico", "No se ha seleccionado una base de datos, no se pudo truncar la Tabla.", Linea, Columna));

            return null;
        }
    }
}
