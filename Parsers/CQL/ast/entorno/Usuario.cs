using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GramaticasCQL.Parsers.CQL.ast.entorno
{
    public class Usuario
    {
        public Usuario(string id, string password)
        {
            Id = id;
            Password = password;
            Permisos = new LinkedList<string>();
        }

        public string Id { get; set; }
        public string Password { get; set; }
        public LinkedList<string> Permisos { get; set; }

        public void AddPermiso(string id)
        {
            Permisos.AddLast(id.ToLower());
        }

        public bool GetPermiso(string id)
        {
            foreach (string permiso in Permisos)
            {
                if (permiso.Equals(id.ToLower()))
                    return true;
            }
            return false;
        }

        public bool RevokePermiso(string id)
        {
            foreach (string permiso in Permisos)
            {
                if (permiso.Equals(id.ToLower()))
                {
                    Permisos.Remove(permiso);
                    return true;
                }
            }
            return false;
        }

        public void Recorrer()
        {
            foreach (string permiso in Permisos)
            {
                Console.Write(permiso + " -- ");
            }
            Console.WriteLine();
        }
    }
}
