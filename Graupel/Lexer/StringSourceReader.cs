using System;
using Graupel.Util;

namespace Graupel.Lexer
{
    public class StringSourceReader : ISourceReader
    {
        public string Description { get; private set; }
        public char Current
        {
            get
            {
                return position >= text.Length
                           ? '\0'
                           : text[position];
            }
        }

        private readonly String text;
        private int position;

        public StringSourceReader(string description, string text)
        {
            Expect.NotEmpty(description, text);

            Description = description;
            this.text = text;
            position = 0;
        }
        
        public void Advance()
        {
            if (position < text.Length) position++;
        }
    }
}
