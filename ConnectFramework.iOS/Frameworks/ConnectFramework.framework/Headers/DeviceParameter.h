#import <Foundation/Foundation.h>
#import <CoreBluetooth/CoreBluetooth.h>

@interface DeviceParameter : NSObject {
}

@property (nonatomic,assign) NSUInteger identifier;
@property (nonatomic,retain) CBUUID* characteristic;
@property (nonatomic,retain) NSString* label;
@property (nonatomic,retain) NSString* description;
@property (nonatomic,assign) BOOL readonly;
@property (nonatomic,assign) BOOL available;

@property (nonatomic,readonly) uint16_t attributeId;
@property (nonatomic,readonly) NSMutableDictionary* options;
@property (nonatomic,readonly) NSMutableArray* optionsIndex;
@property (nonatomic,readonly) NSUInteger cachedValue;
@property (nonatomic,readonly) NSData* cachedValueBytes;
@property (nonatomic,readonly) NSDate* cachedTime;

-(id)initWithIdentifier:(NSUInteger)identifier characteristic:(CBUUID*)characteristic label:(NSString*)label description:(NSString*)description;
-(id)initWithIdentifier:(NSUInteger)identifier attributeId:(NSUInteger)attributeId characteristic:(CBUUID*)characteristic label:(NSString*)label description:(NSString*)description;


-(void)addValueOption:(NSUInteger)value withLabel:(NSString*)valueLabel;

-(NSString*)labelForValue:(NSUInteger)value;

-(void)updateCacheValue:(NSData*)value;

-(BOOL)isCacheValueUsable;

@end
