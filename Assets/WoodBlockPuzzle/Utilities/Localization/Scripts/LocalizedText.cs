using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent (typeof(Text))]
public class LocalizedText : MonoBehaviour
{
	public string txtTag;
	Text thisText;

	void Awake ()
	{
		thisText = GetComponent<Text> ();
	}

	void OnEnable ()
	{
		LocalizationManager.OnLanguageChangedEvent += OnLanguageChangedEvent;
		Invoke ("SetLocalizedText", 0.01F);
	}

	void OnDisable ()
	{
		LocalizationManager.OnLanguageChangedEvent -= OnLanguageChangedEvent;
	}

	public void SetLocalizedText ()
	{
		if (!string.IsNullOrEmpty (txtTag.Trim ())) {
			thisText.SetLocalizedTextForTag (txtTag);
		}
	}

	void OnLanguageChangedEvent (string langCode)
	{
		SetLocalizedText ();
	}
}