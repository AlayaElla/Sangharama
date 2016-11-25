using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ChatEventManager : MonoBehaviour {


	// Use this for initialization
	void Start () {
        //StartStory("shop1_1");
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void StartStory(string stroy)
    {
        Scene scence = SceneManager.GetActiveScene();
        Debug.Log("Now scence: " + scence.name);

        if (scence.name == "Shop")
        {
            ShopUI.ChangeStoryState();
            Character.ChangeStoryState();
        }

        GameObject newobj = new GameObject();
        newobj.name = "ChatSystem";
        ChatManager chatmanager = newobj.AddComponent<ChatManager>();
        chatmanager.SetNowScene(scence.name);
        chatmanager.LoadChatStory(stroy);
    }

}
