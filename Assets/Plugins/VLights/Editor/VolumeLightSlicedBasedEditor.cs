using UnityEngine;
using UnityEditor;

/*
 * VLight
 * Copyright Brian Su 2011-2019
*/
[CustomEditor(typeof(VLight))]
[CanEditMultipleObjects]
public class VolumeLightSlicedBasedEditor : Editor
{
    public VLight Light
    {
        get { return target as VLight; }
    }

    public override void OnInspectorGUI()
    {
        Light.MeshRender.hideFlags = HideFlags.None;

        var property = serializedObject.FindProperty("renderWireFrame");

        property.boolValue = GUILayout.Toggle(property.boolValue, "Render wireframe");

        EditorUtility.SetSelectedRenderState(Light.MeshRender, property.boolValue ? EditorSelectedRenderState.Wireframe : EditorSelectedRenderState.Hidden);

        serializedObject.ApplyModifiedProperties();

        base.OnInspectorGUI();

        GUILayout.Space(20);

        var curvesProp = serializedObject.FindProperty("useCurves");
        if(curvesProp.boolValue)
        {
            GUILayout.Label("Falloff gradient");
            var tex = serializedObject.FindProperty("_fallOffTexture");
            if(tex.objectReferenceValue != null)
            {
                var rect = GUILayoutUtility.GetRect(100, 100);
                GUI.DrawTexture(rect, tex.objectReferenceValue as Texture2D);
            }
        }

        GUILayout.Label("Generate a baked shadow map");
        if(GUILayout.Button("Bake shadow map", GUILayout.Width(200)))
        {
            Light.RenderBakedShadowMap();
        }
    }
}