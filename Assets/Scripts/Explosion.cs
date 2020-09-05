using System;
using System.Security.Cryptography;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Explosion : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (!_spriteRenderer.enabled)
        {
            Destroy(gameObject);
        }
    }
}