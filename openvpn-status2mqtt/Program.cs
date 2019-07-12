using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using JsonConfig;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace openvpnstatus2mqtt
{
    #region ConnectedDevice
    class connectedDevice
    {
        public String CommonName;
        public String RealAddress;
        public UInt64 BytesReceived;
        public UInt64 BytesSent;

        public connectedDevice(String _CommonName, String _RealAddress, String _BytesReceived, String _BytesSent)
        {
            CommonName = _CommonName;
            RealAddress = _RealAddress;
            BytesReceived = Convert.ToUInt64(_BytesReceived);
            BytesSent = Convert.ToUInt64(_BytesSent);
        }

        public connectedDevice(String _CommonName, String _RealAddress, UInt64 _BytesReceived, UInt64 _BytesSent)
        {
            CommonName = _CommonName;
            RealAddress = _RealAddress;
            BytesReceived = _BytesReceived;
            BytesSent = _BytesSent;
        }

    }
    #endregion

    class MainClass
    {
        private static ConsoleOutputLogger Logger;

        public static void Main(string[] args)
        {
            Logger = new ConsoleOutputLogger("", "openvpn-status2mqtt");
            Logger.verbose = true;
            Logger.writeLogfile = false;

            Logger.WriteLine("OpenVPN-Status 2 MQTT (C) 2019 Daniel Kirstenpfad");
            Logger.WriteLine("http://www.schrankmonster.de");

            // read in configuration and do some error checking
            if (!File.Exists("/configuration/openvpn-status2mqtt.json"))
            {
                Logger.WriteLine("Error: Could not find /configuration/openvpn-status2mqtt.json configuration file.");
                return;
            }

            var ConfigReader = new StreamReader("/configuration/openvpn-status2mqtt.json");

            dynamic Configuration = Config.ApplyJson(ConfigReader.ReadToEnd(), new ConfigObject());

            /*- startup:
             *      - connect to mqtt broker
             *      - look for status file
             *      - iterate
             *          - open file
             *          - read all text
             *          - interate by line
             */            

            Logger.WriteLine("Connecting to Host " + (String)Configuration.mqtt_broker_host + ":" + (Int32)Configuration.mqtt_broker_port);
            // create client instance 
            MqttClient client = new MqttClient(IPAddress.Parse((String)Configuration.mqtt_broker_host), (Int32)Configuration.mqtt_broker_port, false, null, null, MqttSslProtocols.None);
            String clientId = Guid.NewGuid().ToString();
            String MQTTCulture = (String)Configuration.mqtt_cultureinfo;
            Int32 WaitTime = (Int32)Configuration.openvpn_status_read_interval;
            String OpenVPNStatusFile = (String)Configuration.openvpn_status_log;
            String TopicPrefix = (String)Configuration.mqtt_topic_prefix;
            String Topic, Message;

            // start with a fresh canvas
            Dictionary<String, connectedDevice> ConnectedDevices = new Dictionary<string, connectedDevice>();
            Dictionary<String, connectedDevice> previousRunConnectedDevices = new Dictionary<string, connectedDevice>();

            connectedDevice tempDevice;

            // start read interval of openvpn-status log
            while (true)
            {
                #region MQTT connection handling
                if (!client.IsConnected)
                {
                    Logger.WriteLine("Connecting to MQTT...");
                    // connect
                    client.Connect(clientId);
                }

                // check connection status again...
                if (client.IsConnected)
                {
                    //Logger.WriteLine("MQTT successfully connected!");
                }
                else
                {
                    Logger.WriteLine("MQTT connection problem - could not connect!");
                }
                #endregion

                #region openvpn-status.log reading...
                //Logger.WriteLine("Reading...");
                String[] AllFileContent = File.ReadAllLines(OpenVPNStatusFile);

                // store the previous run devices and get a fresh set...
                previousRunConnectedDevices = ConnectedDevices;
                ConnectedDevices = new Dictionary<string, connectedDevice>();

                for (int i = 3; i <= AllFileContent.Length-1; i++)
                {
                    if (AllFileContent[i] != "ROUTING TABLE")
                    {
                        Logger.WriteLine(AllFileContent[i]);
                        // this is a hit, disect
                        string[] split = AllFileContent[i].Split(new Char[] { ',' });
                        if (split.Length > 3)
                        {
                            tempDevice = new connectedDevice(split[0], split[1], split[2], split[3]);
                            ConnectedDevices.Add(tempDevice.CommonName, new connectedDevice(tempDevice.CommonName, tempDevice.RealAddress, tempDevice.BytesReceived, tempDevice.BytesSent));
                        }
                    }
                    else
                        break;
                }

                // we got all data...now compare...

                // first: the disconnects... this means - a device in previousRunConnectedDevices but not in this runs...
                foreach (connectedDevice device in previousRunConnectedDevices.Values)
                {
                    if (!ConnectedDevices.ContainsKey(device.CommonName))
                    {
                        // the device has disconnected...
                        Topic = TopicPrefix + device.CommonName.Replace(' ', '_').Replace('-', '_').Replace('+', '_').Replace('#', '_');
                        Logger.WriteLine(device.CommonName + " disconnected");
                        // Online
                        Message = "0";
                        if (client.IsConnected)
                            client.Publish(Topic + "/status", Encoding.UTF8.GetBytes(Message), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
                    }
                }

                // second: the new connects and the updated...
                foreach (connectedDevice device in ConnectedDevices.Values)
                {
                    Topic = TopicPrefix + device.CommonName.Replace(' ', '_').Replace('-', '_').Replace('+', '_').Replace('#', '_');

                    if (previousRunConnectedDevices.ContainsKey(device.CommonName))
                    {
                        // updated...as it is in this run and was in previous run
                        //connectedDevice previousdevice = previousRunConnectedDevices[device.CommonName];
                    }
                    else
                    {
                        Logger.WriteLine(device.CommonName + " connected");
                        // Online
                        Message = "1";
                        if (client.IsConnected)
                            client.Publish(Topic + "/status", Encoding.UTF8.GetBytes(Message), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);

                    }

                    Logger.WriteLine(device.CommonName + " received " + device.BytesReceived + " bytes and sent " + device.BytesSent + " bytes.");

                    Message = Convert.ToString(device.BytesReceived);
                    if (client.IsConnected)
                        client.Publish(Topic + "/received", Encoding.UTF8.GetBytes(Message), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
                    Message = Convert.ToString(device.BytesSent);
                    if (client.IsConnected)
                        client.Publish(Topic + "/sent", Encoding.UTF8.GetBytes(Message), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);

                }

                // clean up what is left in the connecteddevices

                #endregion

                //Logger.WriteLine("...done");
                Thread.Sleep(WaitTime*1000);
            }
        }
    }
}
