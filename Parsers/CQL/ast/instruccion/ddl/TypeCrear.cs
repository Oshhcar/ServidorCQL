using GramaticasCQL.Parsers.CQL.ast.entorno;
using GramaticasCQL.Parsers.CQL.ast.expresion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GramaticasCQL.Parsers.CQL.ast.instruccion.ddl
{
    class TypeCrear : Instruccion
    {
        public TypeCrear(string id, bool ifNotExist, LinkedList<Simbolo> atributos, int linea, int columna) : base(linea, columna)
        {
            Id = id;
            IfNotExist = ifNotExist;
            Atributos = atributos;
        }

        public string Id { get; set; }
        public bool IfNotExist { get; set; }
        public LinkedList<Simbolo> Atributos { get; set; }

        public override object Ejecutar(Entorno e, bool funcion, bool ciclo, bool sw, bool tc, LinkedList<Salida> log, LinkedList<Error> errores)
        {
            BD actual = e.Master.Actual;
            if (actual != null)
            {
                if (actual.GetUserType(Id) == null)
                {
                    actual.Add(new Simbolo(Rol.USERTYPE, Id.ToLower(), new Entorno(null, Atributos)));
                }
                else
                {
                    if (!IfNotExist)
                        return new Throw("TypeAlreadyExists", Linea, Columna);
                        //errores.AddLast(new Error("Semántico", "Ya existe un User Type con el id: " + Id + " en la base de datos.", Linea, Columna));
                }
            }
            else
                return new Throw("UseBDException", Linea, Columna);
                //errores.AddLast(new Error("Semántico", "No se ha seleccionado una base de datos, no se pudo guardar el User Type.", Linea, Columna));

            return null;
        }
    }
}
