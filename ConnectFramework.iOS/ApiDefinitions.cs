using System;
using ConnectFramework;
using CoreBluetooth;
using CoreLocation;
using Foundation;
using ObjCRuntime;

namespace ConnectFramework
{
	// @interface DeviceParameter : NSObject
	[BaseType (typeof(NSObject))]
	interface DeviceParameter
	{
		// @property (assign, nonatomic) NSUInteger identifier;
		[Export ("identifier")]
		nuint Identifier { get; set; }

		// @property (retain, nonatomic) CBUUID * characteristic;
		[Export ("characteristic", ArgumentSemantic.Retain)]
		CBUUID Characteristic { get; set; }

		// @property (retain, nonatomic) NSString * label;
		[Export ("label", ArgumentSemantic.Retain)]
		string Label { get; set; }

		// @property (retain, nonatomic) NSString * description;
		[Export ("description", ArgumentSemantic.Retain)]
		string Description { get; set; }

		// @property (assign, nonatomic) BOOL readonly;
		[Export ("readonly")]
		bool Readonly { get; set; }

		// @property (assign, nonatomic) BOOL available;
		[Export ("available")]
		bool Available { get; set; }

		// @property (readonly, nonatomic) uint16_t attributeId;
		[Export ("attributeId")]
		ushort AttributeId { get; }

		// @property (readonly, nonatomic) NSMutableDictionary * options;
		[Export ("options")]
		NSMutableDictionary Options { get; }

		// @property (readonly, nonatomic) NSMutableArray * optionsIndex;
		[Export ("optionsIndex")]
		NSMutableArray OptionsIndex { get; }

		// @property (readonly, nonatomic) NSUInteger cachedValue;
		[Export ("cachedValue")]
		nuint CachedValue { get; }

		// @property (readonly, nonatomic) NSData * cachedValueBytes;
		[Export ("cachedValueBytes")]
		NSData CachedValueBytes { get; }

		// @property (readonly, nonatomic) NSDate * cachedTime;
		[Export ("cachedTime")]
		NSDate CachedTime { get; }

		// -(id)initWithIdentifier:(NSUInteger)identifier characteristic:(CBUUID *)characteristic label:(NSString *)label description:(NSString *)description;
		[Export ("initWithIdentifier:characteristic:label:description:")]
		IntPtr Constructor (nuint identifier, CBUUID characteristic, string label, string description);

		// -(id)initWithIdentifier:(NSUInteger)identifier attributeId:(NSUInteger)attributeId characteristic:(CBUUID *)characteristic label:(NSString *)label description:(NSString *)description;
		[Export ("initWithIdentifier:attributeId:characteristic:label:description:")]
		IntPtr Constructor (nuint identifier, nuint attributeId, CBUUID characteristic, string label, string description);

		// -(void)addValueOption:(NSUInteger)value withLabel:(NSString *)valueLabel;
		[Export ("addValueOption:withLabel:")]
		void AddValueOption (nuint value, string valueLabel);

		// -(NSString *)labelForValue:(NSUInteger)value;
		[Export ("labelForValue:")]
		string LabelForValue (nuint value);

		// -(void)updateCacheValue:(NSData *)value;
		[Export ("updateCacheValue:")]
		void UpdateCacheValue (NSData value);

		// -(BOOL)isCacheValueUsable;
		[Export ("isCacheValueUsable")]
		
		bool IsCacheValueUsable { get; }
	}

	// @interface DeviceParameterGroup : NSObject
	[BaseType (typeof(NSObject))]
	interface DeviceParameterGroup
	{
		// @property (readonly, nonatomic) NSArray<DeviceParameter *> * parameters;
		[Export ("parameters")]
		DeviceParameter[] Parameters { get; }

		// @property (readonly, nonatomic) NSArray<DeviceParameterGroup *> * groups;
		[Export ("groups")]
		DeviceParameterGroup[] Groups { get; }

		// @property (readonly, nonatomic) NSString * name;
		[Export ("name")]
		string Name { get; }

		// @property (readonly, nonatomic) NSString * explanation;
		[Export ("explanation")]
		string Explanation { get; }

		// @property (assign, nonatomic) _Bool visible;
		[Export ("visible")]
		bool Visible { get; set; }

		// -(id)initWithName:(NSString *)name explanation:(NSString *)explanation parameters:(NSArray<DeviceParameter *> *)parameters groups:(NSArray<DeviceParameterGroup *> *)groups;
		[Export ("initWithName:explanation:parameters:groups:")]
		IntPtr Constructor (string name, string explanation, DeviceParameter[] parameters, DeviceParameterGroup[] groups);
	}

	// @interface ConnectDevice : NSObject
	[BaseType (typeof(NSObject))]
	interface ConnectDevice
	{
		// @property (readonly, nonatomic) NSString * name;
		[Export ("name")]
		string Name { get; }

		// @property (readonly, nonatomic) NSString * version;
		[Export ("version")]
		string Version { get; }

		// -(id)initWithConnectComms:(ConnectComms *)comms name:(NSString *)name version:(NSString *)version;
		[Export ("initWithConnectComms:name:version:")]
		IntPtr Constructor (ConnectComms comms, string name, string version);

		// -(NSArray<DeviceParameter *> *)parameters;
		[Export("parameters")]

		DeviceParameter[] Parameters();

		// -(NSArray<DeviceParameterGroup *> *)parameterGroups;
		[Export ("parameterGroups")]
		
		DeviceParameterGroup[] ParameterGroups { get; }

		// -(DeviceParameter *)parameterForCharacteristic:(CBUUID *)characteristic;
		[Export ("parameterForCharacteristic:")]
		DeviceParameter ParameterForCharacteristic (CBUUID characteristic);

		// -(DeviceParameter *)parameterForIdentifier:(NSUInteger)identifier;
		[Export ("parameterForIdentifier:")]
		DeviceParameter ParameterForIdentifier (nuint identifier);

		// -(DeviceParameter *)parameterForAttributeId:(uint16_t)attributeId;
		[Export ("parameterForAttributeId:")]
		DeviceParameter ParameterForAttributeId (ushort attributeId);

		// -(void)requestAll;
		[Export ("requestAll")]
		void RequestAll ();

		// -(void)notifyAll:(BOOL)notify;
		[Export ("notifyAll:")]
		void NotifyAll (bool notify);

		// -(void)valueUpdated:(CBUUID *)characteristic value:(NSData *)value;
		[Export ("valueUpdated:value:")]
		void ValueUpdated (CBUUID characteristic, NSData value);

		// -(void)discovered:(CBUUID *)characteristic;
		[Export ("discovered:")]
		void Discovered (CBUUID characteristic);

		// -(void)request:(NSUInteger)identifier;
		[Export ("request:")]
		void Request (nuint identifier);

		// -(void)update:(NSUInteger)identifier value:(NSData *)value;
		[Export ("update:value:")]
		void Update (nuint identifier, NSData value);

		// -(void)notify:(NSUInteger)identifier notify:(BOOL)notify;
		[Export ("notify:notify:")]
		void Notify (nuint identifier, bool notify);

		// -(NSArray *)characteristics;
		[Export ("characteristics")]
		NSObject[] Characteristics { get; }

		// -(NSString *)getHardwareType;
		[Export ("getHardwareType")]
		
		string HardwareType { get; }

		// -(int)getVersionAsInt;
		[Export ("getVersionAsInt")]
		
		int VersionAsInt { get; }
	}

	// @interface DeviceLogIndex : NSObject
	[BaseType (typeof(NSObject))]
	interface DeviceLogIndex
	{
		// @property (retain, nonatomic) NSString * date;
		[Export ("date", ArgumentSemantic.Retain)]
		string Date { get; set; }

		// @property (retain, nonatomic) NSString * file;
		[Export ("file", ArgumentSemantic.Retain)]
		string File { get; set; }

		// @property (assign, nonatomic) NSUInteger hour;
		[Export ("hour")]
		nuint Hour { get; set; }

		// @property (assign, nonatomic) NSUInteger offset;
		[Export ("offset")]
		nuint Offset { get; set; }

		// @property (assign, nonatomic) NSUInteger length;
		[Export ("length")]
		nuint Length { get; set; }

		// @property (assign, nonatomic) _Bool selected;
		[Export ("selected")]
		bool Selected { get; set; }

		// @property (retain, nonatomic) NSMutableString * payload;
		[Export ("payload", ArgumentSemantic.Retain)]
		NSMutableString Payload { get; set; }

		// -(_Bool)isDownloaded;
		[Export ("isDownloaded")]
		
		bool IsDownloaded { get; }
	}

	// @protocol R7DeviceResponseDelegate <NSObject>
	[Protocol, Model (AutoGeneratedName = true)]
	[BaseType (typeof(NSObject))]
	interface R7DeviceResponseDelegate
	{
		// @required -(void)deviceReady;
		[Abstract]
		[Export ("deviceReady")]
		void DeviceReady ();

		// @required -(void)deviceConnected:(ConnectDevice *)device activated:(R7ActivationState)activated locked:(BOOL)locked;
		[Abstract]
		[Export ("deviceConnected:activated:locked:")]
		void DeviceConnected (ConnectDevice device, R7ActivationState activated, bool locked);

		// @required -(void)deviceDisconnected;
		[Abstract]
		[Export ("deviceDisconnected")]
		void DeviceDisconnected ();

		// @optional -(void)deviceParameterUpdated:(DeviceParameter *)parameter;
		[Export ("deviceParameterUpdated:")]
		void DeviceParameterUpdated (DeviceParameter parameter);

		// @optional -(void)deviceBatteryUpdated:(NSUInteger)battery at:(NSDate *)timestamp;
		[Export ("deviceBatteryUpdated:at:")]
		void DeviceBatteryUpdated (nuint battery, NSDate timestamp);

		// @optional -(void)deviceStateChanged:(R7ConnectionState)stateTo stateFrom:(R7ConnectionState)stateFrom;
		[Export ("deviceStateChanged:stateFrom:")]
		void DeviceStateChanged (R7ConnectionState stateTo, R7ConnectionState stateFrom);

		// @optional -(void)deviceError:(R7DeviceError)error;
		[Export ("deviceError:")]
		void DeviceError (R7DeviceError error);

		// @optional -(void)deviceLockStatusUpdated:(R7LockState)state;
		[Export ("deviceLockStatusUpdated:")]
		void DeviceLockStatusUpdated (R7LockState state);

		// @optional -(void)deviceCommandReceived:(R7CommandType)command;
		[Export ("deviceCommandReceived:")]
		void DeviceCommandReceived (R7CommandType command);

		// @optional -(void)locationUpdated:(CLLocation *)location;
		[Export ("locationUpdated:")]
		void LocationUpdated (CLLocation location);

		// @optional -(void)creditBalanceUpdated:(NSUInteger)credit;
		[Export ("creditBalanceUpdated:")]
		void CreditBalanceUpdated (nuint credit);

		// @optional -(void)usageTimeout;
		[Export ("usageTimeout")]
		void UsageTimeout ();

		// @optional -(void)deviceSerialDump:(NSData *)data;
		[Export ("deviceSerialDump:")]
		void DeviceSerialDump (NSData data);

		// @optional -(void)deviceNameUpdated:(NSString *)name;
		[Export ("deviceNameUpdated:")]
		void DeviceNameUpdated (string name);

		// @optional -(void)deviceStatusUpdated:(NSUInteger)field value:(NSUInteger)value;
		[Export ("deviceStatusUpdated:value:")]
		void DeviceStatusUpdated (nuint field, nuint value);
	}

	// @protocol R7DeviceDiscoveryDelegate <NSObject>
	[Protocol, Model (AutoGeneratedName = true)]
	[BaseType (typeof(NSObject))]
	interface R7DeviceDiscoveryDelegate
	{
		// @required -(void)discoveryStopped;
		[Abstract]
		[Export ("discoveryStopped")]
		void DiscoveryStopped ();

		// @required -(void)discoveryFoundDevice:(NSUUID *)deviceIdentifier name:(NSString *)deviceName;
		[Abstract]
		[Export ("discoveryFoundDevice:name:")]
		void DiscoveryFoundDevice (NSUuid deviceIdentifier, string deviceName);

		// @optional -(void)discoveryStarted;
		[Export ("discoveryStarted")]
		void DiscoveryStarted ();

		// @optional -(void)discoveryUpdatedDevice:(NSUUID *)deviceIdentifier name:(NSString *)deviceName rssi:(NSNumber *)rssi;
		[Export ("discoveryUpdatedDevice:name:rssi:")]
		void DiscoveryUpdatedDevice (NSUuid deviceIdentifier, string deviceName, NSNumber rssi);
	}

	// @protocol R7DeviceActivationDelegate <NSObject>
	[Protocol, Model (AutoGeneratedName = true)]
	[BaseType (typeof(NSObject))]
	interface R7DeviceActivationDelegate
	{
		// @required -(void)activationCompleted:(NSString *)activationIdentifier previousDevice:(NSString *)previousDevice accountName:(NSString *)accountName;
		[Abstract]
		[Export ("activationCompleted:previousDevice:accountName:")]
		void ActivationCompleted (string activationIdentifier, string previousDevice, string accountName);

		// @required -(void)activationFailed:(R7ActivationError)error;
		[Abstract]
		[Export ("activationFailed:")]
		void ActivationFailed (R7ActivationError error);

		// @required -(void)activationDesist:(R7ActivationDesistStatus)status newDevice:(NSString *)newDevice;
		[Abstract]
		[Export ("activationDesist:newDevice:")]
		void ActivationDesist (R7ActivationDesistStatus status, string newDevice);

		// @optional -(void)activationStarted:(R7ActivationMethod)method;
		[Export ("activationStarted:")]
		void ActivationStarted (R7ActivationMethod method);

		// @optional -(void)activationStateUpdated:(R7ActivationState)state;
		[Export ("activationStateUpdated:")]
		void ActivationStateUpdated (R7ActivationState state);
	}

	// @protocol R7DeviceMessagingDelegate <NSObject>
	[Protocol, Model (AutoGeneratedName = true)]
	[BaseType (typeof(NSObject))]
	interface R7DeviceMessagingDelegate
	{
		// @required -(void)messageProgressCompleted:(NSUInteger)messageId;
		[Abstract]
		[Export ("messageProgressCompleted:")]
		void MessageProgressCompleted (nuint messageId);

		// @required -(BOOL)messageStatusUpdated:(uint16_t)messageId status:(R7MessageStatus)status;
		[Abstract]
		[Export ("messageStatusUpdated:status:")]
		bool MessageStatusUpdated (ushort messageId, R7MessageStatus status);

		// @required -(void)inboxUpdated:(NSUInteger)messages;
		[Abstract]
		[Export ("inboxUpdated:")]
		void InboxUpdated (nuint messages);

		// @required -(BOOL)messageReceived:(uint16_t)messageId data:(NSData *)data;
		[Abstract]
		[Export ("messageReceived:data:")]
		bool MessageReceived (ushort messageId, NSData data);

		// @optional -(void)messageProgressUpdated:(NSUInteger)messageId part:(NSUInteger)part totalParts:(NSInteger)totalParts;
		[Export ("messageProgressUpdated:part:totalParts:")]
		void MessageProgressUpdated (nuint messageId, nuint part, nint totalParts);

		// @optional -(void)nextMessageRequested;
		[Export ("nextMessageRequested")]
		void NextMessageRequested ();

		// @optional -(void)mailboxCheckRequested;
		[Export ("mailboxCheckRequested")]
		void MailboxCheckRequested ();

		// @optional -(void)messageCheckFinished;
		[Export ("messageCheckFinished")]
		void MessageCheckFinished ();
	}

	// @protocol R7DeviceFileTransferDelegate <NSObject>
	[Protocol, Model (AutoGeneratedName = true)]
	[BaseType (typeof(NSObject))]
	interface IR7DeviceFileTransferDelegate
	{
		// @required -(void)fileTransferStarted;
		[Abstract]
		[Export ("fileTransferStarted")]
		void FileTransferStarted ();

		// @required -(void)fileTransferCompleted;
		[Abstract]
		[Export ("fileTransferCompleted")]
		void FileTransferCompleted ();

		// @required -(void)fileTransferProgressUpdate:(NSUInteger)progress total:(NSUInteger)total;
		[Abstract]
		[Export ("fileTransferProgressUpdate:total:")]
		void FileTransferProgressUpdate (nuint progress, nuint total);

		// @required -(void)fileTransferError;
		[Abstract]
		[Export ("fileTransferError")]
		void FileTransferError ();
	}

	// @protocol R7DeviceGprsDelegate <NSObject>
	[Protocol, Model (AutoGeneratedName = true)]
	[BaseType (typeof(NSObject))]
	interface R7DeviceGprsDelegate
	{
		// @required -(void)gprsConfigResponse:(NSDictionary *)config;
		[Abstract]
		[Export ("gprsConfigResponse:")]
		void GprsConfigResponse (NSDictionary config);

		// @required -(void)gprsConfigured;
		[Abstract]
		[Export ("gprsConfigured")]
		void GprsConfigured ();
	}

	// @protocol R7DeviceLogDelegate <NSObject>
	[Protocol, Model (AutoGeneratedName = true)]
	[BaseType (typeof(NSObject))]
	interface R7DeviceLogDelegate
	{
		// @required -(void)logListing:(NSArray<NSString *> *)logs;
		[Abstract]
		[Export ("logListing:")]
		void LogListing (string[] logs);

		// @required -(void)logIndicesReceived:(NSArray<DeviceLogIndex *> *)indices;
		[Abstract]
		[Export ("logIndicesReceived:")]
		void LogIndicesReceived (DeviceLogIndex[] indices);

		// @required -(void)logSegmentReceived:(NSString *)filename offset:(NSUInteger)offset data:(NSData *)data;
		[Abstract]
		[Export ("logSegmentReceived:offset:data:")]
		void LogSegmentReceived (string filename, nuint offset, NSData data);

		// @required -(void)logError:(R7LogFileError)error;
		[Abstract]
		[Export ("logError:")]
		void LogError (R7LogFileError error);
	}

	// @interface ConnectComms : NSObject <CBCentralManagerDelegate, CBPeripheralDelegate>
	[BaseType (typeof(NSObject))]
	interface ConnectComms : ICBCentralManagerDelegate, ICBPeripheralDelegate
	{
		[Wrap ("WeakDiscoveryDelegate")]
		R7DeviceDiscoveryDelegate DiscoveryDelegate { get; set; }

		// @property (nonatomic, weak) id<R7DeviceDiscoveryDelegate> discoveryDelegate;
		[NullAllowed, Export ("discoveryDelegate", ArgumentSemantic.Weak)]
		NSObject WeakDiscoveryDelegate { get; set; }

		[Wrap ("WeakResponseDelegate")]
		R7DeviceResponseDelegate ResponseDelegate { get; set; }

		// @property (nonatomic, weak) id<R7DeviceResponseDelegate> responseDelegate;
		[NullAllowed, Export ("responseDelegate", ArgumentSemantic.Weak)]
		NSObject WeakResponseDelegate { get; set; }

		[Wrap ("WeakFileTransferDelegate")]
		IR7DeviceFileTransferDelegate FileTransferDelegate { get; set; }

		// @property (nonatomic, weak) id<R7DeviceFileTransferDelegate> fileTransferDelegate;
		[NullAllowed, Export ("fileTransferDelegate", ArgumentSemantic.Weak)]
		NSObject WeakFileTransferDelegate { get; set; }

		[Wrap ("WeakActivationDelegate")]
		R7DeviceActivationDelegate ActivationDelegate { get; set; }

		// @property (nonatomic, weak) id<R7DeviceActivationDelegate> activationDelegate;
		[NullAllowed, Export ("activationDelegate", ArgumentSemantic.Weak)]
		NSObject WeakActivationDelegate { get; set; }

		[Wrap ("WeakMessagingDelegate")]
		R7DeviceMessagingDelegate MessagingDelegate { get; set; }

		// @property (nonatomic, weak) id<R7DeviceMessagingDelegate> messagingDelegate;
		[NullAllowed, Export ("messagingDelegate", ArgumentSemantic.Weak)]
		NSObject WeakMessagingDelegate { get; set; }

		[Wrap ("WeakGprsDelegate")]
		R7DeviceGprsDelegate GprsDelegate { get; set; }

		// @property (nonatomic, weak) id<R7DeviceGprsDelegate> gprsDelegate;
		[NullAllowed, Export ("gprsDelegate", ArgumentSemantic.Weak)]
		NSObject WeakGprsDelegate { get; set; }

		[Wrap ("WeakLogDelegate")]
		R7DeviceLogDelegate LogDelegate { get; set; }

		// @property (nonatomic, weak) id<R7DeviceLogDelegate> logDelegate;
		[NullAllowed, Export ("logDelegate", ArgumentSemantic.Weak)]
		NSObject WeakLogDelegate { get; set; }

		// @property (readonly, nonatomic) R7ConnectionState state;
		[Export ("state")]
		R7ConnectionState State { get; }

		// @property (readonly, nonatomic) R7ActivationState activationState;
		[Export ("activationState")]
		R7ActivationState ActivationState { get; }

		// @property (readonly, nonatomic) NSDate * activationTime;
		[Export ("activationTime")]
		NSDate ActivationTime { get; }

		// @property (readonly, nonatomic) NSString * activationIdentifier;
		[Export ("activationIdentifier")]
		string ActivationIdentifier { get; }

		// @property (readonly, nonatomic) NSUInteger activationMessageIdentifier;
		[Export ("activationMessageIdentifier")]
		nuint ActivationMessageIdentifier { get; }

		// @property (readonly, nonatomic) NSString * activationAccount;
		[Export ("activationAccount")]
		string ActivationAccount { get; }

		// @property (readonly, nonatomic) NSString * hardwareIdentifier;
		[Export ("hardwareIdentifier")]
		string HardwareIdentifier { get; }

		// @property (readonly, nonatomic) NSUInteger creditBalance;
		[Export ("creditBalance")]
		nuint CreditBalance { get; }

		// @property (readonly, nonatomic) NSDate * creditBalanceUpdated;
		[Export ("creditBalanceUpdated")]
		NSDate CreditBalanceUpdated { get; }

		// +(ConnectComms *)getConnectComms;
		[Static]
		[Export ("getConnectComms")]
		
		ConnectComms GetConnectComms { get; }

		// -(void)reset;
		[Export ("reset")]
		void Reset ();

		// -(void)startDiscovery;
		[Export ("startDiscovery")]
		void StartDiscovery ();

		// -(void)startDiscoveryAdvanced:(BOOL)incAllEvents;
		[Export ("startDiscoveryAdvanced:")]
		void StartDiscoveryAdvanced (bool incAllEvents);

		// -(void)stopDiscovery;
		[Export ("stopDiscovery")]
		void StopDiscovery ();

		// -(void)enableWithApplicationIdentifier:(NSString *)applicationIdentifier;
		[Export ("enableWithApplicationIdentifier:")]
		void EnableWithApplicationIdentifier (string applicationIdentifier);

		// -(_Bool)connect:(NSUUID *)device;
		[Export ("connect:")]
		bool Connect (NSUuid device);

		// -(void)disconnect;
		[Export ("disconnect")]
		void Disconnect ();

		// -(void)activate:(NSString *)username password:(NSString *)password;
		[Export ("activate:password:")]
		void Activate (string username, string password);

		// -(void)activateIp:(NSString *)username password:(NSString *)password;
		[Export ("activateIp:password:")]
		void ActivateIp (string username, string password);

		// -(void)activateSatellite:(NSString *)username password:(NSString *)password;
		[Export ("activateSatellite:password:")]
		void ActivateSatellite (string username, string password);

		// -(void)activateCancel;
		[Export ("activateCancel")]
		void ActivateCancel ();

		// -(void)changePin:(NSUInteger)oldPin newPin:(NSUInteger)newPin;
		[Export ("changePin:newPin:")]
		void ChangePin (nuint oldPin, nuint newPin);

		// -(void)unlock:(NSUInteger)pin;
		[Export ("unlock:")]
		void Unlock (nuint pin);

		// -(void)lock;
		[Export ("lock")]
		void Lock ();

		// -(NSUInteger)sendMessageWithString:(NSString *)string;
		[Export ("sendMessageWithString:")]
		nuint SendMessageWithString (string @string);

		// -(NSUInteger)sendMessageWithData:(NSData *)data;
		[Export ("sendMessageWithData:")]
		nuint SendMessageWithData (NSData data);

		// -(NSUInteger)sendMessageWithDataAndIdentifier:(NSData *)data messageId:(NSUInteger)messageId;
		[Export ("sendMessageWithDataAndIdentifier:messageId:")]
		nuint SendMessageWithDataAndIdentifier (NSData data, nuint messageId);

		// -(NSUInteger)creditsForMessageWithString:(NSString *)string;
		[Export ("creditsForMessageWithString:")]
		nuint CreditsForMessageWithString (string @string);

		// -(NSUInteger)creditsForMessageWithData:(NSData *)data;
		[Export ("creditsForMessageWithData:")]
		nuint CreditsForMessageWithData (NSData data);

		// -(void)requestMessageStatusUpdate;
		[Export ("requestMessageStatusUpdate")]
		void RequestMessageStatusUpdate ();

		// -(void)requestCreditBalance;
		[Export ("requestCreditBalance")]
		void RequestCreditBalance ();

		// -(void)requestCreditBalanceIp;
		[Export ("requestCreditBalanceIp")]
		void RequestCreditBalanceIp ();

		// -(BOOL)requestCreditBalanceSatellite;
		[Export ("requestCreditBalanceSatellite")]
		
		bool RequestCreditBalanceSatellite { get; }

		// -(void)requestCreditDeduction:(NSUInteger)credits;
		[Export ("requestCreditDeduction:")]
		void RequestCreditDeduction (nuint credits);

		// -(void)requestCurrentGpsPosition;
		[Export ("requestCurrentGpsPosition")]
		void RequestCurrentGpsPosition ();

		// -(void)requestLastKnownGpsPosition;
		[Export ("requestLastKnownGpsPosition")]
		void RequestLastKnownGpsPosition ();

		// -(void)fileTransferWithData:(NSData *)data fileName:(NSString *)fileName;
		[Export ("fileTransferWithData:fileName:")]
		void FileTransferWithData (NSData data, string fileName);

		// -(void)fileTransferCancel;
		[Export ("fileTransferCancel")]
		void FileTransferCancel ();

		// -(void)raiseGenericAlert:(R7CommandGenericAlertType)alert position:(BOOL)position;
		[Export ("raiseGenericAlert:position:")]
		void RaiseGenericAlert (R7CommandGenericAlertType alert, bool position);

		// -(void)requestSatelliteMessageCheck;
		[Export ("requestSatelliteMessageCheck")]
		void RequestSatelliteMessageCheck ();

		// -(void)requestSatelliteUserMessageCheck;
		[Export ("requestSatelliteUserMessageCheck")]
		void RequestSatelliteUserMessageCheck ();

		// -(void)requestNextMessage;
		[Export ("requestNextMessage")]
		void RequestNextMessage ();

		// -(void)requestGeofenceCentre;
		[Export ("requestGeofenceCentre")]
		void RequestGeofenceCentre ();

		// -(void)requestAlert;
		[Export ("requestAlert")]
		void RequestAlert ();

		// -(void)requestManual;
		[Export ("requestManual")]
		void RequestManual ();

		// -(void)requestBatteryStatus;
		[Export ("requestBatteryStatus")]
		void RequestBatteryStatus ();

		// -(void)requestBeep;
		[Export ("requestBeep")]
		void RequestBeep ();

		// -(void)requestGprsStatus;
		[Export ("requestGprsStatus")]
		void RequestGprsStatus ();

		// -(void)requestSerialDump;
		[Export ("requestSerialDump")]
		void RequestSerialDump ();

		// -(void)requestCancelAlertMode;
		[Export ("requestCancelAlertMode")]
		void RequestCancelAlertMode ();

		// -(void)shippingMode;
		[Export ("shippingMode")]
		void ShippingMode ();

		// -(void)factoryReset;
		[Export ("factoryReset")]
		void FactoryReset ();

		// -(void)requestInstall;
		[Export ("requestInstall")]
		void RequestInstall ();

		// -(void)configure:(NSData *)data;
		[Export ("configure:")]
		void Configure (NSData data);

		// -(void)provision;
		[Export ("provision")]
		void Provision ();

		// -(void)requestSelfTest;
		[Export ("requestSelfTest")]
		void RequestSelfTest ();

		// -(void)requestGprsConfig;
		[Export ("requestGprsConfig")]
		void RequestGprsConfig ();

		// -(void)updateGprsConfig:(NSNumber *)index value:(NSData *)value;
		[Export ("updateGprsConfig:value:")]
		void UpdateGprsConfig (NSNumber index, NSData value);

		// -(BOOL)rawMessagingAvailable;
		[Export ("rawMessagingAvailable")]
		
		bool RawMessagingAvailable { get; }

		// -(NSUInteger)sendRawMessageWithData:(NSData *)data;
		[Export ("sendRawMessageWithData:")]
		nuint SendRawMessageWithData (NSData data);

		// -(NSUInteger)sendRawMessageWithDataAndIdentifier:(NSData *)data messageId:(NSUInteger)messageId;
		[Export ("sendRawMessageWithDataAndIdentifier:messageId:")]
		nuint SendRawMessageWithDataAndIdentifier (NSData data, nuint messageId);

		// -(void)requestLogs:(NSUInteger)year month:(NSUInteger)month day:(NSUInteger)day;
		[Export ("requestLogs:month:day:")]
		void RequestLogs (nuint year, nuint month, nuint day);

		// -(void)requestLogIndex:(NSString *)name;
		[Export ("requestLogIndex:")]
		void RequestLogIndex (string name);

		// -(void)requestLogSegment:(NSString *)name offset:(NSUInteger)offset;
		[Export ("requestLogSegment:offset:")]
		void RequestLogSegment (string name, nuint offset);

		// -(void)cancelLogRequests;
		[Export ("cancelLogRequests")]
		void CancelLogRequests ();

		// -(void)toggleWatchState;
		[Export ("toggleWatchState")]
		void ToggleWatchState ();

		// -(void)disableUsageTimeout;
		[Export ("disableUsageTimeout")]
		void DisableUsageTimeout ();

		// -(void)enableUsageTimeout;
		[Export ("enableUsageTimeout")]
		void EnableUsageTimeout ();

		// -(BOOL)isLocked;
		[Export ("isLocked")]
		
		bool IsLocked { get; }

		// -(BOOL)isActivated;
		[Export ("isActivated")]
		
		bool IsActivated { get; }

		// -(BOOL)isBluetoothReady;
		[Export ("isBluetoothReady")]
		
		bool IsBluetoothReady { get; }

		// -(BOOL)isCreditAvailable;
		[Export ("isCreditAvailable")]
		
		bool IsCreditAvailable { get; }

		// -(BOOL)isMessagingReady;
		[Export ("isMessagingReady")]
		
		bool IsMessagingReady { get; }

		// -(BOOL)isConnected;
		[Export ("isConnected")]
		
		bool IsConnected { get; }

		// -(BOOL)isGprsAttached;
		[Export ("isGprsAttached")]
		
		bool IsGprsAttached { get; }

		// -(BOOL)isCommsAttached;
		[Export ("isCommsAttached")]
		
		bool IsCommsAttached { get; }

		// -(BOOL)isBluetoothDaughterboardAttached;
		[Export ("isBluetoothDaughterboardAttached")]
		
		bool IsBluetoothDaughterboardAttached { get; }

		// -(NSString *)commsVersionAttached;
		[Export ("commsVersionAttached")]
		
		string CommsVersionAttached { get; }

		// -(NSString *)bleVersion;
		[Export ("bleVersion")]
		
		string BleVersion { get; }

		// -(NSString *)code7;
		[Export ("code7")]
		
		string Code7 { get; }

		// -(NSString *)encodeCode7:(NSString *)deviceName;
		[Export ("encodeCode7:")]
		string EncodeCode7 (string deviceName);

		// -(NSString *)convertDataToString:(NSData *)data;
		[Export ("convertDataToString:")]
		string ConvertDataToString (NSData data);

		// -(NSData *)convertStringToData:(NSString *)string;
		[Export ("convertStringToData:")]
		NSData ConvertStringToData (string @string);

		// -(NSUInteger)generateMessageId;
		[Export ("generateMessageId")]
		
		nuint GenerateMessageId { get; }

		// -(BOOL)internetConnectionAvailable;
		[Export ("internetConnectionAvailable")]
		
		bool InternetConnectionAvailable { get; }

		// -(void)requestBrand;
		[Export ("requestBrand")]
		void RequestBrand ();

		// -(ConnectDevice *)currentDevice;
		[Export ("currentDevice")]
		
		ConnectDevice CurrentDevice { get; }

		// -(NSUUID *)currentUuid;
		[Export ("currentUuid")]
		
		NSUuid CurrentUuid { get; }

		// -(void)requestCharacteristic:(CBUUID *)characteristic;
		[Export ("requestCharacteristic:")]
		void RequestCharacteristic (CBUUID characteristic);

		// -(void)notifyCharacteristic:(CBUUID *)characteristic notify:(BOOL)notify;
		[Export ("notifyCharacteristic:notify:")]
		void NotifyCharacteristic (CBUUID characteristic, bool notify);

		// -(void)updateCharacteristic:(CBUUID *)characteristic withData:(NSData *)data;
		[Export ("updateCharacteristic:withData:")]
		void UpdateCharacteristic (CBUUID characteristic, NSData data);

		// -(void)processCommand:(NSData *)buffer;
		[Export ("processCommand:")]
		void ProcessCommand (NSData buffer);

		// -(NSData *)identifier;
		[Export ("identifier")]
		NSData Identifier { get; }

		// -(NSString *)getAppId;
		[Export ("getAppId")]
		string AppId { get; }
	}

	// @interface DeviceAccessory : NSObject
	[BaseType (typeof(NSObject))]
	interface DeviceAccessory
	{
		// @property (readonly, nonatomic) NSArray * params;
		[Export ("params")]
		NSObject[] Params { get; }

		// -(id)initWithParams:(NSArray *)deviceParams;
		[Export ("initWithParams:")]
		IntPtr Constructor (NSObject[] deviceParams);
	}

	// @interface DeviceAccessoryParameter : NSObject
	[BaseType (typeof(NSObject))]
	interface DeviceAccessoryParameter
	{
		// @property (readonly, nonatomic) NSNumber * index;
		[Export ("index")]
		NSNumber Index { get; }

		// @property (readonly, nonatomic) NSString * label;
		[Export ("label")]
		string Label { get; }

		// @property (readonly, nonatomic) NSString * about;
		[Export ("about")]
		string About { get; }

		// @property (readonly, nonatomic) DeviceAccessoryParameterType type;
		[Export ("type")]
		DeviceAccessoryParameterType Type { get; }

		// -(id)initWithIndex:(NSNumber *)index label:(NSString *)label type:(DeviceAccessoryParameterType)type about:(NSString *)about;
		[Export ("initWithIndex:label:type:about:")]
		IntPtr Constructor (NSNumber index, string label, DeviceAccessoryParameterType type, string about);

		// -(void)update:(NSData *)value;
		[Export ("update:")]
		void Update (NSData value);
	}

	// @interface FirmwareCacheRecord : NSObject
	[BaseType (typeof(NSObject))]
	interface FirmwareCacheRecord
	{
		// @property (retain, nonatomic) NSString * type;
		[Export ("type", ArgumentSemantic.Retain)]
		string Type { get; set; }

		// @property (retain, nonatomic) NSString * description;
		[Export ("description", ArgumentSemantic.Retain)]
		string Description { get; set; }

		// @property (assign, nonatomic) int version;
		[Export ("version")]
		int Version { get; set; }

		// @property (retain, nonatomic) NSString * versionFormatted;
		[Export ("versionFormatted", ArgumentSemantic.Retain)]
		string VersionFormatted { get; set; }

		// @property (retain, nonatomic) NSString * name;
		[Export ("name", ArgumentSemantic.Retain)]
		string Name { get; set; }

		// @property (retain, nonatomic) NSString * url;
		[Export ("url", ArgumentSemantic.Retain)]
		string Url { get; set; }

		// @property (retain, nonatomic) NSString * path;
		[Export ("path", ArgumentSemantic.Retain)]
		string Path { get; set; }

		// @property (assign, nonatomic) long at;
		[Export ("at")]
		nint At { get; set; }

		// -(id)init:(NSDictionary *)json;
		[Export ("init:")]
		IntPtr Constructor (NSDictionary json);

		// -(NSDictionary *)toJson;
		[Export ("toJson")]
		
		NSDictionary ToJson { get; }

		// -(NSData *)getBytes;
		[Export ("getBytes")]
		
		NSData Bytes { get; }
	}

	// typedef void (^FirmwareUpdaterCheckCallback)(_Bool, NSString *, NSUInteger);
	delegate void FirmwareUpdaterCheckCallback (bool arg0, string arg1, nuint arg2);

	// typedef void (^FirmwareUpdaterCallback)();
	delegate void FirmwareUpdaterCallback ();

	// typedef void (^FirmwareUpdaterProgressCallback)(NSUInteger, NSUInteger, NSString *);
	delegate void FirmwareUpdaterProgressCallback (nuint arg0, nuint arg1, string arg2);

	// typedef void (^FirmwareUpdaterErrorCallback)(R7FirmwareUpdaterError);
	delegate void FirmwareUpdaterErrorCallback (R7FirmwareUpdaterError arg0);

	// @interface FirmwareUpdater : NSObject <R7DeviceFileTransferDelegate>
	[BaseType (typeof(NSObject))]
	interface FirmwareUpdater : IR7DeviceFileTransferDelegate
	{
		// -(void)check:(FirmwareUpdaterCheckCallback)callback;
		[Export ("check:")]
		void Check (FirmwareUpdaterCheckCallback callback);

		// -(void)checkCached:(FirmwareUpdaterCheckCallback)callback;
		[Export ("checkCached:")]
		void CheckCached (FirmwareUpdaterCheckCallback callback);

		// -(void)download:(FirmwareUpdaterCallback)callback;
		[Export ("download:")]
		void Download (FirmwareUpdaterCallback callback);

		// -(void)transfer:(FirmwareUpdaterCallback)callback;
		[Export ("transfer:")]
		void Transfer (FirmwareUpdaterCallback callback);

		// -(void)install:(FirmwareUpdaterCallback)callback;
		[Export ("install:")]
		void Install (FirmwareUpdaterCallback callback);

		// -(void)onError:(FirmwareUpdaterErrorCallback)callback;
		[Export ("onError:")]
		void OnError (FirmwareUpdaterErrorCallback callback);

		// -(void)onProgress:(FirmwareUpdaterProgressCallback)callback;
		[Export ("onProgress:")]
		void OnProgress (FirmwareUpdaterProgressCallback callback);

		// -(void)simpleUpgrade:(FirmwareUpdaterCallback)callback;
		[Export ("simpleUpgrade:")]
		void SimpleUpgrade (FirmwareUpdaterCallback callback);
	}

	// @protocol CachedFirmwareUpdater
	/*
  Check whether adding [Model] to this declaration is appropriate.
  [Model] is used to generate a C# class that implements this protocol,
  and might be useful for protocols that consumers are supposed to implement,
  since consumers can subclass the generated class instead of implementing
  the generated interface. If consumers are not supposed to implement this
  protocol, then [Model] is redundant and will generate code that will never
  be used.
*/[Protocol]
	public interface ICachedFirmwareUpdater
	{
		// @required -(void)onComplete;
		[Abstract]
		[Export ("onComplete")]
		void OnComplete ();

		// @required -(void)onError:(R7FirmwareUpdaterError)error;
		[Abstract]
		[Export ("onError:")]
		void OnError (R7FirmwareUpdaterError error);

		// @required -(void)onProgress:(NSString *)status;
		[Abstract]
		[Export ("onProgress:")]
		void OnProgress (string status);
	}

	// @interface FirmwareCache : NSObject
	[BaseType (typeof(NSObject))]
	interface FirmwareCache
	{
		// -(void)update:(id<CachedFirmwareUpdater>)callback;
		//[Export ("update:")]
		//void Update (ICachedFirmwareUpdater callback);

		// -(BOOL)clearCache;
		[Export ("clearCache")]
		
		bool ClearCache { get; }

		// -(NSArray<FirmwareCacheRecord *> *)getCache;
		[Export ("getCache")]
		
		FirmwareCacheRecord[] Cache { get; }

		// -(NSArray<FirmwareCacheRecord *> *)getCache:(NSString *)type version:(NSUInteger)version;
		[Export ("getCache:version:")]
		FirmwareCacheRecord[] GetCache (string type, nuint version);
	}

	// @interface R7GenericDevice : ConnectDevice
	[BaseType (typeof(ConnectDevice))]
	interface R7GenericDevice
	{
		// @property (readonly, nonatomic) DeviceAccessory * accessory;
		[Export ("accessory")]
		DeviceAccessory Accessory { get; }

		// -(void)requestParameter:(R7GenericDeviceParameter)parameter;
		[Export ("requestParameter:")]
		void RequestParameter (R7GenericDeviceParameter parameter);

		// -(void)updateParameter:(R7GenericDeviceParameter)identifier value:(NSData *)value;
		[Export ("updateParameter:value:")]
		void UpdateParameter (R7GenericDeviceParameter identifier, NSData value);

		// -(void)notifyParameter:(R7GenericDeviceParameter)identifier notify:(BOOL)notify;
		[Export ("notifyParameter:notify:")]
		void NotifyParameter (R7GenericDeviceParameter identifier, bool notify);
	}
}
