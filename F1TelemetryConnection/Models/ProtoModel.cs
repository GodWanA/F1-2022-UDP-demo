using System;
using System.Diagnostics;

namespace F1Telemetry.Models
{
    public abstract class ProtoModel : IDisposable, ICloneable
    {
        private bool disposedValue;

        public ProtoModel() { }

        public int Index { get; protected set; }
        public bool Supported { get; protected set; } = true;

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        /// <summary>
        /// Select reader by packetformat.
        /// </summary>
        /// <param name="format">packetformat</param>
        /// <param name="index">startindex</param>
        /// <param name="array">raw data</param>
        protected void PickReader(int format, byte[] array)
        {
            try
            {
                this.Supported = true;
                switch (format)
                {
                    default:
                        throw new InvalidOperationException("Unsopported packet format!");
                    case 2018:
                        this.Reader2018(array);
                        break;
                    case 2019:
                        this.Reader2019(array);
                        break;
                    case 2020:
                        this.Reader2020(array);
                        break;
                    case 2021:
                        this.Reader2021(array);
                        break;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                this.Supported = false;
            }
        }

        /// <summary>
        /// Reads 2018 games data formats from specific startindex.
        /// </summary>
        /// <param name="array">raw data array</param>
        protected virtual void Reader2018(byte[] array)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Reads 2019 games data formats from specific startindex.
        /// </summary>
        /// <param name="array">raw data array</param>
        protected virtual void Reader2019(byte[] array)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Reads 2020 games data formats from specific startindex.
        /// </summary>
        /// <param name="array">raw data array</param>
        protected virtual void Reader2020(byte[] array)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Reads 2021 games data formats from specific startindex.
        /// </summary>
        /// <param name="array">raw data array</param>
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
