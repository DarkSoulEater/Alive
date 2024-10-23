using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HexMesh : MonoBehaviour {
    Mesh hexMesh_;
    MeshCollider meshCollider_;

    [NonSerialized] List<Vector3> vertices_;
    [NonSerialized] List<int> triangles_;
    [NonSerialized] List<Color> colors_;
    [NonSerialized] List<Vector2> uvs_;

    public bool useCollider;
    public bool useColor;
    public bool useUVCoords;

    void Awake() {
        GetComponent<MeshFilter>().mesh = hexMesh_ = new Mesh();
        hexMesh_.name = "Hex Mesh";

        if (useCollider) {
            meshCollider_ = gameObject.AddComponent<MeshCollider>();
        }
    }

    public void Clear() {
        hexMesh_.Clear();
		vertices_ = ListPool<Vector3>.Get();
        if (useColor) {
		    colors_ = ListPool<Color>.Get();
        }
        if (useUVCoords) {
            uvs_ = ListPool<Vector2>.Get();
        }
		triangles_ = ListPool<int>.Get();
    }

    public void Apply() {
        hexMesh_.SetVertices(vertices_);
        ListPool<Vector3>.Add(vertices_);

        if (useColor) {
            hexMesh_.SetColors(colors_);
            ListPool<Color>.Add(colors_);
        }

        if (useUVCoords) {
            hexMesh_.SetUVs(0, uvs_);
            ListPool<Vector2>.Add(uvs_);
        }

		hexMesh_.SetTriangles(triangles_, 0);
        ListPool<int>.Add(triangles_);

		hexMesh_.RecalculateNormals();
        if (useCollider) {
    		meshCollider_.sharedMesh = hexMesh_;
        }
    }

    // Triangle
    public void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3) {
		int vertexIndex = vertices_.Count;

		vertices_.Add(HexMetrics.Perturb(v1));
		vertices_.Add(HexMetrics.Perturb(v2));
		vertices_.Add(HexMetrics.Perturb(v3));

		triangles_.Add(vertexIndex);
		triangles_.Add(vertexIndex + 1);
		triangles_.Add(vertexIndex + 2);
    }

    public void AddTriangleUnperturbed (Vector3 v1, Vector3 v2, Vector3 v3) {
		int vertexIndex = vertices_.Count;

		vertices_.Add(v1);
		vertices_.Add(v2);
		vertices_.Add(v3);

		triangles_.Add(vertexIndex);
		triangles_.Add(vertexIndex + 1);
		triangles_.Add(vertexIndex + 2);
	}

    public void AddTriangleColor(Color color) {
		colors_.Add(color);
		colors_.Add(color);
		colors_.Add(color);
	}

    public void AddTriangleColor(Color c1, Color c2, Color c3) {
		colors_.Add(c1);
		colors_.Add(c2);
		colors_.Add(c3);
	}

    // Quad
    public void AddQuad(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4) {
		int vertexIndex = vertices_.Count;
		vertices_.Add(HexMetrics.Perturb(v1));
		vertices_.Add(HexMetrics.Perturb(v2));
		vertices_.Add(HexMetrics.Perturb(v3));
		vertices_.Add(HexMetrics.Perturb(v4));

		triangles_.Add(vertexIndex);
		triangles_.Add(vertexIndex + 2);
		triangles_.Add(vertexIndex + 1);
		triangles_.Add(vertexIndex + 1);
		triangles_.Add(vertexIndex + 2);
		triangles_.Add(vertexIndex + 3);
	}

	public void AddQuadColor(Color c1, Color c2, Color c3, Color c4) {
		colors_.Add(c1);
		colors_.Add(c2);
		colors_.Add(c3);
		colors_.Add(c4);
	}

    public void AddQuadColor(Color c1, Color c2) {
		colors_.Add(c1);
		colors_.Add(c1);
		colors_.Add(c2);
		colors_.Add(c2);
	}

    // UV
    public void AddTriangleUV(Vector2 uv1, Vector2 uv2, Vector3 uv3) {
		uvs_.Add(uv1);
		uvs_.Add(uv2);
		uvs_.Add(uv3);
	}
	
	public void AddQuadUV(Vector2 uv1, Vector2 uv2, Vector3 uv3, Vector3 uv4) {
		uvs_.Add(uv1);
		uvs_.Add(uv2);
		uvs_.Add(uv3);
		uvs_.Add(uv4);
	}

    public void AddQuadUV(float uMin, float uMax, float vMin, float vMax) {
		uvs_.Add(new Vector2(uMin, vMin));
		uvs_.Add(new Vector2(uMax, vMin));
		uvs_.Add(new Vector2(uMin, vMax));
		uvs_.Add(new Vector2(uMax, vMax));
	}
}
