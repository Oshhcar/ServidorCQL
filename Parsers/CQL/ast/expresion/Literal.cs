using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GramaticasCQL.Parsers.CQL.ast.entorno;

namespace GramaticasCQL.Parsers.CQL.ast.expresion
{
    class Literal : Expresion
    {
        public Literal(Tipo tipo, object valor, int linea, int columna) : base(linea, columna)
        {
            Tipo = tipo;
            Valor = valor;
        }

        public object Valor { get; set; }

        public override object GetValor(Entorno e, LinkedList<Salida> log, LinkedList<Error> errores)
        {
            if (Tipo.IsDate())
            {
                if (!((Date)Valor).Correcto)
                {
                    errores.AddLast(new Error("Léxico", "Literal date escrita incorrectamente.", Linea, Columna));
                    return null;
                }
            }
            else if (Tipo.IsTime())
            {
                if (!((Time)Valor).Correcto)
                {
                    errores.AddLast(new Error("Léxico", "Literal time escrita incorrectamente.", Linea, Columna));
                    return null;
                }
            }

            return Valor;
        }
    }
}
