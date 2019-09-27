using GramaticasCQL.Parsers.LUP.ast;
using GramaticasCQL.Parsers.LUP.ast.sentencia;
using Irony.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GramaticasCQL.Parsers.LUP
{
    public class AnalizadorLUP
    {
        public ParseTree Raiz { get; set; }

        public bool AnalizarEntrada(String entrada)
        {
            GramaticaLUP gramatica = new GramaticaLUP();
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
                    return new ASTLUP((LinkedList<NodoLUP>)GenerarArbol(hijos[0]));
                case "INSTRUCCIONES":
                    LinkedList<NodoLUP> sentencias = new LinkedList<NodoLUP>();
                    foreach (ParseTreeNode hijo in hijos)
                    {
                        sentencias.AddLast((NodoLUP)GenerarArbol(hijo));
                    }
                    return sentencias;
                case "INSTRUCCION":
                    return GenerarArbol(hijos[0]);
                case "LOGIN":
                    linea = hijos[0].Token.Location.Line + 1;
                    columna = hijos[0].Token.Location.Column + 1;
                    return new Login((string)GenerarArbol(hijos[4]), (string)GenerarArbol(hijos[5]), linea, columna);
                case "USER":
                    linea = hijos[3].Token.Text.Length - 3;
                    if (linea >= 0)
                        return hijos[3].Token.Text.Substring(1, linea).ToLower().Trim();
                    return "";
                case "PASS":
                    linea = hijos[3].Token.Text.Length - 3;
                    if (linea >= 0)
                        return hijos[3].Token.Text.Substring(1, linea).ToLower().Trim();
                    return "";
                case "LOGOUT":
                    linea = hijos[0].Token.Location.Line + 1;
                    columna = hijos[0].Token.Location.Column + 1;
                    return new Logout((string)GenerarArbol(hijos[4]), linea, columna);
                case "QUERY":
                    linea = hijos[0].Token.Location.Line + 1;
                    columna = hijos[0].Token.Location.Column + 1;
                    return new Query((string)GenerarArbol(hijos[4]), (string)GenerarArbol(hijos[5]), linea, columna);
                case "DATA":
                    linea = hijos[3].Token.Text.Length - 3;
                    if (linea >= 0)
                        return hijos[3].Token.Text.Substring(1, linea).ToLower();
                    return "";
                case "STRUC":
                    linea = hijos[0].Token.Location.Line + 1;
                    columna = hijos[0].Token.Location.Column + 1;
                    return new Struc((string)GenerarArbol(hijos[4]), linea, columna);
            }

            return null;
        }
    }
}
