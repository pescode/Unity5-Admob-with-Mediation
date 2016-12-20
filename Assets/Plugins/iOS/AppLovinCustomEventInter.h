//
// AppLovin <--> AdMob Network Adaptors
//

@import GoogleMobileAds;
@import UIKit;
@import AppLovinSDK;

// Use the below import statements if not integrating our SDK as a first-class framework
//#import "ALAdService.h"
//#import "ALInterstitialAd.h"

@interface AppLovinCustomEventInter : NSObject <GADCustomEventInterstitial, ALAdLoadDelegate, ALAdDisplayDelegate>

@property (strong, atomic) ALAd* appLovinAd;

@end
