using GramaticasCQL.Parsers.CQL.ast.entorno;
using GramaticasCQL.Parsers.CQL.ast.expresion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GramaticasCQL.Parsers.CQL.ast.instruccion.condicionales
{
    class SubIf : Instruccion
    {
        public SubIf(Expresion cond, Bloque bloque, int linea, int columna) : base(linea, columna)
        {
            Cond = cond;
            Bloque = bloque;
            Else = false;
            Entra = false;
        }

        public SubIf(Bloque bloque, int linea, int columna) : base(linea, columna)
        {
            Cond = null;
            Bloque = bloque;
            Else = true;
            Entra = true;
        }

        public Expresion Cond { get; set; }
        public Bloque Bloque { get; set; }
        public bool Else { get; set; }
        public bool Entra { get; set; }

        public override object Ejecutar(Entorno e, bool funcion, bool ciclo, bool sw, bool tc, LinkedList<Salida> log, LinkedList<Error> errores)
        {
            if (!Else)
            {
                object valCond = Cond.GetValor(e, log, errores);

                if (valCond != null)
                {
                    if (valCond is Throw)
                        return valCond;

                    if (Cond.Tipo.IsBoolean())
                    {
                        Entra = (bool)valCond;
                    }
                    else
                    {
                        errores.AddLast(new Error("Semántico", "Se esperaba un booleano en condicion if.", Linea, Columna));
                        return null;
                    }
                }
            }

            if (Else || Entra)
            {
                Entorno local = new Entorno(e);
                return Bloque.Ejecutar(local, funcion, ciclo, sw, tc, log, errores);
            }

            return null;
        }
    }
}
