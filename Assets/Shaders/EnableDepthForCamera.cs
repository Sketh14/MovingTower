using UnityEngine;

namespace Moving_Tower
{
    [ExecuteInEditMode]
    public class EnableDepthForCamera : MonoBehaviour
    {
        void OnEnable()
        {
            Camera camera = GetComponent<Camera>();
            camera.depthTextureMode = camera.depthTextureMode | DepthTextureMode.Depth;
        }
    }
}