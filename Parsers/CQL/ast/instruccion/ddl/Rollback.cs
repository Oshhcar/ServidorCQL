using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GramaticasCQL.Parsers.CHISON;
using GramaticasCQL.Parsers.CHISON.ast;
using GramaticasCQL.Parsers.CQL.ast.entorno;

namespace GramaticasCQL.Parsers.CQL.ast.instruccion.ddl
{
    class Rollback : Instruccion
    {
        public Rollback(int linea, int columna) : base(linea, columna) { }

        public override object Ejecutar(Entorno e, bool funcion, bool ciclo, bool sw, bool tc, LinkedList<Salida> log, LinkedList<Error> errores)
        {
            string archivo = "baseDatos.chison";

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
                                    ast.Ejecutar(e, log, errores);
                                    e.Master = e.MasterRollback;
                                }
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Errores leyendo Chison.");
                    }

                }
                else
                {
                    Console.WriteLine("no existe");
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
            return null;
        }
    }
}
