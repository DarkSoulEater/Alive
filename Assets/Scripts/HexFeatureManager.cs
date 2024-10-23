using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexFeatureManager : MonoBehaviour
{
    public Transform featurePrefab;
    public Transform[] featurePlayer1Prefabs;
    public Transform[] featurePlayer2Prefabs;
    Transform container_;
	public void Clear() {
        if (container_) {
            Destroy(container_.gameObject);
        }
        container_ = new GameObject("Features container").transform;
        container_.SetParent(transform, false);
    }

	public void Apply() {}

	public void AddFeature(Vector3 position) {
        Transform instance = Instantiate(featurePrefab);
        position.y += instance.localScale.y * 0.5f; // TODO: del
        instance.localPosition = HexMetrics.Perturb(position);
        instance.localRotation = Quaternion.Euler(0f, 360f * Random.value, 0f);
        instance.SetParent(container_.transform, false);
    }

    public void AddFeature(HexCell cell) {
        if (cell.Status <= 0)
            return;

        var position = cell.Position;
        var featurePlayerPrefabs = (cell.Status == 1 ? featurePlayer1Prefabs : featurePlayer2Prefabs);
        int ind = Random.Range(0, featurePlayerPrefabs.Length);
        Transform instance = Instantiate(featurePlayerPrefabs[ind]);
        // position.y += instance.localScale.y * 0.5f; // TODO: del
        instance.localPosition = HexMetrics.Perturb(position);
        // instance.localRotation = Quaternion.Euler(0f, 360f * Random.value, 0f);
        instance.SetParent(container_.transform, false);
    }
}
