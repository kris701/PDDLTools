using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLTools.Helpers
{
    public static class TaggerHelper
    {
        public static TextExtent? GetExtendOfObjectWord(SnapshotPoint currentRequest)
        {
            int lineNumber = currentRequest.GetContainingLineNumber();
            var line = currentRequest.Snapshot.GetLineFromLineNumber(lineNumber);

            int startIndex = GetStartIndexOfMarking(line, currentRequest);
            int endIndex = GetEndIndexOfMarking(line, currentRequest);

            if (startIndex == -1 || endIndex == -1)
                return null;

            if (endIndex < startIndex)
                return null;

            var newSpan = new SnapshotSpan(line.Snapshot, line.Extent.Start + startIndex, endIndex - startIndex + 1);
            TextExtent word = new TextExtent(newSpan, true);
            return word;
        }

        private static int GetStartIndexOfMarking(ITextSnapshotLine line, SnapshotPoint currentRequest)
        {
            int startIndex = currentRequest.Position - line.Extent.Start;

            var chars = line.Snapshot.ToCharArray(line.Extent.Start, line.Extent.Length);
            if (startIndex < 0)
                return -1;
            if (startIndex >= chars.Length)
                return -1;
            char currentChar = chars[startIndex];
            while (char.IsLetter(currentChar) || char.IsNumber(currentChar) || currentChar == '-' || currentChar == ':')
            {
                startIndex--;
                if (startIndex < 0)
                    return 0;
                currentChar = chars[startIndex];
            }
            startIndex++;
            return startIndex;
        }

        private static int GetEndIndexOfMarking(ITextSnapshotLine line, SnapshotPoint currentRequest)
        {
            int endIndex = currentRequest.Position - line.Extent.Start;

            var chars = line.Snapshot.ToCharArray(line.Extent.Start, line.Extent.Length);
            if (endIndex < 0)
                return -1;
            if (endIndex >= chars.Length)
                return -1;

            char currentChar = chars[endIndex];
            while (char.IsLetter(currentChar) || char.IsNumber(currentChar) || currentChar == '-' || currentChar == ':')
            {
                endIndex++;
                if (endIndex >= chars.Length)
                    return chars.Length - 1;
                currentChar = chars[endIndex];
            }
            endIndex--;
            return endIndex;
        }
    }
}
