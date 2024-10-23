using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class CameraDrawWireframe : MonoBehaviour {
    [SerializeField] bool isActive;
    void OnPreRender() {
        if (isActive) { 
            GL.wireframe = true;
        }
    }

    void OnPostRender() {
        if (isActive) { 
            GL.wireframe = false;
        }
    }
}
