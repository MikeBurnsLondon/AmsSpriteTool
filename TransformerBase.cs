using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;
using WpfApplication.ViewModels;

namespace WpfApplication
{
    public class TransformerBase
    {
        protected List<List<int>> _rawBytes;
        protected int numRows = 0;
        protected int numCols = 0;
        protected int modeNum = 0;

        /// <summary>
        /// Load data from passed in file contents into rawbytes object, structured <Row<Column>>
        /// Initially use conventions from ConvImgCPC, we may enhance later to work with other file variants
        /// We need to determine the number of rows and columns. If this is specified in a header comment (prefixed ";"),
        /// use that. If not, use the number of items in first line and the number of lines
        /// </summary>
        /// <param name="input"></param>
        public void ReadFromFile(string fileName)
        {
            numRows = 0;
            numCols = 0;
            modeNum = 0;

            var fileContents = File.ReadAllText(fileName);
            var stringSeparators = new string[] { "\r\n" };
            var lines = fileContents.Split(stringSeparators, StringSplitOptions.None);

            var numLines = lines.Length;
            var colCount = 0;
            var lineCount = 0;

            _rawBytes = new List<List<int>>() { new List<int>() };  // Clear any previous data, initialise first row, when it's filled we will add another, and so on
            for (int line = 0; line < numLines; line++)
            {
                var curLine = lines[line].Trim();
                if (curLine == "") continue;
                if (curLine.Substring(0, 1) == ";")
                {
                    // Check for grid dimensions
                    // Explanation of this Regex (because one always forgets and has to work it out afresh each time !):
                    // From start of string '^', require but don't capture '(?:' 0 or more whitespace '\s*' followed by semicolon';'
                    // followed by 0 or more whitespace '\s*'. Then require one or more digits '[0-9]+' followed by one or more 
                    // whitespace '\s*' followed by x or * '[x|*]' followed by one or more whitespace '\s*' followed by one or more 
                    // digits '[0-9]+'. The two sets of digits are the matching groups (enclosed in parentheses) 
                    var pattern1 = @"^(?:\s*;\s*)([0-9]+)\s*[x|*]\s*([0-9]+)";
                    var gridSizeMatches = Regex.Matches(curLine, pattern1, RegexOptions.IgnoreCase);
                    if (gridSizeMatches.Count > 0)
                    {
                        numCols = Convert.ToInt32(gridSizeMatches[0].Groups[1].Value);
                        numRows = Convert.ToInt32(gridSizeMatches[0].Groups[2].Value);
                        continue;
                    }

                    // Check for Mode
                    var pattern2 = @"^(?:\s*;\s*)(?:Mode\s*)([0-9])";
                    var modeMatches = Regex.Matches(curLine, pattern2, RegexOptions.IgnoreCase);
                    if (modeMatches.Count > 0) modeNum = Convert.ToInt32(modeMatches[0].Groups[1].Value);

                    continue;
                }

                // Check for valid line of data
                var pattern4 = @"0x[0-9a-f]+|#[0-9a-f]+";
                var dataMatches = Regex.Matches(curLine, pattern4, RegexOptions.IgnoreCase);
                if (dataMatches.Count > 0)
                {
                    // If number of columns wasn't specified in comment, assume number of data points in first line 
                    if (numCols == 0) numCols = dataMatches.Count;
                    foreach (var match in dataMatches)
                    {
                        var intFromHex = Convert.ToInt32(((Match)match).Groups[0].Value.Replace("#", "0x"), 16);
                        _rawBytes[lineCount].Add(intFromHex);
                        colCount++;
                        if (colCount == numCols)
                        {
                            colCount = 0;
                            lineCount++;
                            _rawBytes.Add(new List<int>());
                        }
                    }
                }
            }
            if (numRows == 0) numRows = lineCount;
            if (colCount == 0) { _rawBytes.RemoveAt(lineCount); lineCount--; }
        }

        /// <summary>
        /// Save sprite data from referenced model to disk file. *** MJB *** Note this currently only supports Mode 0 !
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="model"></param>
        public void SaveToFile(string fileName, IDynamicGridViewModel model)
        {
            var opText = new StringBuilder();

            // Output file header
            opText.Append(";\r\n");
            opText.Append(";\r\n");
            opText.Append(";Generated by Amstrad CPC Sprite Manipulation Tool Version " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString() + "\r\n");
            opText.Append(";\r\n");
            opText.Append("; '" + fileName + "'\r\n");
            opText.Append("; Mode 0\r\n");
            opText.Append("; " + model.GridWidth / 2 + "x" + model.GridHeight + "\r\n");
            opText.Append("; Linear\r\n");
            opText.Append(";\r\n\tDB\t");

            // Data bytes
            var opColCount = 0;
            for (var rowNum = 0; rowNum < model.GridHeight; rowNum++)
            {
                for (var colNum = 0; colNum < model.GridWidth / 2; colNum++)
                {
                    var leftBits = Convert.ToString(model.Cells[rowNum][colNum*2].Cell.PenNum, 2).PadLeft(4, '0').ToArray();
                    var rightBits = Convert.ToString(model.Cells[rowNum][(colNum*2)+1].Cell.PenNum, 2).PadLeft(4, '0').ToArray();

                    var opValue = Convert.ToByte(leftBits[3].ToString() + rightBits[3].ToString() + leftBits[1].ToString() + rightBits[1].ToString() +
                                                  leftBits[2].ToString() + rightBits[2].ToString() + leftBits[0].ToString() + rightBits[0].ToString(), 2);
                    opText.AppendFormat("#{0:X2}, ", opValue);

                    opColCount++;
                    if (opColCount == 8)
                    {
                        opText.Remove(opText.Length - 2, 2);
                        opText.Append("\r\n\tDB\t");
                        opColCount = 0;
                    }
                }
            }
            if (opColCount > 0) opText.Remove(opText.Length - 2, 2);

            // Write the file
            File.WriteAllText(fileName, opText.ToString());
        }
    }
}    
