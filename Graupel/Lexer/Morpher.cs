namespace Graupel.Lexer
{
    public class Morpher : ITokenReader
    {
        private readonly ITokenReader reader;
        //private bool eatLines;

        public Morpher(ITokenReader reader)
        {
            this.reader = reader;
            //eatLines = true;
        }

        public Token ReadToken()
        {
            while (true)
            {
                Token token = reader.ReadToken();
                switch (token.Type)
                {
                    case TokenType.WhiteSpace:
                        case TokenType.BlockComment:
                        case TokenType.LineComment:
                        case TokenType.LineEnd:
                        continue;
                    default:
                        return token;
                }
            }
        }
    }
}
