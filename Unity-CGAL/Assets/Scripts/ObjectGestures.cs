using UnityEngine;
using UnityEngine.UI;
using TouchScript.Gestures.TransformGestures;

public class ObjectGestures : MonoBehaviour
{

    private Toggle rotationToggle;
    private float rotationSpeed = 5.0f;
    private Toggle moveToggle;
    public TransformGesture transformGesture;

    // Use this for initialization
    void Start()
    {   
        if (rotationToggle == null)
        {
            var go = GameObject.Find("Rotate");
            rotationToggle = go.GetComponent<Toggle>();
        }
        if (moveToggle == null)
        {
            GameObject go = GameObject.Find("Move Objects");
            moveToggle = go.GetComponent<Toggle>();
        }

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnMouseEnter()
    {
    }

    private void OnEnable()
    {
		if (transformGesture == null)
		{
            transformGesture=this.gameObject.AddComponent<TransformGesture>();
		}
        transformGesture.Transformed += moveHandler;
        transformGesture.Transformed += rotationHandler;
    }


    private void OnDisable()
    {
        transformGesture.Transformed -= moveHandler;
        transformGesture.Transformed -= rotationHandler;
    }

    private void moveHandler(object sender, System.EventArgs e)
    {
        if (moveToggle.isOn)
        {
            this.gameObject.transform.position += transformGesture.DeltaPosition;
            if (transformGesture.NormalizedScreenPosition.x >= 0.9)
            {
                Camera.main.transform.Translate(Vector3.right * Time.deltaTime * 10);
            }
            if (transformGesture.NormalizedScreenPosition.x <= 0.1)
            {
                Camera.main.transform.Translate(Vector3.left * Time.deltaTime * 10);
            }
            if (transformGesture.NormalizedScreenPosition.y >= 0.9)
            {
                Camera.main.transform.Translate(Vector3.up * Time.deltaTime * 10);
            }
            if (transformGesture.NormalizedScreenPosition.y <= 0.1)
            {
                Camera.main.transform.Translate(Vector3.down * Time.deltaTime * 10);
            }

        }
    }

    private void rotationHandler(object sender, System.EventArgs e)
    {
        if (rotationToggle.isOn)
        {
            Vector3 rotationVector = new Vector3(transformGesture.DeltaPosition.z * rotationSpeed, 0f, -transformGesture.DeltaPosition.x * rotationSpeed);
            transform.Rotate(rotationVector, Space.World);
        }
    }

	public void setTransformGesture(TransformGesture gesture)
	{
		this.transformGesture = gesture;
	}

}
