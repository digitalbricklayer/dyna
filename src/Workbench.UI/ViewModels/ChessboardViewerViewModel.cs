﻿using Workbench.Core.Models;

namespace Workbench.ViewModels
{
    public class ChessboardViewerViewModel : ViewerViewModel
    {
        private ChessboardViewModel board;

        public ChessboardViewerViewModel(ChessboardVisualizerModel theChessboardVisualizerModel) 
            : base(theChessboardVisualizerModel)
        {
            Model = theChessboardVisualizerModel;
            Board = new ChessboardViewModel(theChessboardVisualizerModel.Model);
        }

        public ChessboardViewModel Board
        {
            get { return this.board; }
            set
            {
                this.board = value;
                NotifyOfPropertyChange();
            }
        }
    }
}
