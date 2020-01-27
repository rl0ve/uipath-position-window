using System;
using System.Activities;
using System.Activities.Expressions;
using System.Collections.Generic;
using System.Windows.Controls;

namespace UiPathTeam.Positioner.Activities.Design
{
    /// <summary>
    /// Interaction logic for ChildActivityDesigner.xaml
    /// </summary>
    public partial class PositionWindowDesigner
    {
        public PositionWindowDesigner()
        {
            InitializeComponent();

            // Setup the grid that will be shown on the designer itself
            List<DesignerGridRow> myRow = new List<DesignerGridRow>()
            {
                new DesignerGridRow() { Column1 = "", Column2 = "", Column3 = "", Column4 = "", Column5="", Column6="" },
                new DesignerGridRow() { Column1 = "", Column2 = "", Column3 = "", Column4 = "", Column5="", Column6="" },
                new DesignerGridRow() { Column1 = "", Column2 = "", Column3 = "", Column4 = "", Column5="", Column6="" },
                new DesignerGridRow() { Column1 = "", Column2 = "", Column3 = "", Column4 = "", Column5="", Column6="" },
                new DesignerGridRow() { Column1 = "", Column2 = "", Column3 = "", Column4 = "", Column5="", Column6="" },
                new DesignerGridRow() { Column1 = "", Column2 = "", Column3 = "", Column4 = "", Column5="", Column6="" },
            };
            designerGrid.ItemsSource = myRow;

        }

        // On Click of "Set Position" open a dialog box
        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            DataGridDialog myDataGridWindow = new DataGridDialog(ModelItem, designerGrid);
            myDataGridWindow.Show();
        }

        // Setup helper method used to obtain literal values from ModelItem
        public static int GetImmediateValue(InArgument<int> argument)
        {
            Activity myActivity = argument.Expression;
            Literal<int> myLiteral = myActivity as Literal<int>;

            return myLiteral.Value;
        }

        // Whenever the designer comes into view, programatically select (i.e., highlight) the appropriate cells in the grid shown on the designer - based on the InArguments (which are set using the pop-up dialog)
        private void designerGrid_Loaded(object sender, System.Windows.RoutedEventArgs e)
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
                            designerGrid.CurrentCell = new DataGridCellInfo(designerGrid.Items[y], designerGrid.Columns[x]);
                            designerGrid.SelectedCells.Add(designerGrid.CurrentCell);
                        }
                    }

                }
            }
            catch
            {
                Console.WriteLine("Position has not been set yet");
            }
        }
    }

    // Setup a class specifying the columns for the grid that will be shown on the designer
    public class DesignerGridRow
    {
        public string Column1 { get; set; }
        public string Column2 { get; set; }
        public string Column3 { get; set; }
        public string Column4 { get; set; }
        public string Column5 { get; set; }
        public string Column6 { get; set; }

    }
}
