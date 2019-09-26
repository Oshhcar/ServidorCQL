using GramaticasCQL.Parsers.CQL.ast.entorno;
using GramaticasCQL.Parsers.CQL.ast.instruccion.ddl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Type = GramaticasCQL.Parsers.CQL.ast.entorno.Type;

namespace GramaticasCQL.Parsers.CQL.ast.instruccion
{
    class CursorDef : Instruccion
    {
        public CursorDef(string id, Seleccionar select, int linea, int columna) : base(linea, columna)
        {
            Id = id;
            Select = select;
        }

        public string Id { get; set; }
        public Seleccionar Select { get; set; }

        public override object Ejecutar(Entorno e, bool funcion, bool ciclo, bool sw, bool tc, LinkedList<Salida> log, LinkedList<Error> errores)
        {
            Simbolo sim = e.GetLocal(Id);

            if (sim == null)
            {
                Select.Mostrar = false;
                sim = new Simbolo(new Tipo(Type.CURSOR), Rol.VARIABLE, Id.ToLower(), new Cursor(Select));
                e.Add(sim);

            }
            else
                return new Throw("ObjectAlreadyExists", Linea, Columna);
            //errores.AddLast(new Error("Semántico", "Ya se ha declarado una variable con el id: " + Id + ".", Linea, Columna));

            return null;
        }
    }
}
