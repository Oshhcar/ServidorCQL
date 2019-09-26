using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GramaticasCQL.Parsers.CQL.ast.entorno;
using Type = GramaticasCQL.Parsers.CQL.ast.entorno.Type;

namespace GramaticasCQL.Parsers.CQL.ast.expresion
{
    class Instancia : Expresion
    {
        public Instancia(string id, int linea, int columna) : base(linea, columna)
        {
            Id = id;
        }

        public Instancia(string id, Tipo tipo1, Tipo tipo2, int linea, int columna) : base(linea, columna)
        {
            Id = id;
            Tipo1 = tipo1;
            Tipo2 = tipo2;
        }

        public Instancia(string id, Tipo tipo1, int linea, int columna) : base(linea, columna)
        {
            Id = id;
            Tipo1 = tipo1;
        }

        public string Id { get; set; }
        public Tipo Tipo1 { get; set; }
        public Tipo Tipo2 { get; set; }

        public override object GetValor(Entorno e, LinkedList<Salida> log, LinkedList<Error> errores)
        {
            switch (Id.ToLower())
            {
                case "map":
                    Tipo = new Tipo(Tipo1, Tipo2);
                    return new Collection(new Tipo(Tipo1, Tipo2));
                case "list":
                    Tipo = new Tipo(Type.LIST, Tipo1);
                    return new Collection(new Tipo(Type.LIST, Tipo1));
                case "set":
                    Tipo = new Tipo(Type.SET, Tipo1);
                    return new Collection(new Tipo(Type.SET, Tipo1));
            }

            BD actual = e.Master.Actual;

            if (actual != null)
            {
                Simbolo sim = actual.GetUserType(Id);
                if (sim != null)
                {
                    Tipo = new Tipo(Id.ToLower());

                    LinkedList<Simbolo> sims = new LinkedList<Simbolo>();

                    foreach (Simbolo s in ((Entorno)sim.Valor).Simbolos)
                    {
                        sims.AddLast(new Simbolo(s.Tipo, Rol.ATRIBUTO, s.Id));
                    }

                    return new Objeto(Id.ToLower(), new Entorno(null, sims));
                }
                else
                    errores.AddLast(new Error("Semántico", "No existe un User Type con el id: " + Id + " en la base de datos.", Linea, Columna));
            }
            else
                errores.AddLast(new Error("Semántico", "No se ha seleccionado una base de datos, no se pudo buscar el User Type.", Linea, Columna));

            return null;
        }
    }
}
