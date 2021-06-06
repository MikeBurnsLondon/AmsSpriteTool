using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace WpfApplication
{
    public interface IPalette
    {
        // Properties
        int Mode { get; set; }
        int NumPens { get; }

        // Methods
        Color GetColour(int penNum);
        Color GetSystemColour(int colourNum);
        void SetInk(int penNum, Color colourNum);
        void Load(string fileName);
        void Save(string fileName);
        void SetDefaultPalette();
    }
}
