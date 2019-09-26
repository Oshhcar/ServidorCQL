using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GramaticasCQL.Parsers.CQL.ast.entorno;
using GramaticasCQL.Parsers.CQL.ast.instruccion;

namespace GramaticasCQL.Parsers.CQL.ast.expresion
{
    class Identificador : Expresion
    {
        public Identificador(string id, int linea, int columna) : base(linea, columna)
        {
            Id = id;
            IsId2 = false;
            IsASC = true;
            In = true;
        }

        public Identificador(string id, bool isId2, int linea, int columna) : base(linea, columna)
        {
            Id = id;
            IsId2 = isId2;
            IsASC = true;
            In = true;
        }

        public Identificador(string id, bool isId2, bool isAsc, int linea, int columna) : base(linea, columna)
        {
            Id = id;
            IsId2 = isId2;
            IsASC = isAsc;
            In = true;
        }

        public string Id { get; set; }
        public bool IsId2 { get; set; }
        public bool IsASC { get; set; }
        public bool In { get; set; }

        public override object GetValor(Entorno e, LinkedList<Salida> log, LinkedList<Error> errores)
        {
            if (IsId2)
            {
                Simbolo sim = e.Get(Id);

                if (sim != null)
                {
                    Tipo = sim.Tipo;
                    return sim.Valor;
                }

                errores.AddLast(new Error("Semántico", "No hay una variabla declarada con el id: " + Id + ".", Linea, Columna));
            }
            else
            {
                if (e.Master.EntornoActual != null)
                {
                    Simbolo sim = e.Master.EntornoActual.GetCualquiera(Id);
                    if (sim != null)
                    {
                        Tipo = sim.Tipo;
                        return sim.Valor;
                    }

                    return new Throw("ColumnException", Linea, Columna);
                    //errores.AddLast(new Error("Semántico", "No hay una columna con el id: " + Id + " en la consulta.", Linea, Columna));
                    //return null;
                }
                else
                    errores.AddLast(new Error("Semántico", "No esta dentro de una consulta, no se puede buscar: " + Id + ".", Linea, Columna));
            }
            return null;
        }

        public override Simbolo GetSimbolo(Entorno e)
        {
            if(IsId2)
                return e.Get(Id);

            if (e.Master.EntornoActual != null)
                return e.Master.EntornoActual.GetCualquiera(Id);

            return null;
        }

        public override string GetId()
        {
            return Id;
        }
    }
}
