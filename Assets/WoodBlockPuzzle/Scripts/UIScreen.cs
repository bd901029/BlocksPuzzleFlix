using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIScreen : MonoBehaviour 
{
	void Awake()
	{
        Application.targetFrameRate = 60;
		init ();	
	}

	public virtual void init()
	{
		StackManager.Instance.PushWindow (gameObject);
	}
}
