using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HexCell : MonoBehaviour {
    public HexCoords coords;
    Color color_;
	public Color color {
		get { return color_; }
		set {
			if (color_ == value)
				return;

			color_ = value;
			Refresh();
		}
	}

	public RectTransform uiRect;
	int elevation_ = int.MinValue;
	public int Elevation {
		get { return elevation_; }
		set {
			if (elevation_ == value)
				return;

			elevation_ = value;
			Vector3 pos = transform.localPosition;
			pos.y = elevation_ * HexMetrics.elevationStep;
			pos.y += (HexMetrics.SampleNoise(pos).y * 2f - 1f) * HexMetrics.elevationPerturbStrength;
			transform.localPosition = pos;

			Vector3 uiPosition = uiRect.localPosition;
			// uiPosition.z = elevation_ * -HexMetrics.elevationStep - 0.1f;
			uiPosition.z = -pos.y - 0.1f;
			uiRect.localPosition = uiPosition;

			Refresh();
		}
	}

	int status_ = 0;
	public int Status {
		get { return status_; }
		set {
			if (status_ == value)
				return;
			status_ = value;
			RefreshFeatures();
		}
	}

	HexGrid hexGrid_ = null;
	HexGridChunk chunck_ = null;

	public HexCell GetNeighbor(HexDirection direction) {
		if (hexGrid_ == null) {
			hexGrid_ = GetComponentInParent<HexGrid>();
			if (hexGrid_ == null) {
				Debug.LogAssertion("Hex grid must be parent of Hex Cell");
				return null;
			}
		}
		return hexGrid_.GetCell(coords + direction.ToHexCoords());
	}

	public HexEdgeType GetEdgeType(HexDirection dir) {
		return HexMetrics.GetEdgeType(
			elevation_, GetNeighbor(dir).elevation_
		);
	}

	public HexEdgeType GetEdgeType(HexCell otherCell) {
		return HexMetrics.GetEdgeType(
			elevation_, otherCell.elevation_
		);
	}

	public HexGrid GetHexGrid() {
		if (hexGrid_ == null) {
			hexGrid_ = GetComponentInParent<HexGrid>();
			if (hexGrid_ == null) {
				Debug.LogAssertion("Hex grid must be parent of Hex Cell");
				return null;
			}
		}
		return hexGrid_;
	}

	public void Refresh() {
		if (chunck_ == null) {
			chunck_ = GetComponentInParent<HexGridChunk>();
			if (chunck_ == null) {
				Debug.Log($"Cell({Position} not found parent chunk)");
				return;
			}
		}

		chunck_.Refresh();
		for (int k = 0; k <= (int)HexDirection.NW; ++k) {
			var neighbor = GetNeighbor((HexDirection)k);
			if (neighbor != null && neighbor.chunck_ != null && neighbor.chunck_ != chunck_) {
				neighbor.chunck_.Refresh();
			}
		}
	}

	public void RefreshFeatures() {
		if (chunck_ == null) {
			chunck_ = GetComponentInParent<HexGridChunk>();
			if (chunck_ == null) {
				Debug.Log($"Cell({Position} not found parent chunk)");
				return;
			}
		}

		chunck_.RefreshFeatures();
	}

	public Vector3 Position {
		get { return transform.localPosition; }
	}
}
