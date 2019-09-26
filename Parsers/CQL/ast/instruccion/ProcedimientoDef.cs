using GramaticasCQL.Parsers.CQL.ast.entorno;
using GramaticasCQL.Parsers.CQL.ast.expresion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Type = GramaticasCQL.Parsers.CQL.ast.entorno.Type;

namespace GramaticasCQL.Parsers.CQL.ast.instruccion
{
    class ProcedimientoDef : Instruccion
    {
        public ProcedimientoDef(string id, LinkedList<Identificador> parametro, LinkedList<Identificador> retorno, Bloque bloque, int linea, int columna) : base(linea, columna)
        {
            Id = id;
            Parametro = parametro;
            Retorno = retorno;
            Bloque = bloque;
        }

        public string Id { get; set; }
        public LinkedList<Identificador> Parametro { get; set; }
        public LinkedList<Identificador> Retorno { get; set; }
        public Bloque Bloque { get; set; }

        public override object Ejecutar(Entorno e, bool funcion, bool ciclo, bool sw, bool tc, LinkedList<Salida> log, LinkedList<Error> errores)
        {
            BD actual = e.Master.Actual;

            if (actual != null)
            {
                string firma = Id;

                if (Parametro != null)
                {
                    foreach (Identificador id in Parametro)
                    {
                        firma += "-" + id.Tipo.Type.ToString();
                    }
                }

                Simbolo sim = actual.GetProcedimiento(firma);

                if (sim == null)
                {
                    Procedimiento proc = new Procedimiento(Parametro, Retorno, Bloque);
                    sim = new Simbolo(new Tipo(Type.VOID), Rol.PROCEDIMIENTO, firma.ToLower(), proc);
                    actual.Add(sim);
                }
                else
                    return new Throw("ProcedureAlreadyExists", Linea, Columna);
                    //errores.AddLast(new Error("Semántico", "Ya se ha declarado un Procedimiento con la misma firma: " + firma.ToLower() + " en la base de datos.", Linea, Columna));

            }
            else
                errores.AddLast(new Error("Semántico", "No se ha seleccionado una base de datos, no se pudo guardar el Procedimiento.", Linea, Columna));

            return null;
        }
    }
}
