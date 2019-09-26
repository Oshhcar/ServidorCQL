using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GramaticasCQL.Parsers.CQL.ast.entorno
{
    public class MasterBD
    {
        public MasterBD()
        {
            Data = new LinkedList<BD>();
            Usuarios = new LinkedList<Usuario>();
        }
        
        public LinkedList<BD> Data { get; set; }
        public LinkedList<Usuario> Usuarios { get; set; }
        public BD Actual { get; set; }
        public Entorno EntornoActual { get; set; }

        public void Add(string id)
        {
            Data.AddLast(new BD(id.ToLower()));
        }

        public BD Get(string id)
        {
            foreach (BD bd in Data)
            {
                if (bd.Id.Equals(id.ToLower()))
                    return bd;
            }
            return null;
        }

        public bool Drop(string id)
        {
            foreach (BD bd in Data)
            {
                if (bd.Id.Equals(id.ToLower()))
                {
                    if(Actual != null)
                        if (Actual.Equals(bd))
                            Actual = null;

                    //bd.Simbolos.Clear();
                    Data.Remove(bd);

                    foreach (Usuario usuario in Usuarios)
                    {
                        usuario.RevokePermiso(id);
                    }

                    return true;
                }
            }
            return false;
        }

        public void AddUsuario(string id, string password)
        {
            Usuarios.AddLast(new Usuario(id.ToLower(), password));
        }

        public Usuario GetUsuario(string id)
        {
            foreach (Usuario usuario in Usuarios)
            {
                if (usuario.Id.Equals(id.ToLower()))
                    return usuario;
            }
            return null;
        }

        public void Recorrer()
        {
            Console.WriteLine("***************");
            foreach (BD bd in Data)
            {
                Console.WriteLine("DataBase: " + bd.Id);
                bd.Recorrer();
            }
            if (Actual != null)
                Console.WriteLine("\tActual: " + Actual.Id + "\n\n");

            Console.WriteLine("*****************");
            foreach (Usuario usuario in Usuarios)
            {
                Console.WriteLine("Usuario: " + usuario.Id + " Pass:" + usuario.Password);
                usuario.Recorrer();
            }
            Console.WriteLine("\n\n\n");
        }
    }
}
