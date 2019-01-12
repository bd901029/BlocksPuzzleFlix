using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ShapeBlockList", menuName = "BlockPuzzle/ShapeBlockList", order = 1)]
public class ShapeBlockList : ScriptableObject  
{
	public List<ShapeBlockSpawn> ShapeBlocks;
}

[System.Serializable]
public class ShapeBlockSpawn
{
	public int BlockID = 0;
	public GameObject shapeBlock;
	[Range(1,10)]
	public int spawnProbability = 1;
}
