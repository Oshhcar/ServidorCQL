using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GramaticasCQL.Parsers.CQL.ast.entorno
{
    public class Entorno
    {
        public Entorno(Entorno padre)
        {
            Simbolos = new LinkedList<Simbolo>();
            Padre = padre;

            if (padre != null)
            {
                Global = padre.Global;
                Master = padre.Master;
            }
        }

        public Entorno(Entorno padre, LinkedList<Simbolo> simbolos)
        {
            Simbolos = simbolos;
            Padre = padre;
        }

        public LinkedList<Simbolo> Simbolos { get; set; }
        public Entorno Padre { get; set; }
        public Entorno Global { get; set; }
        public MasterBD Master { get; set; }
        public MasterBD MasterRollback { get; set; }

        public void Add(Simbolo sim)
        {
            Simbolos.AddLast(sim);

        }

        public Simbolo Get(string id)
        {
            foreach (Simbolo sim in Simbolos)
            {
                if (sim.Id.Equals(id.ToLower()) && sim.Rol == Rol.VARIABLE)
                {
                    return sim;
                }
            }

            return Padre?.Get(id);
        }

        public Simbolo GetLocal(string id)
        {
            foreach (Simbolo sim in Simbolos)
            {
                if (sim.Id.Equals(id.ToLower()) && sim.Rol == Rol.VARIABLE)
                    return sim;
            }
            return null;
        }

        public Simbolo GetFuncion(string id)
        {
            foreach (Simbolo sim in Global.Simbolos)
            {
                if (sim.Id.Equals(id.ToLower()) && sim.Rol == Rol.FUNCION)
                    return sim;
            }
            return null;
        }

        public Simbolo GetCualquiera(string id)
        {
            foreach (Simbolo sim in Simbolos)
            {
                if (sim.Id.Equals(id.ToLower()))//&& sim.Rol == Rol.COLUMNA || sim.Rol == Rol.PRIMARY)
                    return sim;
            }
            return null;
        }

        public void Recorrer()
        {
            Console.WriteLine("**Entorno**");
            foreach (Simbolo s in Simbolos)
            {
                Console.WriteLine(s.Id + ", " + s.Tipo.ToString() + ", " + s.Rol + " " + s.Valor.ToString());

            }

            Padre?.Recorrer();
        }
    }
}
