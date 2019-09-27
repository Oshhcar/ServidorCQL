using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GramaticasCQL.Parsers.CQL.ast.entorno;

namespace GramaticasCQL.Parsers.LUP.ast.sentencia
{
    public class Struc : Sentencia
    {
        public Struc(string nombre, int linea, int columna) : base(linea, columna)
        {
            Nombre = nombre;
        }

        public string Nombre { get; set; }

        public override object Ejecutar(MasterBD master, LinkedList<Salida> log, LinkedList<Salida> respuesta, LinkedList<Error> errores)
        {
            Usuario user = master.GetUsuario(Nombre.Trim());

            if (user != null)
            {
                string cad = "[+DATABASES]\n";

                foreach (BD bd in master.Data)
                {
                    if (user.GetPermiso(bd.Id))
                    {
                        cad += "[+DATABASE]\n";
                        cad += "[+NAME]\n\t" + bd.Id + "\n[-NAME]\n";

                        cad += "[+TABLES]\n";

                        foreach (Simbolo t in bd.Simbolos)
                        {
                            if (t.Rol == Rol.TABLA)
                            {
                                cad += "[+TABLE]\n";

                                Tabla tabla = (Tabla)t.Valor;
                                cad += "[+NAME]\n\t" + t.Id + "\n[-NAME]\n";
                                cad += "[+COLUMNS]\n";

                                foreach (Simbolo col in tabla.Cabecera.Simbolos)
                                {
                                    cad += "[+COLUMN]\n";
                                    cad += "[+NAME]\n\t" + col.Id + "\n[-NAME]\n";
                                    cad += "[+TYPE]\n\t" + col.Tipo.ToString() + "\n[-TYPE]\n";
                                    cad += "[+PK]\n\t" + (col.Rol == Rol.PRIMARY) + "\n[-PK]\n";
                                    cad += "[-COLUMN]\n";
                                }

                                cad += "[-COLUMNS]\n";
                                cad += "[-TABLE]\n";
                            }
                        }

                        cad += "[-TABLES]\n";

                        cad += "[+TYPES]\n";

                        foreach (Simbolo ty in bd.Simbolos)
                        {
                            if (ty.Rol == Rol.USERTYPE)
                            {
                                cad += "[+TYPE]\n";

                                Entorno objeto = (Entorno)ty.Valor;
                                cad += "[+NAME]\n\t" + ty.Id + "\n[-NAME]\n";
                                cad += "[+ATTRIBUTES]\n";

                                foreach (Simbolo col in objeto.Simbolos)
                                {
                                    cad += "[+COLUMN]\n";
                                    cad += "[+NAME]\n\t" + col.Id + "\n[-NAME]\n";
                                    cad += "[+TYPE]\n\t" + col.Tipo.ToString() + "\n[-TYPE]\n";
                                    cad += "[-COLUMN]\n";
                                }

                                cad += "[-ATTRIBUTES]\n";
                                cad += "[-TYPE]\n";
                            }
                        }

                        cad += "[-TYPES]\n";

                        cad += "[+PROCEDURES]\n";

                        foreach (Simbolo p in bd.Simbolos)
                        {
                            if (p.Rol == Rol.PROCEDIMIENTO)
                            {
                                cad += "[+PROCEDURE]\n";
                                cad += "[+NAME]\n\t" + p.Id + "\n[-NAME]\n";
                                cad += "[-PROCEDURE]\n";
                            }
                        }

                        cad += "[-PROCEDURES]\n";

                        cad += "[-DATABASE]\n";
                    }
                }

                cad += "[-DATABASES]";

                respuesta.AddLast(new Salida(1, cad));
            }

            return null;
        }
    }
}