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
	static int InterstitialNextAdShow = 1;
	static int InterstitialAdCount = 0;
	public static bool HasWatchedRewardedAds = false;
	public delegate void actionRewardedCompleted();
	public static event actionRewardedCompleted OnRewardedCompleted;
	static string adUnitId = "";
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
			adUnitId = "YOUR-ANDROID-INTERSTITIAL-AD-UNIT-ID-GOES-HERE";
			adRewardedUnitId = "YOUR-IOS-INTERSTITIAL-AD-UNIT-ID-GOES-HERE";
			#elif UNITY_IPHONE
			adUnitId = "YOUR-ANDROID-REWARDED-AD-UNIT-ID-GOES-HERE";
			adRewardedUnitId = "YOUR-IOS-REWARDED-AD-UNIT-ID-GOES-HERE";
			#else
			adUnitId = "unexpected_platform";
			#endif

			RequestInterstitial ();
			RequestRewarded ();

		}
	}

	static void RewardBasedVideo_OnAdLoaded (object sender, EventArgs e)
	{
		Debug.Log ("**********************\n**********************\n REWARDED LOADED! \n " + e.ToString());
	}

	static void RewardBasedVideo_OnAdFailedToLoad (object sender, AdFailedToLoadEventArgs e)
	{
		Debug.Log ("**********************\n**********************\nFailed to load rewarded ad " + e.Message);
		RequestRewarded ();
	}

	public static void RequestInterstitial(){
		Debug.Log ("**********************\n**********************\nREQUESTING INTERSTITIAL");
		interstitial = new InterstitialAd(adUnitId);
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
					if (!HasWatchedRewardedAds) {
						InterstitialAdCount = 0;
						InterstitialNextAdShow = UnityEngine.Random.Range (3, 6);
						PlayerPrefs.SetInt ("InterstitialNextAdShow", InterstitialNextAdShow);
						interstitial.Show ();
					}
				}
			} else {
				RequestInterstitial ();
			}
		//}
	}

	static void Interstitial_OnAdLoaded (object sender, EventArgs e)
	{
		Debug.Log ("**********************\n**********************\nINTERSTITIAL LOADED! \n " + e.ToString());
	}

	static void Interstitial_OnAdFailedToLoad (object sender, AdFailedToLoadEventArgs e)
	{
		Debug.Log ("**********************\n**********************\nFailed to load interstitial ad " + e.Message);
		RequestInterstitial ();
	}

	static void InterstitialAdOpening(object sender, EventArgs args)
	{
		Debug.Log ("**********************\n**********************\nInterstitial opening");
		AudioListener.pause = true;
	}

	static void InterstitialAdClose(object sender, EventArgs args)
	{
		Debug.Log ("**********************\n**********************\nInterstitial close");
		AudioListener.pause = false;
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
			FirstTimeRewardedListeners = false;
		}

		AdRequest request = new AdRequest.Builder().Build();
		rewardBasedVideo.LoadAd(request, adRewardedUnitId);

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
