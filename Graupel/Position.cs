using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Graupel
{
    public class Position
    {
        public String SourceFile { get; private set; }
        public int StartLine { get; private set; }
        public int StartCol { get; private set; }
        public int EndLine { get; private set; }
        public int EndCol { get; private set; }

        public static Position None
        {
            get { return new Position(String.Empty, -1, -1, -1, -1); }
        }

        public Position(String sourceFile, int startLine, int startCol, int endLine, int endCol)
        {
            SourceFile = sourceFile;
            StartLine = startLine;
            StartCol = startCol;
            EndLine = endLine;
            EndCol = endCol;
        }

        //public static Position Surrounding(Expr begin, Expr end) {}
        //public static Position Surrounding(List<Expr> expressions) {}

        public Position Union(Position other)
        {
            if (other == null)
                throw new ArgumentNullException("other");
            if (other.SourceFile != SourceFile)
                throw new InvalidOperationException(
                    "Tried to Union Position objects but source files do not match.");
            int startLine = Math.Min(StartLine, other.StartLine);
            int startCol = Math.Min(StartCol, other.StartCol);
            int endLine = Math.Max(EndLine, other.EndLine);
            int endCol = Math.Max(EndCol, other.EndCol);
            return new Position(SourceFile,startLine,startCol,endLine,endCol);
        }

        public static Position Union(Position pos1, Position pos2)
        {
            if (pos1 == null || pos2 == null)
                throw new ArgumentNullException(pos1 == null ? "pos1" : "pos2");
            if (pos1.SourceFile != pos2.SourceFile)
                throw new InvalidOperationException(
                    "Tried to Union Position objects but source files do not match.");
            int startLine = Math.Min(pos1.StartLine, pos2.StartLine);
            int startCol = Math.Min(pos1.StartCol, pos2.StartCol);
            int endLine = Math.Max(pos1.EndLine, pos2.EndLine);
            int endCol = Math.Max(pos1.EndCol, pos2.EndCol);
            return new Position(pos1.SourceFile, startLine, startCol, endLine, endCol);
        }

        public override string ToString()
        {
            if (StartLine == -1)
                return "(Unknown position)";
            if (StartLine == EndLine)
                return String.Format("{0} (line {1}, col {2}-{3})",
                                     SourceFile, StartLine, StartCol, EndCol);
            return String.Format("{0} (line {1} col {2} - line {3} col {4})",
                                 SourceFile, StartLine, StartCol, EndLine, EndCol);
        }
    }
}
