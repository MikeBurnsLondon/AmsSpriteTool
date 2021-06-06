using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace WpfApplication.Models
{
	public interface ICell
	{
		/// <summary>
		/// State of the cell.
		/// </summary>
		bool State { get; set; }

        Color CellColour { get; set; }

        int PenNum { get; set; }
    }
}
