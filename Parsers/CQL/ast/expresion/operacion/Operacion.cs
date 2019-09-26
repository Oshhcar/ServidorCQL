using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GramaticasCQL.Parsers.CQL.ast.entorno;

namespace GramaticasCQL.Parsers.CQL.ast.expresion.operacion
{
    class Operacion : Expresion
    {

        public Operacion(Expresion op1, Expresion op2, Operador op, int linea, int columna) : base(linea, columna)
        {
            Op1 = op1;
            Op2 = op2;
            Op = op;
        }

        public Expresion Op1 { get; set; }
        public Expresion Op2 { get; set; }
        public Operador Op { get; set; }

        public override object GetValor(Entorno e, LinkedList<Salida> log, LinkedList<Error> errores)
        {
            return null;
        }
    }

    public enum Operador
    {
        SUMA,
        RESTA,
        MULTIPLICACION, 
        POTENCIA,
        MODULO,
        DIVISION,
        AUMENTO,
        DECREMENTO,
        MENORQUE,
        MAYORQUE,
        IGUAL,
        MAYORIGUAL,
        MENORIGUAL,
        DIFERENTE,
        OR,
        XOR,
        AND,
        NOT,
        INDEFINIDO
    }
}
