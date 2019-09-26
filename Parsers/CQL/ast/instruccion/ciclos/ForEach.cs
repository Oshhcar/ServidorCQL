using GramaticasCQL.Parsers.CQL.ast.entorno;
using GramaticasCQL.Parsers.CQL.ast.expresion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GramaticasCQL.Parsers.CQL.ast.instruccion.ciclos
{
    class ForEach : Instruccion
    {
        public ForEach(LinkedList<Identificador> parametro, string id, Bloque bloque, int linea, int columna) : base(linea, columna)
        {
            Parametro = parametro;
            Id = id;
            Bloque = bloque;
        }

        public LinkedList<Identificador> Parametro { get; set; }
        public string Id { get; set; }
        public Bloque Bloque { get; set; }

        public override object Ejecutar(Entorno e, bool funcion, bool ciclo, bool sw, bool tc, LinkedList<Salida> log, LinkedList<Error> errores)
        {
            Simbolo sim = e.Get(Id);

            if (sim != null)
            {
                if (sim.Tipo.IsCursor())
                {
                    Cursor cursor = (Cursor)sim.Valor;

                    if (cursor.Data != null)
                    {
                        if (cursor.Data.Count() > 0)
                        {
                            if (Parametro != null)
                            {
                                if (Parametro.Count() == cursor.Data.ElementAt(0).Simbolos.Count())
                                {
                                    foreach (Entorno ent in cursor.Data)
                                    {
                                        Entorno local = new Entorno(e);

                                        for (int i = 0; i < Parametro.Count(); i++)
                                        {
                                            Identificador par = Parametro.ElementAt(i);
                                            Simbolo col = ent.Simbolos.ElementAt(i);

                                            Simbolo var;

                                            if (par.Tipo.Equals(col.Tipo))
                                            {
                                                var = new Simbolo(par.Tipo, Rol.VARIABLE, par.Id.ToLower(), col.Valor);
                                            }
                                            else
                                            {
                                                /*Agregar Collection*/

                                                Casteo cast = new Casteo(par.Tipo, new Literal(col.Tipo, col.Valor, 0, 0), 0, 0)
                                                {
                                                    Mostrar = false
                                                };
                                                object valCol = cast.GetValor(e, log, errores);

                                                if (valCol != null)
                                                {
                                                    if (valCol is Throw)
                                                        return valCol;
                                                      
                                                    var = new Simbolo(par.Tipo, Rol.VARIABLE, par.Id.ToLower(), valCol);
                                                }
                                                else
                                                {
                                                    errores.AddLast(new Error("Semántico", "Los Tipos en los parametros no coinciden con los del Cursor.", Linea, Columna));
                                                    return null;
                                                }
                                            }
                                            local.Add(var);
                                        }

                                        object obj = Bloque.Ejecutar(local, funcion, true, sw, tc, log, errores);

                                        if (obj is Break)
                                            break;
                                        else if (obj is Return)
                                            return obj;
                                        else if (obj is Throw)
                                            return obj;
                                    }
                                    return null;
                                }
                                else
                                    errores.AddLast(new Error("Semántico", "Los parametros no coinciden con los del Cursor.", Linea, Columna));
                            }
                            else
                                errores.AddLast(new Error("Semántico", "Los parametros no coinciden con los del Cursor.", Linea, Columna));
                        }
                    }
                    else
                        errores.AddLast(new Error("Semántico", "El Cursor esta cerrado, ejecute la sentecia Open primero.", Linea, Columna));
                }
                else
                    errores.AddLast(new Error("Semántico", "La variable: " + Id + " no es un cursor.", Linea, Columna));
            }
            return null;
        }
    }
}
