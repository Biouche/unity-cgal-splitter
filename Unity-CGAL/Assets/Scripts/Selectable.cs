using UnityEngine;
using UnityEngine.UI;
public class Selectable : MonoBehaviour
{
	private bool selected;
	Toggle toggle;
	// Use this for initialization
	void Start ()
	{
		selected = false;
		GameObject go = GameObject.Find ("CGAL Functions Toggle");
		toggle = go.GetComponent<Toggle> ();
	}

	// Update is called once per frame
	void Update ()
	{
		MeshRenderer renderer = this.GetComponent<MeshRenderer> ();
		renderer.material.color = selected ? Color.green : Color.gray;
	}

	void OnMouseDown() {
		if (toggle.isOn) {
			if (!CGALGUI.addSelectedObject (this)) {
				Debug.Log ("List is full");
			}
			else {
				selected = !selected;
			}
		}
	}
}

