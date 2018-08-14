using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPushAboveTerrain : MonoBehaviour {
    Terrain terr;
    bool isPushing;
	// Use this for initialization
	void Start () {
        isPushing = false;
        terr = Terrain.activeTerrain;
	}
	
	// Update is called once per frame
	void Update () {
        if(terr == null)
        {
            terr = Terrain.activeTerrain;
        }
        if (terr != null)
        {
            if (transform.position.y < terr.SampleHeight(transform.position))
            {
                if(!isPushing)
                    StartCoroutine(PushAboveTerrain(terr));
            }
        }
	}

    private IEnumerator PushAboveTerrain(Terrain terr)
    {
        isPushing = true;
        yield return new WaitForSeconds(1);
        while (transform.position.y < terr.SampleHeight(transform.position))
        {
            transform.position += Vector3.up;
            yield return null;
        }
        isPushing = false;
    }

}
