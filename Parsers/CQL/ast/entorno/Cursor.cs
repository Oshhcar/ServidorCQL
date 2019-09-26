using GramaticasCQL.Parsers.CQL.ast.instruccion.ddl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GramaticasCQL.Parsers.CQL.ast.entorno
{
    class Cursor
    {
        public Cursor(Seleccionar select)
        {
            Select = select;
            Data = null;
        }

        public Seleccionar Select { get; set; }
        public LinkedList<Entorno> Data { get; set; }
    }
}
