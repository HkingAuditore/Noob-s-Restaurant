// You can use this in combination with LibNoise to generate 3D noise. You must have LibNoise also installed
// GameObject -> Create 3D Noise
#if LIB_NOISE
using UnityEngine;
using UnityEditor;
using System.Collections;
using LibNoise.Generator;
using LibNoise;

public class GenerateNoiseTexture : EditorWindow 
{
	enum NoiseType
	{
		Perlin,
		Voronoi,
		RidgedMultifractal,
		Billow,
		Spheres,
		Checker,
		Cylinders
	};

	float _frequency = 2f;
	float _lancuraity = 2f;
	float _persistence = 2f;
	int _octaves = 8;
	int _res = 32;
	float _displacement = 1.0f;
	float _size = 1.0f;
	bool _seamless = false;
	bool _normalMap = false;
	NoiseType _noiseType = NoiseType.Perlin;
	Texture2D _previewTexture;
	Texture2D _previewTextureNM;
	AnimationCurve _zCurve = new AnimationCurve()
	{
		keys = new Keyframe[] {
			new Keyframe(0, 0),
			new Keyframe(0.5f, 0.5f),
			new Keyframe(1, 0),
		}
	};

	[MenuItem ("GameObject/Create 3D Noise")]
	static void CreateWizard () 
	{
		EditorWindow.GetWindow<GenerateNoiseTexture>().Show();
	}

	void OnEnabled()
	{
		GeneratePreview();
	}

	void OnDestroy ()
	{
		DestroyImmediate(_previewTexture);
		DestroyImmediate(_previewTextureNM);
		EditorUtility.ClearProgressBar();
	}

	void OnGUI()
	{
		var dirty = false;

		var noiseType = (NoiseType)EditorGUILayout.EnumPopup("Noise", _noiseType);
		if(_noiseType != noiseType)
		{
			_noiseType = noiseType;
			dirty = true;
		}

		dirty |= UpdateValueFloat("Frequency", ref _frequency);
		dirty |= UpdateValueFloat("Lancuraity", ref _lancuraity);
		dirty |= UpdateValueFloat("Persistence", ref _persistence);
		dirty |= UpdateValueFloat("Size", ref _size);
		dirty |= UpdateValueInt("Octaves", ref _octaves);
		dirty |= UpdateValueInt("Resolution", ref _res);
		dirty |= UpdateValueBool("Seamless", ref _seamless);
		dirty |= UpdateValueBool("Normal Map", ref _normalMap);
		_zCurve = EditorGUILayout.CurveField("zCurve", _zCurve);

		if(dirty)
		{
			GeneratePreview();
		}

		GUILayout.FlexibleSpace();

		GUILayout.BeginHorizontal();
		GUI.DrawTexture(GUILayoutUtility.GetRect(128, 128), _previewTexture, ScaleMode.ScaleToFit, false);
		GUI.DrawTexture(GUILayoutUtility.GetRect(128, 128), _previewTexture, ScaleMode.ScaleToFit, false);
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUI.DrawTexture(GUILayoutUtility.GetRect(128, 128), _previewTexture, ScaleMode.ScaleToFit, false);
		GUI.DrawTexture(GUILayoutUtility.GetRect(128, 128), _previewTexture, ScaleMode.ScaleToFit, false);
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();

		if(_normalMap)
		{
			GUILayout.BeginHorizontal();
			GUI.DrawTexture(GUILayoutUtility.GetRect(128, 128), _previewTextureNM, ScaleMode.ScaleToFit, false);
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}
		GUILayout.FlexibleSpace();

		if(GUILayout.Button("Create", GUILayout.Width(100)))
		{
			GenerateTexture();
		}
	}

	ModuleBase CreateNoiseModule()
	{
		ModuleBase noiseModule = null;
		switch(_noiseType)
		{
			case NoiseType.Perlin:
				noiseModule = new Perlin(_frequency, _lancuraity, _persistence, _octaves, 2, QualityMode.Low);
				break;
			case NoiseType.Voronoi:
				noiseModule = new Voronoi(_frequency, _displacement, 0, false);
				break;
			case NoiseType.RidgedMultifractal:
				noiseModule = new RidgedMultifractal(_frequency, _lancuraity, _octaves, 0, QualityMode.Low);
				break;
			case NoiseType.Billow:
				noiseModule = new Billow(_frequency, _lancuraity, _persistence, _octaves, 0, QualityMode.Low);
				break;
			case NoiseType.Spheres:
				noiseModule = new Spheres(_frequency);
				break;
			case NoiseType.Checker:
				noiseModule = new Checker();
				break;
			case NoiseType.Cylinders:
				noiseModule = new Cylinders(_frequency);
				break;
		}
		return noiseModule;
	}
	
	void GeneratePreview () 
	{
		var volumeColors = new Color[_res * _res];	
		if(_previewTexture != null)
		{
			DestroyImmediate(_previewTexture);
		}
		_previewTexture = new Texture2D(_res, _res);
		_previewTexture.wrapMode = TextureWrapMode.Clamp;
		_previewTexture.filterMode = FilterMode.Point;
		_previewTexture.hideFlags = HideFlags.DontSave;


		if(_previewTextureNM != null)
		{
			DestroyImmediate(_previewTextureNM);
		}
		
		var noise = new Noise2D(_res, _res, CreateNoiseModule());
		noise.GeneratePlanar(0, _size, 0, _size, _seamless);
		for(int x = 0; x < _res; x++)
		{
			for(int y = 0; y < _res; y++)
			{
				var idx = x + (y * _res);
				volumeColors[idx] = noise[x, y] * Color.white;
			}
		}
		_previewTexture.SetPixels(volumeColors);
		_previewTexture.Apply();

		if(_normalMap)
		{
			_previewTextureNM = noise.GetNormalMap(2.0f);
			_previewTextureNM.wrapMode = TextureWrapMode.Clamp;
			_previewTextureNM.filterMode = FilterMode.Point;
			_previewTextureNM.hideFlags = HideFlags.DontSave;
		}
	}
	
	void GenerateTexture()
	{
		var volumeTex = new Texture3D(_res, _res, _res, TextureFormat.ARGB32, false);
		var volumeColors = new Color[_res * _res * _res];

		var volumeTexNM = new Texture3D(_res, _res, _res, TextureFormat.ARGB32, false);
		var volumeColorsNM = new Color[_res * _res * _res];

		EditorUtility.ClearProgressBar();

		var noise = new Noise2D(_res, _res, CreateNoiseModule());
		for(int z = 0; z < _res; z++)
		{
			noise.zValue = _zCurve.Evaluate(((float)z / _res)) * _size;
			noise.GeneratePlanar(0, _size, 0, _size, _seamless);
			for(int x = 0; x < _res; x++)
			{
				for(int y = 0; y < _res; y++)
				{
					var idx = x + (y * _res) + (z * (_res * _res));
					volumeColors[idx] = noise[x, y] * Color.white;
				}
			}
			if(EditorUtility.DisplayCancelableProgressBar("3D Noise", "Generating 3D Noise", (float)z / _res))
			{
				DestroyImmediate(volumeTex);
				EditorUtility.ClearProgressBar();
				return;
			}

			var nm = noise.GetNormalMap(1.0f);
			var pixels = nm.GetPixels();
			for(int x = 0; x < _res; x++)
			{
				for(int y = 0; y < _res; y++)
				{
					var idx = x + (y * _res) + (z * (_res * _res));
					volumeColorsNM[idx] = pixels[x + (y * _res)];
				}
			}
			DestroyImmediate(nm);
		}


		EditorUtility.ClearProgressBar();

		var path = EditorUtility.SaveFilePanelInProject("Save 3D noise texture.",
		                                                string.Format("{0}_noise.asset", _noiseType),
		                                                "asset",
		                                                "Please enter a file name to save the volume texture");
		if(path.Length != 0) 
		{
			volumeTex.SetPixels(volumeColors);
			volumeTex.Apply();

			AssetDatabase.CreateAsset(volumeTex, path);
			AssetDatabase.Refresh();
		}

		if(_normalMap)
		{
			volumeTexNM.SetPixels(volumeColorsNM);
			volumeTexNM.Apply();

			path = EditorUtility.SaveFilePanelInProject("Save 3D normalmap texture.",
			                                                string.Format("{0}_normalmap.asset", _noiseType),
			                                                "asset",
			                                                "Please enter a file name to save the volume normalmap");
			if(path.Length != 0) 
			{
				AssetDatabase.CreateAsset(volumeTexNM, path);
				AssetDatabase.Refresh();
			}
		}
	}

	static bool UpdateValueBool(string name, ref bool value)
	{
		var newValue = EditorGUILayout.Toggle(name, value);
		if(value != newValue)
		{
			value = newValue;
			return true;
		}
		return false;
	}

	static bool UpdateValueInt(string name, ref int value)
	{
		var newValue = EditorGUILayout.IntField(name, value);
		if(value != newValue)
		{
			value = newValue;
			return true;
		}
		return false;
	}
	
	static bool UpdateValueFloat(string name, ref float value)
	{
		var newValue = EditorGUILayout.FloatField(name, value);
		if(value != newValue)
		{
			value = newValue;
			return true;
		}
		return false;
	}
}
#endif