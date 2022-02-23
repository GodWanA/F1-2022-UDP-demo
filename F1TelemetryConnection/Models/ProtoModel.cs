using System;

namespace F1Telemetry.Models
{
    public abstract class ProtoModel : IDisposable, ICloneable
    {
        private bool disposedValue;

        public ProtoModel() { }

        public int Index { get; protected set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        //public void Dispose()
        //{
        //    GC.SuppressFinalize(this);
        //    GC.WaitForPendingFinalizers();
        //    GC.Collect();
        //}

        /// <summary>
        /// Select reader by packetformat.
        /// </summary>
        /// <param name="format">packetformat</param>
        /// <param name="index">startindex</param>
        /// <param name="array">raw data</param>
        protected void PickReader(int format, byte[] array)
        {
            switch (format)
            {
                default:
                    throw new InvalidOperationException("Unsopported packet format!");
                case 2021:
                    this.Reader2021(array);
                    break;
            }
        }

        /// <summary>
        /// Reads 2021 games data formats from specific startindex.
        /// </summary>
        /// <param name="array">raw data array</param>
        /// <param name="index">startindex</param>
        protected virtual void Reader2021(byte[] array)
        {
            throw new NotImplementedException();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)                    
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~ProtoModel()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            //GC.WaitForPendingFinalizers();
            GC.SuppressFinalize(this);
        }
    }
}
