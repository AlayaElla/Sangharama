using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour {

    ArrayList QuestList = new ArrayList();
    ArrayList QuesGrouptList = new ArrayList();
    ArrayList NowQuestList = new ArrayList();

    QuestUI UIInstance;

    //定义任务的结构体
    public struct QuestBase
    {
        public int ID;
        public int Questgroup;
        public int GroupID;
        public string Smallicon;
        public string Bigicon;
        public string name;
        public string des;

        //前置条件
        public struct QuestNeedBase
        {
            public int PreQuest;
            public int[] NeedGoods;
        }
        public QuestNeedBase QuestNeed;

        //达成条件
        public struct QuestCompleteBase
        {
            public QuestTypeList QuestType;
            public int Num;
            public int[] Parameter;
        }
        public QuestCompleteBase QuestComplete;

        //奖励
        public struct AwardBase
        {
            public int Gold;
            public int Exp;
            public string Stroy;
            public int TaskPoint;
        }
        public AwardBase Award;
    }

    public struct QuestGroupBase
    {

        public int ID;
        public GroupTypeList GroupType;
        public string Name;
    }

    //任务类型
    public enum QuestTypeList
    {
        PutGoods = 0,         //上架物品
        SellGoods,          //售出物品
        ComposeGoods,       //合成物品
        CollectGoods,       //采集物品
        ComposeProperty,    //合成属性
        Arrive,              //到达路点
        Golds               //持有金币数
    };

    //任务类型
    public enum GroupTypeList
    {
        Main = 0,         //主线
        Sub,          //支线
        Secret       //秘密
    };

	// Use this for initialization
	void Start () {
        XmlTool xt = new XmlTool();
        QuestList = xt.loadQuestXmlToArray();
        QuesGrouptList = xt.loadQuestGroupXmlToArray();

        NowQuestList = PlayerInfo.GetPlayerInfo().QuestList;

        //获取UI
        UIInstance = this.GetComponent<QuestUI>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    //更新任务信息到列表
    void AddQuestInfoToList(int questID)
    {
        QuestBase Quest = new QuestBase();
        Quest = GetQuestInfoByID(questID);
    }

    //查找任务信息
    QuestBase GetQuestInfoByID(int questID)
    {
        foreach (QuestBase q in QuestList)
        {
            if (q.ID == questID)
            {
                return q;
            }
        }
        Debug.LogWarning("Can't find questID: " + questID);
        return new QuestBase();
    }

    //查找任务组信息
    QuestGroupBase GetQuestGroupInfo(int groupID)
    {
        foreach (QuestGroupBase g in QuestList)
        {
            if (g.ID == groupID)
            {
                return g;
            }
        }
        Debug.LogWarning("Can't find questgroupID: " + groupID);
        return new QuestGroupBase();
    }


    /// <summary>
    /// 用于检查是否触发事件,物品判断的条件不管使用哪个EventType都一样
    /// </summary>
    /// <param name="QuestType"></param>
    /// <returns></returns>
    public bool CheckQuestList(QuestTypeList Type)
    {
        bool ishit = false;

        //事件判断
        foreach (ChatEvent _event in NowQuestList)
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
                                if ((_event.EventItem == null) || (_event.EventItem != null && CharBag.ContainsGoods(_event.EventItem[0], _event.EventItem[1])))
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
                                    if ((_event.EventItem == null) || (_event.EventItem != null && CharBag.ContainsGoods(_event.EventItem[0], _event.EventItem[1])))
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
                                    if ((_event.EventItem == null) || (_event.EventItem != null && CharBag.ContainsGoods(_event.EventItem[0], _event.EventItem[1])))
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
                                    if ((_event.EventItem == null) || (_event.EventItem != null && CharBag.ContainsGoods(_event.EventItem[0], _event.EventItem[1])))
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
                                    if ((_event.EventItem == null) || (_event.EventItem != null && CharBag.ContainsGoods(_event.EventItem[0], _event.EventItem[1])))
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
                                    if ((_event.EventItem == null) || (_event.EventItem != null && CharBag.ContainsGoods(_event.EventItem[0], _event.EventItem[1])))
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
                                    if ((_event.EventItem == null) || (_event.EventItem != null && CharBag.ContainsGoods(_event.EventItem[0], _event.EventItem[1])))
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
                                    if ((_event.EventItem == null) || (_event.EventItem != null && CharBag.ContainsGoods(_event.EventItem[0], _event.EventItem[1])))
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
                                    if ((_event.EventItem == null) || (_event.EventItem != null && CharBag.ContainsGoods(_event.EventItem[0], _event.EventItem[1])))
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
                                    if ((_event.EventItem == null) || (_event.EventItem != null && CharBag.ContainsGoods(_event.EventItem[0], _event.EventItem[1])))
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
                                    if ((_event.EventItem == null) || (_event.EventItem != null && CharBag.ContainsGoods(_event.EventItem[0], _event.EventItem[1])))
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
                                    if ((_event.EventItem == null) || (_event.EventItem != null && CharBag.ContainsGoods(_event.EventItem[0], _event.EventItem[1])))
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
                                    if ((_event.EventItem == null) || (_event.EventItem != null && CharBag.ContainsGoods(_event.EventItem[0], _event.EventItem[1])))
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
                        if ((_event.EventItem == null) || (_event.EventItem != null && CharBag.ContainsGoods(_event.EventItem[0], _event.EventItem[1])))
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
                        if ((_event.EventItem == null) || (_event.EventItem != null && CharBag.ContainsGoods(_event.EventItem[0], _event.EventItem[1])))
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
                            if ((_event.EventItem == null) || (_event.EventItem != null && CharBag.ContainsGoods(_event.EventItem[0], _event.EventItem[1])))
                            {
                                ishit = true;
                                PlayerInfo.AddEvents(_event.ID);
                                Debug.Log("hit event: " + _event.ID);
                                AddStroyName(_event);
                                break;
                            }
                        }
                    }
                    break;
                case ChatEvent.EventTypeList.Mines:
                    if (_event.Num <= playerInfo.MineCount && !PlayerInfo.CheckEvents(_event.ID))
                    {
                        if ((_event.EventItem == null) || (_event.EventItem != null && CharBag.ContainsGoods(_event.EventItem[0], _event.EventItem[1])))
                        {
                            ishit = true;
                            PlayerInfo.AddEvents(_event.ID);
                            Debug.Log("hit event: " + _event.ID);
                            AddStroyName(_event);
                        }
                    }
                    break;
                case ChatEvent.EventTypeList.Golds:
                    if (_event.Num <= playerInfo.Money && !PlayerInfo.CheckEvents(_event.ID))
                    {
                        if ((_event.EventItem == null) || (_event.EventItem != null && CharBag.ContainsGoods(_event.EventItem[0], _event.EventItem[1])))
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

    //void AddStroyName(ChatEvent events)
    //{
    //    StoryList.Add(events);
    //}


}
