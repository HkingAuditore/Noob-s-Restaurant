using UnityEngine;
using UnityEditor;

/*
 * VLight
 * Copyright Brian Su 2011-2019
*/
public class VolumeLightCreator : EditorWindow
{
    [MenuItem("GameObject/Create Other/V-Lights Volume Area (Beta)", false, 100)]
    public static void Area()
    {
        if (ShowWarning())
        {
            var volumeLightContainer = CreateVolumeLight(VLight.LightTypes.Area);
            Selection.activeGameObject = volumeLightContainer.gameObject;
            volumeLightContainer.transform.rotation = Quaternion.identity;

            Undo.RegisterCreatedObjectUndo(volumeLightContainer, "V-Lights Create Volume Area (Beta)");
        }
    }


    [MenuItem("GameObject/Create Other/V-Lights Spot", false, 100)]
    public static void StandardLight()
    {
        if (ShowWarning())
        {
            var volumeLightContainer = CreateVolumeLight(VLight.LightTypes.Spot);
            Selection.activeGameObject = volumeLightContainer.gameObject;

            Undo.RegisterCreatedObjectUndo(volumeLightContainer, "V-Lights Create Light");
        }
    }

    [MenuItem("GameObject/Create Other/V-Lights Spot With Light", false, 100)]
    public static void SpotWithLight()
    {
        if (ShowWarning())
        {
            var volumeLightContainer = CreateVolumeLight(VLight.LightTypes.Spot);
            var pointLight = new GameObject("Spot light");
            var light = pointLight.AddComponent<Light>();
            light.shadows = LightShadows.Soft;
            light.type = LightType.Spot;
            light.spotAngle = 45;
            light.range = 6;
            pointLight.transform.parent = volumeLightContainer.transform;
            pointLight.transform.localPosition = Vector3.zero;
            pointLight.transform.Rotate(90, 0, 0);
            Selection.activeGameObject = volumeLightContainer.gameObject;

            Undo.RegisterCreatedObjectUndo(volumeLightContainer, "V-Lights Create Light");
            Undo.RegisterCreatedObjectUndo(pointLight, "V-Lights Create Light");
        }
    }

    [MenuItem("GameObject/Create Other/V-Lights Point", false, 100)]
    public static void PointLight()
    {
        if (ShowWarning())
        {
            var volumeLightContainer = CreateVolumeLight(VLight.LightTypes.Point);
            Selection.activeGameObject = volumeLightContainer.gameObject;
            Undo.RegisterCreatedObjectUndo(volumeLightContainer, "V-Lights Create Light");
        }
    }

    [MenuItem("GameObject/Create Other/V-Lights Point With Light", false, 100)]
    public static void PointWithLight()
    {
        if (ShowWarning())
        {
            var volumeLightContainer = CreateVolumeLight(VLight.LightTypes.Point);
            var pointLight = new GameObject("Point light");
            var light = pointLight.AddComponent<Light>();
            light.shadows = LightShadows.Soft;
            light.type = LightType.Point;
            light.spotAngle = 45;
            light.range = 6;
            pointLight.transform.parent = volumeLightContainer.transform;
            pointLight.transform.localPosition = Vector3.zero;
            Selection.activeGameObject = volumeLightContainer.gameObject;

            Undo.RegisterCreatedObjectUndo(volumeLightContainer, "V-Lights Create Light");
            Undo.RegisterCreatedObjectUndo(pointLight, "V-Lights Create Light");
        }
    }

    [MenuItem("GameObject/Create Other/V-Lights Orthographic", false, 100)]
    public static void OrthographicLight()
    {
        if (ShowWarning())
        {
            var volumeLightContainer = CreateVolumeLight(VLight.LightTypes.Orthographic);
            Selection.activeGameObject = volumeLightContainer.gameObject;

            Undo.RegisterCreatedObjectUndo(volumeLightContainer, "V-Lights Create Orthographic Light");
        }
    }

    private static VLight CreateVolumeLight(VLight.LightTypes type)
    {
        var otherLights = GameObject.FindObjectsOfType(typeof(VLight)) as VLight[];
        var volumeLightContainer = new GameObject("V-Light " + type + " " + otherLights.Length);
        if (SceneView.lastActiveSceneView != null)
        {
            SceneView.lastActiveSceneView.MoveToView(volumeLightContainer.transform);
        }
        var light = volumeLightContainer.AddComponent<VLight>();
        var cam = volumeLightContainer.GetComponent<Camera>();
        cam.enabled = false;

        cam.fieldOfView = 45;
        cam.nearClipPlane = 0.1f;
        cam.farClipPlane = 1;
        cam.renderingPath = RenderingPath.VertexLit;
        cam.orthographicSize = 2.5f;

        switch (type)
        {
            case VLight.LightTypes.Spot:
                light.spotRange = 1;
                light.lightType = VLight.LightTypes.Spot;
                break;
            case VLight.LightTypes.Point:
                cam.orthographic = true;
                cam.nearClipPlane = -cam.farClipPlane;
                cam.orthographicSize = cam.farClipPlane * 2;
                light.pointLightRadius = 1;
                light.lightType = VLight.LightTypes.Point;
                break;
            case VLight.LightTypes.Area:
                cam.orthographic = true;
                cam.nearClipPlane = -cam.farClipPlane;
                cam.orthographicSize = cam.farClipPlane * 2;
                light.pointLightRadius = 1;
                light.lightType = VLight.LightTypes.Area;
                light.lightMultiplier = 0.05f;
                light.pointLightRadius = 2.0f;
                break;
            case VLight.LightTypes.Orthographic:
                cam.orthographic = true;
                cam.nearClipPlane = 0;
                cam.farClipPlane = 2;
                light.spotRange = 2;
                light.lightType = VLight.LightTypes.Orthographic;
                light.orthoSize = 0.5f;
                break;
        }

        var layer = LayerMask.NameToLayer(VLightManager.VOLUMETRIC_LIGHT_LAYER_NAME);
        if (layer != -1)
        {
            volumeLightContainer.layer = layer;
            cam.cullingMask = ~(1 << layer);
        }

        volumeLightContainer.transform.Rotate(90, 0, 0);
        return light;
    }

    private static bool ShowWarning()
    {
        bool continueAfterWarning = true;
        if (LayerMask.NameToLayer(VLightManager.VOLUMETRIC_LIGHT_LAYER_NAME) == -1)
        {
            continueAfterWarning = EditorUtility.DisplayDialog("Warning",
                "You don't have a layer in your project called\n\"" + VLightManager.VOLUMETRIC_LIGHT_LAYER_NAME + "\".\n" +
                "Without this layer realtime shadows, interleaved sampling and high speed off screen rendering will not work. Continue using volumetric lights?", "Continue", "Cancel");
        }
        return continueAfterWarning;
    }
}
