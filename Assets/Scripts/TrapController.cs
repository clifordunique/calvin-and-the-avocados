using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapController : MonoBehaviour {

    public bool isRotating;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        if (isRotating)
        {
            RotateTrap();
        }

    }

    void RotateTrap()
    {
        transform.Rotate(Vector3.forward * -20);
    }
}
