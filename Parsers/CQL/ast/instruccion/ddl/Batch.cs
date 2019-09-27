using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GramaticasCQL.Models;
using GramaticasCQL.Parsers.CHISON;
using GramaticasCQL.Parsers.CHISON.ast;
using GramaticasCQL.Parsers.CQL.ast.entorno;

namespace GramaticasCQL.Parsers.CQL.ast.instruccion.ddl
{
    public class Batch : Instruccion
    {
        public Batch(LinkedList<Instruccion> inst, int linea, int columna) : base(linea, columna)
        {
            Inst = inst;
        }

        public LinkedList<Instruccion> Inst { get; set; }

        public override object Ejecutar(Entorno e, bool funcion, bool ciclo, bool sw, bool tc, LinkedList<Salida> log, LinkedList<Error> errores)
        {
            object obj;

            Commit commit = new Commit(Linea, Columna)
            {
                Nombre = "backup.chison"
            };

            commit.Ejecutar(e, funcion, ciclo, sw, tc, log, errores);

            foreach (Instruccion inst in Inst)
            {
                if (inst is Actualizar act)
                {
                    obj = act.Ejecutar(e, funcion, ciclo, sw, tc, log, errores);

                    if (obj is Throw)
                    {
                        Rollback(e, log, errores);
                        return new Throw("BatchException", Linea, Columna);
                    }

                    if (!act.Correcto)
                    {
                        Rollback(e, log, errores);
                        return new Throw("BatchException", Linea, Columna);
                        //errores.AddLast(new Error("Semántico", "Error en Batch.", Linea, Columna));
                        //return null;
                    }
                }
                else if (inst is Insertar inser)
                {
                    obj = inser.Ejecutar(e, funcion, ciclo, sw, tc, log, errores);

                    if (obj is Throw)
                    {
                        Rollback(e, log, errores);
                        return new Throw("BatchException", Linea, Columna);
                    }

                    if (!inser.Correcto)
                    {
                        Rollback(e, log, errores);
                        return new Throw("BatchException", Linea, Columna);
                        //errores.AddLast(new Error("Semántico", "Error en Batch.", Linea, Columna));
                        //return null;
                    }
                }
                else if (inst is Eliminar eli)
                {
                    obj = eli.Ejecutar(e, funcion, ciclo, sw, tc, log, errores);

                    if (obj is Throw)
                    {
                        Rollback(e, log, errores);
                        return new Throw("BatchException", Linea, Columna);
                    }

                    if (!eli.Correcto)
                    {
                        Rollback(e, log, errores);
                        return new Throw("BatchException", Linea, Columna);
                        //errores.AddLast(new Error("Semántico", "Error en Batch.", Linea, Columna));
                        //return null;
                    }
                }
            }
            return null;
        }

        public void Rollback(Entorno e, LinkedList<Salida> log, LinkedList<Error> errores)
        {
            string archivo = BaseDatos.PathDatos.MapPath("/Files/backup.chison");

            StreamReader reader = null;

            try
            {
                if (File.Exists(archivo))
                {
                    reader = new StreamReader(archivo);
                    string entrada = reader.ReadToEnd();

                    AnalizadorCHISON chison = new AnalizadorCHISON();

                    if (chison.AnalizarEntrada(entrada))
                    {
                        object obj = chison.GenerarArbol(chison.Raiz.Root);

                        if (obj != null)
                        {
                            if (obj is ASTCHISON ast)
                            {
                                if (ast.IsPrincipal())
                                {
                                    e.MasterRollback = new MasterBD();
                                    e.MasterRollback.AddUsuario("admin", "admin");
                                    e.MasterRollback.EntornoActual = e.Master.EntornoActual;

                                    string user = null;
                                    if(e.Master.UsuarioActual != null)
                                        user = e.Master.UsuarioActual.Id;
                                    string bd = null;
                                    if (e.Master.Actual != null)
                                        bd = e.Master.Actual.Id;

                                    //e.MasterRollback.UsuarioActual = e.Master.UsuarioActual;
                                    ast.Ejecutar(e, log, errores);
                                    BaseDatos.Master = e.MasterRollback;
                                    e.Master = e.MasterRollback;
                                    e.Master.UsuarioActual = null;
                                    e.Master.Actual = null;

                                    if (user != null)
                                    {
                                        e.Master.UsuarioActual = e.Master.GetUsuario(user);
                                    }

                                    if (bd != null)
                                    {
                                        e.Master.Actual = e.Master.Get(bd);
                                    }


                                    
                                }
                            }
                        }
                    }
                    else
                    {
                        //Console.WriteLine("Errores leyendo Chison.");
                    }

                }
                else
                {
                    //Console.WriteLine("no existe");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Rollback: " + ex.Message);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
        }
    }
}
