using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GramaticasCQL.Parsers.CQL.ast.entorno;

namespace GramaticasCQL.Parsers.CQL.ast.instruccion.ddl
{
    class Batch : Instruccion
    {
        public Batch(LinkedList<Instruccion> inst, int linea, int columna) : base(linea, columna)
        {
            Inst = inst;
        }

        public LinkedList<Instruccion> Inst { get; set; }

        public override object Ejecutar(Entorno e, bool funcion, bool ciclo, bool sw, bool tc, LinkedList<Salida> log, LinkedList<Error> errores)
        {
            object obj;       
            
            foreach (Instruccion inst in Inst)
            {
                if (inst is Actualizar act)
                {
                    obj = act.Ejecutar(e, funcion, ciclo, sw, tc, log, errores);

                    if (obj is Throw)
                        return new Throw("BatchException", Linea, Columna);

                    if (!act.Correcto)
                    {
                        return new Throw("BatchException", Linea, Columna);
                        //errores.AddLast(new Error("Semántico", "Error en Batch.", Linea, Columna));
                        //return null;
                    }
                }
                else if (inst is Insertar inser)
                {
                    obj = inser.Ejecutar(e, funcion, ciclo, sw, tc, log, errores);

                    if(obj is Throw)
                        return new Throw("BatchException", Linea, Columna);

                    if (!inser.Correcto)
                    {
                        return new Throw("BatchException", Linea, Columna);
                        //errores.AddLast(new Error("Semántico", "Error en Batch.", Linea, Columna));
                        //return null;
                    }
                }
                else if (inst is Eliminar eli)
                {
                    obj = eli.Ejecutar(e, funcion, ciclo, sw, tc, log, errores);

                    if(obj is Throw)
                        return new Throw("BatchException", Linea, Columna);

                    if (!eli.Correcto)
                    {
                        return new Throw("BatchException", Linea, Columna);
                        //errores.AddLast(new Error("Semántico", "Error en Batch.", Linea, Columna));
                        //return null;
                    }
                }
            }
            return null;
        }
    }
}
