using UnityEngine;
using System.Collections;

/*
 * VLight
 * Copyright Brian Su 2011-2019
*/
namespace VLights
{
	public static class VLightShaderUtil
	{
		public const string POST_SHADER_NAME = "Hidden/V-Light/Post";
		public const string DEPTH_SHADER_NAME = "V-Light/Volumetric Light Depth";
		public const string INTERLEAVED_SHADER_NAME = "V-Light/Volumetric Light Depth";
		public const string DOWNSCALEDEPTH_SHADER_NAME = "Hidden/V-Light/Downscale Depth";
	}
}
