using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public partial class VLight : MonoBehaviour
{
	#if UNITY_EDITOR
	Texture CreateBakedShadowTexture(LightTypes type)
	{
		switch(type)
		{
		case LightTypes.Point:
			return new Cubemap(shadowMapRes, TextureFormat.ARGB32, false);
		case LightTypes.Spot:
		case LightTypes.Orthographic:
			return new Texture2D(shadowMapRes, shadowMapRes, TextureFormat.ARGB32, false, true);
		}
		return null;
	}
	
	public void RenderBakedShadowMap()
	{
		float far = cam.farClipPlane;

		if(lightType == LightTypes.Area || lightType == LightTypes.Point)
		{
			far = pointLightRadius;
		}
		else
		{
			far = spotRange;
		}
		
		if(SystemInfo.supportsImageEffects)
		{
			cam.backgroundColor = Color.red;
			cam.clearFlags = CameraClearFlags.SolidColor;
			cam.depthTextureMode = DepthTextureMode.None;
			cam.renderingPath = RenderingPath.VertexLit;
			
			var bakedShadowMap = CreateBakedShadowTexture(lightType);

			if(RenderDepthShader != null)
			{
				switch(lightType)
				{
				case LightTypes.Spot:
				case LightTypes.Orthographic:
					var tempShadowMap = RenderTexture.GetTemporary(shadowMapRes, shadowMapRes, 1, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
					cam.targetTexture = tempShadowMap;
					cam.projectionMatrix = CalculateProjectionMatrix();
					cam.RenderWithShader(RenderDepthShader, "RenderType");
					
					//Blur the result
					var pingPong = RenderTexture.GetTemporary(shadowMapRes, shadowMapRes, 0);
					pingPong.DiscardContents();
					PostMaterial.SetFloat("_BlurSize", shadowBlurSize);
					for(int i = 0; i < shadowBlurPasses; i++)
					{
						Graphics.Blit(tempShadowMap, pingPong, PostMaterial, 1);
						tempShadowMap.DiscardContents();
						Graphics.Blit(pingPong, tempShadowMap, PostMaterial, 2);
						pingPong.DiscardContents();
					}

					RenderTexture.active = tempShadowMap;

					var tex = bakedShadowMap as Texture2D;
					tex.ReadPixels(new Rect(0, 0, shadowMapRes, shadowMapRes), 0, 0);
					tex.Apply();
					spotShadow = tex;

					RenderTexture.active = null;
					RenderTexture.ReleaseTemporary(tempShadowMap);
					RenderTexture.ReleaseTemporary(pingPong);
					
					break;
				case LightTypes.Point:
					cam.projectionMatrix = Matrix4x4.Perspective(90, 1.0f, 0.1f, far);
					cam.SetReplacementShader(RenderDepthShader, "RenderType");
					cam.RenderToCubemap(bakedShadowMap as Cubemap, 63);
					cam.ResetReplacementShader();
					
					pointShadow = bakedShadowMap;
					break;
				default:
					break;
				}

				AssetDatabase.CreateAsset(bakedShadowMap, "Assets/" + name + "-shadowmap-" + System.DateTime.Now.ToString("HH-MM-ss") + ".asset");

				SafeDestroy(_depthTexture);
				shadowMode = ShadowMode.Baked;
			}
			else
			{
				Debug.LogWarning("Could not find depth shader. Cannot render shadows");
			}
		}
	}
	#endif
}
