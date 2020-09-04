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


    private Vector3 _velocity;
    public Vector2 Velocity => _velocity;


    [InitializeOnLoadMethod]
    private static void EnablePixelPerfectInEditor()
    {
        UnityEngine.Camera.allCameras.Select(c => c.GetComponent<PixelPerfectCamera>())
            .Where(controller => controller).ToList().ForEach(perfectCamera =>
                perfectCamera.runInEditMode = true);
    }

    private void LateUpdate()
    {
        var position = transform.position;
        var targetPos = new Vector3(target.position.x,
            target.position.y + targetOffsetY, position.z);
        targetPos = new Vector3(Mathf.Clamp(targetPos.x, -xOffset, xOffset),
            Mathf.Clamp(targetPos.y, minPosY, maxPosY), targetPos.z);

        position = Vector3.SmoothDamp(position, targetPos,
            ref _velocity, default, smoothMovementMaxSpeed);
        transform.position = position;
    }
}