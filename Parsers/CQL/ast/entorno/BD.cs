using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GramaticasCQL.Parsers.CQL.ast.entorno
{
    public class BD
    {
        public BD (string id)
        {
            Id = id;
            Simbolos = new LinkedList<Simbolo>();
        }

        public string Id { get; set; }
        public LinkedList<Simbolo> Simbolos { get; set; }

        public void Add(Simbolo sim)
        {
            Simbolos.AddLast(sim);
        }

        public Simbolo GetUserType(string id)
        {
            foreach (Simbolo sim in Simbolos)
            {
                if (sim.Id.Equals(id.ToLower()) && sim.Rol == Rol.USERTYPE)
                    return sim;
            }
            return null;
        }

        public Simbolo GetProcedimiento(string id)
        {
            foreach (Simbolo sim in Simbolos)
            {
                if (sim.Id.Equals(id.ToLower()) && sim.Rol == Rol.PROCEDIMIENTO)
                    return sim;
            }
            return null;
        }

        public Simbolo GetTabla(string id)
        {
            foreach (Simbolo sim in Simbolos)
            {
                if (sim.Id.Equals(id.ToLower()) && sim.Rol == Rol.TABLA)
                    return sim;
            }
            return null;
        }

        public bool DropTabla(string id)
        {
            foreach (Simbolo sim in Simbolos)
            {
                if (sim.Id.Equals(id.ToLower()) && sim.Rol == Rol.TABLA)
                {
                    Simbolos.Remove(sim);
                    return true;
                }
            }
            return false;
        }

        public bool TruncateTabla(string id)
        {
            foreach (Simbolo sim in Simbolos)
            {
                if (sim.Id.Equals(id.ToLower()) && sim.Rol == Rol.TABLA)
                {
                    ((Tabla)sim.Valor).Datos.Clear();
                    return true;
                }
            }
            return false;
        }

        public void Recorrer()
        {
            foreach (Simbolo sim in Simbolos)
            {
                Console.WriteLine(sim.Id + " " + sim.Rol.ToString() + " " + sim.Valor.ToString());

                /*
                if (sim.Valor is Entorno ent)
                     ent.Recorrer();

                if (sim.Valor is Procedimiento proc)
                {
                    Console.WriteLine(proc.Bloque.Cadena);
                }
                */
                if (sim.Valor is Tabla t)
                {
                    t.Recorrer();
                }

            }
        }
    }
}
