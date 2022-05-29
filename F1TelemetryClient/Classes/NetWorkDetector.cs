using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace F1TelemetryApp.Classes
{
    internal class NetWorkDetector
    {
        public static List<Dictionary<string, string>> Devices = new List<Dictionary<string, string>>();


        private static string NetworkGateway()
        {
            string ip = null;

            foreach (NetworkInterface f in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (f.OperationalStatus == OperationalStatus.Up)
                {
                    foreach (GatewayIPAddressInformation d in f.GetIPProperties().GatewayAddresses)
                    {
                        ip = d.Address.ToString();
                    }
                }
            }

            return ip;
        }

        internal static List<Dictionary<string, string>> Ping_all()
        {
            Devices.Clear();
            NetWorkDetector.TryAppendClient(
                    IPAddress.Any.ToString(),
                    "Any IP",
                    ""
                );
            NetWorkDetector.TryAppendClient(
                    "127.0.0.1",
                    "Localhost"
                );

            string gate_ip = NetworkGateway();

            //Extracting and pinging all other ip's.
            string[] array = gate_ip.Split('.');
            var t = new List<Task>();

            for (int i = 0; i <= 255; i++)
            {
                string ping_var = array[0] + "." + array[1] + "." + array[2] + "." + i;

                t.Add(Task.Run(delegate ()
                {
                    try
                    {
                        Ping ping = new Ping();
                        //ping.PingCompleted += new PingCompletedEventHandler(PingCompleted);
                        var reply = ping.Send(ping_var, 100);
                        if (reply != null && reply.Status == IPStatus.Success)
                        {
                            NetWorkDetector.TryAppendClient(ping_var);
                        }
                    }
                    catch
                    {
                        // Do nothing and let it try again until the attempts are exausted.
                        // Exceptions are thrown for normal ping failurs like address lookup
                        // failed.  For this reason we are supressing errors.
                    }
                }));
            }

            Task.WaitAll(t.ToArray());
            return Devices;
        }

        private static bool TryAppendClient(string ip, string hostname = "", string macaddress = "")
        {
            if (hostname == null || hostname == "") hostname = NetWorkDetector.GetHostName(ip);
            if (macaddress == null || macaddress == "") macaddress = NetWorkDetector.GetMacAddress(ip);

            var tmp = new Dictionary<string, string>();
            tmp.Add("IP", ip);
            tmp.Add("HostName", hostname);
            tmp.Add("MACAddress", macaddress);

            if (!NetWorkDetector.Devices.Contains(tmp))
            {
                NetWorkDetector.Devices.Add(tmp);
                return true;
            }
            else
            {
                return false;
            }
        }

        private static string GetHostName(string ipAddress)
        {
            try
            {
                IPHostEntry entry = Dns.GetHostEntry(ipAddress);
                if (entry != null)
                {
                    return entry.HostName;
                }
            }
            catch (SocketException)
            {
                // MessageBox.Show(e.Message.ToString());
            }

            return null;
        }


        //Get MAC address
        private static string GetMacAddress(string ipAddress)
        {
            string macAddress = string.Empty;
            System.Diagnostics.Process Process = new System.Diagnostics.Process();
            Process.StartInfo.FileName = "arp";
            Process.StartInfo.Arguments = "-a " + ipAddress;
            Process.StartInfo.UseShellExecute = false;
            Process.StartInfo.RedirectStandardOutput = true;
            Process.StartInfo.CreateNoWindow = true;
            Process.Start();
            string strOutput = Process.StandardOutput.ReadToEnd();
            string[] substrings = strOutput.Split('-');
            if (substrings.Length >= 8)
            {
                macAddress = substrings[3].Substring(Math.Max(0, substrings[3].Length - 2))
                         + "-" + substrings[4] + "-" + substrings[5] + "-" + substrings[6]
                         + "-" + substrings[7] + "-"
                         + substrings[8].Substring(0, 2);
                return macAddress;
            }

            else
            {
                return "OWN Machine";
            }
        }
    }
}
