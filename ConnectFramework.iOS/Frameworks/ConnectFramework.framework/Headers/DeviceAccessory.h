#import <Foundation/Foundation.h>

@interface DeviceAccessory : NSObject {
    
    NSArray* params;
        
}

@property (nonatomic,readonly) NSArray* params;

-(id)initWithParams:(NSArray*)deviceParams;

@end
