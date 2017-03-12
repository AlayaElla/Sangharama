using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ChatEventManager : MonoBehaviour {


	// Use this for initialization
	void Start () {
        //GetGoods(2, 3);
    }
	
	// Update is called once per frame
	void Update () {
	
	}
    public struct ChatEvent
    {
        public string EventType;
    }


    ChatManager chatmanager;
    ArrayList ChatEventsList;


    public void StartStory(string stroy)
    {
        Scene scence = SceneManager.GetActiveScene();
        Debug.Log("Now scence: " + scence.name);

        if (scence.name == "Shop")
        {
            ShopUI.ChangeStoryState();
            Character.ChangeStoryState();
        }

        if (chatmanager == null)
        {
            GameObject newobj = new GameObject();
            newobj.name = "ChatSystem";
            chatmanager = newobj.AddComponent<ChatManager>();
        }
        chatmanager.SetNowScene(scence.name);
        chatmanager.LoadChatStory(stroy);
    }

    public void GetGoods(int materialType, int ID)
    {
        CharBag.AddGoodsByID(materialType, ID);
    }
}
