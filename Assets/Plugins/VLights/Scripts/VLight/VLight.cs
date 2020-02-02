/*
 * VLight
 * Copyright Brian Su 2011-2019
*/
using System;
using UnityEngine;
using UnityEngine.Rendering;
using VLights;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
[RequireComponent(typeof(Camera)), RequireComponent(typeof(MeshRenderer)), RequireComponent(typeof(MeshFilter))]
[HelpURL("http://vlights-system.blogspot.com.au/p/documentation_15.html")]
public partial class VLight : MonoBehaviour
{
    const int GRADIENT_SIZE = 128;

    public enum VolumeShape
    {
        Cube = 0,
        Sphere = 1,
        RoundedCube = 2,
        Cylinder = 3,
    }

    public enum ShadowMode
    {
        None,
        Realtime,
        Baked
    }

    public enum LightTypes
    {
        Spot,
        Point,
        Area,
        Orthographic
    }

    [HideInInspector]
    public bool lockTransforms = false;
    [HideInInspector]
    [SerializeField]
    public bool renderWireFrame = false;

    [Space(20, order = 0)]
    [Header("==== General light settings ====", order = 1)]
    public LightTypes lightType;
    public float lightMultiplier = 1;
    public float spotExponent = 1;
    public float constantAttenuation = 1;
    public float linearAttenuation = 10;
    public float quadraticAttenuation = 100;
    public float aspect = 1;
    [Header("- only be changed when not playing -")]
    [Range(2, 200)]
    public int slices = 30;
    public Color colorTint = Color.white;
    [Tooltip("- scrolling noise and volume texture -")]
    public Vector3 noiseSpeed;
    [SerializeField]
    [Tooltip("- applies a dither pattern to reduce undersampling -")]
    bool useDithering = false;
    [SerializeField]
    [Range(-200, 200)]
    float ditherAmount = 0;
    [SerializeField]
    [Tooltip("- smooth intersection between geometry -")]
    bool useSoftBlend = false;
    [SerializeField]
    [Tooltip("- control the falloff of the light using a curve -")]
    bool useCurves;
    [SerializeField]
    [Header("- amount to shift noise when light moves -")]
    float worldScrollAmount = 0;
    [Space(20, order = 0)]
    [Header("==== Shadow settings ====", order = 1)]
    public ShadowMode shadowMode;
    [Range(8, 2048)]
    [SerializeField]
    [Tooltip("- This must be a power of 2 -")]
    int shadowMapRes = 256;
    [SerializeField]
    int shadowBlurPasses = 0;
    [SerializeField]
    float shadowBlurSize = 0;
    [SerializeField]
    [Header("- for special objects like speed tree -")]
    bool renderFullShadows = false;
    [SerializeField]
    [Header("- enable if shadow artifacts occur -")]
    private bool _renderShadowMapInUpdate = false;

    [Space(20, order = 0)]
    [Header("==== Spot/Orthographic light settings ====", order = 1)]
    public float spotRange = 1;
    public float spotNear = 0.1f;
    public float spotAngle = 45;
    public float orthoSize = 0.5f;
    [SerializeField]
    Texture spotEmission;
    [SerializeField]
    Texture spotNoise;
    [SerializeField]
    Texture spotShadow;
    [Space(20, order = 0)]
    [Header("==== Point light settings ====", order = 1)]
    public float pointLightRadius = 1;
    [SerializeField]
    Cubemap pointEmission;
    [SerializeField]
    Cubemap pointNoise;
    [SerializeField]
    Texture pointShadow;
    [Space(20, order = 0)]
    [Header("==== Area volume settings ====", order = 1)]
    [SerializeField]
    Texture3D areaVolume;
    [SerializeField]
    VolumeShape volumeShape = VolumeShape.Cube;
    [SerializeField]
    [Range(0, 1)]
    float shapeValue = 0;
    [SerializeField]
    Vector3 volumeTextureOffset = Vector3.zero;
    [SerializeField]
    float volumeTextureScale = 0.5f;

    [SerializeField]
    [Header("- only changed when not playing -")]
    Gradient lightGradient = new Gradient()
    {
        alphaKeys = new GradientAlphaKey[] { new GradientAlphaKey(1, 0), new GradientAlphaKey(1, 1) },
        colorKeys = new GradientColorKey[] { new GradientColorKey(Color.white, 0), new GradientColorKey(Color.black, 1) }
    };
    [SerializeField]
    AnimationCurve fallOffCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [SerializeField]
    private Vector3 _boundsCentreOffset = Vector3.zero;

    [SerializeField]
    [HideInInspector]
    Texture2D _fallOffTexture;

    [SerializeField]
    [HideInInspector]
    Material spotMaterial;
    [SerializeField]
    [HideInInspector]
    Material pointMaterial;
    [SerializeField]
    [HideInInspector]
    Material areaMaterial;
    [SerializeField]
    [HideInInspector]
    Material orthoMaterial;
    [SerializeField]
    [HideInInspector]
    Shader renderDepthShader;
    [HideInInspector]
    [SerializeField]
    Mesh meshContainer;

    int _idColorTint = 0;
    int _idLightMultiplier = 0;
    int _idSpotExponent = 0;
    int _idConstantAttenuation = 0;
    int _idLinearAttenuation = 0;
    int _idQuadraticAttenuation = 0;
    int _idLightParams = 0;
    int _idMinBounds = 0;
    int _idMaxBounds = 0;
    int _idViewWorldLight = 0;
    int _idRotation = 0;
    int _idLocalRotation = 0;
    int _idProjection = 0;
    int _idNoiseOffset = 0;
    int _idJitterAmount = 0;
    int _idFallOffTex = 0;
    int _idDitherTex = 0;
    int _idVolumeParams = 0;
    int _idVolumeOffset = 0;

    LightTypes _prevLightType;
    ShadowMode _prevShadowMode;
    bool _prevRenderFullShadows;
    int _prevSlices;
    bool _frustrumSwitch;
    bool _prevIsOrtho;
    float _prevNear;
    float _prevFar;
    float _prevFov;
    float _prevOrthoSize;
    float _prevPointLightRadius;
    float _prevOrtho;
    float _prevOrthoAspect;
    Vector3 _prevBoundsCentreOffset = Vector3.zero;
    Matrix4x4 _worldToCamera;
    Matrix4x4 _projectionMatrixCached;
    Matrix4x4 _viewWorldToCameraMatrixCached;
    Matrix4x4 _viewCameraToWorldMatrixCached;
    Matrix4x4 _localToWorldMatrix;
    Matrix4x4 _rotation;
    Matrix4x4 _localRotation;
    Matrix4x4 _viewWorldLight;
    Vector3[] _frustrumPoints;
    Vector3 _angle = Vector3.zero;
    Vector3 _minBounds, _maxBounds;
    bool _cameraHasBeenUpdated = false;
    MeshFilter _meshFilter;
    RenderTexture _depthTexture;
    const int VERT_COUNT = 65000;
    const int TRI_COUNT = VERT_COUNT * 3;
    const System.StringComparison STR_CMP_TYPE = System.StringComparison.OrdinalIgnoreCase;
    bool _builtMesh = false;
    int _maxSlices;
    Material _postMaterial;

    public Vector3 MinBounds
    {
        get { return _minBounds; }
    }

    public Vector3 MaxBounds
    {
        get { return _maxBounds; }
    }

    Material LightMaterial
    {
        get
        {
            if (MeshRender.sharedMaterial == null)
            {
                CreateMaterials();
            }
            return MeshRender.sharedMaterial;
        }
    }

    Material PostMaterial
    {
        get
        {
            if (_postMaterial == null)
            {
                _postMaterial = new Material(Shader.Find("Hidden/V-Light/Post"));
                _postMaterial.hideFlags = HideFlags.HideAndDontSave;
            }
            return _postMaterial;
        }
    }

    int MaxSlices
    {
        get { return _maxSlices; }
        set { _maxSlices = value; }
    }

    static Cubemap _emptyCubemap;
    static Cubemap EmptyCubemap
    {
        get
        {
            if (_emptyCubemap == null)
            {
                _emptyCubemap = new Cubemap(1, TextureFormat.ARGB32, false);
                _emptyCubemap.hideFlags = HideFlags.DontSave;
                _emptyCubemap.SetPixel(CubemapFace.NegativeX, 0, 0, Color.gray);
                _emptyCubemap.SetPixel(CubemapFace.NegativeY, 0, 0, Color.gray);
                _emptyCubemap.SetPixel(CubemapFace.NegativeZ, 0, 0, Color.gray);
                _emptyCubemap.SetPixel(CubemapFace.PositiveX, 0, 0, Color.gray);
                _emptyCubemap.SetPixel(CubemapFace.PositiveY, 0, 0, Color.gray);
                _emptyCubemap.SetPixel(CubemapFace.PositiveZ, 0, 0, Color.gray);
                _emptyCubemap.Apply();
            }
            return _emptyCubemap;
        }
    }

    static Texture3D _emptyTexture3D;
    static Texture3D EmptyTexture3D
    {
        get
        {
            if (_emptyTexture3D == null)
            {
                _emptyTexture3D = new Texture3D(1, 1, 1, TextureFormat.ARGB32, false);
                _emptyTexture3D.SetPixels32(new Color32[] { new Color32(128, 128, 128, 128) });
                _emptyTexture3D.hideFlags = HideFlags.DontSave;
                _emptyTexture3D.Apply();
            }
            return _emptyTexture3D;
        }
    }

    static Texture2D _emptyTexture2D;
    static Texture2D EmptyTexture2D
    {
        get
        {
            if (_emptyTexture2D == null)
            {
                _emptyTexture2D = new Texture2D(1, 1, TextureFormat.ARGB32, false);
                _emptyTexture2D.SetPixels32(new Color32[] { new Color32(128, 128, 128, 128) });
                _emptyTexture2D.hideFlags = HideFlags.DontSave;
                _emptyTexture2D.Apply();
            }
            return _emptyTexture2D;
        }
    }

    static Texture2D _ditherTexture;
    static Texture2D DitherTexture
    {
        get
        {
            if (_ditherTexture == null)
            {
                _ditherTexture = new Texture2D(8, 8, TextureFormat.ARGB32, false, true);
                _ditherTexture.wrapMode = TextureWrapMode.Repeat;
                _ditherTexture.hideFlags = HideFlags.DontSave;
                var bayer = new float[64] {
                     0, 32, 8, 40, 2, 34, 10, 42,   /* 8x8 Bayer ordered dithering */
                    48, 16, 56, 24, 50, 18, 58, 26, /* pattern. Each input pixel */
                    12, 44, 4, 36, 14, 46, 6, 38,   /* is scaled to the 0..63 range */
                    60, 28, 52, 20, 62, 30, 54, 22, /* before looking in this table */
                     3, 35, 11, 43, 1, 33, 9, 41,   /* to determine the action. */
                    51, 19, 59, 27, 49, 17, 57, 25,
                    15, 47, 7, 39, 13, 45, 5, 37,
                    63, 31, 55, 23, 61, 29, 53, 21 };

                var color = new Color[64];
                for (int i = 0; i < bayer.Length; i++)
                {
                    color[i] = new Color(bayer[i] / 64.0f, bayer[i] / 64.0f, bayer[i] / 64.0f, 1);
                }
                _ditherTexture.SetPixels(color);
                _ditherTexture.Apply();
            }
            return _ditherTexture;
        }
    }

    Texture2D FallOffTexture
    {
        get
        {
            if (_fallOffTexture == null)
            {
                _fallOffTexture = new Texture2D(GRADIENT_SIZE, 1);
                _fallOffTexture.wrapMode = TextureWrapMode.Clamp;
                _fallOffTexture.hideFlags = HideFlags.DontSave;
                UpdateFalloffCurve();
            }
            return _fallOffTexture;
        }
    }

    public Shader RenderDepthShader
    {
        get
        {
            if (renderDepthShader == null)
            {
                renderDepthShader = Shader.Find(VLightShaderUtil.DEPTH_SHADER_NAME);
            }
            return renderDepthShader;
        }
    }

    MaterialPropertyBlock _propertyBlock;
    public MaterialPropertyBlock PropertyBlock
    {
        get
        {
            if (_propertyBlock == null)
            {
                _propertyBlock = new MaterialPropertyBlock();
            }
            return _propertyBlock;
        }
    }

    Renderer _renderer;
    public Renderer MeshRender
    {
        get
        {
            if (_renderer == null)
            {
                _renderer = GetComponent<Renderer>();
            }
            return _renderer;
        }
    }

    Camera _camera;
    public Camera cam
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

    Transform _cachedTransform;
    Transform CachedTransform
    {
        get
        {
            if (_cachedTransform == null)
            {
                _cachedTransform = transform;
            }
            return _cachedTransform;
        }
    }

    void OnEnable()
    {
#if DEBUG_MODE
        Debug.Log("Enable V-light");
#endif
        _maxSlices = slices;

        int layer = LayerMask.NameToLayer(VLightManager.VOLUMETRIC_LIGHT_LAYER_NAME);
        if (layer != -1)
        {
            gameObject.layer = layer;
        }

        cam.enabled = false;
        cam.cullingMask &= ~(1 << gameObject.layer);

        VLightInterleavedSampling.lightsModified = true;

        _queueRenderShadowMap = true;
    }

    void OnDisable()
    {
        VLightInterleavedSampling.lightsModified = true;
    }

    void OnApplicationQuit()
    {
#if DEBUG_MODE
        Debug.Log("App Quit V-light");
#endif
    }

    void OnDestroy()
    {
#if DEBUG_MODE
        Debug.Log("Destroy V-light");
#endif
        CleanMaterials();
        SafeDestroy(meshContainer, true);
        SafeDestroy(_depthTexture, true);
        SafeDestroy(_fallOffTexture, true);
        SafeDestroy(_ditherTexture, true);
        SafeDestroy(_emptyTexture2D, true);
        SafeDestroy(_emptyTexture3D, true);
        SafeDestroy(_emptyCubemap, true);
    }

    void Start()
    {
#if DEBUG_MODE
        Debug.Log("Start V-light");
#endif

        CreateMaterials();

        // Force build so we get a onwillrender
        UpdateLightMatrices();
        BuildMesh(false, slices, Vector3.one, Vector3.one);

#if UNITY_EDITOR
        EditorUtility.SetSelectedRenderState(MeshRender, renderWireFrame ? EditorSelectedRenderState.Wireframe : EditorSelectedRenderState.Hidden);
#endif
    }

    void Reset()
    {
#if DEBUG_MODE
        Debug.Log("Reset V-light");
#endif
        CleanMaterials();
        SafeDestroy(_emptyTexture2D);
        SafeDestroy(_emptyTexture3D);
        SafeDestroy(_emptyCubemap);
        SafeDestroy(_depthTexture);
        SafeDestroy(_fallOffTexture);
        SafeDestroy(_ditherTexture);
        SafeDestroy(meshContainer);
    }

    void CreateMaterials()
    {
        _idColorTint = Shader.PropertyToID("_Color");
        _idLightMultiplier = Shader.PropertyToID("_Strength");
        _idSpotExponent = Shader.PropertyToID("_SpotExp");
        _idConstantAttenuation = Shader.PropertyToID("_ConstantAttn");
        _idLinearAttenuation = Shader.PropertyToID("_LinearAttn");
        _idQuadraticAttenuation = Shader.PropertyToID("_QuadAttn");
        _idLightParams = Shader.PropertyToID("_LightParams");
        _idMinBounds = Shader.PropertyToID("_minBounds");
        _idMaxBounds = Shader.PropertyToID("_maxBounds");
        _idViewWorldLight = Shader.PropertyToID("_ViewWorldLight");
        _idLocalRotation = Shader.PropertyToID("_LocalRotation");
        _idRotation = Shader.PropertyToID("_Rotation");
        _idProjection = Shader.PropertyToID("_Projection");
        _idNoiseOffset = Shader.PropertyToID("_NoiseOffset");
        _idJitterAmount = Shader.PropertyToID("_JitterAmount");
        _idDitherTex = Shader.PropertyToID("_DitherTex");
        _idFallOffTex = Shader.PropertyToID("_FallOffTex");
        _idVolumeOffset = Shader.PropertyToID("_VolumeOffset");
        _idVolumeParams = Shader.PropertyToID("_VolumeParams");

        var mat = MeshRender.sharedMaterial;

        if (mat != null)
        {
#if UNITY_EDITOR
            if (mat.shader.name == "V-Light/Spot" || mat.shader.name == "V-Light/Point")
            {
                colorTint = mat.GetColor("_Color");
                spotExponent = mat.GetFloat("_SpotExp");
                constantAttenuation = mat.GetFloat("_ConstantAttn");
                linearAttenuation = mat.GetFloat("_LinearAttn");
                quadraticAttenuation = mat.GetFloat("_QuadAttn");
            }

#endif

            SafeDestroy(MeshRender.sharedMaterial);
        }

        if (pointMaterial == null)
        {
            pointMaterial = new Material(Shader.Find("V-Light/Point Light 2"));
            pointMaterial.hideFlags = HideFlags.DontSave;
        }

        if (spotMaterial == null)
        {
            spotMaterial = new Material(Shader.Find("V-Light/Spot Light 2"));
            spotMaterial.hideFlags = HideFlags.DontSave;
        }

        if (areaMaterial == null)
        {
            areaMaterial = new Material(Shader.Find("V-Light/Area"));
            areaMaterial.hideFlags = HideFlags.DontSave;
        }
        if (orthoMaterial == null)
        {
            orthoMaterial = new Material(Shader.Find("V-Light/Orthographic"));
            orthoMaterial.hideFlags = HideFlags.DontSave;
        }

        switch (lightType)
        {
            case LightTypes.Spot:
                MeshRender.sharedMaterial = Instantiate(spotMaterial) as Material;
                break;
            case LightTypes.Point:
                MeshRender.sharedMaterial = Instantiate(pointMaterial) as Material;
                break;
            case LightTypes.Area:
                MeshRender.sharedMaterial = Instantiate(areaMaterial) as Material;
                break;
            case LightTypes.Orthographic:
                MeshRender.sharedMaterial = Instantiate(orthoMaterial) as Material;
                break;
        }

        MeshRender.sharedMaterial.hideFlags = HideFlags.DontSave;
    }

    void CleanMaterials()
    {
        SafeDestroy(meshContainer);
        SafeDestroy(MeshRender.sharedMaterial);
        meshContainer = null;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(MeshRender.bounds.center, 0.05f);

        if (_frustrumPoints == null)
        {
            return;
        }

        Gizmos.color = new Color(0, 1, 0, 0.2f);

        Gizmos.DrawLine(CachedTransform.TransformPoint(_frustrumPoints[0]), CachedTransform.TransformPoint(_frustrumPoints[1]));
        Gizmos.DrawLine(CachedTransform.TransformPoint(_frustrumPoints[2]), CachedTransform.TransformPoint(_frustrumPoints[3]));
        Gizmos.DrawLine(CachedTransform.TransformPoint(_frustrumPoints[4]), CachedTransform.TransformPoint(_frustrumPoints[5]));
        Gizmos.DrawLine(CachedTransform.TransformPoint(_frustrumPoints[6]), CachedTransform.TransformPoint(_frustrumPoints[7]));

        Gizmos.DrawLine(CachedTransform.TransformPoint(_frustrumPoints[1]), CachedTransform.TransformPoint(_frustrumPoints[3]));
        Gizmos.DrawLine(CachedTransform.TransformPoint(_frustrumPoints[3]), CachedTransform.TransformPoint(_frustrumPoints[7]));
        Gizmos.DrawLine(CachedTransform.TransformPoint(_frustrumPoints[7]), CachedTransform.TransformPoint(_frustrumPoints[5]));
        Gizmos.DrawLine(CachedTransform.TransformPoint(_frustrumPoints[5]), CachedTransform.TransformPoint(_frustrumPoints[1]));

        Gizmos.DrawLine(CachedTransform.TransformPoint(_frustrumPoints[0]), CachedTransform.TransformPoint(_frustrumPoints[2]));
        Gizmos.DrawLine(CachedTransform.TransformPoint(_frustrumPoints[2]), CachedTransform.TransformPoint(_frustrumPoints[6]));
        Gizmos.DrawLine(CachedTransform.TransformPoint(_frustrumPoints[6]), CachedTransform.TransformPoint(_frustrumPoints[4]));
        Gizmos.DrawLine(CachedTransform.TransformPoint(_frustrumPoints[4]), CachedTransform.TransformPoint(_frustrumPoints[0]));

    }

    [HideInInspector]
    Vector3[] _pointsViewSpace = new Vector3[8]; // cached these values

    void CalculateMinMax(out Vector3 min, out Vector3 max, bool forceFrustrumUpdate)
    {
        if (_frustrumPoints == null || forceFrustrumUpdate)
        {
            if(lightType == LightTypes.Point || lightType == LightTypes.Area)
            {
                VLightGeometryUtil.RecalculateFrustrumPoints(cam.orthographic, pointLightRadius, spotAngle, -pointLightRadius, pointLightRadius, aspect, out _frustrumPoints);
            }
            else
            {
                VLightGeometryUtil.RecalculateFrustrumPoints(cam.orthographic, orthoSize, spotAngle, Mathf.Max(0.01f, spotNear), spotRange, aspect, out _frustrumPoints);
            }
        }

        Vector3 vecMinBounds = new Vector3(Mathf.NegativeInfinity, Mathf.NegativeInfinity, Mathf.NegativeInfinity);
        Vector3 vecMaxBounds = new Vector3(Mathf.Infinity, Mathf.Infinity, Mathf.Infinity);

        Matrix4x4 minMaxMatrix = _viewWorldToCameraMatrixCached * _localToWorldMatrix;

        for (int i = 0; i < _frustrumPoints.Length; i++)
        {
            _pointsViewSpace[i] = minMaxMatrix.MultiplyPoint3x4(_frustrumPoints[i]);

            vecMinBounds.x = (vecMinBounds.x > _pointsViewSpace[i].x) ? vecMinBounds.x : _pointsViewSpace[i].x;
            vecMinBounds.y = (vecMinBounds.y > _pointsViewSpace[i].y) ? vecMinBounds.y : _pointsViewSpace[i].y;
            vecMinBounds.z = (vecMinBounds.z > _pointsViewSpace[i].z) ? vecMinBounds.z : _pointsViewSpace[i].z;

            vecMaxBounds.x = (vecMaxBounds.x <= _pointsViewSpace[i].x) ? vecMaxBounds.x : _pointsViewSpace[i].x;
            vecMaxBounds.y = (vecMaxBounds.y <= _pointsViewSpace[i].y) ? vecMaxBounds.y : _pointsViewSpace[i].y;
            vecMaxBounds.z = (vecMaxBounds.z <= _pointsViewSpace[i].z) ? vecMaxBounds.z : _pointsViewSpace[i].z;
        }

        min = vecMinBounds;
        max = vecMaxBounds;
    }

    Matrix4x4 CalculateProjectionMatrix()
    {
        float fov = cam.fieldOfView;
        float near = cam.nearClipPlane;
        float far = cam.farClipPlane;

        fov = spotAngle;
        near = Mathf.Max(0.01f, spotNear);
        far = spotRange;

        cam.farClipPlane = far;

        if(lightType == LightTypes.Point || lightType == LightTypes.Area)
        {
            near = -pointLightRadius;
            far = pointLightRadius;
        }

        Matrix4x4 projectionMatrix;
        if (!cam.orthographic)
        {
            projectionMatrix = Matrix4x4.Perspective(fov, aspect, near, far);
        }
        else
        {
            float halfOrtho = orthoSize;
            projectionMatrix = Matrix4x4.Ortho(
                -halfOrtho * aspect,
                halfOrtho * aspect,
                -halfOrtho,
                halfOrtho,
                near, far);
        }
        return projectionMatrix;
    }

    //	[SerializeField]
    //	private AnimationCurve _planeDistribution = AnimationCurve.Linear(0, 0, 1, 1);

    void BuildMesh(bool manualPositioning, int planeCount, Vector3 minBounds, Vector3 maxBounds)
    {
        if (meshContainer == null || meshContainer.name.IndexOf(GetInstanceID().ToString(), System.StringComparison.OrdinalIgnoreCase) != 0)
        {
#if DEBUG_MODE
            Debug.Log("Creating new mesh container");
#endif
            meshContainer = new Mesh();
            meshContainer.MarkDynamic();
            meshContainer.hideFlags = HideFlags.HideAndDontSave;
            meshContainer.name = GetInstanceID().ToString();
        }

        if (_meshFilter == null)
        {
            _meshFilter = GetComponent<MeshFilter>();
        }

        Vector3[] vertBucket = new Vector3[VERT_COUNT];
        int[] triBucket = new int[TRI_COUNT];
        int vertBucketCount = 0;
        int triBucketCount = 0;

        float depthOffset = 1.0f / (float)(planeCount - 1);
        float depth = (manualPositioning) ? 1f : 0f;
        float xLeft = 0f;
        float xRight = 1f;
        float xBottom = 0f;
        float xTop = 1f;

        int vertOffset = 0;
        for (int i = 0; i < planeCount; i++)
        {
            Vector3[] verts = new Vector3[4];
            Vector3[] results;

            if (manualPositioning)
            {
                Plane[] planes = GeometryUtility.CalculateFrustumPlanes(_projectionMatrixCached * cam.worldToCameraMatrix);

                for (int j = 0; j < planes.Length; j++)
                {
                    Vector3 centre = planes[j].normal * -planes[j].distance;
                    planes[j] = new Plane(_viewWorldToCameraMatrixCached.MultiplyVector(planes[j].normal), _viewWorldToCameraMatrixCached.MultiplyPoint3x4(centre));
                }

                verts[0] = CalculateTriLerp(new Vector3(xLeft, xBottom, depth), minBounds, maxBounds);
                verts[1] = CalculateTriLerp(new Vector3(xLeft, xTop, depth), minBounds, maxBounds);
                verts[2] = CalculateTriLerp(new Vector3(xRight, xTop, depth), minBounds, maxBounds);
                verts[3] = CalculateTriLerp(new Vector3(xRight, xBottom, depth), minBounds, maxBounds);
                results = VLightGeometryUtil.ClipPolygonAgainstPlane(verts, planes);
            }
            else
            {
                //				var dp = _planeDistribution.Evaluate(depth);
                var dp = depth;
                verts[0] = new Vector3(xLeft, xBottom, dp);
                verts[1] = new Vector3(xLeft, xTop, dp);
                verts[2] = new Vector3(xRight, xTop, dp);
                verts[3] = new Vector3(xRight, xBottom, dp);
                results = verts;
            }

            depth += (manualPositioning) ? -depthOffset : depthOffset;

            if (results.Length > 2)
            {
                Array.Copy(results, 0, vertBucket, vertBucketCount, results.Length);
                vertBucketCount += results.Length;

                int[] tris = new int[(results.Length - 2) * 3];
                int vertOff = 0;
                for (int j = 0; j < tris.Length; j += 3)
                {
                    tris[j + 0] = vertOffset + 0;
                    tris[j + 1] = vertOffset + (vertOff + 1);
                    tris[j + 2] = vertOffset + (vertOff + 2);
                    vertOff++;
#if DEBUG_MODE
                    Color lightBlue = new Color(0, 0, 1, 0.05f);
                    Matrix4x4 cameraToWorld = _viewCameraToWorldMatrixCached;
                    Debug.DrawLine(cameraToWorld.MultiplyPoint(vertBucket[tris[j + 0]]), cameraToWorld.MultiplyPoint(vertBucket[tris[j + 1]]), lightBlue);
                    Debug.DrawLine(cameraToWorld.MultiplyPoint(vertBucket[tris[j + 1]]), cameraToWorld.MultiplyPoint(vertBucket[tris[j + 2]]), lightBlue);
                    Debug.DrawLine(cameraToWorld.MultiplyPoint(vertBucket[tris[j + 2]]), cameraToWorld.MultiplyPoint(vertBucket[tris[j + 0]]), lightBlue);
#endif
                }
                vertOffset += results.Length;
                Array.Copy(tris, 0, triBucket, triBucketCount, tris.Length);
                triBucketCount += tris.Length;
            }
        }
        meshContainer.Clear();

        Vector3[] newVerts = new Vector3[vertBucketCount];
        Array.Copy(vertBucket, newVerts, vertBucketCount);
        meshContainer.vertices = newVerts;

        int[] newTris = new int[triBucketCount];
        Array.Copy(triBucket, newTris, triBucketCount);
        meshContainer.triangles = newTris;
        meshContainer.normals = new Vector3[vertBucketCount];
        meshContainer.uv = new Vector2[vertBucketCount];

        Vector3 centrePT = Vector3.zero;
        foreach (var vert in _frustrumPoints)
        {
            centrePT += vert;
        }
        centrePT /= _frustrumPoints.Length;

        Bounds localBounds = new Bounds(centrePT, Vector3.zero);
        foreach (var vert in _frustrumPoints)
        {
            localBounds.Encapsulate(vert);
        }

        _meshFilter.sharedMesh = meshContainer;
        localBounds.center += _boundsCentreOffset;
        _meshFilter.sharedMesh.bounds = localBounds;
    }


    Vector3 CalculateTriLerp(Vector3 vertex, Vector3 minBounds, Vector3 maxBounds)
    {
        Vector3 triLerp = new Vector3(1, 1, 1) - vertex;
        Vector3 result =
            new Vector3(minBounds.x * vertex.x, minBounds.y * vertex.y, maxBounds.z * vertex.z) +
            new Vector3(maxBounds.x * triLerp.x, maxBounds.y * triLerp.y, minBounds.z * triLerp.z);
        return result;
    }

    int _waterLayer = -1;
    int WaterLayer
    {
        get
        {
            if (_waterLayer == -1)
            {
                _waterLayer = 1 << LayerMask.NameToLayer("Water");
            }
            return _waterLayer;
        }
    }

    int _vlightLayer = -1;
    int VLightLayer
    {
        get
        {
            if (_vlightLayer == -1)
            {
                _vlightLayer = 1 << LayerMask.NameToLayer(VLightManager.VOLUMETRIC_LIGHT_LAYER_NAME);
            }
            return _vlightLayer;
        }
    }


    public void RenderShadowMap()
    {
        float far = cam.farClipPlane;

        switch (shadowMode)
        {
            case ShadowMode.None:
                cam.depthTextureMode = DepthTextureMode.None;
                break;
            case ShadowMode.Baked:
                cam.depthTextureMode = DepthTextureMode.None;
                break;
            case ShadowMode.Realtime:
                if (SystemInfo.supportsImageEffects)
                {
                    cam.backgroundColor = Color.white;
                    cam.clearFlags = CameraClearFlags.SolidColor;
                    cam.renderingPath = RenderingPath.VertexLit;
                    //prevent any recursive rendering
                    cam.cullingMask &= ~(WaterLayer | VLightLayer);
                    cam.depthTextureMode = renderFullShadows ? DepthTextureMode.Depth : DepthTextureMode.None;

                    CreateDepthTexture(lightType);

                    if (RenderDepthShader != null)
                    {
                        switch (lightType)
                        {
                            case LightTypes.Spot:
                            case LightTypes.Orthographic:
                                cam.targetTexture = _depthTexture;
                                cam.projectionMatrix = CalculateProjectionMatrix();
                                if (renderFullShadows)
                                {
                                    cam.Render();
                                    Graphics.Blit(null, _depthTexture, PostMaterial, 6);
                                }
                                else
                                {
                                    cam.RenderWithShader(RenderDepthShader, "RenderType");
                                }

                                if (shadowBlurPasses > 0)
                                {
                                    //Blur the result
                                    var pingPong = RenderTexture.GetTemporary(shadowMapRes, shadowMapRes, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
                                    pingPong.DiscardContents();
                                    PostMaterial.SetFloat("_BlurSize", shadowBlurSize);
                                    for (int i = 0; i < shadowBlurPasses; i++)
                                    {
                                        Graphics.Blit(_depthTexture, pingPong, PostMaterial, 1);
                                        _depthTexture.DiscardContents();
                                        Graphics.Blit(pingPong, _depthTexture, PostMaterial, 2);
                                        pingPong.DiscardContents();
                                    }

                                    RenderTexture.ReleaseTemporary(pingPong);
                                }
                                break;
                            case LightTypes.Point:
                                cam.projectionMatrix = Matrix4x4.Perspective(90, 1.0f, 0.1f, pointLightRadius);
                                cam.SetReplacementShader(RenderDepthShader, "RenderType");
                                cam.RenderToCubemap(_depthTexture, 63);
                                cam.ResetReplacementShader();
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        Debug.LogWarning("Could not find depth shader. Cannot render shadows");
                    }
                }
                break;
        }
    }

    RenderTexture GenerateShadowMap(int res)
    {
        #if UNITY_IPHONE || UNITY_ANDROID
        return new RenderTexture(res, res, 16, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
        #else
        return new RenderTexture(res, res, 16, RenderTextureFormat.RFloat, RenderTextureReadWrite.Linear);
        #endif
    }

    void CreateDepthTexture(LightTypes type)
    {
        shadowMapRes = Mathf.NextPowerOfTwo(shadowMapRes);
        if (_depthTexture == null || _depthTexture.width != shadowMapRes)
        {
#if DEBUG_MODE
            Debug.Log("Creating new depth texture");
#endif
            if (_depthTexture != null)
            {
                SafeDestroy(_depthTexture);
            }
            _depthTexture = GenerateShadowMap(shadowMapRes);
            _depthTexture.hideFlags = HideFlags.HideAndDontSave;
            _depthTexture.isPowerOfTwo = true;
            switch (type)
            {
                case LightTypes.Point:
                    _depthTexture.dimension = TextureDimension.Cube;
                    break;
            }
        }
        else if (type == LightTypes.Point && (_depthTexture.dimension != TextureDimension.Cube) && _depthTexture.IsCreated())
        {
#if DEBUG_MODE
            Debug.Log("Swapping to cubemap depth texture");
#endif
            SafeDestroy(_depthTexture);
            _depthTexture = GenerateShadowMap(shadowMapRes);

            _depthTexture.hideFlags = HideFlags.HideAndDontSave;
            _depthTexture.isPowerOfTwo = true;
            _depthTexture.dimension = TextureDimension.Cube;
        }
        else if ((type == LightTypes.Spot || type == LightTypes.Orthographic) && (_depthTexture.dimension == TextureDimension.Cube) && _depthTexture.IsCreated())
        {
#if DEBUG_MODE
            Debug.Log("Swapping to non cubemap depth texture");
#endif
            SafeDestroy(_depthTexture);
            _depthTexture = GenerateShadowMap(shadowMapRes);
            _depthTexture.hideFlags = HideFlags.HideAndDontSave;
            _depthTexture.isPowerOfTwo = true;
            _depthTexture.dimension = TextureDimension.Tex2D;
        }
    }

#if DEBUG_MODE
    bool _hasCalledUpdate = false;
#endif

    private bool _queueRenderShadowMap = false;

    public void OnWillRenderObject()
    {
        VLightInterleavedSampling.renderCount++;

        if (!VLightInterleavedSampling.renderingInterleaved)
        {
            UpdateSettings();
            UpdateViewMatrices(Camera.current);
            UpdateLightMatrices();
            if(!_renderShadowMapInUpdate)
            {
                RenderShadowMap();
            }

            _queueRenderShadowMap = true;

            MeshRender.GetPropertyBlock(PropertyBlock);
            SetShaderPropertiesBlock(PropertyBlock);
            MeshRender.SetPropertyBlock(PropertyBlock);
        }

        if (useCurves)
        {
            if (Application.isEditor)
            {
                UpdateFalloffCurve();
            }
        }
    }

    void Update()
    {
        if (useCurves)
        {
            if (Application.isEditor)
            {
                UpdateFalloffCurve();
            }
        }
        else
        {
            SafeDestroy(_fallOffTexture);
        }

        if (_queueRenderShadowMap)
        {
            _queueRenderShadowMap = false;
            if(_renderShadowMapInUpdate)
            {
                RenderShadowMap();
            }
        }

        UpdateSettings();
        UpdateLightMatrices();
    }
    // fix for UFPS need to find something better
    void FixedUpdate()
    {
        if (useCurves)
        {
            if (Application.isEditor)
            {
                UpdateFalloffCurve();
            }
        }
        else
        {
            SafeDestroy(_fallOffTexture);
        }

        if (_queueRenderShadowMap)
        {
            _queueRenderShadowMap = false;
        }

        UpdateSettings();
        UpdateLightMatrices();
    }

    void UpdateFalloffCurve()
    {
        var colors = new Color[GRADIENT_SIZE];
        for (var i = 0; i < GRADIENT_SIZE; i++)
        {
            colors[i] = lightGradient.Evaluate(i / (float)GRADIENT_SIZE);
            colors[i].a *= fallOffCurve.Evaluate(i / (float)GRADIENT_SIZE);
        }
        FallOffTexture.SetPixels(colors);        
        FallOffTexture.Apply();
    }

    bool CameraHasBeenUpdated()
    {
        bool hasBeenUpdated = false;
        hasBeenUpdated |= _meshFilter == null || _meshFilter.sharedMesh == null;
        hasBeenUpdated |= spotRange != _prevFar;
        hasBeenUpdated |= spotNear != _prevNear;
        hasBeenUpdated |= spotAngle != _prevFov;
        hasBeenUpdated |= cam.orthographicSize != _prevOrthoSize;
        hasBeenUpdated |= cam.orthographic != _prevIsOrtho;
        hasBeenUpdated |= pointLightRadius != _prevPointLightRadius;
        hasBeenUpdated |= orthoSize != _prevOrtho;
        hasBeenUpdated |= aspect != _prevOrthoAspect;
        hasBeenUpdated |= _prevSlices != slices;
        hasBeenUpdated |= _prevShadowMode != shadowMode;
        hasBeenUpdated |= _prevLightType != lightType;
        hasBeenUpdated |= _prevRenderFullShadows != renderFullShadows;

        if (!Application.isPlaying)
        {
            hasBeenUpdated |= _prevBoundsCentreOffset != _boundsCentreOffset;
        }

        return hasBeenUpdated;
    }

    void UpdateSettings()
    {
        _cameraHasBeenUpdated = CameraHasBeenUpdated();
        if (_cameraHasBeenUpdated)
        {
            if (_prevLightType != lightType)
            {
                CreateMaterials();
            }

            pointLightRadius = Mathf.Max(0.001f, pointLightRadius);
            orthoSize = Mathf.Max(0.001f, orthoSize);

            cam.ResetProjectionMatrix();
            cam.projectionMatrix = CalculateProjectionMatrix();
            switch (lightType)
            {
                case LightTypes.Area:
                case LightTypes.Point:
                case LightTypes.Orthographic:
                    cam.orthographic = true;
                    break;
                case LightTypes.Spot:
                    cam.orthographic = false;
                    break;
            }

            if (shadowMode == ShadowMode.None || shadowMode == ShadowMode.Baked)
            {
                if (_depthTexture != null)
                {
                    SafeDestroy(_depthTexture);
                }
            }
        }

        _prevSlices = slices;
        _prevFov = spotAngle;
        _prevNear = Mathf.Max(0.01f, spotNear);
        _prevFar = spotRange;
        _prevIsOrtho = cam.orthographic;
        _prevOrthoSize = orthoSize;
        _prevShadowMode = shadowMode;
        _prevLightType = lightType;
        _prevPointLightRadius = pointLightRadius;
        _prevRenderFullShadows = renderFullShadows;
        _prevOrtho = orthoSize;
        _prevOrthoAspect = aspect;

        if (!Application.isPlaying)
        {
            _prevBoundsCentreOffset = _boundsCentreOffset;
        }
    }

    void UpdateLightMatrices()
    {
        _localToWorldMatrix = CachedTransform.localToWorldMatrix;
        _worldToCamera = cam.worldToCameraMatrix;

        switch (lightType)
        {
            case LightTypes.Spot:
                _rotation = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(_angle.x, _angle.y, _angle.z), Vector3.one);
                break;
            case LightTypes.Point:
                var worldOffset = CachedTransform.position * 180.0f * worldScrollAmount;
                _rotation = Matrix4x4.TRS(Vector3.zero, CachedTransform.rotation * Quaternion.Euler(_angle.x + worldOffset.x, _angle.y + worldOffset.y, _angle.z + worldOffset.z), Vector3.one);
                break;
            case LightTypes.Area:
                _rotation = Matrix4x4.TRS(_angle, Quaternion.identity, Vector3.one);
                break;
            case LightTypes.Orthographic:
                _rotation = Matrix4x4.TRS(_angle, Quaternion.identity, Vector3.one);
                break;
        }

        _angle += noiseSpeed * Time.deltaTime;

        RebuildMesh();
    }

    void UpdateViewMatrices(Camera targetCamera)
    {
        _viewWorldToCameraMatrixCached = targetCamera.worldToCameraMatrix;
        _viewCameraToWorldMatrixCached = targetCamera.cameraToWorldMatrix;

        switch (lightType)
        {
            case LightTypes.Spot:
                _viewWorldLight = _worldToCamera * _viewCameraToWorldMatrixCached;
                break;
            case LightTypes.Point:
                _localRotation = Matrix4x4.TRS(Vector3.zero, CachedTransform.rotation, Vector3.one);
                _viewWorldLight = Matrix4x4.TRS(-CachedTransform.position, Quaternion.identity, Vector3.one) * _viewCameraToWorldMatrixCached;
                break;
            case LightTypes.Area:
            case LightTypes.Orthographic:
                _localRotation = Matrix4x4.TRS(Vector3.zero, CachedTransform.rotation, Vector3.one);
                _viewWorldLight = _worldToCamera * _viewCameraToWorldMatrixCached;
                break;
        }
    }

    void RebuildMesh()
    {
        CalculateMinMax(out _minBounds, out _maxBounds, _cameraHasBeenUpdated);

        // Build the mesh if we have modified the parameters
        if (_cameraHasBeenUpdated)
        {
            _projectionMatrixCached = CalculateProjectionMatrix();
            if (Application.isPlaying)
            {
                if (!_builtMesh)
                {
                    _builtMesh = true;
                    BuildMesh(false, slices, _minBounds, _maxBounds);
                }
            }
            else
            {
                BuildMesh(false, slices, _minBounds, _maxBounds);
            }
        }
    }

    void SetShaderPropertiesBlock(MaterialPropertyBlock propertyBlock)
    {
        propertyBlock.SetVector(_idNoiseOffset, _angle);
        propertyBlock.SetVector(_idMinBounds, _minBounds);
        propertyBlock.SetVector(_idMaxBounds, _maxBounds);
        propertyBlock.SetMatrix(_idProjection, _projectionMatrixCached);
        propertyBlock.SetMatrix(_idViewWorldLight, _viewWorldLight);
        propertyBlock.SetMatrix(_idLocalRotation, _localRotation);
        propertyBlock.SetMatrix(_idRotation, _rotation);
        propertyBlock.SetColor(_idColorTint, colorTint);
        propertyBlock.SetFloat(_idLightMultiplier, lightMultiplier);
        propertyBlock.SetVector("_WorldPos", (CachedTransform.position + CachedTransform.forward) * worldScrollAmount);

        var lightMaterial = LightMaterial;

        if (useSoftBlend)
        {
            lightMaterial.EnableKeyword("_SOFTBLEND_ON");
        }
        else
        {
            lightMaterial.DisableKeyword("_SOFTBLEND_ON");
        }

        if (useDithering)
        {
            propertyBlock.SetFloat(_idJitterAmount, ditherAmount);
            propertyBlock.SetTexture(_idDitherTex, DitherTexture);
            lightMaterial.EnableKeyword("_DITHER_ON");
        }
        else
        {
            lightMaterial.DisableKeyword("_DITHER_ON");
        }

        if (useCurves)
        {
            propertyBlock.SetTexture(_idFallOffTex, FallOffTexture);
            lightMaterial.EnableKeyword("_CURVE_ON");
        }
        else
        {
            lightMaterial.DisableKeyword("_CURVE_ON");
        }

        switch (lightType)
        {
            case LightTypes.Point:
            case LightTypes.Spot:
            case LightTypes.Orthographic:
                propertyBlock.SetFloat(_idSpotExponent, spotExponent);
                propertyBlock.SetFloat(_idConstantAttenuation, constantAttenuation);
                propertyBlock.SetFloat(_idLinearAttenuation, linearAttenuation);
                propertyBlock.SetFloat(_idQuadraticAttenuation, quadraticAttenuation);
                break;
            case LightTypes.Area:
                Vector4 p = volumeTextureOffset;
                p.w = volumeTextureScale;
                propertyBlock.SetVector(_idVolumeOffset, p);
                propertyBlock.SetFloat(_idVolumeParams, shapeValue);
                switch (volumeShape)
                {
                    case VolumeShape.Cube:
                        lightMaterial.EnableKeyword("_SHAPE_CUBE");
                        lightMaterial.DisableKeyword("_SHAPE_SPHERE");
                        lightMaterial.DisableKeyword("_SHAPE_ROUNDED_CUBE");
                        lightMaterial.DisableKeyword("_SHAPE_CYLINDER");
                        break;
                    case VolumeShape.Sphere:
                        lightMaterial.EnableKeyword("_SHAPE_SPHERE");
                        lightMaterial.DisableKeyword("_SHAPE_CUBE");
                        lightMaterial.DisableKeyword("_SHAPE_ROUNDED_CUBE");
                        lightMaterial.DisableKeyword("_SHAPE_CYLINDER");
                        break;
                    case VolumeShape.RoundedCube:
                        lightMaterial.EnableKeyword("_SHAPE_ROUNDED_CUBE");
                        lightMaterial.DisableKeyword("_SHAPE_CUBE");
                        lightMaterial.DisableKeyword("_SHAPE_SPHERE");
                        lightMaterial.DisableKeyword("_SHAPE_CYLINDER");
                        break;
                    case VolumeShape.Cylinder:
                        lightMaterial.EnableKeyword("_SHAPE_CYLINDER");
                        lightMaterial.DisableKeyword("_SHAPE_CUBE");
                        lightMaterial.DisableKeyword("_SHAPE_SPHERE");
                        lightMaterial.DisableKeyword("_SHAPE_ROUNDED_CUBE");
                        break;
                }
                break;
        }

        switch (lightType)
        {
            case LightTypes.Area:
                propertyBlock.SetTexture("_MainTex", areaVolume != null ? areaVolume : EmptyTexture3D);
                break;
            case LightTypes.Point:
                propertyBlock.SetTexture("_LightColorEmission", pointEmission != null ? pointEmission : EmptyCubemap);
                propertyBlock.SetTexture("_NoiseTex", pointNoise != null ? pointNoise : EmptyCubemap);
                propertyBlock.SetTexture("_ShadowTexture", pointShadow != null ? pointShadow : EmptyCubemap);
                break;
            case LightTypes.Spot:
            case LightTypes.Orthographic:
                propertyBlock.SetTexture("_LightColorEmission", spotEmission != null ? spotEmission : EmptyTexture2D);
                propertyBlock.SetTexture("_NoiseTex", spotNoise != null ? spotNoise : EmptyTexture2D);
                propertyBlock.SetTexture("_ShadowTexture", spotShadow != null ? spotShadow : EmptyTexture2D);
                break;
        }

        var shouldUseCustomShadowMap = false;

        cam.targetTexture = null;
        switch (shadowMode)
        {
            case ShadowMode.Realtime:
                if (_depthTexture == null)
                {
                    CreateDepthTexture(lightType);
                }
                propertyBlock.SetTexture("_ShadowTexture", _depthTexture);
                lightMaterial.EnableKeyword("_SHADOW_ON");
                shouldUseCustomShadowMap = renderFullShadows;
                break;
            case ShadowMode.Baked:
                lightMaterial.EnableKeyword("_SHADOW_ON");
                shouldUseCustomShadowMap = false;
                break;
            case ShadowMode.None:
                lightMaterial.DisableKeyword("_SHADOW_ON");
                shouldUseCustomShadowMap = false;
                break;
        }

        if (shouldUseCustomShadowMap)
        {
            lightMaterial.EnableKeyword("_SHADOW_EXP");
        }
        else
        {
            lightMaterial.DisableKeyword("_SHADOW_EXP");
        }

        float far = cam.farClipPlane;
        float near = cam.nearClipPlane;
        float fov = cam.fieldOfView;

        far = spotRange;
        near = Mathf.Max(0.01f, spotNear);
        fov = spotAngle;

        cam.farClipPlane = far;

        if (lightType == LightTypes.Point || lightType == LightTypes.Area)
        {
            near = -pointLightRadius;
            far = pointLightRadius;
        }

        switch ((lightType))
        {
            case LightTypes.Point:
            case LightTypes.Area:
                propertyBlock.SetVector(_idLightParams, new Vector4(near, far, aspect, pointLightRadius));
                break;
            case LightTypes.Orthographic:
                propertyBlock.SetVector(_idLightParams, new Vector4(near, far, aspect, orthoSize));
                break;
            default:
                propertyBlock.SetVector(_idLightParams, new Vector4(near, far, aspect, fov * 0.5f * Mathf.Deg2Rad));
                break;
        }
    }

    void SafeDestroy(UnityEngine.Object obj, bool forceImmediate = false)
    {
        if (obj != null)
        {
            if (Application.isPlaying && !forceImmediate)
            {
                Destroy(obj);
            }
            else
            {
                DestroyImmediate(obj, true);
            }
        }
        obj = null;
    }
}

