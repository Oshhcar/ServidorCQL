using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GramaticasCQL.Parsers
{
    public class Salida
    {
        public Salida(int tipo, string contenido)
        {
            Tipo = tipo;
            Contenido = contenido;
        }

        public int Tipo { get; set; }
        public string Contenido { get; set; }
    }
}
