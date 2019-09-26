using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GramaticasCQL.Parsers.CQL.ast.entorno
{
    public class Tipo
    {

        public Tipo(Type type)
        {
            Type = type;
        }

        public Tipo(string objeto)
        {
            Type = Type.OBJECT;
            Objeto = objeto;
        }

        public Tipo(Tipo clave, Tipo valor)
        {
            Type = Type.MAP;
            Clave = clave;
            Valor = valor;
        }

        public Tipo(Type type, Tipo valor)
        {
            Type = type;
            Clave = new Tipo(Type.INT);
            Valor = valor;
        }

        public Type Type { get; set; }
        public string Objeto { get; set; }
        public Tipo Clave { get; set; }
        public Tipo Valor { get; set; }

        public bool IsInt() { return Type == Type.INT; }
        public bool IsDouble() { return Type == Type.DOUBLE; }
        public bool IsString() { return Type == Type.STRING; }
        public bool IsBoolean() { return Type == Type.BOOLEAN; }
        public bool IsDate() { return Type == Type.DATE; }
        public bool IsTime() { return Type == Type.TIME; }
        public bool IsObject() { return Type == Type.OBJECT; }
        public bool IsCounter() { return Type == Type.COUNTER; }
        public bool IsMap () { return Type == Type.MAP; }
        public bool IsList() { return Type == Type.LIST; }
        public bool IsSet() { return Type == Type.SET; }
        public bool IsNull() { return Type == Type.NULL; }
        public bool IsNumeric() { return IsInt() || IsDouble(); }
        public bool IsCollection() { return IsMap() || IsList() || IsSet(); }
        public bool IsNullable() { return IsString() || IsObject() || IsMap() || IsList() || IsSet() || IsNull(); }
        public bool IsCursor() { return Type == Type.CURSOR; }
        public bool IsIn() { return Type == Type.IN; }
        public bool IsOut() { return Type == Type.OUT; }
        public bool IsVoid() { return Type == Type.VOID; }

        public override bool Equals(object obj)
        {
            if (obj is Tipo t)
            {
                if (IsInt() || IsDouble() || IsBoolean() || IsCursor())
                {
                    return Type == t.Type;
                }
                else
                {
                    if (IsObject() && t.IsObject())
                    {
                        return Objeto.Equals(t.Objeto);
                    }
                    else if (IsMap() && t.IsMap())
                    {
                        if(Clave != null  && Valor != null)
                            return Clave.Equals(t.Clave) && Valor.Equals(t.Valor);
                    }
                    else if (IsList() && t.IsList())
                    {
                        if(Valor != null) 
                            return Valor.Equals(t.Valor);
                    }
                    else if (IsSet() && t.IsSet())
                    {
                        if(Valor != null)
                            return Valor.Equals(t.Valor);
                    }

                    return t.IsNull() ? true : Type == t.Type;
                }
            }

            return base.Equals(obj);
        }

        public bool EqualsCollection(Tipo t)
        {
            if (IsMap() && t.IsMap())
                return true;
            else if (IsList() && t.IsList())
                return true;
            else if (IsSet() && t.IsSet())
                return true;
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            if (IsCollection())
            {
                if (IsMap())
                {
                    if (Clave != null && Valor != null)
                        return "MAP<" + Clave.ToString() + ", " + Valor.ToString() + ">";
                }
                else if (IsList())
                {
                    if (Valor != null)
                        return "LIST<" + Valor.ToString() + ">";
                }
                else
                {
                    if (Valor != null)
                        return "SET<" + Valor.ToString() + ">";
                }
            }
            else
            {
                if (IsObject())
                    return Objeto;
                else
                    return Type.ToString();
            }

            return base.ToString();
        }
    }

    public enum Type
    {
        INT,
        DOUBLE,
        STRING,
        BOOLEAN,
        DATE,
        TIME,
        OBJECT,
        COUNTER,
        MAP,
        LIST,
        SET,
        NULL,
        CURSOR,
        VOID,
        IN,
        OUT
    }

    public enum Rol
    {
        VARIABLE,
        FUNCION,
        COLLECTION,
        ATRIBUTO,
        USERTYPE,
        BD,
        TABLA,
        COLUMNA,
        PRIMARY,
        PROCEDIMIENTO
    }
}
