using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class CheckForStandardAssets : MonoBehaviour {

	// Use this for initialization
	void Awake () {
        #if UNITY_EDITOR 
        var guids = AssetDatabase.FindAssets("FXWater4Advanced", null);
	    Debug.Assert(guids.Length > 0, "Please add Unity's Standard Assets to make water works! https://www.assetstore.unity3d.com/en/#!/content/32351");
        #endif
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
