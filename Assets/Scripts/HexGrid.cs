using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Compression;
using TMPro;
using UnityEngine;

public class HexGrid : MonoBehaviour
{
    public int chunkCountX = 4;
    public int chunkCountZ = 3;
    int cellCountX_;
    int cellCountZ_;

    public HexCell cellPrefab;
    public TMP_Text cellLabelPrefab;
    public HexGridChunk chunkPrefab;

    HexGridChunk[,] chunks_ = null;
    HexCell[,] cells_ = null;

    public bool HexCoordsLable;

    public Color defaultColor = Color.white;
	public Color touchedColor = Color.magenta;

    public Texture2D noiceSource;

    void Awake() {
        HexMetrics.noiseSource = noiceSource;
    }

    void OnEnable() {
        HexMetrics.noiseSource = noiceSource;
    }

    public void Create(int newChunkCountX, int newChunkCountZ) {
        Resize(newChunkCountX, newChunkCountZ);
        CreateRandomElevation();
    }

    void Resize(int newChunkCountX, int newChunkCountZ) {
        chunkCountX = newChunkCountX;
        chunkCountZ = newChunkCountZ;

        cellCountX_ = chunkCountX * HexMetrics.chunkSizeX;
        cellCountZ_ = chunkCountZ * HexMetrics.chunkSizeZ;

        if (cells_ != null) {
            foreach (HexCell cell in cells_) {
                Destroy(cell.gameObject);
            }
        }

        cells_ = new HexCell[cellCountX_, cellCountZ_];

        CreateChunks();
        CreateCells();
    }

    void CreateCells() {
        for (int z = 0; z < cellCountZ_; ++z) {
            for (int x = 0; x < cellCountX_; ++x) {
                CreateCell(x, z);
            }
        }
    }

    void CreateCell(int x, int z) {
        Vector3 position = new Vector3(
            (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f)
          , 0
          , z * (HexMetrics.outerRadius * 1.5f)
        );

        HexCell cell = cells_[x, z] = Instantiate<HexCell>(cellPrefab);
        // cell.transform.SetParent(transform, false);
        cell.transform.localPosition = position;
        cell.coords = HexCoords.FromOffsetCoords(x, z);
        cell.color = defaultColor;

        TMP_Text label = Instantiate<TMP_Text>(cellLabelPrefab);
        // label.rectTransform.SetParent(gridCanvas_.transform, false);
        label.rectTransform.anchoredPosition = new Vector2(position.x, position.z);
        if (HexCoordsLable) {
            label.text = cell.coords.ToStringOnSeparateLines();
        } else {
            label.text = cell.coords.ToOffsetCoords().ToString();
        }

        cell.uiRect = label.rectTransform;
        cell.Elevation = 0;

        AddCellToChunk(x, z, cell);
    }

    void CreateChunks() {
        if (chunks_ != null) {
            foreach (var chunk in chunks_) {
                Destroy(chunk.gameObject);
            }
        }

        chunks_ = new HexGridChunk[chunkCountX, chunkCountZ];

        for (int z = 0; z < chunkCountZ; ++z) {
			for (int x = 0; x < chunkCountX; ++x) {
				HexGridChunk chunk = chunks_[x, z] = Instantiate(chunkPrefab);
				chunk.transform.SetParent(transform);
			}
		}
    }

    void AddCellToChunk(int x, int z, HexCell cell) {
        int chunkX = x / HexMetrics.chunkSizeX;
        int chunkZ = z / HexMetrics.chunkSizeZ;
        var chunk = chunks_[chunkX, chunkZ];

        int localX = x - chunkX * HexMetrics.chunkSizeX;
        int localZ = z - chunkZ * HexMetrics.chunkSizeZ;
        chunk.AddCell(localX, localZ, cell);
    }

    public HexCell GetCell(Vector3 position) {
        position = transform.InverseTransformPoint(position);
        HexCoords coords = HexCoords.FromPosition(position);

        var arr_coords = coords.ToOffsetCoords();
        return cells_[arr_coords.x, arr_coords.y];
    }

    // TODO: del
    public void SetRandomElevation() {
        foreach (var cell in cells_) {
            cell.Elevation = UnityEngine.Random.Range(0, 3);
        }
    }

    // For Game
    [SerializeField] Color[] status_colors;

    void CreateRandomElevation() {
        foreach (var cell in cells_) {
            int scale = 1;
            cell.Elevation = UnityEngine.Random.Range(0, 4 * scale);

            Debug.Log(Color.green.ToString());
            if (cell.Elevation / scale == 0) {
                cell.color = new Color(80 / 255f, 213 / 255f, 85 / 255f);
            }
            if (cell.Elevation / scale == 1) {
                cell.color = new Color(248 / 255f, 235 / 255f, 70 / 255f);
            }
            if (cell.Elevation / scale == 2) {
                cell.color = new Color(254 / 255f, 168 / 255f, 121 / 255f);
            }
            if (cell.Elevation / scale == 3) {
                cell.color = new Color(234 / 255f, 235 / 255f, 234 / 255f);
            }
        }
    }

    public void SetCellStatus(int x, int y, int status) {
        if (status < 0)
            return;

        // Color color = Color.black;
        // if (status < status_colors.Length) {
        //     color = status_colors[status];
        // }

        // cells_[x, y].color = color;
        // Debug.Log($"Hex cell status update {status}");
        cells_[x, y].Status = status;
    }

    public HexCell GetCell(HexCoords coords) {
        var arr_coords = coords.ToOffsetCoords();
        if (arr_coords.x < 0 || arr_coords.y < 0 || arr_coords.x >= cellCountX_ || arr_coords.y >= cellCountZ_) {
            return null;
        }
        return cells_[arr_coords.x, arr_coords.y];
    }
}
