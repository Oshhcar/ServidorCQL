using Irony.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GramaticasCQL.Parsers.CQL
{
    class GramaticaCQL : Grammar
    {
        public GramaticaCQL() : base(false)
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
                type_ = ToTerm("type"),
                if_ = ToTerm("if"),
                not_ = ToTerm("not"),
                exists_ = ToTerm("exists"),
                int_ = ToTerm("int"),
                double_ = ToTerm("double"),
                string_ = ToTerm("string"),
                boolean_ = ToTerm("boolean"),
                date_ = ToTerm("date"),
                time_ = ToTerm("time"),
                use_ = ToTerm("use"),
                create_ = ToTerm("create"),
                database_ = ToTerm("database"),
                drop_ = ToTerm("drop"),
                table_ = ToTerm("table"),
                counter_ = ToTerm("counter"),
                primary_ = ToTerm("primary"),
                key_ = ToTerm("key"),
                alter_ = ToTerm("alter"),
                add_ = ToTerm("add"),
                truncate_ = ToTerm("truncate"),
                commit_ = ToTerm("commit"),
                rollback_ = ToTerm("rollback"),
                user_ = ToTerm("user"),
                with_ = ToTerm("with"),
                password_ = ToTerm("password"),
                grant_ = ToTerm("grant"),
                on_ = ToTerm("on"),
                revoke_ = ToTerm("revoke"),
                insert_ = ToTerm("insert"),
                into_ = ToTerm("into"),
                values_ = ToTerm("values"),
                update_ = ToTerm("update"),
                set_ = ToTerm("set"),
                where_ = ToTerm("where"),
                delete_ = ToTerm("delete"),
                from_ = ToTerm("from"),
                select_ = ToTerm("select"),
                order_ = ToTerm("order"),
                by_ = ToTerm("by"),
                asc_ = ToTerm("asc"),
                desc_ = ToTerm("desc"),
                limit_ = ToTerm("limit"),
                begin_ = ToTerm("begin"),
                batch_ = ToTerm("batch"),
                apply_ = ToTerm("apply"),
                count_ = ToTerm("count"),
                min_ = ToTerm("min"),
                max_ = ToTerm("max"),
                sum_ = ToTerm("sum"),
                avg_ = ToTerm("avg"),
                in_ = ToTerm("in"),
                else_ = ToTerm("else"),
                switch_ = ToTerm("switch"),
                case_ = ToTerm("case"),
                default_ = ToTerm("default"),
                while_ = ToTerm("while"),
                do_ = ToTerm("do"),
                for_ = ToTerm("for"),
                new_ = ToTerm("new"),
                map_ = ToTerm("map"),
                list_ = ToTerm("list"),
                procedure_ = ToTerm("procedure"),
                call_ = ToTerm("call"),
                break_ = ToTerm("break"),
                continue_ = ToTerm("continue"),
                return_ = ToTerm("return"),
                cursor_ = ToTerm("cursor"),
                is_ = ToTerm("is"),
                each_ = ToTerm("each"),
                open_ = ToTerm("open"),
                close_ = ToTerm("close"),
                log_ = ToTerm("log"),
                throw_ = ToTerm("throw"),
                try_ = ToTerm("try"),
                catch_ = ToTerm("catch"),
                as_ = ToTerm("as");

            MarkReservedWords("null", "true", "false", "type", "if", "not", "exists", "int", "double", "string", "boolean",
                "date", "time", "use", "create", "database", "drop", "table", "counter", "primary", "key", "alter", "add",
                "truncate", "commit", "rollback", "user", "with", "password", "grant", "on", "revoke", "insert", "into",
                "values", "update", "set", "where", "delete", "from", "select", "order", "by", "asc", "desc", "limit",
                "begin", "batch", "apply", "count", "min", "max", "sum", "avg", "in", "else", "switch", "case", "default",
                "while", "do", "for", "new", "map", "list", "procedure", "call", "break", "continue", "return", "cursor",
                "is", "each", "open", "close", "log", "throw", "try", "catch", "as");

            /* Relational operators */
            KeyTerm
                menorigual = ToTerm("<="),
                mayorigual = ToTerm(">="),
                menorque = ToTerm("<"),
                mayorque = ToTerm(">"),
                igual = ToTerm("=="),
                diferente = ToTerm("!=");

            /* Logic operators */
            KeyTerm
                or = ToTerm("||"),
                and = ToTerm("&&"),
                xor = ToTerm("^"),
                not = ToTerm("!");

            /* Shift operators */
            KeyTerm
                leftShift = ToTerm("--"),
                rightShift = ToTerm("++");

            /* Arithmetic Operators*/
            KeyTerm
                mas = ToTerm("+"),
                menos = ToTerm("-"),
                por = ToTerm("*"),
                division = ToTerm("/"),
                modulo = ToTerm("%"),
                potencia = ToTerm("**");

            /* Symbols*/
            KeyTerm
                equal = ToTerm("="),
                semicolon = ToTerm(";"),
                leftPar = ToTerm("("),
                rightPar = ToTerm(")"),
                dot = ToTerm("."),
                comma = ToTerm(","),
                questionmark = ToTerm("?"),
                colon = ToTerm(":"),
                leftCor = ToTerm("["),
                rightCor = ToTerm("]"),
                leftLla = ToTerm("{"),
                rightLla = ToTerm("}"),
                masEqual = ToTerm("+="),
                menosEqual = ToTerm("-="),
                porEqual = ToTerm("*="),
                divisionEqual = ToTerm("/="),
                menormenor = ToTerm("<<"),
                mayormayor = ToTerm(">>");

            MarkPunctuation(";");

            var number = new NumberLiteral("number");
            //var identifier = new IdentifierTerminal("identifier");
            RegexBasedTerminal identifier = new RegexBasedTerminal("identifier", "([a-zA-ZñÑ]|_)([a-zA-ZñÑ]|[0-9]|_)*");
            RegexBasedTerminal identifier2 = new RegexBasedTerminal("identifier2", "@([a-zA-ZñÑ]|_)([a-zA-ZñÑ]|[0-9]|_)*");
            //RegexBasedTerminal date = new RegexBasedTerminal("date", "\'([0-2][0-9]{3}|[0-9][0-9]{0,2})-([0]?[0-9]|[1][0-2])-([0]?[0-9]|[1-2][0-9]|[3][0-1])\'");
            //RegexBasedTerminal time = new RegexBasedTerminal("time", "\'([0]?[0-9]|[1][0-9]|[2][0-4]):([0]?[0-9]|[1-5][0-9]):([0]?[0-9]|[1-5][0-9])\'");
            RegexBasedTerminal date = new RegexBasedTerminal("date", "\'[0-9]+-[0-9]+-[0-9]+\'");
            RegexBasedTerminal time = new RegexBasedTerminal("time", "\'[0-9]+:[0-9]+:[0-9]+\'");
            var stringliteral = new StringLiteral("stringliteral", "\"", StringOptions.IsTemplate);

            NonTerminal
                INICIO = new NonTerminal("INICIO"),
                INSTRUCCIONES = new NonTerminal("INSTRUCCIONES"),
                INSTRUCCION = new NonTerminal("INSTRUCCION"),

                TYPE = new NonTerminal("TYPE"),
                TYPE_PRIMITIVE = new NonTerminal("TYPE_PRIMITIVE"),
                TYPE_COLLECTION = new NonTerminal("TYPE_COLLECTION"),
                TYPEDEF = new NonTerminal("TYPEDEF"),
                IFNOTEXIST = new NonTerminal("IFNOTEXIST"),
                IFEXIST = new NonTerminal("IFEXIST"),
                ATTRIBUTE_LIST = new NonTerminal("ATTRIBUTE_LIST"),
                ATTRIBUTEREF = new NonTerminal("ATTRIBUTEREF"),
                ATTRIBUTE = new NonTerminal("ATTRIBUTE"),

                USE = new NonTerminal("USE"),
                DATABASEDEF = new NonTerminal("DATABASEDEF"),
                DROP = new NonTerminal("DROP"),
                TABLEDEF = new NonTerminal("TABLEDEF"),
                COLUMN_LIST = new NonTerminal("COLUMN_LIST"),
                COLUMN = new NonTerminal("COLUMN"),
                ID_LIST = new NonTerminal("ID_LIST"),
                TABLEALTER = new NonTerminal("TABLEALTER"),
                TABLEDROP = new NonTerminal("TABLEDROP"),
                TABLETRUNCATE = new NonTerminal("TABLETRUNCATE"),

                COMMIT = new NonTerminal("COMMIT"),
                ROLLBACK = new NonTerminal("ROLLBACK"),

                USERDEF = new NonTerminal("USERDEF"),
                GRANT = new NonTerminal("GRANT"),
                REVOKE = new NonTerminal("REVOKE"),

                WHERE = new NonTerminal("WHERE"),
                INSERT = new NonTerminal("INSERT"),
                UPDATE = new NonTerminal("UPDATE"),
                DELETE = new NonTerminal("DELETE"),
                SELECT = new NonTerminal("SELECT"),
                SELECT_EXP = new NonTerminal("SELECT_EXP"),
                ORDER_LIST = new NonTerminal("ORDER_LIST"),
                ORDER = new NonTerminal("ORDER"),
                BATCH = new NonTerminal("BATCH"),
                DML_LIST = new NonTerminal("DML_LIST"),
                DML = new NonTerminal("DML"),

                TARGET_LIST = new NonTerminal("TARGET_LIST"),
                TARGET = new NonTerminal("TARGET"),

                EXPRESSION_STMT = new NonTerminal("EXPRESSION_STMT"),
                ASSIGNMENT_STMT = new NonTerminal("ASSIGNMENT_STMT"),
                ASSIGNMENTS = new NonTerminal("ASSIGNMENTS"),
                ASSIGNMENT_CALL = new NonTerminal("ASSIGNMENT_CALL"),
                ASSIGNMENT_LIST = new NonTerminal("ASSIGNMENT_LIST"),

                DECLARATION_STMT = new NonTerminal("DECLARATION_STMT"),
                AUGMENTED_ASSIGNMENT_STMT = new NonTerminal("AUGMENTED_ASSIGNMENT_STMT"),
                //AUGTARGET = new NonTerminal("AUGTARGET"),
                AUG_OPERATOR = new NonTerminal("AUG_OPERATOR"),

                IF_STMT = new NonTerminal("IF_STMT"),
                IF_LIST = new NonTerminal("IF_LIST"),
                SWITCH_STMT = new NonTerminal("SWITCH_STMT"),
                CASES = new NonTerminal("CASES"),
                WHILE_STMT = new NonTerminal("WHILE_STMT"),
                DOWHILE_STMT = new NonTerminal("DOWHILE_STMT"),
                FOR_STMT = new NonTerminal("FOR_STMT"),
                FOR_INIT = new NonTerminal("FOR_INIT"),
                FOR_UPDATE = new NonTerminal("FOR_UPDATE"),

                FUNDEF = new NonTerminal("FUNDEF"),
                PARAMETER_LIST = new NonTerminal("PARAMETER_LIST"),

                PROCDEF = new NonTerminal("PROCDEF"),

                BREAK_STMT = new NonTerminal("BREAK_STMT"),
                CONTINUE_STMT = new NonTerminal("CONTINUE_STMT"),
                RETURN_STMT = new NonTerminal("RETURN_STMT"),

                CURSOR_STMT = new NonTerminal("CURSOR_STMT"),
                FOREACH_STMT = new NonTerminal("FOREACH_STMT"),
                OPEN_STMT = new NonTerminal("OPEN_STMT"),
                CLOSE_STMT = new NonTerminal("CLOSE_STMT"),
                LOG_STMT = new NonTerminal("LOG_STMT"),
                THROW_STMT = new NonTerminal("THROW_STMT"),
                TRYCATCH_STMT = new NonTerminal("TRYCATCH_STMT"),

                BLOQUE = new NonTerminal("BLOQUE"),
                SENTENCIAS = new NonTerminal("SENTENCIAS"),
                SENTENCIA = new NonTerminal("SENTENCIA"),

                /*STARRED_EXPRESSION = new NonTerminal("STARRED_EXPRESSION"),
                STARRED_LIST = new NonTerminal("STARRED_LIST"),
                STARRED_ITEM = new NonTerminal("STARRED_ITEM"),*/

                AGGREGATION = new NonTerminal("AGGREGATION"),
                AGGREGATION_FUN = new NonTerminal("AGGREGATION_FUN"),

                EXPRESSION_LIST = new NonTerminal("EXPRESSION_LIST"),
                EXPRESSION = new NonTerminal("EXPRESSION"),
                CONDITIONAL_EXPRESSION = new NonTerminal("CONDITIONAL_EXPRESSION"),
                INSTANCE = new NonTerminal("INSTANCE"),
                OR_EXPR = new NonTerminal("OR_EXPR"),
                AND_EXPR = new NonTerminal("AND_EXPR"),
                XOR_EXPR = new NonTerminal("XOR_EXPR"),
                NOT_EXPR = new NonTerminal("NOT_EXPR"),
                COMPARISON = new NonTerminal("COMPARISON"),
                COMPARISON_EQ = new NonTerminal("COMPARISON_EQ"),
                COMP_OPERATOR = new NonTerminal("COMP_OPERATOR"),
                SHIFT_EXPR = new NonTerminal("SHIFT_EXPR"),
                A_EXPR = new NonTerminal("A_EXPR"),
                M_EXPR = new NonTerminal("M_EXPR"),
                U_EXPR = new NonTerminal("U_EXPR"),
                POWER = new NonTerminal("POWER"),
                PRIMARY = new NonTerminal("PRIMARY"),
                ATOM = new NonTerminal("ATOM"),
                FUNCALL = new NonTerminal("FUNCALL"),
                CALL = new NonTerminal("CALL"),
                ACCESS = new NonTerminal("ACCESS"),
                ENCLOSURE = new NonTerminal("ENCLOSURE"),
                LITERAL = new NonTerminal("LITERAL"),
                PARENTH_FORM = new NonTerminal("PARENTH_FORM"),
                MAP_DISPLAY = new NonTerminal("MAP_DISPLAY"),
                MAP_LIST = new NonTerminal("MAP_LIST"),
                LIST_DISPLAY = new NonTerminal("LIST_DISPLAY"),
                SET_DISPLAY = new NonTerminal("SET_DISPLAY");
                
            this.Root = INICIO;

            INICIO.Rule = INSTRUCCIONES;

            INSTRUCCIONES.Rule = MakePlusRule(INSTRUCCIONES, INSTRUCCION);

            INSTRUCCION.Rule = TYPEDEF + semicolon
                              | USE + semicolon
                              | DATABASEDEF + semicolon
                              | DROP + semicolon
                              | TABLEDEF + semicolon
                              | TABLEALTER + semicolon
                              | TABLEDROP + semicolon
                              | TABLETRUNCATE + semicolon
                              | COMMIT + semicolon
                              | ROLLBACK + semicolon
                              | USERDEF + semicolon
                              | GRANT + semicolon
                              | REVOKE + semicolon
                              | INSERT + semicolon
                              | UPDATE + semicolon
                              | DELETE + semicolon
                              | SELECT + semicolon
                              | BATCH + semicolon

                              | EXPRESSION_STMT + semicolon
                              | DECLARATION_STMT + semicolon
                              | ASSIGNMENT_STMT + semicolon
                              | ASSIGNMENT_CALL + semicolon
                              | AUGMENTED_ASSIGNMENT_STMT + semicolon
                              | IF_STMT
                              | SWITCH_STMT
                              | WHILE_STMT
                              | DOWHILE_STMT + semicolon
                              | FOR_STMT

                              | FUNDEF
                              | PROCDEF

                              | BREAK_STMT + semicolon
                              | CONTINUE_STMT + semicolon
                              | RETURN_STMT + semicolon

                              | CURSOR_STMT + semicolon
                              | FOREACH_STMT
                              | OPEN_STMT + semicolon
                              | CLOSE_STMT + semicolon
                              | LOG_STMT + semicolon
                              | THROW_STMT + semicolon
                              | TRYCATCH_STMT;

            //INSTRUCCION.ErrorRule = SyntaxError + semicolon;


            TYPE.Rule = int_ | double_ | string_ | boolean_ | date_ | time_ | identifier | counter_ | map_ | list_ | set_;

            TYPE_PRIMITIVE.Rule = int_ | double_ | string_ | boolean_ | date_ | time_ ;

            TYPE_COLLECTION.Rule = int_ | double_ | string_ | boolean_ | date_ | time_ | identifier | counter_
                                    | map_ + menorque + TYPE_PRIMITIVE + comma + TYPE_COLLECTION + mayorque
                                    | list_ + menorque + TYPE_COLLECTION + mayorque
                                    | set_ + menorque + TYPE_COLLECTION + mayorque;

            IFNOTEXIST.Rule = if_ + not_ + exists_;

            IFEXIST.Rule = if_ + exists_;

            TYPEDEF.Rule = create_ + type_ + identifier + leftPar + ATTRIBUTE_LIST + rightPar
                         | create_ + type_ + IFNOTEXIST + identifier + leftPar + ATTRIBUTE_LIST + rightPar;

            ATTRIBUTE_LIST.Rule = MakePlusRule(ATTRIBUTE_LIST, comma, ATTRIBUTE);

            ATTRIBUTE.Rule = identifier + TYPE_COLLECTION;

            USE.Rule = use_ + identifier;

            DATABASEDEF.Rule = create_ + database_ + identifier
                            | create_ + database_ + IFNOTEXIST + identifier;

            DROP.Rule = drop_ + database_ + identifier
                        | drop_ + database_ + IFNOTEXIST + identifier;

            TABLEDEF.Rule = create_ + table_ + identifier + leftPar + COLUMN_LIST + rightPar
                           | create_ + table_ + IFNOTEXIST + identifier + leftPar + COLUMN_LIST + rightPar;

            COLUMN_LIST.Rule = MakePlusRule(COLUMN_LIST, comma, COLUMN);

            COLUMN.Rule = identifier + TYPE_COLLECTION
                        | identifier + TYPE_COLLECTION + primary_ + key_
                        | primary_ + key_ + leftPar + ID_LIST + rightPar;

            ID_LIST.Rule = MakePlusRule(ID_LIST, comma, identifier);

            TABLEALTER.Rule = alter_ + table_ + identifier + add_ + ATTRIBUTE_LIST
                            | alter_ + table_ + identifier + drop_ + ID_LIST;

            TABLEDROP.Rule = drop_ + table_ + identifier
                            | drop_ + table_ + IFEXIST + identifier;

            TABLETRUNCATE.Rule = truncate_ + table_ + identifier;


            COMMIT.Rule = commit_;

            ROLLBACK.Rule = rollback_;


            USERDEF.Rule = create_ + user_ + identifier + with_ + password_ + stringliteral;

            GRANT.Rule = grant_ + identifier + on_ + identifier;

            REVOKE.Rule = revoke_ + identifier + on_ + identifier;

            WHERE.Rule = where_ + EXPRESSION
                        | where_ + EXPRESSION + in_ + EXPRESSION_LIST
                        | where_ + EXPRESSION + in_ + leftPar + EXPRESSION_LIST + rightPar;

            INSERT.Rule = insert_ + into_ + identifier + values_ + leftPar + EXPRESSION_LIST + rightPar
                        | insert_ + into_ + identifier + leftPar + ID_LIST + rightPar + values_ + leftPar + EXPRESSION_LIST + rightPar;

            UPDATE.Rule = update_ + identifier + set_ + ASSIGNMENT_LIST
                         | update_ + identifier + set_ + ASSIGNMENT_LIST + WHERE;

            DELETE.Rule = delete_ + from_ + identifier
                        | delete_ + from_ + identifier + WHERE
                        | delete_ + TARGET + from_ + identifier
                        | delete_ + TARGET + from_ + identifier + WHERE;

            SELECT.Rule = select_ + SELECT_EXP + from_ + identifier
                         | select_ + SELECT_EXP + from_ + identifier + WHERE
                         | select_ + SELECT_EXP + from_ + identifier + order_ + by_ + ORDER_LIST
                         | select_ + SELECT_EXP + from_ + identifier + WHERE + order_ + by_ + ORDER_LIST
                         | select_ + SELECT_EXP + from_ + identifier + limit_ + EXPRESSION
                         | select_ + SELECT_EXP + from_ + identifier + WHERE + limit_ + EXPRESSION
                         | select_ + SELECT_EXP + from_ + identifier + order_ + by_ + ORDER_LIST + limit_ + EXPRESSION
                         | select_ + SELECT_EXP + from_ + identifier + WHERE + order_ + by_ + ORDER_LIST + limit_ + EXPRESSION;

            SELECT_EXP.Rule = EXPRESSION_LIST | por;

            ORDER_LIST.Rule = MakePlusRule(ORDER_LIST, comma, ORDER);

            ORDER.Rule = identifier
                        | identifier + asc_
                        | identifier + desc_; //TARGET*

            BATCH.Rule = begin_ + batch_ + DML_LIST + apply_ + batch_;

            DML_LIST.Rule = MakePlusRule(DML_LIST, DML);

            DML.Rule = INSERT + semicolon
                     | UPDATE + semicolon
                     | DELETE + semicolon;

            ////////////////////////////////////////////////////////////////

            BLOQUE.Rule = leftLla + SENTENCIAS + rightLla
                        | leftLla + rightLla;

            SENTENCIAS.Rule = MakePlusRule(SENTENCIAS, SENTENCIA);

            SENTENCIA.Rule =    TYPEDEF + semicolon /****/
                              |  USE + semicolon
                              | DATABASEDEF + semicolon
                              | DROP + semicolon
                              | TABLEDEF + semicolon
                              | TABLEALTER + semicolon
                              | TABLEDROP + semicolon
                              | TABLETRUNCATE + semicolon
                              | COMMIT + semicolon
                              | ROLLBACK + semicolon
                              | USERDEF + semicolon
                              | GRANT + semicolon
                              | REVOKE + semicolon
                              | INSERT + semicolon
                              | UPDATE + semicolon
                              | DELETE + semicolon
                              | SELECT + semicolon
                              | BATCH + semicolon

                              | EXPRESSION_STMT + semicolon
                              | DECLARATION_STMT + semicolon
                              | ASSIGNMENT_STMT + semicolon
                              | ASSIGNMENT_CALL + semicolon
                              | AUGMENTED_ASSIGNMENT_STMT + semicolon
                              | IF_STMT
                              | SWITCH_STMT
                              | WHILE_STMT
                              | DOWHILE_STMT + semicolon
                              | FOR_STMT

                              | BREAK_STMT + semicolon
                              | CONTINUE_STMT + semicolon
                              | RETURN_STMT + semicolon
                              
                              | CURSOR_STMT + semicolon
                              | FOREACH_STMT
                              | OPEN_STMT + semicolon
                              | CLOSE_STMT + semicolon
                              | LOG_STMT + semicolon
                              | THROW_STMT + semicolon
                              | TRYCATCH_STMT;

            TARGET_LIST.Rule = MakePlusRule(TARGET_LIST, comma, TARGET);

            TARGET.Rule = identifier //*
                        | identifier2
                        | ATTRIBUTEREF //*
                        | ACCESS; 

            EXPRESSION_STMT.Rule = SHIFT_EXPR + leftShift | SHIFT_EXPR + rightShift | FUNCALL | CALL | ATTRIBUTEREF;

            DECLARATION_STMT.Rule = TYPE + TARGET_LIST
                                   | TYPE + TARGET_LIST + equal + EXPRESSION;

            ASSIGNMENT_STMT.Rule = TARGET + equal + EXPRESSION
                                  | TARGET + equal + CALL;

            ASSIGNMENT_CALL.Rule = TARGET_LIST + equal + CALL;

            ASSIGNMENT_LIST.Rule = MakePlusRule(ASSIGNMENT_LIST, comma, ASSIGNMENTS);

            ASSIGNMENTS.Rule = ASSIGNMENT_STMT | AUGMENTED_ASSIGNMENT_STMT;

            AUGMENTED_ASSIGNMENT_STMT.Rule = TARGET + AUG_OPERATOR + EXPRESSION;

            //AUGTARGET.Rule = identifier | identifier2 | ATTRIBUTEREF; //*

            AUG_OPERATOR.Rule = masEqual | menosEqual | porEqual | divisionEqual;

            IF_STMT.Rule = IF_LIST + else_ + BLOQUE
                        | IF_LIST;
             
            IF_LIST.Rule = IF_LIST + else_ + if_ + leftPar + EXPRESSION + rightPar + BLOQUE
                          | if_ + leftPar + EXPRESSION + rightPar + BLOQUE;

            SWITCH_STMT.Rule = switch_ + leftPar + EXPRESSION + rightPar + leftLla + CASES + rightLla
                             | switch_ + leftPar + EXPRESSION + rightPar + leftLla + CASES + default_ + colon + BLOQUE + rightLla;

            CASES.Rule = CASES + case_ + EXPRESSION + colon + BLOQUE
                        | case_ + EXPRESSION + colon + BLOQUE;

            WHILE_STMT.Rule = while_ + leftPar + EXPRESSION + rightPar + BLOQUE;

            DOWHILE_STMT.Rule = do_ + BLOQUE + while_ + leftPar + EXPRESSION + rightPar;

            FOR_STMT.Rule = for_ + leftPar + FOR_INIT + semicolon + EXPRESSION + semicolon + FOR_UPDATE + rightPar + BLOQUE;

            FOR_INIT.Rule = DECLARATION_STMT | ASSIGNMENT_STMT;

            FOR_UPDATE.Rule = AUGMENTED_ASSIGNMENT_STMT | ASSIGNMENT_STMT | SHIFT_EXPR + leftShift | SHIFT_EXPR + rightShift;

            FUNDEF.Rule = TYPE + identifier + leftPar + PARAMETER_LIST + rightPar + BLOQUE
                         | TYPE + identifier + leftPar + rightPar + BLOQUE;

            PARAMETER_LIST.Rule = PARAMETER_LIST + comma + TYPE + identifier2
                                | TYPE + identifier2;

            PROCDEF.Rule = procedure_ + identifier + leftPar + PARAMETER_LIST + rightPar + comma + leftPar + PARAMETER_LIST + rightPar + BLOQUE
                          | procedure_ + identifier + leftPar + rightPar + comma + leftPar + rightPar + BLOQUE
                          | procedure_ + identifier + leftPar + PARAMETER_LIST + rightPar + comma + leftPar + rightPar + BLOQUE
                          | procedure_ + identifier + leftPar + rightPar + comma + leftPar + PARAMETER_LIST + rightPar + BLOQUE;

            BREAK_STMT.Rule = break_;

            CONTINUE_STMT.Rule = continue_;

            RETURN_STMT.Rule = return_
                              | return_ + EXPRESSION_LIST;

            CURSOR_STMT.Rule = cursor_ + identifier2 + is_ + SELECT;

            FOREACH_STMT.Rule = for_ + each_ + leftPar + PARAMETER_LIST + rightPar + in_ + identifier2 + BLOQUE
                               |for_ + each_ + leftPar + rightPar + in_ + identifier2 + BLOQUE;

            OPEN_STMT.Rule = open_ + identifier2;

            CLOSE_STMT.Rule = close_ + identifier2;

            LOG_STMT.Rule = log_ + leftPar + EXPRESSION + rightPar;

            THROW_STMT.Rule = throw_ + new_ + identifier;

            TRYCATCH_STMT.Rule = try_ + BLOQUE + catch_ + leftPar + PARAMETER_LIST + rightPar + BLOQUE
                                | try_ + BLOQUE + catch_ + leftPar + rightPar + BLOQUE; ;

            EXPRESSION_LIST.Rule = MakePlusRule(EXPRESSION_LIST, comma, EXPRESSION);

            EXPRESSION.Rule = CONDITIONAL_EXPRESSION | INSTANCE;

            INSTANCE.Rule = new_ + identifier
                            | new_ + map_ + menorque + TYPE_PRIMITIVE + comma + TYPE_COLLECTION + mayorque
                            | new_ + list_ + menorque + TYPE_COLLECTION + mayorque
                            | new_ + set_ + menorque + TYPE_COLLECTION + mayorque;

            CONDITIONAL_EXPRESSION.Rule = OR_EXPR | OR_EXPR + questionmark + EXPRESSION + colon + EXPRESSION;

            OR_EXPR.Rule = AND_EXPR | OR_EXPR + or + AND_EXPR;

            AND_EXPR.Rule = XOR_EXPR | AND_EXPR + and + XOR_EXPR;

            XOR_EXPR.Rule = COMPARISON_EQ | XOR_EXPR + xor + COMPARISON_EQ;

            COMPARISON_EQ.Rule = COMPARISON | COMPARISON_EQ + igual + COMPARISON
                                            | COMPARISON_EQ + diferente + COMPARISON;

            COMPARISON.Rule = A_EXPR | COMPARISON + COMP_OPERATOR + A_EXPR;

            COMP_OPERATOR.Rule = menorque | mayorque | mayorigual | menorigual;

            A_EXPR.Rule = M_EXPR | A_EXPR + mas + A_EXPR | A_EXPR + menos + M_EXPR;

            M_EXPR.Rule = U_EXPR | M_EXPR + por + U_EXPR | M_EXPR + division + U_EXPR
                        | M_EXPR + modulo + U_EXPR;

            U_EXPR.Rule = NOT_EXPR | POWER | menos + U_EXPR | mas + U_EXPR;

            NOT_EXPR.Rule = not + U_EXPR;

            POWER.Rule = SHIFT_EXPR | SHIFT_EXPR + potencia + U_EXPR;

            SHIFT_EXPR.Rule = PRIMARY | SHIFT_EXPR + leftShift | SHIFT_EXPR + rightShift;

            PRIMARY.Rule = ATOM | ATTRIBUTEREF | AGGREGATION | FUNCALL | ACCESS; 

            ATOM.Rule = identifier | identifier2 | LITERAL | ENCLOSURE;

            LITERAL.Rule = number | stringliteral | true_ | false_ | date | time | null_;

            ATTRIBUTEREF.Rule = PRIMARY + dot + identifier
                                | PRIMARY + dot + FUNCALL;

            AGGREGATION.Rule = AGGREGATION_FUN + leftPar + menormenor + SELECT + mayormayor + rightPar;

            AGGREGATION_FUN.Rule = count_ | min_ | max_ | sum_ | avg_;

            ENCLOSURE.Rule = PARENTH_FORM | MAP_DISPLAY | LIST_DISPLAY | SET_DISPLAY;

            PARENTH_FORM.Rule = leftPar + EXPRESSION + rightPar
                               | leftPar + TYPE + rightPar + EXPRESSION;

            MAP_DISPLAY.Rule = leftCor + MAP_LIST + rightCor;

            MAP_LIST.Rule = MAP_LIST + comma + EXPRESSION + colon + EXPRESSION
                           | EXPRESSION + colon + EXPRESSION;

            LIST_DISPLAY.Rule = leftCor + EXPRESSION_LIST + rightCor;

            SET_DISPLAY.Rule = leftLla + EXPRESSION_LIST + rightLla
                             | leftLla + EXPRESSION_LIST + rightLla + as_ + identifier; 

            FUNCALL.Rule = identifier + leftPar + rightPar
                       | identifier + leftPar + EXPRESSION_LIST + rightPar;

            CALL.Rule = call_ + identifier + leftPar + rightPar
                       | call_ + identifier + leftPar + EXPRESSION_LIST + rightPar;

            ACCESS.Rule = PRIMARY + leftCor + EXPRESSION + rightCor;

        }
    }
}
