using Irony.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GramaticasCQL.Parsers.CHISON
{
    class GramaticaCHISON : Grammar
    {
        public GramaticaCHISON() : base(false)
        {
            CommentTerminal blockComment = new CommentTerminal("block-comment", "/*", "*/");
            CommentTerminal lineComment = new CommentTerminal("line-comment", "//",
                "\r", "\n", "\u2085", "\u2028", "\u2029");

            NonGrammarTerminals.Add(blockComment);
            NonGrammarTerminals.Add(lineComment);

            /* Reserved Words */
            KeyTerm
                null_ = ToTerm("null"),
                true_ = ToTerm("true"),
                false_ = ToTerm("false"),
                in_ = ToTerm("in"),
                out_ = ToTerm("out"),
                int_ = ToTerm("int"),
                double_ = ToTerm("double"),
                string_ = ToTerm("string"),
                boolean_ = ToTerm("boolean"),
                date_ = ToTerm("date"),
                time_ = ToTerm("time"),
                counter_ = ToTerm("counter"),
                map_ = ToTerm("map"),
                list_ = ToTerm("list"),
                set_ = ToTerm("set");

            MarkReservedWords("null", "true", "false", "in", "out", "int", "double", "string", "boolean", "date", "time","counter", "map", "list", "set");

            /* Symbols*/
            KeyTerm
                equal = ToTerm("="),
                menorque = ToTerm("<"),
                mayorque = ToTerm(">"),
                leftCor = ToTerm("["),
                rightCor = ToTerm("]"),
                dollar = ToTerm("$"),
                comma = ToTerm(","),
                comilla = ToTerm("\"");

            var number = new NumberLiteral("number");
            var stringliteral = new StringLiteral("stringliteral", "\"", StringOptions.IsTemplate);
            var stringcodigo = new StringLiteral("stringcodigo", "$", StringOptions.AllowsLineBreak);
            RegexBasedTerminal date = new RegexBasedTerminal("date", "\'[0-9]+-[0-9]+-[0-9]+\'");
            RegexBasedTerminal time = new RegexBasedTerminal("time", "\'[0-9]+:[0-9]+:[0-9]+\'");
            RegexBasedTerminal identifier = new RegexBasedTerminal("identifier", "([a-zA-ZñÑ]|_)([a-zA-ZñÑ]|[0-9]|_)*");
            //RegexBasedTerminal date = new RegexBasedTerminal("date", "'[0-2][0-9]{3}-([0][0-9]|[1][0-2])-([0][0-9]|[1][0-9]|[2][0-9]|[3][0-1])'");
            //RegexBasedTerminal time = new RegexBasedTerminal("time", "'([0][0-9]|[1][0-9]|[2][0-4]):[0-5][0-9]:[0-5][0-9]'");
            //IdentifierTerminal fileName = new IdentifierTerminal("fileName", "!@#$%^*_'.?-", "!@#$%^*_'.?0123456789");


            NonTerminal
                INICIO = new NonTerminal("INICIO"),
                ARCHIVO = new NonTerminal("ARCHIVO"),
                TYPE = new NonTerminal("TYPE"),
                TYPE_PRIMITIVE = new NonTerminal("TYPE_PRIMITIVE"),
                TYPE_COLLECTION = new NonTerminal("TYPE_COLLECTION"),
                INSTRUCCIONES = new NonTerminal("INSTRUCCIONES"),
                INSTRUCCION = new NonTerminal("INSTRUCCION"),
                BLOQUE = new NonTerminal("BLOQUE"),
                LISTA_BLOQUE = new NonTerminal("LISTA_BLOQUE"),
                VALOR = new NonTerminal("VALOR"),
                LISTA = new NonTerminal("LISTA"),
                VALORES = new NonTerminal("VALORES");

            this.Root = INICIO;

            INICIO.Rule = ARCHIVO;

            ARCHIVO.Rule = dollar + BLOQUE + dollar
                          | LISTA_BLOQUE
                          | TYPE
                          | TYPE_COLLECTION;

            TYPE.Rule = map_ | list_ | set_;

            TYPE_PRIMITIVE.Rule = int_ | double_ | string_ | boolean_ | date_ | time_;

            TYPE_COLLECTION.Rule = int_ | double_ | string_ | boolean_ | date_ | time_ | identifier | counter_
                                    | map_ + menorque + TYPE_PRIMITIVE + comma + TYPE_COLLECTION + mayorque
                                    | list_ + menorque + TYPE_COLLECTION + mayorque
                                    | set_ + menorque + TYPE_COLLECTION + mayorque;

            LISTA_BLOQUE.Rule = MakePlusRule(LISTA_BLOQUE, comma, BLOQUE);

            BLOQUE.Rule = menorque + INSTRUCCIONES + mayorque;

            INSTRUCCIONES.Rule = MakePlusRule(INSTRUCCIONES, comma, INSTRUCCION);

            INSTRUCCION.Rule = stringliteral + equal + VALOR;

            VALOR.Rule = number | stringliteral | true_ | false_ | date | time | in_ | out_ | null_
                        | LISTA | BLOQUE | stringcodigo;

            LISTA.Rule = leftCor + VALORES + rightCor
                        | leftCor + rightCor;

            VALORES.Rule = MakePlusRule(VALORES, comma, VALOR);

        }
    }
}
