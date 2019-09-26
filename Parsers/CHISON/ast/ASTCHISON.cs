using GramaticasCQL.Parsers.CQL.ast.entorno;
using GramaticasCQL.Parsers.CQL.ast.expresion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GramaticasCQL.Parsers.CHISON.ast
{
    class ASTCHISON
    {
        public ASTCHISON(BloqueChison bloque)
        {
           Bloque = bloque;
        }

        public ASTCHISON(LinkedList<BloqueChison> bloques)
        {
            Bloques = bloques;
        }

        public BloqueChison Bloque { get; set; }
        public LinkedList<BloqueChison> Bloques { get; set; }

        public bool IsPrincipal()
        {
            return Bloque != null;
        }

        public void Ejecutar(Entorno e, LinkedList<Salida> log, LinkedList<Error> errores)
        {
            if (Bloque != null)
            {
                Bloque.Obj = OBJ.PRINCIPAL;
                Bloque.GetValor(e, log, errores);
            }
        }
    }
}
