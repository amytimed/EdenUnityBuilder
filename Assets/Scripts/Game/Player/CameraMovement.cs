using System;
using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour
{
    public Transform HorizontalRotatingTransform;
	
    public Transform VerticalRotatingTransform;
	
    public float Sensitivity;
	
    public float MaxVerticalAngle = 90;
	
    public float MinVerticalAngle = -90;
	
    public TouchField LookInput;
	
    public float RotationSpeed = 1;

    private float _theHorizontalVector;
	
    private float _theVerticalVector;
	

    void Update()
    {
		_theHorizontalVector = LookInput.TouchDist.x * Sensitivity + HorizontalRotatingTransform.eulerAngles.y;
        _theVerticalVector = -LookInput.TouchDist.y * Sensitivity + _theVerticalVector;
        _theVerticalVector = Mathf.Clamp(_theVerticalVector, MinVerticalAngle, MaxVerticalAngle);

        HorizontalRotatingTransform.rotation = Quaternion.Lerp(HorizontalRotatingTransform.rotation, Quaternion.Euler(0, _theHorizontalVector, 0), RotationSpeed);
        VerticalRotatingTransform.localRotation = Quaternion.Lerp(VerticalRotatingTransform.localRotation, Quaternion.Euler(_theVerticalVector, 0, 0), RotationSpeed);
    }
}