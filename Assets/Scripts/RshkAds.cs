/*
Author: Victor Corvalan @pescadon
pescode.wordpress.com

Roshka Studios
roshkastudios.com
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GoogleMobileAds;
using GoogleMobileAds.Api;
using System;
using UnityEngine.Analytics;

public class RshkAds : MonoBehaviour {
	static RshkAds instance;

	static int InterstitialRequestsTry = 0;
	static int RewardedRequestsTry = 0;
	static int BannerRequestsTry = 0;

	static int InterstitialMinNextAdShow = 0;
	static int InterstitialMaxNextAdShow = 2;
	static int InterstitialNextAdShow = 1;
	static int InterstitialAdCount = 0;

	public static bool HasWatchedRewardedAds = false;
	public delegate void actionRewardedCompleted();
	public static event actionRewardedCompleted OnRewardedCompleted;

	static string adInterstitialUnitId = "";
	static string adRewardedUnitId = "";
	static string adBannerUnitId = "";
	static InterstitialAd interstitial;
	static RewardBasedVideoAd rewardBasedVideo;
	static BannerView banner;
	static bool FirstTimeRewardedListeners = true;

	static bool isBannerLoaded = false;
	static bool isBannerShowing = false;
	// Use this for initialization
	void Start () {
		if (instance) {
			Destroy (gameObject);
		} else {
			Debug.Log ("**********************\n**********************\nINITIALIZING AD SYSTEM");
			instance = this;
			DontDestroyOnLoad (gameObject);
			InterstitialAdCount = PlayerPrefs.GetInt ("InterstitialAdCount", InterstitialAdCount);
			InterstitialNextAdShow = PlayerPrefs.GetInt ("InterstitialNextAdShow", InterstitialNextAdShow);
			#if UNITY_ANDROID
			adInterstitialUnitId = "YOUR-ANDROID-INTERSTITIAL-UNIT-ID";
			adRewardedUnitId = "YOUR-ANDROID-REWARDED-UNIT-ID";
			adBannerUnitId = "YOUR-ANDROID-BANNER-UNIT-ID";
			#elif UNITY_IPHONE
			adInterstitialUnitId = "YOUR-IOS-INTERSTITIAL-UNIT-ID";
			adRewardedUnitId = "YOUR-IOS-REWARDED-UNIT-ID";
			adBannerUnitId = "YOUR-IOS-BANNER-UNIT-ID";
			#endif

			RequestInterstitial ();
			RequestRewarded ();
		}
	}

	// Returns an ad request with custom ad targeting.
	static AdRequest CreateAdRequest()
	{
		return new AdRequest.Builder ().Build ();
		// For testing
		return new AdRequest.Builder()
			.AddTestDevice(AdRequest.TestDeviceSimulator)
			.AddTestDevice("C626D9E7427CCCCD80EAF225BD07D0B4")
			.AddKeyword("game")
			.SetGender(GoogleMobileAds.Api.Gender.Male)
			.SetBirthday(new DateTime(1985, 1, 1))
			.TagForChildDirectedTreatment(false)
			.AddExtra("color_bg", "9B30FF")
			.Build();
	}

	static void RewardBasedVideo_OnAdLoaded (object sender, EventArgs e)
	{
		RewardedRequestsTry = 0;
		Debug.Log ("**********************\n**********************\n REWARDED LOADED! \n " + e.ToString());
	}

	static void RewardBasedVideo_OnAdFailedToLoad (object sender, AdFailedToLoadEventArgs e)
	{
		Debug.Log ("**********************\n**********************\nFailed to load rewarded ad " + e.Message);
		RewardedRequestsTry++;
		if(RewardedRequestsTry < 3)
			RequestRewarded ();
	}

	public static void RequestInterstitial(){
		Debug.Log ("**********************\n**********************\nREQUESTING INTERSTITIAL");
		interstitial = new InterstitialAd(adInterstitialUnitId);
		interstitial.OnAdOpening += InterstitialAdOpening;
		interstitial.OnAdClosed += InterstitialAdClose;
		interstitial.OnAdFailedToLoad += Interstitial_OnAdFailedToLoad;
		interstitial.OnAdLoaded += Interstitial_OnAdLoaded;
		//AdRequest request = new AdRequest.Builder().Build();
		interstitial.LoadAd(CreateAdRequest());

	}

	public static void ShowInterstitial(string tag = "interstitial")
	{
		Debug.Log ("**********************\n**********************\nSHOW INTERSTITIAL! \n " + (InterstitialAdCount+1) + "=" + InterstitialNextAdShow);
		//if (!IAP.IsAdsRemoved ()) {
			InterstitialAdCount++;
			PlayerPrefs.SetInt ("InterstitialAdCount", InterstitialAdCount);
			if (interstitial.IsLoaded ()) {
				if (InterstitialAdCount >= InterstitialNextAdShow) {
					if (!HasWatchedRewardedAds) {
						InterstitialAdCount = 0;
						InterstitialNextAdShow = UnityEngine.Random.Range (InterstitialMinNextAdShow, InterstitialMaxNextAdShow);
						PlayerPrefs.SetInt ("InterstitialNextAdShow", InterstitialNextAdShow);
						interstitial.Show ();
						Analytics.CustomEvent ("ADS Interstitial", new Dictionary<string, object> {
							{ "Tag", tag }
						});
					}
				}
			} else {
				RequestInterstitial ();
			}
		//}
	}

	static void Interstitial_OnAdLoaded (object sender, EventArgs e)
	{
		InterstitialRequestsTry = 0;
		Debug.Log ("**********************\n**********************\nINTERSTITIAL LOADED! \n " + e.ToString());
	}

	static void Interstitial_OnAdFailedToLoad (object sender, AdFailedToLoadEventArgs e)
	{
		Debug.Log ("**********************\n**********************\nFailed to load interstitial ad " + e.Message);
		InterstitialRequestsTry++;
		if(InterstitialRequestsTry < 3)
			RequestInterstitial ();
	}

	static void InterstitialAdOpening(object sender, EventArgs args)
	{
		Debug.Log ("**********************\n**********************\nInterstitial opening");
		AudioListener.pause = true;
		Time.timeScale = 0.02f;		//optional to pause the game
	}

	static void InterstitialAdClose(object sender, EventArgs args)
	{
		Debug.Log ("**********************\n**********************\nInterstitial close");
		instance.StartCoroutine (InterstitialDone ());
	}

	static IEnumerator InterstitialDone()
	{
		yield return new WaitForSeconds (1f);
		AudioListener.pause = false;
		Time.timeScale = 1;			//optional to continue the game
		interstitial.Destroy ();
		RequestInterstitial ();
	}

	public static void RequestRewarded(){
		Debug.Log ("**********************\n**********************\nREQUESTING REWARDED");

		if (FirstTimeRewardedListeners) {
			rewardBasedVideo = RewardBasedVideoAd.Instance;
			rewardBasedVideo.OnAdFailedToLoad += RewardBasedVideo_OnAdFailedToLoad;
			rewardBasedVideo.OnAdOpening += RewardedAdOpening;
			rewardBasedVideo.OnAdClosed += RewardedAdClose;
			rewardBasedVideo.OnAdRewarded += RewardedAdCompleted;
			rewardBasedVideo.OnAdLoaded += RewardBasedVideo_OnAdLoaded;
			rewardBasedVideo.OnAdRewarded += RewardBasedVideo_OnAdRewarded;
			FirstTimeRewardedListeners = false;
		}

		AdRequest request = new AdRequest.Builder().Build();
		rewardBasedVideo.LoadAd(request, adRewardedUnitId);

	}

	static void RewardBasedVideo_OnAdRewarded (object sender, Reward e)
	{
		Debug.Log ("**********************\n**********************\nUSER HAS BEEN REWARDED! \n ");
		// NOTE: We don't give extra lifes or virtual coins here. We use RewardedAdClose to reward
		// the user even if he didn't watch the full video ad
	}

	public static bool IsRewardedAdsAvailable()
	{
		return rewardBasedVideo.IsLoaded ();
	}

	public static void ShowRewarded(string tag = "rewarded")
	{
		Debug.Log ("**********************\n**********************\nSHOW REWARDED! \n ");
		Analytics.CustomEvent("ADS Rewarded", new Dictionary<string, object>
			{
				{ "Tag", tag }
			});
		HasWatchedRewardedAds = true;
		InterstitialAdCount = 0;
		rewardBasedVideo.Show();
	}

	static void RewardedAdOpening(object sender, EventArgs args)
	{
		Debug.Log ("**********************\n**********************\nRewarded Ad Opening");
		AudioListener.pause = true;
	}
	static void RewardedAdClose(object sender, EventArgs args)
	{
		Debug.Log ("**********************\n**********************\nRewarded Ad Close");
		instance.StartCoroutine (RewardedDone ());
	}
	// We need this to avoid errors on Android
	static IEnumerator RewardedDone()
	{
		yield return new WaitForSeconds (1f);
		AudioListener.pause = false;
		OnRewardedCompleted();
		RequestRewarded ();
	}
	
	static void RewardedAdCompleted(object sender, Reward args)
	{
		string type = args.Type;
		double amount = args.Amount;
		Debug.Log ("**********************\n**********************\nUser rewarded with: " + amount.ToString() + " " + type);
	}

	public static void ShowBanner()
	{
		//if (!IAP.IsAdsRemoved ()) {
		if (!isBannerLoaded) {
			RequestBanner ();
		} else if(!isBannerShowing){
			banner.Show ();
			isBannerShowing = true;
		}
		//}
	}

	public static void HideBanner()
	{
		if (isBannerShowing) {
			banner.Hide ();
			isBannerShowing = false;
		}
	}

	public static void DestroyBanner()
	{
		if (isBannerLoaded) {
			banner.Destroy ();
			isBannerLoaded = false;
			isBannerShowing = false;
		}
	}

	public static void RequestBanner()
	{
		// Create a 320x50 banner at the top of the screen.
		banner = new BannerView(adBannerUnitId, AdSize.Banner, AdPosition.Bottom);
		banner.OnAdFailedToLoad += Banner_OnAdFailedToLoad;
		banner.OnAdLoaded += Banner_OnAdLoaded;
		// Load the banner with the request.
		banner.LoadAd(CreateAdRequest());
	}

	static void Banner_OnAdLoaded (object sender, EventArgs e)
	{
		BannerRequestsTry = 0;
		isBannerLoaded = true;
		isBannerShowing = true;
		Debug.Log ("**********************\n**********************\nBANNER LOADED! \n " + e.ToString());
	}

	static void Banner_OnAdFailedToLoad (object sender, AdFailedToLoadEventArgs e)
	{
		Debug.Log ("**********************\n**********************\nFailed to load banner ad " + e.Message);
		BannerRequestsTry++;
		banner.Destroy ();
		isBannerLoaded = false;
		isBannerShowing = false;
		if(BannerRequestsTry < 3)
			RequestBanner ();
	}

		
}
