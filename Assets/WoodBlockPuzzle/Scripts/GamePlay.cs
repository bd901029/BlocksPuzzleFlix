using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Xml.Linq;

#if HBDOTween
using DG.Tweening;
#endif

// This script has main logic to run entire gameplay.
public class GamePlay : Singleton<GamePlay>,IPointerDownHandler,IPointerUpHandler, IBeginDragHandler,IDragHandler
{
	[HideInInspector]
	public List<Block> blockGrid;

	ShapeInfo currentShape = null;
	Transform hittingBlock = null;

	List<Block> highlightingBlocks;

	public AudioClip blockPlaceSound;
	public AudioClip blockSelectSound;

	// Line break sounds.
	[SerializeField] private AudioClip lineClear1;
	[SerializeField] private AudioClip lineClear2;
	[SerializeField] private AudioClip lineClear3;
	[SerializeField] private AudioClip lineClear4;

	public AudioClip blockNotPlacedSound;

	[Tooltip("Max no. of times rescue can be used in 1 game. -1 is infinite")]
	[SerializeField] private int MaxAllowedRescuePerGame = 0;

	[Tooltip("Max no. of times rescue can be used in 1 game using watch video. -1 is infinite")]
	[SerializeField] private int MaxAllowedVideoWatchRescue = 0;

	[HideInInspector]
	public int TotalFreeRescueDone = 0;

	[HideInInspector]
	public int TotalRescueDone = 0;

	[HideInInspector]
	public int MoveCount = 0;

	public Sprite BombSprite;
	public Timer timeSlider;

	public  bool isHelpOnScreen = false;

    public int currentID;

	void Start ()
	{
        Application.targetFrameRate = 60;
		//Generate board from GameBoardGenerator Script Component.
		GetComponent<GameBoardGenerator> ().GenerateBoard ();
		highlightingBlocks = new List<Block> ();

		#region time mode
		// Timer will start with TIME and CHALLENGE mode.
		if(GameController.gameMode == GameMode.TIMED || GameController.gameMode == GameMode.CHALLENGE)
		{
			timeSlider.gameObject.SetActive(true);
		}
		#endregion

		#region check for help
		Invoke("CheckForHelp",0.5F);
		#endregion
	}

	#region IBeginDragHandler implementation

	/// <summary>
	/// Raises the begin drag event.
	/// </summary>
	/// <param name="eventData">Event data.</param>
	public void OnBeginDrag (PointerEventData eventData)
	{
		if (currentShape != null) {
			Vector3 pos = Camera.main.ScreenToWorldPoint (eventData.position);
			pos.z = currentShape.transform.localPosition.z;
			currentShape.transform.localPosition = pos;
		}
	}

	#endregion

	#region IPointerDownHandler implementation
	/// <summary>
	/// Raises the pointer down event.
	/// </summary>
	/// <param name="eventData">Event data.</param>
	public void OnPointerDown (PointerEventData eventData)
	{
		if (eventData.pointerCurrentRaycast.gameObject != null) {
			Transform clickedObject = eventData.pointerCurrentRaycast.gameObject.transform;

			if (clickedObject.GetComponent<ShapeInfo>() != null) 
			{
				if (clickedObject.transform.childCount > 0) {
					currentShape = clickedObject.GetComponent<ShapeInfo>();
					Vector3 pos = Camera.main.ScreenToWorldPoint (eventData.position);
					currentShape.transform.localScale = Vector3.one;
					currentShape.transform.localPosition = new Vector3 (pos.x, pos.y, 0);
					AudioManager.Instance.PlaySound (blockSelectSound);

					if (isHelpOnScreen) {
						GetComponent<InGameHelp> ().StopHelp ();
					}
				}
			}
		}
	}
	#endregion

	#region IPointerUpHandler implementation
	/// <summary>
	/// Raises the pointer up event.
	/// </summary>
	/// <param name="eventData">Event data.</param>
	public void OnPointerUp (PointerEventData eventData)
	{
		if (currentShape != null) {

			if (highlightingBlocks.Count > 0) 
			{
				SetImageToPlacingBlocks ();
				Destroy (currentShape.gameObject);
				currentShape = null;
				MoveCount += 1;
				Invoke ("CheckBoardStatus", 0.1F);
			} else {
				#if HBDOTween
				currentShape.transform.DOLocalMove (Vector3.zero, 0.5F);
				currentShape.transform.DOScale (Vector3.one * 0.6F, 0.5F);
				#endif
				currentShape = null;
				AudioManager.Instance.PlaySound (blockNotPlacedSound);
			}
		}
	}
	#endregion

	#region IDragHandler implementation
	/// <summary>
	/// Raises the drag event.
	/// </summary>
	/// <param name="eventData">Event data.</param>
	public void OnDrag (PointerEventData eventData)
	{
		if (currentShape != null) {
			Vector3 pos = Camera.main.ScreenToWorldPoint (eventData.position);
			pos = new Vector3(pos.x, (pos.y + 1F),0F);

			currentShape.transform.position = pos;

			RaycastHit2D hit = Physics2D.Raycast (currentShape.GetComponent<ShapeInfo>().firstBlock.block.position, Vector2.zero, 1);

			if (hit.collider != null) 
			{
				if (hittingBlock == null || hit.collider.transform != hittingBlock) {
					hittingBlock = hit.collider.transform;
					CanPlaceShape (hit.collider.transform);
				}
			} else {
				StopHighlighting ();
			}
		}
	}
	#endregion

	/// <summary>
	/// Determines whether this instance can place shape the specified currentHittingBlock.
	/// </summary>
	public bool CanPlaceShape (Transform currentHittingBlock)
	{
		Block currentCell = currentHittingBlock.GetComponent<Block> ();

		int currentRowID = currentCell.rowID;
		int currentColumnID = currentCell.columnID;

		StopHighlighting ();

		bool canPlaceShape = true;
		foreach (ShapeBlock c in currentShape.ShapeBlocks) 
		{
			Block checkingCell = blockGrid.Find (o => o.rowID == currentRowID + (c.rowID + currentShape.startOffsetX) && o.columnID == currentColumnID + (c.columnID - currentShape.startOffsetY));

			if ((checkingCell == null) || (checkingCell != null && checkingCell.isFilled)) {
				canPlaceShape = false;
				highlightingBlocks.Clear ();
				break;
			} else {
				if (!highlightingBlocks.Contains (checkingCell)) {
					highlightingBlocks.Add (checkingCell);
				}
			}
		}

		if (canPlaceShape) {
			SetHighLightImage ();
		}

		return canPlaceShape;
	}

	/// <summary>
	/// Sets the high light image.
	/// </summary>
	void SetHighLightImage ()
	{
		foreach (Block c in highlightingBlocks) {
			c.SetHighlightImage (currentShape.blockImage);
		}
	}

	/// <summary>
	/// Stops the highlighting.
	/// </summary>
	void StopHighlighting ()
	{
		if (highlightingBlocks != null && highlightingBlocks.Count > 0) {
			foreach (Block c in highlightingBlocks) {
				c.StopHighlighting ();
			}
		}
		hittingBlock = null;
		highlightingBlocks.Clear ();
	}

	/// <summary>
	/// Sets the image to placing blocks.
	/// </summary>
	void SetImageToPlacingBlocks ()
	{
		if (highlightingBlocks != null && highlightingBlocks.Count > 0) 
		{
			foreach (Block c in highlightingBlocks) {
				c.SetBlockImage (currentShape.blockImage,currentShape.ShapeID);
			}
		}
        currentID = int.Parse(currentShape.blockImage.name) - 1;
		AudioManager.Instance.PlaySound (blockPlaceSound);
	}

	/// <summary>
	/// Checks the board status.
	/// </summary>
	void CheckBoardStatus()
	{
		int placingShapeBlockCount = highlightingBlocks.Count;
		List<int> updatedRows = new List<int> ();
		List<int> updatedColumns = new List<int> ();

		List<List<Block>> breakingRows = new List<List<Block>> ();
		List<List<Block>> breakingColumns = new List<List<Block>> ();

		foreach (Block b in highlightingBlocks) {
			if (!updatedRows.Contains (b.rowID)) {
				updatedRows.Add(b.rowID);
			}
			if (!updatedColumns.Contains (b.columnID)) {
				updatedColumns.Add(b.columnID);
			}
		}
		highlightingBlocks.Clear ();

		foreach(int rowID in updatedRows) {
			List<Block> currentRow = GetEntireRow (rowID);
			if (currentRow != null) {
				breakingRows.Add (currentRow);
			}
		}

		foreach (int columnID in updatedColumns) {
			List<Block> currentColumn = GetEntireColumn (columnID);
			if (currentColumn != null) {
				breakingColumns.Add (currentColumn);
			}
		}

		if (breakingRows.Count > 0 || breakingColumns.Count > 0) {
			StartCoroutine(BreakAllCompletedLines(breakingRows, breakingColumns,placingShapeBlockCount));
		} else {
			BlockShapeSpawner.Instance.FillShapeContainer ();
			ScoreManager.Instance.AddScore (10 * placingShapeBlockCount);
		}

		if (GameController.gameMode == GameMode.BLAST || GameController.gameMode == GameMode.CHALLENGE) {
			Invoke ("UpdateBlockCount", 0.5F);
		}
	}

	/// <summary>
	/// Gets the entire row.
	/// </summary>
	/// <returns>The entire row.</returns>
	/// <param name="rowID">Row I.</param>
	List<Block> GetEntireRow(int rowID)
	{
		List<Block> thisRow = new List<Block> ();
		for (int columnIndex = 0; columnIndex < GameBoardGenerator.Instance.TotalColumns; columnIndex++) {
			Block block = blockGrid.Find (o => o.rowID == rowID && o.columnID == columnIndex); 

			if (block.isFilled) {
				thisRow.Add (block);
			} else {
				return null;
			}
		}
		return thisRow;	
	}

	/// <summary>
	/// Gets the entire row for rescue.
	/// </summary>
	/// <returns>The entire row for rescue.</returns>
	/// <param name="rowID">Row I.</param>
	List<Block> GetEntireRowForRescue(int rowID)
	{
		List<Block> thisRow = new List<Block> ();
		for (int columnIndex = 0; columnIndex < GameBoardGenerator.Instance.TotalColumns; columnIndex++) {
			Block block = blockGrid.Find (o => o.rowID == rowID && o.columnID == columnIndex); 
			thisRow.Add (block);
		}
		return thisRow;	
	}

	/// <summary>
	/// Gets the entire column.
	/// </summary>
	/// <returns>The entire column.</returns>
	/// <param name="columnID">Column I.</param>
	List<Block> GetEntireColumn(int columnID)
	{
		List<Block> thisColumn = new List<Block> ();
		for (int rowIndex = 0; rowIndex < GameBoardGenerator.Instance.TotalRows; rowIndex++) {
			Block block = blockGrid.Find (o => o.rowID == rowIndex && o.columnID == columnID); 
			if (block.isFilled) {
				thisColumn.Add (block);
			} else {
				return null;
			}
		}
		return thisColumn;	
	}

	/// <summary>
	/// Gets the entire column for rescue.
	/// </summary>
	/// <returns>The entire column for rescue.</returns>
	/// <param name="columnID">Column I.</param>
	List<Block> GetEntireColumnForRescue(int columnID)
	{
		List<Block> thisColumn = new List<Block> ();
		for (int rowIndex = 0; rowIndex < GameBoardGenerator.Instance.TotalRows; rowIndex++) {
			Block block = blockGrid.Find (o => o.rowID == rowIndex && o.columnID == columnID); 
			thisColumn.Add (block);
		}
		return thisColumn;	
	}

	/// <summary>
	/// Breaks all completed lines.
	/// </summary>
	/// <returns>The all completed lines.</returns>
	/// <param name="breakingRows">Breaking rows.</param>
	/// <param name="breakingColumns">Breaking columns.</param>
	/// <param name="placingShapeBlockCount">Placing shape block count.</param>
	IEnumerator BreakAllCompletedLines(List<List<Block>> breakingRows, List<List<Block>> breakingColumns, int placingShapeBlockCount)
	{
		int TotalBreakingLines = breakingRows.Count + breakingColumns.Count;
	
		if (TotalBreakingLines == 1) {
			AudioManager.Instance.PlaySound (lineClear1);
			ScoreManager.Instance.AddScore (100 + (placingShapeBlockCount * 10));
		} else if (TotalBreakingLines == 2) {
			AudioManager.Instance.PlaySound (lineClear2);
			ScoreManager.Instance.AddScore (300+ (placingShapeBlockCount * 10));
		} else if (TotalBreakingLines == 3) {
			AudioManager.Instance.PlaySound (lineClear3);
			ScoreManager.Instance.AddScore (600+ (placingShapeBlockCount * 10));
		} else if (TotalBreakingLines >= 4) {
			AudioManager.Instance.PlaySound (lineClear4);
			if (TotalBreakingLines == 4) {
				ScoreManager.Instance.AddScore (1000+ (placingShapeBlockCount * 10));
			} else {
				ScoreManager.Instance.AddScore ((300 * TotalBreakingLines)+ (placingShapeBlockCount * 10));
			}
		}

		yield return 0;
		if (breakingRows.Count > 0) {
			foreach(List<Block> thisLine in breakingRows) {
				StartCoroutine(BreakThisLine (thisLine));
			}	
		}
		if (breakingRows.Count > 0) {
			yield return new WaitForSeconds (0.1F);
		}

		if (breakingColumns.Count > 0) {
			foreach(List<Block> thisLine in breakingColumns){
				StartCoroutine(BreakThisLine (thisLine));
			}	
		}


		BlockShapeSpawner.Instance.FillShapeContainer ();

		#region time mode
		if(GameController.gameMode == GameMode.TIMED || GameController.gameMode == GameMode.CHALLENGE){
			timeSlider.AddSeconds(TotalBreakingLines * 5);
		}
		#endregion
	}

	/// <summary>
	/// Breaks the this line.
	/// </summary>
	/// <returns>The this line.</returns>
	/// <param name="breakingLine">Breaking line.</param>
	IEnumerator BreakThisLine(List<Block> breakingLine)
	{
		foreach (Block b in breakingLine) {
            //b.ClearBlock (int.Parse(breakingLine[breakingLine.Count-1].blockImage.GetComponent<Image>().sprite.name) -1);
            b.ClearBlock();
			yield return new WaitForSeconds (0.02F);
		}
		yield return 0;
	}

	/// <summary>
	/// Determines whether this instance can existing blocks placed the specified OnBoardBlockShapes.
	/// </summary>
	/// <returns><c>true</c> if this instance can existing blocks placed the specified OnBoardBlockShapes; otherwise, <c>false</c>.</returns>
	/// <param name="OnBoardBlockShapes">On board block shapes.</param>
	public bool CanExistingBlocksPlaced(List<ShapeInfo> OnBoardBlockShapes)
	{
		foreach (Block block in blockGrid) {
			if (!block.isFilled) {
				foreach (ShapeInfo info in OnBoardBlockShapes) {
					bool canPlace = CheckShapeCanPlace (block, info);
					if (canPlace) {
						return true;
					}
				}
			}
		}
		return false;
	}

	/// <summary>
	/// Checks the shape can place.
	/// </summary>
	/// <returns><c>true</c>, if shape can place was checked, <c>false</c> otherwise.</returns>
	/// <param name="placingBlock">Placing block.</param>
	/// <param name="placingBlockShape">Placing block shape.</param>
	bool CheckShapeCanPlace(Block placingBlock, ShapeInfo placingBlockShape)
	{
		int currentRowID = placingBlock.rowID;
		int currentColumnID = placingBlock.columnID;

		if (placingBlockShape != null && placingBlockShape.ShapeBlocks != null) {
			foreach (ShapeBlock c in placingBlockShape.ShapeBlocks) {
				Block checkingCell = blockGrid.Find (o => o.rowID == currentRowID + (c.rowID + placingBlockShape.startOffsetX) && o.columnID == currentColumnID + (c.columnID - placingBlockShape.startOffsetY));

				if ((checkingCell == null) || (checkingCell != null && checkingCell.isFilled)) {
					return false;
				}
			}
		}
		return true;
	}

	/// <summary>
	/// Raises the unable to place shape event.
	/// </summary>
	public void OnUnableToPlaceShape()
	{
		if ((TotalRescueDone < MaxAllowedRescuePerGame) || MaxAllowedRescuePerGame < 0) {
			GamePlayUI.Instance.ShowRescue (GameOverReason.OUT_OF_MOVES);
		} else {
			OnGameOver ();
		}
	}

	/// <summary>
	/// Raises the bomb counter over event.
	/// </summary>
	public void OnBombCounterOver(){
		if ((TotalRescueDone < MaxAllowedRescuePerGame) || MaxAllowedRescuePerGame < 0) {
			GamePlayUI.Instance.ShowRescue (GameOverReason.BOMB_COUNTER_ZERO);
		} else {
			OnGameOver ();
		}
	}

	/// <summary>
	/// Executes the rescue.
	/// </summary>
	void ExecuteRescue()
	{
		if (GamePlayUI.Instance.currentGameOverReson == GameOverReason.OUT_OF_MOVES) {
			int TotalBreakingColumns = 3;
			int TotalBreakingRows = 3;

			int totalColumns = GameBoardGenerator.Instance.TotalColumns;
			int totalRows = GameBoardGenerator.Instance.TotalRows;

			int startingColumn = (int)((totalColumns / 2F) - (TotalBreakingColumns / 2F));
			int startingRow = (int)((totalRows / 2F) - (TotalBreakingRows / 2F));

			List<List<Block>> breakingColums = new List<List<Block>> ();

			for (int columnIndex = startingColumn; columnIndex <= (startingColumn + (TotalBreakingColumns - 1)); columnIndex++) {
				breakingColums.Add (GetEntireColumnForRescue (columnIndex));
			}

			List<List<Block>> breakingRows = new List<List<Block>> ();

			for (int rowIndex = startingRow; rowIndex <= (startingRow + (TotalBreakingRows - 1)); rowIndex++) {
				breakingRows.Add (GetEntireRowForRescue (rowIndex));
			}
			StartCoroutine (BreakAllCompletedLines (breakingRows, breakingColums, 0));
		}

		#region bomb mode
		if ((GameController.gameMode == GameMode.BLAST || GameController.gameMode == GameMode.CHALLENGE) && GamePlayUI.Instance.currentGameOverReson == GameOverReason.BOMB_COUNTER_ZERO) {
			List<Block> bombBlocks = blockGrid.FindAll (o => o.isBomb);
			foreach (Block block in bombBlocks) {
				if (block.bombCounter <= 1) {
					block.SetCounter (block.bombCounter + 4);
				}
				block.DecreaseCounter ();
			}
		}
		#endregion

		#region time mode
		if (GameController.gameMode == GameMode.TIMED || GameController.gameMode == GameMode.CHALLENGE) 
		{
			if(GamePlayUI.Instance.currentGameOverReson == GameOverReason.TIME_OVER)
			{
				timeSlider.AddSeconds(30);
			}
			timeSlider.ResumeTimer();
		}
		#endregion
	}

	/// <summary>
	/// Ises the free rescue available.
	/// </summary>
	/// <returns><c>true</c>, if free rescue available was ised, <c>false</c> otherwise.</returns>
	public bool isFreeRescueAvailable()
	{
		if((TotalFreeRescueDone < MaxAllowedVideoWatchRescue) || (MaxAllowedVideoWatchRescue < 0))
		{
			return true;
		}
		return false;
	}

	/// <summary>
	/// Raises the rescue done event.
	/// </summary>
	/// <param name="isFreeRescue">If set to <c>true</c> is free rescue.</param>
	public void OnRescueDone(bool isFreeRescue) 
	{
		if (isFreeRescue) {
			TotalFreeRescueDone += 1;
		} else {
			TotalRescueDone += 1;
		}
		CloseRescuePopup ();
		Invoke ("ExecuteRescue", 0.5F);
	}

	/// <summary>
	/// Raises the rescue discarded event.
	/// </summary>
	public void OnRescueDiscarded()
	{
        CloseRescuePopup();
		OnGameOver ();
	}

	/// <summary>
	/// Closes the rescue popup.
	/// </summary>
	void CloseRescuePopup()
	{
		GameObject currentWindow = StackManager.Instance.WindowStack.Peek ();
		if (currentWindow != null) {
			if (currentWindow.OnWindowRemove () == false) {
				Destroy (currentWindow);
			}
			StackManager.Instance.WindowStack.Pop ();
		}
	}

	/// <summary>
	/// Raises the game over event.
	/// </summary>
	public void OnGameOver()
	{
        
		GameObject gameOverScreen = StackManager.Instance.SpawnUIScreen ("GameOver");
        /*
        int coinReward = 0;
        if (GameController.gameMode == GameMode.CLASSIC)
            coinReward = 15;
        else if (GameController.gameMode == GameMode.TIMED)
            coinReward = 10;
        else if (GameController.gameMode == GameMode.CHALLENGE)
            coinReward = 15;
        else if (GameController.gameMode == GameMode.BLAST)
            coinReward = 5;
        else if (GameController.gameMode == GameMode.ADVANCE)
            coinReward = 25;
            */
        gameOverScreen.GetComponent<GameOver> ().SetLevelScore (FindObjectOfType<ScoreManager>().GetScore(), 10);
        //FireBaseManager.instance.LogScreen("High Score : " + FindObjectOfType<ScoreManager>().GetScore().ToString() 
        //               + "  " + GameController.gameMode.ToString());
        StartCoroutine(ShowAds());
	} 
    IEnumerator ShowAds()
    {
        yield return new WaitForSeconds(0.5f);
        AdsControl.Instance.showAds();
    }

	#region Bomb Mode Specific
	/// <summary>
	/// Updates the block count.
	/// </summary>
	void UpdateBlockCount()
	{
		List<Block> bombBlocks = blockGrid.FindAll (o => o.isBomb);
		foreach (Block block in bombBlocks) {
			block.DecreaseCounter ();
		}

		bool doPlaceBomb = false;
		if (MoveCount <= 10) {
			if (MoveCount % 5 == 0) {
				doPlaceBomb = true;
			}
		} else if (MoveCount <= 30) {
			if (((MoveCount-10) % 4 == 0)) {
				doPlaceBomb = true;
			}

		} else if (MoveCount <= 48) {
			if (((MoveCount-30) % 3 == 0)) {
				doPlaceBomb = true;
			}
		} else {
			if (((MoveCount-48) % 2 == 0)) {
				doPlaceBomb = true;
			}
		}

		if (doPlaceBomb) {
			PlaceAtRandomPlace ();
		}
	}

	/// <summary>
	/// Places at random place.
	/// </summary>
	void PlaceAtRandomPlace()
	{
		List<int> BombPlacingRows = new List<int> ();

		for (int rowIndex = 0; rowIndex < GameBoardGenerator.Instance.TotalRows; rowIndex++) {
			bool canPlace = CheckRowForBombPlace (rowIndex);
			if (canPlace) {
				BombPlacingRows.Add (rowIndex);
			}
		}

		int RandomRowForPlacingBomb = 0;
		if (BombPlacingRows.Count > 0) {
			RandomRowForPlacingBomb = BombPlacingRows [UnityEngine.Random.Range (0, BombPlacingRows.Count)];
		} else {
			RandomRowForPlacingBomb = UnityEngine.Random.Range (0, GameBoardGenerator.Instance.TotalRows);
		}

		PlaceBombAtRow (RandomRowForPlacingBomb);
	}

	/// <summary>
	/// Places the bomb at row.
	/// </summary>
	/// <param name="rowIndex">Row index.</param>
	void PlaceBombAtRow(int rowIndex)
	{
		List<Block> emptyBlockForThisRow = blockGrid.FindAll (o => o.rowID == rowIndex && o.isFilled == false); 
		Block randomBlock = emptyBlockForThisRow [UnityEngine.Random.Range (0, emptyBlockForThisRow.Count)];

		if (randomBlock != null) {
			randomBlock.ConvertToBomb ();
		}
	}

	/// <summary>
	/// Checks the row for bomb place.
	/// </summary>
	/// <returns><c>true</c>, if row for bomb place was checked, <c>false</c> otherwise.</returns>
	/// <param name="rowIndex">Row index.</param>
	bool CheckRowForBombPlace(int rowIndex)
	{
		Block block = blockGrid.Find (o => o.rowID == rowIndex && o.isFilled == true);
		int totalEmptyBlocks = blockGrid.FindAll (o => o.rowID == rowIndex && o.isFilled == false).Count;

		if (block != null && totalEmptyBlocks >= 2) {
			return true;
		} else {
			return false;
		}
	}
	#endregion


	#region show help if mode opened first time
	/// <summary>
	/// Checks for help.
	/// </summary>
	public void CheckForHelp()
	{
		bool isHelpShown = false;
		if (GameController.gameMode == GameMode.BLAST || GameController.gameMode == GameMode.ADVANCE || GameController.gameMode == GameMode.TIMED) {
			isHelpShown = PlayerPrefs.GetInt ("isHelpShown_" + GameController.gameMode.ToString (), 0) == 0 ? false : true;
			if (!isHelpShown) {
				InGameHelp inGameHelp = gameObject.AddComponent<InGameHelp> ();
				inGameHelp.StartHelp ();
			} else {
				ShowBasicHelp ();
			}
		} else {
			ShowBasicHelp ();
		}
	}

	/// <summary>
	/// Raises the help popup closed event.
	/// </summary>
	public void OnHelpPopupClosed()
	{
		if (GameController.gameMode == GameMode.TIMED) {
			timeSlider.ResumeTimer ();
		} 
		ShowBasicHelp ();
	}

	/// <summary>
	/// Shows the basic help.
	/// </summary>
	void ShowBasicHelp()
	{
		bool isBasicHelpShown = PlayerPrefs.GetInt ("isBasicHelpShown", 0) == 0 ? false : true;
		if (!isBasicHelpShown) {
			InGameHelp inGameHelp = gameObject.GetComponent<InGameHelp> ();
			if (inGameHelp == null) {
				inGameHelp = gameObject.AddComponent<InGameHelp> ();
			}
			inGameHelp.ShowBasicHelp ();
		}
	}
	#endregion
}