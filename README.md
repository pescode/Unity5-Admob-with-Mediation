# Unity5-Admob-with-Mediation
Google Mobile Ads for iOS & Android with mediation.
Interstitial & Rewarded Ads

Show Admob ads with mediation (interstitial &amp; rewarded) from different providers like AdColony, AppLovin, Chartboost, Facebook Audience Network, UnityADS &amp; Vungle

### HOW TO USE ###

* Download the lastest admobWithMediation.unitypackage from here: https://goo.gl/HnYYOQ. 
Import this package in your project.
* The project contains the adaptors, sdk, manifest and xcode setup ready to use admob with AdColony, AppLovin, Chartboost, Facebook Audience Network, UnityADS and Vungle. You can write/use your own code to display ads or you can optionally use RshkAds.cs .
* In case that you want to use RshkAds.cs, set your interstitial Ad Unit ID and rewarded Ad Unit ID for iOS and Android in RshkAds.cs (Line 41-45).
* Create an empty GameObject and add RshkAds.cs to it as soon as possible (when the game launch, in your loading screen or in the first scene so it can start to fetch interstitials and rewarded ads ASAP).
* To setup your APPLOVIN SDK KEY for Android OPEN Plugins/Android/GoogleMobileAds/AndroidManifest.xml . Add your SDK on line 41.
* To setup your APPLOVIN SDK KEY for iOS OPEN Editor/PostBuildProcess.cs. Add your SDK on line 130.
* If your want to use Facebook Audience Network setup your Facebook App ID and open Plugins/Android/GoogleMobileAds/AndroidManifest.xml. Add your ID on line 54 ex: fb123456789
* To setup Facebook Audience Network on iOS open Editor/PostBuildProcess.cs and add your app name and id in lines 121, 124, 127
* To display interstitial Ads just call RshkAds.ShowInterstitial() anywhere in your code. As soon as the ads closes in will fetch automatically another ad for you.
* To use rewarded Ads you must do the following
In your game logic, in the part where you want to know when the user has closed the rewarded ads and do something else (like give virtual coins or another revive) you must setup a listener like this
* If you want to test, on iOS is pretty straighforward, you can open Test.scene and run. On Android you will have to setup Google Play Games Services first, the plugin version compatible and tested with this is available in releases folder.

void OnEnable()
{
 RshkAds.OnRewardedCompleted += YourFunctionToDoSomething;
}

void OnDisable()
{
 RshkAds.OnRewardedCompleted -= YourFunctionToDoSomething;
}

void YourFunctionToDoSomething()
{
 // reward the player HERE!
}

Before showing your special button like WATCH AD FOR 50 COINS or CONTINUE GAME you probably want to know if there is a RewardedAD ready to show... to know that just call the function that returns a boolean 

 RshkAds.IsRewardedAdsAvailable()

To show the rewarded AD call

 RshkAds.ShowRewarded()

### Plugins ###

* GoogleMobileAds (Unity) v3.0.7 https://github.com/googleads/googleads-mobile-unity/releases .

### iOS ###

* Google-Mobile-Ads-SDK w/ Mediation Adapters Headers v7.11.0
* AdColony Adapter 1.5 & SDK 2.6.2. WARNING: ADCOLONY CURRENTLY DOES NOT SUPPORT BITCODE. IS THIS IS A PROBLEM FOR YOU, REMOVE THE FOLLOWING FILES ON PLUGINS/IOS : AdColony.framework, GADMAdapterAdColonyExtras.h, GADMAdapterAdColonyInitializer.h, libAdapterSDKAdColony
* AppLovin Adaptors & SDK 3.4.3
* Vungle Adapter & SDK 4.0.5
* Chartboost Adapter 1.1.0 & SDK 6.5.1
* UnityAds Adapter 1.0.2 & SDK 2.0.4 FROM https://github.com/Applifier/unity-ads-quickstart-ios
* Adapter Facebook 1.4.0 & Facebook Audience Network 4.15.1

### ANDROID ###

* AdColony SDK https://adcolony-www-common.s3.amazonaws.com/pub-adapter/android/adcolony.jar
* AppLovin SDK 6.3.2
* Vungle SDK 4.0.2
* Chartboost SDK 6.5.1
* UnityAds 2.0.2
* Facebook Adapter & SDK 4.15.0