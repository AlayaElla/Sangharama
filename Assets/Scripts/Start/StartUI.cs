using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartUI : MonoBehaviour {

    public Text resourcesPath;

	// Use this for initialization
	void Start () {
        resourcesPath.text = ResourcesLoader.GetDataPath("");

    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void LoadMainLevel()
    {
        SceneManager.LoadScene("Shop");      
    }
}
