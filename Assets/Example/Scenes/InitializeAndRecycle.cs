using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitializeAndRecycle : MonoBehaviour {

    private const int count = 10;
    private GameObject[] gos = new GameObject[count];

	// Use this for initialization
	void Start () {
        Warehouser.Start();
	}
	
	// Update is called once per frame
	void Update () {

	}

    public void OnGUI()
    {
        if (GUI.Button(new Rect(0, 0, 100, 50), "Instance"))
        {
            for (int i = 0, j = count; i < j; i++)
            {
                gos[i] = Warehouser.Get("Sphere");
            }
        }
        if (GUI.Button(new Rect(0, 50, 100, 50), "Recycle"))
        {
            for (int i = 0; i < count; i++)
            {
                Warehouser.Recycle(gos[i]);
            }
        }
    }


}
