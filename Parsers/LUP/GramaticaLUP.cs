using Irony.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GramaticasCQL.Parsers.LUP
{
    class GramaticaLUP : Grammar
    {
        public GramaticaLUP() : base(false)
        {
            CommentTerminal blockComment = new CommentTerminal("block-comment", "/*", "*/");
            CommentTerminal lineComment = new CommentTerminal("line-comment", "//",
                "\r", "\n", "\u2085", "\u2028", "\u2029");

            NonGrammarTerminals.Add(blockComment);
            NonGrammarTerminals.Add(lineComment);

            /* Reserved Words */
            KeyTerm
                login_ = ToTerm("login"),
                user_ = ToTerm("user"),
                pass_ = ToTerm("pass"),
                logout_ = ToTerm("logout"),
                query_ = ToTerm("query"),
                data_ = ToTerm("data"),
                struc_ = ToTerm("struc");

            MarkReservedWords("login", "user", "pass", "logout", "query", "data", "struc");

            /* Symbols*/
            KeyTerm
                leftCor = ToTerm("["),
                rightCor = ToTerm("]"),
                mas = ToTerm("+"),
                menos = ToTerm("-");

            //MarkPunctuation("[", "]");

            CommentTerminal phrase = new CommentTerminal("phrase", "]", "[-");

            NonTerminal
                INICIO = new NonTerminal("INICIO"),
                INSTRUCCIONES = new NonTerminal("INSTRUCCIONES"),
                INSTRUCCION = new NonTerminal("INSTRUCCION"),

                LOGIN = new NonTerminal("LOGIN"),
                USER = new NonTerminal("USER"),
                PASS = new NonTerminal("PASS"),
                LOGOUT = new NonTerminal("LOGOUT"),
                QUERY = new NonTerminal("QUERY"),
                DATA = new NonTerminal("DATA"),
                STRUC = new NonTerminal("STRUC");

            this.Root = INICIO;

            INICIO.Rule = INSTRUCCIONES;

            INSTRUCCIONES.Rule = MakePlusRule(INSTRUCCIONES, INSTRUCCION);

            INSTRUCCION.Rule = LOGIN | LOGOUT | QUERY | STRUC;

            LOGIN.Rule = leftCor + mas + login_ + rightCor +
                                USER + PASS +
                            leftCor + menos + login_ + rightCor;

            USER.Rule = leftCor + mas + user_ +
                             phrase +
                             user_ + rightCor;

            PASS.Rule = leftCor + mas + pass_ +
                            phrase +
                            pass_ + rightCor;

            LOGOUT.Rule = leftCor + mas + logout_ + rightCor +
                                USER +
                            leftCor + menos + logout_ + rightCor;

            QUERY.Rule = leftCor + mas + query_ + rightCor +
                                USER + DATA +
                            leftCor + menos + query_ + rightCor;

            DATA.Rule = leftCor + mas + data_ +
                            phrase +
                            data_ + rightCor;

            STRUC.Rule = leftCor + mas + struc_ + rightCor +
                                USER +
                            leftCor + menos + struc_ + rightCor;

        }
    }
}
