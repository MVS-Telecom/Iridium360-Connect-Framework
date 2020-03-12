using System;
using System.Collections.Generic;
using System.Text;

namespace Rock
{
    public static class DeviceHelper
    {
        public static bool IsTS(Device device)
        {
            return device?.Firmware?.StartsWith("TS") == true;
        }

        public static bool IsCapabilitySupported(Device device, DeviceCapability capability)
        {
            return IsTS(device)
                ? IsTS_Supported(device, capability)
                : IsYB(device)
                    ? IsYB_Supported(device, capability)
                    : IsGF(device)
                        ? IsGF_Supported(device, capability)
                        : false;
        }

        private static int GetRawFirmware(string firmware)
        {
            string replace = firmware
                .Replace("YB3", "")
                .Replace("TS", "")
                .Replace("GF", "")
                .Replace(" ", "")
                .Replace(".", "");

            return int.Parse(replace);

            //if (C0026az.m75a('.').mo43b((CharSequence)replace) != 2)
            //{
            //    return 0;
            //}
            //int a = C0057cc.m155a(C0026az.m75a('.').mo45c((CharSequence)replace));
            //return a == null ? 0 : a;
        }


        public static bool IsYB(Device device)
        {
            return device?.Firmware?.StartsWith("YB3") == true;
        }


        public static bool IsTS_Supported(Device connectDevice, DeviceCapability fbVar)
        {
            bool z = true;
            int a = GetRawFirmware(connectDevice.Firmware);
            if (fbVar != DeviceCapability.DeviceCapabilityTypeInstallUpdateRestart)
            {
                if (fbVar == DeviceCapability.DeviceCapabilityTypeGeofence)
                {
                    if (a >= 10300)
                    {
                        return true;
                    }
                }
                else if (fbVar == DeviceCapability.DeviceCapabilityTypeFirmwareRestart)
                {
                    if (a >= 10200)
                    {
                        return true;
                    }
                }
                else if (fbVar == DeviceCapability.DeviceCapabilityTypeChangePin)
                {
                    if (a >= 10200)
                    {
                        return true;
                    }
                }
                else if (fbVar == DeviceCapability.DeviceCapabilityTypeExternalPower)
                {
                    if (a >= 10100)
                    {
                        return true;
                    }
                }
                else if (fbVar == DeviceCapability.DeviceCapabilityTypeRawCommands)
                {
                    if (a >= 10200)
                    {
                        return true;
                    }
                }
                else if (fbVar == DeviceCapability.DeviceCapabilityTypeSerialAPI)
                {
                    if (a >= 10302)
                    {
                        return true;
                    }
                }
                else if (fbVar == DeviceCapability.DeviceCapabilityTypeActivitySense)
                {
                    if (a >= 10400)
                    {
                        return true;
                    }
                }
                else if (fbVar == DeviceCapability.DeviceCapabilityTypeExternalPowerAvailability)
                {
                    if (a >= 10408)
                    {
                        return true;
                    }
                }
                else if (fbVar == DeviceCapability.DeviceCapabilityTypeMobWatcher)
                {
                    if (a >= 10200)
                    {
                        return true;
                    }
                }
                else if (fbVar == DeviceCapability.DeviceCapabilityTypeNotify)
                {
                    if (a >= 10409)
                    {
                        return true;
                    }
                }
                else if (fbVar == DeviceCapability.DeviceCapabilityTypeGenericAlerts)
                {
                    if (a >= 10410)
                    {
                        return true;
                    }
                }
                else if (fbVar == DeviceCapability.DeviceCapabilityTypeGprs)
                {
                    if (a >= 10512)
                    {
                        return true;
                    }
                }
                else if (fbVar == DeviceCapability.DeviceCapabilityTypeMaximet)
                {
                    if (a >= 10504)
                    {
                        return true;
                    }
                }
                else if (fbVar == DeviceCapability.DeviceCapabilityTypeRockBoard)
                {
                    if (a >= 10511)
                    {
                        return true;
                    }
                }
                else if (fbVar == DeviceCapability.DeviceCapabilityTypeExternalPowerConfigReport)
                {
                    if (a >= 10522)
                    {
                        return true;
                    }
                }
                else if (fbVar == DeviceCapability.DeviceCapabilityTypeSerialDump)
                {
                    if (a >= 10522)
                    {
                        return true;
                    }
                }
                else if (fbVar == DeviceCapability.DeviceCapabilityTypeUnlockedFirmwareUpdate)
                {
                    if (a < 10016)
                    {
                        return true;
                    }
                }
                else if (fbVar == DeviceCapability.DeviceCapabilityTypeMessageAlignment)
                {
                    if (a < 10522)
                    {
                        return true;
                    }
                }
                else if (fbVar == DeviceCapability.DeviceCapabilityTypeMaximetGmx200)
                {
                    if (a >= 10600)
                    {
                        return true;
                    }
                }
                else if (fbVar == DeviceCapability.DeviceCapabilityTypeInputSensitivity)
                {
                    if (a >= 10600)
                    {
                        return true;
                    }
                }
                else if (fbVar == DeviceCapability.DeviceCapabilityTypeWave)
                {
                    if (a >= 10607)
                    {
                        return true;
                    }
                }
                else if (fbVar == DeviceCapability.DeviceCapabilityTypePolyfence)
                {
                    if (a >= 10700)
                    {
                        return true;
                    }
                }
                else if (fbVar == DeviceCapability.DeviceCapabilityTypeGprsConfig)
                {
                    if (a >= 10700)
                    {
                        return true;
                    }
                }
                else if (fbVar == DeviceCapability.DeviceCapabilityTypeRevisedFrequency)
                {
                    if (a >= 10800)
                    {
                        return true;
                    }
                }
                else if (fbVar == DeviceCapability.DeviceCapabilityTypeVWTP3)
                {
                    if (a >= 10900)
                    {
                        return true;
                    }
                }
                else if (fbVar == DeviceCapability.DeviceCapabilityTypeContextualReporting)
                {
                    if (a < 11000)
                    {
                        z = false;
                    }
                    return z;
                }
                else if (fbVar == DeviceCapability.DeviceCapabilityTypeActivationMode)
                {
                    if (a < 10905)
                    {
                        z = false;
                    }
                    return z;
                }
                else if (fbVar == DeviceCapability.DeviceCapabilityTypeTransmissionFormat)
                {
                    if (a < 11000)
                    {
                        z = false;
                    }
                    return z;
                }
                else if (fbVar == DeviceCapability.DeviceCapabilityTypeFastCellular)
                {
                    if (a < 10905)
                    {
                        z = false;
                    }
                    return z;
                }
                else if (fbVar == DeviceCapability.DeviceCapabilityTypeRevisedPositionFormat)
                {
                    if (a < 11000)
                    {
                        z = false;
                    }
                    return z;
                }
                else if (fbVar == DeviceCapability.DeviceCapabilityTypeDaliaFPSO)
                {
                    if (a < 10904)
                    {
                        z = false;
                    }
                    return z;
                }
            }
            return false;
        }


        private static bool IsGF(this Device device)
        {
            return device?.Firmware?.StartsWith("GF") == true;
        }


        private static bool IsGF_Supported(this Device connectDevice, DeviceCapability fbVar)
        {
            bool z = true;
            int a = GetRawFirmware(connectDevice.Firmware);
            if (fbVar == DeviceCapability.DeviceCapabilityTypeInstallUpdateRestart)
            {
                return false;
            }
            if (fbVar == DeviceCapability.DeviceCapabilityTypeGeofence)
            {
                return true;
            }
            if (fbVar == DeviceCapability.DeviceCapabilityTypeFirmwareRestart)
            {
                return true;
            }
            if (fbVar == DeviceCapability.DeviceCapabilityTypeChangePin)
            {
                return true;
            }
            if (fbVar == DeviceCapability.DeviceCapabilityTypeExternalPower)
            {
                return true;
            }
            if (fbVar == DeviceCapability.DeviceCapabilityTypeRawCommands)
            {
                return true;
            }
            if (fbVar == DeviceCapability.DeviceCapabilityTypeSerialAPI)
            {
                return true;
            }
            if (fbVar == DeviceCapability.DeviceCapabilityTypeActivitySense)
            {
                return true;
            }
            if (fbVar == DeviceCapability.DeviceCapabilityTypeExternalPowerAvailability)
            {
                return true;
            }
            if (fbVar == DeviceCapability.DeviceCapabilityTypeMobWatcher)
            {
                return true;
            }
            if (fbVar == DeviceCapability.DeviceCapabilityTypeNotify)
            {
                return true;
            }
            if (fbVar == DeviceCapability.DeviceCapabilityTypeGenericAlerts)
            {
                return true;
            }
            if (fbVar == DeviceCapability.DeviceCapabilityTypeGprs)
            {
                return true;
            }
            if (fbVar == DeviceCapability.DeviceCapabilityTypeMaximet)
            {
                return false;
            }
            if (fbVar == DeviceCapability.DeviceCapabilityTypeRockBoard)
            {
                return true;
            }
            if (fbVar == DeviceCapability.DeviceCapabilityTypeExternalPowerConfigReport)
            {
                return true;
            }
            if (fbVar == DeviceCapability.DeviceCapabilityTypeSerialDump)
            {
                return true;
            }
            if (fbVar == DeviceCapability.DeviceCapabilityTypeUnlockedFirmwareUpdate)
            {
                return true;
            }
            if (fbVar == DeviceCapability.DeviceCapabilityTypeMessageAlignment)
            {
                return false;
            }
            if (fbVar == DeviceCapability.DeviceCapabilityTypeMaximetGmx200)
            {
                return false;
            }
            if (fbVar == DeviceCapability.DeviceCapabilityTypeInputSensitivity)
            {
                return true;
            }
            if (fbVar == DeviceCapability.DeviceCapabilityTypeWave)
            {
                return false;
            }
            if (fbVar == DeviceCapability.DeviceCapabilityTypePolyfence)
            {
                return true;
            }
            if (fbVar == DeviceCapability.DeviceCapabilityTypeGprsConfig)
            {
                return true;
            }
            if (fbVar == DeviceCapability.DeviceCapabilityTypeRevisedFrequency)
            {
                return true;
            }
            if (fbVar == DeviceCapability.DeviceCapabilityTypeVWTP3)
            {
                return false;
            }
            if (fbVar == DeviceCapability.DeviceCapabilityTypeContextualReporting)
            {
                if (a < 10100)
                {
                    z = false;
                }
                return z;
            }
            else if (fbVar == DeviceCapability.DeviceCapabilityTypeActivationMode)
            {
                if (a < 10100)
                {
                    z = false;
                }
                return z;
            }
            else if (fbVar == DeviceCapability.DeviceCapabilityTypeTransmissionFormat)
            {
                if (a < 10100)
                {
                    z = false;
                }
                return z;
            }
            else if (fbVar == DeviceCapability.DeviceCapabilityTypeFastCellular)
            {
                if (a < 10200)
                {
                    z = false;
                }
                return z;
            }
            else if (fbVar != DeviceCapability.DeviceCapabilityTypeRevisedPositionFormat)
            {
                return fbVar == DeviceCapability.DeviceCapabilityTypeDaliaFPSO ? false : false;
            }
            else
            {
                if (a < 10300)
                {
                    z = false;
                }
                return z;
            }
        }


        private static bool IsYB_Supported(this Device connectDevice, DeviceCapability fbVar)
        {
            int a = GetRawFirmware(connectDevice.Firmware);
            if (fbVar != DeviceCapability.DeviceCapabilityTypeInstallUpdateRestart)
            {
                if (fbVar == DeviceCapability.DeviceCapabilityTypeGeofence)
                {
                    if (a >= 30201)
                    {
                        return true;
                    }
                }
                else if (fbVar == DeviceCapability.DeviceCapabilityTypeFirmwareRestart)
                {
                    if (a > 30034)
                    {
                        return true;
                    }
                }
                else if (fbVar == DeviceCapability.DeviceCapabilityTypeChangePin)
                {
                    if (a >= 30200)
                    {
                        return true;
                    }
                }
                else if (fbVar == DeviceCapability.DeviceCapabilityTypeExternalPower)
                {
                    if (a >= 30200)
                    {
                        return true;
                    }
                }
                else if (fbVar == DeviceCapability.DeviceCapabilityTypeRawCommands)
                {
                    if (a >= 30200)
                    {
                        return true;
                    }
                }
                else if (fbVar == DeviceCapability.DeviceCapabilityTypeSerialAPI)
                {
                    return false;
                }
                else
                {
                    if (fbVar == DeviceCapability.DeviceCapabilityTypeActivitySense)
                    {
                        if (a >= 30300)
                        {
                            return true;
                        }
                    }
                    else if (fbVar == DeviceCapability.DeviceCapabilityTypeExternalPowerAvailability)
                    {
                        if (a >= 30300)
                        {
                            return true;
                        }
                    }
                    else if (fbVar == DeviceCapability.DeviceCapabilityTypeActivitySense)
                    {
                        if (a >= 30100)
                        {
                            return true;
                        }
                    }
                    else if (fbVar == DeviceCapability.DeviceCapabilityTypeNotify)
                    {
                        if (a >= 30301)
                        {
                            return true;
                        }
                    }
                    else if (fbVar == DeviceCapability.DeviceCapabilityTypeGenericAlerts)
                    {
                        if (a >= 30303)
                        {
                            return true;
                        }
                    }
                    else if (fbVar == DeviceCapability.DeviceCapabilityTypeGprs)
                    {
                        return false;
                    }
                    else
                    {
                        if (fbVar == DeviceCapability.DeviceCapabilityTypeMaximet)
                        {
                            return false;
                        }
                        if (fbVar == DeviceCapability.DeviceCapabilityTypeSerialDump)
                        {
                            return false;
                        }
                        if (fbVar == DeviceCapability.DeviceCapabilityTypeUnlockedFirmwareUpdate)
                        {
                            return false;
                        }
                        if (fbVar == DeviceCapability.DeviceCapabilityTypeMessageAlignment)
                        {
                            if (a < 30309)
                            {
                                return true;
                            }
                        }
                        else if (fbVar == DeviceCapability.DeviceCapabilityTypeMaximetGmx200)
                        {
                            return false;
                        }
                        else
                        {
                            if (fbVar == DeviceCapability.DeviceCapabilityTypeInputSensitivity)
                            {
                                return false;
                            }
                            if (fbVar == DeviceCapability.DeviceCapabilityTypeWave)
                            {
                                return false;
                            }
                            if (fbVar == DeviceCapability.DeviceCapabilityTypePolyfence)
                            {
                                if (a >= 30502)
                                {
                                    return true;
                                }
                            }
                            else if (fbVar == DeviceCapability.DeviceCapabilityTypeFriendlyName)
                            {
                                if (a >= 30504)
                                {
                                    return true;
                                }
                            }
                            else if (fbVar == DeviceCapability.DeviceCapabilityTypeRevisedFrequency)
                            {
                                if (a >= 30600)
                                {
                                    return true;
                                }
                            }
                            else if (fbVar == DeviceCapability.DeviceCapabilityTypeContextualReporting)
                            {
                                return false;
                            }
                            else
                            {
                                if (fbVar == DeviceCapability.DeviceCapabilityTypeTransmissionFormat)
                                {
                                    return false;
                                }
                                if (fbVar == DeviceCapability.DeviceCapabilityTypeFastCellular)
                                {
                                    return false;
                                }
                                if (fbVar == DeviceCapability.DeviceCapabilityTypeRevisedPositionFormat)
                                {
                                    return false;
                                }
                                if (fbVar == DeviceCapability.DeviceCapabilityTypeDaliaFPSO)
                                {
                                    return false;
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }
    }
}
