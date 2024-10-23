using UnityEngine;

public static class HexMetrics { 
	public const float outerRadius = 10f;

	public const float innerRadius = outerRadius * 0.866025404f; // sqrt(3) / 2

	public const float solidFactor = 0.8f;
    public const float blendFactor = 1f - solidFactor;

	public const float elevationStep = 3f;
    public const float cellPerturbStrength = 3f;
    public const float elevationPerturbStrength = 1.5f;

	public const int terracesPerSlope = 2;

	public const int terraceSteps = terracesPerSlope * 2 + 1;

	public const float horizontalTerraceStepSize = 1f / terraceSteps;
	public const float verticalTerraceStepSize = 1f / (terracesPerSlope + 1);
	
	public static Vector3 TerraceLerp(Vector3 a, Vector3 b, int step) {
		float h = step * HexMetrics.horizontalTerraceStepSize;
		a.x += (b.x - a.x) * h;
		a.z += (b.z - a.z) * h;
		float v = ((step + 1) / 2) * HexMetrics.verticalTerraceStepSize;
		a.y += (b.y - a.y) * v;
		return a;
	}

	public static Color TerraceLerp (Color a, Color b, int step) {
		float h = step * HexMetrics.horizontalTerraceStepSize;
		return Color.Lerp(a, b, h);
	}

	public static HexEdgeType GetEdgeType (int elevation1, int elevation2) {
		if (elevation1 == elevation2) {
			return HexEdgeType.Flat;
		}
		int delta = elevation2 - elevation1;
		if (delta == 1 || delta == -1) {
			return HexEdgeType.Slope;
		}
		return HexEdgeType.Cliff;
	}

    static Vector3[] corners = {
        new Vector3(0f, 0f, outerRadius),
        new Vector3(innerRadius, 0f, 0.5f * outerRadius),
        new Vector3(innerRadius, 0f, -0.5f * outerRadius),
        new Vector3(0f, 0f, -outerRadius),
        new Vector3(-innerRadius, 0f, -0.5f * outerRadius),
        new Vector3(-innerRadius, 0f, 0.5f * outerRadius),
        new Vector3(0f, 0f, outerRadius) // Solve Out of range for cycle search 
    };

	public static Vector3 GetFirstCorner(HexDirection direction) {
		return corners[(int)direction];
	}

	public static Vector3 GetSecondCorner(HexDirection direction) {
		return corners[(int)direction + 1];
	}

	public static Vector3 GetFirstSolidCorner(HexDirection direction) {
		return corners[(int)direction] * solidFactor;
	}

	public static Vector3 GetSecondSolidCorner(HexDirection direction) {
		return corners[(int)direction + 1] * solidFactor;
	}

	public static Vector3 GetBridge(HexDirection direction) {
		return (corners[(int)direction] + corners[(int)direction + 1]) * blendFactor;
	}

    // Noice
    public static Texture2D noiseSource;
    public const float noiseScale = 0.003f;

    public static Vector4 SampleNoise(Vector3 position) {
		return noiseSource.GetPixelBilinear(position.x * noiseScale, position.z * noiseScale);
	}

    public static Vector3 Perturb(Vector3 position) {
		Vector4 sample = HexMetrics.SampleNoise(position);
		position.x += (sample.x * 2f - 1f) * HexMetrics.cellPerturbStrength;
		position.z += (sample.z * 2f - 1f) * HexMetrics.cellPerturbStrength;
		return position;
	}

    // Chuncks
    public const int chunkSizeX = 5, chunkSizeZ = 5;
};


public enum HexEdgeType {
	Flat, Slope, Cliff
}


[System.Serializable]
public struct HexCoords {
    [SerializeField] int x_, z_;

	public int X { get { return x_; } }

	public int Z { get { return z_; } }

    public int Y { get { return -X - Z; } }

	public HexCoords (int x, int z) {
		x_ = x;
		z_ = z;
	}

	public static HexCoords operator+(HexCoords lhs, HexCoords rhs)
		=> new HexCoords(lhs.x_ + rhs.x_, lhs.z_ + rhs.z_);

    public static HexCoords FromOffsetCoords(int x, int z) {
		return new HexCoords(x - z / 2, z);
	}

    public static HexCoords FromPosition(Vector3 position) {
		float x = position.x / (HexMetrics.innerRadius * 2f);
		float y = -x;

        float offset = position.z / (HexMetrics.outerRadius * 3f);
		x -= offset;
		y -= offset;

        int iX = Mathf.RoundToInt(x);
		int iY = Mathf.RoundToInt(y);
		int iZ = Mathf.RoundToInt(-x -y);

        if (iX + iY + iZ != 0) {
			float dX = Mathf.Abs(x - iX);
			float dY = Mathf.Abs(y - iY);
			float dZ = Mathf.Abs(-x -y - iZ);

			if (dX > dY && dX > dZ) {
				iX = -iY - iZ;
			}
			else if (dZ > dY) {
				iZ = -iX - iY;
			}
		}

		return new HexCoords(iX, iZ);
	}

    public Vector2Int ToOffsetCoords() {
        return new Vector2Int(x_ + z_ / 2, z_);
    }

	public override string ToString() {
		return "(" +
			X.ToString() + ", " + Y.ToString() + ", " + Z.ToString() + ")";
	}

	public string ToStringOnSeparateLines() {
		return X.ToString() + "\n" + Y.ToString() + "\n" + Z.ToString();
	}
}


public enum HexDirection {
	NE, E, SE, SW, W, NW
}

public static class HexDirectionExtensions {
	public static HexCoords ToHexCoords(this HexDirection dir) {
		switch (dir) {
			case HexDirection.NE: {
				return new HexCoords(0, 1);
			}
			case HexDirection.E: {
				return new HexCoords(1, 0);
			}
			case HexDirection.SE: {
				return new HexCoords(1, -1);
			}
			case HexDirection.SW: {
				return new HexCoords(0, -1);
			}
			case HexDirection.W: {
				return new HexCoords(-1, 0);
			}
			case HexDirection.NW: {
				return new HexCoords(-1, 1);
			}
		}
		return new HexCoords(0, 0);
	}

	public static HexDirection Opposite(this HexDirection dir) {
		return (int)dir < 3 ? (dir + 3) : (dir - 3);
	}

	public static HexDirection Previous(this HexDirection direction) {
		return direction == HexDirection.NE ? HexDirection.NW : (direction - 1);
	}

	public static HexDirection Next(this HexDirection direction) {
		return direction == HexDirection.NW ? HexDirection.NE : (direction + 1);
	}
}