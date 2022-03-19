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
    }
}
