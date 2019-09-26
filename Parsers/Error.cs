using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GramaticasCQL.Parsers
{
    public class Error
    {
        public Error(string valor, string descripcion, int linea, int columna)
        {
            Valor = valor;
            Descripcion = descripcion;
            Linea = linea;
            Columna = columna;

        }

        public string Valor { get; set; }
        public string Descripcion { get; set; }
        public int Linea { get; set; }
        public int Columna { get; set; }
    }
}
