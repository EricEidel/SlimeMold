using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MyLightingShaderGUI : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

    void DoAdvanced()
    {
        GUILayout.Label("Advanced Options", EditorStyles.boldLabel);

        //editor.EnableInstancingField();
    }
}
