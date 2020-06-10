#import <Foundation/Foundation.h>
#import "ConnectDevice.h"
#import "R7Device.h"

@interface ConnectComms : NSObject <CBCentralManagerDelegate, CBPeripheralDelegate> {
    
}

@property (nonatomic,weak) id <R7DeviceDiscoveryDelegate> discoveryDelegate;
@property (nonatomic,weak) id <R7DeviceResponseDelegate> responseDelegate;
@property (nonatomic,weak) id <R7DeviceFileTransferDelegate> fileTransferDelegate;
@property (nonatomic,weak) id <R7DeviceActivationDelegate> activationDelegate;
@property (nonatomic,weak) id <R7DeviceMessagingDelegate> messagingDelegate;
@property (nonatomic,weak) id <R7DeviceGprsDelegate> gprsDelegate;
@property (nonatomic,weak) id <R7DeviceLogDelegate> logDelegate;

@property (nonatomic,readonly) R7ConnectionState state;
@property (nonatomic,readonly) R7ActivationState activationState;
@property (nonatomic,readonly) NSDate* activationTime;
@property (nonatomic,readonly) NSString* activationIdentifier;
@property (nonatomic,readonly) NSUInteger activationMessageIdentifier;

@property (nonatomic,readonly) NSString* activationAccount;
@property (nonatomic,readonly) NSString* hardwareIdentifier;
@property (nonatomic,readonly) NSUInteger creditBalance;
@property (nonatomic,readonly) NSDate* creditBalanceUpdated;


//General
+(ConnectComms*)getConnectComms;
-(void)reset;

//Discovery
-(void)startDiscovery;
-(void)startDiscoveryAdvanced:(BOOL)incAllEvents;
-(void)stopDiscovery;

//Connection
-(void)enableWithApplicationIdentifier:(NSString*)applicationIdentifier;
-(bool)connect:(NSUUID*)device;
-(void)disconnect;

//Activation
-(void)activate:(NSString*)username password:(NSString*)password;
-(void)activateIp:(NSString*)username password:(NSString*)password;
-(void)activateSatellite:(NSString*)username password:(NSString*)password;
-(void)activateCancel;

//Locking
-(void)changePin:(NSUInteger)oldPin newPin:(NSUInteger)newPin;
-(void)unlock:(NSUInteger)pin;
-(void)lock;

//Messaging
-(NSUInteger)sendMessageWithString:(NSString*)string;
-(NSUInteger)sendMessageWithData:(NSData*)data;
-(NSUInteger)sendMessageWithDataAndIdentifier:(NSData*)data messageId:(NSUInteger)messageId;
-(NSUInteger)creditsForMessageWithString:(NSString*)string;
-(NSUInteger)creditsForMessageWithData:(NSData*)data;
-(void)requestMessageStatusUpdate;

//Credits
-(void)requestCreditBalance;
-(void)requestCreditBalanceIp;
-(BOOL)requestCreditBalanceSatellite;
-(void)requestCreditDeduction:(NSUInteger)credits;

//GPS
-(void)requestCurrentGpsPosition;
-(void)requestLastKnownGpsPosition;

//File Transfer
-(void)fileTransferWithData:(NSData*)data fileName:(NSString*)fileName;
-(void)fileTransferCancel;

//Alert
-(void)raiseGenericAlert:(R7CommandGenericAlertType)alert position:(BOOL)position;

//Other Commands
-(void)requestSatelliteMessageCheck;
-(void)requestSatelliteUserMessageCheck;
-(void)requestNextMessage;
-(void)requestGeofenceCentre;
-(void)requestAlert;
-(void)requestManual;
-(void)requestBatteryStatus;
-(void)requestBeep;
-(void)requestGprsStatus;
-(void)requestSerialDump;
-(void)requestCancelAlertMode;
-(void)shippingMode;
-(void)factoryReset;
-(void)requestInstall;
-(void)configure:(NSData*)data;
-(void)provision;
-(void)requestSelfTest;

//GPRS
-(void)requestGprsConfig;
-(void)updateGprsConfig:(NSNumber*)index value:(NSData*)value;

//Raw Commands
-(BOOL)rawMessagingAvailable;
-(NSUInteger)sendRawMessageWithData:(NSData*)data;
-(NSUInteger)sendRawMessageWithDataAndIdentifier:(NSData*)data messageId:(NSUInteger)messageId;

//Logs
-(void)requestLogs:(NSUInteger)year month:(NSUInteger)month day:(NSUInteger)day;
-(void)requestLogIndex:(NSString*)name;
-(void)requestLogSegment:(NSString*)name offset:(NSUInteger)offset;
-(void)cancelLogRequests;

//Context
-(void)toggleWatchState;

//Usage Timeout
-(void)disableUsageTimeout;
-(void)enableUsageTimeout;

//State
-(BOOL)isLocked;
-(BOOL)isActivated;
-(BOOL)isBluetoothReady;
-(BOOL)isCreditAvailable;
-(BOOL)isMessagingReady;
-(BOOL)isConnected;
-(BOOL)isGprsAttached;
-(BOOL)isCommsAttached;
-(BOOL)isBluetoothDaughterboardAttached;
-(NSString*)commsVersionAttached;
-(NSString*)bleVersion;
-(NSString*)code7;
-(NSString*)encodeCode7:(NSString*)deviceName;

//Helpers
-(NSString*)convertDataToString:(NSData*)data;
-(NSData*)convertStringToData:(NSString*)string;
-(NSUInteger)generateMessageId;
-(BOOL)internetConnectionAvailable;
-(void)requestBrand;

-(ConnectDevice*)currentDevice;
-(NSUUID*)currentUuid;

-(void)requestCharacteristic:(CBUUID*)characteristic;
-(void)notifyCharacteristic:(CBUUID*)characteristic notify:(BOOL)notify;
-(void)updateCharacteristic:(CBUUID*)characteristic withData:(NSData*)data;

//DEBUG
-(void)processCommand:(NSData*)buffer;
-(NSData*)identifier;
-(NSString*)getAppId;

@end
