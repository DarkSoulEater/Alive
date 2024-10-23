using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HexMapEditor : MonoBehaviour {
    [SerializeField] Field field;

    [SerializeField] int status;

    void Update () {
		if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()) {
			var cell = RaycastHexGrid();

            var coords = cell.coords.ToOffsetCoords();
            field.SetStatus(coords.x, coords.y, status);

            // for (int k = 0; k < 6; ++k) { // Draw all neighbor
            //     var n_cell = cell.GetNeighbor((HexDirection)k);
            //     if (n_cell == null) {
            //         Debug.Log($"{(HexDirection)k} = null");
            //         continue;
            //     }
            //     var n_coords = n_cell.coords.ToOffsetCoords();
            //     field.SetStatus(n_coords.x, n_coords.y, 1);
            // }
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
