using System.Collections;
using System.IO;
using System.Text;
using UnityEngine;

public class ObjectEditor {
    public static void intersects (GameObject cutGO) {
        GameObject objectContainer = GameObject.Find ("ObjectContainer");
        MeshCollider[] colliders = objectContainer.GetComponentsInChildren<MeshCollider> ();
        Debug.Log ("Number of colliders : " + colliders.Length);
        foreach (MeshCollider collider in colliders) {
            if (cutGO.GetComponent<MeshCollider> ().bounds.Intersects (collider.bounds)) {
                Debug.Log ("Intersects");
            }
        }
    }
}