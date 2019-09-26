using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GramaticasCQL.Parsers.CQL.ast.entorno
{
    class Objeto
    {
        public Objeto(string id, Entorno entorno)
        {
            Id = id;
            Entorno = entorno;
        }

        public string Id { get; set; }
        public Entorno Entorno { get; set; } /*Reconsiderar guardar solo una lista aquí*/

        public override string ToString()
        {
            string cad = "{";
            foreach (Simbolo sim in Entorno.Simbolos)
            {
                cad +=sim.Id + ": "+ sim.Valor.ToString();

                if (!Entorno.Simbolos.Last.Value.Equals(sim))
                    cad += ", ";
            }
            cad += "}"; //as " + Id;
            return cad;
        }

        public string ToString2()
        {
            string cad = "<";
            foreach (Simbolo sim in Entorno.Simbolos)
            {
                cad += "\"" + sim.Id + "\"= ";

                if (sim.Valor is Objeto obj)
                    cad += obj.ToString2();
                else if (sim.Valor is Collection coll)
                    cad += coll.ToString2();
                else if (sim.Valor is Cadena cade)
                    cad += cade.ToString2();
                else if (sim.Valor is Date dat)
                    cad += dat.ToString2();
                else if (sim.Valor is Time tim)
                    cad += tim.ToString2();
                else
                    cad += sim.Valor.ToString();

                if (!Entorno.Simbolos.Last.Value.Equals(sim))
                    cad += ", ";
            }
            cad += ">";
            return cad;
        }

        public Simbolo GetAtributo(string id)
        {
            foreach (Simbolo sim in Entorno.Simbolos)
            {
                if (sim.Id.Equals(id.ToLower()))// && sim.Rol == Rol.ATRIBUTO)
                    return sim;
            }
            return null;
        }
    }
}
