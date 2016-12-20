# Unity5-Admob-with-Mediation
Google Mobile Ads for iOS & Android with mediation.
Interstitial & Rewarded Ads

Show Admob ads with mediation (interstitial &amp; rewarded) from different providers like AdColony, AppLovin, Chartboost, Facebook Audience Network, UnityADS &amp; Vungle

### Current version 1.0.6 ###
Updated to Google Mobile Ads 3.1.3 and SDK+Adapter mediation libs. Improvements and bug fixes.

### Version 1.0.5 ###
I've found that on Android we need to wait a little time before doing something in the game when the Ad close. I've added a coroutine in RshkAds.cs to avoid problem with this.

### Version 1.0.4 ###
Special notes: Google Mobile Ads 3.1.0 now includes a script called AdMobDependencies (Assets/PlayServicesResolver/Editor). I've commented the lines 67 - 74 because I wasn't able to prepare a build on iOS, I always receive an error with POD.

### HOW TO USE ###

* Download the lastest admobWithMediation.unitypackage from here: https://goo.gl/HnYYOQ. 
Import this package in your project.
* The project contains the adaptors, sdk, manifest and xcode setup ready to use admob with AdColony, AppLovin, Chartboost, Facebook Audience Network, UnityADS and Vungle. You can write/use your own code to display ads or you can optionally use RshkAds.cs .
* In case that you want to use RshkAds.cs, set your interstitial Ad Unit ID and rewarded Ad Unit ID for iOS and Android in RshkAds.cs (Line 48-52).
* Drag and drop RshkAds prefab in you first scene (when the game launch, in your loading screen or in the first scene so it can start to fetch interstitials and rewarded ads ASAP).
* To setup your APPLOVIN SDK KEY for Android OPEN Plugins/Android/RshkAds/AndroidManifest.xml . Add your SDK on line 39.
* To setup your APPLOVIN SDK KEY for iOS OPEN Editor/PostBuildProcess.cs. Add your SDK on line 130.
* If your want to use Facebook Audience Network setup your Facebook App ID and open Plugins/Android/RshkAds/AndroidManifest.xml. Add your ID on line 52 ex: fb123456789
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

* GoogleMobileAds (Unity) v3.1.3 https://github.com/googleads/googleads-mobile-unity/releases .

### iOS ###

* Google-Mobile-Ads-SDK v7.16.0
https://firebase.google.com/docs/admob/ios/download
* AdColony Adapter 1.6 
http://support.adcolony.com/customer/en/portal/articles/2080486-certified-mediation-partners-documentation-page?b_id=13354
* AdColony SDK 2.6.3 
https://github.com/AdColony/AdColony-iOS-SDK
* AppLovin Adaptors 
https://github.com/AppLovin/SDK-Network-Adaptors/tree/master/AdMob/iOS
* AppLovin SDK 3.5.1 
https://www.applovin.com/integration#iosAdMobIntegration
* Chartboost Adapter 6.5.2.1 
https://dl.google.com/googleadmobadssdk/libadapterchartboost.zip
* Chartboost SDK 6.6.0 
https://answers.chartboost.com/hc/en-us/articles/201220095
* Facebook Audience Network 4.18.0 
https://developers.facebook.com/docs/ios
* Adapter Facebook 1.4.0 
https://firebase.google.com/docs/admob/ios/mediation-networks
* Vungle Adapter 1.3.1 
https://support.vungle.com/hc/en-us/articles/208073977
* Vungle SDK 4.0.9 
https://v.vungle.com/sdk
* UnityAds SDK 2.0.7
https://github.com/Unity-Technologies/unity-ads-ios/releases
* UnityAds Adapter 2.0.4.0 
https://firebase.google.com/docs/admob/android/mediation-networks

### ANDROID ###

* AdColony SDK 
https://adcolony-www-common.s3.amazonaws.com/pub-adapter/android/adcolony.jar
* AdColony Adapter (Included in SDK)
* AppLovin SDK 6.4.0 
https://www.applovin.com/integration#androidIntegration
* AppLovin Adapter 
https://github.com/AppLovin/SDK-Network-Adaptors/tree/master/AdMob/Android
* Chartboost SDK 6.6.1 
https://answers.chartboost.com/hc/en-us/articles/201219545
* Chartboost Adapter 
https://answers.chartboost.com/hc/en-us/articles/209756523-Mediation-AdMob
* Facebook Adapter 4.17.0 
https://firebase.google.com/docs/admob/ios/mediation-networks
* Facebook SDK 4.17.0 
https://developers.facebook.com/docs/android/downloads/
* UnityAds 2.0.7
https://github.com/Unity-Technologies/unity-ads-ios/releases
* UnityAds Adapter
https://firebase.google.com/docs/admob/android/mediation-networks
* Vungle SDK 4.0.3 
https://v.vungle.com/sdk
* Vungle Adapter 2.1.0 
https://support.vungle.com/hc/en-us/articles/207604108

### NOTE ###
If you receive something like "The imported type `Google.JarResolver.PlayServicesSupport' is defined multiple times" just delete JarResolverLib
