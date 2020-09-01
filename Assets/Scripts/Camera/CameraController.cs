using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

[ExecuteAlways]
public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private float _smoothMovementMaxSpeed = Mathf.Infinity;
    [SerializeField] private float _minPosY = Mathf.NegativeInfinity;
    [SerializeField] private float _maxPosY = Mathf.Infinity;
    [SerializeField] private float _targetOffsetY = 0;

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
        Vector3 targetPos = new Vector3(transform.position.x,
            _target.position.y + _targetOffsetY, transform.position.z);
        targetPos = new Vector3(targetPos.x,
            Mathf.Clamp(targetPos.y, _minPosY, _maxPosY), targetPos.z);

        transform.position = Vector3.SmoothDamp(transform.position, targetPos,
            ref _velocity, default, _smoothMovementMaxSpeed);
    }
}