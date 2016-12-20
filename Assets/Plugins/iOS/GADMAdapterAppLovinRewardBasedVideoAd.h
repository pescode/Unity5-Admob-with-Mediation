#import <Foundation/Foundation.h>
#import <GoogleMobileAds/GoogleMobileAds.h>

//  For some older versions of AdMob iOS SDK, you would also need to add the header files from the "Mediation Adapters" folder, coming with the SDK, into your Xcode project, and to use the import below
//#import "GADMRewardBasedVideoAdNetworkAdapterProtocol.h"

@interface GADMAdapterAppLovinRewardBasedVideoAd : NSObject<GADMRewardBasedVideoAdNetworkAdapter>

@end

@interface GADMExtrasAppLovin : NSObject<GADAdNetworkExtras>

@property(nonatomic, assign) NSUInteger requestNumber;

@end
