using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;

namespace Workbench.Core.Models
{
    /// <summary>
    /// An row on a grid.
    /// </summary>
    [Serializable]
    public class TableRowModel : AbstractModel
    {
        private ObservableCollection<TableCellModel> cells;

        /// <summary>
        /// Initialize a row with cell data.
        /// </summary>
        /// <param name="cellData">Cell data.</param>
        public TableRowModel(params string[] cellData)
            : this()
        {
            foreach (var cellContent in cellData)
            {
                AddCell(new TableCellModel(cellContent));
            }
        }

        /// <summary>
        /// Initialize a row with default values.
        /// </summary>
        public TableRowModel()
        {
            this.cells = new ObservableCollection<TableCellModel>();
        }

        /// <summary>
        /// Gets or sets the row cells.
        /// </summary>
        public ObservableCollection<TableCellModel> Cells
        {
            get { return this.cells; }
            set
            {
                this.cells = value;
                OnPropertyChanged();
            }
        }

        public void AddCell(TableCellModel newCell)
        {
            this.cells.Add(newCell);
        }

        public IReadOnlyCollection<TableCellModel> GetCells()
        {
            return new ReadOnlyCollection<TableCellModel>(this.cells);
        }

        public IReadOnlyCollection<object> GetCellContent()
        {
            var accumulator = Cells.Select(aCell => aCell.Text)
                                   .Cast<object>()
                                   .ToList();

            return accumulator.AsReadOnly();
        }

        public TableCellModel GetCellAt(int index)
        {
            return this.cells[index];
        }

        public void UpdateCellsFrom(object[] rowItems)
        {
            var i = 0;
            foreach (var item in rowItems)
            {
                this.cells[i++].Text = Convert.ToString(item);
            }
        }
    }
}
