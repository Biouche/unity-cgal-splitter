using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TouchScript.Gestures.TransformGestures;
using UnityEditor;

public class DrawTrail : MonoBehaviour {
	public Toggle toggle;
	public GameObject sceneObjectPrefab;
	public float cutMeshDepth = 10f;

	private Plane objPlane;
	private TrailRenderer trailRenderer;
	private Transform cuttedMeshTransform;
	private GameObject cuttedGameObject;
	// Use this for initialization
	void Start () {
		trailRenderer = this.GetComponentInChildren<TrailRenderer> ();
		objPlane = new Plane (Camera.main.transform.forward * -1, this.transform.position);

		cuttedGameObject = GameObject.Find ("SceneObject");
		MeshRenderer renderer = cuttedGameObject.GetComponentInChildren<MeshRenderer> ();

		//Set GO position to center of the screen
		Vector3 center = renderer.bounds.center;
        cuttedGameObject.transform.position = new Vector3 ((cuttedGameObject.transform.position.x - center.x), (cuttedGameObject.transform.position.y - center.y), cuttedGameObject.transform.position.z);
		//init private variables
		cuttedMeshTransform = cuttedGameObject.transform;
	}

	// Update is called once per frame
	void Update () {
        if (!(toggle == null) && toggle.isOn)
			handleTrail ();
	}

	void handleTrail () {
		if (((Input.touchCount > 0 && Input.GetTouch (0).phase == TouchPhase.Moved) ||
				Input.GetMouseButton (0))) {
			Ray mRay = Camera.main.ScreenPointToRay (Input.mousePosition);
			float rayDistance;
			if (objPlane.Raycast (mRay, out rayDistance))
				this.transform.position = mRay.GetPoint (rayDistance);
		} else if (Input.GetMouseButtonUp (0) || (Input.touchCount > 0 && Input.GetTouch (0).phase == TouchPhase.Ended)) {
			int pos = trailRenderer.positionCount;
            // If more than one point & trail size is larger than the mesh

			if (pos > 1) {
				//Get Points from renderer
				Vector3[] positions = new Vector3[pos];
				trailRenderer.GetPositions (positions);
				//Generate cut meshes from points
				Mesh cutMesh = GenerateMeshFromPositions (positions, 1);
				Mesh cutMesh2 = GenerateMeshFromPositions (positions, 2);
				//Mesh cutMesh = GenerateMeshFromPositions(positions);
				GameObject cutMeshGO = addMeshToScene ("Cut Mesh", cutMesh, 1f);
				GameObject cutMeshGO2 = addMeshToScene ("Cut Mesh 2", cutMesh2, 1f);

				try {
					//Check Intersection between cut mesh and cutted mesh
					//                if (Intersects(cutMeshGO, go1))
					//                {
					//Do cut with cut mesh 1
					CutMesh (cutMesh, cutMeshGO, "cut1");
					//Do cut with cut mesh 2
					CutMesh (cutMesh2, cutMeshGO2, "cut2");
					//Destroy cutted game object
					Destroy (cuttedGameObject);
					//                }

				} catch (Exception e) {
					Debug.Log (e.Message);
				}
				/*Destroy (cutMeshGO);
				Destroy (cutMeshGO2);*/
			}
			trailRenderer.Clear ();

		}
	}

	void CutMesh (Mesh cutMesh, GameObject cutMeshGO, string name) {

		string cutMeshOff = ObjectFileFormat.MeshToOff (cutMesh, cutMeshGO.transform);
        StreamWriter strw = new StreamWriter("./Assets/Models/"+cutMeshGO.name+".off");
        strw.Write(cutMeshOff);
        strw.Close();

        string cuttedMeshOff = System.IO.File.ReadAllText ("./Assets/Models/squirrel.off");
		StringBuilder strB = new StringBuilder ();
		//Extract transformation matrix

        Matrix4x4 transformationMatrix = cuttedMeshTransform.localToWorldMatrix;
		strB.Append (transformationMatrix.m00).AppendLine ();
		strB.Append (transformationMatrix.m01).AppendLine ();
		strB.Append (transformationMatrix.m02).AppendLine ();
		strB.Append (transformationMatrix.m03).AppendLine ();
		strB.Append (transformationMatrix.m10).AppendLine ();
		strB.Append (transformationMatrix.m11).AppendLine ();
		strB.Append (transformationMatrix.m12).AppendLine ();
		strB.Append (transformationMatrix.m13).AppendLine ();
		strB.Append (transformationMatrix.m20).AppendLine ();
		strB.Append (transformationMatrix.m21).AppendLine ();
		strB.Append (transformationMatrix.m22).AppendLine ();
        strB.Append (transformationMatrix.m23).AppendLine();

		strw = new StreamWriter ("./Assets/models/matrix.txt");
		strw.Write (strB.ToString ());
		strw.Close ();

		IntPtr cuttedMeshPtr = Marshal.StringToHGlobalAnsi (cuttedMeshOff);
		IntPtr cuttedMeshTransformPtr = Marshal.StringToHGlobalAnsi (strB.ToString ());
		IntPtr cutMeshPtr = Marshal.StringToHGlobalAnsi (cutMeshOff);

		IntPtr ptrResult = CGALController.booleanOperationClean (cuttedMeshPtr, cuttedMeshTransformPtr, cutMeshPtr, Marshal.StringToHGlobalAnsi ("difference"));
		//Convert IntPtr to string
		string result = Marshal.PtrToStringAnsi (ptrResult);
		//exportOff (result, "mesh" + Time.fixedDeltaTime);
		// Open string stream on the result off string
		byte[] byteArray = Encoding.UTF8.GetBytes (result);
		MemoryStream stream = new MemoryStream (byteArray);
		//Convert off string to Mesh and add it to scene
		Mesh myMesh = ObjectFileFormat.OffToMesh (new StreamReader (stream));
        stream.Close();
		myMesh.RecalculateNormals();
        ObjExporter.MeshToFile(myMesh, name, "BenchMat");
		addMeshToSceneAndExport (name, myMesh, transform.GetComponent<Transform> ().localScale.x);
	}

	private GameObject addMeshToSceneAndExport (string name, Mesh mesh, float scale) {
        //Export mesh to .obj file
        AssetDatabase.Refresh();
        //Game Object
        Debug.Log(name);
		GameObject go = Instantiate (Resources.Load (name, typeof (GameObject))) as GameObject;
		go.name = name;
		go.transform.parent = GameObject.Find ("ObjectContainer").transform;
		go.transform.position = Vector3.zero;
		go.transform.localScale = new Vector3 (1f, 1f, 1f) * scale;
       
		//go.AddComponent<ObjectGestures>();
		foreach (Transform child in go.GetComponentInChildren<Transform>())
		{
			child.gameObject.AddComponent<ObjectGestures>();
			child.gameObject.AddComponent<MeshCollider> ();
		}
		//og.setTransformGesture(gesture);
		return go;
	}

	private GameObject addMeshToScene(string name, Mesh mesh, float scale)
	{
		GameObject go = new GameObject(name);
		go.transform.parent = GameObject.Find ("ObjectContainer").transform;
		go.transform.position = Vector3.zero;
		go.transform.localScale = new Vector3(1f, 1f, 1f) * scale;
		go.AddComponent<MeshRenderer>();
		MeshFilter mf = go.AddComponent<MeshFilter>();
		mf.mesh = mesh;
		go.AddComponent<MeshCollider> ();

		return go;
	}

	private Mesh GenerateMeshFromPositions (Vector3[] positions, int orientation) {
		Vector3 gazeVector = Camera.main.gameObject.transform.forward;
		int nbVertices = positions.Length * 2;
		Vector3[] vertices = new Vector3[nbVertices]; //add 4 points for extrusion
		int k = 0;
		for (int i = 0; i < positions.Length; i++) {
			vertices[k] = new Vector3 (positions[i].x - (gazeVector.x * cutMeshDepth), positions[i].y - (gazeVector.y * cutMeshDepth), positions[i].z - (gazeVector.z * cutMeshDepth));
			++k;
			//projection along gaze vector
			vertices[k] = new Vector3 (positions[i].x + (gazeVector.x * cutMeshDepth), positions[i].y + (gazeVector.y * cutMeshDepth), positions[i].z + (gazeVector.z * cutMeshDepth));
			++k;
		}

		int nbTriangles = (nbVertices - 2) * 3;
		// Add 8 triangles for extrusion
		int[] triangles = new int[nbTriangles];

		int index = 0, value = 0;
		// Clockwise
		if (orientation == 1) {

			while (index < nbTriangles / 2) {
				triangles[index] = value;
				++index;
				triangles[index] = value + 2;
				++index;
				triangles[index] = value + 1;
				++index;
				value = value + 2;

			}
			value = 1;
			while (index < nbTriangles) {
				triangles[index] = value;
				++index;
				++value;
				triangles[index] = value;
				++index;
				++value;
				triangles[index] = value;
				++index;
			}
		}
		//Anti Clockwise
		else if (orientation == 2) {
			while (index < nbTriangles / 2) {
				triangles[index] = value;
				++index;
				++value;
				triangles[index] = value;
				++index;
				++value;
				triangles[index] = value;
				++index;

			}
			value = 1;
			while (index < nbTriangles) {
				triangles[index] = value;
				++index;
				triangles[index] = value + 2;
				++index;
				triangles[index] = value + 1;
				++index;
				value = value + 2;
			}
		} else {
			return null;
		}

		Mesh cutMesh = new Mesh ();
		cutMesh.vertices = vertices;
		cutMesh.triangles = triangles;
		cutMesh.RecalculateNormals ();
		return cutMesh;
	}

	private Mesh GenerateMeshFromPositions (Vector3[] positions) {

		Vector3 gazeVector = Camera.main.gameObject.transform.forward;

		int nbVertices = positions.Length * 2;
		Vector3[] vertices = new Vector3[nbVertices]; //add 4 points for extrusion
		int k = 0;
		for (int i = 0; i < positions.Length; i++) {
			vertices[k] = new Vector3 (positions[i].x - (gazeVector.x * cutMeshDepth), positions[i].y - (gazeVector.y * cutMeshDepth), positions[i].z - (gazeVector.z * cutMeshDepth));
			++k;
			//projection along gaze vector
			vertices[k] = new Vector3 (positions[i].x + (gazeVector.x * cutMeshDepth), positions[i].y + (gazeVector.y * cutMeshDepth), positions[i].z + (gazeVector.z * cutMeshDepth));
			++k;
		}

		int nbTriangles = (nbVertices - 2) * 3;
		int[] triangles = new int[nbTriangles * 2];

		int index = 0, value = 0;
		// Clockwise
		while (index < nbTriangles / 2) {
			triangles[index] = value;
			++index;
			triangles[index] = value + 2;
			++index;
			triangles[index] = value + 1;
			++index;
			value = value + 2;

		}
		value = 1;
		while (index < nbTriangles) {
			triangles[index] = value;
			++index;
			++value;
			triangles[index] = value;
			++index;
			++value;
			triangles[index] = value;
			++index;
		}
		index = 0;
		value = 0;
		//Anti Clockwise
		while (index < nbTriangles / 2) {
			triangles[index] = value;
			++index;
			++value;
			triangles[index] = value;
			++index;
			++value;
			triangles[index] = value;
			++index;

		}
		value = 1;
		while (index < nbTriangles) {
			triangles[index] = value;
			++index;
			triangles[index] = value + 2;
			++index;
			triangles[index] = value + 1;
			++index;
			value = value + 2;
		}

		Mesh cutMesh = new Mesh ();
		cutMesh.vertices = vertices;
		cutMesh.triangles = triangles;
		cutMesh.RecalculateNormals ();
		return cutMesh;
	}

	private bool Intersects (GameObject cutGO, GameObject go2) {
		if (cutGO.GetComponent<MeshCollider> ().bounds.Intersects (go2.GetComponent<MeshCollider> ().bounds)) {
			string cutMeshOff = ObjectFileFormat.MeshToOff (cutGO.GetComponent<MeshFilter> ().mesh, cutGO.transform);
			string cuttedMeshOff = ObjectFileFormat.MeshToOff (go2.GetComponent<MeshFilter> ().mesh, go2.transform);
			int result = CGALController.checkIntersection (Marshal.StringToHGlobalAnsi (cuttedMeshOff), Marshal.StringToHGlobalAnsi (cutMeshOff));
			if (result == 1)
				return true;
		}
		return false;

	}

	private void exportOff (String offFile, String name) {
		StreamWriter strw = new StreamWriter ("./Assets/Models/" + name + ".off");
		strw.Write (offFile);
		strw.Close ();
	}
}