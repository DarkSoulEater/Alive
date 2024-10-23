using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StartSettings : MonoBehaviour
{
    [SerializeField]
    Grid grid;

    [SerializeField]
    Field field;

    [SerializeField]
    TimeStream time_stream;

    // Input Size
    [SerializeField]
    TMP_InputField input_width;
    [SerializeField]
    TMP_InputField input_height;

    [SerializeField]
    Vector2Int input_size_ = new Vector2Int(10, 10);

    void Start() {
        // Set default size text
        input_width.SetTextWithoutNotify(input_size_.x.ToString());
        input_height.SetTextWithoutNotify(input_size_.y.ToString());
    }

    public void CloseOpen() {
        gameObject.SetActive(gameObject.activeInHierarchy ^ true);
    }

    // Size input callback
    public void OnSizeInputEndEdit() {
        var width = Math.Abs(Int32.Parse(input_width.text));
        var height = Math.Abs(Int32.Parse(input_height.text));

        if (width > 0) {
            input_size_.x = width;
        }
        input_width.SetTextWithoutNotify(input_size_.x.ToString());

        if (height > 0) {
            input_size_.y = height;
        }
        input_height.SetTextWithoutNotify(input_size_.y.ToString());
    }

    public void OnComplete() {
        field.Refresh(new Vector2Int(input_size_.x * HexMetrics.chunkSizeX
                                   , input_size_.y * HexMetrics.chunkSizeZ));

        field.Clear();
        
        time_stream.StartGame();
        transform.gameObject.SetActive(false);
    }
}
