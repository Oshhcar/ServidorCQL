using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GramaticasCQL.Parsers.CQL.ast.entorno;

namespace GramaticasCQL.Parsers.CQL.ast.instruccion.ddl
{
    class Use : Instruccion
    {
        public Use(string id, int linea, int columna) : base(linea, columna)
        {
            Id = id;
        }

        public string Id { get; set; }

        public override object Ejecutar(Entorno e, bool funcion, bool ciclo, bool sw, bool tc, LinkedList<Salida> log, LinkedList<Error> errores)
        {
            BD data = e.Master.Get(Id);

            if (data != null)
            {
                e.Master.Actual = data;
            }
            else
            {
                //errores.AddLast(new Error("Semántico", "No existe una base de datos con el id: " + Id + ".", Linea, Columna));
                return new Throw("BDDontExists", Linea, Columna);
            }
            return null;
        }
    }
}
