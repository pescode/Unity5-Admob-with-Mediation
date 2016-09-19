@import GoogleMobileAds;
@import Foundation;

#import <Chartboost/Chartboost.h>

/// Keys for the Chartboost extra assets.
@interface GADMChartboostExtras : NSObject<GADAdNetworkExtras>

/// Chartboost custom framework.
@property(nonatomic, assign) CBFramework framework;

/// Chartboost custom framework version.
@property(nonatomic, copy) NSString *frameworkVersion;

@end
