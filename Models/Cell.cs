using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ikc5.TypeLibrary;
using System.Windows.Media;

namespace WpfApplication.Models
{
	public class Cell : BaseNotifyPropertyChanged, ICell
	{
		public Cell()
		{
			this.SetDefaultValues();
		}

		#region Implementation ICell

		private bool _state;

		[DefaultValue(false)]
		public bool State
		{
			get { return _state; }
			set { SetProperty(ref _state, value); }
		}

        private Color _cellColour = Colors.Crimson ;  // Color.FromArgb(0xFF, 0xF0, 0x80, 0x80);

        public Color CellColour
        {
            get { return _cellColour; }
            set { SetProperty(ref _cellColour, value); }
        }

        public int PenNum { get; set; }

		#endregion

		public static ICell Empty =>  new Cell();
	}
}
