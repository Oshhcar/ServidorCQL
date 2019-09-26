using GramaticasCQL.Parsers.CQL.ast.entorno;
using GramaticasCQL.Parsers.CQL.ast.expresion;
using GramaticasCQL.Parsers.CQL.ast.instruccion;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GramaticasCQL.Parsers.CHISON.ast
{
    class Users : Instruccion
    {
        public Users(Expresion valor, int linea, int columna) : base(linea, columna)
        {
            Valor = valor;
        }

        public Expresion Valor { get; set; } //Un list con objeto 

        public override object Ejecutar(Entorno e, bool funcion, bool ciclo, bool sw, bool tc, LinkedList<Salida> log, LinkedList<Error> errores)
        {
            if (Valor is Lista lista)
            {
                if (lista.Valores != null)
                {
                    LeerArchivo(lista, e, log, errores);

                    foreach (Expresion expr in lista.Valores)
                    {
                        if (expr is BloqueChison bloque)
                        {
                            bloque.Obj = OBJ.USER;
                            object obj = bloque.GetValor(e, log, errores);

                            if (obj != null)
                            {
                                if (obj is Usuario usuario)
                                {
                                    Usuario old = e.MasterRollback.GetUsuario(usuario.Id);
                                    if (old == null)
                                        e.MasterRollback.Usuarios.AddLast(usuario);
                                    else
                                    {
                                        foreach (string permiso in usuario.Permisos)
                                        {
                                            if (!old.GetPermiso(permiso))
                                                old.AddPermiso(permiso);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
                return null; //debe ser una lista
            return null;
        }

        public void LeerArchivo(Lista lista, Entorno e, LinkedList<Salida> log, LinkedList<Error> errores)
        {
            if (lista.Valores.Count() > 0)
            {
                Expresion val = lista.Valores.ElementAt(0);

                if (val.Tipo.IsVoid())
                {
                    string archivo = val.GetValor(e, log, errores).ToString().Replace("$", "").Replace("{", "").Replace("}", "").Trim();

                    StreamReader reader = null;
                    try
                    {
                        if (File.Exists(archivo))
                        {
                            reader = new StreamReader(archivo);
                            string cadena = reader.ReadToEnd();

                            AnalizadorCHISON chison = new AnalizadorCHISON();

                            if (chison.AnalizarEntrada(cadena))
                            {
                                object obj = chison.GenerarArbol(chison.Raiz.Root);

                                if (obj != null)
                                {
                                    if (obj is ASTCHISON ast)
                                    {
                                        if (ast.Bloques != null)
                                        {
                                            lista.Valores = new LinkedList<Expresion>();
                                            foreach (BloqueChison b in ast.Bloques)
                                            {
                                                lista.Valores.AddLast((Expresion)b);
                                            }
                                        }
                                    }
                                }
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Excepción BloqueChison: " + ex.Message);
                    }
                    finally
                    {
                        if (reader != null)
                            reader.Close();
                    }
                }
            }
        }
    }
}
