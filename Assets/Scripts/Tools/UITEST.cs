using UnityEngine;
using System.Collections;

public class UITEST : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void LoadTestLevel()
    {
        Application.LoadLevel("Test");
    }

    public void LoadTest2Level()
    {
        Application.LoadLevel("Shop");
    }
}
