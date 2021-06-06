using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApplication.ViewModels;
using System.Windows.Media;
using Ikc5.TypeLibrary;
using System.ComponentModel;
using System.Windows.Media.Imaging;

namespace WpfApplication
{
    public class ModelContainer : BaseNotifyPropertyChanged
    {
        #region Constructor

        public ModelContainer(IPalette palette, int penNum)
        {
            Palette = palette;
            CurrentPenNum = penNum;
            CellViewModel.onPenSet += SetCurrentPen;
            CellViewModel.onPenModify += ModifyCurrentPen;
            CellViewModel.onPenSubstitute += SubstituteCurrentPen;
            CellViewModel.onGetCurrentPenColour += GetCurrentPenColour;
            CellViewModel.onGetCurrentPenNum += GetCurrentPenNum;
        }

        #endregion

        #region Properties

        public IPalette Palette { get; }
        public IDynamicGridViewModel EditPaletteModel { get; set; }
        public IDynamicGridViewModel PaletteModel { get; set; }
        public IDynamicGridViewModel AltPaletteModel { get; set; }
        private IDynamicGridViewModel _spriteModel;

        /// <summary>
        /// Model for the main design grid
        /// </summary>
        public IDynamicGridViewModel SpriteModel
        {
            get { return _spriteModel; }
            set { SetProperty(ref _spriteModel, value); }
        }

        private Color _currentPenColour;
        /// <summary>
        /// Current pen colour i.e. selection in Palette
        /// </summary>
        [DefaultValue(typeof(Color), "#FF808080")]
        public Color CurrentPenColour
        {
            get { return _currentPenColour; }
            set { SetProperty(ref _currentPenColour, value); }
        }

        private int _currentPenNum;
        /// <summary>
        /// Current pen number. Setting this updates the current pen colour from the palette, which notifies bound controls
        /// </summary>
        public int CurrentPenNum
        {
            get { return _currentPenNum; }
            set
            {
                SetProperty(ref _currentPenNum, value);
                CurrentPenColour = Palette.GetColour(_currentPenNum);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initialize this master model. Set up the palette model with colours from the palette, and the EditPalette model
        /// with all the system colours. 
        /// </summary>
        public void Initialize()
        {
            // Set up the Palette model
            for (int cellNum = 0; cellNum < Palette.NumPens; cellNum++)
            {
                PaletteModel.Cells[0][cellNum].Cell.CellColour = Palette.GetColour(cellNum);
                PaletteModel.Cells[0][cellNum].Cell.PenNum = cellNum;

                AltPaletteModel.Cells[0][cellNum].Cell.CellColour = Palette.GetColour(cellNum);
                AltPaletteModel.Cells[0][cellNum].Cell.PenNum = cellNum;
            }

            // Set up the Edit Palette model
            EditPaletteModel = new DynamicGridViewModel("ModifyCurrentPen") { GridWidth = 9, GridHeight = 3 };
            for (int rowNum = 0; rowNum < 3; rowNum++)
                for (int colNum = 0; colNum < 9; colNum++)
                {
                    EditPaletteModel.Cells[rowNum][colNum].Cell.CellColour = Palette.GetSystemColour((rowNum * 9) + colNum);
                    // EditPaletteModel.Cells[rowNum][colNum].Cell.PenNum = (rowNum * 9) + colNum; // Slightly naughty, we use Pen num here to store colour number
                }

            SetCurrentPen(0);
            PaletteModel.Cells[0][0].Cell.State = true;
            AltPaletteModel.Cells[0][0].Cell.State = true;
            InitializeSpriteModel();
        }

        /// <summary>
        /// Blank the current sprite model (set all cells to current Pen)
        /// </summary>
        public void InitializeSpriteModel()
        {
            foreach (var row in SpriteModel.Cells)
                foreach (var cellModel in row)
                {
                    cellModel.Cell.PenNum = _currentPenNum;
                    cellModel.Cell.CellColour = _currentPenColour;
                }

            SpriteModel.UpdateImageSource();
        }

        /// <summary>
        /// Update colour of all cells in SpriteModel, for example when new palette is loaded 
        /// </summary>
        public void UpdateSpriteModel()
        {
            foreach (var row in SpriteModel.Cells)
                foreach (var cellModel in row)
                {
                    var penColour = Palette.GetColour(cellModel.Cell.PenNum);
                    cellModel.Cell.CellColour = penColour;
                }
            CurrentPenColour = Palette.GetColour(_currentPenNum);
        }

        /// <summary>
        /// Method to return pen number, because we can't attach a property to an event *** MJB *** check this ! 
        /// </summary>
        /// <returns></returns>
        public int GetCurrentPenNum() { return _currentPenNum; }

        /// <summary>
        /// Provides the current pen colour to a clicked cell, when set up to call FillCell
        /// </summary>
        /// <returns></returns>
        public Color GetCurrentPenColour() { return CurrentPenColour; }

        /// <summary>
        /// Called when a cell in the Palette gets clicked. Set the current Pen number, which also updates current pen colour, which 
        /// in turn notifies bound controls. Also resets the state of all cells, so border is not displayed. 
        /// </summary>
        /// <param name="penColour"></param>
        public void SetCurrentPen(int penNum)
        {
            // Clear the state of all cells 
            foreach (var cellViewModel in PaletteModel.Cells[0]) cellViewModel.Cell.State = false;
            CurrentPenNum = penNum;
        }

        /// <summary>
        /// Switch all cells painted with the current Pen to a different Pen
        /// </summary>
        public void SubstituteCurrentPen(int newPenNum)
        {
            foreach (var row in SpriteModel.Cells)
                foreach (var cellModel in row)
                    if (cellModel.Cell.PenNum == _currentPenNum)
                    {
                        cellModel.Cell.PenNum = newPenNum;
                        cellModel.Cell.CellColour = Palette.GetColour(newPenNum);
                    }
            SpriteModel.UpdateImageSource();
        }

        /// <summary>
        /// Change the Ink in the current Pen 
        /// </summary>
        /// <param name="penColour"></param>
        public void ModifyCurrentPen(Color penColour)
        {
            PaletteModel.Cells[0][_currentPenNum].Cell.CellColour = penColour;
            AltPaletteModel.Cells[0][_currentPenNum].Cell.CellColour = penColour;
            Palette.SetInk(_currentPenNum, penColour);
            CurrentPenColour = penColour;
            // Change all cells in the SpriteModel with the current pen number to the new colour
            foreach (var row in SpriteModel.Cells)
                foreach (var cellModel in row)
                    if (cellModel.Cell.PenNum == _currentPenNum) cellModel.Cell.CellColour = penColour;

            SetDefaultPalette();
            SpriteModel.UpdateImageSource();
        }

        public void RotateClockwise()
        {
            var newSpriteModel = new DynamicGridViewModel("FillCell")
            {
                GridWidth = SpriteModel.GridHeight,
                GridHeight = SpriteModel.GridWidth
            };

            for (var y = 0; y < SpriteModel.GridHeight; y++)
                for (var x = 0; x < SpriteModel.GridWidth; x++)
                {
                    newSpriteModel.Cells[x][SpriteModel.GridHeight - y - 1].Cell.CellColour = SpriteModel.Cells[y][x].Cell.CellColour;
                    newSpriteModel.Cells[x][SpriteModel.GridHeight - y - 1].Cell.PenNum = SpriteModel.Cells[y][x].Cell.PenNum;
                }

            SpriteModel = newSpriteModel;
            SpriteModel.UpdateImageSource();
        }

        public void RotateAnticlockwise()
        {
            var newSpriteModel = new DynamicGridViewModel("FillCell")
            {
                GridWidth = SpriteModel.GridHeight,
                GridHeight = SpriteModel.GridWidth
            };

            for (var y = 0; y < SpriteModel.GridHeight; y++)
                for (var x = 0; x < SpriteModel.GridWidth; x++)
                {
                    newSpriteModel.Cells[SpriteModel.GridWidth - x - 1][y].Cell.CellColour = SpriteModel.Cells[y][x].Cell.CellColour;
                    newSpriteModel.Cells[SpriteModel.GridWidth - x - 1][y].Cell.PenNum = SpriteModel.Cells[y][x].Cell.PenNum;
                }

            SpriteModel = newSpriteModel;
            SpriteModel.UpdateImageSource();
        }

        public void FlipHorizontal()
        {
            var newSpriteModel = new DynamicGridViewModel("FillCell")
            {
                GridWidth = SpriteModel.GridWidth,
                GridHeight = SpriteModel.GridHeight
            };

            for (var y = 0; y < SpriteModel.GridHeight; y++)
                for (var x = 0; x < SpriteModel.GridWidth; x++)
                {
                    newSpriteModel.Cells[SpriteModel.GridHeight - y -1][x].Cell.CellColour = SpriteModel.Cells[y][x].Cell.CellColour;
                    newSpriteModel.Cells[SpriteModel.GridHeight - y - 1][x].Cell.PenNum = SpriteModel.Cells[y][x].Cell.PenNum;
                }

            SpriteModel = newSpriteModel;
            SpriteModel.UpdateImageSource();
        }

        public void FlipVertical()
        {
            var newSpriteModel = new DynamicGridViewModel("FillCell")
            {
                GridWidth = SpriteModel.GridWidth,
                GridHeight = SpriteModel.GridHeight
            };

            for (var y = 0; y < SpriteModel.GridHeight; y++)
                for (var x = 0; x < SpriteModel.GridWidth; x++)
                {
                    newSpriteModel.Cells[y][SpriteModel.GridWidth - x - 1].Cell.CellColour = SpriteModel.Cells[y][x].Cell.CellColour;
                    newSpriteModel.Cells[y][SpriteModel.GridWidth - x - 1].Cell.PenNum = SpriteModel.Cells[y][x].Cell.PenNum;
                }

            SpriteModel = newSpriteModel;
            SpriteModel.UpdateImageSource();
        }

        public void SetDefaultPalette() { Palette.SetDefaultPalette(); }

        public void AddRowAbove() { SpriteModel.AddRowAbove(_currentPenNum, _currentPenColour); SpriteModel.UpdateImageSource(); }
        public void AddRowBelow() { SpriteModel.AddRowBelow(_currentPenNum, _currentPenColour); SpriteModel.UpdateImageSource(); }
        public void AddColumnLeft() { SpriteModel.AddColumnLeft(_currentPenNum, _currentPenColour); SpriteModel.UpdateImageSource(); }
        public void AddColumnRight() { SpriteModel.AddColumnRight(_currentPenNum, _currentPenColour); SpriteModel.UpdateImageSource(); }

        public void RemoveTopRow() { SpriteModel.RemoveTopRow(); SpriteModel.UpdateImageSource(); }
        public void RemoveBottomRow() { SpriteModel.RemoveBottomRow(); SpriteModel.UpdateImageSource(); }
        public void RemoveLeftColumn() { SpriteModel.RemoveLeftColumn(); SpriteModel.UpdateImageSource(); }
        public void RemoveRightColumn() { SpriteModel.RemoveRightColumn(); SpriteModel.UpdateImageSource(); }

        #endregion
    }
}
