using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DishesRenderer))]
public class DishesRendererGUI : Editor {
	private string[] _SelStrings = {"None", "Fry", "Stir", "Boil"};


	public int _Selected=0;

	
    public override void OnInspectorGUI(){
        DrawDefaultInspector();
        DishesRenderer dishesRenderGUI = (DishesRenderer) target;
        _Selected=GUILayout.SelectionGrid(_Selected,_SelStrings,4);
    	if(GUILayout.Button("Add Process")){
            dishesRenderGUI.AddProcess((DishesRenderer.ProcessType)_Selected);
        }
        if (GUILayout.Button("Remove Process"))
        {
            dishesRenderGUI.RemoveProcess();
        }

    }
}
