using GramaticasCQL.Parsers.CQL.ast.entorno;
using GramaticasCQL.Parsers.CQL.ast.expresion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Type = GramaticasCQL.Parsers.CQL.ast.entorno.Type;

namespace GramaticasCQL.Parsers.CQL.ast.instruccion
{
    class TryCatch : Instruccion
    {
        public TryCatch(Bloque bloqueTry, LinkedList<Identificador> parametro, Bloque bloqueCatch, int linea, int columna) : base(linea, columna)
        {
            BloqueTry = bloqueTry;
            Parametro = parametro;
            BloqueCatch = bloqueCatch;
        }

        public TryCatch(Bloque bloqueTry, Bloque bloqueCatch, int linea, int columna) : base(linea, columna)
        {
            BloqueTry = bloqueTry;
            BloqueCatch = bloqueCatch;
        }

        public Bloque BloqueTry { get; set; }
        public LinkedList<Identificador> Parametro { get; set; }
        public Bloque BloqueCatch { get; set; }

        public override object Ejecutar(Entorno e, bool funcion, bool ciclo, bool sw, bool tc, LinkedList<Salida> log, LinkedList<Error> errores)
        {
            Entorno local = new Entorno(e);
            object obj = BloqueTry.Ejecutar(local, funcion, ciclo, sw, true, log, errores);

            if (obj != null)
            {
                if (obj is Throw th)
                {
                    bool sacar = false; 

                    if (Parametro != null)
                    {
                        if(Parametro.Count() > 1)
                            errores.AddLast(new Error("Semántico", "El Catch solo recibe un Parámetro.", Linea, Columna));

                        string id = Parametro.ElementAt(0).Id;

                        Simbolo sim = e.GetLocal(id);

                        if (sim == null)
                        {
                            LinkedList<Simbolo> sims = new LinkedList<Simbolo>();
                            sims.AddLast(new Simbolo(new Tipo(Type.STRING), Rol.ATRIBUTO, "message", th.Mensaje));

                            sim = new Simbolo(new Tipo("exception"), Rol.VARIABLE, id.ToLower(), new Objeto("exception", new Entorno(null, sims)));
                            e.Add(sim);
                            sacar = true;
                        }
                        else
                        {
                            return new Throw("ObjectAlreadyExists", Linea, Columna);
                            //errores.AddLast(new Error("Semántico", "Ya se ha declarado una variable con el id: " + id + ".", Linea, Columna));
                            //return null;
                        }
                    }

                    obj =  BloqueCatch.Ejecutar(e, funcion, ciclo, sw, tc, log, errores);

                    if (Parametro != null && sacar)
                    {
                        string id = Parametro.ElementAt(0).Id;

                        Simbolo sim = e.GetLocal(id);

                        if (sim != null)
                        {
                            e.Simbolos.Remove(sim);
                        }
                    }
                }

                return obj;
            }

            return null;
        }
    }
}
