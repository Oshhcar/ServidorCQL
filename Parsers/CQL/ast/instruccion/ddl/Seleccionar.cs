using GramaticasCQL.Parsers.CQL.ast.entorno;
using GramaticasCQL.Parsers.CQL.ast.expresion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GramaticasCQL.Parsers.CQL.ast.instruccion.ddl
{
    class Seleccionar : Instruccion
    {
        public Seleccionar(LinkedList<Expresion> columnas, string id, int linea, int columna) : base(linea, columna)
        {
            Columnas = columnas;
            Id = id;
            Mostrar = true;
        }

        public Seleccionar(LinkedList<Expresion> columnas, string id, Where where, int linea, int columna) : base(linea, columna)
        {
            Columnas = columnas;
            Id = id;
            Where = where;
            Mostrar = true;
        }

        public Seleccionar(LinkedList<Expresion> columnas, string id, LinkedList<Identificador> order, int linea, int columna) : base(linea, columna)
        {
            Columnas = columnas;
            Id = id;
            Order = order;
            Mostrar = true;
        }

        public Seleccionar(LinkedList<Expresion> columnas, string id, Where where, LinkedList<Identificador> order, int linea, int columna) : base(linea, columna)
        {
            Columnas = columnas;
            Id = id;
            Where = where;
            Order = order;
            Mostrar = true;
        }

        public Seleccionar(LinkedList<Expresion> columnas, string id, Expresion limit, int linea, int columna) : base(linea, columna)
        {
            Columnas = columnas;
            Id = id;
            Limit = limit;
            Mostrar = true;
        }

        public Seleccionar(LinkedList<Expresion> columnas, string id, Where where, Expresion limit, int linea, int columna) : base(linea, columna)
        {
            Columnas = columnas;
            Id = id;
            Where = where;
            Limit = limit;
            Mostrar = true;
        }

        public Seleccionar(LinkedList<Expresion> columnas, string id, LinkedList<Identificador> order, Expresion limit, int linea, int columna) : base(linea, columna)
        {
            Columnas = columnas;
            Id = id;
            Order = order;
            Limit = limit;
            Mostrar = true;
        }

        public Seleccionar(LinkedList<Expresion> columnas, string id, Where where, LinkedList<Identificador> order, Expresion limit, int linea, int columna) : base(linea, columna)
        {
            Columnas = columnas;
            Id = id;
            Where = where;
            Order = order;
            Limit = limit;
            Mostrar = true;
        }

        public LinkedList<Expresion> Columnas;
        public string Id { get; set; }
        public Where Where { get; set; }
        public LinkedList<Identificador> Order { get; set; }
        public Expresion Limit { get; set; }
        public bool Mostrar { get; set; }

        public override object Ejecutar(Entorno e, bool funcion, bool ciclo, bool sw, bool tc, LinkedList<Salida> log, LinkedList<Error> errores)
        {
            BD actual = e.Master.Actual;
            if (actual != null)
            {
                Simbolo sim = actual.GetTabla(Id);

                if (sim != null)
                {
                    Tabla tabla = (Tabla)sim.Valor;

                    LinkedList<Entorno> datos = new LinkedList<Entorno>();

                    if (Order != null)
                    {
                        if (tabla.Datos.Count() > 1)
                        {
                            foreach (Entorno ent in tabla.Datos)
                            {
                                Entorno entActual = new Entorno(null, new LinkedList<Simbolo>());

                                foreach (Simbolo simActual in ent.Simbolos)
                                {
                                    entActual.Add(new Simbolo(simActual.Tipo, simActual.Rol, simActual.Id, simActual.Valor));
                                }

                                datos.AddLast(entActual);
                            }

                            e.Master.EntornoActual = tabla.Datos.ElementAt(0);

                            if (Order.Count() >= 1)
                            {
                                for (int j = Order.Count() - 1; j >= 0; j--)
                                {
                                    Identificador ident = Order.ElementAt(j);

                                    LinkedList<Entorno> tmp = new LinkedList<Entorno>();
                                    IEnumerable<Entorno> ordered;

                                    object identValor = ident.GetValor(e, log, errores);

                                    if (identValor != null)
                                    {

                                        if (identValor is Throw)
                                            return identValor;

                                        if (ident.Tipo.IsString() || ident.Tipo.IsDate() || ident.Tipo.IsTime())
                                            ordered = datos.OrderBy(p => p.GetCualquiera(ident.GetId()).Valor.ToString()).AsEnumerable();
                                        else if (ident.Tipo.IsInt())
                                            ordered = datos.OrderBy(p => (int)p.GetCualquiera(ident.GetId()).Valor).AsEnumerable();
                                        else if (ident.Tipo.IsDouble())
                                            ordered = datos.OrderBy(p => (double)p.GetCualquiera(ident.GetId()).Valor).AsEnumerable();
                                        else
                                        {
                                            errores.AddLast(new Error("Semántico", "Solo se puede usar la cláusula Order By sobre datos primitivos.", Linea, Columna));
                                            return null;
                                        }

                                        if (ident.IsASC)
                                        {
                                            foreach (Entorno eTmp in ordered)
                                            {
                                                tmp.AddLast(eTmp);
                                            }
                                        }
                                        else
                                        {
                                            for (int i = ordered.Count() - 1; i >= 0; i--)
                                            {
                                                tmp.AddLast(ordered.ElementAt(i));
                                            }
                                        }
                                        datos = tmp;
                                    }
                                    else
                                        return null;
                                }
                            }
                            /*
                            else if (Order.Count() >= 2)
                            {
                                Identificador ident = Order.ElementAt(0);
                                Identificador ident2 = Order.ElementAt(1);

                                LinkedList<Entorno> tmp = new LinkedList<Entorno>();
                                IEnumerable<Entorno> ordered;

                                object identValor = ident.GetValor(e, log, errores);
                                object identValor2 = ident2.GetValor(e, log, errores);

                                if (identValor != null && identValor2 != null)
                                {
                                    if (ident.Tipo.IsString() || ident.Tipo.IsDate() || ident.Tipo.IsTime() || ident.Tipo.IsInt() || ident.Tipo.IsDouble())
                                        if(ident2.Tipo.IsString() || ident2.Tipo.IsDate() || ident2.Tipo.IsTime() || ident2.Tipo.IsInt() || ident2.Tipo.IsDouble())
                                            ordered = datos.OrderBy(p => Tuple.Create(p.GetCualquiera(ident.GetId()).Valor.ToString(), p.GetCualquiera(ident2.GetId()).Valor.ToString()), new ComparaTupla()).AsEnumerable();
                                        else
                                        {
                                            errores.AddLast(new Error("Semántico", "Solo se puede usar la cláusula Order By sobre datos primitivos.", Linea, Columna));
                                            return null;
                                        }
                                    else
                                    {
                                        errores.AddLast(new Error("Semántico", "Solo se puede usar la cláusula Order By sobre datos primitivos.", Linea, Columna));
                                        return null;
                                    }

                                    if (ident.IsASC)
                                    {
                                        foreach (Entorno eTmp in ordered)
                                        {
                                            tmp.AddLast(eTmp);
                                        }
                                    }
                                    else
                                    {
                                        for (int i = ordered.Count() - 1; i >= 0; i--)
                                        {
                                            tmp.AddLast(ordered.ElementAt(i));
                                        }
                                    }
                                    datos = tmp;
                                }
                                else
                                    return null;
                            }
                            */
                        }
                        else
                        {
                            datos = tabla.Datos;
                        }
                    }
                    else
                    {
                        datos = tabla.Datos;
                    }

                    LinkedList<Entorno> data = new LinkedList<Entorno>();

                    foreach (Entorno ent in datos)
                    {
                        e.Master.EntornoActual = ent;
                        Entorno entActual = new Entorno(null, new LinkedList<Simbolo>());

                        if (Where != null)
                        {
                            object valWhere = Where.GetValor(e, log, errores);
                            if (valWhere != null)
                            {
                                if (valWhere is Throw)
                                    return valWhere;

                                if (Where.Tipo.IsBoolean())
                                {
                                    if (!(bool)valWhere)
                                        continue;
                                }
                                else
                                {
                                    errores.AddLast(new Error("Semántico", "Cláusula Where debe ser booleana.", Linea, Columna));
                                    return null;
                                }
                            }
                            else
                                return null;
                        }

                        if (Columnas != null)
                        {
                            int numCol = 1;

                            foreach (Expresion colExp in Columnas)
                            {
                                Simbolo simActual = new Simbolo();

                                object valColExp = colExp.GetValor(e, log, errores);

                                if (valColExp != null)
                                {
                                    if (valColExp is Throw)
                                        return valColExp;

                                    simActual.Tipo = colExp.Tipo;
                                    simActual.Rol = Rol.COLUMNA;

                                    if (colExp is Identificador iden)
                                    {
                                        simActual.Id = iden.GetId().ToLower();
                                    }
                                    else
                                    {
                                        simActual.Id = "columna" + numCol++;
                                    }
                                    simActual.Valor = valColExp;
                                }
                                else
                                    return null;

                                entActual.Add(simActual);
                            }
                        }
                        else
                        {
                            foreach (Simbolo col in ent.Simbolos)
                            {
                                entActual.Add(new Simbolo(col.Tipo, col.Rol, col.Id, col.Valor));
                            }
                        }

                        data.AddLast(entActual);
                    }

                    e.Master.EntornoActual = null;

                    int limite;

                    if (Limit != null)
                    {
                        object valLimit = Limit.GetValor(e, log, errores);
                        if (valLimit != null)
                        {
                            if (valLimit is Throw)
                                return valLimit;

                            if (Limit.Tipo.IsInt())
                            {
                                limite = (int)valLimit-1;

                                if((int)valLimit < 0)
                                    errores.AddLast(new Error("Semántico", "El Límite debe ser entero positivo.", Linea, Columna));
                            }
                            else
                            {
                                Casteo cast = new Casteo(new Tipo(entorno.Type.INT), new Literal(Limit.Tipo, valLimit, 0, 0), 0, 0)
                                {
                                    Mostrar = false
                                };
                                valLimit = cast.GetValor(e, log, errores);

                                if (valLimit != null)
                                {
                                    if (valLimit is Throw)
                                        return valLimit;

                                    limite = (int)valLimit-1;
                                    if((int)valLimit < 0)
                                        errores.AddLast(new Error("Semántico", "El Límite debe ser entero positivo.", Linea, Columna));
                                }
                                else
                                {
                                    errores.AddLast(new Error("Semántico", "El Límite debe ser de tipo Entero.", Linea, Columna));
                                    return null;
                                }
                            }
                        }
                        else
                            return null;
                    }
                    else
                        limite = data.Count()-1;

                    limite = limite > data.Count() - 1 ? data.Count() - 1 : limite;

                    if (Mostrar)
                    {
                        string salida;

                        if (data.Count() >= 1)
                        {
                            salida = "<table> \n";

                            salida += "<tr> \n";

                            foreach (Simbolo col in data.ElementAt(0).Simbolos)
                            {
                                salida += "\t<th>" + col.Id + "</th>\n";
                            }

                            salida += "</tr>\n";

                            for (int i = 0; i <= limite; i++)
                            {
                                Entorno ent = data.ElementAt(i);
                                salida += "<tr>\n";

                                foreach (Simbolo col in ent.Simbolos)
                                {
                                    salida += "\t<td>" + col.Valor.ToString() + "</td>\n";
                                }

                                salida += "</tr>\n";
                            }

                            salida += "</table>\n\n\n";

                        }
                        else
                        {
                            salida = "No hay datos en la consulta.\n\n";
                        }

                        log.AddLast(new Salida(2, salida));
                        return null;
                    }
                    else
                        return data;
                }
                else
                    return new Throw("TableDontExists", Linea, Columna);
                    //errores.AddLast(new Error("Semántico", "No existe una Tabla con el id: " + Id + " en la base de datos.", Linea, Columna));
            }
            else
                return new Throw("UseBDException", Linea, Columna);
                //errores.AddLast(new Error("Semántico", "No se ha seleccionado una base de datos, no se pudo Actualizar.", Linea, Columna));

            //return null;
        }

        class ComparaTupla : IComparer<Tuple<string, string>>
        {
            public int Compare(Tuple<string, string> x, Tuple<string, string> y)
            {
                if (x.Item1.ToString().Equals(y.Item1.ToString()))
                {
                    if (x.Item2.ToString().Equals(y.Item2.ToString()))
                        return 0;
                    return x.Item2.ToString().CompareTo(y.Item2.ToString());
                }
                return x.Item1.ToString().CompareTo(y.Item1.ToString());
            }
        }
    }
}
