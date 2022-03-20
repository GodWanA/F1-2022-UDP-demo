using System.Windows;
using System.Windows.Controls;

namespace F1TelemetryApp.Classes
{
    public interface IGridResize
    {
        public enum GridSizes
        {
            XS,
            XM,
            MD,
            LG,
            XL,
        }

        /// <summary>
        /// Smallest view.
        /// </summary>
        public void ResizeXS();
        /// <summary>
        /// small view.
        /// </summary>
        public void ResizeXM();
        /// <summary>
        /// medium view.
        /// </summary>
        public void ResizeMD();
        /// <summary>
        /// Large view.
        /// </summary>
        public void ResizeLG();
        /// <summary>
        /// extra large view.
        /// </summary>
        public void ResizeXL();

        /// <summary>
        /// Recalculate any UI element's position in grid.
        /// </summary>
        /// <param name="container">UI element</param>
        /// <param name="col">Column index</param>
        /// <param name="row">Row index</param>
        /// <param name="colSpan">Column span</param>
        /// <param name="rowSpan">Row span</param>
        internal static void SetGridSettings(UIElement container, int col, int row, int colSpan = 1, int rowSpan = 1)
        {
            if (colSpan < 1) colSpan = 1;
            if (rowSpan < 1) rowSpan = 1;

            Grid.SetColumn(container, col);
            Grid.SetColumnSpan(container, colSpan);
            Grid.SetRow(container, row);
            Grid.SetRowSpan(container, rowSpan);
        }
    }
}
