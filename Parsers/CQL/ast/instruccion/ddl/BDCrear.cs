using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GramaticasCQL.Parsers.CQL.ast.entorno;

namespace GramaticasCQL.Parsers.CQL.ast.instruccion.ddl
{
    class BDCrear : Instruccion
    {
        public BDCrear(string id, bool ifNotExist, int linea, int columna) : base(linea, columna)
        {
            Id = id;
            IfNotExist = ifNotExist;
        }

        public string Id { get; set; }
        public bool IfNotExist { get; set; } 

        public override object Ejecutar(Entorno e, bool funcion, bool ciclo, bool sw, bool tc, LinkedList<Salida> log, LinkedList<Error> errores)
        {
            if (e.Master.Get(Id) == null)
            {
                e.Master.Add(Id);

                Usuario user = e.Master.GetUsuario("admin");

                if (user != null)
                {
                    if (!user.GetPermiso(Id))
                    {
                        user.AddPermiso(Id);
                    }
                }
            }
            else
            {
                if (!IfNotExist)
                    return new Throw("BDAlreadyExists", Linea, Columna);
                    //errores.AddLast(new Error("Semántico", "Ya existe una base de datos con el id: "+Id+".", Linea, Columna));
            }
            return null;
        }
    }
}
