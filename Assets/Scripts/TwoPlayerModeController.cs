using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TwoPlayerModeController : MonoBehaviour
{
    [SerializeField] Field field;

    Slider switch_player_;
    TMP_Text player_text_;
    int active_player_ = 1;
    public int activePlayer {
        get { return active_player_; }
    }

    CreatePatternButton pattern_creator_;

    TimeStream timeStream_;

    void Awake() {
        switch_player_ = GetComponentInChildren<Slider>();
        if (switch_player_ == null) {
            Debug.LogError("Player slider not found", this);
        } else {
            player_text_ = switch_player_.GetComponentInChildren<TMP_Text>();
            if (player_text_ == null) {
                Debug.LogError("Player text not found", this);
            }
        }

        pattern_creator_ = GetComponentInChildren<CreatePatternButton>();
        if (pattern_creator_ == null) {
            Debug.LogError("Pattern creator not found", this);
        }

        timeStream_ = GetComponentInParent<TimeStream>();
    }

    void OnEnable() {
        UpdateActivePlayer();
    }

    public void UpdateActivePlayer() {
        active_player_ = (int)switch_player_.value;
        player_text_.text = "Plyer: " + ((int)active_player_).ToString();
        Debug.Log($"Set active player {active_player_}");
    }

    public void StartDrawPreview() {
        Debug.Log("Start draw");
        preview_ = new Preview(pattern_creator_.pattern, active_player_);
        preview_drawing_ = true;
    }

    bool DrawPreview() {
        if (!preview_drawing_)
            return false;

        if (Input.GetMouseButtonUp(0) && !EventSystem.current.IsPointerOverGameObject()) {
            var cells = preview_.Apply();
            if (cells != null) {
                foreach (var cell in cells) {
                    var coords = cell.coords.ToOffsetCoords();
                    field.SetStatus(coords.x, coords.y, active_player_);
                }
            }

            preview_ = null;
            preview_drawing_ = false;
            return true;
        }

        preview_.Update(RaycastHexGrid());
        
        return true;
    }


    Preview preview_;
    bool preview_drawing_;
    void Update () {
        if (DrawPreview()) {
            return;
        }

		if (Input.GetMouseButtonDown(0) 
        && !EventSystem.current.IsPointerOverGameObject()
        &&  timeStream_.IsStoped()) {
			var cell = RaycastHexGrid();
            if (cell == null)
                return;

            var coords = cell.coords.ToOffsetCoords();
            field.SetStatus(coords.x, coords.y, active_player_);
		}
	}

    HexCell RaycastHexGrid() {
		Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast(inputRay, out hit)) {
			var hexGrid = hit.collider.gameObject.GetComponentInParent<HexGrid>();
            if (!hexGrid) {
                Debug.LogError($"Unexpected ray collision with {hit.transform.name}", this);
                return null;
            }

            return hexGrid.GetCell(hit.point);
		}
        return null;
    }
}
