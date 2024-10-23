using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CreatePatternButton : MonoBehaviour, IPointerDownHandler
{
    public int cellCountX = 15;
    public int cellCountY = 15;

    public Image whiteCell;
    Image[,] images_;

    Slider type_slider_;
    TMP_Text name_text_;

    float scale_ = 0.9f;

    void Awake() {
        var rect = GetComponent<RectTransform>();
        if (rect == null) {
            Debug.LogError("Unccorect object for this component", this);
        }

        type_slider_ = GetComponentInChildren<Slider>();
        if (type_slider_ == null) {
            Debug.LogError("Type slider not found", this);
        }
        type_slider_.minValue = 0;
        type_slider_.maxValue = maxType_;

        name_text_ = GetComponentInChildren<TMP_Text>();
        if (name_text_ == null) {
            Debug.LogError("Text for name not found", this);
        }

        controller_ = GetComponentInParent<TwoPlayerModeController>();
        if (controller_ == null) {
            Debug.LogError("2 Player node controller not found", this);
        }

        images_ = new Image[cellCountX, cellCountY];

        float width = rect.rect.width;
        float height = rect.rect.height;

        float cellWidth = width / cellCountX;
        float cellHeight = height / cellCountY;

        for (int y = 0; y < cellCountY; ++y) {
            for (int x = 0; x < cellCountX; ++x) {
                Image cell = images_[x, y] = Instantiate<Image>(whiteCell);
                var imgWidth = cell.rectTransform.rect.width;
                var imgHeight = cell.rectTransform.rect.height;
                
                cell.rectTransform.localPosition = new Vector2((x + 0.5f) * cellWidth - width / 2, (y + 0.5f) * cellHeight - height / 2);
                cell.rectTransform.localScale = new Vector2(cellWidth / imgWidth * scale_, cellHeight / imgHeight * scale_);
                cell.rectTransform.SetParent(rect, false);
            }
        }

        SetPattern(0);
    }

    int type_ = int.MinValue;
    int maxType_ = 13;
    Pattern pattern_;
    public Pattern pattern {
        get { return pattern_; }
    }

    public void UpdateType() {
        SetPattern((int)type_slider_.value);
    }

    void SetPattern(Pattern pattern) {
        if (pattern_ != null) {
            SetColor(Color.white);
        }

        pattern_ = pattern;
        name_text_.text = pattern_.ToString();

        SetColor(Color.black);
    }

    void SetPattern(int type) {
        type_ = type;
        switch (type) {
            case 0: {
                SetPattern(Pattern.Square());
                break;
            }

            case 1: {
                SetPattern(Pattern.Line3V());
                break;
            }

            case 2: {
                SetPattern(Pattern.Line3H());
                break;
            }

            case 3: {
                SetPattern(Pattern.BeeHive());
                break;
            }

            case 4: {
                SetPattern(Pattern.Loaf());
                break;
            }

            case 5: {
                SetPattern(Pattern.Tub());
                break;
            }

            case 6: {
                SetPattern(Pattern.Toad());
                break;
            }

            case 7: {
                SetPattern(Pattern.Beacon());
                break;
            }

            case 8: {
                SetPattern(Pattern.Glider());
                break;
            }

            case 9: {
                SetPattern(Pattern.LWSS());
                break;
            }

            case 10: {
                SetPattern(Pattern.MWSS());
                break;
            }

            case 11: {
                SetPattern(Pattern.HWSS());
                break;
            }

            case 12: {
                SetPattern(Pattern.Pulsar());
                break;
            }

            case 13: {
                SetPattern(Pattern.PentaDecathlon());
                break;
            }
        }
    }

    void SetColor(Color color) {
        for (int k = 0; k < pattern_.Length; ++k) {
            var coords = pattern_[k] + new Vector2Int(cellCountX / 2 - 1, cellCountY / 2 - 1);
            images_[coords.x, coords.y].color = color;
        }
    }

    TwoPlayerModeController controller_;
    public void OnPointerDown(PointerEventData eventData) {
        controller_.StartDrawPreview();
    }
}
