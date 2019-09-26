using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GramaticasCQL.Parsers.CQL.ast.entorno
{
    class Tabla
    {
        public Tabla()
        {
            Cabecera = new Entorno(null, new LinkedList<Simbolo>());
            Datos = new LinkedList<Entorno>();
            Contador = 1;
        }

        public Entorno Cabecera { get; set; }
        public LinkedList<Entorno> Datos {get; set;}
        public int Contador { get; set; }

        public void Add(Simbolo columna)
        {
            Cabecera.Add(columna);
            AddFilas(columna);
        }

        public void AddFilas(Simbolo columna)
        {
            foreach (Entorno ent in Datos)
            {
                ent.Add(new Simbolo(columna.Tipo, columna.Rol, columna.Id, columna.Valor));
            }
        }

        public int Drop(string columna)
        {
            foreach (Simbolo sim in Cabecera.Simbolos)
            {
                if (sim.Id.Equals(columna))
                {
                    if (sim.Rol != Rol.PRIMARY)
                    {
                        Cabecera.Simbolos.Remove(sim);
                        /*Remover en los datos*/
                        DropFilas(columna);
                        return 1;
                    }
                    return 2; //Es llave primaria
                }
            }
            return 3; // No se encontro
        }

        public void DropFilas(string columna)
        {
            foreach (Entorno ent in Datos)
            {
                Simbolo sim = ent.GetCualquiera(columna);
                if (sim != null)
                {
                    ent.Simbolos.Remove(sim);
                }
            }
        }

        public bool Insertar(Entorno dato, LinkedList<Simbolo> primary)
        {
            if (primary.Count() > 0)
            {
                bool bandera = false;
                foreach (Simbolo sim in primary)
                {
                    if (!BuscarPrimaria(sim.Id, sim.Valor))
                        bandera = true;
                }

                if (bandera)
                {
                    Datos.AddLast(dato);
                    return true;
                }
            }
            else
            {
                Datos.AddLast(dato);
                return true;
            }
            return false;
        }

        public bool BuscarPrimaria(string id, object valor)
        {
            foreach (Entorno ent in Datos)
            {
                foreach (Simbolo sim in ent.Simbolos)
                {
                    if (sim.Id.Equals(id))
                    {
                        if (sim.Valor.Equals(valor))
                            return true;
                    }
                }
            }
            return false;
        }

        public Entorno GetNuevaFila()
        {
            Entorno fila = new Entorno(null, new LinkedList<Simbolo>());

            foreach (Simbolo sim in Cabecera.Simbolos)
            {
                fila.Add(new Simbolo(sim.Tipo, sim.Rol, sim.Id, sim.Valor));
            }

            return fila;
        }

        public void Recorrer()
        {
            foreach (Simbolo col in Cabecera.Simbolos)
            {
                Console.WriteLine(col.Id + " " + col.Tipo.Type.ToString() + " " + col.Rol.ToString() + " " + col.Valor.ToString());
            }
            foreach (Entorno ent in Datos)
            {
                ent.Recorrer();
            }
        }
    }
}
