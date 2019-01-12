using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameMode
{
	CLASSIC = 1,
	TIMED = 2,
	BLAST = 3,
	ADVANCE = 4,
	CHALLENGE = 5
}

public class GameInfo : Singleton<GameInfo> 
{
	public List<GameModesInfo> gameModesInfo;

	[SerializeField]
	private string ReviewURL_Google;

	[SerializeField]
	private string ReviewURL_iOS;

	public static string ReviewURL;

	public string AboutURL;

	void Start()
	{
		#if UNITY_ANDROID
		ReviewURL = ReviewURL_Google;
		#elif UNITY_IOS
		ReviewURL = ReviewURL_iOS;
		#endif
	}
}

[System.Serializable]
public class GameModesInfo
{
	public string modeName;
	public GameMode gameMode;
	public int modeIndex;
	public bool isActive;
}
