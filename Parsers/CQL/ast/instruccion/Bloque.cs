using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GramaticasCQL.Parsers.CQL.ast.entorno;
using GramaticasCQL.Parsers.CQL.ast.expresion;

namespace GramaticasCQL.Parsers.CQL.ast.instruccion
{
    class Bloque : Instruccion
    {
        public Bloque(LinkedList<NodoASTCQL> bloques, string cadena, int linea, int columna) : base(linea, columna)
        {
            Bloques = bloques;
            Cadena = cadena;
        }

        public LinkedList<NodoASTCQL> Bloques { get; set; }
        public string Cadena { get; set; }

        public override object Ejecutar(Entorno e, bool funcion, bool ciclo, bool sw, bool tc, LinkedList<Salida> log, LinkedList<Error> errores)
        {
            if (Bloques != null)
            {
                foreach (NodoASTCQL bloque in Bloques)
                {
                    if (bloque is Instruccion inst)
                    {
                        object obj = inst.Ejecutar(e, funcion, ciclo, sw, tc, log, errores);

                        if (obj is Break bk)
                        {
                            if (ciclo || sw)
                                return obj;
                            else
                                errores.AddLast(new Error("Semántico", "Sentencia break no se encuentra dentro de un switch o ciclo.", bk.Linea, bk.Columna));

                        }
                        else if (obj is Continue co)
                        {
                            if (ciclo)
                                return obj;
                            else
                                errores.AddLast(new Error("Semántico", "Sentencia continue no se encuentra dentro de un ciclo.", co.Linea, co.Columna));

                        }
                        else if (obj is Return re)
                        {
                            if (funcion)
                                return obj;
                            else
                                errores.AddLast(new Error("Semántico", "Sentencia return no se encuentra dentro de una función o procedimiento.", re.Linea, re.Columna));

                        }
                        else if (obj is Throw th)
                        {
                            if(tc)
                                return obj;
                            errores.AddLast(new Error("Semántico", "Excepción no Controlada: " + th.Id + ".", th.Linea, th.Columna));
                        }
                    }
                    else if (bloque is Expresion expr)
                    {
                        if (expr is FuncionCall fun)
                            fun.IsExpresion = false;

                        expr.GetValor(e, log, errores);
                    }
                }
            }
            return null;
        }
    }
}
