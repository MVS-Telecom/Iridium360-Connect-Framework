using ObjCRuntime;

namespace ConnectFramework
{
	[Native]
	public enum R7ActivationState : ulong
	{
		Unknown = 0,
		Pending = 1,
		Activating = 2,
		Activated = 3,
		Error = 99
	}

	[Native]
	public enum R7ActivationError : ulong
	{
		Timeout = 1,
		Authentication = 2,
		Communication = 3,
		State = 4,
		Capability = 5
	}

	[Native]
	public enum R7ActivationMethod : ulong
	{
		Internet = 1,
		Satellite = 2
	}

	[Native]
	public enum R7ActivationDesistStatus : ulong
	{
		ctivation = 0,
		pp = 1
	}

	[Native]
	public enum R7ConnectionMode : ulong
	{
		Manual = 1,
		Automatic = 2
	}

	[Native]
	public enum R7ConnectionState : ulong
	{
		Idle = 0,
		Off = 1,
		Ready = 2,
		Discovering = 3,
		Connecting = 4,
		Connected = 5
	}

	[Native]
	public enum R7MessageStatus : ulong
	{
		R7MessageStatusPending = 1,
		R7MessageStatusReceivedByDevice = 2,
		R7MessageStatusQueuedForTransmission = 3,
		R7MessageStatusTransmitting = 4,
		R7MessageStatusTransmitted = 5,
		R7MessageStatusReceived = 96,
		R7MessageStatusErrorCapability = 95,
		R7MessageStatusErrorTooLong = 97,
		R7MessageStatusErrorNoCredit = 98,
		R7MessageStatusError = 99
	}

	[Native]
	public enum R7MessageChunkType : ulong
	{
		Message = 1,
		File = 2
	}

	[Native]
	public enum R7CommandType : ulong
	{
		SendMessage = 1,
		GetNextMessage = 2,
		DeleteMessage = 3,
		AcknowledgeMessageStatus = 4,
		Internal = 5,
		SendFileSegment = 6,
		Pin = 7,
		ActionRequest = 8,
		GenericAlert = 9,
		SerialDump = 10,
		SetGprsConfig = 11,
		GetGprsConfig = 12,
		TrackerStatus = 13,
		SetParameter = 14,
		GetParameter = 15,
		GetLogSegment = 16,
		GetLogListing = 17,
		GetLogIndex = 18
	}

	[Native]
	public enum R7CommandActionRequestType : ulong
	{
		TypeSendAlert = 0,
		TypeSendManual = 1,
		TypeInstallUpdates = 2,
		TypeMailboxCheck = 3,
		TypeUpdateMessageStatus = 4,
		PositionUpdateLastKnown = 5,
		PositionUpdate = 6,
		TypeBatteryUpdate = 7,
		ShippingMode = 8,
		Activation = 9,
		Beep = 10,
		FactoryReset = 11,
		GeofenceCentre = 12,
		GprsStatus = 13,
		CancelAlertMode = 14,
		SelfTest = 15,
		TypeToggleWatchState = 16
	}

	[Native]
	public enum R7CommandGenericAlertType : ulong
	{
		A = 100,
		B = 101,
		C = 102,
		D = 103,
		E = 104,
		F = 105,
		G = 106,
		H = 107,
		I = 108,
		J = 109,
		K = 110,
		L = 111,
		M = 112,
		N = 113,
		O = 114,
		P = 115,
		Q = 116,
		R = 117,
		S = 118,
		T = 119,
		U = 120,
		V = 121,
		W = 122,
		X = 123,
		Y = 124,
		Z = 125
	}

	[Native]
	public enum R7DeviceError : ulong
	{
		FailedToConnect = 1,
		WriteTimeout = 2,
		BluetoothError = 3,
		LostConnetion = 4,
		NoCredit = 5,
		StateError = 99
	}

	[Native]
	public enum R7LockState : ulong
	{
		R7LockStateUnlocked = 0,
		R7LockStateLocked = 1,
		R7LockStateIncorrectPin = 2,
		R7LockStateUnknown = 99
	}

	[Native]
	public enum R7FileTransferError : ulong
	{
		R7FileTransferErrorFileTooLarge = 1
	}

	[Native]
	public enum R7LogFileError : ulong
	{
		FileNotFound = 1,
		SegmentNotFound = 2,
		Unknown = 99
	}

	[Native]
	public enum R7DeviceContext : ulong
	{
		tSatellite = 1,
		tCellular = 2,
		Distress = 3
	}

	[Native]
	public enum R7DeviceWatchState : ulong
	{
		ff = 1,
		nAwaitingConfirmation = 2,
		n = 3,
		ffAwaitingConfirmation = 4
	}

	[Native]
	public enum DeviceAccessoryParameterType : ulong
	{
		Text = 0,
		Integer = 1,
		HostOrIp = 2
	}

	[Native]
	public enum R7FirmwareUpdaterError : ulong
	{
		NoDevice = 0,
		NoInternet = 1,
		BadResponse = 2,
		DownloadFailure = 3,
		Transfer = 4,
		InvalidState = 5
	}

	[Native]
	public enum R7GenericDeviceParameter : ulong
	{
		AlertsTimerStatus = 1000,
		AlertsTimerTimeout = 1001,
		AlertsBluetoothLossStatus = 1002,
		AlertsCollisionDuration = 1003,
		AlertsCollisionStatus = 1004,
		AlertsCollisionThreshold = 1005,
		AlertsDeadmanStatus = 1006,
		AlertsDeadmanTimeout = 1007,
		AlertsGeofenceDistance = 1008,
		AlertsGeofenceCheckFrequency = 1009,
		AlertsGeofenceStatus = 1010,
		AlertsGeofenceCentre = 2010,
		AlertsPowerLossStatus = 1011,
		AlertsTemperatureCheckFrequency = 1012,
		AlertsHotTemperature = 1013,
		AlertsColdTemperature = 1014,
		AlertsTemperatureStatus = 1015,
		ExternalBaudRate = 1016,
		ExternalStatus = 1017,
		ExternalMobWatcher = 1050,
		ExternalSampleRate = 1018,
		FrameworkBatteryLevel = 1019,
		FrameworkInboxCount = 1020,
		FrameworkOutboxConfirm = 1021,
		FrameworkSerialRX = 1022,
		FrameworkSerialTX = 1023,
		FrameworkLocation = 1024,
		GPSEarlyWakeup = 1025,
		GPSMode = 1026,
		GPSFixesbeforeaccept = 1027,
		GPSHotStatus = 1028,
		GpxLoggingPeriod = 1029,
		GpxLoggingStatus = 1030,
		MailboxCheckFrequency = 1031,
		MailboxCheckStatus = 1032,
		PrivateLogging = 1033,
		PrivateLogo = 1034,
		PrivateUserMode = 1035,
		ScreenAccessPin = 1036,
		ScreenNonActivityThreshold = 1037,
		ScreenScreenBrightness = 1038,
		ScreenTimeout = 1040,
		ScreenStealthStatus = 1041,
		SystemEncryptionStatus = 1042,
		TrackingActivitySenseStatus = 1043,
		TrackingActivitySenseLowThreshold = 1044,
		TrackingActivitySenseHighThreshold = 1045,
		TrackingBurstFixPeriod = 1046,
		TrackingBurstTransmitPeriod = 1047,
		TrackingFrequency = 1048,
		TrackingStatus = 1049,
		BatteryLevel = 2000,
		GpsStatus = 2001,
		IridiumStatus = 2002,
		PowerStatus = 2003,
		Model = 2004,
		ExternalPowerAvailability = 2005,
		Notify = 2006,
		GprsStrategy = 2007,
		GprsStatus = 2008,
		GprsSignal = 2009,
		GprsMsisdn = 2010,
		GprsSim = 2011,
		GprsLocation = 2012,
		InputSensitivity = 2013,
		CellularFrequency = 2014,
		CellularBurstFixPeriod = 2015,
		CellularBurstTransmitPeriod = 2016,
		DistressFrequency = 2017,
		DistressBurstFixPeriod = 2018,
		DistressBurstTransmitPeriod = 2019,
		TransmissionFormat = 2020,
		TrackingActivitySenseThreshold = 2021,
		AscentDescentAlertMode = 2022,
		AscentAlertThreshold = 2023,
		DescentAlertThreshold = 2024,
		AscentAlertTimePeriod = 2025,
		DescentAlertTimePeriod = 2026,
		GpsDynamicPlatformModel = 2027,
		AutoResumeStatus = 2028,
		AutoResumeDistance = 2029,
		HoverAlertStatus = 2030,
		DebugLogging = 2031,
		AirborneVRotate = 2032,
		AirborneVStall = 2033,
		Context = 2034,
		WatchState = 2035,
		SelfTestResult = 2036
	}

	[Native]
	public enum R7GenericDeviceValueAlertsTimerStatus : ulong
	{
		ff = 0,
		n = 1
	}

	[Native]
	public enum R7GenericDeviceValueAlertsTimerTimeout : ulong
	{
		R7GenericDeviceValueAlertsTimerTimeout5min = 0,
		R7GenericDeviceValueAlertsTimerTimeout10min = 1,
		R7GenericDeviceValueAlertsTimerTimeout15min = 2,
		R7GenericDeviceValueAlertsTimerTimeout30min = 3,
		R7GenericDeviceValueAlertsTimerTimeout1hour = 4,
		R7GenericDeviceValueAlertsTimerTimeout2hour = 5,
		R7GenericDeviceValueAlertsTimerTimeout4hour = 6,
		Unknown = 7
	}

	[Native]
	public enum R7GenericDeviceValueAlertsBluetoothLossStatus : ulong
	{
		ff = 0,
		n = 1
	}

	[Native]
	public enum R7GenericDeviceValueAlertsCollisionDuration : ulong
	{
		R7GenericDeviceValueAlertsCollisionDuration1ms = 0,
		R7GenericDeviceValueAlertsCollisionDuration2ms = 1,
		R7GenericDeviceValueAlertsCollisionDuration5ms = 2,
		R7GenericDeviceValueAlertsCollisionDuration10ms = 3,
		R7GenericDeviceValueAlertsCollisionDuration20ms = 4
	}

	[Native]
	public enum R7GenericDeviceValueAlertsCollisionStatus : ulong
	{
		ff = 0,
		n = 1
	}

	[Native]
	public enum R7GenericDeviceValueAlertsNotify : ulong
	{
		None = 0,
		Audible = 1,
		Visual = 2,
		Both = 3
	}

	[Native]
	public enum R7GenericDeviceValueAlertsCollisionThreshold : ulong
	{
		R7GenericDeviceValueAlertsCollisionThreshold1g = 0,
		R7GenericDeviceValueAlertsCollisionThreshold2g = 1,
		R7GenericDeviceValueAlertsCollisionThreshold4g = 2,
		R7GenericDeviceValueAlertsCollisionThreshold8g = 3,
		R7GenericDeviceValueAlertsCollisionThreshold12g = 4,
		R7GenericDeviceValueAlertsCollisionThreshold16g = 5
	}

	[Native]
	public enum R7GenericDeviceValueAlertsDeadmanStatus : ulong
	{
		ff = 0,
		n = 1
	}

	[Native]
	public enum R7GenericDeviceValueAlertsDeadmanTimeout : ulong
	{
		R7GenericDeviceValueAlertsDeadmanTimeout5min = 0,
		R7GenericDeviceValueAlertsDeadmanTimeout10min = 1,
		R7GenericDeviceValueAlertsDeadmanTimeout15min = 2,
		R7GenericDeviceValueAlertsDeadmanTimeout30min = 3,
		R7GenericDeviceValueAlertsDeadmanTimeout1hour = 4,
		R7GenericDeviceValueAlertsDeadmanTimeout2hour = 5,
		R7GenericDeviceValueAlertsDeadmanTimeout4hour = 6,
		Unknown = 7
	}

	[Native]
	public enum R7GenericDeviceValueAlertsGeofenceDistance : ulong
	{
		R7GenericDeviceValueAlertsGeofenceDistance25M = 0,
		R7GenericDeviceValueAlertsGeofenceDistance50M = 1,
		R7GenericDeviceValueAlertsGeofenceDistance100M = 2,
		R7GenericDeviceValueAlertsGeofenceDistance250M = 3,
		R7GenericDeviceValueAlertsGeofenceDistance1KM = 4,
		R7GenericDeviceValueAlertsGeofenceDistance2KM = 5,
		R7GenericDeviceValueAlertsGeofenceDistance3KM = 6
	}

	[Native]
	public enum R7GenericDeviceValueAlertsGeofenceCheckFrequency : ulong
	{
		R7GenericDeviceValueAlertsGeofenceCheckFrequency1min = 0,
		R7GenericDeviceValueAlertsGeofenceCheckFrequency2min = 1,
		R7GenericDeviceValueAlertsGeofenceCheckFrequency3min = 2,
		R7GenericDeviceValueAlertsGeofenceCheckFrequency5min = 3,
		R7GenericDeviceValueAlertsGeofenceCheckFrequency10min = 4,
		R7GenericDeviceValueAlertsGeofenceCheckFrequency15min = 5,
		R7GenericDeviceValueAlertsGeofenceCheckFrequency30min = 6,
		Unknown = 7
	}

	[Native]
	public enum R7GenericDeviceValueAlertsGeofenceStatus : ulong
	{
		ff = 0,
		n = 1,
		nPolyfence = 2
	}

	[Native]
	public enum R7GenericDeviceValueAlertsPowerLossStatus : ulong
	{
		ff = 0,
		n = 1
	}

	[Native]
	public enum R7GenericDeviceValueAlertsTemperatureCheckFrequency : ulong
	{
		R7GenericDeviceValueAlertsTemperatureCheckFrequency1min = 0,
		R7GenericDeviceValueAlertsTemperatureCheckFrequency2min = 1,
		R7GenericDeviceValueAlertsTemperatureCheckFrequency3min = 2,
		R7GenericDeviceValueAlertsTemperatureCheckFrequency5min = 3,
		R7GenericDeviceValueAlertsTemperatureCheckFrequency10min = 4,
		R7GenericDeviceValueAlertsTemperatureCheckFrequency15min = 5,
		R7GenericDeviceValueAlertsTemperatureCheckFrequency30min = 6,
		Unknown = 7
	}

	[Native]
	public enum R7GenericDeviceValueAlertsHotTemperature : ulong
	{
		Minus40 = 0,
		Minus35 = 1,
		Minus30 = 2,
		Minus25 = 3,
		Minus20 = 4,
		Minus15 = 5,
		Minus10 = 6,
		Minus5 = 7,
		Zero = 8,
		Plus5 = 9,
		Plus10 = 10,
		Plus15 = 11,
		Plus20 = 12,
		Plus30 = 14,
		Plus25 = 13,
		Plus35 = 15,
		Plus40 = 16,
		Plus45 = 17,
		Plus50 = 18
	}

	[Native]
	public enum R7GenericDeviceValueAlertsColdTemperature : ulong
	{
		Minus40 = 0,
		Minus35 = 1,
		Minus30 = 2,
		Minus25 = 3,
		Minus20 = 4,
		Minus15 = 5,
		Minus10 = 6,
		Minus5 = 7,
		Zero = 8,
		Plus5 = 9,
		Plus10 = 10,
		Plus15 = 11,
		Plus20 = 12,
		Plus25 = 13,
		Plus30 = 14,
		Plus35 = 15,
		Plus40 = 16,
		Plus45 = 17,
		Plus50 = 18
	}

	[Native]
	public enum R7GenericDeviceValueAlertsTemperatureStatus : ulong
	{
		ff = 0,
		n = 1
	}

	[Native]
	public enum R7GenericDeviceValueExternalBaudRate : ulong
	{
		R7GenericDeviceValueExternalBaudRate4800 = 0,
		R7GenericDeviceValueExternalBaudRate9600 = 1,
		R7GenericDeviceValueExternalBaudRate19200 = 2,
		R7GenericDeviceValueExternalBaudRate38400 = 3,
		R7GenericDeviceValueExternalBaudRate57600 = 4,
		R7GenericDeviceValueExternalBaudRate115200 = 5
	}

	[Native]
	public enum R7GenericDeviceValueExternalPowerAvailiability : ulong
	{
		Unlimited = 0,
		Limited = 1,
		UnlimitedActivate = 2
	}

	[Native]
	public enum R7GenericDeviceValueExternalStatus : ulong
	{
		Off = 0,
		Nmea = 1,
		Hydrosphere = 2,
		SerialApi = 3,
		Maximet = 4,
		MaximetGmx200 = 5,
		Wave = 6,
		Vwtp3 = 7,
		DaliaFPSO = 8
	}

	[Native]
	public enum R7GenericDeviceValueExternalMobWatcher : ulong
	{
		ff = 0,
		n = 1
	}

	[Native]
	public enum R7GenericDeviceValueExternalSampleRate : ulong
	{
		R7GenericDeviceValueExternalSampleRate5sec = 0,
		R7GenericDeviceValueExternalSampleRate10sec = 1,
		R7GenericDeviceValueExternalSampleRate20sec = 2,
		R7GenericDeviceValueExternalSampleRate40sec = 3,
		R7GenericDeviceValueExternalSampleRate60sec = 4
	}

	[Native]
	public enum R7GenericDeviceValueExternalInput1 : ulong
	{
		Off = 0,
		Rising = 1,
		Falling = 2
	}

	[Native]
	public enum R7GenericDeviceValueExternalInput2 : ulong
	{
		Off = 0,
		Rising = 1,
		Falling = 2
	}

	[Native]
	public enum R7GenericDeviceValueGPSEarlyWakeup : ulong
	{
		R7GenericDeviceValueGPSEarlyWakeup20sec = 0,
		R7GenericDeviceValueGPSEarlyWakeup40sec = 1,
		R7GenericDeviceValueGPSEarlyWakeup60sec = 2,
		R7GenericDeviceValueGPSEarlyWakeup120sec = 3,
		R7GenericDeviceValueGPSEarlyWakeup180sec = 4,
		R7GenericDeviceValueGPSEarlyWakeup240sec = 5
	}

	[Native]
	public enum R7GenericDeviceValueGPSMode : ulong
	{
		R7GenericDeviceValueGPSMode2D = 0,
		R7GenericDeviceValueGPSMode3D = 1,
		Unknown = 7
	}

	[Native]
	public enum R7GenericDeviceValueGPSFixesbeforeaccept : ulong
	{
		R7GenericDeviceValueGPSFixesbeforeaccept1Fix = 0,
		R7GenericDeviceValueGPSFixesbeforeaccept5Fix = 1,
		R7GenericDeviceValueGPSFixesbeforeaccept10Fix = 2,
		R7GenericDeviceValueGPSFixesbeforeaccept20Fix = 3,
		Unknown = 7
	}

	[Native]
	public enum R7GenericDeviceValueGPSHotStatus : ulong
	{
		ff = 0,
		n = 1
	}

	[Native]
	public enum R7GenericDeviceValueGpxLoggingPeriod : ulong
	{
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
	}

	[Native]
	public enum R7GenericDeviceValueGpxLoggingStatus : ulong
	{
		ff = 0,
		n = 1
	}

	[Native]
	public enum R7GenericDeviceValueMailboxCheckFrequency : ulong
	{
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
	}

	[Native]
	public enum R7GenericDeviceValueMailboxCheckStatus : ulong
	{
		ff = 0,
		n = 1
	}

	[Native]
	public enum R7GenericDeviceValueScreenNonActivityThreshold : ulong
	{
		R7GenericDeviceValueScreenNonActivityThreshold10sec = 0,
		R7GenericDeviceValueScreenNonActivityThreshold20sec = 1,
		R7GenericDeviceValueScreenNonActivityThreshold30sec = 2,
		R7GenericDeviceValueScreenNonActivityThreshold1min = 3
	}

	[Native]
	public enum R7GenericDeviceValueScreenScreenBrightness : ulong
	{
		R7GenericDeviceValueScreenScreenBrightness25 = 0,
		R7GenericDeviceValueScreenScreenBrightness50 = 1,
		R7GenericDeviceValueScreenScreenBrightness75 = 2,
		R7GenericDeviceValueScreenScreenBrightness100 = 3
	}

	[Native]
	public enum R7GenericDeviceValueScreenLockStatus : ulong
	{
		ff = 0,
		n = 1
	}

	[Native]
	public enum R7GenericDeviceValueScreenTimeout : ulong
	{
		R7GenericDeviceValueScreenTimeout10sec = 0,
		R7GenericDeviceValueScreenTimeout20sec = 1,
		R7GenericDeviceValueScreenTimeout30sec = 2,
		R7GenericDeviceValueScreenTimeout1min = 3
	}

	[Native]
	public enum R7GenericDeviceValueScreenStealthStatus : ulong
	{
		ff = 0,
		n = 1
	}

	[Native]
	public enum R7GenericDeviceValueSystemEncryptionStatus : ulong
	{
		ff = 0,
		n = 1
	}

	[Native]
	public enum R7GenericDeviceValueTrackingActivitySenseStatus : ulong
	{
		Off = 0,
		On = 1,
		Bump = 2,
		Sog = 3,
		BumpAndSog = 4,
		AwayFromHome = 5
	}

	[Native]
	public enum R7GenericDeviceValueTrackingActivitySenseLowThreshold : ulong
	{
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
	}

	[Native]
	public enum R7GenericDeviceValueTrackingActivitySenseHighThreshold : ulong
	{
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
	}

	[Native]
	public enum R7GenericDeviceValueTrackingActivitySenseThreshold : ulong
	{
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
	}

	[Native]
	public enum R7GenericDeviceValueTrackingBurstFixPeriod : ulong
	{
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
	}

	[Native]
	public enum R7GenericDeviceValueTrackingBurstTransmitPeriod : ulong
	{
		R7GenericDeviceValueTrackingBurstTransmitPeriod1min = 0,
		R7GenericDeviceValueTrackingBurstTransmitPeriod2min = 1,
		R7GenericDeviceValueTrackingBurstTransmitPeriod5min = 2,
		R7GenericDeviceValueTrackingBurstTransmitPeriod10min = 3,
		R7GenericDeviceValueTrackingBurstTransmitPeriod15min = 4,
		R7GenericDeviceValueTrackingBurstTransmitPeriod30min = 5,
		R7GenericDeviceValueTrackingBurstTransmitPeriod60min = 6
	}

	[Native]
	public enum R7GenericDeviceValueTrackingFrequency : ulong
	{
		Continuous = 0,
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
		Burst = 14,
		R7GenericDeviceValueTrackingFrequency1min = 15,
		R7GenericDeviceValueTrackingFrequency2min = 16,
		R7GenericDeviceValueTrackingFrequency3min = 17,
		R7GenericDeviceValueTrackingFrequency4min = 18,
		R7GenericDeviceValueTrackingFrequency1440min = 19,
		R7GenericDeviceValueTrackingFrequency6min = 20,
		R7GenericDeviceValueTrackingFrequency8min = 21,
		R7GenericDeviceValueTrackingFrequency12min = 22,
		R7GenericDeviceValueTrackingFrequency15sec = 23,
		R7GenericDeviceValueTrackingFrequency30sec = 24
	}

	[Native]
	public enum R7GenericDeviceValueTrackingStatus : ulong
	{
		ff = 0,
		n = 1
	}

	[Native]
	public enum R7GenericDeviceValueGprsStrategy : ulong
	{
		Never = 0,
		Always = 1,
		Preferred = 2
	}

	[Native]
	public enum R7GenericDeviceValueDaughterboardStatus : ulong
	{
		None = 0,
		Gprs = 1
	}

	[Native]
	public enum R7GenericDeviceValueGprsStatus : ulong
	{
		Gsm = 0,
		GsmCompact = 1,
		Umts = 2,
		GsmEdge = 3,
		UmtsHsdpa = 4,
		UmtsHsupa = 5,
		UmtsHsdpaHsupa = 6,
		Lte = 7
	}

	[Native]
	public enum R7GenericDeviceValueInputSensitivity : ulong
	{
		Fast0123 = 0,
		Fast012 = 1,
		Fast01 = 2,
		Fast0 = 3,
		AllSlow = 4
	}

	[Native]
	public enum R7GenericDeviceGprsParameter : ulong
	{
		ApnName = 0,
		EndpointAddress1 = 1,
		EndpointAddress2 = 2,
		EndpointAddress3 = 3,
		EndpointPort1 = 4,
		EndpointPort2 = 5,
		EndpointPort3 = 6,
		ApnUsername = 7,
		ApnPassword = 8
	}

	[Native]
	public enum R7GenericDeviceValueCellularFrequency : ulong
	{
		Continuous = 0,
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
		Burst = 14,
		R7GenericDeviceValueCellularFrequency1min = 15,
		R7GenericDeviceValueCellularFrequency2min = 16,
		R7GenericDeviceValueCellularFrequency3min = 17,
		R7GenericDeviceValueCellularFrequency4min = 18,
		R7GenericDeviceValueCellularFrequency1440min = 19,
		R7GenericDeviceValueCellularFrequency6min = 20,
		R7GenericDeviceValueCellularFrequency8min = 21,
		R7GenericDeviceValueCellularFrequency12min = 22,
		R7GenericDeviceValueCellularFrequency15sec = 23,
		R7GenericDeviceValueCellularFrequency30sec = 24
	}

	[Native]
	public enum R7GenericDeviceValueCellularBurstFixPeriod : ulong
	{
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
	}

	[Native]
	public enum R7GenericDeviceValueCellularBurstTransmitPeriod : ulong
	{
		R7GenericDeviceValueCellularBurstTransmitPeriod1min = 0,
		R7GenericDeviceValueCellularBurstTransmitPeriod2min = 1,
		R7GenericDeviceValueCellularBurstTransmitPeriod5min = 2,
		R7GenericDeviceValueCellularBurstTransmitPeriod10min = 3,
		R7GenericDeviceValueCellularBurstTransmitPeriod15min = 4,
		R7GenericDeviceValueCellularBurstTransmitPeriod30min = 5,
		R7GenericDeviceValueCellularBurstTransmitPeriod60min = 6
	}

	[Native]
	public enum R7GenericDeviceValueDistressFrequency : ulong
	{
		Continuous = 0,
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
		Burst = 14,
		R7GenericDeviceValueDistressFrequency1min = 15,
		R7GenericDeviceValueDistressFrequency2min = 16,
		R7GenericDeviceValueDistressFrequency3min = 17,
		R7GenericDeviceValueDistressFrequency4min = 18,
		R7GenericDeviceValueDistressFrequency1440min = 19,
		R7GenericDeviceValueDistressFrequency6min = 20,
		R7GenericDeviceValueDistressFrequency8min = 21,
		R7GenericDeviceValueDistressFrequency12min = 22,
		R7GenericDeviceValueDistressFrequency15sec = 23,
		R7GenericDeviceValueDistressFrequency30sec = 24
	}

	[Native]
	public enum R7GenericDeviceValueDistressBurstFixPeriod : ulong
	{
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
	}

	[Native]
	public enum R7GenericDeviceValueDistressBurstTransmitPeriod : ulong
	{
		R7GenericDeviceValueDistressBurstTransmitPeriod1min = 0,
		R7GenericDeviceValueDistressBurstTransmitPeriod2min = 1,
		R7GenericDeviceValueDistressBurstTransmitPeriod5min = 2,
		R7GenericDeviceValueDistressBurstTransmitPeriod10min = 3,
		R7GenericDeviceValueDistressBurstTransmitPeriod15min = 4,
		R7GenericDeviceValueDistressBurstTransmitPeriod30min = 5,
		R7GenericDeviceValueDistressBurstTransmitPeriod60min = 6
	}

	[Native]
	public enum R7GenericDeviceValueTransmissionFormat : ulong
	{
		Standard = 0,
		Compact = 1,
		Aes = 2
	}

	[Native]
	public enum R7GenericDeviceValueAscentDescentAlertMode : ulong
	{
		Off = 0,
		Desent = 1,
		Asent = 2,
		Both = 3
	}

	[Native]
	public enum R7GenericDeviceValueAlertThreshold : ulong
	{
		R7GenericDeviceValueAscentAlertThreshold1000 = 0,
		R7GenericDeviceValueAscentAlertThreshold1500 = 1,
		R7GenericDeviceValueAscentAlertThreshold2000 = 2,
		R7GenericDeviceValueAscentAlertThreshold2500 = 3,
		R7GenericDeviceValueAscentAlertThreshold3000 = 4
	}

	[Native]
	public enum R7GenericDeviceValueDescentAlertThreshold : ulong
	{
		R7GenericDeviceValueDescentAlertThreshold1000 = 0,
		R7GenericDeviceValueDescentAlertThreshold1500 = 1,
		R7GenericDeviceValueDescentAlertThreshold2000 = 2,
		R7GenericDeviceValueDescentAlertThreshold2500 = 3,
		R7GenericDeviceValueDescentAlertThreshold3000 = 4
	}

	[Native]
	public enum R7GenericDeviceValueAscentAlertTimePeriod : ulong
	{
		R7GenericDeviceValueAscentAlertTimePeriod5sec = 0,
		R7GenericDeviceValueAscentAlertTimePeriod10sec = 1,
		R7GenericDeviceValueAscentAlertTimePeriod15sec = 2,
		R7GenericDeviceValueAscentAlertTimePeriod20sec = 3,
		R7GenericDeviceValueAscentAlertTimePeriod25sec = 4,
		R7GenericDeviceValueAscentAlertTimePeriod30sec = 5
	}

	[Native]
	public enum R7GenericDeviceValueDescentAlertTimePeriod : ulong
	{
		R7GenericDeviceValueDescentAlertTimePeriod5sec = 0,
		R7GenericDeviceValueDescentAlertTimePeriod10sec = 1,
		R7GenericDeviceValueDescentAlertTimePeriod15sec = 2,
		R7GenericDeviceValueDescentAlertTimePeriod20sec = 3,
		R7GenericDeviceValueDescentAlertTimePeriod25sec = 4,
		R7GenericDeviceValueDescentAlertTimePeriod30sec = 5
	}

	[Native]
	public enum R7GenericDeviceValueGpsDynamicPlatformModel : ulong
	{
		Portable = 0,
		Automotive = 1,
		Marine = 2,
		Airborne = 3
	}

	[Native]
	public enum R7GenericDeviceValueAutoResumeStatus : ulong
	{
		ff = 0,
		n = 1
	}

	[Native]
	public enum R7GenericDeviceValueAutoResumeDistance : ulong
	{
		R7GenericDeviceValueAutoResumeDistance2km = 0,
		R7GenericDeviceValueAutoResumeDistance3km = 1,
		R7GenericDeviceValueAutoResumeDistance5km = 2,
		R7GenericDeviceValueAutoResumeDistance10km = 3
	}

	[Native]
	public enum R7GenericDeviceValueHoverAlertStatus : ulong
	{
		Off = 0,
		Hover = 1,
		FixedWing = 2
	}

	[Native]
	public enum R7GenericDeviceValueDebugLogging : ulong
	{
		None = 0,
		Internal = 1,
		External = 2
	}

	[Native]
	public enum R7GenericDeviceValueAirborneVRotate : ulong
	{
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
	}

	[Native]
	public enum R7GenericDeviceValueAirborneVStall : ulong
	{
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
	}

	[Native]
	public enum R7GenericDeviceValueWatchMeState : ulong
	{
		ff = 0,
		nRequestSent = 1,
		n = 2,
		ffRequestSent = 3
	}

	[Native]
	public enum R7GenericDeviceValueContext : ulong
	{
		Off = 0,
		Distress = 1,
		Cellular = 2,
		Satellite = 3,
		CellularDistress = 4,
		SatelliteDistress = 5
	}
}
