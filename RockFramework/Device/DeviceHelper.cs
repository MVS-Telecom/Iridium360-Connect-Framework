using System;
using System.Collections.Generic;
using System.Text;

namespace Rock
{
    public static class DeviceHelper
    {
        public static bool IsTS(string firmware)
        {
            return firmware?.StartsWith("TS") == true;
        }

        public static bool IsCapabilitySupported(string firmware, DeviceCapability capability)
        {
            return IsTS(firmware)
                ? IsTS_Supported(firmware, capability)
                : IsYB(firmware)
                    ? IsYB_Supported(firmware, capability)
                    : IsGF(firmware)
                        ? IsGF_Supported(firmware, capability)
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


        public static bool IsYB(string firmware)
        {
            return firmware?.StartsWith("YB3") == true;
        }


        public static bool IsTS_Supported(string firmware, DeviceCapability fbVar)
        {
            bool z = true;
            int a = GetRawFirmware(firmware);

            if (fbVar != DeviceCapability.InstallUpdateRestart)
            {
                if (fbVar == DeviceCapability.Geofence)
                {
                    if (a >= 10300)
                    {
                        return true;
                    }
                }
                else if (fbVar == DeviceCapability.FirmwareRestart)
                {
                    if (a >= 10200)
                    {
                        return true;
                    }
                }
                else if (fbVar == DeviceCapability.ChangePin)
                {
                    if (a >= 10200)
                    {
                        return true;
                    }
                }
                else if (fbVar == DeviceCapability.ExternalPower)
                {
                    if (a >= 10100)
                    {
                        return true;
                    }
                }
                else if (fbVar == DeviceCapability.RawCommands)
                {
                    if (a >= 10200)
                    {
                        return true;
                    }
                }
                else if (fbVar == DeviceCapability.SerialAPI)
                {
                    if (a >= 10302)
                    {
                        return true;
                    }
                }
                else if (fbVar == DeviceCapability.ActivitySense)
                {
                    if (a >= 10400)
                    {
                        return true;
                    }
                }
                else if (fbVar == DeviceCapability.ExternalPowerAvailability)
                {
                    if (a >= 10408)
                    {
                        return true;
                    }
                }
                else if (fbVar == DeviceCapability.MobWatcher)
                {
                    if (a >= 10200)
                    {
                        return true;
                    }
                }
                else if (fbVar == DeviceCapability.Notify)
                {
                    if (a >= 10409)
                    {
                        return true;
                    }
                }
                else if (fbVar == DeviceCapability.GenericAlerts)
                {
                    if (a >= 10410)
                    {
                        return true;
                    }
                }
                else if (fbVar == DeviceCapability.Gprs)
                {
                    if (a >= 10512)
                    {
                        return true;
                    }
                }
                else if (fbVar == DeviceCapability.Maximet)
                {
                    if (a >= 10504)
                    {
                        return true;
                    }
                }
                else if (fbVar == DeviceCapability.RockBoard)
                {
                    if (a >= 10511)
                    {
                        return true;
                    }
                }
                else if (fbVar == DeviceCapability.ExternalPowerConfigReport)
                {
                    if (a >= 10522)
                    {
                        return true;
                    }
                }
                else if (fbVar == DeviceCapability.SerialDump)
                {
                    if (a >= 10522)
                    {
                        return true;
                    }
                }
                else if (fbVar == DeviceCapability.UnlockedFirmwareUpdate)
                {
                    if (a < 10016)
                    {
                        return true;
                    }
                }
                else if (fbVar == DeviceCapability.MessageAlignment)
                {
                    if (a < 10522)
                    {
                        return true;
                    }
                }
                else if (fbVar == DeviceCapability.MaximetGmx200)
                {
                    if (a >= 10600)
                    {
                        return true;
                    }
                }
                else if (fbVar == DeviceCapability.InputSensitivity)
                {
                    if (a >= 10600)
                    {
                        return true;
                    }
                }
                else if (fbVar == DeviceCapability.Wave)
                {
                    if (a >= 10607)
                    {
                        return true;
                    }
                }
                else if (fbVar == DeviceCapability.Polyfence)
                {
                    if (a >= 10700)
                    {
                        return true;
                    }
                }
                else if (fbVar == DeviceCapability.GprsConfig)
                {
                    if (a >= 10700)
                    {
                        return true;
                    }
                }
                else if (fbVar == DeviceCapability.RevisedFrequency)
                {
                    if (a >= 10800)
                    {
                        return true;
                    }
                }
                else if (fbVar == DeviceCapability.VWTP3)
                {
                    if (a >= 10900)
                    {
                        return true;
                    }
                }
                else if (fbVar == DeviceCapability.ContextualReporting)
                {
                    if (a < 11000)
                    {
                        z = false;
                    }
                    return z;
                }
                else if (fbVar == DeviceCapability.ActivationMode)
                {
                    if (a < 10905)
                    {
                        z = false;
                    }
                    return z;
                }
                else if (fbVar == DeviceCapability.TransmissionFormat)
                {
                    if (a < 11000)
                    {
                        z = false;
                    }
                    return z;
                }
                else if (fbVar == DeviceCapability.FastCellular)
                {
                    if (a < 10905)
                    {
                        z = false;
                    }
                    return z;
                }
                else if (fbVar == DeviceCapability.RevisedPositionFormat)
                {
                    if (a < 11000)
                    {
                        z = false;
                    }
                    return z;
                }
                else if (fbVar == DeviceCapability.DaliaFPSO)
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


        private static bool IsGF(this string firmware)
        {
            return firmware?.StartsWith("GF") == true;
        }


        private static bool IsGF_Supported(string firmware, DeviceCapability fbVar)
        {
            bool z = true;
            int a = GetRawFirmware(firmware);
            if (fbVar == DeviceCapability.InstallUpdateRestart)
            {
                return false;
            }
            if (fbVar == DeviceCapability.Geofence)
            {
                return true;
            }
            if (fbVar == DeviceCapability.FirmwareRestart)
            {
                return true;
            }
            if (fbVar == DeviceCapability.ChangePin)
            {
                return true;
            }
            if (fbVar == DeviceCapability.ExternalPower)
            {
                return true;
            }
            if (fbVar == DeviceCapability.RawCommands)
            {
                return true;
            }
            if (fbVar == DeviceCapability.SerialAPI)
            {
                return true;
            }
            if (fbVar == DeviceCapability.ActivitySense)
            {
                return true;
            }
            if (fbVar == DeviceCapability.ExternalPowerAvailability)
            {
                return true;
            }
            if (fbVar == DeviceCapability.MobWatcher)
            {
                return true;
            }
            if (fbVar == DeviceCapability.Notify)
            {
                return true;
            }
            if (fbVar == DeviceCapability.GenericAlerts)
            {
                return true;
            }
            if (fbVar == DeviceCapability.Gprs)
            {
                return true;
            }
            if (fbVar == DeviceCapability.Maximet)
            {
                return false;
            }
            if (fbVar == DeviceCapability.RockBoard)
            {
                return true;
            }
            if (fbVar == DeviceCapability.ExternalPowerConfigReport)
            {
                return true;
            }
            if (fbVar == DeviceCapability.SerialDump)
            {
                return true;
            }
            if (fbVar == DeviceCapability.UnlockedFirmwareUpdate)
            {
                return true;
            }
            if (fbVar == DeviceCapability.MessageAlignment)
            {
                return false;
            }
            if (fbVar == DeviceCapability.MaximetGmx200)
            {
                return false;
            }
            if (fbVar == DeviceCapability.InputSensitivity)
            {
                return true;
            }
            if (fbVar == DeviceCapability.Wave)
            {
                return false;
            }
            if (fbVar == DeviceCapability.Polyfence)
            {
                return true;
            }
            if (fbVar == DeviceCapability.GprsConfig)
            {
                return true;
            }
            if (fbVar == DeviceCapability.RevisedFrequency)
            {
                return true;
            }
            if (fbVar == DeviceCapability.VWTP3)
            {
                return false;
            }
            if (fbVar == DeviceCapability.ContextualReporting)
            {
                if (a < 10100)
                {
                    z = false;
                }
                return z;
            }
            else if (fbVar == DeviceCapability.ActivationMode)
            {
                if (a < 10100)
                {
                    z = false;
                }
                return z;
            }
            else if (fbVar == DeviceCapability.TransmissionFormat)
            {
                if (a < 10100)
                {
                    z = false;
                }
                return z;
            }
            else if (fbVar == DeviceCapability.FastCellular)
            {
                if (a < 10200)
                {
                    z = false;
                }
                return z;
            }
            else if (fbVar != DeviceCapability.RevisedPositionFormat)
            {
                return fbVar == DeviceCapability.DaliaFPSO ? false : false;
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


        private static bool IsYB_Supported(string firmware, DeviceCapability fbVar)
        {
            int a = GetRawFirmware(firmware);

            if (fbVar != DeviceCapability.InstallUpdateRestart)
            {
                if (fbVar == DeviceCapability.Geofence)
                {
                    if (a >= 30201)
                    {
                        return true;
                    }
                }
                else if (fbVar == DeviceCapability.FirmwareRestart)
                {
                    if (a > 30034)
                    {
                        return true;
                    }
                }
                else if (fbVar == DeviceCapability.ChangePin)
                {
                    if (a >= 30200)
                    {
                        return true;
                    }
                }
                else if (fbVar == DeviceCapability.ExternalPower)
                {
                    if (a >= 30200)
                    {
                        return true;
                    }
                }
                else if (fbVar == DeviceCapability.RawCommands)
                {
                    if (a >= 30200)
                    {
                        return true;
                    }
                }
                else if (fbVar == DeviceCapability.SerialAPI)
                {
                    return false;
                }
                else
                {
                    if (fbVar == DeviceCapability.ActivitySense)
                    {
                        if (a >= 30300)
                        {
                            return true;
                        }
                    }
                    else if (fbVar == DeviceCapability.ExternalPowerAvailability)
                    {
                        if (a >= 30300)
                        {
                            return true;
                        }
                    }
                    else if (fbVar == DeviceCapability.ActivitySense)
                    {
                        if (a >= 30100)
                        {
                            return true;
                        }
                    }
                    else if (fbVar == DeviceCapability.Notify)
                    {
                        if (a >= 30301)
                        {
                            return true;
                        }
                    }
                    else if (fbVar == DeviceCapability.GenericAlerts)
                    {
                        if (a >= 30303)
                        {
                            return true;
                        }
                    }
                    else if (fbVar == DeviceCapability.Gprs)
                    {
                        return false;
                    }
                    else
                    {
                        if (fbVar == DeviceCapability.Maximet)
                        {
                            return false;
                        }
                        if (fbVar == DeviceCapability.SerialDump)
                        {
                            return false;
                        }
                        if (fbVar == DeviceCapability.UnlockedFirmwareUpdate)
                        {
                            return false;
                        }
                        if (fbVar == DeviceCapability.MessageAlignment)
                        {
                            if (a < 30309)
                            {
                                return true;
                            }
                        }
                        else if (fbVar == DeviceCapability.MaximetGmx200)
                        {
                            return false;
                        }
                        else
                        {
                            if (fbVar == DeviceCapability.InputSensitivity)
                            {
                                return false;
                            }
                            if (fbVar == DeviceCapability.Wave)
                            {
                                return false;
                            }
                            if (fbVar == DeviceCapability.Polyfence)
                            {
                                if (a >= 30502)
                                {
                                    return true;
                                }
                            }
                            else if (fbVar == DeviceCapability.FriendlyName)
                            {
                                if (a >= 30504)
                                {
                                    return true;
                                }
                            }
                            else if (fbVar == DeviceCapability.RevisedFrequency)
                            {
                                if (a >= 30600)
                                {
                                    return true;
                                }
                            }
                            else if (fbVar == DeviceCapability.ContextualReporting)
                            {
                                return false;
                            }
                            else
                            {
                                if (fbVar == DeviceCapability.TransmissionFormat)
                                {
                                    return false;
                                }
                                if (fbVar == DeviceCapability.FastCellular)
                                {
                                    return false;
                                }
                                if (fbVar == DeviceCapability.RevisedPositionFormat)
                                {
                                    ///TODO: ЭТО НЕ ТОЧНО, возможно поддерживают и более ранние прошивки
                                    if (a >= 30910)
                                    {
                                        return true;
                                    }
                                }
                                if (fbVar == DeviceCapability.DaliaFPSO)
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
