using UnityEngine;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;

public static class ObjectFileFormat
{
	const string PrefixNormal = "N";
	const string PrefixColor = "C";
	const string PrefixTextureCoordinate = "ST";
	const string PrefixNDimension = "n";
	const string PrefixHomogeneousCoordinate = "4";

	public static Mesh OffToMesh(StreamReader off)
	{
		var mesh = new Mesh();
		var tokens = getTokensOfNonEmptyLines(off);
		var parser = parseOff(mesh);
		while (parser.MoveNext())
		{
			if (tokens.MoveNext())
				parser.Current(tokens.Current);
			else
				throw new Exception("Parse error.");
		}
		off.Close();
		mesh.RecalculateBounds();
		return mesh;
	}



//	public static void MeshToOff(Mesh mesh, TextWriter off, Transform ObjectTransform)
//	{
//		/*if (mesh.uv.Length != 0)
//			off.Write(PrefixTextureCoordinate);*/
//		if (mesh.colors.Length != 0)
//			off.Write(PrefixColor);
//		if (mesh.normals.Length != 0)
//			off.Write(PrefixNormal);
//		off.WriteLine("OFF");
//		var verts = mesh.vertices;
//		var norms = mesh.normals;
//		var colors = mesh.colors;
//		var uvs = mesh.uv;
//		uvs = new Vector2[0];
//		colors = new Color[0];
//		var tris = mesh.triangles;
//		var faceCount = tris.Length / 3;
//
//		off.WriteLine(string.Format("{0} {1} {2}", verts.Length, faceCount, 0));
//		Debug.Log ("Number of vertices : " + verts.Length);
//		for (int i = 0; i < verts.Length; i++)
//		{
//			Vector3 curVert = ObjectTransform.TransformPoint (verts [i]);
//
//			off.Write(curVert.x);
//			off.Write(" ");
//			off.Write(curVert.y);
//			off.Write(" ");
//			off.Write(curVert.z);
//			if (norms.Length != 0)
//			{
//				off.Write(" ");
//				off.Write(norms[i].x);
//				off.Write(" ");
//				off.Write(norms[i].y);
//				off.Write(" ");
//				off.Write(norms[i].z);
//			}
//			if (colors.Length != 0)
//			{
//				off.Write(" ");
//				off.Write(colors[i].r);
//				off.Write(" ");
//				off.Write(colors[i].g);
//				off.Write(" ");
//				off.Write(colors[i].b);
//				off.Write(" ");
//				off.Write(colors[i].a);
//			}
//			if (uvs.Length != 0)
//			{
//				off.Write(" ");
//				off.Write(uvs[i].x);
//				off.Write(" ");
//				off.Write(uvs[i].y);
//			}
//			off.WriteLine();
//		}
//
//		for(int i = 0; i < faceCount; i++)
//		{
//
//			int ind1 = i*3;
//			int ind2 = i * 3 + 1;
//			int ind3 = i * 3 + 2;
//
//			//check area
//			Vector3 A = verts[tris[ind1]];
//			Vector3 B = verts[tris[ind2]];
//			Vector3 C = verts[tris[ind3]];
//			Vector3 V = Vector3.Cross(A-B, A-C);
//			float area = V.magnitude * 0.5f;
//
//			Debug.Log (area);
//	
//
//			off.Write("3 ");
//			off.Write(tris[ind1]);
//			off.Write(" ");
//			off.Write(tris[ind2]);
//			off.Write(" ");
//			off.WriteLine(tris[ind3]);
//		}
//		off.Close();
//	}

	public static string MeshToOff(Mesh mesh, Transform ObjectTransform)
	{
		StringBuilder strB = new StringBuilder ();
		if (mesh.colors.Length != 0)
			strB.Append(PrefixColor);
		if (mesh.normals.Length != 0)
			strB.Append(PrefixNormal);
		strB.AppendLine("OFF");
		var verts = mesh.vertices;
		var norms = mesh.normals;
		var colors = mesh.colors;
		var uvs = mesh.uv;
		uvs = new Vector2[0];
		colors = new Color[0];
		var tris = mesh.triangles;
		var faceCount = tris.Length / 3;

		strB.AppendLine(string.Format("{0} {1} {2}", verts.Length, faceCount, 0));
		for (int i = 0; i < verts.Length; i++)
		{
			var curVert = ObjectTransform.TransformPoint (verts [i]);
			strB.Append(curVert.x);
			strB.Append(" ");
			strB.Append(curVert.y);
			strB.Append(" ");
			strB.Append(curVert.z);
			if (norms.Length != 0)
			{
				strB.Append(" ");
				strB.Append(norms[i].x);
				strB.Append(" ");
				strB.Append(norms[i].y);
				strB.Append(" ");
				strB.Append(norms[i].z);
			}
			if (colors.Length != 0)
			{
				strB.Append(" ");
				strB.Append(colors[i].r);
				strB.Append(" ");
				strB.Append(colors[i].g);
				strB.Append(" ");
				strB.Append(colors[i].b);
				strB.Append(" ");
				strB.Append(colors[i].a);
			}
			if (uvs.Length != 0)
			{
				strB.Append(" ");
				strB.Append(uvs[i].x);
				strB.Append(" ");
				strB.Append(uvs[i].y);
			}
			strB.AppendLine();
		}

		for(int i = 0; i < faceCount; i++)
		{
			int ind1 = i * 3;
			int ind2 = i * 3 + 1;
			int ind3 = i * 3 + 2;

			////check area size (degenerate faces if area==0)
			//Vector3 A = verts[tris[ind1]];
			//Vector3 B = verts[tris[ind2]];
			//Vector3 C = verts[tris[ind3]];
			//Vector3 V = Vector3.Cross(A-B, A-C);
			//float area = V.magnitude * 0.5f;

			//Debug.Log (area);

			strB.Append("3 ");
			strB.Append(tris[ind1]);
			strB.Append(" ");
			strB.Append(tris[ind2]);
			strB.Append(" ");
			strB.AppendLine(tris[ind3].ToString());
		}

		return strB.ToString ();
	}



	public static string MeshToOffClean(Mesh mesh, Transform ObjectTransform)
	{
		StringBuilder strB = new StringBuilder ();
		if (mesh.colors.Length != 0)
			strB.Append(PrefixColor);
		if (mesh.normals.Length != 0)
			strB.Append(PrefixNormal);
		strB.AppendLine("OFF");
		var verts = mesh.vertices;
		var norms = mesh.normals;
		var colors = mesh.colors;
		var uvs = mesh.uv;
		uvs = new Vector2[0];
		colors = new Color[0];
		var tris = mesh.triangles;
		var faceCount = tris.Length / 3;
		List<Vector3> geomVerts = new List<Vector3> ();
		List<int> vertsIndices = new List<int> ();

		for (int i = 0; i < faceCount; i++) {
			int ind1 = i * 3;
			int ind2 = i * 3 + 1;
			int ind3 = i * 3 + 2;

			if (!geomVerts.Contains (verts [tris [ind1]])) {
				geomVerts.Add (verts[tris[ind1]]);
				vertsIndices.Add (tris [ind1]);
			}
				
			if (!geomVerts.Contains (verts [tris [ind2]])) {
				vertsIndices.Add (tris [ind2]);
				geomVerts.Add (verts[tris[ind2]]);
			}
				
			if (!geomVerts.Contains (verts [tris [ind3]])) {
				vertsIndices.Add (tris [ind3]);
				geomVerts.Add (verts[tris[ind3]]);
			}

		}
	

		strB.AppendLine(string.Format("{0} {1} {2}", geomVerts.Count, faceCount, 0));
	
		for(int i = 0; i < geomVerts.Count; i++)
		{
				
			var curVert = ObjectTransform.TransformPoint (geomVerts[i]);
			var curInd = vertsIndices [i];
			strB.Append(curVert.x);
			strB.Append(" ");
			strB.Append(curVert.y);
			strB.Append(" ");
			strB.Append(curVert.z);
			if (norms.Length != 0)
			{
				strB.Append(" ");
				strB.Append(norms[curInd].x);
				strB.Append(" ");
				strB.Append(norms[curInd].y);
				strB.Append(" ");
				strB.Append(norms[curInd].z);
			}
			if (colors.Length != 0)
			{
				strB.Append(" ");
				strB.Append(colors[curInd].r);
				strB.Append(" ");
				strB.Append(colors[curInd].g);
				strB.Append(" ");
				strB.Append(colors[curInd].b);
				strB.Append(" ");
				strB.Append(colors[curInd].a);
			}
			if (uvs.Length != 0)
			{
				strB.Append(" ");
				strB.Append(uvs[curInd].x);
				strB.Append(" ");
				strB.Append(uvs[curInd].y);
			}
			strB.AppendLine();
			
		}

		for (int i = 0; i < faceCount; i++) {
			int ind1 = i * 3;
			int ind2 = i * 3 + 1;
			int ind3 = i * 3 + 2;
			strB.Append ("3 ");
			strB.Append (tris [ind1]);
			strB.Append (" ");
			strB.Append (tris [ind2]);
			strB.Append (" ");
			strB.AppendLine (tris [ind3].ToString ());
		}

		return strB.ToString ();
	}

	static IEnumerator<string[]> getTokensOfNonEmptyLines(StreamReader off)
	{
		var re = new Regex(@"\s+");
		while (off.Peek() > 0)
		{
			var line = off.ReadLine();
			var sharp = line.IndexOf("#");
			if (sharp >= 0)
			{
				line = line.Substring(0, sharp);
			}
			line = line.Trim(" \t\n\r".ToCharArray());
			if (line.Length > 0) yield return re.Split(line);
		}
	}

	static IEnumerator<Action<string[]>> parseOff(Mesh mesh)
	{
		var hasNormal = false;
		var hasColor = false;
		var hasUv = false;
		var hasHomo = false;
		var hasDim = false;
		var dim = 3;

		var vertexCount = 0;
		var faceCount = 0;

		// Parse Header
		yield return tokens =>
		{
			if (tokens.Length != 1){
				foreach(String str in tokens)
					Debug.Log(str);
				throw new Exception("Invalid OFF header: (tokens.Length != 1) ");
			}
				
			var re = new Regex("(?<ST>ST)?(?<C>C)?(?<N>N)?(?<4>4)?(?<n>n)?OFF");
			var match = re.Match(tokens[0]);
			if (!match.Success)
				throw new Exception("Invalid OFF header : " + tokens[0]);

			hasNormal = match.Groups[PrefixNormal].Value == PrefixNormal;
			hasColor = match.Groups[PrefixColor].Value == PrefixColor;
			hasUv = match.Groups[PrefixTextureCoordinate].Value == PrefixTextureCoordinate;
			hasHomo = match.Groups[PrefixHomogeneousCoordinate].Value == PrefixHomogeneousCoordinate;
			hasDim = match.Groups[PrefixNDimension].Value == PrefixNDimension;
		};

		// Dimension
		if (hasDim)
		{
			yield return tokens =>
			{
				if (tokens.Length != 1
					|| !int.TryParse(tokens[0], out dim)
					|| dim > 3)
				{
					throw new Exception("Dimension should not be more than 3.");
				}
			};
		}

		// Counts
		yield return tokens =>
		{
			if (!int.TryParse(tokens[0], out vertexCount)
				|| !int.TryParse(tokens[1], out faceCount))
				throw new Exception("Invalid vertex or face count.");
		};

		// Vertex
		var verts = new Vector3[vertexCount];
		var normals = hasNormal ? new Vector3[vertexCount] : null;
		var colors = hasColor ? new Color[vertexCount] : null;
		var uvs = hasUv ? new Vector2[vertexCount] : null;
		var normOff = hasHomo ? dim + 1 : dim;
		var colOff = hasNormal ? normOff + dim : normOff;
		var uvOff = hasColor ? colOff + 4 : colOff;
		int i = 0;
		Action<string[]> parseVert = tokens =>
		{
			var w = 1f;
			if ((dim > 0 && !float.TryParse(tokens[0], out verts[i].x)) ||
				(dim > 1 && !float.TryParse(tokens[1], out verts[i].y)) ||
				(dim > 2 && !float.TryParse(tokens[2], out verts[i].z)) ||
				(hasHomo && !float.TryParse(tokens[dim], out w)) ||
				(hasNormal && !(
					float.TryParse(tokens[normOff + 0], out normals[i].x) &&
					float.TryParse(tokens[normOff + 1], out normals[i].y) &&
					float.TryParse(tokens[normOff + 2], out normals[i].z))) ||
				(hasColor && !(
					float.TryParse(tokens[colOff + 0], out colors[i].r) &&
					float.TryParse(tokens[colOff + 1], out colors[i].g) &&
					float.TryParse(tokens[colOff + 2], out colors[i].b) &&
					float.TryParse(tokens[colOff + 3], out colors[i].a))) ||
				(hasUv && !(
					float.TryParse(tokens[uvOff + 0], out uvs[i].x) &&
					float.TryParse(tokens[uvOff + 1], out uvs[i].y))))
			{
				throw new Exception("Vertex Parse error: " + i + ".");
			}
			if (hasHomo)
				verts[i] /= w;
		};
		for (i = 0; i < vertexCount; i++)
		{
			yield return parseVert;
		}

		// Indexes
		var tris = new int[faceCount * 3];
		Action<string[]> parseFace = tokens =>
		{
			if (tokens[0] != "3" ||
				!int.TryParse(tokens[1], out tris[i * 3 + 0]) ||
				!int.TryParse(tokens[2], out tris[i * 3 + 1]) ||
				!int.TryParse(tokens[3], out tris[i * 3 + 2]))
			{
				throw new Exception("Face Parse error.");
			}
		};
		for (i = 0; i < faceCount; i++)
		{
			yield return parseFace;
		}

		mesh.vertices = verts;
		mesh.normals = normals;
		mesh.colors = colors;
		mesh.uv = uvs;
		mesh.triangles = tris;
	}
}
