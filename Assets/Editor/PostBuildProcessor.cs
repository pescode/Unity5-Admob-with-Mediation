#if UNITY_5
/*
Author: Victor Corvalan @pescadon
pescode.wordpress.com

Roshka Studios
roshkastudios.com
*/

using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Collections;
using UnityEditor.iOS.Xcode;
using System.IO;

public class PostBuildProcessor : MonoBehaviour
{
	#if UNITY_CLOUD_BUILD
	public static void OnPostprocessBuildiOS (string exportPath)
	{
		Debug.Log("OnPostprocessBuildiOS");
		ProcessPostBuild(BuildTarget.iPhone,exportPath);
	}
	#endif
	
	[PostProcessBuild]
	public static void OnPostprocessBuild (BuildTarget buildTarget, string path)
	{
		//if (buildTarget != BuildTarget.iPhone) { // For Unity < 5
		if (buildTarget != BuildTarget.iOS) {
			Debug.LogWarning("Target is not iOS. AdColonyPostProcess will not run");
			return;
   	 	}

		#if !UNITY_CLOUD_BUILD
    	Debug.Log ("OnPostprocessBuild");
    	ProcessPostBuild (buildTarget, path);
		ChangeXcodePlist (buildTarget, path);
		#endif
	}

	private static void ProcessPostBuild (BuildTarget buildTarget, string path)
	{
	    string projPath = path + "/Unity-iPhone.xcodeproj/project.pbxproj";

	    PBXProject proj = new PBXProject();
	    proj.ReadFromString(File.ReadAllText(projPath));

	    string target = proj.TargetGuidByName("Unity-iPhone");
		proj.AddBuildProperty(target, "OTHER_LDFLAGS", "-ObjC");
		proj.AddBuildProperty(target, "OTHER_LDFLAGS", "-fobjc-arc");
	    //
	    //Required Frameworks
	    
		proj.AddFrameworkToProject(target, "AudioToolbox.framework", false);
	    proj.AddFrameworkToProject(target, "AVFoundation.framework", false);
	    proj.AddFrameworkToProject(target, "CoreGraphics.framework", false);
	    proj.AddFrameworkToProject(target, "CoreTelephony.framework", false);
	    proj.AddFrameworkToProject(target, "CoreMedia.framework", false);
	    proj.AddFrameworkToProject(target, "EventKit.framework", false);
	    proj.AddFrameworkToProject(target, "EventKitUI.framework", false);
	    proj.AddFrameworkToProject(target, "MediaPlayer.framework", false);
	    proj.AddFrameworkToProject(target, "MessageUI.framework", false);
	    proj.AddFrameworkToProject(target, "QuartzCore.framework", false);
	    proj.AddFrameworkToProject(target, "SystemConfiguration.framework", false);
		proj.AddFrameworkToProject(target, "CoreFoundation.framework", false);

	    proj.AddFileToBuild(target, proj.AddFile("usr/lib/libz.1.2.5.dylib", "Frameworks/libz.1.2.5.dylib", PBXSourceTree.Sdk));

	    //Optional Frameworks
	    proj.AddFrameworkToProject(target, "AdSupport.framework", true);
	    proj.AddFrameworkToProject(target, "Social.framework", true);
	    proj.AddFrameworkToProject(target, "StoreKit.framework", true);
	    proj.AddFrameworkToProject(target, "Webkit.framework", true);

	    File.WriteAllText(projPath, proj.WriteToString());
	}

	public static void ChangeXcodePlist(BuildTarget buildTarget, string pathToBuiltProject) {

		if (buildTarget == BuildTarget.iOS) {

			// Get plist
			string plistPath = pathToBuiltProject + "/Info.plist";
			PlistDocument plist = new PlistDocument();
			plist.ReadFromString(File.ReadAllText(plistPath));

			// Get root
			PlistElementDict rootDict = plist.root;

			//
			PlistElementArray queriesSchemes = rootDict.CreateArray("LSApplicationQueriesSchemes");
			queriesSchemes.AddString("fb");
			queriesSchemes.AddString("instagram");
			queriesSchemes.AddString("tumblr");
			queriesSchemes.AddString("twitter");

			rootDict.CreateDict ("NSCalendarsUsageDescription");
			rootDict.SetString ("NSCalendarsUsageDescription","Adding events");

			rootDict.CreateDict ("NSPhotoLibraryUsageDescription");
			rootDict.SetString ("NSPhotoLibraryUsageDescription","Taking selfies");

			rootDict.CreateDict ("NSCameraUsageDescription");
			rootDict.SetString ("NSCameraUsageDescription","Taking selfies");

			rootDict.CreateDict ("NSMotionUsageDescription");
			rootDict.SetString ("NSMotionUsageDescription","Interactive ad controls");

			rootDict.CreateDict ("NSBluetoothPeripheralUsageDescription");
			rootDict.SetString ("NSBluetoothPeripheralUsageDescription","Advertisement would like to use bluetooth.");

			rootDict.CreateDict ("AppLovinSdkKey");
			rootDict.SetString ("AppLovinSdkKey","YOUR-APPLOVIN-SDK-KEY-GOES-HERE");

			// Write to file
			File.WriteAllText(plistPath, plist.WriteToString());
		}
	}

}
#endif
