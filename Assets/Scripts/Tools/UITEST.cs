using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;


public class UITEST : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void LoadTestLevel()
    {
        SceneManager.LoadScene("Map");
    }

    public void LoadTest2Level()
    {
        SceneManager.LoadScene("Shop");        
    }

    public void LoadTest3Level()
    {
        Loading.GetInstance().LoadingStoryScene("start",()=>
        {
            SceneManager.LoadScene("Story");
        });
        
    }
}
