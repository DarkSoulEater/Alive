using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexGridChunk : MonoBehaviour
{
    HexCell[,] cells_;

    public HexMesh terrain;
    public HexFeatureManager features;
    Canvas gridCanvas_;

    bool triangulateOnlyFeatures_;

    void Awake() {
        gridCanvas_ = GetComponentInChildren<Canvas>();

        cells_ = new HexCell[HexMetrics.chunkSizeX, HexMetrics.chunkSizeZ];
    }
    public void Refresh() {
        enabled = true;
    }

    public void RefreshFeatures() {
        enabled = true;
        triangulateOnlyFeatures_ = true;
    }

    void LateUpdate() {
        if (triangulateOnlyFeatures_) {
            TriangulateFeatures();
        } else {
            Triangulate();
        }
        enabled = false;
    }

    public void AddCell(int x, int z, HexCell cell) {
        cells_[x, z] = cell;
        cell.transform.SetParent(transform, false);
        cell.uiRect.SetParent(gridCanvas_.transform, false);
    }

    void Triangulate() {
        terrain.Clear();
        features.Clear();
        for (int z = 0; z < cells_.GetLength(1); ++z) {
            for (int x = 0; x < cells_.GetLength(0); ++x) {
                Triangulate(cells_[x, z]);
            }
        }
        terrain.Apply();
        features.Apply();
    }

    void TriangulateFeatures() {
        features.Clear();
        for (int z = 0; z < cells_.GetLength(1); ++z) {
            for (int x = 0; x < cells_.GetLength(0); ++x) {
                features.AddFeature(cells_[x, z]);
            }
        }
        features.Apply();
    }

    // Triangulate // TODO: Refactor is

    void Triangulate(HexCell cell) {
        for (HexDirection dir = HexDirection.NE; dir <= HexDirection.NW; ++dir) {
            Triangulate(dir, cell);
        }
        // features.AddFeature(cell.Position);
    }

    void Triangulate(HexDirection dir, HexCell cell) {
        Vector3 center = cell.Position;
		EdgeVertices e = new EdgeVertices(
			center + HexMetrics.GetFirstSolidCorner(dir),
			center + HexMetrics.GetSecondSolidCorner(dir)
		);

		TriangulateEdgeFan(center, e, cell.color);

        if (dir <= HexDirection.SE) {
            TriangulateBridge(dir, cell, e);
        }

        if (dir <= HexDirection.E) {
            TriangulateCorner(dir, cell, e);
        }
    }

    void TriangulateBridge(HexDirection dir, HexCell cell, EdgeVertices e1) {
        HexCell neighbor = cell.GetNeighbor(dir);
        if (neighbor == null) 
            return;

        Vector3 bridge = HexMetrics.GetBridge(dir);
		bridge.y = neighbor.Position.y - cell.Position.y;
		EdgeVertices e2 = new EdgeVertices(
			e1.v1 + bridge,
			e1.v4 + bridge
        );

        if (cell.GetEdgeType(dir) == HexEdgeType.Slope) {
            TriangulateBridgeTerraces(e1, cell, e2, neighbor);
        } else {
			TriangulateEdgeStrip(e1, cell.color, e2, neighbor.color);
        }
    }

    void TriangulateBridgeTerraces (
		EdgeVertices begin, HexCell beginCell,
		EdgeVertices end, HexCell endCell
	) {
        EdgeVertices e2 = EdgeVertices.TerraceLerp(begin, end, 1);
		Color c2 = HexMetrics.TerraceLerp(beginCell.color, endCell.color, 1);

		TriangulateEdgeStrip(begin, beginCell.color, e2, c2);

        for (int i = 2; i < HexMetrics.terraceSteps; i++) {
			EdgeVertices e1 = e2;
			Color c1 = c2;
			e2 = EdgeVertices.TerraceLerp(begin, end, i);
			c2 = HexMetrics.TerraceLerp(beginCell.color, endCell.color, i);
			TriangulateEdgeStrip(e1, c1, e2, c2);
		}

		TriangulateEdgeStrip(e2, c2, end, endCell.color);
	}

    void TriangulateCorner(HexDirection dir, HexCell cell, EdgeVertices e1) {
        HexCell neighbor = cell.GetNeighbor(dir);
        HexCell nextNeighbor = cell.GetNeighbor(dir.Next());
		if (neighbor == null || nextNeighbor == null)
            return;

        Vector3 bridge = HexMetrics.GetBridge(dir);
		bridge.y = neighbor.Position.y - cell.Position.y;
		EdgeVertices e2 = new EdgeVertices(
			e1.v1 + bridge,
			e1.v4 + bridge
        );

        Vector3 v5 = e1.v4 + HexMetrics.GetBridge(dir.Next());
        v5.y = nextNeighbor.Position.y;

        if (cell.Elevation <= neighbor.Elevation) {
            if (cell.Elevation <= nextNeighbor.Elevation) {
                TriangulateCornerTerrace(e1.v4, cell, e2.v4, neighbor, v5, nextNeighbor);
            }
            else {
                TriangulateCornerTerrace(v5, nextNeighbor, e1.v4, cell, e2.v4, neighbor);
            }
        }
        else if (neighbor.Elevation <= nextNeighbor.Elevation) {
            TriangulateCornerTerrace(e2.v4, neighbor, v5, nextNeighbor, e1.v4, cell);
        }
        else {
            TriangulateCornerTerrace(v5, nextNeighbor, e1.v4, cell, e2.v4, neighbor);
        }
    }

    void TriangulateCornerTerrace(
        Vector3 bottom, HexCell bottomCell,
		Vector3 left, HexCell leftCell,
		Vector3 right, HexCell rightCell
    ) {
        HexEdgeType leftEdgeType = bottomCell.GetEdgeType(leftCell);
		HexEdgeType rightEdgeType = bottomCell.GetEdgeType(rightCell);

		if (leftEdgeType == HexEdgeType.Slope) {
			if (rightEdgeType == HexEdgeType.Slope) {
				TriangulateCornerTerraces_SF(
					bottom, bottomCell, left, leftCell, right, rightCell
				);
			}
			else if (rightEdgeType == HexEdgeType.Flat) {
				TriangulateCornerTerraces_SF(
					left, leftCell, right, rightCell, bottom, bottomCell
				);
			}
			else {
				TriangulateCornerTerracesCliff(
					bottom, bottomCell, left, leftCell, right, rightCell
				);
			}
		}
		else if (rightEdgeType == HexEdgeType.Slope) {
			if (leftEdgeType == HexEdgeType.Flat) {
				TriangulateCornerTerraces_SF(
					right, rightCell, bottom, bottomCell, left, leftCell
				);
			}
			else {
				TriangulateCornerCliffTerraces(
					bottom, bottomCell, left, leftCell, right, rightCell
				);
			}
		}
		else if (leftCell.GetEdgeType(rightCell) == HexEdgeType.Slope) {
			if (leftCell.Elevation < rightCell.Elevation) {
				TriangulateCornerCliffTerraces(
					right, rightCell, bottom, bottomCell, left, leftCell
				);
			}
			else {
				TriangulateCornerTerracesCliff(
					left, leftCell, right, rightCell, bottom, bottomCell
				);
			}
		}
		else {
			terrain.AddTriangle(bottom, left, right);
			terrain.AddTriangleColor(bottomCell.color, leftCell.color, rightCell.color);
		}
    }

    void TriangulateCornerTerraces_SF (
		Vector3 begin, HexCell beginCell,
		Vector3 left, HexCell leftCell,
		Vector3 right, HexCell rightCell
	) {
        Vector3 v3 = HexMetrics.TerraceLerp(begin, left, 1);
        Vector3 v4 = HexMetrics.TerraceLerp(begin, right, 1);
		Color c3 = HexMetrics.TerraceLerp(beginCell.color, leftCell.color, 1);
		Color c4 = HexMetrics.TerraceLerp(beginCell.color, rightCell.color, 1);

		terrain.AddTriangle(begin, v3, v4);
		terrain.AddTriangleColor(beginCell.color, c3, c4);

        for (int i = 2; i < HexMetrics.terraceSteps; i++) {
			Vector3 v1 = v3;
			Vector3 v2 = v4;
			Color c1 = c3;
			Color c2 = c4;
			v3 = HexMetrics.TerraceLerp(begin, left, i);
			v4 = HexMetrics.TerraceLerp(begin, right, i);
			c3 = HexMetrics.TerraceLerp(beginCell.color, leftCell.color, i);
			c4 = HexMetrics.TerraceLerp(beginCell.color, rightCell.color, i);
			terrain.AddQuad(v1, v2, v3, v4);
			terrain.AddQuadColor(c1, c2, c3, c4);
		}

        terrain.AddQuad(v3, v4, left, right);
		terrain.AddQuadColor(c3, c4, leftCell.color, rightCell.color);
	}

    void TriangulateCornerTerracesCliff (
		Vector3 begin, HexCell beginCell,
		Vector3 left, HexCell leftCell,
		Vector3 right, HexCell rightCell
	) {
        float b = Mathf.Abs(1f / (rightCell.Elevation - beginCell.Elevation));
		Vector3 boundary = Vector3.Lerp(HexMetrics.Perturb(begin), HexMetrics.Perturb(right), b);
		Color boundaryColor = Color.Lerp(beginCell.color, rightCell.color, b);

		TriangulateBoundaryTriangle(
			begin, beginCell, left, leftCell, boundary, boundaryColor
		);

        if (leftCell.GetEdgeType(rightCell) == HexEdgeType.Slope) {
			TriangulateBoundaryTriangle(
				left, leftCell, right, rightCell, boundary, boundaryColor
			);
		}
		else {
			terrain.AddTriangleUnperturbed(HexMetrics.Perturb(left), HexMetrics.Perturb(right), boundary);
			terrain.AddTriangleColor(leftCell.color, rightCell.color, boundaryColor);
		}
	}

    void TriangulateBoundaryTriangle (
		Vector3 begin, HexCell beginCell,
		Vector3 left, HexCell leftCell,
		Vector3 boundary, Color boundaryColor
	) {
		Vector3 v2 = HexMetrics.Perturb(HexMetrics.TerraceLerp(begin, left, 1));
		Color c2 = HexMetrics.TerraceLerp(beginCell.color, leftCell.color, 1);

		terrain.AddTriangleUnperturbed(HexMetrics.Perturb(begin), v2, boundary);
		terrain.AddTriangleColor(beginCell.color, c2, boundaryColor);

		for (int i = 2; i < HexMetrics.terraceSteps; i++) {
			Vector3 v1 = v2;
			Color c1 = c2;
			v2 = HexMetrics.Perturb(HexMetrics.TerraceLerp(begin, left, i));
			c2 = HexMetrics.TerraceLerp(beginCell.color, leftCell.color, i);
			terrain.AddTriangleUnperturbed(v1, v2, boundary);
			terrain.AddTriangleColor(c1, c2, boundaryColor);
		}

		terrain.AddTriangleUnperturbed(v2, HexMetrics.Perturb(left), boundary);
		terrain.AddTriangleColor(c2, leftCell.color, boundaryColor);
	}

    void TriangulateCornerCliffTerraces (
		Vector3 begin, HexCell beginCell,
		Vector3 left, HexCell leftCell,
		Vector3 right, HexCell rightCell
	) {
		float b = Mathf.Abs(1f / (leftCell.Elevation - beginCell.Elevation));
		Vector3 boundary = Vector3.Lerp(HexMetrics.Perturb(begin), HexMetrics.Perturb(left), b);
		Color boundaryColor = Color.Lerp(beginCell.color, leftCell.color, b);

		TriangulateBoundaryTriangle(
			right, rightCell, begin, beginCell, boundary, boundaryColor
		);

		if (leftCell.GetEdgeType(rightCell) == HexEdgeType.Slope) {
			TriangulateBoundaryTriangle(
				left, leftCell, right, rightCell, boundary, boundaryColor
			);
		}
		else {
			terrain.AddTriangleUnperturbed(HexMetrics.Perturb(left), HexMetrics.Perturb(right), boundary);
			terrain.AddTriangleColor(leftCell.color, rightCell.color, boundaryColor);
		}
	}

    void TriangulateEdgeFan (Vector3 center, EdgeVertices edge, Color color) {
		terrain.AddTriangle(center, edge.v1, edge.v2);
		terrain.AddTriangleColor(color);
		terrain.AddTriangle(center, edge.v2, edge.v3);
		terrain.AddTriangleColor(color);
		terrain.AddTriangle(center, edge.v3, edge.v4);
		terrain.AddTriangleColor(color);
	}

    void TriangulateEdgeStrip (
		EdgeVertices e1, Color c1,
		EdgeVertices e2, Color c2
	) {
		terrain.AddQuad(e1.v1, e1.v2, e2.v1, e2.v2);
		terrain.AddQuadColor(c1, c2);
		terrain.AddQuad(e1.v2, e1.v3, e2.v2, e2.v3);
		terrain.AddQuadColor(c1, c2);
		terrain.AddQuad(e1.v3, e1.v4, e2.v3, e2.v4);
		terrain.AddQuadColor(c1, c2);
	}
}
