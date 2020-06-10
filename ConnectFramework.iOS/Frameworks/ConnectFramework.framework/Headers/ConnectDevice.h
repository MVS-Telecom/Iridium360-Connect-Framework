#import <Foundation/Foundation.h>
#import <CoreBluetooth/CoreBluetooth.h>
#import "DeviceParameter.h"
#import "DeviceParameterGroup.h"

@class ConnectComms;

@interface ConnectDevice : NSObject {
    
    NSMutableArray<DeviceParameter*>* deviceParameters;
    NSMutableArray<DeviceParameter*>* notifiable;
    
    NSArray<DeviceParameterGroup*>* parameterGroups;
    
}

@property (nonatomic,readonly) NSString* name;
@property (nonatomic,readonly) NSString* version;


-(id)initWithConnectComms:(ConnectComms*)comms name:(NSString*)name version:(NSString*)version;

//Parameters
-(NSArray<DeviceParameter*>*)parameters;
-(NSArray<DeviceParameterGroup*>*)parameterGroups;

-(DeviceParameter*)parameterForCharacteristic:(CBUUID*)characteristic;
-(DeviceParameter*)parameterForIdentifier:(NSUInteger)identifier;
-(DeviceParameter*)parameterForAttributeId:(uint16_t)attributeId;

-(void)requestAll;
-(void)notifyAll:(BOOL)notify;
-(void)valueUpdated:(CBUUID*)characteristic value:(NSData*)value;
-(void)discovered:(CBUUID*)characteristic;

-(void)request:(NSUInteger)identifier;
-(void)update: (NSUInteger)identifier value:(NSData*)value;
-(void)notify: (NSUInteger)identifier notify:(BOOL)notify;

-(NSArray*)characteristics;

-(NSString*) getHardwareType;
-(int) getVersionAsInt;

@end
