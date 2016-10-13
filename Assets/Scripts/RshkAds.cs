/*
Author: Victor Corvalan @pescadon
pescode.wordpress.com

Roshka Studios
roshkastudios.com
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GoogleMobileAds.Api;
using System;

public class RshkAds : MonoBehaviour {
	static RshkAds instance;

	static int InterstitialRequestsTry = 0;
	static int RewardedRequestsTry = 0;

	static int InterstitialMinNextAdShow = 0;
	static int InterstitialMaxNextAdShow = 4;
	static int InterstitialNextAdShow = 1;
	static int InterstitialAdCount = 0;

	public static bool HasWatchedRewardedAds = false;
	public delegate void actionRewardedCompleted();
	public static event actionRewardedCompleted OnRewardedCompleted;
	static string adInterstitialUnitId = "";
	static string adRewardedUnitId = "";
	static InterstitialAd interstitial;
	static RewardBasedVideoAd rewardBasedVideo;
	static bool FirstTimeRewardedListeners = true;

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
			adInterstitialUnitId = "YOUR-ANDROID-INTERSTITIAL-AD-UNIT-ID-GOES-HERE";
			adRewardedUnitId = "YOUR-ANDROID-REWARDED-AD-UNIT-ID-GOES-HERE";
			#elif UNITY_IPHONE
			adInterstitialUnitId = "YOUR-IOS-INTERSTITIAL-AD-UNIT-ID-GOES-HERE";
			adRewardedUnitId = "YOUR-IOS-REWARDED-AD-UNIT-ID-GOES-HERE";
			#else
			adUnitId = "unexpected_platform";
			#endif

			//RequestInterstitial ();
			//RequestRewarded ();

		}
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
		AdRequest request = new AdRequest.Builder().Build();
		interstitial.LoadAd(request);
		interstitial.OnAdOpening += InterstitialAdOpening;
		interstitial.OnAdClosed += InterstitialAdClose;
		interstitial.OnAdFailedToLoad += Interstitial_OnAdFailedToLoad;
		interstitial.OnAdLoaded += Interstitial_OnAdLoaded;
	}

	public static void ShowInterstitial()
	{
		Debug.Log ("**********************\n**********************\nSHOW INTERSTITIAL! \n " + (InterstitialAdCount+1) + "=" + InterstitialNextAdShow);
		//if (!IAP.IsAdsRemoved ()) {
			InterstitialAdCount++;
			PlayerPrefs.SetInt ("InterstitialAdCount", InterstitialAdCount);
			if (interstitial.IsLoaded ()) {
				if (InterstitialAdCount >= InterstitialNextAdShow) {
					//if (!HasWatchedRewardedAds) {
						InterstitialAdCount = 0;
						InterstitialNextAdShow = UnityEngine.Random.Range (InterstitialMinNextAdShow, InterstitialMaxNextAdShow);
						PlayerPrefs.SetInt ("InterstitialNextAdShow", InterstitialNextAdShow);
						interstitial.Show ();
					//}
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
		Time.timeScale = 0;
	}

	static void InterstitialAdClose(object sender, EventArgs args)
	{
		Debug.Log ("**********************\n**********************\nInterstitial close");
		AudioListener.pause = false;
		Time.timeScale = 1;
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

	public static void ShowRewarded()
	{
		Debug.Log ("**********************\n**********************\nSHOW REWARDED! \n ");
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
		
}
