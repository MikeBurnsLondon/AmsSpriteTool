using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Media;

namespace WpfApplication.ViewModels
{
	public interface IDynamicGridViewModel
	{
		/// <summary>
		/// 2-dimensional collections for CellViewModels.
		/// </summary>
		ObservableCollection<ObservableCollection<ICellViewModel>> Cells { get; }

		/// <summary>
		/// Count of grid columns.
		/// </summary>
		int GridWidth { get; set; }

		/// <summary>
		/// Count of grid rows.
		/// </summary>
		int GridHeight { get; set; }

        /// <summary>
        /// Thickness of cell borders in pixels
        /// </summary>s
        int BorderPixels { get; set; }
   
        /// <summary>
        /// Color of borders around cells.
        /// </summary>
        Color BorderColour { get; set; }

        void AddRowAbove(int currentPenNum, Color currentPenColour);
        void AddRowBelow(int currentPenNum, Color currentPenColour);
        void AddColumnLeft(int currentPenNum, Color currentPenColour);
        void AddColumnRight(int currentPenNum, Color currentPenColour);

        void RemoveTopRow();
        void RemoveBottomRow();
        void RemoveLeftColumn();
        void RemoveRightColumn();

        void UpdateImageSource();
    }
}