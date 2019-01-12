using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class LocalizationExtention
{

	public static void SetTag(this Text txt, string localizedTag)
	{
		if (txt.GetComponent<LocalizedText> () != null) {
			LocalizedText localizedText = txt.GetComponent<LocalizedText> ();
			localizedText.txtTag = localizedTag;
			localizedText.SetLocalizedText ();
		}
	}

	public static void SetTag(this GameObject txt, string localizedTag)
	{
		if (txt.GetComponent<Text> () != null) {
			txt.GetComponent<Text> ().SetTag (localizedTag);
		}
	}

	public static void SetLocalizedTextForTag (this Text txt, string tag)
	{
		txt.text = LocalizationManager.Instance.GetLocalizedTextForTag (tag);
	}

	public static void SetLocalizedTextForTag (this GameObject txt, string tag)
	{
		if (txt.GetComponent<Text> () != null) {
			txt.GetComponent<Text> ().SetLocalizedTextForTag (tag);
		}
	}

	public static void SetLocalizedTextForTag (this Text txt, string tag, List<SpecialTagReplacer> specialTagsToReplace)
	{
		string localizedText = LocalizationManager.Instance.GetLocalizedTextForTag (tag);

		foreach (SpecialTagReplacer spTag in specialTagsToReplace) {
			localizedText = localizedText.Replace (spTag.specialTag, spTag.replaceValue);
		}

		txt.text = localizedText;
	}

	public static void SetLocalizedTextForTag (this GameObject txt, string tag, List<SpecialTagReplacer> specialTagsToReplace)
	{
		if (txt.GetComponent<Text> () != null) {
			txt.GetComponent<Text> ().SetLocalizedTextForTag (tag, specialTagsToReplace);
		}
	}

	public static void SetText (this Text txt , string thisText)
	{
		txt.text = thisText;
	}
}
