using System;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Enemy
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class Dissolve : MonoBehaviour
    {
        private static class ShaderParams
        {
            public static readonly int Dissolve =
                Shader.PropertyToID("_Dissolve");
            public static readonly int DissolveScale =
                Shader.PropertyToID("_DissolveScale");
        }

        [Header("Material")]
        [SerializeField] private Material dissolveMaterial;

        [Header("Parameters")]
        [SerializeField, Range(1, 100)] private float dissolveScale = 30f;
        [SerializeField] private float dissolveDuration;

        private SpriteRenderer _spriteRenderer;

        private Material _defaultMaterial;

        private float _dissolveValue = 0;


        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _defaultMaterial = new Material(_spriteRenderer.material);
        }

        private void Start()
        {
            enabled = false;
        }

        private void OnEnable()
        {
            dissolveMaterial.SetFloat(ShaderParams.DissolveScale, dissolveScale);
            _spriteRenderer.material = new Material(dissolveMaterial);
            _dissolveValue = 0;
        }

        private void Destroy()
        {
            enabled = false;
            Destroy(transform.parent.gameObject);
        }

        private void Update()
        {
            _dissolveValue += Time.deltaTime;
            var t = Mathf.InverseLerp(0, dissolveDuration, _dissolveValue);
            _spriteRenderer.material.SetFloat(ShaderParams.Dissolve, t);
            if (t >= 1)
            {
                Destroy();
            }
        }
    }
}