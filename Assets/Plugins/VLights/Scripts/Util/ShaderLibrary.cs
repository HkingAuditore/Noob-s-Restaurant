using UnityEngine;

namespace com.brian.vlights
{
    // [CreateAssetMenuAttribute]
    public class ShaderLibrary : ScriptableObject
    {
        [SerializeField]
        private Shader[] _shaders;
    }
}