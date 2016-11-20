using UnityEngine;
using System.Collections;

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
        GameObject newobj = new GameObject();
        newobj.name = "ChatSystem";
        ChatManager chatmanager = newobj.AddComponent<ChatManager>();
        chatmanager.LoadChatStory(stroy);
    }

}
