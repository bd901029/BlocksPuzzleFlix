using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// This class is defined only for creating extention methods.
/// </summary>
public static class Extentions 
{
	/// <summary>
	/// Tries the parse int.
	/// </summary>
	/// <returns>The parse int.</returns>
	/// <param name="s">S.</param>
	/// <param name="defaultResult">Default result.</param>
	public static int TryParseInt(this string s, int defaultResult = 0)
	{
		int result = defaultResult;
		int.TryParse (s, out result);
		return result;
	}

	/// <summary>
	/// Shuffle the specified list.
	/// </summary>
	/// <param name="list">List.</param>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	public static void Shuffle<T>(this List<T> list)  
	{  
		System.Random rng = new System.Random();  
		int n = list.Count;  
		while (n > 1) {  
			n--;  
			int k = rng.Next(n + 1);  
			T value = list[k];  
			list[k] = list[n];  
			list[n] = value;  
		}  
	}
}
