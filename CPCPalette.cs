using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.IO;
using System.Collections.Specialized;

namespace WpfApplication
{
    public class CPCPalette : IPalette
    {
        // Translations to Amstrad hardware number and RGB (PC) for each Amstrad ink number. Initialised in consructor.
        private static readonly Dictionary<int, Tuple<int, Color>> PCColours = new Dictionary<int, Tuple<int, Color>>();

        // Current Mode number
        private int _mode;

        // The number of pens in the palette - each can be assigned a different ink. We default to Mode 0 - so 16 Pens
        private int _numPens = 16;

        // List structure holding the ink number for each pen
        private List<int> penInks = Properties.Settings.Default.DefaultPalette.Cast<String>().Select(int.Parse).ToList();

        /// <summary>
        /// Save the current palette as Default
        /// </summary>
        public void SetDefaultPalette()
        {
            var result = new StringCollection();
            result.AddRange(penInks.Select(x => x.ToString()).ToArray());
            Properties.Settings.Default.DefaultPalette = result;
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Constructor. Initilaise colour translation dictionary. 
        /// </summary>
        public CPCPalette()
        {
            PCColours.Add(0, new Tuple<int, Color>(0x54, Color.FromRgb(0x00, 0x00, 0x00)));    // 0 - Black
            PCColours.Add(1, new Tuple<int, Color>(0x44, Color.FromRgb(0x00, 0x00, 0x80)));    // 1 - Blue
            PCColours.Add(2, new Tuple<int, Color>(0x55, Color.FromRgb(0x00, 0x00, 0xFF)));    // 2 - Bright Blue
            PCColours.Add(3, new Tuple<int, Color>(0x5C, Color.FromRgb(0x80, 0x00, 0x00)));    // 3 - Red
            PCColours.Add(4, new Tuple<int, Color>(0x58, Color.FromRgb(0x80, 0x00, 0x80)));    // 4 - Magenta
            PCColours.Add(5, new Tuple<int, Color>(0x5D, Color.FromRgb(0x80, 0x00, 0xFF)));    // 5 - Mauve
            PCColours.Add(6, new Tuple<int, Color>(0x4C, Color.FromRgb(0xFF, 0x00, 0x00)));    // 6 - Bright Red

            PCColours.Add(7, new Tuple<int, Color>(0x45, Color.FromRgb(0xFF, 0x00, 0x80)));    // 7 - Purple
            PCColours.Add(8, new Tuple<int, Color>(0x4D, Color.FromRgb(0xFF, 0x00, 0xFF)));    // 8 - Bright Magenta
            PCColours.Add(9, new Tuple<int, Color>(0x56, Color.FromRgb(0x00, 0x80, 0x00)));    // 9 - Green
            PCColours.Add(10, new Tuple<int, Color>(0x46, Color.FromRgb(0x00, 0x80, 0x80)));    // 10 - Cyan
            PCColours.Add(11, new Tuple<int, Color>(0x57, Color.FromRgb(0x00, 0x80, 0xFF)));    // 11 - Sky Blue
            PCColours.Add(12, new Tuple<int, Color>(0x5E, Color.FromRgb(0x80, 0x80, 0x00)));    // 12 - Yellow
            PCColours.Add(13, new Tuple<int, Color>(0x40, Color.FromRgb(0x80, 0x80, 0x80)));    // 13 - White

            PCColours.Add(14, new Tuple<int, Color>(0x5F, Color.FromRgb(0x80, 0x80, 0xFF)));    // 14 - Pastel Blue
            PCColours.Add(15, new Tuple<int, Color>(0x4E, Color.FromRgb(0xFF, 0x80, 0x00)));    // 15 - Orange
            PCColours.Add(16, new Tuple<int, Color>(0x47, Color.FromRgb(0xFF, 0x80, 0x80)));    // 16 - Pink
            PCColours.Add(17, new Tuple<int, Color>(0x4F, Color.FromRgb(0xFF, 0x80, 0xFF)));    // 17 - Pastel Magenta
            PCColours.Add(18, new Tuple<int, Color>(0x52, Color.FromRgb(0x00, 0xFF, 0x00)));    // 18 - Bright Green
            PCColours.Add(19, new Tuple<int, Color>(0x42, Color.FromRgb(0x00, 0xFF, 0x80)));    // 19 - Sea Green
            PCColours.Add(20, new Tuple<int, Color>(0x53, Color.FromRgb(0x00, 0xFF, 0xFF)));    // 20 - Bright Cyan

            PCColours.Add(21, new Tuple<int, Color>(0x5A, Color.FromRgb(0x80, 0xFF, 0x00)));    // 21 - Lime
            PCColours.Add(22, new Tuple<int, Color>(0x59, Color.FromRgb(0x80, 0xFF, 0x80)));    // 22 - Pastel Green
            PCColours.Add(23, new Tuple<int, Color>(0x5B, Color.FromRgb(0x80, 0xFF, 0xFF)));    // 23 - Pastel Cyan
            PCColours.Add(24, new Tuple<int, Color>(0x4A, Color.FromRgb(0xFF, 0xFF, 0x00)));    // 24 - Bright Yellow
            PCColours.Add(25, new Tuple<int, Color>(0x43, Color.FromRgb(0xFF, 0xFF, 0x80)));    // 25 - Pastel Yellow
            PCColours.Add(26, new Tuple<int, Color>(0x4B, Color.FromRgb(0xFF, 0xFF, 0xFF)));    // 26 - Bright White
        }

        /// <summary>
        /// Get/Set Screen Mode (0, 1, 2)
        /// </summary>
        public int Mode
        {
            get { return _mode; }
            set
            {
                if (value < 0 || value > 2) throw new Exception("Invalid mode selected");
                _mode = value;
                switch (_mode)
                {
                    case 0: _numPens = 16; break;
                    case 1: _numPens = 4; break;
                    case 2: _numPens = 2; break;
                }
            }
        }

        /// <summary>
        /// Set ink number for an individual Pen in the Palette - takes pen number and Windows Colour (RGB)
        /// </summary>
        /// <param name="penNum"></param>
        /// <param name="colour"></param>
        public void SetInk(int penNum, Color colour)
        {
            if (penNum < 0 || penNum > (_numPens - 1)) throw new Exception("Invalid pen number for current Mode");
            if (!PCColours.Where(x => x.Value.Item2 == colour).Any()) throw new Exception("Invalid colour - not supported by target machine");
            var colourNum = PCColours.Where(x => x.Value.Item2 == colour).First().Key;
            penInks[penNum] = colourNum;
        }

        /// <summary>
        /// Get RGB colour for specified pen number
        /// </summary>
        /// <param name="penNum">Pen number (i.e. index into penInks collection)</param>
        /// <returns>Windows Color (RGB)</returns>
        public Color GetColour(int penNum)
        {
            return PCColours[penInks[penNum]].Item2; 
        }

        /// <summary>
        /// Get colour from system palette (i.e. all possible colours) by colour number. Used to populate the EditPalette grid.
        /// </summary>
        /// <param name="colourNum">Amstrad Colour Number (0-26)</param>
        /// <returns>Windows Color (RGB)</returns>
        public Color GetSystemColour(int colourNum)
        {
            return PCColours[colourNum].Item2;
        }

        /// <summary>
        /// Get number of pens allowed in current screen Mode 
        /// </summary>
        public int NumPens
        {
            get { return _numPens; }
        }

        /// <summary>
        /// Load palette from disk file. File format is AMSDOS header (128 bytes):
        /// Byte 00: User number (value from 0 to 15 or #E5 for deleted entries)
        /// Byte 01 to 08: filename(fill unused char with spaces)
        /// Byte 09 to 11: Extension(fill unused char with spaces)
        /// Byte 16: first block(tape only)
        /// Byte 17: first block(tape only)
        /// Byte 18: file type(0:basic 1:protected 2:binary)
        /// Byte 21 and 22: loading address LSB first
        /// Byte 23: first block(tape only?)
        /// Byte 24 and 25: file length LSB first
        /// Byte 26 and 27: execution address for machine code program LSB first
        /// Byte 64 and 66: 24 bits file length LSB first.Just a copy, not used!
        /// Byte 67 and 68: checksum for bytes 00-66 stored LSB first
        /// Byte 69 to 127: undefined content, free to use
        /// Followed by Advanced OCP Art Studio (Amstrad Application) palette file format (colours are Amstrad hardware numbers ORed with 64d):  
        /// Offset(D)   Count   Description
        /// 0	        1	    Screen mode (see note 1)
        /// 1	        1	    Colour animation flag: "FF" = colour animation active, "00" = no colour animation
        /// 2           1	    Colour animation delay in frames - 1 
        /// 3	        1*12	palette entry 0: colours for 12 frames
        /// 15	        1*12	palette entry 1: colours for 12 frames
        /// 27	        1*12	palette entry 2: colours for 12 frames
        /// 39	        1*12	palette entry 3: colours for 12 frames
        /// 51	        1*12	palette entry 4: colours for 12 frames
        /// 63	        1*12	palette entry 5: colours for 12 frames
        /// 75	        1*12	palette entry 6: colours for 12 frames
        /// 87	        1*12	palette entry 7: colours for 12 frames
        /// 99	        1*12	palette entry 8: colours for 12 frames
        /// 111	        1*12	palette entry 9: colours for 12 frames
        /// 123	        1*12	palette entry 10: colours for 12 frames
        /// 135	        1*12	palette entry 11: colours for 12 frames
        /// 147	        1*12	palette entry 12: colours for 12 frames
        /// 159	        1*12	palette entry 13: colours for 12 frames
        /// 171	        1*12	palette entry 14: colours for 12 frames
        /// 183	        1*12	palette entry 15: colours for 12 frames
        /// 195	        1*12	border: colours for 12 frames
        /// 207	        16	    excluded inks
        /// 223	        16	    protected inks
        /// </summary>
        /// <param name="fileName"></param>
        public void Load(string fileName)
        {
            var fileContents = File.ReadAllBytes(fileName);
            // We are only interested in bytes 132 to 324, and the first byte of each block of 12 within that
            for (var bytePtr=131; bytePtr<323; bytePtr+=12)
            {
                penInks[(bytePtr - 131) / 12] = PCColours.Where(x => x.Value.Item1 == (fileContents[bytePtr] | 64)).Select(y => y.Key).First();
            }
        }

        /// <summary>
        /// Save palette file to disk. Format is as described in Load method above.
        /// </summary>
        /// <param name="fileName"></param>
        public void Save(string fileName)
        {
            var header = new byte[] {
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x02, 0xEF, 0x00, 0x09, 0x88, 0x00, 0xEF, 0x00, 0x09, 0x88, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0xEF, 0x00, 0x00, 0xC7, 0x06, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            };
            var paletteData = new byte[239];    // Palette data is 239 bytes (#EF)

            // Update header with file name - truncate or pad with spaces to ensure length 8/3
            var path = fileName.Substring(0, fileName.LastIndexOf("\\")+1);
            var noPath = fileName.Substring(fileName.LastIndexOf("\\")+1);
            if (noPath.IndexOf(".") < 0) noPath = noPath + ".";
            var part1 = noPath.Substring(0, noPath.IndexOf('.')).ToUpper().PadRight(8).Substring(0,8);
            var part2 = noPath.Substring(noPath.IndexOf('.')+1).ToUpper().PadRight(3).Substring(0,3);
            for (int i = 1; i < 9; i++) header[i] = (byte)part1[i - 1];
            for (int i=9; i<12; i++) header[i] = (byte)part2[i - 9];

            // Update checksum (first 67 bytes of header)
            var checkSum = header.ToList().Take(67).Select(x => (int)x).Sum();
            header[67] = (byte)checkSum;
            header[68] = (byte)(checkSum >> 8);

            // Generate Palette data
            for (int i=0; i<16; i++)
            {
                var colourCode = (byte)(PCColours[penInks[i]].Item1 | 64);
                for (int j=0; j<12; j++) paletteData[(i * 12) + j + 3] = colourCode;
            }

            var combined = new byte[header.Length + paletteData.Length];
            Buffer.BlockCopy(header, 0, combined, 0, header.Length);
            Buffer.BlockCopy(paletteData, 0, combined, header.Length, paletteData.Length);

            File.WriteAllBytes(path + part1 + "." + part2, combined);
        }
    }
}
