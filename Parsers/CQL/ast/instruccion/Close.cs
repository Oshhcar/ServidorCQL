using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GramaticasCQL.Parsers.CQL.ast.entorno;

namespace GramaticasCQL.Parsers.CQL.ast.instruccion
{
    class Close : Instruccion
    {
        public Close(string id, int linea, int columna) : base(linea, columna)
        {
            Id = id;
        }
        public string Id { get; set; }

        public override object Ejecutar(Entorno e, bool funcion, bool ciclo, bool sw, bool tc, LinkedList<Salida> log, LinkedList<Error> errores)
        {
            Simbolo sim = e.Get(Id);

            if (sim != null)
            {
                if(sim.Tipo.IsCursor())
                { 
                    ((Cursor)sim.Valor).Data = null;
                }
                else
                    errores.AddLast(new Error("Semántico", "La variable: " + Id + " no es un Cursor.", Linea, Columna));
            }
            else
                errores.AddLast(new Error("Semántico", "No se ha declarado un Cursor con el id: " + Id + ".", Linea, Columna));

            return null;
        }
    }
}
