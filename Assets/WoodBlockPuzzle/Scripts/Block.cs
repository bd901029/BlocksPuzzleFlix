using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if HBDOTween
using DG.Tweening;
#endif

public class Block : MonoBehaviour 
{
	//Row Index of block.
	public int rowID;

	//Column Index of block.
	public int columnID;

	//Status whether block is empty or filled.
	[HideInInspector] public bool isFilled = false;

	//Block image instance.
	[HideInInspector] public Image blockImage;
	public int blockID = -1;

	//Bomb blast counter, will keep reducing with each move.
	[HideInInspector] public int bombCounter = 0;
	Text txtCounter;

	//Determines whether this block is normal or bomb.
	[HideInInspector] public bool isBomb = false;

   



	/// <summary>
	/// Raises the enable event.
	/// </summary>
	void OnEnable()
	{
		//Counter will be used on Blast and challenge mode only.
		if (GameController.gameMode == GameMode.BLAST || GameController.gameMode == GameMode.CHALLENGE) {
			txtCounter = transform.GetChild (0).GetChild (0).GetComponent<Text> ();
		}
	}

	/// <summary>
	/// Sets the highlight image.
	/// </summary>
	/// <param name="sprite">Sprite.</param>
	public void SetHighlightImage(Sprite sprite)
	{
		blockImage.sprite = sprite;
		blockImage.color = new Color (1, 1, 1, 0.5F);
	}

	/// <summary>
	/// Stops the highlighting.
	/// </summary>
	public void StopHighlighting()
	{
		blockImage.sprite = null;
		blockImage.color = new Color (1, 1, 1, 0);
	}

	/// <summary>
	/// Sets the block image.
	/// </summary>
	/// <param name="sprite">Sprite.</param>
	/// <param name="_blockID">Block I.</param>
	public void SetBlockImage(Sprite sprite, int _blockID)
	{
		blockImage.sprite = sprite;
		blockImage.color = new Color (1, 1, 1, 1);
		blockID = _blockID;
		isFilled = true;
	}

	/// <summary>
	/// Converts to filled block.
	/// </summary>
	/// <param name="_blockID">Block I.</param>
	public void ConvertToFilledBlock(int _blockID)
	{
		blockImage.sprite = BlockShapeSpawner.Instance.ActiveShapeBlockModule.ShapeBlocks.Find (o => o.BlockID == _blockID).shapeBlock.transform.GetChild (0).GetComponent<Image> ().sprite;
		blockImage.color = new Color (1, 1, 1, 1);
		blockID = _blockID;
		isFilled = true;
	}

	#region bomb mode specific
	/// <summary>
	/// Converts to bomb.
	/// </summary>
	/// <param name="counterValue">Counter value.</param>
	public void ConvertToBomb(int counterValue = 9){
		blockImage.sprite = GamePlay.Instance.BombSprite;
		blockImage.color = new Color (1, 1, 1, 1);
		isFilled = true;
		isBomb = true;
		SetCounter (counterValue);
	}

	/// <summary>
	/// Sets the counter.
	/// </summary>
	/// <param name="counterValue">Counter value.</param>
	public void SetCounter(int counterValue = 9)
	{
		txtCounter.gameObject.SetActive (true);
		txtCounter.text = counterValue.ToString ();
		bombCounter = counterValue;
	}

	/// <summary>
	/// Decreases the counter.
	/// </summary>
	public void DecreaseCounter()
	{
		bombCounter -= 1;
		txtCounter.text = bombCounter.ToString ();

		if (bombCounter == 0) {
			GamePlay.Instance.OnBombCounterOver ();
		}
	}

	/// <summary>
	/// Removes the counter.
	/// </summary>
	void RemoveCounter()
	{
		txtCounter.text = "";
		txtCounter.gameObject.SetActive (false);
		bombCounter = 0;
		isBomb = false;
	}
	#endregion

	/// <summary>
	/// Clears the block.
	/// </summary>
	public void ClearBlock()
	{
       

		transform.GetComponent<Image> ().color = new Color (1, 1, 1, 0);
		
		blockImage.transform.DOScale (Vector3.zero, 0.35F).OnComplete (() => 
		{
			blockImage.transform.localScale = Vector3.one;
			blockImage.sprite = null;
		});

		transform.GetComponent<Image> ().DOFade (1, 0.35F).SetDelay (0.3F);
		blockImage.DOFade(0,0.3F);

		blockID = -1;

		isFilled = false;

		if (GameController.gameMode == GameMode.BLAST || GameController.gameMode == GameMode.CHALLENGE) {
			RemoveCounter ();
		}
		
	}

}
