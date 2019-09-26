using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GramaticasCQL.Parsers.CQL.ast.entorno;

namespace GramaticasCQL.Parsers.CQL.ast.instruccion.condicionales
{
    class If : Instruccion
    {
        public If(LinkedList<SubIf> subIfs, int linea, int columna) : base(linea, columna)
        {
            SubIfs = subIfs;
        }

        public LinkedList<SubIf> SubIfs { get; set; }

        public override object Ejecutar(Entorno e, bool funcion, bool ciclo, bool sw, bool tc, LinkedList<Salida> log, LinkedList<Error> errores)
        {
            foreach (SubIf subIf in SubIfs)
            {
                object obj = subIf.Ejecutar(e, funcion, ciclo, sw, tc, log, errores);

                if (obj != null)
                    return obj;

                if (subIf.Entra)
                    return null;
            }
            return null;
        }
    }
}
