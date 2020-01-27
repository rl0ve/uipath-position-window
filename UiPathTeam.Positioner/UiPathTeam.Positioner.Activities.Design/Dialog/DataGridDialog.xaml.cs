using System;
using System.Activities;
using System.Activities.Presentation;
using System.Activities.Presentation.Model;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Activities.Expressions;

namespace UiPathTeam.Positioner.Activities.Design
{
    public partial class DataGridDialog : WorkflowElementDialog
    {
        public int startingX { get; set; }
        public int endingX { get; set; }
        public int startingY { get; set; }
        public int endingY { get; set; }
      
        public new ModelItem ModelItem { get; set; }
        public DataGrid myDesignerGrid { get; set; }
        public new EditingContext Context { get; set; }

        // Setup the grid that will be used for selection in the pop-up dialog
        public DataGridDialog(ModelItem modelItem, DataGrid designerGrid)
        {
            InitializeComponent();

            List<GridRow> myRow = new List<GridRow>()
            {
                new GridRow() { Column1 = "", Column2 = "", Column3 = "", Column4 = "", Column5="", Column6="" },
                new GridRow() { Column1 = "", Column2 = "", Column3 = "", Column4 = "", Column5="", Column6="" },
                new GridRow() { Column1 = "", Column2 = "", Column3 = "", Column4 = "", Column5="", Column6="" },
                new GridRow() { Column1 = "", Column2 = "", Column3 = "", Column4 = "", Column5="", Column6="" },
                new GridRow() { Column1 = "", Column2 = "", Column3 = "", Column4 = "", Column5="", Column6="" },
                new GridRow() { Column1 = "", Column2 = "", Column3 = "", Column4 = "", Column5="", Column6="" },
            };

            dgSimple.ItemsSource = myRow;

            this.ModelItem = modelItem;
            this.Context = ModelItem.GetEditingContext();

            this.myDesignerGrid = designerGrid;
        }

        // Setup helper method used to obtain literal values from ModelItem
        public static int GetImmediateValue(InArgument<int> argument)
        {
            Activity myActivity = argument.Expression;
            Literal<int> myLiteral = myActivity as Literal<int>;
       
            return myLiteral.Value;
        }

        // Whenever the pop-up is loaded, show the correct cells selected (i.e., highlighted) based on the current InArguments
        private void dgSimple_Loaded(object sender, RoutedEventArgs e)
        {


            try
            {
                int firstX = GetImmediateValue(ModelItem.Properties["FirstX"].Value.GetCurrentValue() as InArgument<int>);
                int firstY = GetImmediateValue(ModelItem.Properties["FirstY"].Value.GetCurrentValue() as InArgument<int>);
                int lastX = GetImmediateValue(ModelItem.Properties["LastX"].Value.GetCurrentValue() as InArgument<int>);
                int lastY = GetImmediateValue(ModelItem.Properties["LastY"].Value.GetCurrentValue() as InArgument<int>);

                if (firstX >= 0)
                {
                    for (int x = firstX; x <= lastX; x++)
                    {
                        for (int y = firstY; y <= lastY; y++)
                        {
                            dgSimple.CurrentCell = new DataGridCellInfo(dgSimple.Items[y], dgSimple.Columns[x]);
                            dgSimple.SelectedCells.Add(dgSimple.CurrentCell);
                        }
                    }

                }
            }
            catch
            {
                Console.WriteLine("Position has not been set yet");
            }
        }

        // On close, set the InArgument Values, for whatever cells were selected
        protected override void OnWorkflowElementDialogClosed(bool? dialogResult)
        {
            if (dialogResult.Value == true)
            {
                IList<DataGridCellInfo> selectedCells = dgSimple.SelectedCells;
                List<int> xValues = new List<int>();
                List<int> yValues = new List<int>();

                myDesignerGrid.UnselectAllCells();

                foreach (DataGridCellInfo cell in selectedCells)
                {
                    xValues.Add(cell.Column.DisplayIndex);
                    yValues.Add(dgSimple.Items.IndexOf(cell.Item));

                    DataGridCellInfo myCell = new DataGridCellInfo(myDesignerGrid.Items[dgSimple.Items.IndexOf(cell.Item)], myDesignerGrid.Columns[cell.Column.DisplayIndex]);
                    myDesignerGrid.SelectedCells.Add(myCell);
                }

                // Set the InArguments according to min/max values of what was selected in the grid
                startingX = xValues.Min();
                startingY = yValues.Min();
                endingX = xValues.Max();
                endingY = yValues.Max();

                ModelItem.Properties["FirstX"].SetValue(new InArgument<int>(startingX));
                ModelItem.Properties["FirstY"].SetValue(new InArgument<int>(startingY));
                ModelItem.Properties["LastX"].SetValue(new InArgument<int>(endingX));
                ModelItem.Properties["LastY"].SetValue(new InArgument<int>(endingY));
            }
        }
    }

    // Setup a class specifying the columns for the grid in the pop-up dialog
    public class GridRow
    {
        public string Column1 { get; set; }
        public string Column2 { get; set; }
        public string Column3 { get; set; }
        public string Column4 { get; set; }
        public string Column5 { get; set; }
        public string Column6 { get; set; }

    }

}