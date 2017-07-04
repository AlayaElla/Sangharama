using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ChatEventManager : MonoBehaviour {


	// Use this for initialization
	void Start () {
        XmlTool xl = new XmlTool();
        ChatEventsList = xl.loadChatEventListXmlToArray();
        StoryList = new ArrayList();
    }

    // Update is called once per frame
    void Update () {
	
	}
    public struct ChatEvent
    {
        public int ID;
        public enum EventTypeList {
            PutGoods=0,         //上架物品
            SellGoods,          //售出物品
            ComposeGoods,       //合成物品
            CollectGoods,       //采集物品
            ComposeProperty,    //合成属性
            InShop,             //进入商店界面
            InMap,              //进入地图界面
            Arrive,         //到达路点
            Mines,              //挖掘次数
            Golds               //金币数
        };
        public EventTypeList EventType;
        public int Num;
        public int[] Parameter;
        public int[] EventItem;
        public string StoryName;
    }

    ChatManager chatmanager;
    ArrayList ChatEventsList;
    ArrayList StoryList;

    public void StartStory()
    {
        if (StoryList.Count == 0 || StoryList == null)
        {
            Debug.LogError("Can't Start Story,Don't have StoryList!");
            return;
        }
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
        chatmanager.PushStoryList(StoryList);

        Loading.GetInstance().LoadingStoryScene((string)chatmanager.GetStoryList()[0], () =>
        {
            chatmanager.LoadChatStory((string)chatmanager.GetStoryList()[0]);
        });
    }

    public void StartStory(string storyname)
    {
        AddStroyName(storyname);
        StartStory();
    }

    /// <summary>
    /// 用于检查是否触发事件,物品判断的条件不管使用哪个EventType都一样
    /// </summary>
    /// <param name="EventType"></param>
    /// <returns></returns>
    public bool CheckEventList(ChatEvent.EventTypeList EventType,bool ClearStoryList)
    {
        bool ishit = false;
        if(ClearStoryList)  StoryList = new ArrayList();
        PlayerInfo.Info playerInfo = PlayerInfo.GetPlayerInfo();

        //筛选需要判断的事件
        ArrayList CheckList = new ArrayList();
        foreach (ChatEvent _event in ChatEventsList)
        {
            if (CompareEventType(_event.EventType, EventType))
                CheckList.Add(_event);
        }

        //事件判断
        foreach (ChatEvent _event in CheckList)
        {
            switch (_event.EventType)
            {
                //给物品相关的判断，统一处理
                case ChatEvent.EventTypeList.PutGoods:
                case ChatEvent.EventTypeList.SellGoods:
                case ChatEvent.EventTypeList.ComposeGoods:
                case ChatEvent.EventTypeList.CollectGoods:
                case ChatEvent.EventTypeList.ComposeProperty:        
                    break;
                case ChatEvent.EventTypeList.InShop:
                    break;
                case ChatEvent.EventTypeList.InMap:
                    break;
                case ChatEvent.EventTypeList.Arrive:
                    foreach (PlayerInfo.MapInfo mapinfo in playerInfo.MapInfoList)
                    {
                        if (mapinfo.ID == _event.Parameter[0] && _event.Num <= mapinfo.InCount && !playerInfo.CompleteEvents.Contains(_event.ID))
                        {
                            ishit = true;
                            playerInfo.CompleteEvents.Add(_event.ID);
                            AddStroyName(_event.StoryName);
                        }
                    }
                    break;
                case ChatEvent.EventTypeList.Mines:
                    if (_event.Num <= playerInfo.MineCount && !playerInfo.CompleteEvents.Contains(_event.ID))
                    {
                        ishit = true;
                        playerInfo.CompleteEvents.Add(_event.ID);
                        AddStroyName(_event.StoryName);
                    }
                    break;
                case ChatEvent.EventTypeList.Golds:
                    if (_event.Num <= playerInfo.Money&& !playerInfo.CompleteEvents.Contains(_event.ID))
                    {
                        ishit = true;
                        playerInfo.CompleteEvents.Add(_event.ID);
                        AddStroyName(_event.StoryName);
                    }
                    break;
                default:
                    Debug.LogWarning("Can't Check EventType:" + EventType + "!");
                    ishit = false;
                    break;
            }
        }
        return ishit;
    }

    bool CompareEventType(ChatEvent.EventTypeList type1, ChatEvent.EventTypeList type2)
    {
        bool ishit = false;
        switch (type1)
        {
            //给物品相关的判断，统一处理
            case ChatEvent.EventTypeList.PutGoods:
            case ChatEvent.EventTypeList.SellGoods:
            case ChatEvent.EventTypeList.ComposeGoods:
            case ChatEvent.EventTypeList.CollectGoods:
            case ChatEvent.EventTypeList.ComposeProperty:
                if (type2 == ChatEvent.EventTypeList.PutGoods
                    || type2 == ChatEvent.EventTypeList.SellGoods
                    || type2 == ChatEvent.EventTypeList.ComposeGoods
                    || type2 == ChatEvent.EventTypeList.CollectGoods
                    || type2 == ChatEvent.EventTypeList.ComposeProperty)
                    ishit = true;
                break;
            case ChatEvent.EventTypeList.InShop:
                if (type2 == ChatEvent.EventTypeList.InShop) ishit = true;
                break;
            case ChatEvent.EventTypeList.InMap:
                if (type2 == ChatEvent.EventTypeList.InMap) ishit = true;
                break;
            case ChatEvent.EventTypeList.Arrive:
                if (type2 == ChatEvent.EventTypeList.Arrive) ishit = true;
                break;
            case ChatEvent.EventTypeList.Mines:
                if (type2 == ChatEvent.EventTypeList.Mines) ishit = true;
                break;
            case ChatEvent.EventTypeList.Golds:
                if (type2 == ChatEvent.EventTypeList.Golds) ishit = true;
                break;
            default:
                Debug.LogWarning("Can't Check EventType:" + type1 + "!");
                ishit = false;
                break;
        }
        return ishit;
    }

    void AddStroyName(string name)
    {
        StoryList.Add(name);
    }
}
