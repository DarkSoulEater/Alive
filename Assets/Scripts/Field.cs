using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Unity.VisualScripting;
using UnityEngine;

public class Field : MonoBehaviour
{
    Vector2Int size_ = Vector2Int.zero;

    int[,] field_;

    [SerializeField] 
    HexGrid grid;

    SortedSet<Vector2Int> evolution_candidate_;

    int[] statistics_;
    public int aliveValue = 1;
    public int spawnValue = 2;

    void Awake() {
        evolution_candidate_ = new SortedSet<Vector2Int>(new Vector2IntComp());
        statistics_ = new int[3];
    }

    public void ResetStatistics() {
        for (int k = 0; k < statistics_.Length; ++k) {
            statistics_[k] = 0;
        }
    }

    public int GetStatistics(int player) {
        return statistics_[player];
    }

    public void Refresh(Vector2Int size) {
        if (size.x <= 0 || size.y <= 0) {
            Debug.LogAssertion($"Uncorrect field size: {size_.x}, {size_.y}", this);
        }

        if (size_.x != size.x || size_.y != size.y) {
            field_ = new int[size.x, size.y];
            grid.Create(size.x / HexMetrics.chunkSizeX, size.y / HexMetrics.chunkSizeZ);
        }

        size_ = size;
        evolution_candidate_.Clear();
        ResetStatistics();
    }

    public void RandomSpawn() {
        Refresh(size_);

        for (int y = 1; y < size_.y - 1; ++y) {
            for (int x = 1; x < size_.x - 1; ++x) {
                var val = Random.Range(0, 10);
                if (val > 7) {
                    field_[x, y] = Random.Range(1, 3);
                    AddEvolutionCadidate(x, y, evolution_candidate_);
                } else {
                    field_[x, y] = 0;
                }
                UpdateGrid(x, y, field_[x, y]);
            }
        }
    }

    bool IsWall(int x, int y) {
        return field_[x, y] < 0;
    }

    bool IsAlive(int x, int y) {
        return field_[x, y] > 0;
    }

    void AddEvolutionCadidate(int x, int y, SortedSet<Vector2Int> set) {
        for (int dy = -1; dy <= 1; ++dy) {
            for (int dx = -1; dx <= 1; ++dx) {
                if (x + dx < 0 || y + dy < 0 || x + dx >= size_.x || y + dy >= size_.y) 
                    continue;
                set.Add(new Vector2Int(x + dx, y + dy));
            }
        }
    }

    int СheckNeighbors(int x, int y) {
        if (IsWall(x, y)) {
            return field_[x, y];
        }

        int alive_cnt = 0;
        Dictionary<int, int> alive_cnt_2 = new Dictionary<int, int>();
        for (int dy = -1; dy <= 1; ++dy) {
            for (int dx = -1; dx <= 1; ++dx) {
                if (dx == 0 && dy == 0) 
                    continue;
                if (x + dx < 0 || y + dy < 0 || x + dx >= size_.x || y + dy >= size_.y) 
                    continue;

                if (IsAlive(x + dx, y + dy)) {
                    alive_cnt += 1;

                    alive_cnt_2[field_[x + dx, y + dy]] = alive_cnt_2.GetValueOrDefault(field_[x + dx, y + dy]) + 1;
                }
            }
        }

        if (IsAlive(x, y)) {
            if (alive_cnt == 2 || alive_cnt == 3) {
                statistics_[field_[x, y]] += aliveValue;
                return field_[x, y];
            }
        } else {
            if (alive_cnt == 3) {
                int status = 0, cnt = 0;
                foreach (var cell in alive_cnt_2) {
                    if (cell.Value > cnt) {
                        status = cell.Key;
                        cnt = cell.Value;
                    }
                }
                // Debug.Log($"{status}");
                statistics_[status] += spawnValue;
                return status;
            }
        }

        return 0;
    }

    public void UpdateLive() {
        int[,] field = new int[size_.x, size_.y];
        SortedSet<Vector2Int> new_evol_cand = new SortedSet<Vector2Int>(new Vector2IntComp());

        foreach (Vector2Int candidate in evolution_candidate_) {
            int x = candidate.x;
            int y = candidate.y;

            field[x, y] = СheckNeighbors(x, y);

            if (field[x, y] > 0) {
                AddEvolutionCadidate(x, y, new_evol_cand);
            }

            UpdateGrid(x, y, field[x, y]);
        }

        field_ = field;
        evolution_candidate_ = new_evol_cand;
    }

    void UpdateGrid(int x, int y, int status) {
        if (grid) {
            grid.SetCellStatus(x, y, status);
        }
    }

    public void SetStatus(int x, int y, int status) {
        if (IsAlive(x, y) && field_[x, y] != status) {
            return;
        } else if (IsAlive(x, y)) {
            status = 0;
        }

        // Debug.Log($"Set cell({x}, {y}) status = {status}");

        field_[x, y] = status;
        AddEvolutionCadidate(x, y, evolution_candidate_);
        UpdateGrid(x, y, status);
    }

    public void Clear() {
        for (int y = 0; y < size_.y; ++y) {
            for (int x = 0; x < size_.x; ++x) {
                if (IsAlive(x, y)) {
                    field_[x, y] = 0;
                }
                UpdateGrid(x, y, field_[x, y]);
            }
        }
        evolution_candidate_.Clear();
        ResetStatistics();
    }
}

public class Vector2IntComp : IComparer<Vector2Int> {
    public int Compare(Vector2Int lhs, Vector2Int rhs) {
        if (lhs == rhs)
            return 0;
        
        if (lhs.x == rhs.x)
            return lhs.y - rhs.y;
        
        return lhs.x - rhs.x;
    }
}