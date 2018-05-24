using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderWithShader : MonoBehaviour {

    Camera cam;
    public Shader shader;

	// Use this for initialization
	void Start () {
        cam = this.GetComponent<Camera>();
        cam.SetReplacementShader(shader, "RenderType");
	}

}
