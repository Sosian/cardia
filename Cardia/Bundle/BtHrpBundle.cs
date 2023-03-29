﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MGT.HRM;
using MGT.HRM.HRP;
using Windows.Devices.Enumeration;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using System.Threading;
using static MGT.Cardia.Configuration;
using log4net;

namespace MGT.Cardia
{
    public class BtHrpBundle : Bundle
    {
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly BtHrp btHrp = new BtHrp();
        private readonly BtHrpFrm btHrpFrm;
        private readonly BtHrpLoggerCSV csvLogger = new BtHrpLoggerCSV();
        private readonly BtHrpLoggerXML xmlLogger = new BtHrpLoggerXML();
        private readonly BtHrpLoggerUDP udpLogger = new BtHrpLoggerUDP();

        public DeviceInformationCollection BtSmartDevices { get; private set; }

        public BtHrpBundle()
        {
            logger.Debug("Retrieving heart rate capable devices");

            var task = DeviceInformation.FindAllAsync(
                GattDeviceService.GetDeviceSelectorFromUuid(GattServiceUuids.HeartRate),
                new string[] { "System.Devices.ContainerId" });

            try
            {
                while (true)
                {
                    logger.Debug("Attempting to retrieve async result...");

                    Thread.Sleep(100);

                    var status = task.Status;

                    if (status == Windows.Foundation.AsyncStatus.Canceled || task.Status == Windows.Foundation.AsyncStatus.Error)
                        break;

                    if (status == Windows.Foundation.AsyncStatus.Completed)
                    {
                        BtSmartDevices = task.GetResults();
                        break;
                    }
                }
            }
            finally
            {
                task.Close();
            }

            logger.Debug($"Found {BtSmartDevices.Count} heart rate capable deices");
            foreach (DeviceInformation device in BtSmartDevices.ToList())
            {
                logger.Debug($"{device.Name}: " +
                    $"id = {device.Id}, " +
                    $"default = {device.IsDefault}, " +
                    $"enabled = {device.IsEnabled},  " +
                    $"paired = {device.Pairing.IsPaired}");
            }

            if (BtSmartDevices.Count > 0)
            {
                btHrp.Device = BtSmartDevices[0];
            }

            btHrpFrm = new BtHrpFrm(this);
        }

        public override HeartRateMonitor Device => btHrp;
        public override HRMDeviceFrm DeviceControlForm => btHrpFrm;
        public override IHRMFileLogger CSVLogger => csvLogger;
        public override IHRMFileLogger XMLLogger => xmlLogger;
        public override IHRMNetLogger UDPLogger => udpLogger;
        public override DeviceConfiguration.DeviceType ConfigEnumerator => DeviceConfiguration.DeviceType.BtHrp;

        public BtHrp BtHrp => btHrp;

        public override void InitDevice()
        {
            
        }

        public override void InitControlForm()
        {

        }

        public override void LoadConfig(Configuration.DeviceConfiguration deviceConfiguration, LogConfiguration logConfiguration)
        {
            if (deviceConfiguration.BtHrp.DeviceId != null)
            {
                foreach (DeviceInformation deviceInformation in BtSmartDevices)
                {
                    if (deviceInformation.Id == deviceConfiguration.BtHrp.DeviceId)
                    {
                        btHrp.Device = deviceInformation;
                        break;
                    }
                }
            }

            btHrp.CharacteristicIndex = deviceConfiguration.BtHrp.CharacteristicIndex;
            btHrp.InitDelay = deviceConfiguration.BtHrp.InitDelay;

            udpLogger.Address = logConfiguration.Address;
            udpLogger.Port = logConfiguration.Port;
        }

        public override void SaveConfig(DeviceConfiguration deviceConfiguration)
        {
            deviceConfiguration.Type = Configuration.DeviceConfiguration.DeviceType.BtHrp;
            if (btHrp.Device != null)
                deviceConfiguration.BtHrp.DeviceId = btHrp.Device.Id;
            deviceConfiguration.BtHrp.CharacteristicIndex = btHrp.CharacteristicIndex;
            deviceConfiguration.BtHrp.InitDelay = btHrp.InitDelay;
        }
    }
}
