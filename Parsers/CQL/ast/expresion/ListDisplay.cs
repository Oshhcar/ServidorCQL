using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GramaticasCQL.Parsers.CQL.ast.entorno;
using GramaticasCQL.Parsers.CQL.ast.instruccion;
using Type = GramaticasCQL.Parsers.CQL.ast.entorno.Type;

namespace GramaticasCQL.Parsers.CQL.ast.expresion
{
    class ListDisplay : Expresion
    {
        public ListDisplay(LinkedList<Expresion> collection, int linea, int columna) : base(linea, columna)
        {
            Collection = collection;
        }

        public LinkedList<Expresion> Collection { get; set; }

        public override object GetValor(Entorno e, LinkedList<Salida> log, LinkedList<Error> errores)
        {
            Expresion valor = Collection.ElementAt(0);
            object valValor = valor.GetValor(e, log, errores);

            if (valValor != null)
            {
                if (valValor is Throw)
                    return valValor;

                Tipo = new Tipo(Type.LIST, valor.Tipo);
                Collection list = new Collection(new Tipo(Type.LIST, valor.Tipo));
                list.Insert(list.Posicion++, valValor);

                for (int i = 1; i < Collection.Count(); i++)
                {
                    valor = Collection.ElementAt(i);
                    valValor = valor.GetValor(e, log, errores);

                    if (valValor != null)
                    {
                        if (valValor is Throw)
                            return valValor;

                        if (list.Tipo.Valor.Equals(valor.Tipo))
                            list.Insert(list.Posicion++, valValor);
                        else
                        {
                            Casteo cast = new Casteo(list.Tipo.Valor, new Literal(valor.Tipo, valValor, 0, 0), 0, 0)
                            {
                                Mostrar = false
                            };

                            valValor = cast.GetValor(e, log, errores);

                            if (valValor != null)
                            {
                                if (valValor is Throw)
                                    return valValor;

                                list.Insert(list.Posicion++, valValor);
                                continue;
                            }
                            errores.AddLast(new Error("Semántico", "El tipo no coinciden con el valor del List.", Linea, Columna));
                        }
                        //continue;
                    }
                    //return null;
                }

                return list;
            }
            return null;
        }
    }
}
