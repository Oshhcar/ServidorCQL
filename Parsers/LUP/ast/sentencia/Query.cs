using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GramaticasCQL.Parsers.CQL;
using GramaticasCQL.Parsers.CQL.ast;
using GramaticasCQL.Parsers.CQL.ast.entorno;

namespace GramaticasCQL.Parsers.LUP.ast.sentencia
{
    public class Query : Sentencia
    {
        public Query(string user, string data, int linea, int columna) : base(linea, columna)
        {
            User = user;
            Data = data;
        }

        public string User { get; set; }
        public string Data { get; set; }

        public override object Ejecutar(MasterBD master, LinkedList<Salida> log, LinkedList<Salida> respuesta, LinkedList<Error> errores)
        {
            if (!User.Equals(""))
            {
                Usuario user = master.GetUsuario(User);
                if (user != null)
                {
                    if (!Data.Equals(""))
                    {
                        master.UsuarioActual = user;

                        AnalizadorCQL analizador = new AnalizadorCQL();

                        if (analizador.AnalizarEntrada(Data))
                        {
                            ASTCQL ast = (ASTCQL)analizador.GenerarArbol(analizador.Raiz.Root);

                            if (ast != null)
                            {
                                try
                                {
                                    ast.Ejecutar(log, errores, master);
                                }
                                catch (Exception ex)
                                {
                                    respuesta.AddLast(new Salida(1, "[+MESSAGE]\n\t$Error en Data, revice las Instrucciones.\n" + ex.Message + "$[-MESSAGE]"));
                                }
                            }
                            else
                                respuesta.AddLast(new Salida(1, "[+MESSAGE]\n\t$Error en Data, revice las Instrucciones.$\n[-MESSAGE]"));
                        }
                        else
                            respuesta.AddLast(new Salida(1, "[+MESSAGE]\n\t$Error en Data, revice las Instrucciones.$\n[-MESSAGE]"));
                    }
                }
                else
                    respuesta.AddLast(new Salida(1, "[+MESSAGE]\n\t$El usuario que hizo la consulta no existe.$\n[-MESSAGE]"));
            }
            else
                respuesta.AddLast(new Salida(1, "[+MESSAGE]\n\t$El parametro User no puede estar vacío.$\n[-MESSAGE]"));

            return null;
        }
    }
}