using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GramaticasCQL.Parsers.CQL.ast.entorno;
using GramaticasCQL.Parsers.CQL.ast.expresion;

namespace GramaticasCQL.Parsers.CQL.ast.instruccion
{
    class FuncionDef : Instruccion
    {
        public FuncionDef(Tipo tipo, string id, LinkedList<Identificador> parametro, Bloque bloque, int linea, int columna) : base(linea, columna)
        {
            Tipo = tipo;
            Id = id;
            Parametro = parametro;
            Bloque = bloque;
        }

        public FuncionDef(Tipo tipo, string id, Bloque bloque, int linea, int columna) : base(linea, columna)
        {
            Tipo = tipo;
            Id = id;
            Parametro = null;
            Bloque = bloque;
        }

        public Tipo Tipo { get; set; }
        public string Id { get; set; }
        public LinkedList<Identificador> Parametro { get; set; }
        public Bloque Bloque { get; set; }

        public override object Ejecutar(Entorno e, bool funcion, bool ciclo, bool sw, bool tc, LinkedList<Salida> log, LinkedList<Error> errores)
        {
            string firma = Id;

            if (Parametro != null)
            {
                foreach (Identificador id in Parametro)
                {
                    firma += "-" + id.Tipo.Type.ToString();
                }
            }

            Simbolo sim = e.GetFuncion(firma);

            if (sim == null)
            {
                Funcion fun = new Funcion(Parametro, Bloque);
                sim = new Simbolo(Tipo, Rol.FUNCION, firma.ToLower(), fun);
                e.Add(sim);
            }
            else
                return new Throw("FunctionAreadyExists", Linea, Columna);
            //errores.AddLast(new Error("Semántico", "Ya se ha declarado una función con la misma firma: " + firma.ToLower() + ".", Linea, Columna));

            return null;
        }
    }
}
