using System;
using System.Collections.Generic;
using System.Text;

namespace KyleBushCompiler
{
    public static class Utilities
    {
        public enum Token
        {
            GOTO = 0,
            INTEGER = 1,
            TO = 2,
            DO = 3,
            IF = 4,
            THEN = 5,
            ELSE = 6,
            FOR = 7,
            OF = 8,
            WRITELN = 10,
            READLN = 11,
            END = 12,
            VAR = 13,
            WHILE = 14,
            UNIT = 15,
            LABEL = 16,
            REPEAT = 17,
            UNTIL = 18,
            PROCEDURE = 19,
            DOWNTO = 20,
            FUNCTION = 21,
            RETURN = 22,
            REAL = 23,
            STRING = 24,
            ARRAY = 25,
            SLASH = 30,
            STAR = 31,
            PLUS = 32,
            MINUS = 33,
            LPAR = 34,
            RPAR = 35,
            SEMICOLON = 36,
            COLON_EQUALS = 37,
            GT = 38,
            LT = 39,
            GTEQ = 40,
            LTEQ = 41,
            EQUAL = 42,
            NOT = 43,
            COMMA = 44,
            LBRAC = 45,
            RBRAC = 46,
            COLON = 47,
            PERIOD = 48,
            IDENTIFIER = 50,
            INTTYPE = 51,
            FLOAT = 52,
            STRINGTYPE = 53,
            UNDEFINED = 99
        }
    }
}
