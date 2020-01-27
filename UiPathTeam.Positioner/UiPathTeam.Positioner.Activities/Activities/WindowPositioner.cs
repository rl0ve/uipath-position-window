using PInvoke;
using System;
using System.Activities;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using UiPath.Core;
using UiPath.Core.Activities;
using UiPathTeam.Positioner.Activities.Properties;

namespace UiPathTeam.Positioner.Activities
{
    [LocalizedDisplayName(nameof(Resources.PositionWindowDisplayName))]
    [LocalizedDescription(nameof(Resources.PositionWindowDescription))]
    public class PositionWindow : BaseWindow
    {
        #region Properties
        protected int rowBegin;
        protected int rowFinal;
        protected int columnBegin;
        protected int columnFinal;

        [Browsable(false)]
        [LocalizedCategory(nameof(Resources.Input))]
        [RequiredArgument]
        public InArgument<int> FirstX { get; set; }

        [Browsable(false)]
        [LocalizedCategory(nameof(Resources.Input))]
        [RequiredArgument]
        public InArgument<int> FirstY { get; set; }

        [Browsable(false)]
        [LocalizedCategory(nameof(Resources.Input))]
        [RequiredArgument]
        public InArgument<int> LastX { get; set; }

        [Browsable(false)]
        [LocalizedCategory(nameof(Resources.Input))]
        [RequiredArgument]
        public InArgument<int> LastY { get; set; }
        #endregion

        #region Property Validation

        /// <summary>
        /// Validates properties at design-time.
        /// </summary>
        /// <param name="metadata"></param>
        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            // if (FirstX == null) metadata.AddValidationError(string.Format(Resources.MetadataValidationError));
            base.CacheMetadata(metadata);
        }

        #endregion

        #region Main Execution
        /// <summary>
        /// Runs the main logic of the activity. Has access to the context, 
        /// which holds the values of properties for this activity and those from the parent scope.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// 


        protected override Task ExecuteAsync(AsyncCodeActivityContext context, CancellationToken cancellationToken)
        {
            rowBegin = FirstY.Get(context) + 1;
            rowFinal = LastY.Get(context) + 1;
            columnBegin = FirstX.Get(context) + 1;
            columnFinal = LastX.Get(context) + 1;
            return base.ExecuteAsync(context, cancellationToken);
        }

        protected override void ExecuteWindowAction(Window window)
        {
            Region position = new UiElement(window).GetPosition();
            if (position.Rectangle.HasValue)
            {

                var grid = 6;

                // Determine the current screen width and height
                var screenHeight = Screen.PrimaryScreen.WorkingArea.Height;
                var screenWidth = Screen.PrimaryScreen.WorkingArea.Width;

                // Establish variables to hold the starting X or Y position
                var startingX = 1;
                var startingY = 1;

                // Set the starting and ending points (this logic allows for selections where the columnBegin is greater than the ColumnFinal. Only applicable if Input Arguments become browsable)
                if (columnBegin == 1)
                {
                    startingX = 0;
                }
                else if (columnFinal == 1)
                {
                    startingX = 0;
                }
                else if (columnBegin <= columnFinal)
                {
                    startingX = ((screenWidth / grid) * (columnBegin - 1));
                }
                else
                {
                    startingX = ((screenWidth / grid) * (columnFinal - 1));
                }

                // Set the starting and ending points (this logic allows for selections where the RowBegin is greater than the RowFinal. Only Applicable if Input Arguments Become Browsable)
                if (rowBegin == 1)
                {
                    startingY = 0;
                }
                else if (rowFinal == 1)
                {
                    startingY = 0;
                }
                else if (rowBegin <= rowFinal)
                {
                    startingY = ((screenHeight / grid) * (rowBegin - 1));
                }
                else
                {
                    startingY = ((screenHeight / grid) * (rowFinal - 1));
                }

                // Set the Width and Height of the new Screen
                var windowWidth = (Math.Abs(columnFinal - columnBegin) + 1) * (screenWidth / grid);
                var windowHeight = (Math.Abs(rowFinal - rowBegin) + 1) * (screenHeight / grid);

                int myBorderThickness = windowBorderThickness(window.Handle);
                window.Move(startingX - myBorderThickness, startingY, windowWidth + 2 * myBorderThickness, windowHeight + myBorderThickness);
            }
        }

        #endregion

        #region Helper Methods

        // Determine the window border thickness
        private int windowBorderThickness(IntPtr windowHandle)
        {
            UiPath.Core.Window myWindow = new Window(windowHandle);
            UiPath.Core.UiElement myElement = myWindow;

            RECT windowRect = GetWindowRect(windowHandle);
            RECT windowExtendedBounds = DwmGetWindowAttribute(windowHandle);
            int borderWidth = windowRect.right - windowExtendedBounds.right;

            return borderWidth;

        }

        // import DLLs for Win32 API
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);

        [DllImport("dwmapi.dll")]
        static extern int DwmGetWindowAttribute(IntPtr hwnd, int dwAttribute, out RECT pvAttribute, int cbAttribute);

        // determine the location of the Window
        public static RECT GetWindowRect(IntPtr hWnd)
        {
            RECT result;
            GetWindowRect(hWnd, out result);
            return result;
        }

        // determine the location of the window borders
        public static RECT DwmGetWindowAttribute(IntPtr hWnd)
        {
            RECT result;
            const int DWMWA_EXTENDED_FRAME_BOUNDS = 9;
            DwmGetWindowAttribute(hWnd, DWMWA_EXTENDED_FRAME_BOUNDS, out result, Marshal.SizeOf(typeof(RECT)));
            return result;
        }
    }
    #endregion
}