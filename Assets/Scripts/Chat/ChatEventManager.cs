using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ChatEventManager : MonoBehaviour {


	// Use this for initialization
	void Start () {
        XmlTool xl = new XmlTool();
        ChatEventsList = xl.loadChatEventListXmlToArray();
    }
	
	// Update is called once per frame
	void Update () {
	
	}
    public struct ChatEvent
    {
        public int ID;
        public string EventType;
        public int Num;
        public int[] Parameter;
        public int[] EventItem;
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
            newobj.AddComponent<AudioSource>();
        }
        chatmanager.SetNowScene(scence.name);
        chatmanager.LoadChatStory(stroy);
    }
}
