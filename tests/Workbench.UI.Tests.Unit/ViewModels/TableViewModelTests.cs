﻿using System.Threading;
using NUnit.Framework;
using Workbench.Core.Models;
using Workbench.ViewModels;

namespace Workbench.UI.Tests.Unit.ViewModels
{
    [TestFixture]
    [Apartment(ApartmentState.STA)]
    public class TableViewModelTests
    {
        [Test]
        public void GetRowDataFromTableReturnsExpectedValue()
        {
            var theGridModel = new TableModel();
            var sut = new TableViewModel(theGridModel);
            sut.AddColumn(new TableColumnModel("X"));
            sut.AddColumn(new TableColumnModel("Y"));
            sut.AddColumn(new TableColumnModel("Z"));
            sut.AddRow(new TableRowModel("1", "2", "3"));
            sut.AddRow(new TableRowModel("4", "5", "6"));
            sut.AddRow(new TableRowModel("7", "8", "9"));

            var actualRow = sut.GetRowAt(1);
            Assert.That(actualRow.Cells[2].Text, Is.EqualTo("6"));
        }

        [Test]
        public void GetRowDataFromDefaultTableReturnsExpectedValue()
        {
            var sut = new TableViewModel(TableModel.Default);

            var actualRow = sut.GetRowAt(1);
            Assert.That(actualRow.Cells[1].Text, Is.Empty);
        }
    }
}
