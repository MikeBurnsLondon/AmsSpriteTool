using System.Windows.Input;
using Ikc5.TypeLibrary;
using WpfApplication.Models;
using System;
using System.Windows.Media;

namespace WpfApplication.ViewModels
{
	public class CellViewModel : BaseNotifyPropertyChanged, ICellViewModel
	{
        public delegate void SetPen(int penNum);                        // Used to set the current pen in the master model
        public delegate void ModifyPen(Color colour);                   // Used to change the ink in a pen in the master model
        public delegate void SubstitutePen(int penNum);

        public delegate Color GetCurrentPenColour();                    // Used to retrieve current pen colour from master model
        public delegate int GetCurrentPenNum();                         // Used to retrieve current pen number from master model

        public delegate void CellFilled(int rowNum, int colNum, Color cellColour);  // Used to inform the parent DynamicGridViewModel (when in "FillCell" mode i.e. the main design grid) that a cell has been modified 

        public static event SetPen onPenSet;                            
        public static event ModifyPen onPenModify;
        public static event SubstitutePen onPenSubstitute;

        public static event GetCurrentPenColour onGetCurrentPenColour;  
        public static event GetCurrentPenNum onGetCurrentPenNum;

        public static event CellFilled onFillCell;

        public int ThisCellRow { get; set; }
        public int ThisCellCol { get; set; }

		public CellViewModel(int rowNum, int colNum, ICell cell = null, string clickMethod = null)
		{
            ThisCellRow = rowNum;
            ThisCellCol = colNum;

            if (clickMethod != null)
            {
                if (clickMethod == "FillCell") CellClickCommand = new DelegateCommand(FillCell);
                if (clickMethod == "SetCurrentPen") CellClickCommand = new DelegateCommand(SetCurrentPen);  // Uses column number from Grid
                if (clickMethod == "ModifyCurrentPen") CellClickCommand = new DelegateCommand(ModifyCurrentPen);
                if (clickMethod == "SubstitutePen") CellClickCommand = new DelegateCommand(SubstituteCurrentPen);
            }
            if (cell != null) Cell = cell;
		}

		#region Cell model

		private ICell _cell;

		public ICell Cell
		{
			get { return _cell; }
			set { SetProperty(ref _cell, value); }
		}

        #endregion Cell model

        #region Commands

        public ICommand CellClickCommand { get; set; }

        #endregion

        #region Click Event Handlers

        internal void FillCell(object obj)
        {
            Cell.CellColour = onGetCurrentPenColour();
            Cell.PenNum = onGetCurrentPenNum();
            onFillCell(ThisCellRow, ThisCellCol, Cell.CellColour);
        }

        internal void SetCurrentPen(object obj)
        {
            // SetCurrentPen will clear the state of all cells 
            onPenSet((int)obj);
            // Then set this one to True, to display the border
            if (Cell != null) Cell.State = true;
        }

        internal void ModifyCurrentPen(object obj)
        {
            onPenModify(Cell.CellColour);
        }

        internal void SubstituteCurrentPen(object obj)
        {
            onPenSubstitute((int)obj);
        }

        #endregion
    }
}
