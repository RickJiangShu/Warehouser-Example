using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitializeAndRecycle : MonoBehaviour {

	// Use this for initialization
	void Start () {
        ResourceManager.Start();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnGUI()
    {
        if (GUI.Button(new Rect(0, 0, 100, 50), "Instance"))
        {
            for (int i = 0, j = 10; i < j; i++)
            {
                ResourceManager.GetInstance("Sphere");
            }
        }
    }


}
