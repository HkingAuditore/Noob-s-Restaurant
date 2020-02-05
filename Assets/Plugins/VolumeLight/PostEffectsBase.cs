using UnityEngine;
using System.Collections;

namespace PostEffect
{
	public class PostEffectsBase : MonoBehaviour
    {

		protected void CheckResources() {
			bool isSupported = CheckSupport();
			
			if (isSupported == false) {
				NotSupported();
			}
		}

		protected bool CheckSupport()
        {						
			return true;
		}

		protected void NotSupported() {
			enabled = false;
		}
		
		protected void Start() {
			CheckResources();
		}

		protected Material CheckShaderAndCreateMaterial(Shader shader, Material material) {
			if (shader == null) {
				return null;
			}
			
			if (shader.isSupported && material && material.shader == shader)
				return material;
			
			if (!shader.isSupported) {
				Debug.LogWarningFormat("{0}.shader is no supported!",shader.name);
				return null;
			}
			else {
				material = new Material(shader);
				material.hideFlags = HideFlags.DontSave;
				if (material)
					return material;
				else 
					return null;
			}
		}
	}
}

