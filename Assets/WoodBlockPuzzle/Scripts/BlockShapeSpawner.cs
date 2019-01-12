using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

#if HBDOTween
using DG.Tweening;
#endif


public class BlockShapeSpawner : Singleton<BlockShapeSpawner> 
{
	[Tooltip("Setting this true means placing a block will add new block instantly, false means new shape blocks will be added only once all three are placed on the board.")]
	public bool keepFilledAlways = false;

	[SerializeField] ShapeBlockList shapeBlockList;

	[SerializeField] ShapeBlockList shapeBlockList_Plus;

	[HideInInspector] public ShapeBlockList ActiveShapeBlockModule;

	[SerializeField] Transform[] ShapeContainers;

	List<int> shapeBlockProbabilityPool;

	int shapeBlockPoolCount = 1;

	/// <summary>
	/// Awake this instance.
	/// </summary>
	void Awake()
	{
		if (GameController.gameMode == GameMode.ADVANCE || GameController.gameMode == GameMode.CHALLENGE) {
			ActiveShapeBlockModule = shapeBlockList_Plus;
		} else {
			ActiveShapeBlockModule= shapeBlockList;
		}
	}

	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start()
	{
		#region blast mode
		if (GameController.gameMode == GameMode.BLAST || GameController.gameMode == GameMode.CHALLENGE) {
			keepFilledAlways = true;
		}
		#endregion

		Invoke ("SetupPreviousSessionShapes", 0.2F);
		Invoke ("createShapeBlockProbabilityList", 0.5F);
		Invoke ("FillShapeContainer", 0.5F);
	}

	/// <summary>
	/// Setups the previous session shapes.
	/// </summary>
	void SetupPreviousSessionShapes()
	{
		if (GameBoardGenerator.Instance.previousSessionData != null) {
			List<int> shapes = GameBoardGenerator.Instance.previousSessionData.shapeInfo.Split(',').Select(Int32.Parse).ToList();

			int shapeIndex = 0;
			foreach (int shapeID in shapes) {
				if (shapeID >= 0) {
					CreateShapeWithID (ShapeContainers [shapeIndex], shapeID);
				}
				shapeIndex += 1;
			}
		}
	}

	/// <summary>
	/// Creates the shape block probability list.
	/// </summary>
	void createShapeBlockProbabilityList()
	{
		shapeBlockProbabilityPool = new List<int> ();
		if (ActiveShapeBlockModule != null) {
			foreach (ShapeBlockSpawn shapeBlock in ActiveShapeBlockModule.ShapeBlocks) {
				AddShapeInProbabilityPool (shapeBlock.BlockID, shapeBlock.spawnProbability);
			}
		}
		shapeBlockProbabilityPool.Shuffle ();
	}

	/// <summary>
	/// Adds the shape in probability pool.
	/// </summary>
	/// <param name="blockID">Block I.</param>
	/// <param name="probability">Probability.</param>
	void AddShapeInProbabilityPool(int blockID, int probability) {
		int probabiltyTimesToAdd = shapeBlockPoolCount * probability;

		for (int index = 0; index < probabiltyTimesToAdd; index++) {
			shapeBlockProbabilityPool.Add (blockID);
		}
	}

	/// <summary>
	/// Fills the shape container.
	/// </summary>
	public void FillShapeContainer()
	{
		ReorderShapes ();

		if (!keepFilledAlways) {
			bool isAllEmpty = true;
			foreach (Transform shapeContainer in ShapeContainers) {
				if (shapeContainer.childCount > 0) {
					isAllEmpty = false;
				}
			}

			if (isAllEmpty) {
				foreach (Transform shapeContainer in ShapeContainers) {
					AddRandomShapeToContainer (shapeContainer);
				}
			}
		} else {
			foreach (Transform shapeContainer in ShapeContainers) {
				if (shapeContainer.childCount <= 0) {
					AddRandomShapeToContainer (shapeContainer);
				}
			}
		}

		Invoke ("CheckOnBoardShapeStatus", 0.2F);
	}

	/// <summary>
	/// Adds the random shape to container.
	/// </summary>
	/// <param name="shapeContainer">Shape container.</param>
	public void AddRandomShapeToContainer(Transform shapeContainer)
	{
		if (shapeBlockProbabilityPool == null || shapeBlockProbabilityPool.Count <= 0) {
			createShapeBlockProbabilityList ();
		}

		int RandomShape = shapeBlockProbabilityPool [0];
		shapeBlockProbabilityPool.RemoveAt (0);

		GameObject newShapeBlock = ActiveShapeBlockModule.ShapeBlocks.Find(o => o.BlockID == RandomShape).shapeBlock;
		GameObject spawningShapeBlock = (GameObject)(Instantiate (newShapeBlock)) as GameObject;
		spawningShapeBlock.transform.SetParent (shapeContainer);
		spawningShapeBlock.transform.localScale = Vector3.one * 0.6F;
		spawningShapeBlock.GetComponent<RectTransform> ().anchoredPosition3D = new Vector3 (800F, 0, 0);
		#if HBDOTween
		spawningShapeBlock.transform.DOLocalMove (Vector3.zero, 0.3F);
		#endif
	}

	/// <summary>
	/// Creates the shape with I.
	/// </summary>
	/// <param name="shapeContainer">Shape container.</param>
	/// <param name="shapeID">Shape I.</param>
	void CreateShapeWithID(Transform shapeContainer, int shapeID)
	{
		GameObject newShapeBlock = ActiveShapeBlockModule.ShapeBlocks.Find(o => o.BlockID == shapeID).shapeBlock;
		GameObject spawningShapeBlock = (GameObject)(Instantiate (newShapeBlock)) as GameObject;
		spawningShapeBlock.transform.SetParent (shapeContainer);
		spawningShapeBlock.transform.localScale = Vector3.one * 0.6F;
		spawningShapeBlock.GetComponent<RectTransform> ().anchoredPosition3D = new Vector3 (800F, 0, 0);
		#if HBDOTween
		spawningShapeBlock.transform.DOLocalMove (Vector3.zero, 0.3F);
		#endif
	}

	/// <summary>
	/// Checks the on board shape status.
	/// </summary>
	public void CheckOnBoardShapeStatus()
	{
		List<ShapeInfo> OnBoardBlockShapes = new List<ShapeInfo> ();
		foreach (Transform shapeContainer in ShapeContainers) {
			if (shapeContainer.childCount > 0) {
				OnBoardBlockShapes.Add (shapeContainer.GetChild (0).GetComponent<ShapeInfo> ());
			}
		}

		bool canExistingBlocksPlaced = GamePlay.Instance.CanExistingBlocksPlaced (OnBoardBlockShapes);

		if (canExistingBlocksPlaced == false) {
			GamePlay.Instance.OnUnableToPlaceShape ();
		}
	}

	/// <summary>
	/// Reorders the shapes.
	/// </summary>
	void ReorderShapes()
	{
		List<Transform> EmptyShapes = new List<Transform> ();

		foreach (Transform shapeContainer in ShapeContainers) {
			if (shapeContainer.childCount == 0) {
				EmptyShapes.Add (shapeContainer);
			} else {
				if (EmptyShapes.Count > 0) {
					Transform emptyContainer = EmptyShapes [0];
					shapeContainer.GetChild (0).SetParent (emptyContainer);
					EmptyShapes.RemoveAt (0);
					#if HBDOTween
					emptyContainer.GetChild (0).DOLocalMove (Vector3.zero, 0.3F);
					#endif
					EmptyShapes.Add (shapeContainer);
				}
			}
		}
	}

	/// <summary>
	/// Gets all on board shape names.
	/// </summary>
	/// <returns>The all on board shape names.</returns>
	public string GetAllOnBoardShapeNames()
	{
		string shapeNames = "";
		foreach (Transform shapeContainer in ShapeContainers) {
			if (shapeContainer.childCount > 0) {
				shapeNames = shapeNames + shapeContainer.GetChild (0).GetComponent<ShapeInfo> ().ShapeID + ",";
			} else {
				shapeNames = shapeNames + "-1,";
			}
		}

		shapeNames = shapeNames.Remove (shapeNames.Length-1);
		return shapeNames;
	}
}

