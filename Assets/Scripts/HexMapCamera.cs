using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexMapCamera : MonoBehaviour
{
    Transform swivel_, stick_;

    float zoom_ = 1f;
    public float stickMinZoom = -250, stickMaxZoom = -45;
    public float swivelMinZoom = 90, swivelMaxZoom = 45;

    public float moveSpeedMinZoom = 400, moveSpeedMaxZoom = 100;

    public float rotationSpeed = 180;

    public HexGrid grid;

    void Awake() {
        swivel_ = transform.GetChild(0);
        stick_ = swivel_.GetChild(0);
        AdjustZoom(-0.5f);
    }

    void Update() {
        float zoomDelta = Input.GetAxis("Mouse ScrollWheel");
        if (zoomDelta != 0f) {
            AdjustZoom(zoomDelta);
        }

        float rotationDelta = Input.GetAxis("Rotation");
		if (rotationDelta != 0f) {
			AdjustRotation(rotationDelta);
		}

        float xDelta = Input.GetAxis("Horizontal");
		float zDelta = Input.GetAxis("Vertical");
		if (xDelta != 0f || zDelta != 0f) {
			AdjustPosition(xDelta, zDelta);
		}
    }

    void AdjustZoom(float delta) {
        zoom_ = Mathf.Clamp01(zoom_ + delta);

        float distance = Mathf.Lerp(stickMinZoom, stickMaxZoom, zoom_);
        stick_.localPosition = new Vector3(0f, 0f, distance);

        float angle = Mathf.Lerp(swivelMinZoom, swivelMaxZoom, zoom_);
		swivel_.localRotation = Quaternion.Euler(angle, 0f, 0f);
    }

	void AdjustPosition (float xDelta, float zDelta) {
        Vector3 direction =
			transform.localRotation *
			new Vector3(xDelta, 0f, zDelta).normalized;
        float damping = Mathf.Max(Mathf.Abs(xDelta), Mathf.Abs(zDelta));
        float distance =
			Mathf.Lerp(moveSpeedMinZoom, moveSpeedMaxZoom, zoom_) *
			damping * Time.deltaTime;

		Vector3 position = transform.localPosition;
		position += direction * distance;
		transform.localPosition = ClampPosition(position);
	}

    Vector3 ClampPosition (Vector3 position) {
		float xMax =
			(grid.chunkCountX * HexMetrics.chunkSizeX - 0.5f) *
			(2f * HexMetrics.innerRadius);
		position.x = Mathf.Clamp(position.x, 0f, xMax);

		float zMax =
			(grid.chunkCountZ * HexMetrics.chunkSizeZ - 1) *
			(1.5f * HexMetrics.outerRadius);
		position.z = Mathf.Clamp(position.z, 0f, zMax);

		return position;
	}

    float rotationAngle;
    void AdjustRotation (float delta) {
		rotationAngle += delta * rotationSpeed * Time.deltaTime;
		if (rotationAngle < 0f) {
			rotationAngle += 360f;
		}
		else if (rotationAngle >= 360f) {
			rotationAngle -= 360f;
		}
		transform.localRotation = Quaternion.Euler(0f, rotationAngle, 0f);
	}
}
