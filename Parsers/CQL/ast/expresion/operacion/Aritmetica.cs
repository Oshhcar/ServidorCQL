using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GramaticasCQL.Parsers.CQL.ast.entorno;
using GramaticasCQL.Parsers.CQL.ast.instruccion;
using Type = GramaticasCQL.Parsers.CQL.ast.entorno.Type;

namespace GramaticasCQL.Parsers.CQL.ast.expresion.operacion
{
    class Aritmetica : Operacion
    {

        public Aritmetica(Expresion op1, Expresion op2, Operador op, int linea, int columna) : base(op1, op2, op, linea, columna) { }

        public override object GetValor(Entorno e, LinkedList<Salida> log, LinkedList<Error> errores)
        {
            object valOp1 = Op1.GetValor(e, log, errores);
            object valOp2 = Op2.GetValor(e, log, errores);

            if (valOp1 != null && valOp2 != null)
            {
                if (valOp1 is Throw)
                    return valOp1;

                if (valOp2 is Throw)
                    return valOp2;

                TipoDominante(errores);

                if (Tipo != null)
                {
                    switch (Tipo.Type)
                    {
                        case Type.STRING:
                            Cadena cad = new Cadena
                            {
                                Valor = valOp1.ToString() + valOp2.ToString()
                            };
                            return cad;
                        case Type.DOUBLE:
                            switch (Op)
                            {
                                case Operador.SUMA:
                                    return Convert.ToDouble(valOp1) + Convert.ToDouble(valOp2);
                                case Operador.RESTA:
                                    return Convert.ToDouble(valOp1) - Convert.ToDouble(valOp2);
                                case Operador.MULTIPLICACION:
                                    return Convert.ToDouble(valOp1) * Convert.ToDouble(valOp2);
                                case Operador.POTENCIA:
                                    return Math.Pow(Convert.ToDouble(valOp1), Convert.ToDouble(valOp2));
                                case Operador.MODULO:
                                    return Convert.ToDouble(valOp1) % Convert.ToDouble(valOp2);
                                case Operador.DIVISION:
                                    if (Convert.ToDouble(valOp2) != 0)
                                    {
                                        return Convert.ToDouble(valOp1) / Convert.ToDouble(valOp2);
                                    }
                                    //errores.AddLast(new Error("Semántico", "División entre 0.", Linea, Columna));
                                    return new Throw("ArithmeticException", Linea, Columna);

                            }
                            break;
                        case Type.INT:
                            switch (Op)
                            {
                                case Operador.SUMA:
                                    return Convert.ToInt32(valOp1) + Convert.ToInt32(valOp2);
                                case Operador.RESTA:
                                    return Convert.ToInt32(valOp1) - Convert.ToInt32(valOp2);
                                case Operador.MULTIPLICACION:
                                    return Convert.ToInt32(valOp1) * Convert.ToInt32(valOp2);
                                case Operador.POTENCIA:
                                    return Math.Pow(Convert.ToInt32(valOp1), Convert.ToInt32(valOp2));
                                case Operador.MODULO:
                                    return Convert.ToInt32(valOp1) % Convert.ToInt32(valOp2);
                                case Operador.DIVISION:
                                    if (Convert.ToInt32(valOp2) != 0)
                                    {
                                        return Convert.ToInt32(valOp1) / Convert.ToInt32(valOp2);
                                    }
                                    //errores.AddLast(new Error("Semántico", "División entre 0.", Linea, Columna));
                                    return new Throw("ArithmeticException", Linea, Columna);

                            }
                            break;
                        case Type.MAP:
                            if(valOp1 is Null)
                                return new Throw("NullPointerException", Linea, Columna);
                            if(valOp2 is Null)
                                return new Throw("NullPointerException", Linea, Columna);

                            Collection map1 = (Collection)valOp1;
                            Collection map2 = (Collection)valOp2;

                            foreach (CollectionValue value in map2.Valores)
                            {
                                if (Op == Operador.SUMA)
                                {
                                    if (map1.Tipo.Clave.Equals(map2.Tipo.Clave) && map1.Tipo.Valor.Equals(map2.Tipo.Valor))
                                    {
                                        if (map1.Get(value.Clave) == null)
                                        {
                                            map1.Insert(value.Clave, value.Valor);
                                        }
                                        else
                                        {
                                            map1.Set(value.Clave, value.Valor);
                                            //errores.AddLast(new Error("Semántico", "Ya existe un valor con la clave: " + value.Clave.ToString() + " en Map.", Linea, Columna));
                                        }
                                    }
                                    else
                                    {
                                        Casteo cast1 = new Casteo(map1.Tipo.Clave, new Literal(map2.Tipo.Clave, value.Clave, 0, 0), 0, 0)
                                        {
                                            Mostrar = false
                                        };

                                        Casteo cast2 = new Casteo(map1.Tipo.Valor, new Literal(map2.Tipo.Valor, value.Valor, 0, 0), 0, 0)
                                        {
                                            Mostrar = false
                                        };

                                        object clave = cast1.GetValor(e, log, errores);
                                        object valor = cast2.GetValor(e, log, errores);

                                        if (clave != null && valor != null)
                                        {
                                            if (clave is Throw)
                                                return clave;

                                            if (valor is Throw)
                                                return valor;

                                            if (map1.Get(clave) == null)
                                            {
                                                map1.Insert(clave, valor);
                                            }
                                            else
                                            {
                                                map1.Set(clave, valor);
                                                //errores.AddLast(new Error("Semántico", "Ya existe un valor con la clave: " + clave.ToString() + " en Map.", Linea, Columna));
                                            }
                                            continue;
                                        }

                                        errores.AddLast(new Error("Semántico", "Los tipos de los parametros no coinciden con la clave:valor del Map.", Linea, Columna));
                                        return null;
                                    }
                                }
                                else if (Op == Operador.RESTA)
                                {
                                    if (map1.Tipo.Clave.Equals(map2.Tipo.Valor))
                                    {
                                        if (!map1.Remove(value.Valor))
                                            errores.AddLast(new Error("Semántico", "No existe un valor con la clave: " + value.Valor.ToString() + " en Map.", Linea, Columna));
                                    }
                                    else
                                    {
                                        Casteo cast = new Casteo(map1.Tipo.Clave, new Literal(map2.Tipo.Valor, value.Valor, 0, 0), 0, 0)
                                        {
                                            Mostrar = false
                                        };

                                        object clave = cast.GetValor(e, log, errores);

                                        if (clave != null)
                                        {
                                            if (!map1.Remove(clave))
                                                errores.AddLast(new Error("Semántico", "No existe un valor con la clave: " + clave.ToString() + " en Map.", Linea, Columna));
                                        }
                                    }
                                }
                                else
                                    return new Throw("ArithmeticException", Linea, Columna);

                            }
                            return map1;
                        case Type.LIST:
                        case Type.SET:
                            if (valOp1 is Null)
                                return new Throw("NullPointerException", Linea, Columna);
                            if (valOp2 is Null)
                                return new Throw("NullPointerException", Linea, Columna);

                            Collection set1 = (Collection)valOp1;
                            Collection set2 = (Collection)valOp2;

                            foreach (CollectionValue value in set2.Valores)
                            {
                                if (Op == Operador.SUMA)
                                {
                                    if (set1.Tipo.Valor.Equals(set2.Tipo.Valor))
                                    {
                                        set1.Insert(set1.Posicion++, value.Valor);
                                    }
                                    else
                                    {
                                        Casteo cast = new Casteo(set1.Tipo.Valor, new Literal(set2.Tipo.Valor, value.Valor, 0, 0), 0, 0)
                                        {
                                            Mostrar = false
                                        };
                                        object valor = cast.GetValor(e, log, errores);

                                        if (valor != null)
                                        {
                                            if (valor is Throw)
                                                return valor;

                                            set1.Insert(set1.Posicion++, valor);
                                            continue;
                                        }

                                        errores.AddLast(new Error("Semántico", "El tipo del parametro no coinciden con el valor de la Collection.", Linea, Columna));
                                        return null;
                                    }
                                }
                                else if (Op == Operador.RESTA)
                                {
                                    if (set1.Tipo.Valor.Equals(set2.Tipo.Valor))
                                    {
                                        if (!set1.RemoveValor(value.Valor))
                                            errores.AddLast(new Error("Semántico", "No existe un valor: " + value.Valor.ToString() + " en Collection.", Linea, Columna));

                                    }
                                    else
                                    {
                                        Casteo cast = new Casteo(set1.Tipo.Valor, new Literal(set2.Tipo.Valor, value.Valor, 0, 0), 0, 0)
                                        {
                                            Mostrar = false
                                        };
                                        object valor = cast.GetValor(e, log, errores);

                                        if (valor != null)
                                        {
                                            if (valor is Throw)
                                                return valor;

                                            if (!set1.RemoveValor(valor))
                                                errores.AddLast(new Error("Semántico", "No existe un valor: " + valor.ToString() + " en Collection.", Linea, Columna));

                                            continue;
                                        }

                                        errores.AddLast(new Error("Semántico", "El tipo del parametro no coinciden con el valor de la Collection.", Linea, Columna));
                                        return null;
                                    }
                                }
                                else
                                    return new Throw("ArithmeticException", Linea, Columna);
                            }

                            return set1;
                    }
                }
                return new Throw("ArithmeticException", Linea, Columna);
            }

            return null;
        }

        public void TipoDominante(LinkedList<Error> errores)
        {
            if (Op1.Tipo != null && Op2.Tipo != null)
            {
                if (Op1.Tipo.IsCollection() && Op2.Tipo.IsCollection())
                {
                    //if (Op1.Tipo.EqualsCollection(Op2.Tipo))
                        Tipo = Op1.Tipo;
                }
                else if (Op1.Tipo.IsString() || Op2.Tipo.IsString())
                {
                    /*Verificar Objetos y otros tipos sin toString()*/
                    if (Op == Operador.SUMA)
                    {
                        Tipo = new Tipo(Type.STRING);
                        return;
                    }
                    else
                    {
                        //errores.AddLast(new Error("Semántico", "Las cadenas solo admiten el operador: " + "+.", Linea, Columna));
                        return;
                    }
                }
                else if (!Op1.Tipo.IsBoolean() && !Op2.Tipo.IsBoolean())
                {
                    if (Op1.Tipo.IsDouble() || Op2.Tipo.IsDouble())
                    {
                        Tipo = new Tipo(Type.DOUBLE);
                        return;
                    }
                    else if (Op1.Tipo.IsInt() || Op2.Tipo.IsInt())
                    {
                        if (Op == Operador.POTENCIA)
                        {
                            Tipo = new Tipo(Type.DOUBLE);
                            return;
                        }
                        Tipo = new Tipo(Type.INT);
                        return;
                    }
                }
                //errores.AddLast(new Error("Semántico", "Error de tipos en operación aritmética.", Linea, Columna));
            }
        }

    }
}
