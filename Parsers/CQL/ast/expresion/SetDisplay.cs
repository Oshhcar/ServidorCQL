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
    class SetDisplay : Expresion
    {
        public SetDisplay(LinkedList<Expresion> collection, int linea, int columna) : base(linea, columna)
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

                Tipo = new Tipo(Type.SET, valor.Tipo);

                Collection set = new Collection(new Tipo(Type.SET, valor.Tipo));
                set.Insert(set.Posicion++, valValor);

                for (int i = 1; i < Collection.Count(); i++)
                {
                    valor = Collection.ElementAt(i);
                    valValor = valor.GetValor(e, log, errores);

                    if (valValor != null)
                    {
                        if (valValor is Throw)
                            return valValor;

                        if (set.Tipo.Valor.Equals(valor.Tipo))
                            set.Insert(set.Posicion++, valValor);
                        else
                        {
                            Casteo cast = new Casteo(set.Tipo.Valor, new Literal(valor.Tipo, valValor, 0, 0), 0, 0)
                            {
                                Mostrar = false
                            };

                            valValor = cast.GetValor(e, log, errores);

                            if (valValor != null)
                            {
                                if (valValor is Throw)
                                    return valValor;

                                set.Insert(set.Posicion++, valValor);
                                continue;
                            }

                            errores.AddLast(new Error("Semántico", "El tipo no coinciden con el valor del List.", Linea, Columna));
                        }
                        //continue;
                    }
                    //return null;
                }

                set.Ordenar();

                return set;
            }
            return null;
        }
    }
}
