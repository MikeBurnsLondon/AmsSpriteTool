using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Media;
using Ikc5.TypeLibrary;
using WpfApplication.Models;
using System.Windows.Input;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace WpfApplication.ViewModels
{
    public class DynamicGridViewModel : BaseNotifyPropertyChanged, IDynamicGridViewModel
    {
        // BMP and DIB header template for BMP data block
        static byte[] bmpDibHeader = new byte[54] { 0x42, 0x4D, 0, 0, 0, 0, 0, 0, 0, 0, 0x36, 0, 0, 0,            // BMP Header Update: bytes 2-5 with file size
                                                    0x28, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0x18, 0, 0, 0,   // DIB Header
                                                    0, 0, 0, 0, 0, 0, 0x13, 0x0B, 0, 0, 0x13, 0x0B, 0, 0, 0, 0,   // Update: bytes 18-21 pixel width, 22-24 pixel height, 34-37 size of raw bitmap data
                                                    0, 0, 0, 0, 0, 0};

        #region Constructors

        public DynamicGridViewModel() : this(null) { }
        public DynamicGridViewModel(string cellClickCommandName)
        {
            _cellClickCmdName = cellClickCommandName;
            
            this.SetDefaultValues();

            CellViewModel.onFillCell += CellFilled;
        }

        #endregion  

        #region Grid initialization/resize

        /// <summary>
        /// Create 2-dimensional array of cells. Each cell will know its own row/column number within the grid.
        /// </summary>
        /// <returns></returns>
        private ObservableCollection<ObservableCollection<ICellViewModel>> CreateCells()
        {
            var cells = new ObservableCollection<ObservableCollection<ICellViewModel>>();
            for (var posRow = 0; posRow < GridHeight; posRow++)
            {
                var row = new ObservableCollection<ICellViewModel>();
                for (var posCol = 0; posCol < GridWidth; posCol++)
                {
                    var cellViewModel = new CellViewModel(posRow, posCol, Cell.Empty, _cellClickCmdName);
                    row.Add(cellViewModel);
                }
                cells.Add(row);
            }
            return cells;
        }

        /// <summary>
        /// Add a row top the top of the grid. Note if you are using ThisCellRow (used for Palette grid), this method will break it - you 
        /// would need to correct all the values
        /// </summary>
        /// <param name="currentPenNum"></param>
        /// <param name="currentPenColour"></param>
        public void AddRowAbove(int currentPenNum, Color currentPenColour)
        {
            var row = new ObservableCollection<ICellViewModel>();
            for (var posCol = 0; posCol < GridWidth; posCol++)
            {
                var cellViewModel = new CellViewModel(0, posCol, Cell.Empty, _cellClickCmdName);
                cellViewModel.Cell.CellColour = currentPenColour;
                cellViewModel.Cell.PenNum = currentPenNum;
                row.Add(cellViewModel);
            }
            Cells.Insert(0, row);
            SetProperty(ref _gridHeight, _gridHeight + 1, "GridHeight");
        }

        /// <summary>
        /// Add a row top the botton of the grid. Note if you are using ThisCellRow (used for Palette grid), this method will break it - you 
        /// would need to correct all the values
        /// </summary>
        /// <param name="currentPenNum"></param>
        /// <param name="currentPenColour"></param>
        public void AddRowBelow(int currentPenNum, Color currentPenColour)
        {
            var row = new ObservableCollection<ICellViewModel>();
            for (var posCol = 0; posCol < GridWidth; posCol++)
            {
                var cellViewModel = new CellViewModel(0, posCol, Cell.Empty, _cellClickCmdName);
                cellViewModel.Cell.CellColour = currentPenColour;
                cellViewModel.Cell.PenNum = currentPenNum;
                row.Add(cellViewModel);
            }
            Cells.Add(row);
            SetProperty(ref _gridHeight, _gridHeight + 1, "GridHeight");
        }

        /// <summary>
        /// Add 2 columns to the left of the grid (note this is for Mode 0 only). Note if you are using ThisCellRow (used for Palette grid), 
        /// this method will break it - you would need to correct all the values
        /// </summary>
        /// <param name="currentPenNum"></param>
        /// <param name="currentPenColour"></param>
        public void AddColumnLeft(int currentPenNum, Color currentPenColour)
        {
            for (int colCount=1; colCount > -1; colCount--) // In Mode 0 insert columns 2 at a time
                foreach(var row in Cells)
                {
                    var cellViewModel = new CellViewModel(0, colCount, Cell.Empty, _cellClickCmdName);
                    cellViewModel.Cell.CellColour = currentPenColour;
                    cellViewModel.Cell.PenNum = currentPenNum;
                    row.Insert(0, cellViewModel);
                }
            SetProperty(ref _gridWidth, _gridWidth + 2, "GridWidth");
        }

        /// <summary>
        /// Add 2 columns to the left of the grid (note this is for Mode 0 only). Note if you are using ThisCellRow (used for Palette grid), 
        /// this method will break it - you would need to correct all the values
        /// </summary>
        /// <param name="currentPenNum"></param>
        /// <param name="currentPenColour"></param>
        public void AddColumnRight(int currentPenNum, Color currentPenColour)
        {
            for (int colCount = _gridWidth; colCount < _gridWidth+2; colCount++) // In Mode 0 insert columns 2 at a time
                foreach (var row in Cells)
                {
                    var cellViewModel = new CellViewModel(0, colCount, Cell.Empty, _cellClickCmdName);
                    cellViewModel.Cell.CellColour = currentPenColour;
                    cellViewModel.Cell.PenNum = currentPenNum;
                    row.Add(cellViewModel);
                }
            SetProperty(ref _gridWidth, _gridWidth + 2, "GridWidth");
        }

        /// <summary>
        /// Remove top row of cells from grid. Note if you are using ThisCellRow (used for Palette grid), 
        /// this method will break it - you would need to correct all the values
        /// </summary>
        public void RemoveTopRow()
        {
            Cells.RemoveAt(0);
            SetProperty(ref _gridHeight, _gridHeight - 1, "GridHeight");
        }

        /// <summary>
        /// Remove bottom row of cells from grid. Note if you are using ThisCellRow (used for Palette grid), 
        /// this method will break it - you would need to correct all the values
        /// </summary>
        public void RemoveBottomRow()
        {
            Cells.RemoveAt(_gridHeight - 1);
            SetProperty(ref _gridHeight, _gridHeight - 1, "GridHeight");
        }

        /// <summary>
        /// Remove leftmost column of cells from grid. Note if you are using ThisCellRow (used for Palette grid), 
        /// this method will break it - you would need to correct all the values
        /// </summary>
        public void RemoveLeftColumn()
        {
            for (int colCount = 1; colCount > -1; colCount--) // In Mode 0 remove columns 2 at a time
                foreach (var row in Cells)
                {
                    row.RemoveAt(0);
                }
            SetProperty(ref _gridWidth, _gridWidth - 2, "GridWidth");
        }

        /// <summary>
        /// Remove rightmost column of cells from grid. Note if you are using ThisCellRow (used for Palette grid), 
        /// this method will break it - you would need to correct all the values
        /// </summary>
        public void RemoveRightColumn()
        {
            for (int colCount = 1; colCount > -1; colCount--) // In Mode 0 remove columns 2 at a time
                foreach (var row in Cells)
                {
                    row.RemoveAt(_gridWidth-2+colCount);
                }
            SetProperty(ref _gridWidth, _gridWidth - 2, "GridWidth");
        }

        #endregion

        #region IDynamicGridViewModel

        private int _gridWidth;                     // Grid width in cells (number of columns)
        private int _gridHeight;                    // Grid Height in cells (number of rows)

        private ImageSource _imageSource;           // Image source

        private int _borderPixels;                  // Border thickness in pixels
        private Color _borderColour;                // Cell border colour

        private string _cellClickCmdName;
        /// <summary>
        /// Name of the command to be called when a cell is clicked
        /// </summary>
        public string CellClickCommandName { get { return _cellClickCmdName; }}

        private ObservableCollection<ObservableCollection<ICellViewModel>> _cells;
        /// <summary>
        /// Collection of CellViewModel objects representing each cell in the grid
        /// </summary>
        public ObservableCollection<ObservableCollection<ICellViewModel>> Cells
        {
            get { return _cells; }
            set { SetProperty(ref _cells, value); }
        }

        /// <summary>
        /// Grid width (columns)
        /// </summary>
		[DefaultValue(5)]
		public int GridWidth
		{
			get { return _gridWidth; }
			set
			{
				var oldValue = _gridWidth;
				SetProperty(ref _gridWidth, value);

				if (oldValue != value) Cells = CreateCells();
			}
		}

        /// <summary>
        /// Grid height (rows)
        /// </summary>
		[DefaultValue(5)]
		public int GridHeight
		{
			get { return _gridHeight; }
			set
			{
				var oldValue = _gridHeight;
				SetProperty(ref _gridHeight, value);

				if (oldValue != value) Cells = CreateCells();
			}
		}

        public ImageSource ImageSource
        {
            get { return _imageSource; }
            set
            {
                SetProperty(ref _imageSource, value);
            }
        }

        /// <summary>
        /// Thickness of border in pixels
        /// </summary>
        [DefaultValue(typeof(int), "0")]
        public int BorderPixels
        {
            get { return _borderPixels; }
            set { SetProperty(ref _borderPixels, value); }
        }

        /// <summary>
        /// Color of borders around cells.
        /// </summary>
        [DefaultValue(typeof(Color), "#FF808080")]
		public Color BorderColour
		{
			get { return _borderColour; }
			set { SetProperty(ref _borderColour, value); }
		}

        #endregion

        public void CellFilled(int rowNum, int colNum, Color colourNum)
        {
            // *** Just temporary, do it less bluntly that recreating the whole BMP array
            UpdateImageSource();
        }

        public void UpdateImageSource() 
        {
            // Pixel format RGB24. Pixel rows are padded for 4-byte alignment

            var headerSize = bmpDibHeader.Length;
            var rowDataBytes = (GridWidth * 3 * 2);                                                         // *** Doubled for Mode 0
            var rowPaddingBytes = (4-(rowDataBytes % 4))%4;
            var numDataBytes = GridHeight * (rowDataBytes + rowPaddingBytes);
            var numFileBytes = headerSize + numDataBytes;

            var bmpArray = new byte[numFileBytes];
            Buffer.BlockCopy(bmpDibHeader, 0, bmpArray, 0, headerSize);

            var pi = BitConverter.GetBytes(numFileBytes);
            bmpArray[2] = pi[0]; bmpArray[3] = pi[1]; bmpArray[4] = pi[2]; bmpArray[5] = pi[3];
            pi = BitConverter.GetBytes(GridWidth * 2);                                                      // *** Doubled for Mode 0
            bmpArray[18] = pi[0]; bmpArray[19] = pi[1]; bmpArray[20] = pi[2]; bmpArray[21] = pi[3];
            pi = BitConverter.GetBytes(GridHeight);
            bmpArray[22] = pi[0]; bmpArray[23] = pi[1]; bmpArray[24] = pi[2]; bmpArray[25] = pi[3];
            pi = BitConverter.GetBytes(numDataBytes);
            bmpArray[34] = pi[0]; bmpArray[35] = pi[1]; bmpArray[36] = pi[2]; bmpArray[37] = pi[3];

            // Loop traverses top to bottom, but bytes will be writtem bottom to top.
            // So, first pixel, is 0,0 in the cell array. This needs to be written 

            for (var y = 0; y < GridHeight; y++)        // Loop traverses top to bottom, but bytes will be writtem bottom to top
            { 
                for (var x = 0; x < GridWidth; x++) 
                {
                    var targetOffset = headerSize + ((GridHeight - y - 1)*((GridWidth*3*2)+rowPaddingBytes)) + (x*3*2); // *** GridWith and x Doubled for Mode 0
                    // Colour order is Blue, Green, Red
                    bmpArray[targetOffset] = _cells[y][x].Cell.CellColour.B;
                    bmpArray[targetOffset+1] = _cells[y][x].Cell.CellColour.G;
                    bmpArray[targetOffset+2] = _cells[y][x].Cell.CellColour.R;

                    bmpArray[targetOffset + 3] = _cells[y][x].Cell.CellColour.B;
                    bmpArray[targetOffset + 4] = _cells[y][x].Cell.CellColour.G;
                    bmpArray[targetOffset + 5] = _cells[y][x].Cell.CellColour.R;
                } 
            }

            using (var ms = new System.IO.MemoryStream(bmpArray))
            {
                var image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad; 
                image.StreamSource = ms;
                image.EndInit();

                ImageSource = image;
            }
        }
    }
}
