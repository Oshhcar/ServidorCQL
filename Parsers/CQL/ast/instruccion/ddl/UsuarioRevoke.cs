﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GramaticasCQL.Parsers.CQL.ast.entorno;

namespace GramaticasCQL.Parsers.CQL.ast.instruccion.ddl
{
    public class UsuarioRevoke : Instruccion
    {
        public UsuarioRevoke(string id, string permiso, int linea, int columna) : base(linea, columna)
        {
            Id = id;
            Permiso = permiso;
        }

        public string Id { get; set; }
        public string Permiso { get; set; }

        public override object Ejecutar(Entorno e, bool funcion, bool ciclo, bool sw, bool tc, LinkedList<Salida> log, LinkedList<Error> errores)
        {
            Usuario usuario = e.Master.GetUsuario(Id);
            if (usuario != null)
            {
                if (e.Master.Get(Permiso) != null)
                {
                    if (!usuario.RevokePermiso(Permiso))
                        errores.AddLast(new Error("Semántico", "El Usuario no tenía permisos sobre: " + Permiso + ".", Linea, Columna));
                }
                else
                    errores.AddLast(new Error("Semántico", "No existe una Base de Datos con el id: " + Permiso + ".", Linea, Columna));
            }
            else
                return new Throw("UserDontExists", Linea, Columna);
                //errores.AddLast(new Error("Semántico", "No existe una Usuario con el id: " + Id + ".", Linea, Columna));

            return null;
        }
    }
}
