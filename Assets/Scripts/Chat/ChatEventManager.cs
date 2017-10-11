using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChatEventManager : MonoBehaviour {

    public Sprite[] eventHint;

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
            Golds,              //金币数
            Quests              //任务事件，不用判断
        };
        public int GroupType;   //事件类型 0:故事事件；1:任务事件；2:隐藏事件
        public EventTypeList EventType;
        public int Num;
        public int[] Parameter;
        public int[] EventItem;
        public int NeedQuest;
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

        if (PlayerInfo.GetNowscene() == 0)
        {
            ShopUI.ChangeStoryState();
            Character.ChangeStoryState();
        }
        if (PlayerInfo.GetNowscene() > 0)
        {
            MapPathManager.ChangeStoryState();
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

        ChatEvent ct = (ChatEvent)chatmanager.GetStoryList()[0];
        Loading.GetInstance().LoadingStoryScene(ct.StoryName, () =>
        {
            chatmanager.LoadChatStory(ct.StoryName);
        });
    }

    public void StartStory(string storyname)
    {
        ChatEvent ct = new ChatEvent();
        ct.StoryName = storyname;
        ct.ID = -1;

        AddStroyName(ct);
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
                    if (_event.EventType == ChatEvent.EventTypeList.ComposeProperty)
                    {
                        foreach (PlayerInfo.PropertysInfo pinfo in playerInfo.MaterialInfoList.Propertys)
                        {
                            if (pinfo.ID == _event.Parameter[0] && _event.Num <= pinfo.RecipeCount && !PlayerInfo.CheckEvents(_event.ID))
                            {
                                if (((_event.EventItem == null) || (_event.EventItem != null && CharBag.ContainsGoods(_event.EventItem[0], _event.EventItem[1])))
                                    && (_event.NeedQuest==0||(_event.NeedQuest!=0&&PlayerInfo.CheckCompleteQuest(_event.NeedQuest))))
                                {
                                    ishit = true;
                                    PlayerInfo.AddEvents(_event.ID);
                                    Debug.Log("hit event: " + _event.ID);
                                    AddStroyName(_event);
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (_event.Parameter == null || _event.Parameter.Length <= 0) Debug.LogWarning("事件配置表Parameter出错！ eventID:" + _event.ID);
                        if (_event.Parameter[0] == 0)
                        {
                            foreach (PlayerInfo.ItemsInfo items in playerInfo.MaterialInfoList.Items)
                            {
                                if (items.ID == _event.Parameter[1] && _event.EventType == ChatEvent.EventTypeList.PutGoods && _event.Num <= items.PutCount && !PlayerInfo.CheckEvents(_event.ID))
                                {
                                    if (((_event.EventItem == null) || (_event.EventItem != null && CharBag.ContainsGoods(_event.EventItem[0], _event.EventItem[1])))
                                        && (_event.NeedQuest == 0 || (_event.NeedQuest != 0 && PlayerInfo.CheckCompleteQuest(_event.NeedQuest))))
                                    {
                                        ishit = true;
                                        PlayerInfo.AddEvents(_event.ID);
                                        Debug.Log("hit event: " + _event.ID);
                                        AddStroyName(_event);
                                        break;
                                    }
                                }
                                else if (items.ID == _event.Parameter[1] && _event.EventType == ChatEvent.EventTypeList.SellGoods && _event.Num <= items.SellCount && !PlayerInfo.CheckEvents(_event.ID))
                                {
                                    if (((_event.EventItem == null) || (_event.EventItem != null && CharBag.ContainsGoods(_event.EventItem[0], _event.EventItem[1])))
                                        && (_event.NeedQuest == 0 || (_event.NeedQuest != 0 && PlayerInfo.CheckCompleteQuest(_event.NeedQuest))))
                                    {
                                        ishit = true;
                                        PlayerInfo.AddEvents(_event.ID);
                                        Debug.Log("hit event: " + _event.ID);
                                        AddStroyName(_event);
                                        break;
                                    }
                                }
                                else if (items.ID == _event.Parameter[1] && _event.EventType == ChatEvent.EventTypeList.ComposeGoods && _event.Num <= items.RecipeCount && !PlayerInfo.CheckEvents(_event.ID))
                                {
                                    if (((_event.EventItem == null) || (_event.EventItem != null && CharBag.ContainsGoods(_event.EventItem[0], _event.EventItem[1])))
                                        && (_event.NeedQuest == 0 || (_event.NeedQuest != 0 && PlayerInfo.CheckCompleteQuest(_event.NeedQuest))))
                                    {
                                        ishit = true;
                                        PlayerInfo.AddEvents(_event.ID);
                                        Debug.Log("hit event: " + _event.ID);
                                        AddStroyName(_event);
                                        break;
                                    }
                                }
                                else if (items.ID == _event.Parameter[1] && _event.EventType == ChatEvent.EventTypeList.CollectGoods && _event.Num <= items.CollectCount && !PlayerInfo.CheckEvents(_event.ID))
                                {
                                    if (((_event.EventItem == null) || (_event.EventItem != null && CharBag.ContainsGoods(_event.EventItem[0], _event.EventItem[1])))
                                        && (_event.NeedQuest == 0 || (_event.NeedQuest != 0 && PlayerInfo.CheckCompleteQuest(_event.NeedQuest))))
                                    {
                                        ishit = true;
                                        PlayerInfo.AddEvents(_event.ID);
                                        Debug.Log("hit event: " + _event.ID);
                                        AddStroyName(_event);
                                        break;
                                    }
                                }
                            }
                        }   //item
                        else if (_event.Parameter[0] == 1)
                        {
                            foreach (PlayerInfo.MindsInfo minds in playerInfo.MaterialInfoList.Minds)
                            {
                                if (minds.ID == _event.Parameter[1] && _event.EventType == ChatEvent.EventTypeList.PutGoods && _event.Num <= minds.PutCount && !PlayerInfo.CheckEvents(_event.ID))
                                {
                                    if (((_event.EventItem == null) || (_event.EventItem != null && CharBag.ContainsGoods(_event.EventItem[0], _event.EventItem[1])))
                                        && (_event.NeedQuest == 0 || (_event.NeedQuest != 0 && PlayerInfo.CheckCompleteQuest(_event.NeedQuest))))
                                    {
                                        ishit = true;
                                        PlayerInfo.AddEvents(_event.ID);
                                        Debug.Log("hit event: " + _event.ID);
                                        AddStroyName(_event);
                                        break;
                                    }
                                }
                                else if (minds.ID == _event.Parameter[1] && _event.EventType == ChatEvent.EventTypeList.SellGoods && _event.Num <= minds.SellCount && !PlayerInfo.CheckEvents(_event.ID))
                                {
                                    if (((_event.EventItem == null) || (_event.EventItem != null && CharBag.ContainsGoods(_event.EventItem[0], _event.EventItem[1])))
                                        && (_event.NeedQuest == 0 || (_event.NeedQuest != 0 && PlayerInfo.CheckCompleteQuest(_event.NeedQuest))))
                                    {
                                        ishit = true;
                                        PlayerInfo.AddEvents(_event.ID);
                                        Debug.Log("hit event: " + _event.ID);
                                        AddStroyName(_event);
                                        break;
                                    }
                                }
                                else if (minds.ID == _event.Parameter[1] && _event.EventType == ChatEvent.EventTypeList.ComposeGoods && _event.Num <= minds.RecipeCount && !PlayerInfo.CheckEvents(_event.ID))
                                {
                                    if (((_event.EventItem == null) || (_event.EventItem != null && CharBag.ContainsGoods(_event.EventItem[0], _event.EventItem[1])))
                                        && (_event.NeedQuest == 0 || (_event.NeedQuest != 0 && PlayerInfo.CheckCompleteQuest(_event.NeedQuest))))
                                    {
                                        ishit = true;
                                        PlayerInfo.AddEvents(_event.ID);
                                        Debug.Log("hit event: " + _event.ID);
                                        AddStroyName(_event);
                                        break;
                                    }
                                }
                                else if (minds.ID == _event.Parameter[1] && _event.EventType == ChatEvent.EventTypeList.CollectGoods && _event.Num <= minds.CollectCount && !PlayerInfo.CheckEvents(_event.ID))
                                {
                                    if (((_event.EventItem == null) || (_event.EventItem != null && CharBag.ContainsGoods(_event.EventItem[0], _event.EventItem[1])))
                                        && (_event.NeedQuest == 0 || (_event.NeedQuest != 0 && PlayerInfo.CheckCompleteQuest(_event.NeedQuest))))
                                    {
                                        ishit = true;
                                        PlayerInfo.AddEvents(_event.ID);
                                        Debug.Log("hit event: " + _event.ID);
                                        AddStroyName(_event);
                                        break;
                                    }
                                }
                            }
                        }   //mind
                        else if (_event.Parameter[0] == 2)
                        {
                            foreach (PlayerInfo.SpecialItemsInfo spitems in playerInfo.MaterialInfoList.SpecialItems)
                            {
                                if (spitems.ID == _event.Parameter[1] && _event.EventType == ChatEvent.EventTypeList.PutGoods && _event.Num <= spitems.PutCount && !PlayerInfo.CheckEvents(_event.ID))
                                {
                                    if (((_event.EventItem == null) || (_event.EventItem != null && CharBag.ContainsGoods(_event.EventItem[0], _event.EventItem[1])))
                                        && (_event.NeedQuest == 0 || (_event.NeedQuest != 0 && PlayerInfo.CheckCompleteQuest(_event.NeedQuest))))
                                    {
                                        ishit = true;
                                        PlayerInfo.AddEvents(_event.ID);
                                        Debug.Log("hit event: " + _event.ID);
                                        AddStroyName(_event);
                                        break;
                                    }
                                }
                                else if (spitems.ID == _event.Parameter[1] && _event.EventType == ChatEvent.EventTypeList.SellGoods && _event.Num <= spitems.SellCount && !PlayerInfo.CheckEvents(_event.ID))
                                {
                                    if (((_event.EventItem == null) || (_event.EventItem != null && CharBag.ContainsGoods(_event.EventItem[0], _event.EventItem[1])))
                                        && (_event.NeedQuest == 0 || (_event.NeedQuest != 0 && PlayerInfo.CheckCompleteQuest(_event.NeedQuest))))
                                    {
                                        ishit = true;
                                        PlayerInfo.AddEvents(_event.ID);
                                        Debug.Log("hit event: " + _event.ID);
                                        AddStroyName(_event);
                                        break;
                                    }
                                }
                                else if (spitems.ID == _event.Parameter[1] && _event.EventType == ChatEvent.EventTypeList.ComposeGoods && _event.Num <= spitems.RecipeCount && !PlayerInfo.CheckEvents(_event.ID))
                                {
                                    if (((_event.EventItem == null) || (_event.EventItem != null && CharBag.ContainsGoods(_event.EventItem[0], _event.EventItem[1])))
                                        && (_event.NeedQuest == 0 || (_event.NeedQuest != 0 && PlayerInfo.CheckCompleteQuest(_event.NeedQuest))))
                                    {
                                        ishit = true;
                                        PlayerInfo.AddEvents(_event.ID);
                                        Debug.Log("hit event: " + _event.ID);
                                        AddStroyName(_event);
                                        break;
                                    }
                                }
                                else if (spitems.ID == _event.Parameter[1] && _event.EventType == ChatEvent.EventTypeList.CollectGoods && _event.Num <= spitems.CollectCount && !PlayerInfo.CheckEvents(_event.ID))
                                {
                                    if (((_event.EventItem == null) || (_event.EventItem != null && CharBag.ContainsGoods(_event.EventItem[0], _event.EventItem[1])))
                                        && (_event.NeedQuest == 0 || (_event.NeedQuest != 0 && PlayerInfo.CheckCompleteQuest(_event.NeedQuest))))
                                    {
                                        ishit = true;
                                        PlayerInfo.AddEvents(_event.ID);
                                        Debug.Log("hit event: " + _event.ID);
                                        AddStroyName(_event);
                                        break;
                                    }
                                }
                            }
                        }   //specail
                    }                      
                    break;
                case ChatEvent.EventTypeList.InShop:
                    PlayerInfo.SenceInfo shop_senceinfo = (PlayerInfo.SenceInfo)playerInfo.SenceInfoList[0];
                    if (_event.Num <= shop_senceinfo.InCount && !PlayerInfo.CheckEvents(_event.ID))
                    {
                        if (((_event.EventItem == null) || (_event.EventItem != null && CharBag.ContainsGoods(_event.EventItem[0], _event.EventItem[1])))
                            && (_event.NeedQuest == 0 || (_event.NeedQuest != 0 && PlayerInfo.CheckCompleteQuest(_event.NeedQuest))))
                        {
                            ishit = true;
                            PlayerInfo.AddEvents(_event.ID);
                            Debug.Log("hit event: " + _event.ID);
                            AddStroyName(_event);
                        }
                    }
                    break;
                case ChatEvent.EventTypeList.InMap:
                    PlayerInfo.SenceInfo map_senceinfo = (PlayerInfo.SenceInfo)playerInfo.SenceInfoList[1];
                    if (_event.Num <= map_senceinfo.InCount && !PlayerInfo.CheckEvents(_event.ID))
                    {
                        if (((_event.EventItem == null) || (_event.EventItem != null && CharBag.ContainsGoods(_event.EventItem[0], _event.EventItem[1])))
                            && (_event.NeedQuest == 0 || (_event.NeedQuest != 0 && PlayerInfo.CheckCompleteQuest(_event.NeedQuest))))
                        {
                            ishit = true;
                            PlayerInfo.AddEvents(_event.ID);
                            Debug.Log("hit event: " + _event.ID);
                            AddStroyName(_event);
                        }
                    }
                    break;
                case ChatEvent.EventTypeList.Arrive:
                    foreach (PlayerInfo.MapInfo mapinfo in playerInfo.MapInfoList)
                    {
                        if (mapinfo.ID == _event.Parameter[0] && _event.Num <= mapinfo.InCount && !PlayerInfo.CheckEvents(_event.ID))
                        {
                            if (((_event.EventItem == null) || (_event.EventItem != null && CharBag.ContainsGoods(_event.EventItem[0], _event.EventItem[1])))
                                && (_event.NeedQuest == 0 || (_event.NeedQuest != 0 && PlayerInfo.CheckCompleteQuest(_event.NeedQuest))))
                            {
                                ishit = true;
                                PlayerInfo.AddEvents(_event.ID);
                                Debug.Log("hit event: " + _event.ID);
                                AddStroyName(_event);
                            }
                            break;
                        }
                    }
                    break;
                case ChatEvent.EventTypeList.Mines:
                    if (_event.Num <= playerInfo.MineCount && !PlayerInfo.CheckEvents(_event.ID))
                    {
                        if (((_event.EventItem == null) || (_event.EventItem != null && CharBag.ContainsGoods(_event.EventItem[0], _event.EventItem[1])))
                            && (_event.NeedQuest == 0 || (_event.NeedQuest != 0 && PlayerInfo.CheckCompleteQuest(_event.NeedQuest))))
                        {
                            ishit = true;
                            PlayerInfo.AddEvents(_event.ID);
                            Debug.Log("hit event: " + _event.ID);
                            AddStroyName(_event);
                        }
                    }
                    break;
                case ChatEvent.EventTypeList.Golds:
                    if (_event.Num <= playerInfo.Money&& !PlayerInfo.CheckEvents(_event.ID))
                    {
                        if (((_event.EventItem == null) || (_event.EventItem != null && CharBag.ContainsGoods(_event.EventItem[0], _event.EventItem[1])))
                            && (_event.NeedQuest == 0 || (_event.NeedQuest != 0 && PlayerInfo.CheckCompleteQuest(_event.NeedQuest))))
                        {
                            ishit = true;
                            PlayerInfo.AddEvents(_event.ID);
                            Debug.Log("hit event: " + _event.ID);
                            AddStroyName(_event);
                        }
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
            case ChatEvent.EventTypeList.Quests:
                break;
            default:
                Debug.LogWarning("Can't Check EventType:" + type1 + "!");
                ishit = false;
                break;
        }
        return ishit;
    }

    void AddStroyName(ChatEvent events)
    {
        if(!StoryList.Contains(events))
            StoryList.Add(events);
    }

    public void AddStroyWithEventID(int id)
    {
        ChatEvent ct = FindEvetByID(id);
        if (ct.StoryName != null)
        {
            PlayerInfo.AddEvents(id);
            AddStroyName(ct);
        }
    }

    /// <summary>
    /// 检查是否有未完成事件
    /// </summary>
    /// <returns></returns>
    public bool CheckUnCompleteEvent()
    {
        bool ishit = false;
        PlayerInfo.Info playerInfo = PlayerInfo.GetPlayerInfo();
        foreach (PlayerInfo.EventInfo info in playerInfo.CompleteEvents)
        {
            if (info.Type == PlayerInfo.EventInfo.EventInfoType.Todo)
            {
                ChatEvent ct = FindEvetByID(info.ID);
                if (ct.StoryName != null)
                {
                    AddStroyName(ct);
                    ishit = true;
                }
            }
        }
        return ishit;
    }

    ChatEvent FindEvetByID(int id)
    {
        foreach (ChatEvent _event in ChatEventsList)
        {
            if (_event.ID == id)
                return _event;
        }
        return new ChatEvent();
    }


    //预检查事件 ，用于显示可以触发的任务
    /// <summary>
    /// 预检查事件 ，用于显示可以触发的任务
    /// sceneID 0;shop;1:map
    /// </summary>
    /// <param name="sceneID"></param>
    /// <returns></returns>
    public void PreCheckEventList(int sceneID)
    {
        ArrayList preCheckEvent = new ArrayList();
        PlayerInfo.Info playerInfo = PlayerInfo.GetPlayerInfo();

        //筛选需要判断的事件
        ArrayList CheckList = new ArrayList();
        foreach (ChatEvent _event in ChatEventsList)
        {
            if (_event.EventType == ChatEvent.EventTypeList.Arrive
                || _event.EventType == ChatEvent.EventTypeList.InMap
                || _event.EventType == ChatEvent.EventTypeList.InShop)
                CheckList.Add(_event);
        }

        //事件判断
        foreach (ChatEvent _event in CheckList)
        {
            switch (_event.EventType)
            {
                case ChatEvent.EventTypeList.InShop:
                    if (sceneID == 0) break;

                    PlayerInfo.SenceInfo shop_senceinfo = (PlayerInfo.SenceInfo)playerInfo.SenceInfoList[0];
                    if (_event.Num <= shop_senceinfo.InCount + 1 && !PlayerInfo.CheckEvents(_event.ID))
                    {
                        if (((_event.EventItem == null) || (_event.EventItem != null && CharBag.ContainsGoods(_event.EventItem[0], _event.EventItem[1])))
                            && (_event.NeedQuest == 0 || (_event.NeedQuest != 0 && PlayerInfo.CheckCompleteQuest(_event.NeedQuest))))
                        {
                            Debug.Log("prehit event: " + _event.ID);
                            preCheckEvent.Add(_event);
                        }
                    }
                    break;
                case ChatEvent.EventTypeList.InMap:
                    if (sceneID == 1) break;

                    PlayerInfo.SenceInfo map_senceinfo = (PlayerInfo.SenceInfo)playerInfo.SenceInfoList[1];
                    if (_event.Num <= map_senceinfo.InCount + 1 && !PlayerInfo.CheckEvents(_event.ID))
                    {
                        if (((_event.EventItem == null) || (_event.EventItem != null && CharBag.ContainsGoods(_event.EventItem[0], _event.EventItem[1])))
                            && (_event.NeedQuest == 0 || (_event.NeedQuest != 0 && PlayerInfo.CheckCompleteQuest(_event.NeedQuest))))
                        {
                            Debug.Log("prehit event: " + _event.ID);
                            preCheckEvent.Add(_event);
                        }
                    }
                    break;
                case ChatEvent.EventTypeList.Arrive:
                    foreach (PlayerInfo.MapInfo mapinfo in playerInfo.MapInfoList)
                    {
                        if (mapinfo.ID == _event.Parameter[0]&& _event.Num <= mapinfo.InCount + 1 && !PlayerInfo.CheckEvents(_event.ID))
                        {
                            if (((_event.EventItem == null) || (_event.EventItem != null && CharBag.ContainsGoods(_event.EventItem[0], _event.EventItem[1])))
                                && (_event.NeedQuest == 0 || (_event.NeedQuest != 0 && PlayerInfo.CheckCompleteQuest(_event.NeedQuest))))
                            {
                                Debug.Log("prehit event: " + _event.ID);
                                preCheckEvent.Add(_event);
                            }
                            break;
                        }
                    }
                    break;
                default:
                    Debug.LogWarning("Can't Check EventType:" + _event.EventType + "!");
                    break;
            }
        }
        if (preCheckEvent.Count > 0)
            ShowEventHint(sceneID, preCheckEvent);
    }

    void ShowEventHint(int sceneID,ArrayList preCheckEvent)
    {
        Transform root;
        if (sceneID == 0)
            root = GameObject.Find("Canvas/Button/").transform;
        else if (sceneID == 1)
            root = GameObject.Find("Canvas/Scroll View/Viewport/Content/map/").transform;
        else
            root = null;
        //事件判断
        foreach (ChatEvent _event in preCheckEvent)
        {
            Transform p;
            switch (_event.EventType)
            {
                case ChatEvent.EventTypeList.InShop:
                    if (sceneID == 0) break;
                    p = root.Find("action1/1");
                    CreateHintSprite(sceneID,p, _event.GroupType);
                    break;
                case ChatEvent.EventTypeList.InMap:
                    if (sceneID == 1) break;
                    CreateHintSprite(sceneID,root, _event.GroupType);
                    break;
                case ChatEvent.EventTypeList.Arrive:
                    if (sceneID == 0)
                    {
                        CreateHintSprite(sceneID,root, _event.GroupType);
                        break;
                    };

                    p = root.Find("action" + _event.Parameter[0] + "/" + _event.Parameter[0]);
                    CreateHintSprite(sceneID,p, _event.GroupType);
                    break;
                default:
                    Debug.LogWarning("Can't Check EventType:" + _event.EventType + "!");
                    break;
            }
        }
    }

    void CreateHintSprite(int sceneID,Transform t,int type)
    {
        //type 2隐藏任务，直接跳过
        if (type == 2) return;

        Transform tt = t.Find("hint");
        if (tt == null)
        {
            GameObject obj = new GameObject();
            obj.name = "hint";
            obj.transform.SetParent(t, false);
            if (sceneID == 0)
                obj.transform.localPosition = new Vector2(-50, 0);
            else if (sceneID == 1)
                obj.transform.localPosition = new Vector2(0, 0);
            Image img = obj.AddComponent<Image>();
            obj.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0);
            if (type == 0)
                img.color = Color.white;
            else if (type == 1)
                img.color = Color.blue;

            AniController.Get(obj.gameObject).AddSprite(eventHint);
            AniController.Get(obj.gameObject).PlayAni(0, 3, AniController.AniType.Loop, 5);
        }
        else
        {
            Image img = tt.GetComponent<Image>();
            tt.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0);
            if (type == 0)
                img.color = Color.white;
            else if (type == 1)
                img.color = Color.blue;

            AniController.Get(tt.gameObject).AddSprite(eventHint);
            AniController.Get(tt.gameObject).PlayAni(0, 3, AniController.AniType.Loop, 5);
        }
    }
}
