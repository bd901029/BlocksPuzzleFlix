using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class LanguageButton : MonoBehaviour 
{
	private Button btnLanguage;
	[HideInInspector]
	public string LanguageCode;

	[SerializeField]
	private Image checkMarkImage;

	[SerializeField]
	private Image FlagImage;

	public Text txtLanguageName;

	void Awake()
	{
		btnLanguage = GetComponent<Button> ();
	}

	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start()
	{
		if (LanguageCode == LocalizationManager.Instance.getCurrentLanguageCode()) {
			btnLanguage.transform.SetAsFirstSibling ();
			checkMarkImage.gameObject.SetActive (true);
		} else {
			checkMarkImage.gameObject.SetActive (false);
		}
	}

	/// <summary>
	/// Raises the enable event.
	/// </summary>
	void OnEnable()
	{
		btnLanguage.onClick.AddListener(()=>{
			if(InputManager.Instance.canInput())
			{
				AudioManager.Instance.PlayButtonClickSound();
				LocalizationManager.Instance.SetLanguage(LanguageCode);
			}
		});

		LocalizationManager.OnLanguageChangedEvent += OnLanguageChangedEvent;
	}

	/// <summary>
	/// Raises the disable event.
	/// </summary>
	void OnDisable()
	{
		LocalizationManager.OnLanguageChangedEvent -= OnLanguageChangedEvent;
	}

	/// <summary>
	/// Raises the language changed event event.
	/// </summary>
	/// <param name="langCode">Lang code.</param>
	void OnLanguageChangedEvent (string langCode)
	{
		if (langCode == LanguageCode) {
			btnLanguage.transform.SetAsFirstSibling ();
			checkMarkImage.gameObject.SetActive (true);
		} else {
			checkMarkImage.gameObject.SetActive (false);
		}
	}

	/// <summary>
	/// Sets the langugae detail.
	/// </summary>
	/// <param name="lang">Lang.</param>
	public void SetLangugaeDetail(Langauge lang)
	{
		FlagImage.sprite = lang.imgFlag;
		LanguageCode = lang.LangaugeCode;
		txtLanguageName.text = lang.LanguageName;
	}
}
