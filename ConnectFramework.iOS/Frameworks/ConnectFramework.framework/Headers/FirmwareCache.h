#import <Foundation/Foundation.h>
#import "FirmwareCacheRecord.h"
#import "FirmwareUpdater.h"

@protocol CachedFirmwareUpdater

    -(void)onComplete;

    -(void)onError:(R7FirmwareUpdaterError) error;

    -(void)onProgress:(NSString*)status;

@end

@interface FirmwareCache : NSObject {
    
}

-(void)update:(id<CachedFirmwareUpdater>)callback;
-(BOOL)clearCache;
-(NSArray<FirmwareCacheRecord*>*)getCache;
-(NSArray<FirmwareCacheRecord*>*)getCache:(NSString*)type version:(NSUInteger)version;

@end
