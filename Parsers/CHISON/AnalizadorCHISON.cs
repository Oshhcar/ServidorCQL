using GramaticasCQL.Parsers.CHISON.ast;
using GramaticasCQL.Parsers.CQL.ast.entorno;
using GramaticasCQL.Parsers.CQL.ast.expresion;
using GramaticasCQL.Parsers.CQL.ast.instruccion;
using Irony.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Type = GramaticasCQL.Parsers.CQL.ast.entorno.Type;

namespace GramaticasCQL.Parsers.CHISON
{
    class AnalizadorCHISON
    { 

        public ParseTree Raiz { get; set; }

        public bool AnalizarEntrada(String entrada)
        {
            GramaticaCHISON gramatica = new GramaticaCHISON();
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
                case "ARCHIVO":
                    if (hijos.Count() == 3)
                        return new ASTCHISON((BloqueChison)GenerarArbol(hijos[1]));
                    else
                    {
                        if (hijos[0].Term.Name.Equals("LISTA_BLOQUE"))
                            return new ASTCHISON((LinkedList<BloqueChison>)GenerarArbol(hijos[0]));
                        else
                            return GenerarArbol(hijos[0]);
                    }
                case "TYPE":
                    switch (hijos[0].Term.Name)
                    {
                        case "map":
                            return new Tipo(Type.MAP);
                        case "list":
                            return new Tipo(Type.LIST);
                        case "set":
                            return new Tipo(Type.SET);
                    }
                    return null;
                case "TYPE_PRIMITIVE":
                    switch (hijos[0].Term.Name)
                    {
                        case "int":
                            return new Tipo(Type.INT);
                        case "double":
                            return new Tipo(Type.DOUBLE);
                        case "string":
                            return new Tipo(Type.STRING);
                        case "boolean":
                            return new Tipo(Type.BOOLEAN);
                        case "date":
                            return new Tipo(Type.DATE);
                        case "time":
                            return new Tipo(Type.TIME);
                        default:
                            return null;
                    }
                case "TYPE_COLLECTION":
                    switch (hijos[0].Term.Name)
                    {
                        case "int":
                            return new Tipo(Type.INT);
                        case "double":
                            return new Tipo(Type.DOUBLE);
                        case "string":
                            return new Tipo(Type.STRING);
                        case "boolean":
                            return new Tipo(Type.BOOLEAN);
                        case "date":
                            return new Tipo(Type.DATE);
                        case "time":
                            return new Tipo(Type.TIME);
                        case "identifier":
                            return new Tipo(hijos[0].Token.Text.ToLower());
                        case "counter":
                            return new Tipo(Type.COUNTER);
                        case "map":
                            return new Tipo((Tipo)GenerarArbol(hijos[2]), (Tipo)GenerarArbol(hijos[4]));
                        case "list":
                            return new Tipo(Type.LIST, (Tipo)GenerarArbol(hijos[2]));
                        case "set":
                            return new Tipo(Type.SET, (Tipo)GenerarArbol(hijos[2]));
                        default:
                            return null;
                    }
                case "LISTA_BLOQUE":
                    LinkedList<BloqueChison> bloques = new LinkedList<BloqueChison>();
                    foreach (ParseTreeNode hijo in hijos)
                    {
                        bloques.AddLast((BloqueChison)GenerarArbol(hijo));
                    }
                    return bloques;
                case "BLOQUE":
                    linea = hijos[0].Token.Location.Line + 1;
                    columna = hijos[0].Token.Location.Column + 1;
                    return new BloqueChison((LinkedList<Instruccion>)GenerarArbol(hijos[1]), linea, columna);
                case "INSTRUCCIONES":
                    LinkedList<Instruccion> insts = new LinkedList<Instruccion>();
                    foreach (ParseTreeNode hijo in hijos)
                    {
                        insts.AddLast((Instruccion)GenerarArbol(hijo));
                    }
                    return insts;
                case "INSTRUCCION":
                    linea = hijos[1].Token.Location.Line + 1;
                    columna = hijos[1].Token.Location.Column + 1;

                    Cadena cadena = new Cadena(hijos[0].Token.Text.ToLower());

                    if (cadena.ToString().ToLower().Equals("databases"))
                    {
                        return new Databases(GenerarArbol(hijos[2]), linea, columna);
                    }
                    else if (cadena.ToString().ToLower().Equals("users"))
                    {
                        return new Users((Expresion)GenerarArbol(hijos[2]), linea, columna);

                    }
                    else
                    {
                        return new Atributo(cadena, (Expresion)GenerarArbol(hijos[2]), linea, columna);
                    }
                case "VALOR":
                    string nodo = hijos[0].Term.Name;
                    if (nodo.Equals("LISTA") || nodo.Equals("BLOQUE"))
                        return GenerarArbol(hijos[0]);
                    else
                    {
                        linea = hijos[0].Token.Location.Line + 1;
                        columna = hijos[0].Token.Location.Column + 1;

                        switch (nodo)
                        {
                            case "number":
                                try
                                {
                                    int valor2 = Convert.ToInt32(hijos[0].Token.Text);
                                    return new Literal(new Tipo(Type.INT), valor2, linea, columna);
                                }
                                catch (Exception)
                                {
                                    double valor = Convert.ToDouble(hijos[0].Token.Text);
                                    return new Literal(new Tipo(Type.DOUBLE), valor, linea, columna);
                                }
                            case "stringliteral":
                                return new Literal(new Tipo(Type.STRING), new Cadena(hijos[0].Token.Text), linea, columna);
                            case "true":
                                return new Literal(new Tipo(Type.BOOLEAN), true, linea, columna);
                            case "false":
                                return new Literal(new Tipo(Type.BOOLEAN), false, linea, columna);
                            case "date":
                                return new Literal(new Tipo(Type.DATE), new Date(hijos[0].Token.Text), linea, columna);
                            case "time":
                                return new Literal(new Tipo(Type.TIME), new Time(hijos[0].Token.Text), linea, columna);
                            case "null":
                                return new Literal(new Tipo(Type.NULL), new Null(), linea, columna);
                            case "in":
                                return new Literal(new Tipo(Type.IN), new Null(), linea, columna);
                            case "out":
                                return new Literal(new Tipo(Type.OUT), new Null(), linea, columna);
                            case "stringcodigo":
                                return new Literal(new Tipo(Type.VOID), hijos[0].Token.Text, linea, columna);
                        }
                    }
                    return null;
                case "LISTA":
                    linea = hijos[0].Token.Location.Line + 1;
                    columna = hijos[0].Token.Location.Column + 1;
                    if (hijos.Count() == 2)
                        return new Lista(null, linea, columna);
                    return new Lista((LinkedList<Expresion>)GenerarArbol(hijos[1]), linea, columna);
                case "VALORES":
                    LinkedList<Expresion> valores = new LinkedList<Expresion>();
                    foreach (ParseTreeNode hijo in hijos)
                    {
                        valores.AddLast((Expresion)GenerarArbol(hijo));
                    }
                    return valores;
            }

            return null;
        }
    }
}
