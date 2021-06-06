using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication
{
    public class CPCTransformer : TransformerBase,  ITransformer
    {
        public void PopulateDesignGrid(ModelContainer models)
        {
            models.SpriteModel.GridHeight = numRows;
            models.SpriteModel.GridWidth = numCols*2; // Note this only the case for Mode 0! 

            // *** TODO *** Do it first for Mode 0, add other modes later

            for (int rowNum=0; rowNum<numRows; rowNum++)
            {
                for (int colNum=0; colNum<numCols; colNum++)
                {
                    char[] bits = Convert.ToString(_rawBytes[rowNum][colNum], 2).PadLeft(8, '0').ToArray();

                    var leftPen = Convert.ToInt32(bits[6].ToString() + bits[2].ToString() + bits[4].ToString() + bits[0].ToString(), 2);
                    var rightPen = Convert.ToInt32(bits[7].ToString() + bits[3].ToString() + bits[5].ToString() + bits[1].ToString(), 2);

                    models.SpriteModel.Cells[rowNum][colNum * 2].Cell.PenNum = leftPen;
                    models.SpriteModel.Cells[rowNum][colNum * 2].Cell.CellColour = models.Palette.GetColour(leftPen);
                    models.SpriteModel.Cells[rowNum][(colNum * 2) + 1].Cell.PenNum = rightPen;
                    models.SpriteModel.Cells[rowNum][(colNum * 2) +1 ].Cell.CellColour = models.Palette.GetColour(rightPen);
                }
            }
        }

        public void PopulatePaletteGrid(ModelContainer models)
        {
            for (int i = 0; i < 16; i++)
            {
                models.PaletteModel.Cells[0][i].Cell.CellColour = models.Palette.GetColour(i);
                models.AltPaletteModel.Cells[0][i].Cell.CellColour = models.Palette.GetColour(i);
            }
        }
    }
}
