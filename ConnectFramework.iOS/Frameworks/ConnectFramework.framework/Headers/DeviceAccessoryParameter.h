#import <Foundation/Foundation.h>


typedef NS_ENUM(NSUInteger, DeviceAccessoryParameterType) {
    DeviceAccessoryParameterTypeText = 0,
    DeviceAccessoryParameterTypeInteger = 1,
    DeviceAccessoryParameterTypeHostOrIp = 2
};

@interface DeviceAccessoryParameter : NSObject {
    
    NSNumber* index;
    NSString* label;
    NSString* about;
    DeviceAccessoryParameterType type;
    
}

@property (nonatomic,readonly) NSNumber* index;
@property (nonatomic,readonly) NSString* label;
@property (nonatomic,readonly) NSString* about;
@property (nonatomic,readonly) DeviceAccessoryParameterType type;

-(id)initWithIndex:(NSNumber*)index label:(NSString*)label type:(DeviceAccessoryParameterType)type about:(NSString*)about;

-(void)update:(NSData*)value;

@end
