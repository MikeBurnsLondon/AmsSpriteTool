using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfApplication.ViewModels;

namespace WpfApplication
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
        private EditPalette editPaletteWin;
        private SubstitutePen subsPenWin;

        /// <summary>
        /// Constructor. Initialise the WPF window, set up the top level data model. 
        /// </summary>
		public MainWindow()
		{
			InitializeComponent();

            var modelContainer = new ModelContainer(new CPCPalette(), 1)
            {
                SpriteModel = new DynamicGridViewModel("FillCell")
                {
                    GridWidth = 25,
                    GridHeight = 15
                    // ,ImageSource = new BitmapImage(new Uri("/WpfApplication;component/Resources/Games-Edit.ico", UriKind.Relative))
                },
                PaletteModel = new DynamicGridViewModel("SetCurrentPen")
                {
                    GridWidth = 16,
                    GridHeight = 1,
                    BorderPixels = 2,
                    BorderColour = Colors.DarkTurquoise
                },
                AltPaletteModel = new DynamicGridViewModel("SubstitutePen")
                {
                    GridWidth = 16,
                    GridHeight = 1
                }
            };

            modelContainer.Initialize();

            DataContext = modelContainer;

        }

        /// <summary>
        /// Display the About screen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mnuHelpAbout_Click(object sender, RoutedEventArgs e)
        {
            About aboutWin = new About();
            aboutWin.Show();
        }

        /// <summary>
        /// Load a Sprite from disk
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mnuFileOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Asm files (*.asm)|*.asm|All files (*.*)|*.*";
            var trans = new CPCTransformer();
            if (openFileDialog.ShowDialog() == true)
            {
                trans.ReadFromFile(openFileDialog.FileName);
                trans.PopulateDesignGrid((ModelContainer)DataContext);
                ((ModelContainer)DataContext).SpriteModel.UpdateImageSource();  // This is just for debugging, probably refactor 
            }
        }

        /// <summary>
        /// Load palette from disk
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mnuPaletteOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Pal files (*.pal)|*.pal|All files (*.*)|*.*";
            var trans = new CPCTransformer();
            if (openFileDialog.ShowDialog() == true)
            {
                ((ModelContainer)DataContext).Palette.Load(openFileDialog.FileName);
                trans.PopulatePaletteGrid((ModelContainer)DataContext);
            }
            // Update the Sprite Grid with the new colours 
            ((ModelContainer)DataContext).UpdateSpriteModel();
            ((ModelContainer)DataContext).SetDefaultPalette();
            ((ModelContainer)DataContext).SpriteModel.UpdateImageSource();
        }

        /// <summary>
        /// Save current palette to disk 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mnuPaletteSave_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Pal files (*.pal)|*.pal|All files (*.*)|*.*";
            if (saveFileDialog.ShowDialog() == true)
            {
                ((ModelContainer)DataContext).Palette.Save(saveFileDialog.FileName);
            }
        }

        /// <summary>
        /// Exit the Application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mnuFileExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        /// <summary>
        /// Display the Palette Edit window 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mnuPaletteEdit_Click(object sender, RoutedEventArgs e)
        {
            if (editPaletteWin == null)
            {
                editPaletteWin = new EditPalette(DataContext);
                editPaletteWin.Closed += (a, b) => editPaletteWin = null;
                editPaletteWin.Show();
            }
            else editPaletteWin.Show();
        }

        /// <summary>
        /// Discard existing Sprite and start afresh
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mnuFileNew_Click(object sender, RoutedEventArgs e)
        {
            ((ModelContainer)DataContext).SpriteModel = new DynamicGridViewModel("FillCell")
            {
                GridWidth = 25,
                GridHeight = 15
            };
            ((ModelContainer)DataContext).InitializeSpriteModel();

        }

        /// <summary>
        /// Save current pixel in design grid to disk
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mnuFileSave_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Asm files (*.asm)|*.asm|All files (*.*)|*.*";
            var trans = new CPCTransformer();
            if (saveFileDialog.ShowDialog() == true)
            {
                trans.SaveToFile(saveFileDialog.FileName, ((ModelContainer)DataContext).SpriteModel);
            }
        }

        private void mnuAddRowAbove_Click(object sender, RoutedEventArgs e) { ((ModelContainer)DataContext).AddRowAbove(); }
        private void mmuAddRowBelow_Click(object sender, RoutedEventArgs e) { ((ModelContainer)DataContext).AddRowBelow(); }
        private void mnuAddColumnLeft_Click(object sender, RoutedEventArgs e) { ((ModelContainer)DataContext).AddColumnLeft(); }
        private void mnuAddColumnRight_Click(object sender, RoutedEventArgs e) { ((ModelContainer)DataContext).AddColumnRight(); }

        private void mnuRemoveTopRow_Click(object sender, RoutedEventArgs e) { ((ModelContainer)DataContext).RemoveTopRow(); }
        private void mnuRemoveBottomRow_Click(object sender, RoutedEventArgs e) { ((ModelContainer)DataContext).RemoveBottomRow(); }
        private void mnuRemoveLeftColumn_Click(object sender, RoutedEventArgs e) { ((ModelContainer)DataContext).RemoveLeftColumn(); }
        private void mnuRemoveRightColumn_Click(object sender, RoutedEventArgs e) { ((ModelContainer)DataContext).RemoveRightColumn(); }

        private void mnuSubstitute_Click(object sender, RoutedEventArgs e)
        {
            if (subsPenWin == null)
            {
                subsPenWin = new SubstitutePen(DataContext);
                subsPenWin.Closed += (a, b) => subsPenWin = null;
                subsPenWin.Show();
            }
            else subsPenWin.Show();
        }

        private void mnuRotateClockwise_Click(object sender, RoutedEventArgs e)
        {
            ((ModelContainer)DataContext).RotateClockwise();
        }

        private void mnuRotateAntiClockwise_Click(object sender, RoutedEventArgs e)
        {
            ((ModelContainer)DataContext).RotateAnticlockwise();
        }

        private void mnuToolsOptions_Click(object sender, RoutedEventArgs e)
        {
            // Default new grid size, OP file in C (0x..) or Assembler (DB #..) format 
        }

        private void mnuFlipHorizontal_Click(object sender, RoutedEventArgs e)
        {
            ((ModelContainer)DataContext).FlipHorizontal();
        }

        private void mnuFlipVertical_Click(object sender, RoutedEventArgs e)
        {
            ((ModelContainer)DataContext).FlipVertical();
        }

    }
}
