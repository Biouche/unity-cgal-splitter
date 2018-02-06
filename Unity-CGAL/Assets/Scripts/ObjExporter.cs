using System.Collections;
using System.IO;
using System.Text;
using UnityEngine;

public class ObjExporter {

    public static string MeshFilterToString (MeshFilter mf) {
        Mesh m = mf.mesh;
        Material[] mats = mf.GetComponent<Renderer> ().sharedMaterials;
        StringBuilder sb = new StringBuilder ();

        sb.Append ("g ").Append (mf.name).Append ("\n");
        foreach (Vector3 v in m.vertices) {
            sb.Append (string.Format ("v {0} {1} {2}\n", v.x, v.y, v.z));
        }
        sb.Append ("\n");
        foreach (Vector3 v in m.normals) {
            sb.Append (string.Format ("vn {0} {1} {2}\n", v.x, v.y, v.z));
        }
        sb.Append ("\n");
        foreach (Vector3 v in m.uv) {
            sb.Append (string.Format ("vt {0} {1}\n", v.x, v.y));
        }
        for (int material = 0; material < m.subMeshCount; material++) {
            sb.Append ("\n");
            sb.Append ("usemtl ").Append (mats[material].name).Append ("\n");
            sb.Append ("usemap ").Append (mats[material].name).Append ("\n");

            int[] triangles = m.GetTriangles (material);
            for (int i = 0; i < triangles.Length; i += 3) {
                sb.Append (string.Format ("f {0}/{0}/{0} {1}/{1}/{1} {2}/{2}/{2}\n",
                    triangles[i] + 1, triangles[i + 1] + 1, triangles[i + 2] + 1));
            }
        }
        return sb.ToString ();
    }

    public static string MeshToString (Mesh m, string name, string matName) {
        StringBuilder sb = new StringBuilder ();

        sb.Append ("g ").Append (name).Append ("\n");
        foreach (Vector3 v in m.vertices) {
            sb.Append (string.Format ("v {0} {1} {2}\n", v.x, v.y, v.z));
        }
        sb.Append ("\n");
        foreach (Vector3 v in m.normals) {
            sb.Append (string.Format ("vn {0} {1} {2}\n", v.x, v.y, v.z));
        }
        sb.Append ("\n");
        foreach (Vector3 v in m.uv) {
            sb.Append (string.Format ("vt {0} {1}\n", v.x, v.y));
        }
        for (int material = 0; material < m.subMeshCount; material++) {
            sb.Append ("\n");
            sb.Append ("usemtl ").Append (matName).Append ("\n");
            sb.Append ("usemap ").Append (matName).Append ("\n");

            int[] triangles = m.GetTriangles (material);
            for (int i = 0; i < triangles.Length; i += 3) {
                sb.Append (string.Format ("f {0}/{0}/{0} {1}/{1}/{1} {2}/{2}/{2}\n",
                    triangles[i] + 1, triangles[i + 1] + 1, triangles[i + 2] + 1));
            }
        }
        return sb.ToString ();
    }

    public static void MeshFilterToFile (MeshFilter mf, string filename) {
        using (StreamWriter sw = new StreamWriter ("./Assets/Resources/" + filename + ".obj")) {
            sw.Write (MeshFilterToString (mf));
        }
    }

    public static void MeshToFile (Mesh m, string name, string matName) {
        using (StreamWriter sw = new StreamWriter ("./Assets/Resources/" + name + ".obj")) {
            sw.Write (MeshToString (m, name, matName));
            sw.Close();
        }
    }

}