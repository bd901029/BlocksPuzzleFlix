using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if HBDOTween
using DG.Tweening;
#endif


public class GameBoardGenerator : Singleton<GameBoardGenerator>
{
	/// Total Rows, Configurable from inspector.
	public int TotalRows;

	/// Total Column Count, Configurable from inspector.
	public int TotalColumns;

	/// Space between each blocks, Configurable from inspector.
	public int blockSpace = 5;

	/// The content of the board.
	public GameObject BoardContent;

	/// The empty block template.
	public GameObject emptyBlockTemplate;

    /// The empty block template.
    public GameObject blockPanel;

	int startPosx = 0;
	int startPosy = 0;

	int blockWidth = 0;
	int blockHeight = 0;

	int cellIndex = 0;

	public PreviousSessionData previousSessionData;
	void Start()
	{
		///checks if level needs to start from previos session or start new session.
		previousSessionData = GetComponent<GameProgressManager> ().GetPreviousSessionData ();
	}

	/// <summary>
	/// Generates the board.
	/// </summary>
	public void GenerateBoard ()
	{	
		if (previousSessionData != null) 
		{
			if(previousSessionData.blockGridInfo.Count > 0)
			{
				TotalRows = previousSessionData.blockGridInfo.Count;
				TotalColumns = previousSessionData.blockGridInfo [0].Split (',').Length;
			}
		}

		blockHeight = (int)emptyBlockTemplate.GetComponent<RectTransform>().sizeDelta.x;
		blockWidth = (int)emptyBlockTemplate.GetComponent<RectTransform>().sizeDelta.y;

		startPosx = -(((TotalColumns - 1) * (blockHeight + blockSpace)) / 2);
		startPosy = (((TotalRows - 1) * (blockWidth + blockSpace)) / 2);

		int newPosX = startPosx;
		int newPosY = startPosy;

        //gen block panel
        GameObject blockPanelObj = (GameObject)Instantiate(blockPanel);
        blockPanelObj.transform.SetParent(BoardContent.transform);
        blockPanelObj.transform.localScale = new Vector3(0.95f, 0.95f, 0.95f);
        blockPanelObj.GetComponent<RectTransform>().anchoredPosition3D  = Vector3.zero;

		for (int row = 0; row < TotalRows; row++) {
			List<Block> thisRowCells = new List<Block> ();
			for (int column = 0; column < TotalColumns; column++) {

				GameObject newCell = GenerateNewBlock (row, column);
				newCell.GetComponent<RectTransform> ().anchoredPosition3D = new Vector3 (newPosX, (newPosY), 0);
				newPosX += (blockWidth + blockSpace);
				Block thisCellInfo = newCell.GetComponent<Block> ();
				thisCellInfo.blockImage = newCell.transform.GetChild (0).GetComponent<Image> ();
				thisCellInfo.rowID = row;
				thisCellInfo.columnID = column;
				thisRowCells.Add (thisCellInfo);
				cellIndex++;
			}
			GamePlay.Instance.blockGrid.AddRange (thisRowCells);
			newPosX = startPosx;
			newPosY -= (blockHeight + blockSpace);
		}

		SetupPreviousSessionBoard ();
	}

	/// <summary>
	/// Setups the previous session board.
	/// </summary>
	void SetupPreviousSessionBoard()
	{
		if (previousSessionData != null) 
		{
			#region normal blocks setup on board
			if (previousSessionData.blockGridInfo.Count > 0) 
			{
				int rowIndex = 0;
				foreach (string gridRow in previousSessionData.blockGridInfo) {
					int columnIndex = 0;
					foreach (string blockID in gridRow.Split(',')) {
						int thisBlockID = blockID.TryParseInt ();
						if (thisBlockID >= 0) {
							Block thisBlock = GamePlay.Instance.blockGrid.Find (o => o.rowID == rowIndex && o.columnID == columnIndex);
							if (thisBlock != null) {
								thisBlock.ConvertToFilledBlock (thisBlockID);
							}
						}
						columnIndex++;
					}
					rowIndex ++;
				}
			}
			#endregion

			#region place previous session bombs
			if(GameController.gameMode == GameMode.BLAST || GameController.gameMode == GameMode.CHALLENGE)
			{
				if (previousSessionData.placedBombInfo != null) {
					foreach(PlacedBomb bomb in previousSessionData.placedBombInfo) {
						Block thisBlock = GamePlay.Instance.blockGrid.Find (o => o.rowID == bomb.rowID && o.columnID == bomb.columnID);
						thisBlock.ConvertToBomb(bomb.bombCounter);
					}
				}
			}
			#endregion

			#region set score
			ScoreManager.Instance.AddScore(previousSessionData.score,false);
			#endregion

			#region moves count
			GamePlay.Instance.MoveCount = previousSessionData.movesCount;
			#endregion

			#region set timer for time and challenge mode
			if(GameController.gameMode == GameMode.TIMED || GameController.gameMode == GameMode.CHALLENGE)
			{
				GamePlay.Instance.timeSlider.SetTime(previousSessionData.remainingTime);
			}
			#endregion
		}
		GetComponent<GameProgressManager> ().ClearProgress ();
	}

	/// <summary>
	/// Generates the new block.
	/// </summary>
	/// <returns>The new block.</returns>
	/// <param name="rowIndex">Row index.</param>
	/// <param name="columnIndex">Column index.</param>
	GameObject GenerateNewBlock (int rowIndex, int columnIndex)
	{
		GameObject newBlock = (GameObject)Instantiate (emptyBlockTemplate);
		newBlock.GetComponent<RectTransform> ().sizeDelta = new Vector2 ((blockWidth), (blockHeight));
		newBlock.transform.SetParent (BoardContent.transform);
		newBlock.transform.localScale = Vector3.one;
		newBlock.transform.SetAsLastSibling ();
		newBlock.name = "Block-" + rowIndex.ToString () + "-" + columnIndex.ToString ();
		return newBlock;
	}
}