using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if HBDOTween
using DG.Tweening;
#endif


/// <summary>
/// Help classic.
/// </summary>
public class HelpClassic : MonoBehaviour
{
	[SerializeField]
	private Transform handImage;

	Vector2 firstPosition = Vector2.zero;
	Vector2 secondPosition = Vector2.zero;

	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start ()
	{
		Invoke ("StartHelp", 1F);
	}

	/// <summary>
	/// Starts the help.
	/// </summary>
	void StartHelp ()
	{
		GameObject firstShape = BlockShapeSpawner.Instance.transform.GetChild (0).gameObject;

		firstPosition = firstShape.transform.position;
		firstPosition -= new Vector2 (-0.2F, 0.4F);
		handImage.gameObject.SetActive (true);
		handImage.transform.position = firstPosition;
		secondPosition = GamePlay.Instance.transform.Find ("Game-Content").position;

		if (firstShape.transform.childCount > 0) {
			firstShape.transform.GetChild (0).GetComponent<Canvas> ().sortingOrder = 3;
		}
		#if HBDOTween
		transform.GetComponent<CanvasGroup> ().DOFade (1F, 0.5F).OnComplete (() => {
			AnimateInLoop ();
		});
		#endif
	}

	/// <summary>
	/// Animates the in loop.
	/// </summary>
	void AnimateInLoop ()
	{
		#if HBDOTween
		handImage.transform.position = firstPosition;
		handImage.transform.DOMove (secondPosition, 1F).SetDelay (1).OnComplete (() => {
			handImage.transform.DOMove (firstPosition, 0.5F).SetDelay (1).OnComplete (AnimateInLoop);
		});
		#endif
	}
}
