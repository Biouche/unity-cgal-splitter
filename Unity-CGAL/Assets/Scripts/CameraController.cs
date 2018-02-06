using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TouchScript.Gestures.TransformGestures;

public class CameraController : MonoBehaviour
{

    public float RotationSpeed = 100f;
    public float ZoomSpeed = 50f;
    public float TranslationSpeed = 0.5f;
    public Toggle toggle;
    public ScreenTransformGesture ManipulationGesture;
    public ScreenTransformGesture OneFingerMoveGesture;
    private Transform cam;
    private Transform pivot;

    // Use this for initialization
    void Start()
    {
        pivot = this.transform;
        cam = transform.Find("Main Camera");
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnEnable()
    {
        ManipulationGesture.Transformed += twoFingersManipulation;
        OneFingerMoveGesture.Transformed += oneFingerTranslation;
    }

    private void OnDisable()
    {
        ManipulationGesture.Transformed -= twoFingersManipulation;
        OneFingerMoveGesture.Transformed -= oneFingerTranslation;
    }

    private void twoFingersManipulation(object sender, System.EventArgs e)
    {
        if (toggle.isOn)
        {
            var rotation = Quaternion.Euler(ManipulationGesture.DeltaPosition.y / Screen.height * RotationSpeed,
                -ManipulationGesture.DeltaPosition.x / Screen.width * RotationSpeed,
                ManipulationGesture.DeltaRotation);
            pivot.localRotation *= rotation;
            cam.transform.localPosition += Vector3.forward * (ManipulationGesture.DeltaScale - 1f) * ZoomSpeed;
        }
    }

    private void oneFingerTranslation(object sender, System.EventArgs e)
    {
        if (toggle.isOn)
            pivot.localPosition -= pivot.rotation * OneFingerMoveGesture.DeltaPosition * TranslationSpeed;
    }
}
