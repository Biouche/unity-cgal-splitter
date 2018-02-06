using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Text;
using System.Runtime.InteropServices;

public class CGALController : MonoBehaviour {
    [DllImport("cgal-plugin")]
    public static extern IntPtr booleanOperation(IntPtr offFile1, IntPtr offFile2, IntPtr operationName);
    [DllImport("cgal-plugin")]
    public static extern int checkIntersection(IntPtr cuttedMeshOff, IntPtr cutMeshOff);
	[DllImport("cgal-plugin")]
	public static extern IntPtr booleanOperationClean(IntPtr offFile1, IntPtr transform1, IntPtr offFile2, IntPtr operationName);
    void Start()
	{
		#if UNITY_WEBGL && !UNITY_EDITOR
		RegisterPlugin();
		#endif
	}
}
