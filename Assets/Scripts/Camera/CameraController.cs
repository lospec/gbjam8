﻿using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Serialization;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float smoothMovementMaxSpeed = Mathf.Infinity;
    [SerializeField] private float minPosY = Mathf.NegativeInfinity;
    [SerializeField] private float maxPosY = Mathf.Infinity;
    [SerializeField] private float targetOffsetY = 0;
    [SerializeField] private float xOffset = 3f;

    [SerializeField] private int pixelPerUnit = 8;


    private Vector3 _previousPosition;

    private Vector3 _velocity;
    public Vector2 Velocity { get; private set; }

    private void Start()
    {
        _previousPosition = transform.position;
    }

#if UNITY_EDITOR
	[InitializeOnLoadMethod]
	private static void EnablePixelPerfectInEditor()
    {
        UnityEngine.Camera.allCameras.Select(c => c.GetComponent<PixelPerfectCamera>())
            .Where(controller => controller).ToList().ForEach(perfectCamera =>
                perfectCamera.runInEditMode = true);
    }
#endif

	private void LateUpdate()
    {
        var position = transform.position;
        var targetPos = new Vector3(target.position.x,
            target.position.y + targetOffsetY, position.z);
        targetPos = new Vector3(Mathf.Clamp(targetPos.x, -xOffset, xOffset),
            Mathf.Clamp(targetPos.y, minPosY, maxPosY), targetPos.z);

        position = Vector3.SmoothDamp(position, targetPos,
            ref _velocity, default, smoothMovementMaxSpeed);

        position = new Vector3
        {
            x = Mathf.Round(position.x * pixelPerUnit) / pixelPerUnit,
            y = Mathf.Round(position.y * pixelPerUnit) / pixelPerUnit,
            z = Mathf.Round(position.z * pixelPerUnit) / pixelPerUnit
        };
        transform.position = position;
        Velocity = position - _previousPosition;
        _previousPosition = position;
    }
}