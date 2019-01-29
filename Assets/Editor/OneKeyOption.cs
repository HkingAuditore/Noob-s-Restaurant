using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.PostProcessing;
using UnityStandardAssets.CinematicEffects;

public class OneKeyOption : Editor
{
    [MenuItem("One Key Option/Performance/Separate/Reflection Probe/Enable")]
    private static void EnableAllReflectionProbe()
    {
        ReflectionProbe[] rps = FindObjectsOfType<ReflectionProbe>();
        Array.ForEach(rps, (rp) => { rp.enabled = true; rp.mode = UnityEngine.Rendering.ReflectionProbeMode.Baked; });

        Debug.Log(rps.Length + " Reflection Probes were enabled");
    }

    [MenuItem("One Key Option/Performance/Separate/Reflection Probe/Disable")]
    private static void DisableAllReflectionProbe()
    {
        ReflectionProbe[] rps = FindObjectsOfType<ReflectionProbe>();
        Array.ForEach(rps, (rp) => rp.enabled = false);

        Debug.Log(rps.Length + " Reflection Probes were disabled");
    }

    [MenuItem("One Key Option/Performance/Separate/Light/Enable")]
    private static void EnableAllLight()
    {
        Light[] lights = FindObjectsOfType<Light>();
        Array.ForEach(lights, (rp) => { rp.enabled = true; });

        Debug.Log(lights.Length + " Lights were enabled");
    }

    [MenuItem("One Key Option/Performance/Separate/Light/Disable")]
    private static void DisableAllLight()
    {
        Light[] lights = FindObjectsOfType<Light>();
        Array.ForEach(lights, (rp) => rp.enabled = false);

        Debug.Log(lights.Length + " Lights were disabled");
    }

    [MenuItem("One Key Option/Performance/Separate/Post Processing/Enable")]
    private static void EnablePostProcessing()
    {
        GameObject camera = GameObject.Find("Camera/Main Camera");
        //PostProcessingBehaviour pp = camera.GetComponent<PostProcessingBehaviour>();
        AntiAliasing aa = camera.GetComponent<AntiAliasing>();
        MotionBlur mb = camera.GetComponent<MotionBlur>();
        //pp.enabled = true;
        aa.enabled = true;
        mb.enabled = true;

        Debug.Log("Post Processings were enabled");
    }

    [MenuItem("One Key Option/Performance/Separate/Post Processing/Disable")]
    private static void DisablePostProcessing()
    {
        GameObject camera = GameObject.Find("Camera/Main Camera");
        PostProcessingBehaviour pp = camera.GetComponent<PostProcessingBehaviour>();
        AntiAliasing aa = camera.GetComponent<AntiAliasing>();
        MotionBlur mb = camera.GetComponent<MotionBlur>();
        pp.enabled = false;
        aa.enabled = false;
        mb.enabled = false;

        Debug.Log("Post Processings were disabled");
    }

    [MenuItem("One Key Option/Performance/OverallQuality/High")]
    private static void EnableHighQuality()
    {
        EnableAllReflectionProbe();
        EnableAllLight();
        EnablePostProcessing();
    }

    [MenuItem("One Key Option/Performance/OverallQuality/Low")]
    private static void EnableLowQuality()
    {
        DisableAllReflectionProbe();
        DisableAllLight();
        DisablePostProcessing();
    }
}
