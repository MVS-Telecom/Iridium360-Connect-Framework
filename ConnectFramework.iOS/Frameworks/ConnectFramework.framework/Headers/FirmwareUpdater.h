#import <Foundation/Foundation.h>
#import "R7Device.h"

typedef NS_ENUM(NSUInteger, R7FirmwareUpdaterError) {
    
    //Not connected/lost connection during process
    R7FirmwareUpdaterErrorNoDevice = 0,
    
    //Unable to establish connection to Firmware Service
    R7FirmwareUpdaterErrorNoInternet = 1,
    
    //Unexpected response received from Firmware Service
    R7FirmwareUpdaterErrorBadResponse = 2,
    
    //Failed to Download Firmware
    R7FirmwareUpdaterErrorDownloadFailure = 3,
    
    //Error occured transfering file to Tracker
    R7FirmwareUpdaterErrorTransfer = 4,
    
    //Unable to perform request with current state
    R7FirmwareUpdaterErrorInvalidState = 5
    
};

typedef void (^FirmwareUpdaterCheckCallback)(bool updates, NSString* description, NSUInteger fileCount);
typedef void (^FirmwareUpdaterCallback)(void);
typedef void (^FirmwareUpdaterProgressCallback)(NSUInteger progress, NSUInteger total, NSString* status);
typedef void (^FirmwareUpdaterErrorCallback)(R7FirmwareUpdaterError error);

@interface FirmwareUpdater : NSObject <R7DeviceFileTransferDelegate> {
}

//Check for Updates
-(void)check:(FirmwareUpdaterCheckCallback)callback;

//Check for Updates (CACHED)
-(void)checkCached:(FirmwareUpdaterCheckCallback)callback;

//Download Updates
-(void)download:(FirmwareUpdaterCallback)callback;

//Transfer File(s) to Tracker
-(void)transfer:(FirmwareUpdaterCallback)callback;

//Install Updates on Tracker
-(void)install:(FirmwareUpdaterCallback)callback;

//Register Error Handler
-(void)onError:(FirmwareUpdaterErrorCallback)callback;

//Register Progress Handler
-(void)onProgress:(FirmwareUpdaterProgressCallback)callback;

//Convienience : Will do Check/Download/Transfer/Install
-(void)simpleUpgrade:(FirmwareUpdaterCallback)callback;

@end
