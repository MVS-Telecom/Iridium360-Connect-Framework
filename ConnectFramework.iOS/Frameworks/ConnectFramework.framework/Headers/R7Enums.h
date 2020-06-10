#import <Foundation/Foundation.h>

typedef NS_ENUM(NSUInteger, R7ActivationState) {
    
    R7ActivationStateUnknown = 0,
    R7ActivationStatePending = 1,
    R7ActivationStateActivating = 2,
    R7ActivationStateActivated = 3,
    R7ActivationStateError = 99,
    
};

typedef NS_ENUM(NSUInteger, R7ActivationError) {
    
    R7ActivationErrorTimeout = 1,
    R7ActivationErrorAuthentication = 2,
    R7ActivationErrorCommunication = 3,
    R7ActivationErrorState = 4,
    R7ActivationErrorCapability = 5
    
};

typedef NS_ENUM(NSUInteger, R7ActivationMethod) {
    
    R7ActivationMethodInternet = 1,
    R7ActivationMethodSatellite = 2
    
};

typedef NS_ENUM(NSUInteger, R7ActivationDesistStatus) {
  
    R7ActivationDesistStatusNoActivation = 0,   //Activated Elsewhere, or no Acitivatio
    R7ActivationDesistStatusNoApp = 1           //App has been deactivated
    
};

//Device
typedef NS_ENUM(NSUInteger, R7ConnectionMode) {
    
    R7ConnectionModeManual = 1,
    R7ConnectionModeAutomatic = 2
    
};

typedef NS_ENUM(NSUInteger, R7ConnectionState) {
    
    R7ConnectionStateIdle = 0,
    R7ConnectionStateOff = 1,
    R7ConnectionStateReady = 2,
    R7ConnectionStateDiscovering = 3,
    R7ConnectionStateConnecting = 4,
    R7ConnectionStateConnected = 5
    
};

typedef NS_ENUM(NSUInteger, R7MessageStatus) {
    
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
    
};

typedef NS_ENUM(NSUInteger, R7MessageChunkType) {
    
    R7MessageChunkTypeMessage = 1,
    R7MessageChunkTypeFile = 2
    
};

typedef NS_ENUM(NSUInteger, R7CommandType) {
    
    R7CommandTypeSendMessage = 1,
    R7CommandTypeGetNextMessage = 2,
    R7CommandTypeDeleteMessage = 3,
    R7CommandTypeAcknowledgeMessageStatus = 4,
    R7CommandTypeInternal = 5,
    R7CommandTypeSendFileSegment = 6,
    R7CommandTypePin = 7,
    R7CommandTypeActionRequest = 8,
    R7CommandTypeGenericAlert = 9,
    R7CommandTypeSerialDump = 10,
    R7CommandTypeSetGprsConfig = 11,
    R7CommandTypeGetGprsConfig = 12,
    R7CommandTypeTrackerStatus = 13,
    R7CommandTypeSetParameter = 14,
    R7CommandTypeGetParameter = 15,
    R7CommandTypeGetLogSegment = 16,
    R7CommandTypeGetLogListing = 17,
    R7CommandTypeGetLogIndex = 18
    
};

typedef NS_ENUM(NSUInteger, R7CommandActionRequestType) {
    
    R7CommandActionRequestTypeSendAlert = 0,
    R7CommandActionRequestTypeSendManual = 1,
    R7CommandActionRequestTypeInstallUpdates = 2,
    R7CommandActionRequestTypeMailboxCheck = 3,
    R7CommandActionRequestTypeUpdateMessageStatus = 4,
    R7CommandActionRequestPositionUpdateLastKnown = 5,
    R7CommandActionRequestPositionUpdate = 6,
    R7CommandActionRequestTypeBatteryUpdate = 7,
    R7CommandActionRequestShippingMode = 8,
    R7CommandActionRequestActivation = 9,
    R7CommandActionRequestBeep = 10,
    R7CommandActionRequestFactoryReset = 11, 
    R7CommandActionRequestGeofenceCentre = 12,
    R7CommandActionRequestGprsStatus = 13,
    R7CommandActionRequestCancelAlertMode = 14,
    R7CommandActionRequestSelfTest = 15,
    R7CommandActionRequestTypeToggleWatchState = 16
    
};

typedef NS_ENUM(NSUInteger, R7CommandGenericAlertType) {
    
    R7CommandGenericAlertTypeA = 100,
    R7CommandGenericAlertTypeB = 101,
    R7CommandGenericAlertTypeC = 102,
    R7CommandGenericAlertTypeD = 103,
    R7CommandGenericAlertTypeE = 104,
    R7CommandGenericAlertTypeF = 105,
    R7CommandGenericAlertTypeG = 106,
    R7CommandGenericAlertTypeH = 107,
    R7CommandGenericAlertTypeI = 108,
    R7CommandGenericAlertTypeJ = 109,
    R7CommandGenericAlertTypeK = 110,
    R7CommandGenericAlertTypeL = 111,
    R7CommandGenericAlertTypeM = 112,
    R7CommandGenericAlertTypeN = 113,
    R7CommandGenericAlertTypeO = 114,
    R7CommandGenericAlertTypeP = 115,
    R7CommandGenericAlertTypeQ = 116,
    R7CommandGenericAlertTypeR = 117,
    R7CommandGenericAlertTypeS = 118,
    R7CommandGenericAlertTypeT = 119,
    R7CommandGenericAlertTypeU = 120,
    R7CommandGenericAlertTypeV = 121,
    R7CommandGenericAlertTypeW = 122,
    R7CommandGenericAlertTypeX = 123,
    R7CommandGenericAlertTypeY = 124,
    R7CommandGenericAlertTypeZ = 125

};

typedef NS_ENUM(NSUInteger, R7DeviceError) {
    
    R7DeviceErrorFailedToConnect = 1,
    R7DeviceErrorWriteTimeout = 2,
    R7DeviceErrorBluetoothError = 3,
    R7DeviceErrorLostConnetion = 4,
    R7DeviceErrorNoCredit = 5,
    R7DeviceErrorStateError = 99
    
};

typedef NS_ENUM(NSUInteger, R7LockState) {

    R7LockStateUnlocked = 0,
    R7LockStateLocked = 1,
    R7LockStateIncorrectPin = 2,
    
    R7LockStateUnknown = 99
    
};

//File Transfer
typedef NS_ENUM(NSUInteger, R7FileTransferError) {
    
    R7FileTransferErrorFileTooLarge = 1
    
};

//Log File
typedef NS_ENUM(NSUInteger, R7LogFileError) {
    
    R7LogFileErrorFileNotFound = 1,
    R7LogFileErrorSegmentNotFound = 2,
    
    R7LogFileErrorUnknown = 99
    
};

typedef NS_ENUM(NSUInteger, R7DeviceContext) {
    
    R7DeviceContextSatellite = 1,
    R7DeviceContextCellular = 2,
    R7DeviceContexDistress = 3
    
};

typedef NS_ENUM(NSUInteger, R7DeviceWatchState) {
    
    R7DeviceWatchStateOff = 1,
    R7DeviceWatchStateOnAwaitingConfirmation = 2,
    R7DeviceWatchStateOn = 3,
    R7DeviceWatchStateOffAwaitingConfirmation = 4
    
};
