using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AttendanceTracker
{
    class Program
    {
        private List<String> OnlineDevices = new List<string>();
        private static int connectedDevices;

        public static List<Host> hosts;

        static void Main(string[] args)
        {
            try
            {
                while (true)
                {
                    Program p = new Program();
                    Console.BackgroundColor = ConsoleColor.Blue;
                    hosts = new List<Host>();
                    p.Ping_all();

                    Console.WriteLine("GOING TO SLEEP");
                    Console.BackgroundColor = ConsoleColor.DarkYellow;
                    Thread.Sleep(10000);
                    if (hosts.Count != 0)
                    {
                        HttpClient authClient = new HttpClient();

                        string stringPayload = JsonConvert.SerializeObject(hosts);

                        var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");

                        Console.WriteLine("JSON Content to Post = " + stringPayload);

                        HttpResponseMessage message = authClient.PostAsync(ConfigurationManager.AppSettings["StatusUpdateUrl"], httpContent).GetAwaiter().GetResult();

                        Console.WriteLine("Status Code : " + message.StatusCode);
                        Console.WriteLine("Status Result : " + message.Content.ReadAsStringAsync().Result);

                        Console.WriteLine("TOTAL CONNECTED DEVICES = " + connectedDevices);
                        connectedDevices = 0;
                    }
                    Thread.Sleep(300000);
                    Console.Clear();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);                
            }
            
        }

        string NetworkGateway()
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

        public void Ping_all()
        {

            string gate_ip = NetworkGateway();

            //Extracting and pinging all other ip's.
            string[] array = gate_ip.Split('.');

            for (int i = 2; i <= 255; i++)
            {

                string ping_var = array[0] + "." + array[1] + "." + array[2] + "." + i;

                //time in milliseconds           
                Ping(ping_var, 5, 5000);

            }

        }

        public void Ping(string host, int attempts, int timeout)
        {
            Thread thread = new Thread(delegate ()
            {
                for (int i = 0; i < attempts; i++)
                {

                    try
                    {
                        System.Net.NetworkInformation.Ping ping = new System.Net.NetworkInformation.Ping();
                        //ping.PingCompleted += new PingCompletedEventHandler(PingCompleted);
                        //Console.WriteLine(string.Format("Pinging : {0} || Attempt : {1}", host, (i + 1)));
                        PingReply reply = ping.Send(host, timeout);
                        if (reply.Status == IPStatus.Success)
                        {
                            string hostname = GetHostName(host);
                            string macaddres = GetMacAddress(host);
                            string[] arr = new string[3];

                            Host currentHost = new Host()
                            {
                                Id = macaddres==null?Guid.NewGuid().ToString():macaddres.Replace(":","").Replace("-",""),
                                HostName = hostname,
                                LastAccessedIP = host,
                                LastStatusChecked = DateTime.UtcNow,
                                Status = 1
                            };

                            hosts.Add(currentHost);

                            //store all three parameters to be shown on ListView
                            arr[0] = host;
                            arr[1] = hostname;
                            arr[2] = macaddres;
                            connectedDevices++;
                            //Console.WriteLine(string.Format("IP : {0}||Hostname : {1}||MAC Address : {2}\n==================", arr[0], arr[1], arr[2]));
                            attempts = 0;
                        }
                    }
                    catch (Exception ex)
                    {
                        // Do nothing and let it try again until the attempts are exausted.
                        // Exceptions are thrown for normal ping failurs like address lookup
                        // failed.  For this reason we are supressing errors.
                        Console.WriteLine(ex.Message);
                    }
                }
            });

            thread.Name = String.Format("Thread : {0}", host);
            //Console.WriteLine("Thread name : " + thread.Name);
            thread.Start();

        }

        private void PingCompleted(object sender, PingCompletedEventArgs e)
        {
            string ip = (string)e.UserState;
            if (e.Reply != null && e.Reply.Status == IPStatus.Success)
            {
                string hostname = GetHostName(ip);
                string macaddres = GetMacAddress(ip);
                string[] arr = new string[3];

                //store all three parameters to be shown on ListView
                arr[0] = ip;
                arr[1] = hostname;
                arr[2] = macaddres;
                connectedDevices++;
                Console.WriteLine(string.Format("IP : {0}||Hostname : {1}||MAC Address : {2}\n==================", arr[0], arr[1], arr[2]));
            }
            else
            {
                // MessageBox.Show(e.Reply.Status.ToString());
            }
        }

        public string GetHostName(string ipAddress)
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
        public string GetMacAddress(string ipAddress)
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
                foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
                {
                    // Only consider Ethernet network interfaces
                    if (nic.NetworkInterfaceType == NetworkInterfaceType.Ethernet &&
                        nic.OperationalStatus == OperationalStatus.Up)
                    {
                        return nic.GetPhysicalAddress().ToString();
                    }
                }
                return "OWN Machine";
            }
        }
    }
}
