using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectLanguage : MonoBehaviour 
{
	[SerializeField] 
	private GameObject LangaugeButtonTemplate;

	[SerializeField] 
	private Transform langaugeSelectionContent;


	void Start()
	{
		CreateLanguageList ();
	}

	void CreateLanguageList()
	{
		foreach (Langauge lang in LocalizationManager.Instance.LanguageList) {
			if (lang.isAvailable) {
				CreateLanguageButton (lang);
			}
		}
	}

	void CreateLanguageButton(Langauge languauge)
	{
		GameObject languageButton = (GameObject)Instantiate (LangaugeButtonTemplate);
		languageButton.name = "btn-" + languauge.LanguageName;
		languageButton.GetComponent<LanguageButton> ().SetLangugaeDetail (languauge);
		languageButton.transform.SetParent (langaugeSelectionContent);
		languageButton.transform.localScale = Vector3.one;
		languageButton.SetActive (true);
	}

	public void OnCloseButtonPressed()
	{
		if (InputManager.Instance.canInput ()) {
			AudioManager.Instance.PlayButtonClickSound ();
			StackManager.Instance.OnCloseButtonPressed ();
		}
	}
}
