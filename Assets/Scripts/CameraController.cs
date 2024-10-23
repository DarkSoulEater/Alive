using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    float speed;
    void Update() {
        Vector2 velocity = new Vector2(0, 0);
        if (Input.GetKey(KeyCode.A)) {
            velocity.x -= 1;
        }
        if (Input.GetKey(KeyCode.D)) {
            velocity.x += 1;
        }
        if (Input.GetKey(KeyCode.W)) {
            velocity.y -= 1;
        }
        if (Input.GetKey(KeyCode.S)) {
            velocity.y += 1;
        }

        transform.SetLocalPositionAndRotation(transform.position 
                + Vector3.forward * velocity.y * speed * Time.deltaTime
                + Vector3.left * velocity.x * speed * Time.deltaTime
            , transform.rotation);
    }
}
