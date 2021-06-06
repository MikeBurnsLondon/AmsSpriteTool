using System.Windows.Input;
using WpfApplication.Models;

namespace WpfApplication.ViewModels
{
	internal class DesignCellViewModel : ICellViewModel
	{
		public ICell Cell { get; set; } = Models.Cell.Empty;
        public ICommand CellClickCommand { get; } = null;
    }
}
