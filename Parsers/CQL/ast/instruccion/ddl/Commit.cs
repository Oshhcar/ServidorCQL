using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GramaticasCQL.Parsers.CQL.ast.entorno;
using GramaticasCQL.Parsers.CQL.ast.expresion;

namespace GramaticasCQL.Parsers.CQL.ast.instruccion.ddl
{
    class Commit : Instruccion
    {
        public Commit(int linea, int columna) : base(linea, columna) { }

        public override object Ejecutar(Entorno e, bool funcion, bool ciclo, bool sw, bool tc, LinkedList<Salida> log, LinkedList<Error> errores)
        {
            string archivo = "baseDatos.chison";

            StreamWriter writer = null;

            try
            {
                writer = new StreamWriter(archivo);

                string contenido = "$<\n";

                contenido += "\t\"DATABASES\"= [\n";
                foreach (BD bd in e.Master.Data)
                {
                    contenido += "\t  <\n";
                    contenido += "\t  \"NAME\"= \"" + bd.Id + "\",\n";
                    contenido += "\t  \"DATA\"= [";

                    bool first = true;
                    bool anterior = false;
                    /*UserTypes*/
                    foreach (Simbolo sim in bd.Simbolos)
                    {
                        if (sim.Rol == Rol.USERTYPE)
                        {
                            anterior = true;
                            if (first)
                            {
                                contenido += "\n\t   <\n";
                                first = false;
                            }
                            else
                                contenido += ",\n\t   <\n";
                            contenido += "\t   \"CQL-TYPE\"= \"OBJECT\",\n";
                            contenido += "\t   \"NAME\"= \"" + sim.Id + "\",\n";
                            contenido += "\t   \"ATTRS\"= [\n";

                            Entorno ent = (Entorno)sim.Valor;

                            foreach (Simbolo att in ent.Simbolos)
                            {
                                contenido += "\t    <\n";
                                contenido += "\t    \"NAME\"= \"" + att.Id + "\",\n";
                                contenido += "\t    \"TYPE\"= \"" + att.Tipo.ToString() + "\"\n";
                                contenido += "\t    >";

                                if (ent.Simbolos.Last.Value.Equals(att))
                                    contenido += "\n";
                                else
                                    contenido += ",\n";
                            }

                            contenido += "\t   ]\n\t   >";
                        }
                    }

                    foreach (Simbolo sim in bd.Simbolos)
                    {
                        if (sim.Rol == Rol.PROCEDIMIENTO)
                        {
                            Procedimiento proc = (Procedimiento)sim.Valor;

                            if (anterior)
                                contenido += ",\n\t   <\n";
                            else
                            {
                                anterior = true;
                                contenido += "\n\t   <\n";
                            }

                            contenido += "\t   \"CQL-TYPE\"= \"PROCEDURE\",\n";

                            string proName;

                            if (sim.Id.Contains("-"))
                            {
                                proName = sim.Id.Split('-')[0];
                            }
                            else
                                proName = sim.Id;

                            contenido += "\t   \"NAME\"= \"" + proName + "\",\n";
                            contenido += "\t   \"PARAMETERS\"= [";

                            bool parametro = false;

                            if (proc.Parametro != null)
                            {
                                parametro = true;
                                foreach (Identificador par in proc.Parametro)
                                {
                                    contenido += "\n\t    <\n";
                                    contenido += "\t    \"NAME\"= \"" + par.Id + "\",\n";
                                    contenido += "\t    \"TYPE\"= \"" + par.Tipo.ToString() + "\",\n";
                                    contenido += "\t    \"AS\"= IN\n";
                                    contenido += "\t    >";

                                    if (!proc.Parametro.Last.Value.Equals(par))
                                        contenido += ",";
                                }
                            }

                            if (proc.Retorno != null)
                            {
                                if (parametro)
                                    contenido += ",";

                                foreach (Identificador par in proc.Retorno)
                                {
                                    contenido += "\n\t    <\n";
                                    contenido += "\t    \"NAME\"= \"" + par.Id + "\",\n";
                                    contenido += "\t    \"TYPE\"= \"" + par.Tipo.ToString() + "\",\n";
                                    contenido += "\t    \"AS\"= OUT\n";
                                    contenido += "\t    >";

                                    if (!proc.Retorno.Last.Value.Equals(par))
                                        contenido += ",";
                                }
                            }

                            contenido += "\n\t   ],\n";
                            contenido += "\t   \"INSTR\"= $\n";
                            if (!proc.Bloque.Cadena.Equals(""))
                            {
                                int fin = proc.Bloque.Cadena.Length-3;
                                if(fin >= 0)
                                    contenido += proc.Bloque.Cadena.Substring(1, fin) + "\n";
                            }
                            contenido += "\t   $\n\t   >";
                        }
                    }

                    /*Tablas*/
                    foreach (Simbolo sim in bd.Simbolos)
                    {
                        if (sim.Rol == Rol.TABLA)
                        {
                            Tabla tabla = (Tabla)sim.Valor;

                            if (anterior)
                                contenido += ",\n\t   <\n";
                            else
                            {
                                anterior = true;
                                contenido += "\n\t   <\n";
                            }

                            contenido += "\t   \"CQL-TYPE\"= \"TABLE\",\n";
                            contenido += "\t   \"NAME\"= \"" + sim.Id + "\",\n";
                            contenido += "\t   \"COLUMNS\"= [";

                            foreach (Simbolo col in tabla.Cabecera.Simbolos)
                            {
                                contenido += "\n\t    <\n";
                                contenido += "\t    \"NAME\"= \"" + col.Id + "\",\n";
                                contenido += "\t    \"TYPE\"= \"" + col.Tipo.ToString() + "\",\n";
                                contenido += "\t    \"PK\"= " + (col.Rol == Rol.PRIMARY ? "TRUE" : "FALSE") + "\n";
                                contenido += "\t    >";

                                if (!tabla.Cabecera.Simbolos.Last.Value.Equals(col))
                                    contenido += ",";
                            }

                            contenido += "\n\t   ],\n";
                            contenido += "\t   \"DATA\"= [";

                            foreach (Entorno ent in tabla.Datos)
                            {
                                contenido += "\n\t    <";
                                foreach (Simbolo data in ent.Simbolos)
                                {
                                    contenido += "\n\t     \"" + data.Id + "\"= ";

                                    if (data.Valor is Objeto obj)
                                        contenido += obj.ToString2();
                                    else if (data.Valor is Collection coll)
                                        contenido += coll.ToString2();
                                    else if (data.Valor is Cadena cad)
                                        contenido += cad.ToString2();
                                    else if (data.Valor is Date dat)
                                        contenido += dat.ToString2();
                                    else if (data.Valor is Time tim)
                                        contenido += tim.ToString2();
                                    else
                                        contenido += data.Valor.ToString();

                                    if (!ent.Simbolos.Last.Value.Equals(data))
                                        contenido += ",";
                                }
                                contenido += "\n\t    >";

                                if (!tabla.Datos.Last.Value.Equals(ent))
                                    contenido += ",";
                            }

                            contenido += "\n\t   ]\n\t   >";
                        }
                    }

                    contenido += "\n\t  ]\n";
                    contenido += "\t  >";

                    if (e.Master.Data.Last.Value.Equals(bd))
                        contenido += "\n";
                    else
                        contenido += ",\n";
                }
                contenido += "\t],\n";

                contenido += "\t\"USERS\"= [\n";
                foreach (Usuario user in e.Master.Usuarios)
                {
                    contenido += "\t  <\n";
                    contenido += "\t  \"NAME\"= \"" + user.Id + "\",\n";
                    contenido += "\t  \"PASSWORD\"= \"" + user.Password + "\",\n";
                    contenido += "\t  \"PERMISSIONS\"= [\n";

                    foreach (string bd in user.Permisos)
                    {
                        contenido += "\t   <\n";
                        contenido += "\t   \"NAME\"= \"" + bd + "\"\n\t   >";

                        if (user.Permisos.Last.Value.Equals(bd))
                            contenido += "\n";
                        else
                            contenido += ",\n";
                    }

                    contenido += "\t  ]\n\t  >";

                    if (e.Master.Usuarios.Last.Value.Equals(user))
                        contenido += "\n";
                    else
                        contenido += ",\n";
                }
                contenido += "\t]\n";

                contenido += ">$\n";

                writer.Write(contenido);
                writer.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Commit" + ex.Message);
            }
            finally
            {
                if (writer != null)
                    writer.Close();
            }

            return null;
        }
    }
}
