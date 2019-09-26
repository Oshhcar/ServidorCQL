using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GramaticasCQL.Parsers.CQL.ast.entorno;
using GramaticasCQL.Parsers.CQL.ast.expresion.operacion;
using GramaticasCQL.Parsers.CQL.ast.instruccion;
using GramaticasCQL.Parsers.CQL.ast.instruccion.ddl;
using Type = GramaticasCQL.Parsers.CQL.ast.entorno.Type;

namespace GramaticasCQL.Parsers.CQL.ast.expresion
{
    class Agregacion : Expresion
    {
        public Agregacion(Aggregation funcion, Seleccionar select, int linea, int columna) : base(linea, columna)
        {
            Funcion = funcion;
            Select = select;
        }

        public Aggregation Funcion { get; set; }
        public Seleccionar Select { get; set; }

        public override object GetValor(Entorno e, LinkedList<Salida> log, LinkedList<Error> errores)
        {
            Select.Mostrar = false;
            object obj = Select.Ejecutar(e, false, false, false, false, log, errores);

            if (obj != null)
            {
                if (obj is Throw)
                    return obj;

                LinkedList<Entorno> data = (LinkedList<Entorno>)obj;

                Aritmetica op;
                Relacional rel;

                switch (Funcion)
                {
                    case Aggregation.COUNT:
                        int count = data.Count();
                        Tipo = new Tipo(Type.INT);
                        return count;
                    case Aggregation.MIN:
                        if (data.Count() > 0)
                        {
                            object min;

                            Entorno ent1 = data.ElementAt(0);
                            Simbolo sim1 = ent1.Simbolos.ElementAt(0);

                            if (ent1.Simbolos.Count() > 1)
                                errores.AddLast(new Error("Semántico", "El Select solo debería retornar una Columna.", Linea, Columna));

                            if (sim1.Tipo.IsNumeric() || sim1.Tipo.IsDate() || sim1.Tipo.IsTime())
                            {
                                Tipo = sim1.Tipo;
                                min = sim1.Valor;
                            }
                            else
                            {
                                errores.AddLast(new Error("Semántico", "La función de agregación MIN solo puede ser utilizada en datos numéricos, fechas y horas.", Linea, Columna));
                                return null;
                            }

                            for (int i = 1; i < data.Count(); i++)
                            {
                                ent1 = data.ElementAt(i);
                                sim1 = ent1.Simbolos.ElementAt(0);

                                rel = new Relacional(new Literal(Tipo, min, Linea, Columna), new Literal(sim1.Tipo, sim1.Valor, Linea, Columna), Operador.MENORQUE, Linea, Columna);

                                object valRel = rel.GetValor(e, log, errores);

                                if (valRel != null)
                                {
                                    if (valRel is Throw)
                                        return valRel;

                                    if (!((bool)valRel))
                                        min = sim1.Valor;
                                }
                                else
                                {
                                    errores.AddLast(new Error("Semántico", "No se pudo realizar la función de agregación MIN.", Linea, Columna));
                                    return null;
                                }
                            }
                            return min;
                        }
                        Tipo = new Tipo(Type.NULL);
                        return new Null();
                    case Aggregation.MAX:
                        if (data.Count() > 0)
                        {
                            object max;

                            Entorno ent1 = data.ElementAt(0);
                            Simbolo sim1 = ent1.Simbolos.ElementAt(0);

                            if (ent1.Simbolos.Count() > 1)
                                errores.AddLast(new Error("Semántico", "El Select solo debería retornar una Columna.", Linea, Columna));

                            if (sim1.Tipo.IsNumeric() || sim1.Tipo.IsDate() || sim1.Tipo.IsTime())
                            {
                                Tipo = sim1.Tipo;
                                max = sim1.Valor;
                            }
                            else
                            {
                                errores.AddLast(new Error("Semántico", "La función de agregación MAX solo puede ser utilizada en datos numéricos, fechas y horas.", Linea, Columna));
                                return null;
                            }

                            for (int i = 1; i < data.Count(); i++)
                            {
                                ent1 = data.ElementAt(i);
                                sim1 = ent1.Simbolos.ElementAt(0);

                                rel = new Relacional(new Literal(Tipo, max, Linea, Columna), new Literal(sim1.Tipo, sim1.Valor, Linea, Columna), Operador.MAYORQUE, Linea, Columna);

                                object valRel = rel.GetValor(e, log, errores);

                                if (valRel != null)
                                {
                                    if (valRel is Throw)
                                        return valRel;

                                    if (!((bool)valRel))
                                        max = sim1.Valor;
                                }
                                else
                                {
                                    errores.AddLast(new Error("Semántico", "No se pudo realizar la función de agregación MAX.", Linea, Columna));
                                    return null;
                                }
                            }
                            return max;
                        }
                        Tipo = new Tipo(Type.NULL);
                        return new Null();
                    case Aggregation.SUM:
                        if (data.Count() > 0)
                        {
                            object sum;

                            Entorno ent1 = data.ElementAt(0);
                            Simbolo sim1 = ent1.Simbolos.ElementAt(0);
                            Tipo = sim1.Tipo;
                            sum = sim1.Valor;

                            if(ent1.Simbolos.Count() > 1)
                                errores.AddLast(new Error("Semántico", "El Select solo debería retornar una Columna.", Linea, Columna));

                            if (!(Tipo.IsNumeric() || Tipo.IsString()))
                            {
                                errores.AddLast(new Error("Semántico", "La función de agregación AVG solo puede ser utilizada sobre datos numéricos y Cadenas.", Linea, Columna));
                                return null;
                            }

                            for (int i = 1; i < data.Count(); i++)
                            { 
                                ent1 = data.ElementAt(i);
                                sim1 = ent1.Simbolos.ElementAt(0);

                                op = new Aritmetica(new Literal(Tipo, sum, Linea, Columna), new Literal(sim1.Tipo, sim1.Valor, Linea, Columna), Operador.SUMA, Linea, Columna);
                                sum = op.GetValor(e, log, errores);
                                Tipo = op.Tipo;

                                if (sum == null)
                                {
                                    errores.AddLast(new Error("Semántico", "No se pudo realizar la función de agregación SUM.", Linea, Columna));
                                    return null;
                                }

                                if (sum is Throw)
                                    return sum;

                            }

                            if (Tipo.IsString() || Tipo.IsNumeric())
                                return sum;
                            else
                            {
                                errores.AddLast(new Error("Semántico", "No se pudo realizar la función de agregación SUM.", Linea, Columna));
                                return null;
                            }
                        }
                        Tipo = new Tipo(Type.NULL);
                        return new Null();
                    case Aggregation.AVG:
                        if (data.Count() > 0)
                        {
                            object sum;

                            Entorno ent1 = data.ElementAt(0);
                            Simbolo sim1 = ent1.Simbolos.ElementAt(0);
                            Tipo = sim1.Tipo;
                            sum = sim1.Valor;

                            if (ent1.Simbolos.Count() > 1)
                                errores.AddLast(new Error("Semántico", "El Select solo debería retornar una Columna.", Linea, Columna));

                            if (!Tipo.IsNumeric())
                            {
                                errores.AddLast(new Error("Semántico", "La función de agregación AVG solo puede ser utilizada sobre datos numéricos.", Linea, Columna));
                                return null;
                            }

                            for (int i = 1; i < data.Count(); i++)
                            {
                                ent1 = data.ElementAt(i);
                                sim1 = ent1.Simbolos.ElementAt(0);

                                op = new Aritmetica(new Literal(Tipo, sum, Linea, Columna), new Literal(sim1.Tipo, sim1.Valor, Linea, Columna), Operador.SUMA, Linea, Columna);
                                sum = op.GetValor(e, log, errores);
                                Tipo = op.Tipo;

                                if (sum == null)
                                {
                                    errores.AddLast(new Error("Semántico", "No se pudo realizar la función de agregación AVG.", Linea, Columna));
                                    return null;
                                }

                                if (sum is Throw)
                                    return sum;
                            }

                            int total = data.Count();
                            op = new Aritmetica(new Literal(Tipo, sum, Linea, Columna), new Literal(new Tipo(Type.INT), total, Linea, Columna), Operador.DIVISION, Linea, Columna);

                            sum = op.GetValor(e, log, errores);

                            if (sum != null)
                            {
                                Tipo = op.Tipo;
                                return sum;
                            }
                            else
                            {
                                errores.AddLast(new Error("Semántico", "No se pudo realizar la función de agregación AVG.", Linea, Columna));
                                return null;
                            }
                        }
                        Tipo = new Tipo(Type.NULL);
                        return new Null();

                }
            }
            return null;
        }
    }

    public enum Aggregation
    {
        COUNT,
        MIN,
        MAX,
        SUM,
        AVG
    }
    
}
