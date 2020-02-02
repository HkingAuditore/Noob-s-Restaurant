using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * VLight
 * Copyright Brian Su 2011-2015
*/

[ExecuteInEditMode]
public class VLightManager : MonoBehaviour
{
	public const string VOLUMETRIC_LIGHT_LAYER_NAME = "vlight";
	
	//Optional camere to use instead of Camera.main
	public Camera targetCamera;
	public float maxDistance = 50;
	//public AnimationCurve lodFallOff = AnimationCurve.Linear(0, 1, 1, 0);

	private static VLightManager _instance;

	public static VLightManager Instance
	{
		get
		{
			if(_instance == null)
			{
				_instance = GameObject.FindObjectOfType(typeof(VLightManager)) as VLightManager;
				if(_instance == null)
				{
					GameObject goManager = new GameObject("Volume Light Manager");
					_instance = goManager.AddComponent<VLightManager>();
				}
			}
			return _instance;
		}
	}
	
	private Matrix4x4 _projection;

	public Matrix4x4 ViewProjection
	{
		get { return _projection; }

	}

	private Matrix4x4 _cameraToWorld;

	public Matrix4x4 ViewCameraToWorldMatrix
	{
		get { return _cameraToWorld; }
	}

	private Matrix4x4 _worldToCamera;

	public Matrix4x4 ViewWorldToCameraMatrix
	{
		get { return _worldToCamera; }
	}

	private List<VLight> _vLights = new List<VLight>();

	public List<VLight> VLights
	{
		get { return _vLights; }
		set { _vLights = value; }
	}

	public void UpdateViewCamera(Camera viewCam)
	{
		if(viewCam == null)
		{
			return;
		}
		_cameraToWorld = viewCam.cameraToWorldMatrix;
		_worldToCamera = viewCam.worldToCameraMatrix;
		_projection = viewCam.projectionMatrix;
	}
	
	private void Update()
	{
		if(Application.isPlaying)
		{
			Camera cam;
			
			if(Camera.current != null)
			{
				cam = Camera.current;
			}
			else if(targetCamera != null)
			{
				cam = targetCamera;
			}
			else
			{
				cam = Camera.main;
			}
				 
			if(cam == null)
			{
				return;
			}
			
			//// Flush out and lights deleted while running
			//_vLights = _vLights.FindAll((vLight)=> vLight != null);
			
			//Vector3 camPos = cam.transform.position;
			//foreach (var vLight in _vLights)
			//{
			//    if (vLight.dynamicLevelOfDetail)
			//    {
			//        float distance = Vector3.Distance(vLight.transform.position, camPos);
			//        float value = lodFallOff.Evaluate(1 - Mathf.Clamp(maxDistance / distance, 0, 1));
			//        vLight.slices = (int)Mathf.Lerp(vLight.minSlices, vLight.MaxSlices, Mathf.Clamp(value, 0, 1));
			//    }
			//}
		}		
	}

	private void Start()
	{
		_vLights.Clear();
		VLight[] vLights = GameObject.FindObjectsOfType(typeof(VLight)) as VLight[];
		_vLights.AddRange(vLights);
	}
	
	private void Enabled()
	{
		_vLights.Clear();
		VLight[] vLights = GameObject.FindObjectsOfType(typeof(VLight)) as VLight[];
		_vLights.AddRange(vLights);
	}
}
