#import <Foundation/Foundation.h>

@interface FirmwareCacheRecord : NSObject {
    
    NSString* type;
    NSString* description;
    int version;
    NSString* versionFormatted;
    NSString* name;
    NSString* url;
    NSString* path;
    long at;
        
}

@property (nonatomic,retain) NSString* type;
@property (nonatomic,retain) NSString* description;
@property (nonatomic,assign) int version;
@property (nonatomic,retain) NSString* versionFormatted;
@property (nonatomic,retain) NSString* name;
@property (nonatomic,retain) NSString* url;
@property (nonatomic,retain) NSString* path;
@property (nonatomic,assign) long at;

-(id)init:(NSDictionary*)json;

-(NSDictionary*)toJson;
-(NSData*)getBytes;

@end
