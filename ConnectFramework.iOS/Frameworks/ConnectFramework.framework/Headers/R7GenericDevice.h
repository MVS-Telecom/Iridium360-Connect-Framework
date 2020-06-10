#import "ConnectDevice.h"
#import "DeviceAccessory.h"

typedef NS_ENUM(NSUInteger, R7GenericDeviceParameter) {
    
    R7GenericDeviceParameterAlertsTimerStatus = 1000,
    R7GenericDeviceParameterAlertsTimerTimeout = 1001,
    R7GenericDeviceParameterAlertsBluetoothLossStatus = 1002,
    R7GenericDeviceParameterAlertsCollisionDuration = 1003,
    R7GenericDeviceParameterAlertsCollisionStatus = 1004,
    R7GenericDeviceParameterAlertsCollisionThreshold = 1005,
    R7GenericDeviceParameterAlertsDeadmanStatus = 1006,
    R7GenericDeviceParameterAlertsDeadmanTimeout = 1007,
    R7GenericDeviceParameterAlertsGeofenceDistance = 1008,
    R7GenericDeviceParameterAlertsGeofenceCheckFrequency = 1009,
    R7GenericDeviceParameterAlertsGeofenceStatus = 1010,
    R7GenericDeviceParameterAlertsGeofenceCentre = 2010,
    R7GenericDeviceParameterAlertsPowerLossStatus = 1011,
    R7GenericDeviceParameterAlertsTemperatureCheckFrequency = 1012,
    R7GenericDeviceParameterAlertsHotTemperature = 1013,
    R7GenericDeviceParameterAlertsColdTemperature = 1014,
    R7GenericDeviceParameterAlertsTemperatureStatus = 1015,
    R7GenericDeviceParameterExternalBaudRate = 1016,
    R7GenericDeviceParameterExternalStatus = 1017,
    R7GenericDeviceParameterExternalMobWatcher = 1050,
    R7GenericDeviceParameterExternalSampleRate = 1018,
    R7GenericDeviceParameterFrameworkBatteryLevel = 1019,
    R7GenericDeviceParameterFrameworkInboxCount = 1020,
    R7GenericDeviceParameterFrameworkOutboxConfirm = 1021,
    R7GenericDeviceParameterFrameworkSerialRX = 1022,
    R7GenericDeviceParameterFrameworkSerialTX = 1023,
    R7GenericDeviceParameterFrameworkLocation = 1024,
    R7GenericDeviceParameterGPSEarlyWakeup = 1025,
    R7GenericDeviceParameterGPSMode = 1026,
    R7GenericDeviceParameterGPSFixesbeforeaccept = 1027,
    R7GenericDeviceParameterGPSHotStatus = 1028,
    R7GenericDeviceParameterGpxLoggingPeriod = 1029,
    R7GenericDeviceParameterGpxLoggingStatus = 1030,
    R7GenericDeviceParameterMailboxCheckFrequency = 1031,
    R7GenericDeviceParameterMailboxCheckStatus = 1032,
    R7GenericDeviceParameterPrivateLogging = 1033,
    R7GenericDeviceParameterPrivateLogo = 1034,
    R7GenericDeviceParameterPrivateUserMode = 1035,
    R7GenericDeviceParameterScreenAccessPin = 1036,
    R7GenericDeviceParameterScreenNonActivityThreshold = 1037,
    R7GenericDeviceParameterScreenScreenBrightness = 1038,
    R7GenericDeviceParameterScreenTimeout = 1040,
    R7GenericDeviceParameterScreenStealthStatus = 1041,
    R7GenericDeviceParameterSystemEncryptionStatus = 1042,
    R7GenericDeviceParameterTrackingActivitySenseStatus = 1043,
    R7GenericDeviceParameterTrackingActivitySenseLowThreshold = 1044,       //Deprecated
    R7GenericDeviceParameterTrackingActivitySenseHighThreshold = 1045,      //Deprecated
    
    R7GenericDeviceParameterTrackingBurstFixPeriod = 1046,
    R7GenericDeviceParameterTrackingBurstTransmitPeriod = 1047,
    R7GenericDeviceParameterTrackingFrequency = 1048,
    R7GenericDeviceParameterTrackingStatus = 1049,
    
    R7GenericDeviceParameterBatteryLevel = 2000,
    R7GenericDeviceParameterGpsStatus = 2001,
    R7GenericDeviceParameterIridiumStatus = 2002,
    R7GenericDeviceParameterPowerStatus = 2003,
    R7GenericDeviceParameterModel = 2004,
    R7GenericDeviceParameterExternalPowerAvailability = 2005,
    R7GenericDeviceParameterNotify = 2006,
    R7GenericDeviceParameterGprsStrategy = 2007,
    R7GenericDeviceParameterGprsStatus = 2008,
    R7GenericDeviceParameterGprsSignal = 2009,
    R7GenericDeviceParameterGprsMsisdn = 2010,
    R7GenericDeviceParameterGprsSim = 2011,
    R7GenericDeviceParameterGprsLocation = 2012,
    
    R7GenericDeviceParameterInputSensitivity = 2013,
    R7GenericDeviceParameterCellularFrequency = 2014,                       //Added 19/01/2016
    R7GenericDeviceParameterCellularBurstFixPeriod = 2015,
    R7GenericDeviceParameterCellularBurstTransmitPeriod = 2016,
    R7GenericDeviceParameterDistressFrequency = 2017,
    R7GenericDeviceParameterDistressBurstFixPeriod = 2018,
    R7GenericDeviceParameterDistressBurstTransmitPeriod = 2019,
    
    R7GenericDeviceParameterTransmissionFormat = 2020,                       //Added 14/03/2017
    R7GenericDeviceParameterTrackingActivitySenseThreshold = 2021,
    
    R7GenericDeviceParameterAscentDescentAlertMode = 2022,                  //Added 29/11/2017
    R7GenericDeviceParameterAscentAlertThreshold = 2023,
    R7GenericDeviceParameterDescentAlertThreshold = 2024,
    R7GenericDeviceParameterAscentAlertTimePeriod = 2025,
    R7GenericDeviceParameterDescentAlertTimePeriod = 2026,
    R7GenericDeviceParameterGpsDynamicPlatformModel = 2027,
    
    R7GenericDeviceParameterAutoResumeStatus = 2028,                        //Added 11/01/2018
    R7GenericDeviceParameterAutoResumeDistance = 2029,
    R7GenericDeviceParameterHoverAlertStatus = 2030,
    R7GenericDeviceParameterDebugLogging = 2031,
    
    R7GenericDeviceParameterAirborneVRotate = 2032,                         //Added 16/03/2018
    R7GenericDeviceParameterAirborneVStall = 2033,
    
    R7GenericDeviceParameterContext = 2034,                                 //Added 12/07/2018
    R7GenericDeviceParameterWatchState = 2035,
      
    R7GenericDeviceParameterSelfTestResult = 2036                           //Added 27/07/2018
    
};

@interface R7GenericDevice : ConnectDevice {
}

@property (nonatomic,readonly) DeviceAccessory* accessory;

-(void)requestParameter:(R7GenericDeviceParameter)parameter;
-(void)updateParameter:(R7GenericDeviceParameter)identifier value:(NSData*)value;
-(void)notifyParameter:(R7GenericDeviceParameter)identifier notify:(BOOL)notify;

typedef NS_ENUM(NSUInteger, R7GenericDeviceValueAlertsTimerStatus) {
    R7GenericDeviceValueAlertsTimerStatusOff = 0,
    R7GenericDeviceValueAlertsTimerStatusOn = 1
};

typedef NS_ENUM(NSUInteger, R7GenericDeviceValueAlertsTimerTimeout) {
    R7GenericDeviceValueAlertsTimerTimeout5min = 0,
    R7GenericDeviceValueAlertsTimerTimeout10min = 1,
    R7GenericDeviceValueAlertsTimerTimeout15min = 2,
    R7GenericDeviceValueAlertsTimerTimeout30min = 3,
    R7GenericDeviceValueAlertsTimerTimeout1hour = 4,
    R7GenericDeviceValueAlertsTimerTimeout2hour = 5,
    R7GenericDeviceValueAlertsTimerTimeout4hour = 6,
    R7GenericDeviceValueAlertsTimerTimeoutUnknown = 7
};



typedef NS_ENUM(NSUInteger, R7GenericDeviceValueAlertsBluetoothLossStatus) {
    R7GenericDeviceValueAlertsBluetoothLossStatusOff = 0,
    R7GenericDeviceValueAlertsBluetoothLossStatusOn = 1
};



typedef NS_ENUM(NSUInteger, R7GenericDeviceValueAlertsCollisionDuration) {
    R7GenericDeviceValueAlertsCollisionDuration1ms = 0,
    R7GenericDeviceValueAlertsCollisionDuration2ms = 1,
    R7GenericDeviceValueAlertsCollisionDuration5ms = 2,
    R7GenericDeviceValueAlertsCollisionDuration10ms = 3,
    R7GenericDeviceValueAlertsCollisionDuration20ms = 4
};

typedef NS_ENUM(NSUInteger, R7GenericDeviceValueAlertsCollisionStatus) {
    R7GenericDeviceValueAlertsCollisionStatusOff = 0,
    R7GenericDeviceValueAlertsCollisionStatusOn = 1
};

typedef NS_ENUM(NSUInteger, R7GenericDeviceValueAlertsNotify) {
    R7GenericDeviceValueAlertsNotifyNone = 0,
    R7GenericDeviceValueAlertsNotifyAudible = 1,
    R7GenericDeviceValueAlertsNotifyVisual = 2,
    R7GenericDeviceValueAlertsNotifyBoth = 3
};

typedef NS_ENUM(NSUInteger, R7GenericDeviceValueAlertsCollisionThreshold) {
    R7GenericDeviceValueAlertsCollisionThreshold1g = 0,
    R7GenericDeviceValueAlertsCollisionThreshold2g = 1,
    R7GenericDeviceValueAlertsCollisionThreshold4g = 2,
    R7GenericDeviceValueAlertsCollisionThreshold8g = 3,
    R7GenericDeviceValueAlertsCollisionThreshold12g = 4,
    R7GenericDeviceValueAlertsCollisionThreshold16g = 5
};

typedef NS_ENUM(NSUInteger, R7GenericDeviceValueAlertsDeadmanStatus) {
    R7GenericDeviceValueAlertsDeadmanStatusOff = 0,
    R7GenericDeviceValueAlertsDeadmanStatusOn = 1
};



typedef NS_ENUM(NSUInteger, R7GenericDeviceValueAlertsDeadmanTimeout) {
    R7GenericDeviceValueAlertsDeadmanTimeout5min = 0,
    R7GenericDeviceValueAlertsDeadmanTimeout10min = 1,
    R7GenericDeviceValueAlertsDeadmanTimeout15min = 2,
    R7GenericDeviceValueAlertsDeadmanTimeout30min = 3,
    R7GenericDeviceValueAlertsDeadmanTimeout1hour = 4,
    R7GenericDeviceValueAlertsDeadmanTimeout2hour = 5,
    R7GenericDeviceValueAlertsDeadmanTimeout4hour = 6,
    R7GenericDeviceValueAlertsDeadmanTimeoutUnknown = 7
};



typedef NS_ENUM(NSUInteger, R7GenericDeviceValueAlertsGeofenceDistance){
    R7GenericDeviceValueAlertsGeofenceDistance25M = 0,
    R7GenericDeviceValueAlertsGeofenceDistance50M = 1,
    R7GenericDeviceValueAlertsGeofenceDistance100M = 2,
    R7GenericDeviceValueAlertsGeofenceDistance250M = 3,
    R7GenericDeviceValueAlertsGeofenceDistance1KM = 4,
    R7GenericDeviceValueAlertsGeofenceDistance2KM = 5,
    R7GenericDeviceValueAlertsGeofenceDistance3KM = 6
};


typedef NS_ENUM(NSUInteger, R7GenericDeviceValueAlertsGeofenceCheckFrequency) {
    R7GenericDeviceValueAlertsGeofenceCheckFrequency1min = 0,
    R7GenericDeviceValueAlertsGeofenceCheckFrequency2min = 1,
    R7GenericDeviceValueAlertsGeofenceCheckFrequency3min = 2,
    R7GenericDeviceValueAlertsGeofenceCheckFrequency5min = 3,
    R7GenericDeviceValueAlertsGeofenceCheckFrequency10min = 4,
    R7GenericDeviceValueAlertsGeofenceCheckFrequency15min = 5,
    R7GenericDeviceValueAlertsGeofenceCheckFrequency30min = 6,
    R7GenericDeviceValueAlertsGeofenceCheckFrequencyUnknown = 7
};



typedef NS_ENUM(NSUInteger, R7GenericDeviceValueAlertsGeofenceStatus) {
    R7GenericDeviceValueAlertsGeofenceStatusOff = 0,
    R7GenericDeviceValueAlertsGeofenceStatusOn = 1,
    R7GenericDeviceValueAlertsGeofenceStatusOnPolyfence = 2
};



typedef NS_ENUM(NSUInteger, R7GenericDeviceValueAlertsPowerLossStatus) {
    R7GenericDeviceValueAlertsPowerLossStatusOff = 0,
    R7GenericDeviceValueAlertsPowerLossStatusOn = 1
};



typedef NS_ENUM(NSUInteger, R7GenericDeviceValueAlertsTemperatureCheckFrequency) {
    R7GenericDeviceValueAlertsTemperatureCheckFrequency1min = 0,
    R7GenericDeviceValueAlertsTemperatureCheckFrequency2min = 1,
    R7GenericDeviceValueAlertsTemperatureCheckFrequency3min = 2,
    R7GenericDeviceValueAlertsTemperatureCheckFrequency5min = 3,
    R7GenericDeviceValueAlertsTemperatureCheckFrequency10min = 4,
    R7GenericDeviceValueAlertsTemperatureCheckFrequency15min = 5,
    R7GenericDeviceValueAlertsTemperatureCheckFrequency30min = 6,
    R7GenericDeviceValueAlertsTemperatureCheckFrequencyUnknown = 7
};



typedef NS_ENUM(NSUInteger, R7GenericDeviceValueAlertsHotTemperature) {
    R7GenericDeviceValueAlertsHotTemperatureMinus40 = 0,
    R7GenericDeviceValueAlertsHotTemperatureMinus35 = 1,
    R7GenericDeviceValueAlertsHotTemperatureMinus30 = 2,
    R7GenericDeviceValueAlertsHotTemperatureMinus25 = 3,
    R7GenericDeviceValueAlertsHotTemperatureMinus20 = 4,
    R7GenericDeviceValueAlertsHotTemperatureMinus15 = 5,
    R7GenericDeviceValueAlertsHotTemperatureMinus10 = 6,
    R7GenericDeviceValueAlertsHotTemperatureMinus5 = 7,
    R7GenericDeviceValueAlertsHotTemperatureZero = 8,
    R7GenericDeviceValueAlertsHotTemperaturePlus5 = 9,
    R7GenericDeviceValueAlertsHotTemperaturePlus10 = 10,
    R7GenericDeviceValueAlertsHotTemperaturePlus15 = 11,
    R7GenericDeviceValueAlertsHotTemperaturePlus20 = 12,
    R7GenericDeviceValueAlertsHotTemperaturePlus30 = 14,
    R7GenericDeviceValueAlertsHotTemperaturePlus25 = 13,
    R7GenericDeviceValueAlertsHotTemperaturePlus35 = 15,
    R7GenericDeviceValueAlertsHotTemperaturePlus40 = 16,
    R7GenericDeviceValueAlertsHotTemperaturePlus45 = 17,
    R7GenericDeviceValueAlertsHotTemperaturePlus50 = 18
};



typedef NS_ENUM(NSUInteger, R7GenericDeviceValueAlertsColdTemperature) {
    R7GenericDeviceValueAlertsColdTemperatureMinus40 = 0,
    R7GenericDeviceValueAlertsColdTemperatureMinus35 = 1,
    R7GenericDeviceValueAlertsColdTemperatureMinus30 = 2,
    R7GenericDeviceValueAlertsColdTemperatureMinus25 = 3,
    R7GenericDeviceValueAlertsColdTemperatureMinus20 = 4,
    R7GenericDeviceValueAlertsColdTemperatureMinus15 = 5,
    R7GenericDeviceValueAlertsColdTemperatureMinus10 = 6,
    R7GenericDeviceValueAlertsColdTemperatureMinus5 = 7,
    R7GenericDeviceValueAlertsColdTemperatureZero = 8,
    R7GenericDeviceValueAlertsColdTemperaturePlus5 = 9,
    R7GenericDeviceValueAlertsColdTemperaturePlus10 = 10,
    R7GenericDeviceValueAlertsColdTemperaturePlus15 = 11,
    R7GenericDeviceValueAlertsColdTemperaturePlus20 = 12,
    R7GenericDeviceValueAlertsColdTemperaturePlus25 = 13,
    R7GenericDeviceValueAlertsColdTemperaturePlus30 = 14,
    R7GenericDeviceValueAlertsColdTemperaturePlus35 = 15,
    R7GenericDeviceValueAlertsColdTemperaturePlus40 = 16,
    R7GenericDeviceValueAlertsColdTemperaturePlus45 = 17,
    R7GenericDeviceValueAlertsColdTemperaturePlus50 = 18
};

typedef NS_ENUM(NSUInteger, R7GenericDeviceValueAlertsTemperatureStatus) {
    R7GenericDeviceValueAlertsTemperatureStatusOff = 0,
    R7GenericDeviceValueAlertsTemperatureStatusOn = 1
};

typedef NS_ENUM(NSUInteger, R7GenericDeviceValueExternalBaudRate) {
    R7GenericDeviceValueExternalBaudRate4800 = 0,
    R7GenericDeviceValueExternalBaudRate9600 = 1,
    R7GenericDeviceValueExternalBaudRate19200 = 2,
    R7GenericDeviceValueExternalBaudRate38400 = 3,
    R7GenericDeviceValueExternalBaudRate57600 = 4,
    R7GenericDeviceValueExternalBaudRate115200 = 5
};

typedef NS_ENUM(NSUInteger, R7GenericDeviceValueExternalPowerAvailiability) {
    R7GenericDeviceValueExternalPowerAvailiabilityUnlimited = 0,
    R7GenericDeviceValueExternalPowerAvailiabilityLimited = 1,
    R7GenericDeviceValueExternalPowerAvailiabilityUnlimitedActivate = 2
};

typedef NS_ENUM(NSUInteger, R7GenericDeviceValueExternalStatus) {
    R7GenericDeviceValueExternalStatusOff = 0,
    R7GenericDeviceValueExternalStatusNMEA = 1,
    R7GenericDeviceValueExternalStatusHydrosphere = 2,
    R7GenericDeviceValueExternalStatusSerialApi = 3,                    //Added 06/01/2014
    R7GenericDeviceValueExternalStatusMaximet = 4,                      //Added 03/06/2015
    R7GenericDeviceValueExternalStatusMaximetGmx200 = 5,                //Added 01/01/2016
    R7GenericDeviceValueExternalStatusWave = 6,                         //Added 31/03/2016
    R7GenericDeviceValueExternalStatusVWTP3 = 7,                        //Added 24/11/2016
    R7GenericDeviceValueExternalStatusDaliaFPSO = 8                     //Added 20/03/2017
};

typedef NS_ENUM(NSUInteger, R7GenericDeviceValueExternalMobWatcher) {
    R7GenericDeviceValueExternalMobWatcherOff = 0,
    R7GenericDeviceValueExternalMobWatcherOn = 1
};

typedef NS_ENUM(NSUInteger, R7GenericDeviceValueExternalSampleRate) {
    R7GenericDeviceValueExternalSampleRate5sec = 0,
    R7GenericDeviceValueExternalSampleRate10sec = 1,
    R7GenericDeviceValueExternalSampleRate20sec = 2,
    R7GenericDeviceValueExternalSampleRate40sec = 3,
    R7GenericDeviceValueExternalSampleRate60sec = 4
};

typedef NS_ENUM(NSUInteger, R7GenericDeviceValueExternalInput1) {
    R7GenericDeviceValueExternalInput1Off = 0,
    R7GenericDeviceValueExternalInput1Rising = 1,
    R7GenericDeviceValueExternalInput1Falling = 2
};

typedef NS_ENUM(NSUInteger, R7GenericDeviceValueExternalInput2) {
    R7GenericDeviceValueExternalInput2Off = 0,
    R7GenericDeviceValueExternalInput2Rising = 1,
    R7GenericDeviceValueExternalInput2Falling = 2
};

typedef NS_ENUM(NSUInteger, R7GenericDeviceValueGPSEarlyWakeup) {
    R7GenericDeviceValueGPSEarlyWakeup20sec = 0,
    R7GenericDeviceValueGPSEarlyWakeup40sec = 1,
    R7GenericDeviceValueGPSEarlyWakeup60sec = 2,
    R7GenericDeviceValueGPSEarlyWakeup120sec = 3,
    R7GenericDeviceValueGPSEarlyWakeup180sec = 4,
    R7GenericDeviceValueGPSEarlyWakeup240sec = 5
};



typedef NS_ENUM(NSUInteger, R7GenericDeviceValueGPSMode) {
    R7GenericDeviceValueGPSMode2D = 0,
    R7GenericDeviceValueGPSMode3D = 1,
    R7GenericDeviceValueGPSModeUnknown = 7
};



typedef NS_ENUM(NSUInteger, R7GenericDeviceValueGPSFixesbeforeaccept) {
    R7GenericDeviceValueGPSFixesbeforeaccept1Fix = 0,
    R7GenericDeviceValueGPSFixesbeforeaccept5Fix = 1,
    R7GenericDeviceValueGPSFixesbeforeaccept10Fix = 2,
    R7GenericDeviceValueGPSFixesbeforeaccept20Fix = 3,
    R7GenericDeviceValueGPSFixesbeforeacceptUnknown = 7
};



typedef NS_ENUM(NSUInteger, R7GenericDeviceValueGPSHotStatus) {
    R7GenericDeviceValueGPSHotStatusOff = 0,
    R7GenericDeviceValueGPSHotStatusOn = 1
};



typedef NS_ENUM(NSUInteger, R7GenericDeviceValueGpxLoggingPeriod) {
    R7GenericDeviceValueGpxLoggingPeriod1Sec = 0,
    R7GenericDeviceValueGpxLoggingPeriod5Sec = 1,
    R7GenericDeviceValueGpxLoggingPeriod10Sec = 2,
    R7GenericDeviceValueGpxLoggingPeriod30Sec = 3,
    R7GenericDeviceValueGpxLoggingPeriod1Min = 4,
    R7GenericDeviceValueGpxLoggingPeriod5Min = 5,
    R7GenericDeviceValueGpxLoggingPeriod10Min = 6,
    R7GenericDeviceValueGpxLoggingPeriod15Min = 7,
    R7GenericDeviceValueGpxLoggingPeriod20Min = 8,
    R7GenericDeviceValueGpxLoggingPeriod30Min = 9,
    R7GenericDeviceValueGpxLoggingPeriod60Min = 10
};



typedef NS_ENUM(NSUInteger, R7GenericDeviceValueGpxLoggingStatus) {
    R7GenericDeviceValueGpxLoggingStatusOff = 0,
    R7GenericDeviceValueGpxLoggingStatusOn = 1
};



typedef NS_ENUM(NSUInteger, R7GenericDeviceValueMailboxCheckFrequency) {
    R7GenericDeviceValueMailboxCheckFrequency5min = 0,
    R7GenericDeviceValueMailboxCheckFrequency10min = 1,
    R7GenericDeviceValueMailboxCheckFrequency15min = 2,
    R7GenericDeviceValueMailboxCheckFrequency20min = 3,
    R7GenericDeviceValueMailboxCheckFrequency30min = 4,
    R7GenericDeviceValueMailboxCheckFrequency60min = 5,
    R7GenericDeviceValueMailboxCheckFrequency90min = 6,
    R7GenericDeviceValueMailboxCheckFrequency120min = 7,
    R7GenericDeviceValueMailboxCheckFrequency180min = 8,
    R7GenericDeviceValueMailboxCheckFrequency240min = 9,
    R7GenericDeviceValueMailboxCheckFrequency360min = 10,
    R7GenericDeviceValueMailboxCheckFrequency480min = 11,
    R7GenericDeviceValueMailboxCheckFrequency720min = 12
};



typedef NS_ENUM(NSUInteger, R7GenericDeviceValueMailboxCheckStatus) {
    R7GenericDeviceValueMailboxCheckStatusOff = 0,
    R7GenericDeviceValueMailboxCheckStatusOn = 1
};



typedef NS_ENUM(NSUInteger, R7GenericDeviceValueScreenNonActivityThreshold) {
    R7GenericDeviceValueScreenNonActivityThreshold10sec = 0,
    R7GenericDeviceValueScreenNonActivityThreshold20sec = 1,
    R7GenericDeviceValueScreenNonActivityThreshold30sec = 2,
    R7GenericDeviceValueScreenNonActivityThreshold1min = 3
};



typedef NS_ENUM(NSUInteger, R7GenericDeviceValueScreenScreenBrightness) {
    R7GenericDeviceValueScreenScreenBrightness25 = 0,
    R7GenericDeviceValueScreenScreenBrightness50 = 1,
    R7GenericDeviceValueScreenScreenBrightness75 = 2,
    R7GenericDeviceValueScreenScreenBrightness100 = 3
};



typedef NS_ENUM(NSUInteger, R7GenericDeviceValueScreenLockStatus) {
    R7GenericDeviceValueScreenLockStatusOff = 0,
    R7GenericDeviceValueScreenLockStatusOn = 1
};



typedef NS_ENUM(NSUInteger, R7GenericDeviceValueScreenTimeout) {
    R7GenericDeviceValueScreenTimeout10sec = 0,
    R7GenericDeviceValueScreenTimeout20sec = 1,
    R7GenericDeviceValueScreenTimeout30sec = 2,
    R7GenericDeviceValueScreenTimeout1min = 3
};



typedef NS_ENUM(NSUInteger, R7GenericDeviceValueScreenStealthStatus) {
    R7GenericDeviceValueScreenStealthStatusOff = 0,
    R7GenericDeviceValueScreenStealthStatusOn = 1
};



typedef NS_ENUM(NSUInteger, R7GenericDeviceValueSystemEncryptionStatus) {
    R7GenericDeviceValueSystemEncryptionStatusOff = 0,
    R7GenericDeviceValueSystemEncryptionStatusOn = 1
};



typedef NS_ENUM(NSUInteger, R7GenericDeviceValueTrackingActivitySenseStatus) {
    R7GenericDeviceValueTrackingActivitySenseStatusOff = 0,
    R7GenericDeviceValueTrackingActivitySenseStatusOn = 1,  //Power //TS 1.4.0 / LS 3.3.0
    R7GenericDeviceValueTrackingActivitySenseStatusBump = 2,
    R7GenericDeviceValueTrackingActivitySenseStatusSog = 3,
    R7GenericDeviceValueTrackingActivitySenseStatusBumpAndSog = 4,
    R7GenericDeviceValueTrackingActivitySenseStatusAwayFromHome = 5     //Added 09/05/2019
    
};

typedef NS_ENUM(NSUInteger, R7GenericDeviceValueTrackingActivitySenseLowThreshold) {
    R7GenericDeviceValueTrackingActivitySenseLowThreshold0 = 0,
    R7GenericDeviceValueTrackingActivitySenseLowThreshold1 = 1,
    R7GenericDeviceValueTrackingActivitySenseLowThreshold2 = 2,
    R7GenericDeviceValueTrackingActivitySenseLowThreshold3 = 3,
    R7GenericDeviceValueTrackingActivitySenseLowThreshold4 = 4,
    R7GenericDeviceValueTrackingActivitySenseLowThreshold5 = 5,
    R7GenericDeviceValueTrackingActivitySenseLowThreshold6 = 6,
    R7GenericDeviceValueTrackingActivitySenseLowThreshold7 = 7,
    R7GenericDeviceValueTrackingActivitySenseLowThreshold8 = 8,
    R7GenericDeviceValueTrackingActivitySenseLowThreshold9 = 9,
    R7GenericDeviceValueTrackingActivitySenseLowThreshold10 = 10,
    R7GenericDeviceValueTrackingActivitySenseLowThreshold11 = 11,
    R7GenericDeviceValueTrackingActivitySenseLowThreshold12 = 12,
    R7GenericDeviceValueTrackingActivitySenseLowThreshold13 = 13,
    R7GenericDeviceValueTrackingActivitySenseLowThreshold14 = 14,
    R7GenericDeviceValueTrackingActivitySenseLowThreshold15 = 15,
    R7GenericDeviceValueTrackingActivitySenseLowThreshold16 = 16,
    R7GenericDeviceValueTrackingActivitySenseLowThreshold17 = 17,
    R7GenericDeviceValueTrackingActivitySenseLowThreshold18 = 18,
    R7GenericDeviceValueTrackingActivitySenseLowThreshold19 = 19,
    R7GenericDeviceValueTrackingActivitySenseLowThreshold20 = 20,
    R7GenericDeviceValueTrackingActivitySenseLowThreshold21 = 21,
    R7GenericDeviceValueTrackingActivitySenseLowThreshold22 = 22,
    R7GenericDeviceValueTrackingActivitySenseLowThreshold23 = 23,
    R7GenericDeviceValueTrackingActivitySenseLowThreshold24 = 24,
    R7GenericDeviceValueTrackingActivitySenseLowThreshold25 = 25,
    R7GenericDeviceValueTrackingActivitySenseLowThreshold26 = 26,
    R7GenericDeviceValueTrackingActivitySenseLowThreshold27 = 27,
    R7GenericDeviceValueTrackingActivitySenseLowThreshold28 = 28,
    R7GenericDeviceValueTrackingActivitySenseLowThreshold29 = 29,
    R7GenericDeviceValueTrackingActivitySenseLowThreshold30 = 30,
    R7GenericDeviceValueTrackingActivitySenseLowThreshold31 = 31,
    R7GenericDeviceValueTrackingActivitySenseLowThreshold32 = 32
};


typedef NS_ENUM(NSUInteger, R7GenericDeviceValueTrackingActivitySenseHighThreshold) {
    R7GenericDeviceValueTrackingActivitySenseHighThreshold0 = 0,
    R7GenericDeviceValueTrackingActivitySenseHighThreshold1 = 1,
    R7GenericDeviceValueTrackingActivitySenseHighThreshold2 = 2,
    R7GenericDeviceValueTrackingActivitySenseHighThreshold3 = 3,
    R7GenericDeviceValueTrackingActivitySenseHighThreshold4 = 4,
    R7GenericDeviceValueTrackingActivitySenseHighThreshold5 = 5,
    R7GenericDeviceValueTrackingActivitySenseHighThreshold6 = 6,
    R7GenericDeviceValueTrackingActivitySenseHighThreshold7 = 7,
    R7GenericDeviceValueTrackingActivitySenseHighThreshold8 = 8,
    R7GenericDeviceValueTrackingActivitySenseHighThreshold9 = 9,
    R7GenericDeviceValueTrackingActivitySenseHighThreshold10 = 10,
    R7GenericDeviceValueTrackingActivitySenseHighThreshold11 = 11,
    R7GenericDeviceValueTrackingActivitySenseHighThreshold12 = 12,
    R7GenericDeviceValueTrackingActivitySenseHighThreshold13 = 13,
    R7GenericDeviceValueTrackingActivitySenseHighThreshold14 = 14,
    R7GenericDeviceValueTrackingActivitySenseHighThreshold15 = 15,
    R7GenericDeviceValueTrackingActivitySenseHighThreshold16 = 16,
    R7GenericDeviceValueTrackingActivitySenseHighThreshold17 = 17,
    R7GenericDeviceValueTrackingActivitySenseHighThreshold18 = 18,
    R7GenericDeviceValueTrackingActivitySenseHighThreshold19 = 19,
    R7GenericDeviceValueTrackingActivitySenseHighThreshold20 = 20,
    R7GenericDeviceValueTrackingActivitySenseHighThreshold21 = 21,
    R7GenericDeviceValueTrackingActivitySenseHighThreshold22 = 22,
    R7GenericDeviceValueTrackingActivitySenseHighThreshold23 = 23,
    R7GenericDeviceValueTrackingActivitySenseHighThreshold24 = 24,
    R7GenericDeviceValueTrackingActivitySenseHighThreshold25 = 25,
    R7GenericDeviceValueTrackingActivitySenseHighThreshold26 = 26,
    R7GenericDeviceValueTrackingActivitySenseHighThreshold27 = 27,
    R7GenericDeviceValueTrackingActivitySenseHighThreshold28 = 28,
    R7GenericDeviceValueTrackingActivitySenseHighThreshold29 = 29,
    R7GenericDeviceValueTrackingActivitySenseHighThreshold30 = 30,
    R7GenericDeviceValueTrackingActivitySenseHighThreshold31 = 31,
    R7GenericDeviceValueTrackingActivitySenseHighThreshold32 = 32
};

typedef NS_ENUM(NSUInteger, R7GenericDeviceValueTrackingActivitySenseThreshold) {
    R7GenericDeviceValueTrackingActivitySenseThreshold18 = 0,
    R7GenericDeviceValueTrackingActivitySenseThreshold19 = 1,
    R7GenericDeviceValueTrackingActivitySenseThreshold20 = 2,
    R7GenericDeviceValueTrackingActivitySenseThreshold21 = 3,
    R7GenericDeviceValueTrackingActivitySenseThreshold22 = 4,
    R7GenericDeviceValueTrackingActivitySenseThreshold23 = 5,
    R7GenericDeviceValueTrackingActivitySenseThreshold24 = 6,
    R7GenericDeviceValueTrackingActivitySenseThreshold25 = 7,
    R7GenericDeviceValueTrackingActivitySenseThreshold26 = 8,
    R7GenericDeviceValueTrackingActivitySenseThreshold27 = 9,
    R7GenericDeviceValueTrackingActivitySenseThreshold28 = 10,
    R7GenericDeviceValueTrackingActivitySenseThreshold29 = 11,
    R7GenericDeviceValueTrackingActivitySenseThreshold30 = 12,
    R7GenericDeviceValueTrackingActivitySenseThreshold31 = 13,
    R7GenericDeviceValueTrackingActivitySenseThreshold32 = 14,
    R7GenericDeviceValueTrackingActivitySenseThreshold33 = 15,
    R7GenericDeviceValueTrackingActivitySenseThreshold34 = 16,
    R7GenericDeviceValueTrackingActivitySenseThreshold35 = 17,
    R7GenericDeviceValueTrackingActivitySenseThreshold36 = 18,
    R7GenericDeviceValueTrackingActivitySenseThreshold37 = 19,
    R7GenericDeviceValueTrackingActivitySenseThreshold38 = 20,
    R7GenericDeviceValueTrackingActivitySenseThreshold39 = 21,
    R7GenericDeviceValueTrackingActivitySenseThreshold40 = 22,
    R7GenericDeviceValueTrackingActivitySenseThreshold41 = 23,
    R7GenericDeviceValueTrackingActivitySenseThreshold42 = 24,
    R7GenericDeviceValueTrackingActivitySenseThreshold43 = 25,
    R7GenericDeviceValueTrackingActivitySenseThreshold44 = 26,
    R7GenericDeviceValueTrackingActivitySenseThreshold45 = 27,
    R7GenericDeviceValueTrackingActivitySenseThreshold46 = 28,
    R7GenericDeviceValueTrackingActivitySenseThreshold47 = 29,
    R7GenericDeviceValueTrackingActivitySenseThreshold48 = 30,
    R7GenericDeviceValueTrackingActivitySenseThreshold49 = 31,
    R7GenericDeviceValueTrackingActivitySenseThreshold50 = 32,
    R7GenericDeviceValueTrackingActivitySenseThreshold51 = 33,
    R7GenericDeviceValueTrackingActivitySenseThreshold52 = 34,
    R7GenericDeviceValueTrackingActivitySenseThreshold53 = 35,
    R7GenericDeviceValueTrackingActivitySenseThreshold54 = 36
};

typedef NS_ENUM(NSUInteger, R7GenericDeviceValueTrackingBurstFixPeriod) {
    R7GenericDeviceValueTrackingBurstFixPeriod5sec = 0,
    R7GenericDeviceValueTrackingBurstFixPeriod10sec = 1,
    R7GenericDeviceValueTrackingBurstFixPeriod15sec = 2,
    R7GenericDeviceValueTrackingBurstFixPeriod20sec = 3,
    R7GenericDeviceValueTrackingBurstFixPeriod30sec = 4,
    R7GenericDeviceValueTrackingBurstFixPeriod1min = 5,
    R7GenericDeviceValueTrackingBurstFixPeriod2min = 6,
    R7GenericDeviceValueTrackingBurstFixPeriod5min = 7,
    R7GenericDeviceValueTrackingBurstFixPeriod10min = 8,
    R7GenericDeviceValueTrackingBurstFixPeriod15min = 9,
    R7GenericDeviceValueTrackingBurstFixPeriod20min = 10
};



typedef NS_ENUM(NSUInteger, R7GenericDeviceValueTrackingBurstTransmitPeriod) {
    R7GenericDeviceValueTrackingBurstTransmitPeriod1min = 0,
    R7GenericDeviceValueTrackingBurstTransmitPeriod2min = 1,
    R7GenericDeviceValueTrackingBurstTransmitPeriod5min = 2,
    R7GenericDeviceValueTrackingBurstTransmitPeriod10min = 3,
    R7GenericDeviceValueTrackingBurstTransmitPeriod15min = 4,
    R7GenericDeviceValueTrackingBurstTransmitPeriod30min = 5,
    R7GenericDeviceValueTrackingBurstTransmitPeriod60min = 6
};



typedef NS_ENUM(NSUInteger, R7GenericDeviceValueTrackingFrequency) {
    
    R7GenericDeviceValueTrackingFrequencyContinuous = 0,
    
    R7GenericDeviceValueTrackingFrequency5min = 1,
    R7GenericDeviceValueTrackingFrequency10min = 2,
    R7GenericDeviceValueTrackingFrequency15min = 3,
    R7GenericDeviceValueTrackingFrequency20min = 4,
    R7GenericDeviceValueTrackingFrequency30min = 5,
    R7GenericDeviceValueTrackingFrequency60min = 6,
    R7GenericDeviceValueTrackingFrequency90min = 7,
    R7GenericDeviceValueTrackingFrequency120min = 8,
    R7GenericDeviceValueTrackingFrequency180min = 9,
    R7GenericDeviceValueTrackingFrequency240min = 10,
    R7GenericDeviceValueTrackingFrequency360min = 11,
    R7GenericDeviceValueTrackingFrequency480min = 12,
    R7GenericDeviceValueTrackingFrequency720min = 13,
    R7GenericDeviceValueTrackingFrequencyBurst = 14,
    R7GenericDeviceValueTrackingFrequency1min = 15,
    R7GenericDeviceValueTrackingFrequency2min = 16,
    R7GenericDeviceValueTrackingFrequency3min = 17,
    R7GenericDeviceValueTrackingFrequency4min = 18,
    R7GenericDeviceValueTrackingFrequency1440min = 19,
    R7GenericDeviceValueTrackingFrequency6min = 20,     //Added 19/01/2017
    R7GenericDeviceValueTrackingFrequency8min = 21,
    R7GenericDeviceValueTrackingFrequency12min = 22,
    
    R7GenericDeviceValueTrackingFrequency15sec = 23,    //Added 20/04/2017
    R7GenericDeviceValueTrackingFrequency30sec = 24
};

typedef NS_ENUM(NSUInteger, R7GenericDeviceValueTrackingStatus) {
    R7GenericDeviceValueTrackingStatusOff = 0,
    R7GenericDeviceValueTrackingStatusOn = 1
};

typedef NS_ENUM(NSUInteger, R7GenericDeviceValueGprsStrategy) {
    R7GenericDeviceValueGprsStrategyNever = 0,
    R7GenericDeviceValueGprsStrategyAlways = 1,
    R7GenericDeviceValueGprsStrategyPreferred = 2
};

typedef NS_ENUM(NSUInteger, R7GenericDeviceValueDaughterboardStatus) {
    R7GenericDeviceValueDaughterboardStatusNone = 0,
    R7GenericDeviceValueDaughterboardStatusGprs = 1
};

typedef NS_ENUM(NSUInteger, R7GenericDeviceValueGprsStatus) {
    R7GenericDeviceValueGprsStatusGsm = 0,
    R7GenericDeviceValueGprsStatusGsmCompact = 1,
    R7GenericDeviceValueGprsStatusUmts = 2,
    R7GenericDeviceValueGprsStatusGsmEdge = 3,
    R7GenericDeviceValueGprsStatusUmtsHsdpa = 4,
    R7GenericDeviceValueGprsStatusUmtsHsupa = 5,
    R7GenericDeviceValueGprsStatusUmtsHsdpaHsupa = 6,
    R7GenericDeviceValueGprsStatusLte = 7
};

typedef NS_ENUM(NSUInteger, R7GenericDeviceValueInputSensitivity) {
    R7GenericDeviceValueInputSensitivityFast0123 = 0,
    R7GenericDeviceValueInputSensitivityFast012 = 1,
    R7GenericDeviceValueInputSensitivityFast01 = 2,
    R7GenericDeviceValueInputSensitivityFast0 = 3,
    R7GenericDeviceValueInputSensitivityAllSlow = 4
};

//GPRS
typedef NS_ENUM(NSUInteger, R7GenericDeviceGprsParameter) {
    
    R7GenericDeviceGprsParameterApnName = 0,
    R7GenericDeviceGprsParameterEndpointAddress1 = 1,
    R7GenericDeviceGprsParameterEndpointAddress2 = 2,
    R7GenericDeviceGprsParameterEndpointAddress3 = 3,
    R7GenericDeviceGprsParameterEndpointPort1 = 4,
    R7GenericDeviceGprsParameterEndpointPort2 = 5,
    R7GenericDeviceGprsParameterEndpointPort3 = 6,
    R7GenericDeviceGprsParameterApnUsername = 7,
    R7GenericDeviceGprsParameterApnPassword = 8            //UPDATE
    
};




//--------
typedef NS_ENUM(NSUInteger, R7GenericDeviceValueCellularFrequency) {
    
    R7GenericDeviceValueCellularFrequencyContinuous = 0,
    R7GenericDeviceValueCellularFrequency5min = 1,
    R7GenericDeviceValueCellularFrequency10min = 2,
    R7GenericDeviceValueCellularFrequency15min = 3,
    R7GenericDeviceValueCellularFrequency20min = 4,
    R7GenericDeviceValueCellularFrequency30min = 5,
    R7GenericDeviceValueCellularFrequency60min = 6,
    R7GenericDeviceValueCellularFrequency90min = 7,
    R7GenericDeviceValueCellularFrequency120min = 8,
    R7GenericDeviceValueCellularFrequency180min = 9,
    R7GenericDeviceValueCellularFrequency240min = 10,
    R7GenericDeviceValueCellularFrequency360min = 11,
    R7GenericDeviceValueCellularFrequency480min = 12,
    R7GenericDeviceValueCellularFrequency720min = 13,
    R7GenericDeviceValueCellularFrequencyBurst = 14,
    R7GenericDeviceValueCellularFrequency1min = 15,
    R7GenericDeviceValueCellularFrequency2min = 16,
    R7GenericDeviceValueCellularFrequency3min = 17,
    R7GenericDeviceValueCellularFrequency4min = 18,
    R7GenericDeviceValueCellularFrequency1440min = 19,
    R7GenericDeviceValueCellularFrequency6min = 20,             //Added 19/01/2017
    R7GenericDeviceValueCellularFrequency8min = 21,
    R7GenericDeviceValueCellularFrequency12min = 22,
    R7GenericDeviceValueCellularFrequency15sec = 23,            //Added 19/04/2017
    R7GenericDeviceValueCellularFrequency30sec = 24             //Added 19/04/2017
    
};

typedef NS_ENUM(NSUInteger, R7GenericDeviceValueCellularBurstFixPeriod) {
    R7GenericDeviceValueCellularBurstFixPeriod5sec = 0,
    R7GenericDeviceValueCellularBurstFixPeriod10sec = 1,
    R7GenericDeviceValueCellularBurstFixPeriod15sec = 2,
    R7GenericDeviceValueCellularBurstFixPeriod20sec = 3,
    R7GenericDeviceValueCellularBurstFixPeriod30sec = 4,
    R7GenericDeviceValueCellularBurstFixPeriod1min = 5,
    R7GenericDeviceValueCellularBurstFixPeriod2min = 6,
    R7GenericDeviceValueCellularBurstFixPeriod5min = 7,
    R7GenericDeviceValueCellularBurstFixPeriod10min = 8,
    R7GenericDeviceValueCellularBurstFixPeriod15min = 9,
    R7GenericDeviceValueCellularBurstFixPeriod20min = 10
};

typedef NS_ENUM(NSUInteger, R7GenericDeviceValueCellularBurstTransmitPeriod) {
    R7GenericDeviceValueCellularBurstTransmitPeriod1min = 0,
    R7GenericDeviceValueCellularBurstTransmitPeriod2min = 1,
    R7GenericDeviceValueCellularBurstTransmitPeriod5min = 2,
    R7GenericDeviceValueCellularBurstTransmitPeriod10min = 3,
    R7GenericDeviceValueCellularBurstTransmitPeriod15min = 4,
    R7GenericDeviceValueCellularBurstTransmitPeriod30min = 5,
    R7GenericDeviceValueCellularBurstTransmitPeriod60min = 6
};


//-----
typedef NS_ENUM(NSUInteger, R7GenericDeviceValueDistressFrequency) {
    
    R7GenericDeviceValueDistressFrequencyContinuous = 0,
    R7GenericDeviceValueDistressFrequency5min = 1,
    R7GenericDeviceValueDistressFrequency10min = 2,
    R7GenericDeviceValueDistressFrequency15min = 3,
    R7GenericDeviceValueDistressFrequency20min = 4,
    R7GenericDeviceValueDistressFrequency30min = 5,
    R7GenericDeviceValueDistressFrequency60min = 6,
    R7GenericDeviceValueDistressFrequency90min = 7,
    R7GenericDeviceValueDistressFrequency120min = 8,
    R7GenericDeviceValueDistressFrequency180min = 9,
    R7GenericDeviceValueDistressFrequency240min = 10,
    R7GenericDeviceValueDistressFrequency360min = 11,
    R7GenericDeviceValueDistressFrequency480min = 12,
    R7GenericDeviceValueDistressFrequency720min = 13,
    R7GenericDeviceValueDistressFrequencyBurst = 14,
    R7GenericDeviceValueDistressFrequency1min = 15,
    R7GenericDeviceValueDistressFrequency2min = 16,
    R7GenericDeviceValueDistressFrequency3min = 17,
    R7GenericDeviceValueDistressFrequency4min = 18,
    R7GenericDeviceValueDistressFrequency1440min = 19,
    R7GenericDeviceValueDistressFrequency6min = 20,            //Added 19/01/2017
    R7GenericDeviceValueDistressFrequency8min = 21,
    R7GenericDeviceValueDistressFrequency12min = 22,
    R7GenericDeviceValueDistressFrequency15sec = 23,            //Added 20/04/2017
    R7GenericDeviceValueDistressFrequency30sec = 24

};

typedef NS_ENUM(NSUInteger, R7GenericDeviceValueDistressBurstFixPeriod) {
    R7GenericDeviceValueDistressBurstFixPeriod5sec = 0,
    R7GenericDeviceValueDistressBurstFixPeriod10sec = 1,
    R7GenericDeviceValueDistressBurstFixPeriod15sec = 2,
    R7GenericDeviceValueDistressBurstFixPeriod20sec = 3,
    R7GenericDeviceValueDistressBurstFixPeriod30sec = 4,
    R7GenericDeviceValueDistressBurstFixPeriod1min = 5,
    R7GenericDeviceValueDistressBurstFixPeriod2min = 6,
    R7GenericDeviceValueDistressBurstFixPeriod5min = 7,
    R7GenericDeviceValueDistressBurstFixPeriod10min = 8,
    R7GenericDeviceValueDistressBurstFixPeriod15min = 9,
    R7GenericDeviceValueDistressBurstFixPeriod20min = 10
};

typedef NS_ENUM(NSUInteger, R7GenericDeviceValueDistressBurstTransmitPeriod) {
    R7GenericDeviceValueDistressBurstTransmitPeriod1min = 0,
    R7GenericDeviceValueDistressBurstTransmitPeriod2min = 1,
    R7GenericDeviceValueDistressBurstTransmitPeriod5min = 2,
    R7GenericDeviceValueDistressBurstTransmitPeriod10min = 3,
    R7GenericDeviceValueDistressBurstTransmitPeriod15min = 4,
    R7GenericDeviceValueDistressBurstTransmitPeriod30min = 5,
    R7GenericDeviceValueDistressBurstTransmitPeriod60min = 6
};


typedef NS_ENUM(NSUInteger, R7GenericDeviceValueTransmissionFormat) {
    R7GenericDeviceValueTransmissionFormatStandard = 0,
    R7GenericDeviceValueTransmissionFormatCompact = 1,
    R7GenericDeviceValueTransmissionFormatAes = 2           //Not in App
};


typedef NS_ENUM(NSUInteger, R7GenericDeviceValueAscentDescentAlertMode) {
    
    R7GenericDeviceValueAscentDescentAlertModeOff = 0,
    R7GenericDeviceValueAscentDescentAlertModeDesent = 1,
    R7GenericDeviceValueAscentDescentAlertModeAsent = 2,
    R7GenericDeviceValueAscentDescentAlertModeBoth = 3
    
};

typedef NS_ENUM(NSUInteger, R7GenericDeviceValueAlertThreshold) {
    
    R7GenericDeviceValueAscentAlertThreshold1000 = 0,
    R7GenericDeviceValueAscentAlertThreshold1500 = 1,
    R7GenericDeviceValueAscentAlertThreshold2000 = 2,
    R7GenericDeviceValueAscentAlertThreshold2500 = 3,
    R7GenericDeviceValueAscentAlertThreshold3000 = 4
    
};

typedef NS_ENUM(NSUInteger, R7GenericDeviceValueDescentAlertThreshold) {
    
    R7GenericDeviceValueDescentAlertThreshold1000 = 0,
    R7GenericDeviceValueDescentAlertThreshold1500 = 1,
    R7GenericDeviceValueDescentAlertThreshold2000 = 2,
    R7GenericDeviceValueDescentAlertThreshold2500 = 3,
    R7GenericDeviceValueDescentAlertThreshold3000 = 4
    
};

typedef NS_ENUM(NSUInteger, R7GenericDeviceValueAscentAlertTimePeriod) {
    
    R7GenericDeviceValueAscentAlertTimePeriod5sec = 0,
    R7GenericDeviceValueAscentAlertTimePeriod10sec = 1,
    R7GenericDeviceValueAscentAlertTimePeriod15sec = 2,
    R7GenericDeviceValueAscentAlertTimePeriod20sec = 3,
    R7GenericDeviceValueAscentAlertTimePeriod25sec = 4,
    R7GenericDeviceValueAscentAlertTimePeriod30sec = 5
    
};

typedef NS_ENUM(NSUInteger, R7GenericDeviceValueDescentAlertTimePeriod) {
    
    R7GenericDeviceValueDescentAlertTimePeriod5sec = 0,
    R7GenericDeviceValueDescentAlertTimePeriod10sec = 1,
    R7GenericDeviceValueDescentAlertTimePeriod15sec = 2,
    R7GenericDeviceValueDescentAlertTimePeriod20sec = 3,
    R7GenericDeviceValueDescentAlertTimePeriod25sec = 4,
    R7GenericDeviceValueDescentAlertTimePeriod30sec = 5
    
};

typedef NS_ENUM(NSUInteger, R7GenericDeviceValueGpsDynamicPlatformModel) {
    
    R7GenericDeviceValueGpsDynamicPlatformModelPortable = 0,
    R7GenericDeviceValueGpsDynamicPlatformModelAutomotive = 1,
    R7GenericDeviceValueGpsDynamicPlatformModelMarine = 2,
    R7GenericDeviceValueGpsDynamicPlatformModelAirborne = 3
    
};

typedef NS_ENUM(NSUInteger, R7GenericDeviceValueAutoResumeStatus) {
    
    R7GenericDeviceValueAutoResumeStatusOff = 0,
    R7GenericDeviceValueAutoResumeStatusOn = 1
    
};

typedef NS_ENUM(NSUInteger, R7GenericDeviceValueAutoResumeDistance) {
    
    R7GenericDeviceValueAutoResumeDistance2km = 0,
    R7GenericDeviceValueAutoResumeDistance3km = 1,
    R7GenericDeviceValueAutoResumeDistance5km = 2,
    R7GenericDeviceValueAutoResumeDistance10km = 3
    
};

typedef NS_ENUM(NSUInteger, R7GenericDeviceValueHoverAlertStatus) {
    
    R7GenericDeviceValueHoverAlertStatusOff = 0,
    R7GenericDeviceValueHoverAlertStatusHover = 1,
    R7GenericDeviceValueHoverAlertStatusFixedWing = 2
    
};

typedef NS_ENUM(NSUInteger, R7GenericDeviceValueDebugLogging) {
    
    R7GenericDeviceValueDebugLoggingNone = 0,
    R7GenericDeviceValueDebugLoggingInternal= 1,
    R7GenericDeviceValueDebugLoggingExternal = 2
    
};

typedef NS_ENUM(NSUInteger, R7GenericDeviceValueAirborneVRotate) {
    
    R7GenericDeviceValueAirborneVRotate0 = 0,
    R7GenericDeviceValueAirborneVRotate2 = 1,
    R7GenericDeviceValueAirborneVRotate4 = 2,
    R7GenericDeviceValueAirborneVRotate6 = 3,
    R7GenericDeviceValueAirborneVRotate8 = 4,
    R7GenericDeviceValueAirborneVRotate10 = 5,
    R7GenericDeviceValueAirborneVRotate12 = 6,
    R7GenericDeviceValueAirborneVRotate14 = 7,
    R7GenericDeviceValueAirborneVRotate16 = 8,
    R7GenericDeviceValueAirborneVRotate18 = 9,
    R7GenericDeviceValueAirborneVRotate20 = 10,
    R7GenericDeviceValueAirborneVRotate22 = 11,
    R7GenericDeviceValueAirborneVRotate24 = 12,
    R7GenericDeviceValueAirborneVRotate26 = 13,
    R7GenericDeviceValueAirborneVRotate28 = 14,
    R7GenericDeviceValueAirborneVRotate30 = 15,
    R7GenericDeviceValueAirborneVRotate32 = 16,
    R7GenericDeviceValueAirborneVRotate34 = 17,
    R7GenericDeviceValueAirborneVRotate36 = 18,
    R7GenericDeviceValueAirborneVRotate38 = 19,
    R7GenericDeviceValueAirborneVRotate40 = 20,
    R7GenericDeviceValueAirborneVRotate42 = 21,
    R7GenericDeviceValueAirborneVRotate44 = 22,
    R7GenericDeviceValueAirborneVRotate46 = 23,
    R7GenericDeviceValueAirborneVRotate48 = 24,
    R7GenericDeviceValueAirborneVRotate50 = 25,
    R7GenericDeviceValueAirborneVRotate52 = 26,
    R7GenericDeviceValueAirborneVRotate54 = 27,
    R7GenericDeviceValueAirborneVRotate56 = 28,
    R7GenericDeviceValueAirborneVRotate58 = 29,
    R7GenericDeviceValueAirborneVRotate60 = 30,
    R7GenericDeviceValueAirborneVRotate62 = 31,
    R7GenericDeviceValueAirborneVRotate64 = 32,
    R7GenericDeviceValueAirborneVRotate66 = 33,
    R7GenericDeviceValueAirborneVRotate68 = 34,
    R7GenericDeviceValueAirborneVRotate70 = 35,
    R7GenericDeviceValueAirborneVRotate72 = 36,
    R7GenericDeviceValueAirborneVRotate74 = 37,
    R7GenericDeviceValueAirborneVRotate76 = 38,
    R7GenericDeviceValueAirborneVRotate78 = 39,
    R7GenericDeviceValueAirborneVRotate80 = 40,
    R7GenericDeviceValueAirborneVRotate82 = 41,
    R7GenericDeviceValueAirborneVRotate84 = 42,
    R7GenericDeviceValueAirborneVRotate86 = 43,
    R7GenericDeviceValueAirborneVRotate88 = 44,
    R7GenericDeviceValueAirborneVRotate90 = 45,
    R7GenericDeviceValueAirborneVRotate92 = 46,
    R7GenericDeviceValueAirborneVRotate94 = 47,
    R7GenericDeviceValueAirborneVRotate96 = 48,
    R7GenericDeviceValueAirborneVRotate98 = 49,
    R7GenericDeviceValueAirborneVRotate100 = 50,
    R7GenericDeviceValueAirborneVRotate102 = 51,
    R7GenericDeviceValueAirborneVRotate104 = 52,
    R7GenericDeviceValueAirborneVRotate106 = 53,
    R7GenericDeviceValueAirborneVRotate108 = 54,
    R7GenericDeviceValueAirborneVRotate110 = 55,
    R7GenericDeviceValueAirborneVRotate112 = 56,
    R7GenericDeviceValueAirborneVRotate114 = 57,
    R7GenericDeviceValueAirborneVRotate116 = 58,
    R7GenericDeviceValueAirborneVRotate118 = 59,
    R7GenericDeviceValueAirborneVRotate120 = 60,
    R7GenericDeviceValueAirborneVRotate122 = 61,
    R7GenericDeviceValueAirborneVRotate124 = 62,
    R7GenericDeviceValueAirborneVRotate126 = 63,
    R7GenericDeviceValueAirborneVRotate128 = 64,
    R7GenericDeviceValueAirborneVRotate130 = 65,
    R7GenericDeviceValueAirborneVRotate132 = 66,
    R7GenericDeviceValueAirborneVRotate134 = 67,
    R7GenericDeviceValueAirborneVRotate136 = 68,
    R7GenericDeviceValueAirborneVRotate138 = 69,
    R7GenericDeviceValueAirborneVRotate140 = 70,
    R7GenericDeviceValueAirborneVRotate142 = 71,
    R7GenericDeviceValueAirborneVRotate144 = 72,
    R7GenericDeviceValueAirborneVRotate146 = 73,
    R7GenericDeviceValueAirborneVRotate148 = 74,
    R7GenericDeviceValueAirborneVRotate150 = 75,
    R7GenericDeviceValueAirborneVRotate152 = 76,
    R7GenericDeviceValueAirborneVRotate154 = 77,
    R7GenericDeviceValueAirborneVRotate156 = 78,
    R7GenericDeviceValueAirborneVRotate158 = 79,
    R7GenericDeviceValueAirborneVRotate160 = 80,
    R7GenericDeviceValueAirborneVRotate162 = 81,
    R7GenericDeviceValueAirborneVRotate164 = 82,
    R7GenericDeviceValueAirborneVRotate166 = 83,
    R7GenericDeviceValueAirborneVRotate168 = 84,
    R7GenericDeviceValueAirborneVRotate170 = 85,
    R7GenericDeviceValueAirborneVRotate172 = 86,
    R7GenericDeviceValueAirborneVRotate174 = 87,
    R7GenericDeviceValueAirborneVRotate176 = 88,
    R7GenericDeviceValueAirborneVRotate178 = 89,
    R7GenericDeviceValueAirborneVRotate180 = 90,
    R7GenericDeviceValueAirborneVRotate182 = 91,
    R7GenericDeviceValueAirborneVRotate184 = 92,
    R7GenericDeviceValueAirborneVRotate186 = 93,
    R7GenericDeviceValueAirborneVRotate188 = 94,
    R7GenericDeviceValueAirborneVRotate190 = 95,
    R7GenericDeviceValueAirborneVRotate192 = 96,
    R7GenericDeviceValueAirborneVRotate194 = 97,
    R7GenericDeviceValueAirborneVRotate196 = 98,
    R7GenericDeviceValueAirborneVRotate198 = 99,
    R7GenericDeviceValueAirborneVRotate200 = 100,
    R7GenericDeviceValueAirborneVRotate202 = 101,
    R7GenericDeviceValueAirborneVRotate204 = 102,
    R7GenericDeviceValueAirborneVRotate206 = 103,
    R7GenericDeviceValueAirborneVRotate208 = 104,
    R7GenericDeviceValueAirborneVRotate210 = 105,
    R7GenericDeviceValueAirborneVRotate212 = 106,
    R7GenericDeviceValueAirborneVRotate214 = 107,
    R7GenericDeviceValueAirborneVRotate216 = 108,
    R7GenericDeviceValueAirborneVRotate218 = 109,
    R7GenericDeviceValueAirborneVRotate220 = 110,
    R7GenericDeviceValueAirborneVRotate222 = 111,
    R7GenericDeviceValueAirborneVRotate224 = 112,
    R7GenericDeviceValueAirborneVRotate226 = 113,
    R7GenericDeviceValueAirborneVRotate228 = 114,
    R7GenericDeviceValueAirborneVRotate230 = 115,
    R7GenericDeviceValueAirborneVRotate232 = 116,
    R7GenericDeviceValueAirborneVRotate234 = 117,
    R7GenericDeviceValueAirborneVRotate236 = 118,
    R7GenericDeviceValueAirborneVRotate238 = 119,
    R7GenericDeviceValueAirborneVRotate240 = 120,
    R7GenericDeviceValueAirborneVRotate242 = 121,
    R7GenericDeviceValueAirborneVRotate244 = 122,
    R7GenericDeviceValueAirborneVRotate246 = 123,
    R7GenericDeviceValueAirborneVRotate248 = 124,
    R7GenericDeviceValueAirborneVRotate250 = 125,
    R7GenericDeviceValueAirborneVRotate252 = 126
};

typedef NS_ENUM(NSUInteger, R7GenericDeviceValueAirborneVStall) {
    
    R7GenericDeviceValueAirborneVStall0 = 0,
    R7GenericDeviceValueAirborneVStall2 = 1,
    R7GenericDeviceValueAirborneVStall4 = 2,
    R7GenericDeviceValueAirborneVStall6 = 3,
    R7GenericDeviceValueAirborneVStall8 = 4,
    R7GenericDeviceValueAirborneVStall10 = 5,
    R7GenericDeviceValueAirborneVStall12 = 6,
    R7GenericDeviceValueAirborneVStall14 = 7,
    R7GenericDeviceValueAirborneVStall16 = 8,
    R7GenericDeviceValueAirborneVStall18 = 9,
    R7GenericDeviceValueAirborneVStall20 = 10,
    R7GenericDeviceValueAirborneVStall22 = 11,
    R7GenericDeviceValueAirborneVStall24 = 12,
    R7GenericDeviceValueAirborneVStall26 = 13,
    R7GenericDeviceValueAirborneVStall28 = 14,
    R7GenericDeviceValueAirborneVStall30 = 15,
    R7GenericDeviceValueAirborneVStall32 = 16,
    R7GenericDeviceValueAirborneVStall34 = 17,
    R7GenericDeviceValueAirborneVStall36 = 18,
    R7GenericDeviceValueAirborneVStall38 = 19,
    R7GenericDeviceValueAirborneVStall40 = 20,
    R7GenericDeviceValueAirborneVStall42 = 21,
    R7GenericDeviceValueAirborneVStall44 = 22,
    R7GenericDeviceValueAirborneVStall46 = 23,
    R7GenericDeviceValueAirborneVStall48 = 24,
    R7GenericDeviceValueAirborneVStall50 = 25,
    R7GenericDeviceValueAirborneVStall52 = 26,
    R7GenericDeviceValueAirborneVStall54 = 27,
    R7GenericDeviceValueAirborneVStall56 = 28,
    R7GenericDeviceValueAirborneVStall58 = 29,
    R7GenericDeviceValueAirborneVStall60 = 30,
    R7GenericDeviceValueAirborneVStall62 = 31,
    R7GenericDeviceValueAirborneVStall64 = 32,
    R7GenericDeviceValueAirborneVStall66 = 33,
    R7GenericDeviceValueAirborneVStall68 = 34,
    R7GenericDeviceValueAirborneVStall70 = 35,
    R7GenericDeviceValueAirborneVStall72 = 36,
    R7GenericDeviceValueAirborneVStall74 = 37,
    R7GenericDeviceValueAirborneVStall76 = 38,
    R7GenericDeviceValueAirborneVStall78 = 39,
    R7GenericDeviceValueAirborneVStall80 = 40,
    R7GenericDeviceValueAirborneVStall82 = 41,
    R7GenericDeviceValueAirborneVStall84 = 42,
    R7GenericDeviceValueAirborneVStall86 = 43,
    R7GenericDeviceValueAirborneVStall88 = 44,
    R7GenericDeviceValueAirborneVStall90 = 45,
    R7GenericDeviceValueAirborneVStall92 = 46,
    R7GenericDeviceValueAirborneVStall94 = 47,
    R7GenericDeviceValueAirborneVStall96 = 48,
    R7GenericDeviceValueAirborneVStall98 = 49,
    R7GenericDeviceValueAirborneVStall100 = 50,
    R7GenericDeviceValueAirborneVStall102 = 51,
    R7GenericDeviceValueAirborneVStall104 = 52,
    R7GenericDeviceValueAirborneVStall106 = 53,
    R7GenericDeviceValueAirborneVStall108 = 54,
    R7GenericDeviceValueAirborneVStall110 = 55,
    R7GenericDeviceValueAirborneVStall112 = 56,
    R7GenericDeviceValueAirborneVStall114 = 57,
    R7GenericDeviceValueAirborneVStall116 = 58,
    R7GenericDeviceValueAirborneVStall118 = 59,
    R7GenericDeviceValueAirborneVStall120 = 60,
    R7GenericDeviceValueAirborneVStall122 = 61,
    R7GenericDeviceValueAirborneVStall124 = 62
    
};

typedef NS_ENUM(NSUInteger, R7GenericDeviceValueWatchMeState) {
    
    R7GenericDeviceValueWatchMeStateOff = 0,
    R7GenericDeviceValueWatchMeStateOnRequestSent = 1,
    R7GenericDeviceValueWatchMeStateOn = 2,
    R7GenericDeviceValueWatchMeStateOffRequestSent = 3
    
};

typedef NS_ENUM(NSUInteger, R7GenericDeviceValueContext) {
    
    R7GenericDeviceValueContextOff = 0,
    R7GenericDeviceValueContextDistress = 1,    //Deprecated
    R7GenericDeviceValueContextCellular = 2,
    R7GenericDeviceValueContextSatellite = 3,
    R7GenericDeviceValueContextCellularDistress = 4,
    R7GenericDeviceValueContextSatelliteDistress = 5
    
};

@end
