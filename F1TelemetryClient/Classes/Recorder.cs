using F1Telemetry.Helpers;
using F1Telemetry.Models;
using F1Telemetry.Models.EventPacket;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace F1TelemetryApp.Classes
{
    internal class Recorder : IDisposable, IConnectUDP
    {
        private bool disposedValue;
        private Dictionary<DateTime, byte[]> protoPacket;
        private bool isAdding;

        public Recorder()
        {
            this.protoPacket = new Dictionary<DateTime, byte[]>();
            this.SubscribeUDPEvents();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    this.UnsubscribeUDPEvents();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~Recorder()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public void SubscribeUDPEvents()
        {
            if (u.Connention != null)
            {
                u.Connention.RawDataRecieved += Connention_RawDataRecieved;
                u.Connention.EventPacket += Connention_EventPacket;
            }
        }

        private void Connention_EventPacket(PacketEventData packet, EventArgs e)
        {
            if (packet.EventType == Appendences.EventTypes.SessionEnded) SaveRawData();
        }

        public void UnsubscribeUDPEvents()
        {
            if (u.Connention != null)
            {
                u.Connention.RawDataRecieved += Connention_RawDataRecieved;
            }
        }

        private void Connention_RawDataRecieved(byte[] rawData, PacketHeader head, EventArgs e)
        {
            if (!this.isAdding)
            {
                this.isAdding = true;
                this.protoPacket.Add(DateTime.Now, rawData);
                this.isAdding = false;
            }
        }

        private void SaveRawData()
        {
            string path = "Saves\\";

            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            using (var memStream = new MemoryStream())
            using (var fileStream = new FileStream(path + DateTime.Now.ToString("yyyyMMddHHmmss") + ".f1t", FileMode.Create))
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(memStream, this.protoPacket);
                fileStream.Write(Recorder.Compress(memStream.ToArray()));

                fileStream.Flush();
                fileStream.Close();

                this.protoPacket.Clear();

                GC.WaitForFullGCApproach();
                GC.WaitForPendingFinalizers();
                GC.WaitForFullGCComplete();
                GC.Collect();
            }
        }

        public static byte[] Compress(byte[] data)
        {
            byte[] compressArray = null;
            try
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (DeflateStream deflateStream = new DeflateStream(memoryStream, CompressionMode.Compress))
                    {
                        deflateStream.Write(data, 0, data.Length);
                    }
                    compressArray = memoryStream.ToArray();
                }
            }
            catch (Exception exception)
            {
                // do something !
            }
            return compressArray;
        }

        public static byte[] Decompress(byte[] data)
        {
            byte[] decompressedArray = null;
            try
            {
                using (MemoryStream decompressedStream = new MemoryStream())
                {
                    using (MemoryStream compressStream = new MemoryStream(data))
                    {
                        using (DeflateStream deflateStream = new DeflateStream(compressStream, CompressionMode.Decompress))
                        {
                            deflateStream.CopyTo(decompressedStream);
                        }
                    }
                    decompressedArray = decompressedStream.ToArray();
                }
            }
            catch (Exception exception)
            {
                // do something !
            }

            return decompressedArray;
        }
    }
}
