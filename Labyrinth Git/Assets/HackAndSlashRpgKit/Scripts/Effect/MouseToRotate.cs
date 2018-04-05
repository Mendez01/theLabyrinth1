using UnityEngine;

/// <summary>
/// Author : Hwan Kim
/// </summary>
public class MouseToRotate : MonoBehaviour
{
    public enum RotatePivot
    {
        X, Y, Z
    }

    public RotatePivot pivot;
    private float _sensitivity;
    private Vector3 _mouseReference;
    private Vector3 _mouseOffset;
    private Vector3 _rotation;
    private bool _isRotating;
    private Transform thisTran;

    private void Start()
    {
        _sensitivity = 0.4f;
        _rotation = Vector3.zero;
        thisTran = transform;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // rotating flag
            _isRotating = true;

            // store mouse
            _mouseReference = Input.mousePosition;
        }
        if (Input.GetMouseButtonUp(0))
        {
            // rotating flag
            _isRotating = false;
        }
        if (_isRotating)
        {
            // offset
            _mouseOffset = (Input.mousePosition - _mouseReference);
            // apply rotation
            switch (pivot)
            {
                case RotatePivot.X:
                    _rotation.x = -(_mouseOffset.x) * _sensitivity;
                    break;

                case RotatePivot.Y:

                    _rotation.y = -(_mouseOffset.x) * _sensitivity;
                    break;

                case RotatePivot.Z:
                    _rotation.z = -(_mouseOffset.x) * _sensitivity;
                    break;
            }
            // rotate
            thisTran.Rotate(_rotation);

            // store mouse
            _mouseReference = Input.mousePosition;
        }
    }
}