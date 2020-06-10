#import <Foundation/Foundation.h>
#import <CoreBluetooth/CoreBluetooth.h>
#import <CoreLocation/CoreLocation.h>
#import "ConnectDevice.h"
#import "R7Enums.h"
#import "DeviceLogIndex.h"


//Response
@protocol R7DeviceResponseDelegate <NSObject>

    -(void)deviceReady;

    -(void)deviceConnected:(ConnectDevice*)device activated:(R7ActivationState)activated locked:(BOOL)locked;

    -(void)deviceDisconnected;

    @optional

        -(void)deviceParameterUpdated:(DeviceParameter*)parameter;

        -(void)deviceBatteryUpdated:(NSUInteger)battery at:(NSDate*)timestamp;

        -(void)deviceStateChanged:(R7ConnectionState)stateTo stateFrom:(R7ConnectionState)stateFrom;

        -(void)deviceError:(R7DeviceError)error;

        -(void)deviceLockStatusUpdated:(R7LockState)state;

        -(void)deviceCommandReceived:(R7CommandType)command;

        -(void)locationUpdated:(CLLocation*)location;

        -(void)creditBalanceUpdated:(NSUInteger)credit;

        -(void)usageTimeout;

        -(void)deviceSerialDump:(NSData*)data;

        -(void)deviceNameUpdated:(NSString*)name;

        -(void)deviceStatusUpdated:(NSUInteger)field value:(NSUInteger)value;

@end


//Discovery
@protocol R7DeviceDiscoveryDelegate <NSObject>

    -(void)discoveryStopped;

    -(void)discoveryFoundDevice:(NSUUID*)deviceIdentifier name:(NSString*)deviceName;

    @optional

        -(void)discoveryStarted;

        -(void)discoveryUpdatedDevice:(NSUUID*)deviceIdentifier name:(NSString*)deviceName rssi:(NSNumber*)rssi;

@end


//Activation
@protocol R7DeviceActivationDelegate <NSObject>

    -(void)activationCompleted:(NSString*)activationIdentifier previousDevice:(NSString *)previousDevice accountName:(NSString*)accountName;

    -(void)activationFailed:(R7ActivationError)error;

    -(void)activationDesist:(R7ActivationDesistStatus)status newDevice:(NSString*)newDevice;

 @optional

    -(void)activationStarted:(R7ActivationMethod)method;

    -(void)activationStateUpdated:(R7ActivationState)state;

@end


//Messaging
@protocol R7DeviceMessagingDelegate <NSObject>

    -(void)messageProgressCompleted:(NSUInteger)messageId;

    -(BOOL)messageStatusUpdated:(uint16_t)messageId status:(R7MessageStatus)status;                                             //True = Acknowledges Message

    -(void)inboxUpdated:(NSUInteger)messages;
    -(BOOL)messageReceived:(uint16_t)messageId data:(NSData*)data;                                                              //True = Acknowledges Message Status

@optional

    -(void)messageProgressUpdated:(NSUInteger)messageId part:(NSUInteger)part totalParts:(NSInteger)totalParts;                 //Progress Sending Message to Device (20 byte chunks)

    -(void)nextMessageRequested;
    -(void)mailboxCheckRequested;
    -(void)messageCheckFinished;

@end


//File Transfer
@protocol R7DeviceFileTransferDelegate <NSObject>

    -(void)fileTransferStarted;
    -(void)fileTransferCompleted;
    -(void)fileTransferProgressUpdate:(NSUInteger)progress total:(NSUInteger)total;
    -(void)fileTransferError;

@end


//GPRS
@protocol R7DeviceGprsDelegate <NSObject>

    -(void)gprsConfigResponse:(NSDictionary*)config;
    -(void)gprsConfigured;

@end


//Logs
@protocol R7DeviceLogDelegate <NSObject>

    -(void)logListing:(NSArray<NSString*>*)logs;
    -(void)logIndicesReceived:(NSArray<DeviceLogIndex*>*)indices;
    -(void)logSegmentReceived:(NSString*)filename offset:(NSUInteger)offset data:(NSData*)data;
    -(void)logError:(R7LogFileError)error;

@end
