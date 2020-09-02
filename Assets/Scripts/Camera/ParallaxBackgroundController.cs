using System;
using UnityEngine;

namespace Camera
{
    [RequireComponent(typeof(CameraController))]
    public class ParallaxBackgroundController : MonoBehaviour
    {
        private static class BackgroundParams
        {
            public static readonly int Offset = Shader.PropertyToID("_Offset");
        }

        private const float ParallaxScale = 10000;

        [SerializeField] private MeshRenderer background;
        [SerializeField] private float parallaxStrength;

        private CameraController _camera;
        private Vector2 BackgroundOffset
        {
            get => background.material.GetVector(BackgroundParams.Offset);
            set => background.material.SetVector(BackgroundParams.Offset, value);
        }

        private void Start()
        {
            _camera = GetComponent<CameraController>();
        }


        private void Update()
        {
            BackgroundOffset += _camera.Velocity * (parallaxStrength / ParallaxScale);
        }
    }
}