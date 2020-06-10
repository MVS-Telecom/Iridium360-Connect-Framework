#import <Foundation/Foundation.h>
#import "DeviceParameter.h"
#import "DeviceParameterGroup.h"

@interface DeviceParameterGroup : NSObject {
}

@property (nonatomic, readonly) NSArray<DeviceParameter*>* parameters;
@property (nonatomic, readonly) NSArray<DeviceParameterGroup*>* groups;
@property (nonatomic, readonly) NSString* name;
@property (nonatomic, readonly) NSString* explanation;
@property (nonatomic, assign) bool visible;

-(id)initWithName:(NSString*)name
     explanation:(NSString*)explanation
       parameters:(NSArray<DeviceParameter*>*)parameters
           groups:(NSArray<DeviceParameterGroup*>*)groups;

@end
