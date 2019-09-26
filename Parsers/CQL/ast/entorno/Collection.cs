using GramaticasCQL.Parsers.CQL.ast.expresion;
using GramaticasCQL.Parsers.CQL.ast.instruccion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GramaticasCQL.Parsers.CQL.ast.entorno
{
    class Collection
    {
        public Collection(Tipo tipo)
        {
            Tipo = tipo;
            Valores = new LinkedList<CollectionValue>();
            Posicion = 0;
        }

        public Tipo Tipo { get; set; }
        public LinkedList<CollectionValue> Valores { get; set; }
        public int Posicion { get; set; }

        public bool Castear(Tipo t, Entorno e, LinkedList<Salida> log, LinkedList<Error> errores)
        {
            foreach (CollectionValue value in Valores)
            {
                if (Tipo.IsMap())
                {
                    if (!Tipo.Clave.Equals(t.Clave))
                    {
                        Casteo castClave = new Casteo(t.Clave, new Literal(Tipo.Clave, value.Clave, 0, 0), 0, 0)
                        {
                            Mostrar = false
                        };

                        object valorCastClave = castClave.GetValor(e, log, errores);

                        if (valorCastClave != null)
                        {
                            if (valorCastClave is Throw)
                                return false;

                            value.Clave = valorCastClave;
                        }
                        else
                            return false;
                    }
                }

                if (!Tipo.Valor.Equals(t.Valor))
                {
                    Casteo cast = new Casteo(t.Valor, new Literal(Tipo.Valor, value.Valor, 0, 0), 0, 0)
                    {
                        Mostrar = false
                    };

                    object valorCast = cast.GetValor(e, log, errores);

                    if (valorCast != null)
                    {
                        if (valorCast is Throw)
                            return false;

                        value.Valor = valorCast;
                        continue;
                    }
                    return false;
                }
            }
            Tipo = t;
            return true;
        }

        public override string ToString()
        {
            string cad;
            if (Tipo.IsMap())
            {
                cad = "[";
                foreach (CollectionValue value in Valores)
                {
                    cad += value.Clave.ToString() + " : " + value.Valor.ToString();

                    if (!Valores.Last.Value.Equals(value))
                        cad += ", ";
                }
                cad += "]";
                return cad;
            }
            else if (Tipo.IsList())
            {
                cad = "[";
                foreach (CollectionValue value in Valores)
                {
                    cad += value.Valor.ToString();
                    if (!Valores.Last.Value.Equals(value))
                        cad += ", ";
                }
                cad += "]";
                return cad;
            }
            else if (Tipo.IsSet())
            {
                cad = "{";
                foreach (CollectionValue value in Valores)
                {
                    cad += value.Valor.ToString();
                    if (!Valores.Last.Value.Equals(value))
                        cad += ", ";
                }
                cad += "}";
                return cad;
            }
            return base.ToString();
        }

        public string ToString2()
        {
            string cad;

            if (Tipo.IsMap())
            {
                cad = "<";

                foreach (CollectionValue value in Valores)
                {
                    cad += "\"" + value.Clave.ToString() + "\"= ";

                    if (value.Valor is Objeto obj2)
                        cad += obj2.ToString2();
                    else if (value.Valor is Collection coll)
                        cad += coll.ToString2();
                    else if (value.Valor is Cadena cade)
                        cad += cade.ToString2();
                    else if (value.Valor is Date dat)
                        cad += dat.ToString2();
                    else if (value.Valor is Time tim)
                        cad += tim.ToString2();
                    else
                        cad += value.Valor.ToString();

                    if (!Valores.Last.Value.Equals(value))
                        cad += ", ";
                }

                cad += ">";
            }
            else
            {
                //if (Tipo.IsList())
                    cad = "[";
                //else
                    //cad = "{";

                foreach (CollectionValue value in Valores)
                {
                    if (value.Valor is Objeto obj2)
                        cad += obj2.ToString2();
                    else if (value.Valor is Collection coll)
                        cad += coll.ToString2();
                    else if (value.Valor is Cadena cade)
                        cad += cade.ToString2();
                    else if (value.Valor is Date dat)
                        cad += dat.ToString2();
                    else if (value.Valor is Time tim)
                        cad += tim.ToString2();
                    else
                        cad += value.Valor.ToString();

                    if (!Valores.Last.Value.Equals(value))
                        cad += ", ";
                }
                //if (Tipo.IsList())
                    cad += "]";
                //else
                    //cad += "}";
            }

            return cad;
        }

        public void Insert(object clave, object valor)
        {
            Valores.AddLast(new CollectionValue(clave.ToString(), valor ?? Predefinido()));

        }

        public object Get(object clave)
        {
            foreach (CollectionValue val in Valores)
            {
                if (val.Clave.Equals(clave.ToString()))
                    return val.Valor;
            }
            return null;
        }

        public CollectionValue GetCollection(object clave)
        {
            foreach (CollectionValue val in Valores)
            {
                if (val.Clave.Equals(clave.ToString()))
                    return val;
            }
            return null;
        }

        public bool Set(object clave, object valor)
        {
            foreach (CollectionValue val in Valores)
            {
                if (val.Clave.Equals(clave.ToString()))
                {
                    val.Valor = valor ?? Predefinido();
                    return true;
                }
            }
            return false;
        }

        public bool Remove(object clave)
        {
            foreach (CollectionValue val in Valores)
            {
                if (val.Clave.Equals(clave.ToString()))
                {
                    Valores.Remove(val);
                    return true;
                }
            }
            return false;
        }

        public bool RemoveValor(object valor)
        {
            foreach (CollectionValue val in Valores)
            {
                if (val.Valor.ToString().Equals(valor.ToString()))
                {
                    Valores.Remove(val);
                    return true;
                }
            }
            return false;
        }

        public bool RemoveList(object clave)
        {
            foreach (CollectionValue val in Valores)
            {
                if (val.Clave.Equals(clave.ToString()))
                {
                    val.Valor = Predefinido();
                    return true;
                }
            }
            return false;
        }

        //Para list
        public bool Contains(object valor)
        {
            foreach (CollectionValue val in Valores)
            {
                /*Provar si se puede hacer una relacional*/
                if (val.Valor.Equals(valor))
                {
                    return true;
                }
            }
            return false;
        }

        //Para set
        public void Ordenar()
        {
            LinkedList<CollectionValue> tmp = new LinkedList<CollectionValue>();
            IEnumerable<CollectionValue> ordered;

            if (Tipo.Valor.IsString() || Tipo.Valor.IsDate() || Tipo.Valor.IsTime())
                ordered = Valores.OrderBy(p => p.Valor.ToString()).AsEnumerable();
            else if (Tipo.Valor.IsInt())
                ordered = Valores.OrderBy(p => (int)p.Valor).AsEnumerable();
            else if (Tipo.Valor.IsDouble())
                ordered = Valores.OrderBy(p => (double)p.Valor).AsEnumerable();
            else
                ordered = null;

            if (ordered != null)
            {
                int contador = 0;

                foreach (CollectionValue value in ordered)
                {
                    value.Clave = (contador++).ToString();
                    tmp.AddLast(value);
                }
                Valores = tmp;
            }
        }

        public object Predefinido()
        {
            if (Tipo.Valor.IsInt())
                return 0;
            else if (Tipo.Valor.IsDouble())
                return 0.0;
            else if (Tipo.Valor.IsBoolean())
                return false;
            else
                return new Null();
        }

        public void Recorrer()
        {
            Console.WriteLine("***Map***");
            foreach (CollectionValue val in Valores)
            {
                Console.WriteLine("\t" + val.Clave.ToString() + ":" + val.Valor.ToString());
            }
            Console.WriteLine("*********");
        }
    }
}
