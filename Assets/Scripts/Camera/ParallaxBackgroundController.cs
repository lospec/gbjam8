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
        [SerializeField] private Transform leftDeco;
        [SerializeField] private Transform rightDeco;


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


        private void LateUpdate()
        {
            BackgroundOffset += _camera.Velocity * (parallaxStrength / ParallaxScale);
            var leftDecoPosition = leftDeco.position;
            var position = transform.position;
            leftDecoPosition = new Vector3(leftDecoPosition.x, position
                .y, leftDecoPosition.z);
            leftDeco.position = leftDecoPosition;
            var rightDecoPosition = rightDeco.position;
            rightDecoPosition = new Vector3(rightDecoPosition.x, position
                .y, rightDecoPosition.z);
            rightDeco.position = rightDecoPosition;
        }
    }
}