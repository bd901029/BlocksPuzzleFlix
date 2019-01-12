using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;

public class EditorUtility : MonoBehaviour {

	[MenuItem("Block Magic Puzzle/Plugin Setup/Check Setup",false,1)]
	private static void SetUp()
	{
		DependencyChecker.OpenWelcomeWindow();
	}

	[MenuItem("Block Magic Puzzle/Plugin Setup/Setup IAP Catalog",false,2)]
	private static void SetUpIAPCatalog()
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

	[MenuItem("Block Magic Puzzle/Clear PlayerPrefs")]
	private static void ClearPrefs()
	{
		PlayerPrefs.DeleteAll();
	}

	[MenuItem("Block Magic Puzzle/Persistent Data Path/Remove All Files")]
	private static void DeleteAllFilesFromPersistentDataPath()
	{
		FileUtil.DeleteFileOrDirectory (Application.persistentDataPath);
	}

	[MenuItem("Block Magic Puzzle/Capture Screenshot/1X")]
	private static void Capture1XScreenshot()
	{
		string imgName = "IMG-"+ DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString("00") + DateTime.Now.Day.ToString("00") + "-" + DateTime.Now.Hour.ToString("00") + DateTime.Now.Minute.ToString("00") + DateTime.Now.Second.ToString("00") +".png";
		//ScreenCapture.CaptureScreenshot ((Application.dataPath + "/" + imgName),1);
	}

	[MenuItem("Block Magic Puzzle/Capture Screenshot/2X")]
	private static void Capture2XScreenshot()
	{
		string imgName = "IMG-"+ DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString("00") + DateTime.Now.Day.ToString("00") + "-" + DateTime.Now.Hour.ToString("00") + DateTime.Now.Minute.ToString("00") + DateTime.Now.Second.ToString("00") +".png";
		//ScreenCapture.CaptureScreenshot ((Application.dataPath + "/" + imgName),2);
	}

}
