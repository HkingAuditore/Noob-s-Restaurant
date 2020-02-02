using UnityEngine;
using VLights;

/*
 * VLight
 * Copyright Brian Su 2011-2019
*/

[ExecuteInEditMode()]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("V-Lights/VLight Image Effects")]
public class VLightInterleavedSampling : MonoBehaviour
{
    public static bool renderingInterleaved = false;
    public static bool lightsModified = false;

    [SerializeField]
    [Header("Reduces banding. Requires floating point textures support.")]
    private bool _useHighPrecisionFrameBuffer = false;
    [SerializeField]
    [Header("Min pixel width to use interleaved")]
    private int minInterleavedRes = 128;

    [SerializeField]
    private bool useInterleavedSampling = true;
    [SerializeField]
    private float ditherOffset = 0.02f;
    [SerializeField]
    private float blurRadius = 1.5f;
    [SerializeField]
    private int blurIterations = 1;
    [SerializeField]
    [Header("Locked to 2 when using Bilateral filtering")]
    private int downSample = 4;
    [SerializeField]
    private Shader postEffectShader;
    [SerializeField]
    private Shader volumeLightShader;

    [Header("Reduce edge bleeding at a cost to performance")]
    [SerializeField]
    private bool _useBilateralFiltering = false;
    [SerializeField]
    private Shader _downScaleDepthShader;
    [SerializeField]
    private float _depthThreshold = 0.01f;
    [SerializeField]
    private float _blurDepth = 100.0f;

    //
    private Camera _ppCameraGO = null;
    private LayerMask _volumeLightLayer;
    private RenderTexture interleavedBuffer;
    private VLight[] _vlights;

    private Material _postMaterial;
    private Material PostMaterial
    {
        get
        {
            if (_postMaterial == null)
            {
                _postMaterial = new Material(postEffectShader);
                _postMaterial.hideFlags = HideFlags.HideAndDontSave;
            }
            return _postMaterial;
        }
    }

    private Material _downscaleDepthMaterial;
    private Material DownscaleDepthMaterial
    {
        get
        {
            if (_downscaleDepthMaterial == null)
            {
                _downscaleDepthMaterial = new Material(_downScaleDepthShader);
                _downscaleDepthMaterial.hideFlags = HideFlags.HideAndDontSave;
            }
            return _downscaleDepthMaterial;
        }
    }

    Camera _camera;
    Camera cam
    {
        get
        {
            if (_camera == null)
            {
                _camera = GetComponent<Camera>();
            }
            return _camera;
        }
    }

    private void OnEnable()
    {
        Init();
    }

    private void OnDisable()
    {
        CleanUp();
    }

    public static int renderCount = 0;

    //	private void OnGUI()
    //	{
    //		var bounds = new Bounds();
    //		var vlights = GameObject.FindObjectsOfType<VLight>();
    //		foreach(var vlight in vlights)
    //		{
    //			bounds.max = Camera.main.cameraToWorldMatrix.MultiplyPoint(vlight.MaxBounds);
    //			bounds.min = Camera.main.cameraToWorldMatrix.MultiplyPoint(vlight.MinBounds);
    //
    //			var rect = VLightGeometryUtil.BoundsToRect(bounds, Camera.main);
    //			rect.y = rect.y;
    //			GUI.Box(rect, "");
    //		}
    //	}

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        var cam = Camera.current;
        var planes = GeometryUtility.CalculateFrustumPlanes(cam);
        if (!Application.isPlaying)
        {
            _vlights = GameObject.FindObjectsOfType<VLight>();
        }

        if (lightsModified)
        {
            lightsModified = false;
            _vlights = GameObject.FindObjectsOfType<VLight>();
        }

        var bounds = new Bounds();
        var lightsVisible = false;
        var renderInterleaved = false;
        for (var i = 0; i < _vlights.Length; i++)
        {
            var vlight = _vlights[i];
            if (GeometryUtility.TestPlanesAABB(planes, vlight.MeshRender.bounds))
            {
                bounds.max = cam.cameraToWorldMatrix.MultiplyPoint(vlight.MaxBounds);
                bounds.min = cam.cameraToWorldMatrix.MultiplyPoint(vlight.MinBounds);
                var rect = VLightGeometryUtil.BoundsToRect(bounds, cam);
                var area = rect.width * rect.height;
                if (area > (minInterleavedRes * minInterleavedRes))
                {
                    renderInterleaved = true;
                }
                lightsVisible = true;
            }
        }

        if (!lightsVisible)
        {
            Graphics.Blit(source, destination);
            return;
        }

        var downsampleFactor = Mathf.Clamp(downSample, 1, 20);
        blurIterations = Mathf.Clamp(blurIterations, 0, 20);

        if (_useBilateralFiltering)
        {
            downsampleFactor = 2;
        }

        var width = cam.pixelWidth;
        var height = cam.pixelHeight;
        var dsWidth = cam.pixelWidth / downsampleFactor;
        var dsHeight = cam.pixelHeight / downsampleFactor;
        var frameBufferFormat = _useHighPrecisionFrameBuffer ? RenderTextureFormat.ARGBFloat : RenderTextureFormat.ARGB32;

#if UNITY_EDITOR
        if (!SystemInfo.SupportsRenderTextureFormat(frameBufferFormat))
        {
            frameBufferFormat = RenderTextureFormat.ARGBHalf;
            if (!SystemInfo.SupportsRenderTextureFormat(frameBufferFormat))
            {
                frameBufferFormat = RenderTextureFormat.ARGB32;
                Debug.LogWarning("Platform does not support floating point textures");
            }
        }
#endif

        // 4 samples for the interleaved buffer
        var bufferA = RenderTexture.GetTemporary(dsWidth, dsHeight, 0, frameBufferFormat);

        if (interleavedBuffer != null && (interleavedBuffer.width != width || interleavedBuffer.height != height))
        {
            if (Application.isPlaying)
            {
                Destroy(interleavedBuffer);
            }
            else
            {
                DestroyImmediate(interleavedBuffer);
            }
            interleavedBuffer = null;
        }

        if (interleavedBuffer == null)
        {
            interleavedBuffer = new RenderTexture(width, height, 0);

            interleavedBuffer.hideFlags = HideFlags.HideAndDontSave;
        }

        var ppCamera = GetPPCamera();
        ppCamera.CopyFrom(cam);
        ppCamera.enabled = false;
        ppCamera.depthTextureMode = DepthTextureMode.None;
        ppCamera.clearFlags = CameraClearFlags.SolidColor;
        ppCamera.cullingMask = _volumeLightLayer;
        ppCamera.useOcclusionCulling = false;
        ppCamera.backgroundColor = Color.clear;
        ppCamera.renderingPath = RenderingPath.VertexLit;

        renderingInterleaved = false;

        if (useInterleavedSampling && renderInterleaved)
        {
            var bufferB = RenderTexture.GetTemporary(dsWidth, dsHeight, 0, frameBufferFormat);
            var bufferC = RenderTexture.GetTemporary(dsWidth, dsHeight, 0, frameBufferFormat);
            var bufferD = RenderTexture.GetTemporary(dsWidth, dsHeight, 0, frameBufferFormat);

            // For odd projection matrices
            ppCamera.projectionMatrix = cam.projectionMatrix;
            ppCamera.pixelRect = new Rect(
                0,
                0,
                cam.pixelWidth / cam.rect.width + Screen.width / cam.rect.width,
                cam.pixelHeight / cam.rect.height + Screen.height / cam.rect.height);

            // Render the interleaved samples
            float offset = 0.0f;

            renderCount = 0;
            RenderSample(offset, ppCamera, bufferA);
            if (renderCount == 0)
            {
                Graphics.Blit(source, destination);
                RenderTexture.ReleaseTemporary(bufferA);
                RenderTexture.ReleaseTemporary(bufferB);
                RenderTexture.ReleaseTemporary(bufferC);
                RenderTexture.ReleaseTemporary(bufferD);
                return;
            }

            renderingInterleaved = true;

            offset += ditherOffset;
            RenderSample(offset, ppCamera, bufferB);
            offset += ditherOffset;
            RenderSample(offset, ppCamera, bufferC);
            offset += ditherOffset;
            RenderSample(offset, ppCamera, bufferD);

            //Combine the 4 samples to make an interleaved image and the edge border
            PostMaterial.SetTexture("_MainTexA", bufferA);
            PostMaterial.SetTexture("_MainTexB", bufferB);
            PostMaterial.SetTexture("_MainTexC", bufferC);
            PostMaterial.SetTexture("_MainTexD", bufferD);
            interleavedBuffer.DiscardContents();
            Graphics.Blit(null, interleavedBuffer, PostMaterial, 0);

            RenderTexture.ReleaseTemporary(bufferB);
            RenderTexture.ReleaseTemporary(bufferC);
            RenderTexture.ReleaseTemporary(bufferD);
        }
        else
        {
            ppCamera.projectionMatrix = cam.projectionMatrix;
            ppCamera.pixelRect = new Rect(
                0,
                0,
                cam.pixelWidth / cam.rect.width + Screen.width / cam.rect.width,
                cam.pixelHeight / cam.rect.height + Screen.height / cam.rect.height);

            renderCount = 0;
            RenderSample(0, ppCamera, bufferA);
            if (renderCount == 0)
            {
                Graphics.Blit(source, destination);
                RenderTexture.ReleaseTemporary(bufferA);
                return;
            }

            Graphics.Blit(bufferA, interleavedBuffer);
        }

        renderingInterleaved = false;


        var pingPong = RenderTexture.GetTemporary(width, height, 0);
        pingPong.DiscardContents();

        PostMaterial.SetFloat("_BlurSize", blurRadius);

        if (_useBilateralFiltering)
        {
            var quarterDepthTexture = RenderTexture.GetTemporary(dsWidth, dsHeight, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear, 1);
            //quarterDepthTexture.filterMode = FilterMode.Point;
            quarterDepthTexture.wrapMode = TextureWrapMode.Clamp;
            Graphics.Blit(source, quarterDepthTexture, DownscaleDepthMaterial);

            PostMaterial.SetFloat("BlurDepthFalloff", _blurDepth);
            PostMaterial.SetTexture("LowResDepthTexture", quarterDepthTexture);

            for (int i = 0; i < blurIterations; i++)
            {
                PostMaterial.SetVector("BlurDir", new Vector2(0, blurRadius));
                Graphics.Blit(interleavedBuffer, pingPong, PostMaterial, 4);

                PostMaterial.SetVector("BlurDir", new Vector2(blurRadius, 0));
                Graphics.Blit(pingPong, interleavedBuffer, PostMaterial, 4);

                PostMaterial.SetVector("BlurDir", new Vector2(0, blurRadius));
                Graphics.Blit(interleavedBuffer, pingPong, PostMaterial, 4);

                PostMaterial.SetVector("BlurDir", new Vector2(blurRadius, 0));
                Graphics.Blit(pingPong, interleavedBuffer, PostMaterial, 4);
            }

            RenderTexture.ReleaseTemporary(pingPong);
            RenderTexture.ReleaseTemporary(bufferA);

            PostMaterial.SetFloat("DepthThreshold", _depthThreshold);

            var interleavedBufferPoint = RenderTexture.GetTemporary(dsWidth, dsHeight, 0, RenderTextureFormat.ARGB32);
            Graphics.Blit(interleavedBuffer, interleavedBufferPoint);
            interleavedBufferPoint.filterMode = FilterMode.Point;

            PostMaterial.SetTexture("_MainTexBlurred", interleavedBuffer);
            PostMaterial.SetTexture("_MainTexBlurredPoint", interleavedBufferPoint);

            Graphics.Blit(source, destination, PostMaterial, 3);

            RenderTexture.ReleaseTemporary(interleavedBufferPoint);
            RenderTexture.ReleaseTemporary(quarterDepthTexture);
        }
        else
        {
            for (int i = 0; i < blurIterations; i++)
            {
                Graphics.Blit(interleavedBuffer, pingPong, PostMaterial, 1);
                interleavedBuffer.DiscardContents();
                Graphics.Blit(pingPong, interleavedBuffer, PostMaterial, 2);
                pingPong.DiscardContents();
            }

            PostMaterial.SetTexture("_MainTexBlurred", interleavedBuffer);
            Graphics.Blit(source, destination, PostMaterial, 5);

            RenderTexture.ReleaseTemporary(pingPong);
            RenderTexture.ReleaseTemporary(bufferA);
        }
    }

    private void RenderSample(float offset, Camera ppCamera, RenderTexture buffer)
    {
        Shader.SetGlobalFloat("_InterleavedOffset", offset);
        ppCamera.targetTexture = buffer;
        ppCamera.SetReplacementShader(volumeLightShader, "RenderType");
        ppCamera.Render();
    }

    private void Init()
    {
        if (LayerMask.NameToLayer(VLightManager.VOLUMETRIC_LIGHT_LAYER_NAME) == -1)
        {
            Debug.LogWarning(VLightManager.VOLUMETRIC_LIGHT_LAYER_NAME + " layer does not exist! Cannot use interleaved sampling please add this layer.");
            return;
        }

        if (!SystemInfo.supportsImageEffects)
        {
            Debug.LogWarning("Cannot use interleaved sampling. Image effects not supported");
            return;
        }

        _volumeLightLayer = 1 << LayerMask.NameToLayer(VLightManager.VOLUMETRIC_LIGHT_LAYER_NAME);

        cam.cullingMask &= ~_volumeLightLayer;

        cam.depthTextureMode |= DepthTextureMode.Depth;


        if (_downScaleDepthShader == null)
        {
            _downScaleDepthShader = Shader.Find(VLightShaderUtil.DOWNSCALEDEPTH_SHADER_NAME);
        }

        if (postEffectShader == null)
        {
            postEffectShader = Shader.Find(VLightShaderUtil.POST_SHADER_NAME);
        }

        if (volumeLightShader == null)
        {
            volumeLightShader = Shader.Find(VLightShaderUtil.INTERLEAVED_SHADER_NAME);
        }

        _vlights = GameObject.FindObjectsOfType<VLight>();
    }

    private void CleanUp()
    {
        cam.cullingMask |= _volumeLightLayer;
        if (Application.isEditor)
        {
            DestroyImmediate(_downscaleDepthMaterial);
            DestroyImmediate(_postMaterial);

            if (interleavedBuffer != null)
            {
                DestroyImmediate(interleavedBuffer);
            }
        }
        else
        {
            Destroy(_downscaleDepthMaterial);
            Destroy(_postMaterial);

            if (interleavedBuffer != null)
            {
                Destroy(interleavedBuffer);
            }
        }
    }

    private Camera GetPPCamera()
    {
        if (_ppCameraGO == null)
        {
            var go = GameObject.Find("Post Processing Camera");
            if (go != null && go.GetComponent<Camera>() != null)
            {
                _ppCameraGO = go.GetComponent<Camera>();
                _ppCameraGO.useOcclusionCulling = false;
            }
            else
            {
                var newGO = new GameObject("Post Processing Camera");
                _ppCameraGO = newGO.AddComponent<Camera>();
                _ppCameraGO.useOcclusionCulling = false;
                _ppCameraGO.enabled = false;
                newGO.hideFlags = HideFlags.HideAndDontSave;
            }
        }
        return _ppCameraGO;
    }
}

