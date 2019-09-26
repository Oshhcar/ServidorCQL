using GramaticasCQL.Parsers.CQL.ast.entorno;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GramaticasCQL.Parsers.CQL.ast.instruccion
{
    class Throw : Instruccion
    {
        public Throw(string id, int linea, int columna) : base(linea, columna)
        {
            Id = id;
            Mensaje = GetMensaje();
        }

        public string Id { get; set; }
        public object Mensaje { get; set; }

        public object GetMensaje()
        {
            if (Id.Equals("typealreadyexists", StringComparison.InvariantCultureIgnoreCase))
                return "Excepción TypeAlreadyExists: El User Type ya existe en la base de datos, Linea: " + Linea + ".";
            else if (Id.Equals("typedontexists", StringComparison.InvariantCultureIgnoreCase))
                return "Excepción TypeDontExists: El User Type no existe en la base de datos, Linea: " + Linea + ".";
            else if (Id.Equals("bdalreadyexists", StringComparison.InvariantCultureIgnoreCase))
                return "Excepción BDAlreadyExists: La base de datos ya existe, Linea: " + Linea + ".";
            else if (Id.Equals("bddontexists", StringComparison.InvariantCultureIgnoreCase))
                return "Excepción BDDontExists: La base de datos no existe, Linea: " + Linea + ".";
            else if (Id.Equals("usebdexception", StringComparison.InvariantCultureIgnoreCase))
                return "Excepción UseBDException: No se ha seleccionado una base de datos, utilice USE, Linea: " + Linea + ".";
            else if (Id.Equals("tablealreadyexists", StringComparison.InvariantCultureIgnoreCase))
                return "Excepción TableAlreadyExists: La Tabla ya existe en la base de datos, Linea: " + Linea + ".";
            else if (Id.Equals("tabledontexists", StringComparison.InvariantCultureIgnoreCase))
                return "Excepción TableDontExists: La tabla no existe en la base de datos, Linea: " + Linea + ".";
            else if (Id.Equals("countertypeexception", StringComparison.InvariantCultureIgnoreCase))
                return "Excepción CounterTypeException: El tipo de dato Counter no se puede insertar o actualizar , Linea: " + Linea + ".";
            else if (Id.Equals("useralreadyexists", StringComparison.InvariantCultureIgnoreCase))
                return "Excepción UserAlreadyExists: El Usuario ya existe en la base de datos, Linea: " + Linea + ".";
            else if (Id.Equals("userdontexists", StringComparison.InvariantCultureIgnoreCase))
                return "Excepción UserDontExists: El Usuario no existe en la base de datos, Linea: " + Linea + ".";
            else if (Id.Equals("valuesexception", StringComparison.InvariantCultureIgnoreCase))
                return "Excepción ValuesException: Los Valores no coinciden con las columnas en la Tabla, Linea: " + Linea + ".";
            else if (Id.Equals("columnexception", StringComparison.InvariantCultureIgnoreCase))
                return "Excepción ColumnException: La Columna no existe en la Tabla, Linea: " + Linea + ".";
            else if (Id.Equals("batchexception", StringComparison.InvariantCultureIgnoreCase))
                return "Excepción BatchException: El Batch no pudo ser ejecutado correctamente, Linea: " + Linea + ".";
            else if (Id.Equals("indexoutexception", StringComparison.InvariantCultureIgnoreCase))
                return "Excepción IndexOutException: El índice no existe en la Collection, Linea: " + Linea + ".";
            else if (Id.Equals("arithmeticexception", StringComparison.InvariantCultureIgnoreCase))
                return "Excepción ArithmeticException: Operación aritmética con errores, Linea: " + Linea + ".";
            else if (Id.Equals("nullpointerexception", StringComparison.InvariantCultureIgnoreCase))
                return "Excepción NullPointerException: Se está haciendo referencia a un Null, Linea: " + Linea + ".";
            else if (Id.Equals("numberreturnsexception", StringComparison.InvariantCultureIgnoreCase))
                return "Excepción NumberReturnsException: No coinciden la cantidad de Returns con los del Procedimiento, Linea: " + Linea + ".";
            else if (Id.Equals("funcionalreadyexists", StringComparison.InvariantCultureIgnoreCase))
                return "Excepción FuncionAlreadyExists: La Función ya existe con la misma firma, Linea: " + Linea + ".";
            else if (Id.Equals("procedurealreadyexists", StringComparison.InvariantCultureIgnoreCase))
                return "Excepción ProcedureAlreadyExists: El Procedimiento ya existe con la misma firma, Linea: " + Linea + ".";
            else if (Id.Equals("Objectalreadyexists", StringComparison.InvariantCultureIgnoreCase))
                return "Excepción ObjectAlreadyExists: La variable ya existe en el Entorno, Linea: " + Linea + ".";
            else
            {
                return new Null();
            }
        }

        public override object Ejecutar(Entorno e, bool funcion, bool ciclo, bool sw, bool tc, LinkedList<Salida> log, LinkedList<Error> errores)
        {
            if (!tc)
            {
                errores.AddLast(new Error("Semántico", "Sentencia Throw no se encuentra dentro de un Try Catch.", Linea, Columna));
                return null;
            }

            if (Mensaje is Null)
            {
                errores.AddLast(new Error("Semántico", "No existe la excepción: "+ Id +".", Linea, Columna));
                return null;
            } 

            return this;
        }
    }
}
