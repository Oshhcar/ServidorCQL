using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GramaticasCQL.Parsers.CQL.ast.entorno;
using GramaticasCQL.Parsers.CQL.ast.expresion;
using GramaticasCQL.Parsers.CQL.ast.expresion.operacion;

namespace GramaticasCQL.Parsers.CQL.ast.instruccion.condicionales
{
    class Case : Instruccion
    {
        public Case(Expresion expr, Bloque bloque, int linea, int columna) : base(linea, columna)
        {
            Expr = expr;
            Bloque = bloque;
            Default = false;
            Continuar = false;
        }
        public Case(Bloque bloque, int linea, int columna) : base(linea, columna)
        {
            Expr = null;
            Bloque = bloque;
            Default = true;
            Continuar = false;
        }
        public Expresion Expr { get; set; }
        public Bloque Bloque { get; set; }
        public Expresion ExprSwitch { get; set; }
        public bool Default { get; set; }
        public bool Continuar { get; set; }
        public override object Ejecutar(Entorno e, bool funcion, bool ciclo, bool sw, bool tc, LinkedList<Salida> log, LinkedList<Error> errores)
        {
            if (!Default)
            {
                if (!Continuar)
                {
                    object valExpr = Expr.GetValor(e, log, errores);

                    if (valExpr != null)
                    {
                        if (valExpr is Throw)
                            return valExpr;

                        Relacional rel = new Relacional(ExprSwitch, new Literal(Expr.Tipo, valExpr, Linea, Columna), Operador.IGUAL, Linea, Columna);
                        object valRel = rel.GetValor(e, log, errores);

                        if (valRel != null)
                        {
                            if ((bool)valRel)
                                Continuar = true;
                        }
                    }
                }
            }
            else
                Continuar = true;

            if (Continuar)
            {
                Entorno local = new Entorno(e);
                return Bloque.Ejecutar(local, funcion, ciclo, sw, tc, log, errores);
            }

            return null;
        }
    }
}
