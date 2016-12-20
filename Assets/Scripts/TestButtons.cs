using UnityEngine;
using System.Collections;

public class TestButtons : MonoBehaviour {

	void OnEnable()
	{
		RshkAds.OnRewardedCompleted += UserHasBeenRewarded;
	}

	void OnDisable()
	{
		RshkAds.OnRewardedCompleted -= UserHasBeenRewarded;
	}

	public void UserHasBeenRewarded()
	{
		Debug.Log ("User has been rewarded!");
	}

	public void ClickInterstitial()
	{
		RshkAds.ShowInterstitial ();
	}

	public void ClickRewarded()
	{
		RshkAds.ShowRewarded ();
	}

	public void ClickLoadRewarded()
	{
		RshkAds.RequestRewarded ();
	}

	public void ClickLoadInterstitial()
	{
		RshkAds.RequestInterstitial ();
	}

	public void ClickShowBanner()
	{
		RshkAds.ShowBanner ();
	}

	public void ClickHideBanner()
	{
		RshkAds.HideBanner ();
	}

	public void ClickDestroyBanner()
	{
		RshkAds.DestroyBanner ();
	}
}
