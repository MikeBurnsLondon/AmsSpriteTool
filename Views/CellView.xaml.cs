using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfApplication.Views
{
	/// <summary>
	/// Externally controllable properties for grid cells
	/// </summary>
	public partial class CellView : UserControl
	{
		public CellView()
		{
			InitializeComponent();
		}

        /// <summary>
        /// Thickness of cell borders in pixels. Set at instantiation of DynamicGridViewModel.
        /// </summary>
        public int BorderPixels
        {
            get { return (int)GetValue(BorderPixelsProperty); }
            set { SetValue(BorderPixelsProperty, value); }
        }

        public static readonly DependencyProperty BorderPixelsProperty = DependencyProperty.Register("BorderPixels", typeof(int), typeof(CellView));

        /// <summary>
        /// Cell border colour. Set at instantiation of DynamicGridViewModel.
        /// </summary>
        public Color BorderColour
		{
			get { return (Color)GetValue(BorderColourProperty); }
			set { SetValue(BorderColourProperty, value); }
		}

		public static readonly DependencyProperty BorderColourProperty = DependencyProperty.Register("BorderColour", typeof(Color), typeof(CellView));
    }

}
