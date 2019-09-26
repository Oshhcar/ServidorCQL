using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GramaticasCQL.Parsers.CQL.ast.entorno
{
    class CollectionValue
    {
        public CollectionValue(object clave, object valor)
        {
            Clave = clave;
            Valor = valor;
        }

        public object Clave { get; set; }
        public object Valor { get; set; }
    }
}
