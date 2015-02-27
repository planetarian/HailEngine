using System;
using System.Collections.Generic;
using System.Text;
using Graupel.Util;

namespace Graupel.Lexer
{
    public class Lexer : ITokenReader
    {
        private readonly ISourceReader reader;
        private String read;
        private int startLine;
        private int startCol;
        private int line;
        private int col;
        private char cur;
        private char prev;

        private static readonly Dictionary<string, TokenType> keywords
            = new Dictionary<string, TokenType>();

        private char Current
        {
            get { return reader.Current; }
        }

        private Position CurrentPosition
        {
            get { return new Position(reader.Description, startLine, startCol, line, col); }
        }

        static Lexer()
        {
            keywords.Add("global", TokenType.Global);
            keywords.Add("scene", TokenType.Scene);
            keywords.Add("template", TokenType.Template);
            keywords.Add("entity", TokenType.Entity);
            keywords.Add("group", TokenType.Group);
            keywords.Add("eval", TokenType.Eval);
            keywords.Add("true", TokenType.Boolean);
            keywords.Add("false", TokenType.Boolean);
            keywords.Add("?", TokenType.Question);
            keywords.Add("(", TokenType.LeftParen);
            keywords.Add(")", TokenType.RightParen);
            keywords.Add("{", TokenType.LeftBrace);
            keywords.Add("}", TokenType.RightBrace);
            keywords.Add(",", TokenType.Comma);
            keywords.Add(":", TokenType.Colon);
            keywords.Add(";", TokenType.SemiColon);
            keywords.Add("*", TokenType.Asterisk);
            keywords.Add("/", TokenType.Slash);
            keywords.Add("%", TokenType.Percent);
            keywords.Add("+", TokenType.Plus);
            keywords.Add("-", TokenType.Minus);
            keywords.Add("^", TokenType.Caret);
        }

        public Lexer(ISourceReader reader)
        {
            Expect.NotNull(reader);

            this.reader = reader;
            line = 1;
            col = 1;
            startLine = 1;
            startCol = 1;
            read = String.Empty;
        }

        public Token ReadToken()
        {
            prev = cur;
            cur = Advance();
            switch (cur)
            {
                case ' ':
                case '\t':
                    return ReadWhitespace();

                case '(':
                    return MakeToken(TokenType.LeftParen);
                case ')':
                    return MakeToken(TokenType.RightParen);
                case '{':
                    return MakeToken(TokenType.LeftBrace);
                case '}':
                    return MakeToken(TokenType.RightBrace);
                case ',':
                    return MakeToken(TokenType.Comma);
                case ';':
                    return MakeToken(TokenType.SemiColon);
                case ':':
                    return MakeToken(TokenType.Colon);
                case '?':
                    return MakeToken(TokenType.Question);
                case '!':
                    return MakeToken(TokenType.Bang);

                case '\r':
                case '\n':
                    return MakeToken(TokenType.LineEnd);

                case '"':
                    return ReadString();

                case '/':
                    switch (Current)
                    {
                        case '/':
                            return ReadLineComment();
                        case '*':
                            return ReadBlockComment();
                        default:
                            return ReadIdentifier();
                    }

                case '-':
                case '.':
                    if (cur == '.' && IsDigit(Current))
                        return ReadNumber();
                    if (cur == '-')
                    {
                        if (!IsDigit(prev))
                        {
                            if (IsDigit(Current))
                                return ReadNumber();
                        }
                    }
                    if (IsOperator(Current))
                        return ReadIdentifier();
                    return MakeToken(TokenType.Identifier);

                case '\0':
                    return MakeToken(TokenType.EOF);

                default:
                    if (IsIdentifier(cur))
                        return ReadIdentifier();
                    if (IsOperator(cur))
                        return ReadOperator();
                    if (IsDigit(cur))
                        return ReadNumber();
                    throw new ParseException(Position.None, "Unknown character.");
            }
        }

        private Token ReadWhitespace()
        {
            while (true)
            {
                switch (Current)
                {
                    case ' ':
                    case '\t':
                        Advance();
                        break;
                    default:
                        return MakeToken(TokenType.WhiteSpace);
                }
            }
        }

        private Token ReadString()
        {
            var escaped = new StringBuilder();
            while (true)
            {

                char c = Advance();
                switch (c)
                {
                    case '\\':
                        // escape sequence
                        char e = Advance();
                        switch (e)
                        {
                            case '\'':
                                escaped.Append('\'');
                                break;
                            case '"':
                                escaped.Append('\"');
                                break;
                            case '\\':
                                escaped.Append('\\');
                                break;
                            case '0':
                                escaped.Append('\0');
                                break;
                            case 'a':
                                escaped.Append('\a');
                                break;
                            case 'b':
                                escaped.Append('\b');
                                break;
                            case 'f':
                                escaped.Append('\f');
                                break;
                            case 'n':
                                escaped.Append('\n');
                                break;
                            case 'r':
                                escaped.Append('\r');
                                break;
                            case 't':
                                escaped.Append('\t');
                                break;
                            case 'v':
                                escaped.Append('\v');
                                break;
                            //TODO case 'U'
                            //case 'U':
                                //escaped.Append('\U');
                                //break;
                            case 'u':
                            case 'x':
                                int a = ReadHexDigit();
                                int b = ReadHexDigit();
                                int code = (a << 4) | b;
                                //TODO: 4-digit codes too
                                escaped.Append((char)code);
                                break;
                            default:
                                // TODO: error token
                                throw new ParseException(Position.None, "Unknown string escape sequence.");
                        }
                        break;
                    case '"':
                        return MakeToken(TokenType.String, escaped.ToString());
                    case '\0':
                        // TODO: error token
                        throw new ParseException(Position.None, "Unexpected end of file within string value.");
                    default:
                        escaped.Append(c);
                        break;
                }

            }
        }

        private int ReadHexDigit()
        {
            char c = char.ToLower(Advance());
            int digit = "0123456789abcdef".IndexOf(c);
            if (digit == -1)
            {
                // TODO: error token instead of exception
                throw new ParseException(Position.None, "Expected hex digit.");
            }
            return digit;
        }

        private Token ReadLineComment()
        {
            Advance(); // second '/'

            int slashCount = 2;
            
            while (Current == '/')
            {
                ++slashCount;
                Advance();
            }

            while (true)
            {
                switch (Current)
                {
                    case '\n':
                    case '\r':
                    case '\0':
                        string value = read.Substring(slashCount).Trim();
                        return MakeToken(TokenType.LineComment, value);
                    default:
                        Advance();
                        break;
                }
            }

        }

        private Token ReadBlockComment()
        {
            const string nullError = "Unexpected end of file inside block comment.";
            while (true)
            {
                switch(Advance())
                {
                    case '*':
                        switch (Advance())
                        {
                            case '/':
                                return MakeToken(TokenType.BlockComment);
                            case '\0':
                                // TODO: Emit error token instead of exception
                                throw new ParseException(Position.None, nullError);
                                // Otherwise keep advancing.
                        }
                        break;
                    case '\0':
                        // TODO: Emit error token instead of exception
                        throw new ParseException(Position.None, nullError);
                    // Otherwise keep advancing.
                }
            }
        }

        private Token ReadIdentifier()
        {
            int idx = 0;
            while (true)
            {
                if (IsIdentifier(Current) || (idx > 0 && IsDigit(Current)))
                    Advance();
                else
                    return MakeToken(TokenType.Identifier);
                idx++;
            }
        }

        private Token ReadOperator()
        {
            while (true)
            {
                if (IsIdentifier(Current) || IsOperator(Current))
                    Advance();
                else
                    return MakeToken(TokenType.Identifier);
            }
        }

        private Token ReadNumber()
        {
            int periods = read[0] == '.' ? 1 : 0;
            while (true)
            {
                char c = Current;
                if (c == '.')
                {
                    periods++;
                    if (periods > 1)
                        throw new ParseException(Position.None, "Invalid decimal value.");
                    Advance();
                }
                else if (IsDigit(c))
                {
                    Advance();
                }
                else
                {
                    return periods > 0
                        ? MakeToken(TokenType.Float, float.Parse(read))
                        : MakeToken(TokenType.Integer, Int32.Parse(read));
                }
            }
        }

        private char Advance()
        {
            char c = Current;
            reader.Advance();
            read += c;

            // update position
            if (c == '\n')
            {
                line++;
                col = 1;
            }
            else col++;

            return c;
        }

        private Token MakeToken(TokenType type)
        {
            return MakeToken(type, read);
        }

        private Token MakeToken(TokenType type, Object value)
        {
            Expect.NotEmpty(read);
            string readLower = read.ToLower();
            if (type == TokenType.Identifier)
            {
                if (keywords.ContainsKey(readLower))
                {
                    type = keywords[readLower];
                }

                if (readLower == "true")
                    value = true;
                else if (readLower == "false")
                    value = false;
            }

            var token = new Token(CurrentPosition, type, read, value);

            startLine = line;
            startCol = col;
            read = String.Empty;

            return token;
        }

        private static bool IsDigit(char c)
        {
            return (c >= '0') && (c <= '9');
        }

        private static bool IsIdentifier(char c)
        {
            return ((c >= 'a') && (c <= 'z'))
                || ((c >= 'A') && (c <= 'Z'))
                || (c == '_');//TODO: || (c == '.');
        }

        private static bool IsOperator(char c)
        {
            return "%*/-+".IndexOf(c) != -1;
        }

        public TokenType TypeFromKeyword(string keyword)
        {
            return keywords.ContainsKey(keyword) ? keywords[keyword] : TokenType.Invalid;
        }
    }
}
