using UnityEditor;

/*
 * VLight
 * Copyright Brian Su 2011-2019
*/
[CustomEditor(typeof(VLightManager))]
public class VolumeLightManagerEditor : Editor
{
    public VLightManager Manager
    {
        get { return target as VLightManager; }
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
    }
}