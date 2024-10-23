using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Pattern {
    Vector2Int[] pattern_ = null;

    string name_;

    Pattern(Vector2Int[] pattern, string name = "") {
        pattern_ = pattern;
        name_ = name;
    }

    public Vector2Int this[int k] {
        get => pattern_[k];
    }

    public int Length {
        get => pattern_.Length;
    }

    public override string ToString() {
        return name_;
    }

    public static Pattern Square() {
        var pattern = new Vector2Int[4];
        pattern[0] = new Vector2Int(0, 0);
        pattern[1] = new Vector2Int(0, 1);
        pattern[2] = new Vector2Int(1, 0);
        pattern[3] = new Vector2Int(1, 1);
        return new Pattern(pattern, "Square");
    }

    public static Pattern Line3V() {
        var pattern = new Vector2Int[3];
        pattern[0] = new Vector2Int(0, 0);
        pattern[1] = new Vector2Int(0, 1);
        pattern[2] = new Vector2Int(0, -1);
        return new Pattern(pattern, "Blinker");
    }

    public static Pattern Line3H() {
        var pattern = new Vector2Int[3];
        pattern[0] = new Vector2Int(0, 0);
        pattern[1] = new Vector2Int(1, 0);
        pattern[2] = new Vector2Int(-1, 0);
        return new Pattern(pattern, "Blinker");
    }

    public static Pattern BeeHive() {
        var pattern = new Vector2Int[6];
        pattern[0] = new Vector2Int(-1, 0);
        pattern[1] = new Vector2Int(0, 1);
        pattern[2] = new Vector2Int(0, -1);
        pattern[3] = new Vector2Int(1, 1);
        pattern[4] = new Vector2Int(1, -1);
        pattern[5] = new Vector2Int(2, 0);
        return new Pattern(pattern, "Bee-hive");
    }

    public static Pattern Loaf() {
        var pattern = new Vector2Int[5];
        pattern[0] = new Vector2Int(-1, 0);
        pattern[1] = new Vector2Int(-1, 1);
        pattern[2] = new Vector2Int(0, 1);
        pattern[3] = new Vector2Int(0, -1);
        pattern[4] = new Vector2Int(1, 0);
        return new Pattern(pattern, "Loaf");
    }

    public static Pattern Tub() {
        var pattern = new Vector2Int[4];
        pattern[0] = new Vector2Int(-1, 0);
        pattern[1] = new Vector2Int(0, 1);
        pattern[2] = new Vector2Int(0, -1);
        pattern[3] = new Vector2Int(1, 0);
        return new Pattern(pattern, "Tub");
    }

    public static Pattern Toad() {
        var pattern = new Vector2Int[6];
        pattern[0] = new Vector2Int(-1, 0);
        pattern[1] = new Vector2Int(0, 0);
        pattern[2] = new Vector2Int(0, 1);
        pattern[3] = new Vector2Int(1, 0);
        pattern[4] = new Vector2Int(1, 1);
        pattern[5] = new Vector2Int(2, 1);
        return new Pattern(pattern, "Toad");
    }

    public static Pattern Beacon() {
        var pattern = new Vector2Int[6];
        pattern[0] = new Vector2Int(-1, 0);
        pattern[1] = new Vector2Int(-1, 1);
        pattern[2] = new Vector2Int(0, 1);
        pattern[3] = new Vector2Int(1, -2);
        pattern[4] = new Vector2Int(2, -1);
        pattern[5] = new Vector2Int(2, -2);
        return new Pattern(pattern, "Beacon");
    }

    public static Pattern Glider() {
        var pattern = new Vector2Int[5];
        pattern[0] = new Vector2Int(0, 0);
        pattern[1] = new Vector2Int(-1, 1);
        pattern[2] = new Vector2Int(0, -1);
        pattern[3] = new Vector2Int(1, 0);
        pattern[4] = new Vector2Int(1, 1);
        return new Pattern(pattern, "Glider");
    }

    public static Pattern LWSS() {
        var pattern = new Vector2Int[9];
        pattern[0] = new Vector2Int(-2, 0);
        pattern[1] = new Vector2Int(-2, -2);
        pattern[2] = new Vector2Int(-1, 1);
        pattern[3] = new Vector2Int(0, 1);
        pattern[4] = new Vector2Int(1, 1);
        pattern[5] = new Vector2Int(1, -2);
        pattern[6] = new Vector2Int(2, 1);
        pattern[7] = new Vector2Int(2, 0);
        pattern[8] = new Vector2Int(2, -1);
        return new Pattern(pattern, "LWSS");
    }

    public static Pattern MWSS() {
        var pattern = new Vector2Int[15];
        pattern[0] = new Vector2Int(0, 0);
        pattern[1] = new Vector2Int(-2, 0);
        pattern[2] = new Vector2Int(-2, -1);
        pattern[3] = new Vector2Int(-1, 0);
        pattern[4] = new Vector2Int(-1, -1);
        pattern[5] = new Vector2Int(-1, -2);
        pattern[6] = new Vector2Int(0, -1);
        pattern[7] = new Vector2Int(0, -2);
        pattern[8] = new Vector2Int(1, 1);
        pattern[9] = new Vector2Int(1, -1);
        pattern[10] = new Vector2Int(1, -2);
        pattern[11] = new Vector2Int(2, 1);
        pattern[12] = new Vector2Int(2, 0);
        pattern[13] = new Vector2Int(2, -1);
        pattern[14] = new Vector2Int(3, 0);
        return new Pattern(pattern, "MWSS");
    }

    public static Pattern HWSS() {
        var pattern = new Vector2Int[18];
        pattern[0] = new Vector2Int(-3, 0);
        pattern[1] = new Vector2Int(-3, -1);
        pattern[2] = new Vector2Int(-2, 1);
        pattern[3] = new Vector2Int(-2, 0);
        pattern[4] = new Vector2Int(-2, -1);
        pattern[5] = new Vector2Int(-1, 1);
        pattern[6] = new Vector2Int(-1, 0);
        pattern[7] = new Vector2Int(-1, -1);
        pattern[8] = new Vector2Int(0, 1);
        pattern[9] = new Vector2Int(0, -1);
        pattern[10] = new Vector2Int(1, 1);
        pattern[11] = new Vector2Int(1, 0);
        pattern[12] = new Vector2Int(1, -2);
        pattern[13] = new Vector2Int(2, 0);
        pattern[14] = new Vector2Int(2, -1);
        pattern[15] = new Vector2Int(2, -2);
        pattern[16] = new Vector2Int(3, -1);
        pattern[17] = new Vector2Int(0, 0);
        return new Pattern(pattern, "HWSS");
    }

    public static Pattern Pulsar() {
        var pattern = new Vector2Int[48];
        pattern[0] = new Vector2Int(-2, 1);
        pattern[1] = new Vector2Int(-3, 1);
        pattern[2] = new Vector2Int(-4, 1);
        pattern[3] = new Vector2Int(-1, 2);
        pattern[4] = new Vector2Int(-1, 3);
        pattern[5] = new Vector2Int(-1, 4);
        pattern[6] = new Vector2Int(-2, 6);
        pattern[7] = new Vector2Int(-3, 6);
        pattern[8] = new Vector2Int(-4, 6);
        pattern[9] = new Vector2Int(-6, 2);
        pattern[10] = new Vector2Int(-6, 3);
        pattern[11] = new Vector2Int(-6, 4);

        pattern[12] = new Vector2Int(2, 1);
        pattern[13] = new Vector2Int(3, 1);
        pattern[14] = new Vector2Int(4, 1);
        pattern[15] = new Vector2Int(1, 2);
        pattern[16] = new Vector2Int(1, 3);
        pattern[17] = new Vector2Int(1, 4);
        pattern[18] = new Vector2Int(2, 6);
        pattern[19] = new Vector2Int(3, 6);
        pattern[20] = new Vector2Int(4, 6);
        pattern[21] = new Vector2Int(6, 2);
        pattern[22] = new Vector2Int(6, 3);
        pattern[23] = new Vector2Int(6, 4);

        pattern[24] = new Vector2Int(-2, -1);
        pattern[25] = new Vector2Int(-3, -1);
        pattern[26] = new Vector2Int(-4, -1);
        pattern[27] = new Vector2Int(-1, -2);
        pattern[28] = new Vector2Int(-1, -3);
        pattern[29] = new Vector2Int(-1, -4);
        pattern[30] = new Vector2Int(-2, -6);
        pattern[31] = new Vector2Int(-3, -6);
        pattern[32] = new Vector2Int(-4, -6);
        pattern[33] = new Vector2Int(-6, -2);
        pattern[34] = new Vector2Int(-6, -3);
        pattern[35] = new Vector2Int(-6, -4);

        pattern[36] = new Vector2Int(2, -1);
        pattern[37] = new Vector2Int(3, -1);
        pattern[38] = new Vector2Int(4, -1);
        pattern[39] = new Vector2Int(1, -2);
        pattern[40] = new Vector2Int(1, -3);
        pattern[41] = new Vector2Int(1, -4);
        pattern[42] = new Vector2Int(2, -6);
        pattern[43] = new Vector2Int(3, -6);
        pattern[44] = new Vector2Int(4, -6);
        pattern[45] = new Vector2Int(6, -2);
        pattern[46] = new Vector2Int(6, -3);
        pattern[47] = new Vector2Int(6, -4);
        return new Pattern(pattern, "Pulsar");
    }

    public static Pattern PentaDecathlon() {
        var pattern = new Vector2Int[18];
        pattern[0] = new Vector2Int(0, 4);
        pattern[1] = new Vector2Int(-1, 4);
        pattern[2] = new Vector2Int(-2, 3);
        pattern[3] = new Vector2Int(-3, 2);
        pattern[4] = new Vector2Int(-4, 0);
        pattern[5] = new Vector2Int(-4, -1);
        pattern[6] = new Vector2Int(-3, -3);
        pattern[7] = new Vector2Int(-2, -4);
        pattern[8] = new Vector2Int(-1, -5);
        pattern[9] = new Vector2Int(0, -5);

        pattern[10] = new Vector2Int(1, -5);
        pattern[11] = new Vector2Int(2, -4);
        pattern[12] = new Vector2Int(3, -3);
        pattern[13] = new Vector2Int(4, -1);
        pattern[14] = new Vector2Int(4, 0);
        pattern[15] = new Vector2Int(3, 2);
        pattern[16] = new Vector2Int(2, 3);
        pattern[17] = new Vector2Int(1, 4);
        return new Pattern(pattern, "Penta-Decathlon");
    }

    // public static Pattern () {
    //     var pattern = new Vector2Int[]
    //     return new Pattern(pattern);
    // }
}

public class Preview {
    Pattern pattern_;
    int status_;
    public Pattern pattern {
        get { return pattern_; }
    }

    HexCell base_cell_;
    int[] save_status_;
    HexCell[] save_cell_;
    bool correct_state_;

    public Preview(Pattern pattern, int status) {
        status_ = status;
        pattern_ = pattern;
        save_cell_ = new HexCell[pattern.Length];
        save_status_ = new int[pattern_.Length];
    }

    public void Update(HexCell cell) {
        if (cell == null) {
            correct_state_ = false;
            return;
        }
        
        if (cell == base_cell_)
            return;

        correct_state_ = true;
        base_cell_ = cell;
        var base_coords = cell.coords.ToOffsetCoords();

        for (int k = 0; k < save_cell_.Length; ++k) {
            if (save_cell_[k] != null) {
                save_cell_[k].Status = save_status_[k];
                save_cell_[k] = null;
            }
        }

        var hex_grid = cell.GetHexGrid();
        for (int k = 0; k < pattern_.Length; ++k) {
            var pos = pattern_[k];
            var n_pos = base_coords + pos;

            var neighbor = hex_grid.GetCell(HexCoords.FromOffsetCoords(n_pos.x, n_pos.y));
            if (neighbor == null) {
                correct_state_ = false;
                continue;
            }

            if (neighbor.Status != 0) {
                correct_state_ = false;
                continue;
            }

            save_cell_[k] = neighbor;
            save_status_[k] = neighbor.Status;
            neighbor.Status = status_;
        }
    }

    public HexCell[] Apply() {
        if (correct_state_ == false) {
            for (int k = 0; k < save_cell_.Length; ++k) {
                if (save_cell_[k] != null) {
                    save_cell_[k].Status = save_status_[k];
                    save_cell_[k] = null;
                }
            }
            return null;
        }

        return save_cell_;
    }
}
