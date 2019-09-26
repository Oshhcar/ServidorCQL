using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GramaticasCQL.Parsers.CQL.ast.entorno;
using GramaticasCQL.Parsers.CQL.ast.expresion;

namespace GramaticasCQL.Parsers.CQL.ast.instruccion.ddl
{
    class Insertar : Instruccion
    {
        public Insertar(string id, LinkedList<Expresion> valores, int linea, int columna) : base(linea, columna)
        {
            Id = id;
            Valores = valores;
        }

        public Insertar(string id, LinkedList<string> columnas, LinkedList<Expresion> valores, int linea, int columna) : base(linea, columna)
        {
            Id = id;
            Columnas = columnas;
            Valores = valores;
        }

        public string Id { get; set; }
        public LinkedList<Expresion> Valores { get; set; }
        public LinkedList<string> Columnas { get; set; }
        public bool Correcto { get; set; }

        public override object Ejecutar(Entorno e, bool funcion, bool ciclo, bool sw, bool tc, LinkedList<Salida> log, LinkedList<Error> errores)
        {
            BD actual = e.Master.Actual;

            if (actual != null)
            {
                Simbolo sim = actual.GetTabla(Id);

                if (sim != null)
                {
                    Tabla tabla = (Tabla)sim.Valor;

                    if (Columnas != null)
                    {
                        if (Columnas.Count() == Valores.Count())
                        {
                            Entorno datos = tabla.GetNuevaFila();
                            LinkedList<Simbolo> primary = new LinkedList<Simbolo>();

                            for (int i = 0; i < Columnas.Count(); i++)
                            {
                                Expresion valor = Valores.ElementAt(i);
                                object valValor = valor.GetValor(e, log, errores);
                                if (valValor != null)
                                {
                                    if (valValor is Throw)
                                        return valValor;

                                    string col = Columnas.ElementAt(i);
                                    Simbolo dato = datos.GetCualquiera(col);
                                    if (dato != null)
                                    {
                                        if (dato.Tipo.IsCounter())
                                        {
                                            return new Throw("CounterTypeException", Linea, Columna);
                                            //dato.Valor = tabla.Contador;
                                            //continue;
                                        }

                                        if (dato.Tipo.Equals(valor.Tipo))
                                        {
                                            dato.Valor = valValor;

                                            if (dato.Rol == Rol.PRIMARY)
                                            {
                                                if (valValor is Null)
                                                {
                                                    errores.AddLast(new Error("Semántico", "Una llave primaria no puede ser Null.", Linea, Columna));
                                                    return null;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            Casteo cast = new Casteo(dato.Tipo, new Literal(valor.Tipo, valValor, 0, 0), 0, 0)
                                            {
                                                Mostrar = false
                                            };
                                            valValor = cast.GetValor(e, log, errores);

                                            if (valValor != null)
                                            {
                                                if (valValor is Throw)
                                                    return valValor;

                                                dato.Valor = valValor;
                                                continue;
                                            }

                                            return new Throw("ValuesException", Linea, Columna);
                                            //errores.AddLast(new Error("Semántico", "El tipo de la expresión no corresponde al tipo en la columna: " + sim.Id + ".", Linea, Columna));
                                            //return null;
                                        }
                                    }
                                    else
                                    {
                                        errores.AddLast(new Error("Semántico", "No hay un campo con el id: " + col +" en la Tabla.", Linea, Columna));
                                        return null;
                                    }
                                }
                                else
                                    return null;
                                
                            }

                            foreach (Simbolo col in datos.Simbolos)
                            {
                                if (col.Tipo.IsCounter())
                                    col.Valor = tabla.Contador;

                                if (col.Rol == Rol.PRIMARY)
                                {
                                    if (col.Valor is Null)
                                    {
                                        errores.AddLast(new Error("Semántico", "Una llave primaria no puede ser Null.", Linea, Columna));
                                        return null;
                                    }
                                    primary.AddLast(col);
                                }
                            }

                            
                            if (!tabla.Insertar(datos, primary))
                                errores.AddLast(new Error("Semántico", "No se pueden insertar valores con la misma llave primaria.", Linea, Columna));
                            else
                            {
                                tabla.Contador++;
                                Correcto = true;
                            }

                            return null;
                        }
                        else
                            errores.AddLast(new Error("Semántico", "La lista de campos no corresponde a la lista de valores.", Linea, Columna));
                    }
                    else
                    {
                        if (tabla.Cabecera.Simbolos.Count() == Valores.Count())
                        {
                            Entorno datos = new Entorno(null, new LinkedList<Simbolo>());
                            LinkedList<Simbolo> primary = new LinkedList<Simbolo>();

                            for(int i = 0; i < Valores.Count(); i++)
                            {
                                Expresion valor = Valores.ElementAt(i);
                                object valValor = valor.GetValor(e, log, errores);
                                if (valValor != null)
                                {
                                    if (valValor is Throw)
                                        return valValor;

                                    Simbolo col = tabla.Cabecera.Simbolos.ElementAt(i);

                                    if (col.Tipo.IsCounter())
                                    {
                                        return new Throw("CounterTypeException", Linea, Columna);
                                        //errores.AddLast(new Error("Semántico", "No se puede insertar un valor en una columna tipo Counter.", Linea, Columna));
                                        //return null;
                                    }

                                    if (!col.Tipo.Equals(valor.Tipo))
                                    {
                                        Casteo cast = new Casteo(col.Tipo, new Literal(valor.Tipo, valValor, 0, 0), 0, 0)
                                        {
                                            Mostrar = false
                                        };
                                        valValor = cast.GetValor(e, log, errores);

                                        if (valValor == null)
                                        {
                                            if (valValor is Throw)
                                                return valValor;

                                            return new Throw("ValuesException", Linea, Columna);
                                            //errores.AddLast(new Error("Semántico", "El tipo de la expresión no corresponde al tipo en la columna: " + col.Id + ".", Linea, Columna));
                                            //return null;
                                        }
                                    }

                                    Simbolo dato = new Simbolo(col.Tipo, col.Rol, col.Id, valValor);
                                    if (col.Rol == Rol.PRIMARY)
                                    {
                                        if (valValor is Null)
                                        {
                                            errores.AddLast(new Error("Semántico", "Una llave primaria no puede ser Null.", Linea, Columna));
                                            return null;
                                        }
                                        primary.AddLast(dato);
                                    }
                                    datos.Add(dato);
                                }
                                else
                                    return null;
                            }

                            if (!tabla.Insertar(datos, primary))
                                errores.AddLast(new Error("Semántico", "No se pueden insertar valores con la misma llave primaria.", Linea, Columna));
                            else
                                Correcto = true;
                            return null;
                        }
                        else
                            return new Throw("ValuesException", Linea, Columna);
                            //errores.AddLast(new Error("Semántico", "Los valores no corresponden a las columnas en la Tabla.", Linea, Columna));
                    }

                }
                else
                    return new Throw("TableDontExists", Linea, Columna);
                    //errores.AddLast(new Error("Semántico", "No existe una Tabla con el id: " + Id + " en la base de datos.", Linea, Columna));
            }
            else
                return new Throw("UseBDException", Linea, Columna);
                //errores.AddLast(new Error("Semántico", "No se ha seleccionado una base de datos, no se pudo Insertar.", Linea, Columna));

            return null;
        }
    }
}
