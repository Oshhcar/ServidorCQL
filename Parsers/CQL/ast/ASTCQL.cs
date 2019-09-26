using GramaticasCQL.Parsers.CQL.ast.entorno;
using GramaticasCQL.Parsers.CQL.ast.expresion;
using GramaticasCQL.Parsers.CQL.ast.instruccion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GramaticasCQL.Parsers.CQL.ast
{
    public class ASTCQL
    {
        public ASTCQL(LinkedList<NodoASTCQL> sentencias, string cadena)
        {
            Sentencias = sentencias;
            Cadena = cadena;
        }

        public LinkedList<NodoASTCQL> Sentencias { get; set; }
        public string Cadena { get; set; }

        public void Ejecutar(LinkedList<Salida> log, LinkedList<Error> errores, MasterBD master)
        {
            //MasterBD master = new MasterBD(); //aqui iran todas las tablas
            //master.AddUsuario("admin", "admin");
            
            Entorno global = new Entorno(null);
            global.Global = global;
            global.Master = master;

            object obj;

            foreach (NodoASTCQL stmt in Sentencias)
            {
                if (stmt is FuncionDef fun)
                {
                    obj = fun.Ejecutar(global, false, false, false, false, log, errores);

                    if (obj is Throw th)
                        errores.AddLast(new Error("Semántico", "Excepción no Controlada: " + th.Id + ".", th.Linea, th.Columna));
                }
            }

            foreach (NodoASTCQL stmt in Sentencias)
            {
                if (stmt is Instruccion instr)
                {
                    if (!(stmt is FuncionDef))
                    {
                        obj = instr.Ejecutar(global, false, false, false, false, log, errores);

                        if (obj != null)
                        {
                            if (obj is Throw th)
                                errores.AddLast(new Error("Semántico", "Excepción no Controlada: " + th.Id + ".", th.Linea, th.Columna));
                            else if (obj is Break bk)
                                errores.AddLast(new Error("Semántico", "Sentencia break no se encuentra dentro de un switch o ciclo.", bk.Linea, bk.Columna));
                            else if (obj is Continue co)
                                errores.AddLast(new Error("Semántico", "Sentencia continue no se encuentra dentro de un ciclo.", co.Linea, co.Columna));
                            else if (obj is Return re)
                                errores.AddLast(new Error("Semántico", "Sentencia return no se encuentra dentro de una función o procedimiento.", re.Linea, re.Columna));
                        }
                    }
                }
                else  if(stmt is Expresion expr)
                {
                    if (expr is FuncionCall fun)
                        fun.IsExpresion = false;

                    obj = expr.GetValor(global, log, errores);

                    if(obj is Throw th)
                        errores.AddLast(new Error("Semántico", "Excepción no Controlada: " + th.Id + ".", th.Linea, th.Columna));
                }

            }
        }

    }
}
