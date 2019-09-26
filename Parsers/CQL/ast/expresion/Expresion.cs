using GramaticasCQL.Parsers.CQL.ast.entorno;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GramaticasCQL.Parsers.CQL.ast.expresion
{
    abstract class Expresion : NodoASTCQL
    {
        public Expresion(int linea, int columna) : base(linea, columna) { Tipo = null; }

        public Tipo Tipo { get; set; }

        public abstract object GetValor(Entorno e, LinkedList<Salida> log, LinkedList<Error> errores);

        public virtual Simbolo GetSimbolo(Entorno e) { return null; }

        public virtual string GetId() { return null; }
    }
}
