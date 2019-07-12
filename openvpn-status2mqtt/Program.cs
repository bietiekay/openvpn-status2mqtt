using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading;
using JsonConfig;
using uPLibrary.Networking.M2Mqtt;

namespace openvpnstatus2mqtt
{
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
            if (!File.Exists("openvpn-status2mqtt.json"))
            {
                Logger.WriteLine("Error: Could not find openvpn-status2mqtt.json configuration file.");
                return;
            }

            var ConfigReader = new StreamReader("openvpn-status2mqtt.json");

            dynamic Configuration = Config.ApplyJson(ConfigReader.ReadToEnd(), new ConfigObject());

            Logger.WriteLine("Connecting to Host " + (String)Configuration.mqtt_broker_host + ":" + (Int32)Configuration.mqtt_broker_port);
            // create client instance 
            MqttClient client = new MqttClient(IPAddress.Parse((String)Configuration.mqtt_broker_host), (Int32)Configuration.mqtt_broker_port, false, null, null, MqttSslProtocols.None);
            String clientId = Guid.NewGuid().ToString();

        }
    }
}
