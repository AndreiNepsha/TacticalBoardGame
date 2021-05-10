using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class GameBoardMesh : MonoBehaviour
{
    Mesh hexMesh;
	List<Vector3> vertices = new List<Vector3>();
	List<int> triangles = new List<int>();
    List<Color> colors = new List<Color>();

	void Awake () {
		GetComponent<MeshFilter>().mesh = hexMesh = new Mesh();
		hexMesh.name = "Hex Mesh";
	}

    public void Render(IEnumerable<GameBoardCell> cells) {
        hexMesh.Clear();
		vertices.Clear();
		triangles.Clear();
        colors.Clear();
		foreach (var cell in cells) {
			Triangulate(cell);
		}
		hexMesh.vertices = vertices.ToArray();
		hexMesh.triangles = triangles.ToArray();
        hexMesh.colors = colors.ToArray();
		hexMesh.RecalculateNormals();
    }

    private void Triangulate(GameBoardCell cell)
	{
		Vector3 center = cell.transform.localPosition;
        AddTriangle(
            center + GameBoardMetrics.CELL_CORNERS[2], 
            center + GameBoardMetrics.CELL_CORNERS[1],
            center + GameBoardMetrics.CELL_CORNERS[0]);
        AddTriangle(
            center + GameBoardMetrics.CELL_CORNERS[2],
            center + GameBoardMetrics.CELL_CORNERS[0], 
            center + GameBoardMetrics.CELL_CORNERS[3]);
        AddTriangleColor(cell.Color);
        AddTriangleColor(cell.Color);
	}

    private void AddTriangle (Vector3 v1, Vector3 v2, Vector3 v3) {
		int vertexIndex = vertices.Count;
		vertices.Add(v1);
		vertices.Add(v2);
		vertices.Add(v3);
		triangles.Add(vertexIndex);
		triangles.Add(vertexIndex + 1);
		triangles.Add(vertexIndex + 2);
	}

    private void AddTriangleColor(Color color) {
        colors.Add(color);
		colors.Add(color);
		colors.Add(color);
    }
}