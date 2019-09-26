using GramaticasCQL.Parsers.CQL.ast.expresion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GramaticasCQL.Parsers.CQL.ast.entorno
{
    class Simbolo
    {

        public Simbolo(Tipo tipo, Rol rol, string id)
        {
            Tipo = tipo;
            Rol = rol;
            Id = id;
            Valor = Predefinido();
        }

        public Simbolo(Tipo tipo, Rol rol, string id, object valor)
        {
            Tipo = tipo;
            Rol = rol;
            Id = id;
            Valor = valor ?? Predefinido();
        }

        public Simbolo(Rol rol, string id, object valor)
        {
            Rol = rol;
            Id = id;
            Valor = valor;
        }

        public Simbolo()
        {
        }

        public Tipo Tipo { get; set; }
        public Rol Rol { get; set; }
        public string Id { get; set; }
        public object Valor { get; set; }


        public object Predefinido()
        {
            if (Tipo.IsInt())
                return 0;
            else if (Tipo.IsDouble())
                return 0.0;
            else if (Tipo.IsBoolean())
                return false;
            else
                return new Null();
        }

    }
}
