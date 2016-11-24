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
        Scene sence = SceneManager.GetActiveScene();
        Debug.Log(sence.name);

        if (sence.name == "Shop")
        {
            Debug.Log("!!!!!!!!");
            ShopUI.ChangeStoryState();
            Character.ChangeStoryState();
        }

        GameObject newobj = new GameObject();
        newobj.name = "ChatSystem";
        ChatManager chatmanager = newobj.AddComponent<ChatManager>();
        chatmanager.LoadChatStory(stroy);
    }

}
