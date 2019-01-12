using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System;
using System.Linq;

[InitializeOnLoad]
public class DependencyChecker : EditorWindow
{
	public const bool dotweenRequired = true;
	public const bool IAPRequired = true;
	public static string HBDOTween = "HBDOTween";
	public static string HBIAP = "HBIAP";

	public static bool isDoTweenInstalled = false;
	public static bool isUnityIAPInstalled = false;

	public static bool isIAPCatalogSetupDone = false;

	static DependencyChecker()
	{
		EditorApplication.update += RunOnce;
	}

	static void RunOnce() 
	{
		EditorApplication.update -= RunOnce;
		RunHBDOTweenCheckerOnLaunch();
	}

	public static void RunHBDOTweenCheckerOnLaunch()
	{
		if(dotweenRequired || IAPRequired)
		{
			EditorApplication.update += CheckItNow;
		}
	}

	public static void CheckItNow()
	{
		if (Directory.Exists ("Assets/Demigiant"))
		{
			if(!isDoTweenInstalled)
			{
				AddSymbolForPlatform (BuildTargetGroup.Standalone,HBDOTween);
				AddSymbolForPlatform (BuildTargetGroup.Android, HBDOTween);
				AddSymbolForPlatform (BuildTargetGroup.iOS, HBDOTween); 
				AddSymbolForPlatform (BuildTargetGroup.WSA, HBDOTween);		
				AddSymbolForPlatform (BuildTargetGroup.WebGL, HBDOTween);
				isDoTweenInstalled = true;
			}
		}
		
		if(Directory.Exists ("Assets/Plugins/UnityPurchasing"))
		{
			if(!isUnityIAPInstalled)
			{
				AddSymbolForPlatform (BuildTargetGroup.Standalone,HBIAP);
				AddSymbolForPlatform (BuildTargetGroup.Android, HBIAP);
				AddSymbolForPlatform (BuildTargetGroup.iOS, HBIAP); 
				AddSymbolForPlatform (BuildTargetGroup.WSA, HBIAP);		
				AddSymbolForPlatform (BuildTargetGroup.WebGL, HBIAP);
				isUnityIAPInstalled = true;
			}
		}

		if (File.Exists ("Assets/Plugins/UnityPurchasing/Resources/IAPProductCatalog.json"))
		{
			isIAPCatalogSetupDone = true;
		}
			
		if(!isDoTweenInstalled || !isUnityIAPInstalled || !isIAPCatalogSetupDone)
		{
			OpenWelcomeWindow();
		}
		else
		{
			return;
		}

		EditorApplication.update -= CheckItNow;
	}

	public static void OpenWelcomeWindow()
	{
		GetWindow<DependencyChecker>(true);
	}
	
	static void AddSymbolForPlatform(BuildTargetGroup platform, string newSymbol)
	{
		string existingSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(platform);

		string sTemp = newSymbol;

		if(!existingSymbols.Contains(newSymbol))
		{
			existingSymbols = existingSymbols.Replace(newSymbol + ";","");
			existingSymbols = existingSymbols.Replace(newSymbol,"");  
			existingSymbols = newSymbol + ";" + existingSymbols;
			PlayerSettings.SetScriptingDefineSymbolsForGroup(platform,existingSymbols);
		}
	}

	static void RemoveSymbolForPlatform(BuildTargetGroup platform, string removingSymbol)
	{
		string existingSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(platform);
		
		List<string> allSymbols = existingSymbols.Split(';').ToList();

		if(allSymbols.Contains(removingSymbol))
		{
			allSymbols.Remove(removingSymbol);
		}

		existingSymbols = "";
		foreach(string symbol in allSymbols) {
			existingSymbols = symbol +";";
		}
		PlayerSettings.SetScriptingDefineSymbolsForGroup(platform,existingSymbols);
	}

	void OnEnable()
	{
		titleContent = new GUIContent("Please Fix Plugin Dependencies."); 
	}	

	public void OnGUI()
	{
		GUILayoutUtility.GetRect(300, 10);
		GUILayout.Space(1);
		GUILayout.BeginVertical();	
		GUIStyle myStyle = new GUIStyle();
  	  	myStyle.fontStyle = FontStyle.Bold;
	
		if(!isDoTweenInstalled)
		{
			GUILayout.Label(" DOTWEEN REQUIRED.",myStyle);
			GUILayout.Label("It's completely FREE and available on Unity assetstore.");
			if(GUILayout.Button("Get DOTWEEN(HOTween v2) It's Free!",GUILayout.Height(50)))
			{
				Application.OpenURL("http://u3d.as/aZ1");
			}
		}

		GUILayout.Space(20);
		if(!isUnityIAPInstalled)
		{
			GUILayout.Label(" UNITY PURCHASING REQUIRED.",myStyle);
			GUILayout.Label("It's completely FREE and available on Unity assetstore.");
			GUILayout.Label("You can also activate and import via Windows -> Services -> In-App Purchasing -> Import.");
			if(GUILayout.Button("Setup Unity IAP",GUILayout.Height(50)))
			{
				Application.OpenURL("http://u3d.as/xM1");
			}
		}

		if(isDoTweenInstalled && isUnityIAPInstalled)
		{
			GUILayout.Label(" CONGRATULATIONS.",myStyle);
			GUILayout.Label("Your plugin dependencies seems correct.");

			if (!File.Exists ("Assets/Plugins/UnityPurchasing/Resources/IAPProductCatalog.json"))
			{
				GUILayout.Space(20);
				GUILayout.Label(" JUST ONE LAST STEP REMAINING.",myStyle);
				GUILayout.Label("Still seems like IAP Catalog is not setup, we have already kept is ready \nSimply goto Block Puzzle Magic -> Plugin Setup -> Setup IAP Catalog");
				
				if(GUILayout.Button("Setup IAP Catalog Now!",GUILayout.Height(50)))
				{
					string sourcePath = Application.dataPath +"/Block-Puzzle-Magic/UnityIAPCatalog/IAPProductCatalog.json";
					string destPath = Application.dataPath +"/Plugins/UnityPurchasing/Resources/IAPProductCatalog.json";
					
					if(File.Exists(sourcePath))
					{
						if(!File.Exists(destPath))
						{		
							File.Copy(sourcePath, destPath);
							AssetDatabase.Refresh();
						}
						else
						{
							Debug.LogWarning("IAP Catalog already exists.");
						}
					}
				}

				GUILayout.Label("Altenatively you can setup from Windows -> Unity IAP -> IAP Catalog");
			}
		}
	}
}
         