#import <Foundation/Foundation.h>

@interface DeviceLogIndex : NSObject {
    

}

@property (nonatomic,retain) NSString* date;
@property (nonatomic,retain) NSString* file;
@property (nonatomic,assign) NSUInteger hour;
@property (nonatomic,assign) NSUInteger offset;
@property (nonatomic,assign) NSUInteger length;
@property (nonatomic,assign) bool selected;
@property (nonatomic,retain) NSMutableString* payload;

-(bool)isDownloaded;

@end
