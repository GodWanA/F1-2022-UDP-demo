using System;

namespace F1Telemetry.Models
{
    public abstract class ProtoModel : IDisposable, ICloneable
    {
        public ProtoModel() { }

        public int Index { get; protected set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            GC.Collect();
        }

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
    }
}
