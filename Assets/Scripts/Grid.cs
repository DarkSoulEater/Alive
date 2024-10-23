using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
// using Unity.VisualScripting;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class Grid : MonoBehaviour
{
    [SerializeField]
    Cell DeadCell, AliveCell, P1ActiveCell, P2ActiveCell;
    [SerializeField] GameObject[] objects_;
    [SerializeField] GameObject[] P1Objects;
    [SerializeField] GameObject[] P2Objects;

    [SerializeField] Field field_;

    Cell CreateCell(Vector2Int position, int status, bool draw_visual_effects = true) {
        float scale = 2.2f; // TODO:

        Cell cell = null;

        if (status == 0) {
            cell = Instantiate<Cell>(DeadCell, new Vector3(position.x * scale, 0, position.y * scale), Quaternion.identity);
        } else if (status == 2) {
            cell = Instantiate<Cell>(P1ActiveCell, new Vector3(position.x * scale, 0, position.y * scale), Quaternion.identity);
        } else if (status == 3) {
            cell = Instantiate<Cell>(P2ActiveCell, new Vector3(position.x * scale, 0, position.y * scale), Quaternion.identity);
        } else if (status > 0) {
            cell = Instantiate<Cell>(AliveCell, new Vector3(position.x * scale, 0, position.y * scale), Quaternion.identity);
        }

        cell.transform.SetParent(transform);
        cell.position = position;
        // cell.status = status;

        if (draw_visual_effects) {
            if (status == 2) {
                if (Random.Range(0, 10) >= 6) {
                    GameObject obj = Instantiate(P1Objects[Random.Range(0, P1Objects.Length - 1)], cell.transform);
                }
            } else if (status == 3) {
                if (Random.Range(0, 10) >= 6) {
                    GameObject obj = Instantiate(P2Objects[Random.Range(0, P2Objects.Length - 1)], cell.transform);
                }
            } else if (status > 0) {
                if (Random.Range(0, 10) >= 6) {
                    GameObject obj = Instantiate(objects_[Random.Range(0, objects_.Length - 1)], cell.transform);
                }
            }
        }

        return cell;
    }

    Cell[,] cells_ = null;
    public void Create(int size_x, int size_y) {
        if (cells_ != null) {
            foreach (Cell cell in cells_) {
                Destroy(cell.transform.gameObject);
            }
        }

        cells_ = new Cell[size_x, size_y];

        for (int y = 0; y < size_y; ++y) {
            for (int x = 0; x < size_x; ++x) {
                cells_[x, y] = CreateCell(new Vector2Int(x, y), 0);
            }
        }
    }

    public void SetCellStatus(int x, int y, int status) {
        if (status > 0) {
            Destroy(cells_[x, y].transform.gameObject);
            cells_[x, y] = CreateCell(new Vector2Int(x, y), status);
        } else {
            Destroy(cells_[x, y].transform.gameObject);
            cells_[x, y] = CreateCell(new Vector2Int(x, y), 0);
        }
    }

    [SerializeField]
    TimeStream stream;

    public int Status = 1;
    void OnClickCell(Cell cell) {
        if (stream.IsStoped()) {
            if (field_) {
                field_.SetStatus(cell.position.x, cell.position.y, Status);
            } else {
                Debug.LogAssertion("Field not set", this);
            }
        } else {}
    }

    Cell Raycast() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000)) {
            Debug.DrawLine(ray.origin, hit.point, Color.blue, 10);

            var cell = hit.transform.GetComponent<Cell>();
            if (!cell) {
                Debug.LogError($"Unexpected ray collision with {hit.transform.name}", this);
            }
            return cell;
        }

        return null;
    }

    [SerializeField] // TODO: DEL
    public bool DrawPreview = false;
    Preview preview_ = null;

    public void Draw() {
        DrawPreview = true;
    }

    void Update() {
        if (DrawPreview) {
            if (preview_ == null) {
                preview_ = new Preview(this);
            }

            var cell = Raycast();
            if (cell) {
                preview_.MoveOn(cell);
            }
        }

        if (Input.GetMouseButtonDown((int)MouseButton.LeftMouse)) {
            var cell = Raycast();
            if (cell) {
                OnClickCell(cell);
            }
        }
    }

    public void UpdateMesh() { // TODO: DEL
        return;
    }


    // Preview
    class Preview {
        Grid grid_; // Parent grid

        Cell[,] cells_ = null;
        Cell[] deactive_cells_ = null;
        GameObject preview_parent_ = null;
        Cell[] preview_cells_ = null;

        Pattern pattern_;

        public Preview(Grid grid) {
            grid_ = grid;
            cells_ = grid_.cells_;
            pattern_ = Pattern.Square();

            var lenght = pattern_.Length;
            deactive_cells_ = new Cell[lenght];
            preview_cells_ = new Cell[lenght];

            preview_parent_ = new GameObject("PreviewParent");
            preview_parent_.transform.SetParent(grid_.transform);

            for (int k = 0; k < lenght; ++k) {
                deactive_cells_[k] = null;

                preview_cells_[k] = grid_.CreateCell(new Vector2Int(pattern_[k].x, pattern_[k].y), 1, false);
                preview_cells_[k].transform.SetParent(preview_parent_.transform);
            }
        }

        void SwapToPreview(int x, int y, int k) {
            if (x < 0 || y < 0 || x >= cells_.GetLength(0) || y >= cells_.GetLength(1)) {
                Debug.Log("Preview bound error"); // TODO: impl red box
                return;
            }
            deactive_cells_[k] = cells_[x, y];
            deactive_cells_[k].gameObject.SetActive(false);
        }

        public void MoveOn(Cell cell) {
            int x = cell.position.x;
            int y = cell.position.y;

            var lenght = pattern_.Length;

            for (int k = 0; k < lenght; ++k) {
                if (deactive_cells_[k] != null) 
                    deactive_cells_[k].gameObject.SetActive(true);
                deactive_cells_[k] = null;
            }

            for (int k = 0; k < lenght; ++k) {
                var coords = pattern_[k];

                SwapToPreview(x + coords.x, y + coords.y, k);
            }

            preview_parent_.transform.SetLocalPositionAndRotation(new Vector3(x * 2.2f, 0, y * 2.2f), Quaternion.identity); // TODO: Del magic const from gris scale
        }
    }
}

