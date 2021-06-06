using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Media;

namespace WpfApplication.ViewModels
{
	internal class DesignDynamicGridViewModel : IDynamicGridViewModel
	{
		public ObservableCollection<ObservableCollection<ICellViewModel>> Cells { get; } = null;
		public int GridWidth { get; set; } = 5;
		public int GridHeight { get; set; } = 5;
        public int BorderPixels { get; set; } = 0;
        public Color FinishColor { get; set; } = Colors.DarkBlue;
        public Color BorderColour { get; set; } = Colors.Green;
        public void AddRowAbove(int currentPenNum, Color currentPenColour) { }
        public void AddRowBelow(int currentPenNum, Color currentPenColour) { }
        public void AddColumnLeft(int currentPenNum, Color currentPenColour) { }
        public void AddColumnRight(int currentPenNum, Color currentPenColour) { }
        public void RemoveTopRow() { }
        public void RemoveBottomRow() { }
        public void RemoveLeftColumn() { }
        public void RemoveRightColumn() { }
        public void UpdateImageSource() { }
    }
}
