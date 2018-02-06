using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

using UnityEngine;
using UnityEngine.UI;

public class CGALGUI : MonoBehaviour
{
    static List<Selectable> selectableObjects;
    Component[] buttons;
    public Text userMessage;
    public GameObject sceneObjectPrefab;
    // Use this for initialization
    void Start()
    {
        selectableObjects = new List<Selectable>();
        buttons = this.gameObject.GetComponentsInChildren<Button>();

        StreamReader strR = new StreamReader("./Assets/Models/bunny.off");
        Mesh mesh = ObjectFileFormat.OffToMesh(strR);
        strR.Close();
        addMeshToScene("bunny", mesh, 10f);
    }

    // Update is called once per frame
    void Update()
    {
        if (selectableObjects.Count == 2)
        {
            userMessage.text = "Select the action to perform.";
            foreach (Button btn in buttons)
            {
                btn.interactable = true;
            }
        }
        else
        {
            userMessage.text = "Please select 2 objects.";
            foreach (Button btn in buttons)
            {
                btn.interactable = false;
            }
        }
    }

    public static bool addSelectedObject(Selectable go)
    {
        if (selectableObjects.Contains(go))
        {
            selectableObjects.Remove(go);
        }
        else if (selectableObjects.Count == 2)
        {
            return false;
        }
        else
        {
            selectableObjects.Add(go);
        }
        return true;
    }

    public void activateCGALGUI(bool activated)
    {
        this.gameObject.SetActive(activated);
        userMessage.gameObject.SetActive(activated);
    }

    public void onClickBooleanOperation(string name)
    {
        //Get GameObjects and their mesh from scene
        //GameObject go1 = GameObject.Find ("bunny");
        GameObject go1 = selectableObjects[0].gameObject;
        GameObject go2 = selectableObjects[1].gameObject;
        MeshCollider col1 = go1.GetComponent<MeshCollider>();
        MeshCollider col2 = go2.GetComponent<MeshCollider>();
        if (col1.bounds.Intersects(col2.bounds))
        {
            Mesh mesh1 = go1.GetComponent<MeshFilter>().mesh;
            Mesh mesh2 = go2.GetComponent<MeshFilter>().mesh;

            //Convert mesh to off string to IntPtr
            IntPtr ptr1 = Marshal.StringToHGlobalAnsi(ObjectFileFormat.MeshToOff(mesh1, go1.transform));
            IntPtr ptr2 = Marshal.StringToHGlobalAnsi(ObjectFileFormat.MeshToOff(mesh2, go2.transform));
            //Boolean union computation
            IntPtr ptrResult = CGALController.booleanOperation(ptr1, ptr2, Marshal.StringToHGlobalAnsi(name));
            //Convert IntPtr to string
            string result = Marshal.PtrToStringAnsi(ptrResult);
            /*Debug.Log (result);
			//Write the computed off in a file
			StreamWriter strW = new StreamWriter ("./Assets/result.off");
			strW.Write (result);
			strW.Close ();*/

            // Open string stream on the result off string
            byte[] byteArray = Encoding.UTF8.GetBytes(result);
            MemoryStream stream = new MemoryStream(byteArray);

            //Convert off string to Mesh and add it to scene
            addMeshToScene("result", ObjectFileFormat.OffToMesh(new StreamReader(stream)), 1f);

            //Remove previous objects from scene
            Destroy(go1);
            Destroy(go2);
        }
        else
        {
            Debug.Log("Selected objects do not overlap");
        }
    }

    private GameObject addMeshToScene(string name, Mesh mesh, float scale)
    {
        GameObject go = Instantiate(sceneObjectPrefab) as GameObject;
        go.name = name;
        go.transform.parent = GameObject.Find("ObjectContainer").transform;
        go.transform.localScale = new Vector3(1f, 1f, 1f) * scale;
        MeshFilter meshFilter = go.GetComponent<MeshFilter>();
        meshFilter.mesh = mesh;
        MeshRenderer renderer = go.GetComponent<MeshRenderer>();
        renderer.material.shader = Shader.Find("Standard");
        MeshCollider collider = go.GetComponent<MeshCollider>();
        collider.sharedMesh = mesh;
        return go;
    }

}
