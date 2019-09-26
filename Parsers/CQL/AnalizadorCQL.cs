using GramaticasCQL.Parsers.CQL.ast;
using GramaticasCQL.Parsers.CQL.ast.entorno;
using GramaticasCQL.Parsers.CQL.ast.expresion;
using GramaticasCQL.Parsers.CQL.ast.expresion.operacion;
using GramaticasCQL.Parsers.CQL.ast.instruccion;
using GramaticasCQL.Parsers.CQL.ast.instruccion.ciclos;
using GramaticasCQL.Parsers.CQL.ast.instruccion.condicionales;
using GramaticasCQL.Parsers.CQL.ast.instruccion.ddl;
using Irony.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Type = GramaticasCQL.Parsers.CQL.ast.entorno.Type;

namespace GramaticasCQL.Parsers.CQL
{
    class AnalizadorCQL
    {
        public AnalizadorCQL()
        {
            Raiz = null;
            Bloque = "";
        }

        public ParseTree Raiz { get; set; }
        public string Bloque { get; set; }

        public bool AnalizarEntrada(String entrada)
        {
            GramaticaCQL gramatica = new GramaticaCQL();
            LanguageData lenguaje = new LanguageData(gramatica);
            Parser parser = new Parser(lenguaje);
            ParseTree arbol = parser.Parse(entrada);
            this.Raiz = arbol;

            if (arbol.Root != null)
                return true;

            return false;
        }

        public object GenerarArbol(ParseTreeNode raiz)
        {
            string r = raiz.ToString();
            ParseTreeNode[] hijos = null;

            object obj;
            object obj2;
            object obj3;
            int contador; 

            if (raiz.ChildNodes.Count > 0)
            {
                hijos = raiz.ChildNodes.ToArray();
            }


            int linea;
            int columna;

            switch (r)
            {
                case "INICIO":
                    return GenerarArbol(hijos[0]);
                case "INSTRUCCIONES":
                    LinkedList<NodoASTCQL> sentencias = new LinkedList<NodoASTCQL>();
                    foreach (ParseTreeNode hijo in hijos)
                    {
                        sentencias.AddLast((NodoASTCQL)GenerarArbol(hijo));
                    }
                    return new ASTCQL(sentencias, Bloque);
                case "INSTRUCCION":
                    obj = GenerarArbol(hijos[0]);
                    r = hijos[0].Term.Name;
                    if (!r.Equals("IF_STMT") && !r.Equals("SWITCH_STMT") && !r.Equals("WHILE_STMT") && !r.Equals("FOR_STMT")
                        && !r.Equals("TRYCATCH_STMT") && !r.Equals("FOREACH_STMT") && !r.Equals("FUNDEF") && !r.Equals("PROCDEF"))
                    {
                        Bloque += ";\n";
                    }
                    return obj;
                case "TYPE":
                    switch (hijos[0].Term.Name)
                    {
                        case "int":
                            Bloque += "int";
                            return new Tipo(Type.INT);
                        case "double":
                            Bloque += "double";
                            return new Tipo(Type.DOUBLE);
                        case "string":
                            Bloque += "string";
                            return new Tipo(Type.STRING);
                        case "boolean":
                            Bloque += "boolean";
                            return new Tipo(Type.BOOLEAN);
                        case "date":
                            Bloque += "date";
                            return new Tipo(Type.DATE);
                        case "time":
                            Bloque += "time";
                            return new Tipo(Type.TIME);
                        case "identifier":
                            Bloque += hijos[0].Token.Text.ToLower();
                            return new Tipo(hijos[0].Token.Text.ToLower());
                        case "counter":
                            Bloque += "counter";
                            return new Tipo(Type.COUNTER);
                        case "map":
                            Bloque += "map";
                            return new Tipo(Type.MAP);
                        case "list":
                            Bloque += "list";
                            return new Tipo(Type.LIST);
                        case "set":
                            Bloque += "set";
                            return new Tipo(Type.SET);
                        default:
                            return null;
                    }
                case "TYPE_PRIMITIVE":
                    switch (hijos[0].Term.Name)
                    {
                        case "int":
                            Bloque += "int";
                            return new Tipo(Type.INT);
                        case "double":
                            Bloque += "double";
                            return new Tipo(Type.DOUBLE);
                        case "string":
                            Bloque += "string";
                            return new Tipo(Type.STRING);
                        case "boolean":
                            Bloque += "boolean";
                            return new Tipo(Type.BOOLEAN);
                        case "date":
                            Bloque += "date";
                            return new Tipo(Type.DATE);
                        case "time":
                            Bloque += "time";
                            return new Tipo(Type.TIME);
                        default:
                            return null;
                    }
                case "TYPE_COLLECTION":
                    switch (hijos[0].Term.Name)
                    {
                        case "int":
                            Bloque += "int";
                            return new Tipo(Type.INT);
                        case "double":
                            Bloque += "double";
                            return new Tipo(Type.DOUBLE);
                        case "string":
                            Bloque += "string";
                            return new Tipo(Type.STRING);
                        case "boolean":
                            Bloque += "boolean";
                            return new Tipo(Type.BOOLEAN);
                        case "date":
                            Bloque += "date";
                            return new Tipo(Type.DATE);
                        case "time":
                            Bloque += "time";
                            return new Tipo(Type.TIME);
                        case "identifier":
                            Bloque += hijos[0].Token.Text;
                            return new Tipo(hijos[0].Token.Text.ToLower());
                        case "counter":
                            Bloque += "counter";
                            return new Tipo(Type.COUNTER);
                        case "map":
                            Bloque += "map<";
                            obj = GenerarArbol(hijos[2]);
                            Bloque += ",";
                            obj2 = GenerarArbol(hijos[4]);
                            Bloque += ">";
                            return new Tipo((Tipo)obj, (Tipo)obj2);
                        case "list":
                            Bloque += "list<";
                            obj = GenerarArbol(hijos[2]);
                            Bloque += ">";
                            return new Tipo(Type.LIST, (Tipo)obj);
                        case "set":
                            Bloque += "set<";
                            obj = GenerarArbol(hijos[2]);
                            Bloque += ">";
                            return new Tipo(Type.SET, (Tipo)obj);
                        default:
                            return null;
                    }
                case "TYPEDEF":
                    linea = hijos[0].Token.Location.Line + 1;
                    columna = hijos[0].Token.Location.Column + 1;

                    if (hijos.Count() == 6)
                    {
                        Bloque += "create type " + hijos[2].Token.Text + "(";
                        obj = GenerarArbol(hijos[4]);
                        Bloque += ")";
                        return new TypeCrear(hijos[2].Token.Text, false, (LinkedList<Simbolo>)obj, linea, columna);
                    }
                    else
                    {
                        Bloque += "Create Type if not exists " + hijos[3].Token.Text + "(";
                        obj = GenerarArbol(hijos[5]);
                        Bloque += ")";
                        return new TypeCrear(hijos[3].Token.Text, true, (LinkedList<Simbolo>)obj, linea, columna);
                    }
                case "ATTRIBUTE_LIST": /*CLASE PARA ESTO, VERIFICAR TIPOS*/
                    LinkedList<Simbolo> atribute = new LinkedList<Simbolo>();
                    contador = 1;
                    foreach (ParseTreeNode hijo in hijos)
                    {
                        Simbolo atr = (Simbolo)GenerarArbol(hijo);

                        bool cont = false;
                        foreach (Simbolo sim in atribute)
                        {
                            if(sim.Id.Equals(atr.Id))
                            {
                                cont = true;
                                break;
                            }
                        }

                        if (cont)
                            continue;

                        atribute.AddLast(atr);

                        if (hijos.Count() != contador++)
                            Bloque += ", ";
                    }
                    return atribute;
                case "ATTRIBUTE":
                    Bloque += hijos[0].Token.Text + " ";
                    return new Simbolo((Tipo)GenerarArbol(hijos[1]), Rol.ATRIBUTO, hijos[0].Token.Text.ToLower());
                case "USE":
                    linea = hijos[0].Token.Location.Line + 1;
                    columna = hijos[0].Token.Location.Column + 1;
                    Bloque += "use " + hijos[1].Token.Text;
                    return new Use(hijos[1].Token.Text, linea, columna);
                case "DATABASEDEF":
                    linea = hijos[0].Token.Location.Line + 1;
                    columna = hijos[0].Token.Location.Column + 1;
                    if (hijos.Count() == 3)
                    {
                        Bloque += "create database " + hijos[2].Token.Text;
                        return new BDCrear(hijos[2].Token.Text, false, linea, columna);
                    }
                    else
                    {
                        Bloque += "create database if not exists " + hijos[3].Token.Text;
                        return new BDCrear(hijos[3].Token.Text, true, linea, columna);
                    }
                case "DROP":
                    linea = hijos[0].Token.Location.Line + 1;
                    columna = hijos[0].Token.Location.Column + 1;
                    if (hijos.Count() == 3)
                    {
                        Bloque += "drop database " + hijos[2].Token.Text;
                        return new BDBorrar(hijos[2].Token.Text, false, linea, columna);
                    }
                    else
                    {
                        Bloque += "drop database if not exists " + hijos[3].Token.Text;
                        return new BDBorrar(hijos[3].Token.Text, true, linea, columna);
                    }
                case "TABLEDEF":
                    linea = hijos[0].Token.Location.Line + 1;
                    columna = hijos[0].Token.Location.Column + 1;
                    TablaCrear tabla;
                    if (hijos.Count() == 6)
                    {
                        Bloque += "create table " + hijos[2].Token.Text + " (";
                        tabla = (TablaCrear)GenerarArbol(hijos[4]);
                        tabla.Id = hijos[2].Token.Text;
                    }
                    else
                    {
                        Bloque += "create table if not exists " + hijos[3].Token.Text + " (";
                        tabla = (TablaCrear)GenerarArbol(hijos[5]);
                        tabla.IfNotExist = true;
                        tabla.Id = hijos[3].Token.Text;
                    }
                    Bloque += ")";
                    tabla.Linea = linea;
                    tabla.Columna = columna;
                    return tabla;
                case "COLUMN_LIST":
                    TablaCrear t = new TablaCrear();
                    contador = 1;
                    foreach (ParseTreeNode hijo in hijos)
                    {
                        obj = GenerarArbol(hijo);
                        if (obj is Simbolo)
                            t.Simbolos.AddLast((Simbolo)obj);
                        else
                            t.Primary = (LinkedList<string>)obj;

                        if (hijos.Count() != contador++)
                            Bloque += ", ";
                    }
                    return t;
                case "COLUMN":
                    if (hijos.Count() == 2)
                    {
                        Bloque += hijos[0].Token.Text + " ";
                        return new Simbolo((Tipo)GenerarArbol(hijos[1]), Rol.COLUMNA, hijos[0].Token.Text.ToLower());
                    }
                    else if (hijos.Count() == 4)
                    {
                        Bloque += hijos[0].Token.Text + " ";
                        obj = GenerarArbol(hijos[1]);
                        Bloque += " primary key";
                        return new Simbolo((Tipo)obj, Rol.PRIMARY, hijos[0].Token.Text.ToLower());
                    }
                    else
                    {
                        Bloque += "primary key(";
                        obj = GenerarArbol(hijos[3]);
                        Bloque += ")";
                        return obj;
                    }
                case "ID_LIST":
                    LinkedList<string> idList = new LinkedList<string>();
                    contador = 1;
                    foreach (ParseTreeNode hijo in hijos)
                    {
                        idList.AddLast(hijo.Token.Text.ToLower());

                        Bloque += hijo.Token.Text;
                        if (hijos.Count() != contador++)
                            Bloque += ", ";
                    }
                    return idList;
                case "TABLEALTER":
                    linea = hijos[0].Token.Location.Line + 1;
                    columna = hijos[0].Token.Location.Column + 1;
                    if (hijos[3].Term.Name.Equals("add"))
                    {
                        Bloque += "alter table " + hijos[2].Token.Text + " add ";
                        return new TablaModificar(hijos[2].Token.Text, (LinkedList<Simbolo>)GenerarArbol(hijos[4]), linea, columna);
                    }
                    else
                    {
                        Bloque += "alter table " + hijos[2].Token.Text + " drop ";
                        return new TablaModificar(hijos[2].Token.Text, (LinkedList<string>)GenerarArbol(hijos[4]), linea, columna);
                    }
                case "TABLEDROP":
                    linea = hijos[0].Token.Location.Line + 1;
                    columna = hijos[0].Token.Location.Column + 1;
                    if (hijos.Count() == 3)
                    {
                        Bloque += "drop table " + hijos[2].Token.Text;
                        return new TablaBorrar(hijos[2].Token.Text, false, linea, columna);
                    }
                    Bloque += "drop table if exists " + hijos[3].Token.Text;
                    return new TablaBorrar(hijos[3].Token.Text, true, linea, columna);
                case "TABLETRUNCATE":
                    linea = hijos[0].Token.Location.Line + 1;
                    columna = hijos[0].Token.Location.Column + 1;
                    Bloque += "truncate table " + hijos[2].Token.Text + " add ";
                    return new TablaTruncar(hijos[2].Token.Text, linea, columna);
                case "COMMIT":
                    linea = hijos[0].Token.Location.Line + 1;
                    columna = hijos[0].Token.Location.Column + 1;
                    Bloque += "commit";
                    return new Commit(linea, columna);
                case "ROLLBACK":
                    linea = hijos[0].Token.Location.Line + 1;
                    columna = hijos[0].Token.Location.Column + 1;
                    Bloque += "rollback";
                    return new Rollback(linea, columna);
                case "USERDEF":
                    linea = hijos[0].Token.Location.Line + 1;
                    columna = hijos[0].Token.Location.Column + 1;
                    Bloque += "create user " + hijos[2].Token.Text + " with password " + hijos[5].Token.Text;
                    return new UsuarioCrear(hijos[2].Token.Text, new Cadena(hijos[5].Token.Text), linea, columna);
                case "GRANT":
                    linea = hijos[0].Token.Location.Line + 1;
                    columna = hijos[0].Token.Location.Column + 1;
                    Bloque += "grant " + hijos[1].Token.Text + " on " + hijos[3].Token.Text;
                    return new UsuarioGrant(hijos[1].Token.Text, hijos[3].Token.Text, linea, columna);
                case "REVOKE":
                    linea = hijos[0].Token.Location.Line + 1;
                    columna = hijos[0].Token.Location.Column + 1;
                    Bloque += "revoke " + hijos[1].Token.Text + " on " + hijos[3].Token.Text;
                    return new UsuarioRevoke(hijos[1].Token.Text, hijos[3].Token.Text, linea, columna);
                case "WHERE":
                    linea = hijos[0].Token.Location.Line + 1;
                    columna = hijos[0].Token.Location.Column + 1;
                    if (hijos.Count() == 2)
                    {
                        Bloque += " where ";
                        return new Where((Expresion)GenerarArbol(hijos[1]), linea, columna);
                    }
                    else if (hijos.Count() == 4)
                    {
                        Bloque += " where ";
                        obj = GenerarArbol(hijos[1]);
                        Bloque += " in ";
                        obj2 = GenerarArbol(hijos[3]);
                        return new Where((Expresion)obj, (LinkedList<Expresion>)obj2, linea, columna);
                    }
                    Bloque += " where ";
                    obj = GenerarArbol(hijos[1]);
                    Bloque += " in (";
                    obj2 = GenerarArbol(hijos[4]);
                    Bloque += ")";
                    return new Where((Expresion)obj, (LinkedList<Expresion>)obj2, linea, columna);
                case "INSERT":
                    linea = hijos[0].Token.Location.Line + 1;
                    columna = hijos[0].Token.Location.Column + 1;
                    if (hijos.Count() == 7)
                    {
                        Bloque += "insert into " + hijos[2].Token.Text + " values (";
                        obj = GenerarArbol(hijos[5]);
                        Bloque += ")";
                        return new Insertar(hijos[2].Token.Text, (LinkedList<Expresion>)obj, linea, columna);
                    }
                    Bloque += "insert into " + hijos[2].Token.Text + " (";
                    obj = GenerarArbol(hijos[4]);
                    Bloque += ") values (";
                    obj2 = GenerarArbol(hijos[8]);
                    Bloque += ")";
                    return new Insertar(hijos[2].Token.Text, (LinkedList<string>)obj, (LinkedList<Expresion>)obj2, linea, columna);
                case "UPDATE":
                    linea = hijos[0].Token.Location.Line + 1;
                    columna = hijos[0].Token.Location.Column + 1;
                    Bloque += "update " + hijos[1].Token.Text + " set ";
                    if (hijos.Count() == 4)
                        return new Actualizar(hijos[1].Token.Text, (LinkedList<Instruccion>)GenerarArbol(hijos[3]), linea, columna);
                    return new Actualizar(hijos[1].Token.Text, (LinkedList<Instruccion>)GenerarArbol(hijos[3]), (Where)GenerarArbol(hijos[4]), linea, columna);
                case "DELETE":
                    linea = hijos[0].Token.Location.Line + 1;
                    columna = hijos[0].Token.Location.Column + 1;
                    if (hijos.Count() == 3)
                    {
                        Bloque += "delete from " + hijos[2].Token.Text;
                        return new Eliminar(hijos[2].Token.Text, linea, columna);
                    }
                    else if (hijos.Count() == 4)
                    {
                        if (hijos[1].Term.Name.Equals("from"))
                        {
                            Bloque += "delete from " + hijos[2].Token.Text;
                            return new Eliminar(hijos[2].Token.Text, (Where)GenerarArbol(hijos[3]), linea, columna);
                        }
                        else
                        {
                            Bloque += "delete ";
                            obj = GenerarArbol(hijos[1]);
                            Bloque += " from " + hijos[3].Token.Text;
                            return new Eliminar((Expresion)obj, hijos[3].Token.Text, linea, columna);
                        }
                    }
                    else
                    {
                        Bloque += "delete ";
                        obj = GenerarArbol(hijos[1]);
                        Bloque += " from " + hijos[3].Token.Text;
                        return new Eliminar((Expresion)GenerarArbol(hijos[1]), hijos[3].Token.Text, (Where)GenerarArbol(hijos[4]), linea, columna);
                    }
                case "SELECT":
                    linea = hijos[0].Token.Location.Line + 1;
                    columna = hijos[0].Token.Location.Column + 1;
                    if (hijos.Count() == 4)
                    {
                        Bloque += "select ";
                        obj = GenerarArbol(hijos[1]);
                        Bloque += " from " + hijos[3].Token.Text;
                        return new Seleccionar((LinkedList<Expresion>)obj, hijos[3].Token.Text, linea, columna);
                    }
                    else if (hijos.Count() == 5)
                    {
                        Bloque += "select ";
                        obj = GenerarArbol(hijos[1]);
                        Bloque += " from " + hijos[3].Token.Text;
                        return new Seleccionar((LinkedList<Expresion>)obj, hijos[3].Token.Text, (Where)GenerarArbol(hijos[4]), linea, columna);
                    }
                    else if (hijos.Count() == 6)
                    {
                        Bloque += "select ";
                        obj = GenerarArbol(hijos[1]);
                        Bloque += " from " + hijos[3].Token.Text + " limit ";
                        return new Seleccionar((LinkedList<Expresion>)obj, hijos[3].Token.Text, (Expresion)GenerarArbol(hijos[5]), linea, columna);
                    }
                    else if (hijos.Count() == 7)
                    {
                        if (hijos[5].Term.Name.Equals("limit"))
                        {
                            Bloque += "select ";
                            obj = GenerarArbol(hijos[1]);
                            Bloque += " from " + hijos[3].Token.Text;
                            obj2 = GenerarArbol(hijos[4]);
                            Bloque += " limit ";
                            return new Seleccionar((LinkedList<Expresion>)obj, hijos[3].Token.Text, (Where)obj2, (Expresion)GenerarArbol(hijos[6]), linea, columna);
                        }
                        else
                        {
                            Bloque += "select ";
                            obj = GenerarArbol(hijos[1]);
                            Bloque += " from " + hijos[3].Token.Text + " order by ";
                            return new Seleccionar((LinkedList<Expresion>)obj, hijos[3].Token.Text, (LinkedList<Identificador>)GenerarArbol(hijos[6]), linea, columna);
                        }
                    }
                    else if (hijos.Count() == 8)
                    {
                        Bloque += "select ";
                        obj = GenerarArbol(hijos[1]);
                        Bloque += " from " + hijos[3].Token.Text;
                        obj2 = GenerarArbol(hijos[4]);
                        Bloque += " order by ";
                        return new Seleccionar((LinkedList<Expresion>)obj, hijos[3].Token.Text, (Where)obj2, (LinkedList<Identificador>)GenerarArbol(hijos[7]), linea, columna);
                    }
                    else if (hijos.Count() == 9)
                    {
                        Bloque += "select ";
                        obj = GenerarArbol(hijos[1]);
                        Bloque += " from " + hijos[3].Token.Text + " order by ";
                        obj2 = GenerarArbol(hijos[6]);
                        Bloque += " limit ";
                        return new Seleccionar((LinkedList<Expresion>)obj, hijos[3].Token.Text, (LinkedList<Identificador>)obj2, (Expresion)GenerarArbol(hijos[8]), linea, columna);
                    }
                    Bloque += "select ";
                    obj = GenerarArbol(hijos[1]);
                    Bloque += " from " + hijos[3].Token.Text;
                    obj2 = GenerarArbol(hijos[4]);
                    Bloque += " order by ";
                    obj3 = GenerarArbol(hijos[7]);
                    Bloque += " limit ";
                    return new Seleccionar((LinkedList<Expresion>)obj, hijos[3].Token.Text, (Where)obj2, (LinkedList<Identificador>)obj3, (Expresion)GenerarArbol(hijos[9]), linea, columna);
                case "SELECT_EXP":
                    if (hijos[0].Term.Name.Equals("*"))
                    {
                        Bloque += "*";
                        return null;
                    }
                    return GenerarArbol(hijos[0]);
                case "ORDER_LIST":
                    LinkedList<Identificador> order = new LinkedList<Identificador>();
                    contador = 1;
                    foreach (ParseTreeNode hijo in hijos)
                    {
                        order.AddLast((Identificador)GenerarArbol(hijo));

                        if (hijos.Count() != contador++)
                            Bloque += ", ";
                    }
                    return order;
                case "ORDER":
                    linea = hijos[0].Token.Location.Line + 1;
                    columna = hijos[0].Token.Location.Column + 1;
                    if (hijos.Count() == 2)
                    {
                        if (hijos[1].Term.Name.Equals("desc"))
                        {
                            Bloque += hijos[0].Token.Text + " desc";
                            return new Identificador(hijos[0].Token.Text, false, false, linea, columna);
                        }
                        else
                        {
                            Bloque += hijos[0].Token.Text + " asc";
                            return new Identificador(hijos[0].Token.Text, linea, columna);
                        }
                    }
                    else
                    {
                        Bloque += hijos[0].Token.Text;
                        return new Identificador(hijos[0].Token.Text, linea, columna);
                    }
                case "BATCH":
                    linea = hijos[0].Token.Location.Line + 1;
                    columna = hijos[0].Token.Location.Column + 1;
                    Bloque += "begin batch ";
                    obj = GenerarArbol(hijos[2]);
                    Bloque += " appy batch";
                    return new Batch((LinkedList<Instruccion>)obj, linea, columna);
                case "DML_LIST":
                    LinkedList<Instruccion> dmlList = new LinkedList<Instruccion>();
                    foreach (ParseTreeNode hijo in hijos)
                    {
                        dmlList.AddLast((Instruccion)GenerarArbol(hijo));
                        Bloque += ";\n";
                    }
                    return dmlList;
                case "DML":
                    return GenerarArbol(hijos[0]);
                case "BLOQUE":
                    linea = hijos[0].Token.Location.Line + 1;
                    columna = hijos[0].Token.Location.Column + 1;
                    if (hijos.Count() == 3)
                    {
                        Bloque += "{\n";
                        obj = GenerarArbol(hijos[1]);
                        Bloque += "}\n";

                        return new Bloque((LinkedList<NodoASTCQL>)obj, Bloque, linea, columna);
                    }
                    else
                        return new Bloque(null, "", linea, columna);
                case "SENTENCIAS":
                    LinkedList<NodoASTCQL> bloques = new LinkedList<NodoASTCQL>();
                    foreach (ParseTreeNode hijo in hijos)
                    {
                        bloques.AddLast((NodoASTCQL)GenerarArbol(hijo));
                    }
                    return bloques;
                case "SENTENCIA":
                    obj = GenerarArbol(hijos[0]);
                    r = hijos[0].Term.Name;
                    if (!r.Equals("IF_STMT") && !r.Equals("SWITCH_STMT") && !r.Equals("WHILE_STMT") && !r.Equals("FOR_STMT") 
                        && !r.Equals("TRYCATCH_STMT") && !r.Equals("FOREACH_STMT"))
                    {
                        Bloque += ";\n";
                    }
                    return obj;
                case "TARGET_LIST":
                    LinkedList<Expresion> target = new LinkedList<Expresion>();
                    contador = 1;
                    foreach (ParseTreeNode hijo in hijos)
                    {
                        target.AddLast((Expresion)GenerarArbol(hijo));

                        if (hijos.Count() != contador++)
                            Bloque += ", ";
                    }
                    return target;
                case "TARGET":
                    if (hijos[0].Term.Name.Equals("identifier"))
                    {
                        Bloque += hijos[0].Token.Text;
                        return new Identificador(hijos[0].Token.Text, hijos[0].Token.Location.Line + 1, hijos[0].Token.Location.Column + 1);
                    }
                    else if (hijos[0].Term.Name.Equals("identifier2"))
                    {
                        Bloque += hijos[0].Token.Text;
                        return new Identificador(hijos[0].Token.Text, true, hijos[0].Token.Location.Line + 1, hijos[0].Token.Location.Column + 1);
                    }
                    else
                    {
                        return GenerarArbol(hijos[0]);
                    }
                case "EXPRESSION_STMT":
                    if (hijos.Count() == 1)
                        return GenerarArbol(hijos[0]);
                    else
                    {
                        linea = hijos[1].Token.Location.Line + 1;
                        columna = hijos[1].Token.Location.Column + 1;
                        obj = GenerarArbol(hijos[0]);
                        Bloque += hijos[1].Term.Name;
                        return new Unario((Expresion)obj, GetOperador(hijos[1]), linea, columna);
                    }
                case "DECLARATION_STMT":
                    linea = hijos[0].ChildNodes.ToArray()[0].Token.Location.Line + 1;
                    columna = hijos[0].ChildNodes.ToArray()[0].Token.Location.Column + 1;

                    if (hijos.Count() == 2)
                    {
                        obj = GenerarArbol(hijos[0]);
                        Bloque += " ";
                        return new Declaracion((Tipo)obj, (LinkedList<Expresion>)GenerarArbol(hijos[1]), null, linea, columna);
                    }
                    else
                    {
                        obj = GenerarArbol(hijos[0]);
                        Bloque += " ";
                        obj2 = GenerarArbol(hijos[1]);
                        Bloque += " = ";
                        return new Declaracion((Tipo)obj, (LinkedList<Expresion>)obj2, (Expresion)GenerarArbol(hijos[3]), linea, columna);
                    }
                case "ASSIGNMENT_STMT":
                    linea = hijos[1].Token.Location.Line + 1;
                    columna = hijos[1].Token.Location.Column + 1;
                    if (hijos[2].Term.Name.Equals("CALL"))
                    {
                        LinkedList<Expresion> targetList = new LinkedList<Expresion>();
                        targetList.AddLast((Expresion)GenerarArbol(hijos[0]));
                        Bloque += " = ";
                        return new AsignacionCall(targetList, (Call)GenerarArbol(hijos[2]), linea, columna);
                    }
                    obj = GenerarArbol(hijos[0]);
                    Bloque += " = ";
                    return new Asignacion((Expresion)obj, (Expresion)GenerarArbol(hijos[2]), linea, columna);
                case "ASSIGNMENT_CALL":
                    linea = hijos[1].Token.Location.Line + 1;
                    columna = hijos[1].Token.Location.Column + 1;
                    obj = GenerarArbol(hijos[0]);
                    Bloque += " = ";
                    return new AsignacionCall((LinkedList<Expresion>)obj, (Call)GenerarArbol(hijos[2]), linea, columna);
                case "ASSIGNMENT_LIST":
                    LinkedList<Instruccion> asignaLista = new LinkedList<Instruccion>();
                    contador = 1;
                    foreach (ParseTreeNode hijo in hijos)
                    {
                        asignaLista.AddLast((Instruccion)GenerarArbol(hijo));

                        if (hijos.Count() != contador++)
                            Bloque += ", ";
                    }
                    return asignaLista;
                case "ASSIGNMENTS":
                    return GenerarArbol(hijos[0]);
                case "AUGMENTED_ASSIGNMENT_STMT":
                    linea = hijos[1].ChildNodes.ToArray()[0].Token.Location.Line + 1;
                    columna = hijos[1].ChildNodes.ToArray()[0].Token.Location.Column + 1;
                    return new AsignacionOperacion((Expresion)GenerarArbol(hijos[0]), (Operador)GenerarArbol(hijos[1]), (Expresion)GenerarArbol(hijos[2]), linea, columna);
                case "AUG_OPERATOR":
                    Bloque += hijos[0].Term.Name;
                    return GetAugOperador(hijos[0]);
                case "IF_STMT":
                    if (hijos.Count() == 1)
                        return new If((LinkedList<SubIf>)GenerarArbol(hijos[0]), 0, 0);
                    else
                    {
                        LinkedList<SubIf> subIfs = (LinkedList<SubIf>)GenerarArbol(hijos[0]);
                        linea = hijos[1].Token.Location.Line + 1;
                        columna = hijos[1].Token.Location.Column + 1;
                        Bloque += " else ";
                        subIfs.AddLast(new SubIf((Bloque)GenerarArbol(hijos[2]), linea, columna));
                        return new If(subIfs, 0, 0);
                    }
                case "IF_LIST":
                    if (hijos.Count() == 5)
                    {
                        Bloque += "if (";
                        obj = GenerarArbol(hijos[2]);
                        Bloque += ")";
                        LinkedList<SubIf> subIfs = new LinkedList<SubIf>();
                        linea = hijos[0].Token.Location.Line + 1;
                        columna = hijos[0].Token.Location.Column + 1;
                        subIfs.AddLast(new SubIf((Expresion)obj, (Bloque)GenerarArbol(hijos[4]), linea, columna));
                        return subIfs;
                    }
                    else
                    {
                        LinkedList<SubIf> subIfs = (LinkedList<SubIf>)GenerarArbol(hijos[0]);
                        Bloque += " else if (";
                        obj = GenerarArbol(hijos[4]);
                        Bloque += ")";
                        linea = hijos[1].Token.Location.Line + 1;
                        columna = hijos[1].Token.Location.Column + 1;
                        subIfs.AddLast(new SubIf((Expresion)obj, (Bloque)GenerarArbol(hijos[6]), linea, columna));
                        return subIfs;

                    }
                case "SWITCH_STMT":
                    linea = hijos[0].Token.Location.Line + 1;
                    columna = hijos[0].Token.Location.Column + 1;

                    Bloque += "switch (";
                    obj = GenerarArbol(hijos[2]);
                    Bloque += "){\n";
                    obj2 = GenerarArbol(hijos[5]);


                    if (hijos.Count() == 7)
                    {
                        Bloque += "}\n";
                        return new Switch((Expresion)obj, (LinkedList<Case>)obj2, linea, columna);
                    }
                    else
                    {
                        Bloque += "default:";
                        obj3 = GenerarArbol(hijos[8]);
                        Bloque += "}\n";
                        LinkedList<Case> cases = (LinkedList<Case>)obj2;
                        cases.AddLast(new Case((Bloque)obj3, linea, columna));
                        return new Switch((Expresion)obj, cases, linea, columna);
                    }
                case "CASES":
                    if (hijos.Count() == 4)
                    {
                        Bloque += "case ";
                        obj = GenerarArbol(hijos[1]);
                        Bloque += ":";
                        LinkedList<Case> cases = new LinkedList<Case>();
                        linea = hijos[0].Token.Location.Line + 1;
                        columna = hijos[0].Token.Location.Column + 1;
                        cases.AddLast(new Case((Expresion)obj, (Bloque)GenerarArbol(hijos[3]), linea, columna));
                        return cases;
                    }
                    else
                    {
                        LinkedList<Case> cases = (LinkedList<Case>)GenerarArbol(hijos[0]);
                        Bloque += "case ";
                        obj = GenerarArbol(hijos[2]);
                        Bloque += ":";
                        linea = hijos[1].Token.Location.Line + 1;
                        columna = hijos[1].Token.Location.Column + 1;
                        cases.AddLast(new Case((Expresion)obj, (Bloque)GenerarArbol(hijos[4]), linea, columna));
                        return cases;

                    }
                case "WHILE_STMT":
                    linea = hijos[0].Token.Location.Line + 1;
                    columna = hijos[0].Token.Location.Column + 1;
                    Bloque += "while (";
                    obj = GenerarArbol(hijos[2]);
                    Bloque += ")";
                    return new While((Expresion)obj, (Bloque)GenerarArbol(hijos[4]), linea, columna);
                case "DOWHILE_STMT":
                    linea = hijos[0].Token.Location.Line + 1;
                    columna = hijos[0].Token.Location.Column + 1;
                    Bloque += "do";
                    obj = GenerarArbol(hijos[1]);
                    Bloque += "while (";
                    obj2 = GenerarArbol(hijos[4]);
                    Bloque += ")";
                    return new DoWhile((Expresion)obj2, (Bloque)obj, linea, columna);
                case "FOR_STMT":
                    linea = hijos[0].Token.Location.Line + 1;
                    columna = hijos[0].Token.Location.Column + 1;
                    Bloque += "for (";
                    obj = GenerarArbol(hijos[2]);
                    Bloque += "; ";
                    obj2 = GenerarArbol(hijos[3]);
                    Bloque += "; ";
                    obj3 = GenerarArbol(hijos[4]);
                    Bloque += ") ";
                    return new For((Instruccion)obj, (Expresion)obj2, (NodoASTCQL)obj3, (Bloque)GenerarArbol(hijos[6]), linea, columna);
                case "FOR_INIT":
                    return GenerarArbol(hijos[0]);
                case "FOR_UPDATE":
                    if (hijos.Count() == 1)
                        return GenerarArbol(hijos[0]);
                    else
                    {
                        linea = hijos[1].Token.Location.Line + 1;
                        columna = hijos[1].Token.Location.Column + 1;
                        obj = GenerarArbol(hijos[0]);
                        Bloque += hijos[1].Term.Name;

                        return new Unario((Expresion)obj, GetOperador(hijos[1]), linea, columna);
                    }
                case "FUNDEF":
                    linea = hijos[1].Token.Location.Line + 1;
                    columna = hijos[1].Token.Location.Column + 1;
                    if (hijos.Count() == 5)
                    {
                        obj = GenerarArbol(hijos[0]);
                        obj2 = GenerarArbol(hijos[4]);
                        Bloque = "";
                        return new FuncionDef((Tipo)obj, hijos[1].Token.Text, (Bloque)obj2, linea, columna);
                    }
                    else
                    {
                        obj = GenerarArbol(hijos[0]);
                        obj2 = GenerarArbol(hijos[3]);
                        obj3 = GenerarArbol(hijos[5]);
                        Bloque = "";
                        return new FuncionDef((Tipo)obj, hijos[1].Token.Text, (LinkedList<Identificador>)obj2, (Bloque)obj3, linea, columna);
                    }
                case "PARAMETER_LIST":
                    if (hijos.Count() == 2)
                    {
                        linea = hijos[1].Token.Location.Line + 1;
                        columna = hijos[1].Token.Location.Column + 1;

                        LinkedList<Identificador> parametro = new LinkedList<Identificador>();
                        Identificador id = new Identificador(hijos[1].Token.Text, linea, columna);
                        id.Tipo = (Tipo)GenerarArbol(hijos[0]);
                        Bloque += " " + hijos[1].Token.Text;
                        parametro.AddLast(id);
                        return parametro;
                    }
                    else
                    {
                        linea = hijos[3].Token.Location.Line + 1;
                        columna = hijos[3].Token.Location.Column + 1;

                        LinkedList<Identificador> parametro = (LinkedList<Identificador>)GenerarArbol(hijos[0]);
                        Bloque += ", ";
                        Identificador id = new Identificador(hijos[3].Token.Text, linea, columna);
                        id.Tipo = (Tipo)GenerarArbol(hijos[2]);
                        Bloque += " " + hijos[3].Token.Text;
                        parametro.AddLast(id);
                        return parametro;
                    }
                case "PROCDEF":
                    linea = hijos[0].Token.Location.Line + 1;
                    columna = hijos[0].Token.Location.Column + 1;
                    if (hijos.Count() == 10)
                    {
                        obj = GenerarArbol(hijos[3]);
                        obj2 = GenerarArbol(hijos[7]);
                        Bloque = "";
                        return new ProcedimientoDef(hijos[1].Token.Text, (LinkedList<Identificador>)obj, (LinkedList<Identificador>)obj2, (Bloque)GenerarArbol(hijos[9]), linea, columna);
                    }
                    else if (hijos.Count() == 8)
                    {
                        Bloque = "";
                        return new ProcedimientoDef(hijos[1].Token.Text, null, null, (Bloque)GenerarArbol(hijos[7]), linea, columna);
                    }
                    else
                    {
                        if (hijos[4].Term.Name.Equals(","))
                        {
                            obj = GenerarArbol(hijos[6]);
                            Bloque = "";
                            return new ProcedimientoDef(hijos[1].Token.Text, null, (LinkedList<Identificador>)obj, (Bloque)GenerarArbol(hijos[8]), linea, columna);
                        }
                        else
                        {
                            obj = GenerarArbol(hijos[3]);
                            Bloque = "";
                            return new ProcedimientoDef(hijos[1].Token.Text, (LinkedList<Identificador>)obj, null, (Bloque)GenerarArbol(hijos[8]), linea, columna);
                        }
                    }
                case "BREAK_STMT":
                    linea = hijos[0].Token.Location.Line + 1;
                    columna = hijos[0].Token.Location.Column + 1;
                    Bloque += "break";
                    return new Break(linea, columna);
                case "CONTINUE_STMT":
                    linea = hijos[0].Token.Location.Line + 1;
                    columna = hijos[0].Token.Location.Column + 1;
                    Bloque += "continue";
                    return new Continue(linea, columna);
                case "RETURN_STMT":
                    linea = hijos[0].Token.Location.Line + 1;
                    columna = hijos[0].Token.Location.Column + 1;
                    Bloque += "return ";
                    if (hijos.Count() == 1)
                        return new Return(linea, columna);
                    else
                        return new Return((LinkedList<Expresion>)GenerarArbol(hijos[1]), linea, columna);
                case "CURSOR_STMT":
                    linea = hijos[0].Token.Location.Line + 1;
                    columna = hijos[0].Token.Location.Column + 1;
                    Bloque += "cursor " + hijos[1].Token.Text + " is ";
                    return new CursorDef(hijos[1].Token.Text, (Seleccionar)GenerarArbol(hijos[3]), linea, columna);
                case "FOREACH_STMT":
                    linea = hijos[0].Token.Location.Line + 1;
                    columna = hijos[0].Token.Location.Column + 1;
                    if (hijos.Count() == 7)
                    {
                        Bloque += "for each ( ) in " + hijos[5].Token.Text;
                        return new ForEach(null, hijos[5].Token.Text, (Bloque)GenerarArbol(hijos[6]), linea, columna);
                    }
                    Bloque += "for each (";
                    obj = GenerarArbol(hijos[3]);
                    Bloque += ") in " + hijos[6].Token.Text;
                    return new ForEach((LinkedList<Identificador>)obj, hijos[6].Token.Text, (Bloque)GenerarArbol(hijos[7]), linea, columna);
                case "OPEN_STMT":
                    linea = hijos[0].Token.Location.Line + 1;
                    columna = hijos[0].Token.Location.Column + 1;
                    Bloque += "open " + hijos[1].Token.Text;
                    return new Open(hijos[1].Token.Text, linea, columna);
                case "CLOSE_STMT":
                    linea = hijos[0].Token.Location.Line + 1;
                    columna = hijos[0].Token.Location.Column + 1;
                    Bloque += "close " + hijos[1].Token.Text;
                    return new Close(hijos[1].Token.Text, linea, columna);
                case "LOG_STMT":
                    Bloque += "log (";
                    obj = GenerarArbol(hijos[2]);
                    Bloque += ")";
                    return new Log((Expresion)obj, hijos[0].Token.Location.Line + 1, hijos[0].Token.Location.Column + 1);
                case "THROW_STMT":
                    linea = hijos[0].Token.Location.Line + 1;
                    columna = hijos[0].Token.Location.Column + 1;
                    Bloque += "throw new " + hijos[2].Token.Text;
                    return new Throw(hijos[2].Token.Text, linea, columna);
                case "TRYCATCH_STMT":
                    linea = hijos[0].Token.Location.Line + 1;
                    columna = hijos[0].Token.Location.Column + 1;
                    if (hijos.Count() == 6)
                    {
                        Bloque += "try";
                        obj = GenerarArbol(hijos[1]);
                        Bloque += "catch ( )";
                        return new TryCatch((Bloque)obj, (Bloque)GenerarArbol(hijos[5]), linea, columna);
                    }
                    Bloque += "try";
                    obj = GenerarArbol(hijos[1]);
                    Bloque += "catch (";
                    obj2 = GenerarArbol(hijos[4]);
                    Bloque += ")";
                    return new TryCatch((Bloque)obj, (LinkedList<Identificador>)obj2, (Bloque)GenerarArbol(hijos[6]), linea, columna);
                case "EXPRESSION_LIST":
                    LinkedList<Expresion> exprList = new LinkedList<Expresion>();
                    contador = 1;
                    foreach (ParseTreeNode hijo in hijos)
                    {
                        exprList.AddLast((Expresion)GenerarArbol(hijo));

                        if (hijos.Count() != contador++)
                            Bloque += ", ";
                    }
                    return exprList;
                case "EXPRESSION":
                    return GenerarArbol(hijos[0]);
                case "INSTANCE":
                    linea = hijos[0].Token.Location.Line + 1;
                    columna = hijos[0].Token.Location.Column + 1;
                    if (hijos.Count() == 2)
                    {
                        Bloque += "new " + hijos[1].Token.Text;
                        return new Instancia(hijos[1].Token.Text, linea, columna);
                    }
                    else if (hijos.Count() == 5)
                    {
                        Bloque += "new " + hijos[1].Term.Name + "<";
                        obj = GenerarArbol(hijos[3]);
                        Bloque += ">";
                        return new Instancia(hijos[1].Token.Text, (Tipo)obj, linea, columna);
                    }
                    else
                    {
                        Bloque += "new " + hijos[1].Term.Name + "<";
                        obj = GenerarArbol(hijos[3]);
                        Bloque += ", ";
                        obj2 = GenerarArbol(hijos[5]);
                        Bloque += ">";

                        return new Instancia(hijos[1].Token.Text, (Tipo)obj, (Tipo)obj2, linea, columna);
                    }
                case "CONDITIONAL_EXPRESSION":
                    if (hijos.Count() == 1)
                        return GenerarArbol(hijos[0]);
                    else
                    {
                        linea = hijos[1].Token.Location.Line + 1;
                        columna = hijos[1].Token.Location.Column + 1;
                        obj = GenerarArbol(hijos[0]);
                        Bloque += "? ";
                        obj2 = GenerarArbol(hijos[2]);
                        Bloque += ": ";
                        return new Ternario((Expresion)obj, (Expresion)obj2, (Expresion)GenerarArbol(hijos[4]), linea, columna);
                    }
                case "OR_EXPR":
                    if (hijos.Count() == 1)
                        return GenerarArbol(hijos[0]);
                    else
                    {
                        linea = hijos[1].Token.Location.Line + 1;
                        columna = hijos[1].Token.Location.Column + 1;
                        obj = GenerarArbol(hijos[0]);
                        Bloque += " || ";
                        return new Logica((Expresion)obj, (Expresion)GenerarArbol(hijos[2]), Operador.OR, linea, columna);
                    }
                case "AND_EXPR":
                    if (hijos.Count() == 1)
                        return GenerarArbol(hijos[0]);
                    else
                    {
                        linea = hijos[1].Token.Location.Line + 1;
                        columna = hijos[1].Token.Location.Column + 1;
                        obj = GenerarArbol(hijos[0]);
                        Bloque += " && ";
                        return new Logica((Expresion)obj, (Expresion)GenerarArbol(hijos[2]), Operador.AND, linea, columna);
                    }
                case "XOR_EXPR":
                    if (hijos.Count() == 1)
                        return GenerarArbol(hijos[0]);
                    else
                    {
                        linea = hijos[1].Token.Location.Line + 1;
                        columna = hijos[1].Token.Location.Column + 1;
                        obj = GenerarArbol(hijos[0]);
                        Bloque += " ^ ";
                        return new Logica((Expresion)obj, (Expresion)GenerarArbol(hijos[2]), Operador.XOR, linea, columna);
                    }
                case "COMPARISON_EQ":
                    if (hijos.Count() == 1)
                        return GenerarArbol(hijos[0]);
                    else
                    {
                        linea = hijos[1].Token.Location.Line + 1;
                        columna = hijos[1].Token.Location.Column + 1;
                        obj = GenerarArbol(hijos[0]);
                        Bloque += " " + hijos[1].Term.Name + " ";
                        return new Relacional((Expresion)obj, (Expresion)GenerarArbol(hijos[2]), GetOperador(hijos[1]), linea, columna);
                    }
                case "COMPARISON":
                    if (hijos.Count() == 1)
                        return GenerarArbol(hijos[0]);
                    else
                    {
                        linea = hijos[1].ChildNodes.ToArray()[0].Token.Location.Line + 1;
                        columna = hijos[1].ChildNodes.ToArray()[0].Token.Location.Column + 1;
                        obj = GenerarArbol(hijos[0]);
                        obj2 = GenerarArbol(hijos[1]);
                        return new Relacional((Expresion)obj, (Expresion)GenerarArbol(hijos[2]), (Operador)obj2, linea, columna);
                    }
                case "COMP_OPERATOR":
                    Bloque += " " + hijos[0].Term.Name + " ";
                    return GetOperador(hijos[0]);
                case "A_EXPR":
                    if (hijos.Count() == 1)
                        return GenerarArbol(hijos[0]);
                    else
                    {
                        linea = hijos[1].Token.Location.Line + 1;
                        columna = hijos[1].Token.Location.Column + 1;
                        obj = GenerarArbol(hijos[0]);
                        Bloque += " " + hijos[1].Term.Name + " ";
                        return new Aritmetica((Expresion)obj, (Expresion)GenerarArbol(hijos[2]), GetOperador(hijos[1]), linea, columna);
                    }
                case "M_EXPR":
                    if (hijos.Count() == 1)
                        return GenerarArbol(hijos[0]);
                    else
                    {
                        linea = hijos[1].Token.Location.Line + 1;
                        columna = hijos[1].Token.Location.Column + 1;
                        obj = GenerarArbol(hijos[0]);
                        Bloque += " " + hijos[1].Term.Name + " ";
                        return new Aritmetica((Expresion)obj, (Expresion)GenerarArbol(hijos[2]), GetOperador(hijos[1]), linea, columna);
                    }
                case "U_EXPR":
                    if (hijos.Count() == 1)
                        return GenerarArbol(hijos[0]);
                    else
                    {
                        linea = hijos[0].Token.Location.Line + 1;
                        columna = hijos[0].Token.Location.Column + 1;
                        Bloque += hijos[0].Term.Name;
                        obj = GenerarArbol(hijos[1]);
                        return new Unario((Expresion)obj, GetOperador(hijos[0]), linea, columna);
                    }
                case "NOT_EXPR":
                    if (hijos.Count() == 1)
                        return GenerarArbol(hijos[0]);
                    else
                    {
                        linea = hijos[0].Token.Location.Line + 1;
                        columna = hijos[0].Token.Location.Column + 1;
                        Bloque += "!";
                        obj = GenerarArbol(hijos[1]);
                        return new Logica((Expresion)obj, linea, columna);
                    }
                case "POWER":
                    if (hijos.Count() == 1)
                        return GenerarArbol(hijos[0]);
                    else
                    {
                        linea = hijos[1].Token.Location.Line + 1;
                        columna = hijos[1].Token.Location.Column + 1;
                        obj = GenerarArbol(hijos[0]);
                        Bloque += " " + hijos[1].Term.Name + " ";
                        return new Aritmetica((Expresion)GenerarArbol(hijos[0]), (Expresion)GenerarArbol(hijos[2]), GetOperador(hijos[1]), linea, columna);
                    }
                case "SHIFT_EXPR":
                    if (hijos.Count() == 1)
                        return GenerarArbol(hijos[0]);
                    else
                    {
                        linea = hijos[1].Token.Location.Line + 1;
                        columna = hijos[1].Token.Location.Column + 1;
                        obj = GenerarArbol(hijos[0]);
                        Bloque += " " + hijos[1].Term.Name + " ";
                        return new Unario((Expresion)obj, GetOperador(hijos[1]), linea, columna);
                    }
                case "PRIMARY":
                    return GenerarArbol(hijos[0]);
                case "ATOM":
                    if (hijos[0].Term.Name.Equals("identifier"))
                    {
                        linea = hijos[0].Token.Location.Line + 1;
                        columna = hijos[0].Token.Location.Column + 1;
                        Bloque += hijos[0].Token.Text;
                        return new Identificador(hijos[0].Token.Text, linea, columna);

                    }
                    else if (hijos[0].Term.Name.Equals("identifier2"))
                    {
                        linea = hijos[0].Token.Location.Line + 1;
                        columna = hijos[0].Token.Location.Column + 1;
                        Bloque += hijos[0].Token.Text;
                        return new Identificador(hijos[0].Token.Text, true, linea, columna);
                    }
                    return GenerarArbol(hijos[0]);
                case "LITERAL":
                    linea = hijos[0].Token.Location.Line + 1;
                    columna = hijos[0].Token.Location.Column + 1;
                    if (hijos[0].Term.Name.Equals("number"))
                    {
                        try
                        {
                            int valor2 = Convert.ToInt32(hijos[0].Token.Text);
                            Bloque += valor2;
                            return new Literal(new Tipo(Type.INT), valor2, linea, columna);
                        }
                        catch (Exception)
                        {
                            double valor = Convert.ToDouble(hijos[0].Token.Text);
                            Bloque += valor;
                            return new Literal(new Tipo(Type.DOUBLE), valor, linea, columna);
                        }
                    }
                    else if (hijos[0].Term.Name.Equals("stringliteral"))
                    {
                        Bloque += hijos[0].Token.Text;
                        return new Literal(new Tipo(Type.STRING), new Cadena(hijos[0].Token.Text), linea, columna);
                    }
                    else if (hijos[0].Term.Name.Equals("true"))
                    {
                        Bloque += "true";
                        return new Literal(new Tipo(Type.BOOLEAN), true, linea, columna);
                    }
                    else if (hijos[0].Term.Name.Equals("false"))
                    {
                        Bloque += "false";
                        return new Literal(new Tipo(Type.BOOLEAN), false, linea, columna);
                    }
                    else if (hijos[0].Term.Name.Equals("date"))
                    {
                        Bloque += hijos[0].Token.Text;
                        return new Literal(new Tipo(Type.DATE), new Date(hijos[0].Token.Text), linea, columna);
                    }
                    else if (hijos[0].Term.Name.Equals("time"))
                    {
                        Bloque += hijos[0].Token.Text;
                        return new Literal(new Tipo(Type.TIME), new Time(hijos[0].Token.Text), linea, columna);
                    }
                    else if (hijos[0].Term.Name.Equals("null"))
                    {
                        Bloque += "null";
                        return new Literal(new Tipo(Type.NULL), new Null(), linea, columna);
                    }
                    return null;
                case "ATTRIBUTEREF":
                    linea = hijos[1].Token.Location.Line + 1;
                    columna = hijos[1].Token.Location.Column + 1;
                    if (hijos[2].Term.Name.Equals("identifier"))
                    {
                        obj = GenerarArbol(hijos[0]);
                        Bloque += "." + hijos[2].Token.Text;
                        return new AtributoRef((Expresion)obj, new Identificador(hijos[2].Token.Text, linea, columna), linea, columna);
                    }
                    else
                    {
                        obj = GenerarArbol(hijos[0]);
                        Bloque += ".";
                        return new AtributoRef((Expresion)obj, (Expresion)GenerarArbol(hijos[2]), linea, columna);
                    }
                case "AGGREGATION":
                    linea = hijos[0].ChildNodes.ToArray()[0].Token.Location.Line + 1;
                    columna = hijos[0].ChildNodes.ToArray()[0].Token.Location.Column + 1;
                    obj = GenerarArbol(hijos[0]);
                    Bloque += "(<<";
                    obj2 = GenerarArbol(hijos[3]);
                    Bloque += ">>)";
                    return new Agregacion((Aggregation)obj, (Seleccionar)obj2, linea, columna);
                case "AGGREGATION_FUN":
                    Bloque += hijos[0].Term.Name;
                    switch (hijos[0].Term.Name)
                    {
                        case "count":
                            return Aggregation.COUNT;
                        case "min":
                            return Aggregation.MIN;
                        case "max":
                            return Aggregation.MAX;
                        case "sum":
                            return Aggregation.SUM;
                        default:
                            return Aggregation.AVG;
                    }
                case "ENCLOSURE":
                    return GenerarArbol(hijos[0]);
                case "PARENTH_FORM":
                    if (hijos.Count() == 3)
                    {
                        Bloque += "(";
                        obj = GenerarArbol(hijos[1]);
                        Bloque += ")";
                        return obj;
                    }
                    else
                    {
                        linea = hijos[0].Token.Location.Line + 1;
                        columna = hijos[0].Token.Location.Column + 1;
                        Bloque += "(";
                        obj = GenerarArbol(hijos[1]);
                        Bloque += ") ";
                        return new Casteo((Tipo)obj, (Expresion)GenerarArbol(hijos[3]), linea, columna);
                    }
                case "MAP_DISPLAY":
                    linea = hijos[0].Token.Location.Line + 1;
                    columna = hijos[0].Token.Location.Column + 1;
                    Bloque += "[";
                    obj = GenerarArbol(hijos[1]);
                    Bloque += "]";
                    return new MapDisplay((LinkedList<CollectionValue>)obj, linea, columna);
                case "MAP_LIST":
                    if (hijos.Count() == 3)
                    {
                        LinkedList<CollectionValue> collection = new LinkedList<CollectionValue>();
                        obj = GenerarArbol(hijos[0]);
                        Bloque += ": ";
                        collection.AddLast(new CollectionValue(obj, GenerarArbol(hijos[2])));
                        return collection;
                    }
                    else
                    {
                        LinkedList<CollectionValue> collection = (LinkedList<CollectionValue>)GenerarArbol(hijos[0]);
                        Bloque += ", ";
                        obj = GenerarArbol(hijos[2]);
                        Bloque += ": ";
                        collection.AddLast(new CollectionValue(obj, GenerarArbol(hijos[4])));
                        return collection;
                    }
                case "LIST_DISPLAY":
                    linea = hijos[0].Token.Location.Line + 1;
                    columna = hijos[0].Token.Location.Column + 1;
                    Bloque += "[";
                    obj = GenerarArbol(hijos[1]);
                    Bloque += "]";
                    return new ListDisplay((LinkedList<Expresion>)obj, linea, columna);
                case "SET_DISPLAY":
                    linea = hijos[0].Token.Location.Line + 1;
                    columna = hijos[0].Token.Location.Column + 1;
                    if (hijos.Count() == 3)
                    {
                        Bloque += "{";
                        obj = GenerarArbol(hijos[1]);
                        Bloque += "}";
                        return new SetDisplay((LinkedList<Expresion>)obj, linea, columna);
                    }
                    else
                    {
                        Bloque += "{";
                        obj = GenerarArbol(hijos[1]);
                        Bloque += "} as " + hijos[4].Token.Text;

                        return new ObjetoDisplay(hijos[4].Token.Text, (LinkedList<Expresion>)obj, linea, columna);
                    }
                case "FUNCALL":
                    linea = hijos[0].Token.Location.Line + 1;
                    columna = hijos[0].Token.Location.Column + 1;
                    if (hijos.Count() == 3)
                    {
                        Bloque += hijos[0].Token.Text + "()";
                        return new FuncionCall(hijos[0].Token.Text, linea, columna);
                    }
                    else
                    {
                        Bloque += hijos[0].Token.Text + "(";
                        obj = GenerarArbol(hijos[2]);
                        Bloque += ")";
                        return new FuncionCall(hijos[0].Token.Text, (LinkedList<Expresion>)obj, linea, columna);
                    }
                case "CALL":
                    linea = hijos[0].Token.Location.Line + 1;
                    columna = hijos[0].Token.Location.Column + 1;
                    if (hijos.Count() == 4)
                    {
                        Bloque += "call " + hijos[1].Token.Text + "()";
                        return new Call(hijos[1].Token.Text, linea, columna);
                    }
                    {
                        Bloque += "call " + hijos[1].Token.Text + "(";
                        obj = GenerarArbol(hijos[3]);
                        Bloque += ")";
                        return new Call(hijos[1].Token.Text, (LinkedList<Expresion>)obj, linea, columna);
                    }
                case "ACCESS":
                    linea = hijos[1].Token.Location.Line + 1;
                    columna = hijos[1].Token.Location.Column + 1;
                    obj = GenerarArbol(hijos[0]);
                    Bloque += "[";
                    obj2 = GenerarArbol(hijos[2]);
                    Bloque += "]";
                    return new Acceso((Expresion)obj, (Expresion)obj2, linea, columna);
            }

            return null;
        }

        private Operador GetOperador(ParseTreeNode raiz)
        {
            switch (raiz.Token.Text)
            {
                case "+":
                    return Operador.SUMA;
                case "-":
                    return Operador.RESTA;
                case "*":
                    return Operador.MULTIPLICACION;
                case "**":
                    return Operador.POTENCIA;
                case "%":
                    return Operador.MODULO;
                case "/":
                    return Operador.DIVISION;
                case "++":
                    return Operador.AUMENTO;
                case "--":
                    return Operador.DECREMENTO;
                case "<":
                    return Operador.MENORQUE;
                case ">":
                    return Operador.MAYORQUE;
                case "==":
                    return Operador.IGUAL;
                case ">=":
                    return Operador.MAYORIGUAL;
                case "<=":
                    return Operador.MENORIGUAL;
                case "!=":
                    return Operador.DIFERENTE;
            }
            return Operador.INDEFINIDO;
        }

        public Operador GetAugOperador(ParseTreeNode raiz)
        {
            switch (raiz.Token.Text)
            {
                case "+=":
                    return Operador.SUMA;
                case "-=":
                    return Operador.RESTA;
                case "*=":
                    return Operador.MULTIPLICACION;
                case "/=":
                    return Operador.DIVISION;
            }
            return Operador.INDEFINIDO;
        }
    }
}
