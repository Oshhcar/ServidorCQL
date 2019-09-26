using GramaticasCQL.Parsers.CQL.ast.entorno;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GramaticasCQL.Parsers.CQL.ast.instruccion
{
    abstract class Instruccion : NodoASTCQL
    {
        public Instruccion(int linea, int columna) : base(linea, columna) { }

        public abstract Object Ejecutar(Entorno e, bool funcion, bool ciclo, bool sw, bool tc, LinkedList<Salida> log, LinkedList<Error> errores);
    }
}
