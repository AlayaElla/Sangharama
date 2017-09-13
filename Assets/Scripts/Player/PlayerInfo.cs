using UnityEngine;
using System;
using System.Collections;

public class PlayerInfo : MonoBehaviour {

    [Serializable]
    public struct Info
    {
        public int Money;   //金钱
        public int MineCount;   //采集次数
        public string Languege;

        public MaterialInfo MaterialInfoList;
        public ArrayList SenceInfoList;
        public ArrayList MapInfoList;
        public ArrayList CompleteEvents;
        public ArrayList QuestList;
        public ArrayList CompleteQuests;
    }

    static Info playerinfo;

    [Serializable]
    public struct ItemsInfo
    {
        public int ID;     //ID

        //事件判断
        public int PutCount;    //上架次数
        public int SellCount;    //卖出次数
        public int RecipeCount;    //合成次数
        public int CollectCount;    //采集次数
    }

    [Serializable]
    public struct MindsInfo
    {
        public int ID;     //ID

        //事件判断
        public int PutCount;    //上架次数
        public int SellCount;    //卖出次数
        public int RecipeCount;    //合成次数
        public int CollectCount;    //采集次数
    }

    [Serializable]
    public struct SpecialItemsInfo
    {
        public int ID;     //ID 0:shop；1:map

        //事件判断
        public int PutCount;    //上架次数
        public int SellCount;    //卖出次数
        public int RecipeCount;    //合成次数
        public int CollectCount;    //采集次数
    }

    [Serializable]
    public struct PropertysInfo
    {
        public int ID;     //ID

        //事件判断
        public int RecipeCount;    //合成次数
    }

    //材料信息，用于判断事件
    [Serializable]
    public struct MaterialInfo
    {
        public ArrayList Items;
        public ArrayList Minds;
        public ArrayList SpecialItems;
        public ArrayList Propertys;
    }
    
    //进入场景的信息，用于判断事件
    [Serializable]
    public struct SenceInfo
    {
        public int ID;     //ID     0:shop; 1:map

        //事件判断
        public int InCount;    //进入次数次数
    }

    //进入地图路点的信息，用于判断事件
    [Serializable]
    public struct MapInfo
    {
        public int ID;     //ID

        //事件判断
        public int InCount;    //进入次数次数
    }

    //事件的状态类型
    [Serializable]
    public struct EventInfo
    {
        public enum EventInfoType
        {
            Todo=0,
            Complete
        }
        public EventInfoType Type;
        public int ID;     //ID
    }

    //任务的状态类型
    [Serializable]
    public struct QuestInfo
    {
        public enum QuestInfoType
        {
            Todo = 0,
            WaitingCheck,
            Complete
        }
        public QuestInfoType Type;
        public int Progress;
        public int Goal;
        public int TaskPoint;
        public int ID;     //ID
    }
    static int Nowscene = 0;

	// Use this for initialization
	void Awake () {
        playerinfo = new Info();
        LoadPlayerInfo();
	}
	
	// Update is called once per frame
	void Update () {
	
	}


    //读取角色信息
    void LoadPlayerInfo()
    {
        ArrayList _info = PlayerData.PlayerInfoData.Load();
        if (_info != null)
        {
            playerinfo = (Info)_info[0];
        }
        else
        {
            playerinfo = new Info();
            InitPlayerInfoData();
            Debug.Log("dont have playerinfo!");
            PlayerData.PlayerInfoData.Save(playerinfo);
        }
    }

    void InitPlayerInfoData()
    {
        playerinfo.Languege = "zh";
        playerinfo.MineCount = 0;
        playerinfo.Money = 0;

        playerinfo.MapInfoList = new ArrayList();
        playerinfo.MaterialInfoList = new MaterialInfo();
        playerinfo.MaterialInfoList.Items = new ArrayList();
        playerinfo.MaterialInfoList.Minds = new ArrayList();
        playerinfo.MaterialInfoList.SpecialItems = new ArrayList();
        playerinfo.MaterialInfoList.Propertys = new ArrayList();
        playerinfo.SenceInfoList = new ArrayList();
        playerinfo.CompleteEvents = new ArrayList();
        playerinfo.QuestList = new ArrayList();
        playerinfo.CompleteQuests = new ArrayList();

        //初始材料
        foreach (Materiral.Items m in Materiral.GetItemList())
        {
            ItemsInfo _m = new ItemsInfo();
            _m.ID = m.ID;
            _m.PutCount = 0;
            _m.SellCount = 0;
            _m.RecipeCount = 0;
            _m.CollectCount = 0;
            playerinfo.MaterialInfoList.Items.Add(_m);
        }
        foreach (Materiral.Minds m in Materiral.GetMindList())
        {
            MindsInfo _m = new MindsInfo();
            _m.ID = m.ID;
            _m.PutCount = 0;
            _m.SellCount = 0;
            _m.RecipeCount = 0;
            _m.CollectCount = 0;
            playerinfo.MaterialInfoList.Minds.Add(_m);
        }
        foreach (Materiral.SpecialItem m in Materiral.GetSpecialItemList())
        {
            SpecialItemsInfo _m = new SpecialItemsInfo();
            _m.ID = m.ID;
            _m.PutCount = 0;
            _m.SellCount = 0;
            _m.RecipeCount = 0;
            _m.CollectCount = 0;
            playerinfo.MaterialInfoList.SpecialItems.Add(_m);
        }
        foreach (Materiral.Property m in Materiral.GetPropertyList())
        {
            PropertysInfo _m = new PropertysInfo();
            _m.ID = m.ID;
            _m.RecipeCount = 0;
            playerinfo.MaterialInfoList.Propertys.Add(_m);
        }

        //初始化场景数据
        for (int i = 0; i <= 1; i++)
        {
            SenceInfo s = new SenceInfo();
            s.ID = i;
            s.InCount = 0;
            playerinfo.SenceInfoList.Add(s);
        }

        ////初始化地图路点数据
        XmlTool xt = new XmlTool();
        ArrayList _list = xt.loadPathXmlToArray();
        MapPathManager.Path[] PathList = new MapPathManager.Path[_list.Count];
        _list.CopyTo(PathList);
        xt = null;_list.Clear();

        foreach (MapPathManager.Path p in PathList)
        {
            MapInfo m = new MapInfo();
            m.ID = p.Map;
            m.InCount = 0;
            playerinfo.MapInfoList.Add(m);
        }
    }


    //外部获取角色信息
    static public Info GetPlayerInfo()
    {
        return playerinfo;
    }

    //改变语言
    static public void SetLanguege(string lan)
    {
        playerinfo.Languege = lan;
        PlayerData.PlayerInfoData.Save(playerinfo);
    }

    //改变金钱
    static public void ChangeMoney(int num)
    {
        playerinfo.Money += num;
        PlayerData.PlayerInfoData.Save(playerinfo);
    }

    //改变挖掘次数
    static public void ChangeMineCount(int num)
    {
        playerinfo.MineCount += num;
        PlayerData.PlayerInfoData.Save(playerinfo);
    }

    //增加路点次数信息
    static public void AddMapInfo(int id)
    {
        for (int i = 0; i < playerinfo.MapInfoList.Count; i++)
        {
            MapInfo info = (MapInfo)playerinfo.MapInfoList[i];
            if (info.ID== id)
            {
                info.InCount++;
                playerinfo.MapInfoList[i] = info;
                break;
            }
        }
        PlayerData.PlayerInfoData.Save(playerinfo);
    }

    /// <summary>
    ///增加场景进入次数信息;ID 0:shop; 1:map
    /// </summary>
    /// <param name="id"></param>
    static public void AddSenceInfo(int id)
    {
        for (int i = 0; i < playerinfo.SenceInfoList.Count; i++)
        {
            SenceInfo info = (SenceInfo)playerinfo.SenceInfoList[i];
            if (info.ID == id)
            {
                info.InCount++;
                playerinfo.SenceInfoList[i] = info;
                break;
            }
        }
        PlayerData.PlayerInfoData.Save(playerinfo);
    }

    //_m.PutCount = 0;
    //    _m.SellCount = 0;
    //    _m.RecipeCount = 0;
    //    _m.CollectCount = 0;

    public enum GoodsInfoType
    {
        PutCount=0,
        SellCount,
        RecipeCount,
        CollectCount
    }

    static public void AddGoodsInfo(int goodstype, int id, GoodsInfoType type)
    {
        ArrayList goodlist = new ArrayList();
        if (goodstype == 0)
        {
            goodlist = playerinfo.MaterialInfoList.Items;
            //查找对应的物品信息
            for (int i = 0; i <= goodlist.Count - 1; i++)
            {
                ItemsInfo info = (ItemsInfo)goodlist[i];
                if (info.ID == id)
                {
                    if (type == GoodsInfoType.PutCount)
                    {
                        info.PutCount++;
                        goodlist[i] = info;
                    }
                    else if (type == GoodsInfoType.SellCount)
                    {
                        info.SellCount++;
                        goodlist[i] = info;
                    }
                    else if (type == GoodsInfoType.RecipeCount)
                    {
                        info.RecipeCount++;
                        goodlist[i] = info;
                    }
                    else if (type == GoodsInfoType.CollectCount)
                    {
                        info.CollectCount++;
                        goodlist[i] = info;
                    }
                    playerinfo.MaterialInfoList.Items = goodlist;
                    break;
                }
            }
        }
        else if (goodstype == 1)
        {
            goodlist = playerinfo.MaterialInfoList.Minds;
            //查找对应的物品信息
            for (int i = 0; i <= goodlist.Count - 1; i++)
            {
                MindsInfo info = (MindsInfo)goodlist[i];
                if (info.ID == id)
                {
                    if (type == GoodsInfoType.PutCount)
                    {
                        info.PutCount++;
                        goodlist[i] = info;
                    }
                    else if (type == GoodsInfoType.SellCount)
                    {
                        info.SellCount++;
                        goodlist[i] = info;
                    }
                    else if (type == GoodsInfoType.RecipeCount)
                    {
                        info.RecipeCount++;
                        goodlist[i] = info;
                    }
                    else if (type == GoodsInfoType.CollectCount)
                    {
                        info.CollectCount++;
                        goodlist[i] = info;
                    }
                    playerinfo.MaterialInfoList.Minds = goodlist;
                    break;
                }
            }
        }
        else if (goodstype == 0)
        {
            goodlist = playerinfo.MaterialInfoList.SpecialItems;
            //查找对应的物品信息
            for (int i = 0; i <= goodlist.Count - 1; i++)
            {
                SpecialItemsInfo info = (SpecialItemsInfo)goodlist[i];
                if (info.ID == id)
                {
                    if (type == GoodsInfoType.PutCount)
                    {
                        info.PutCount++;
                        goodlist[i] = info;
                    }
                    else if (type == GoodsInfoType.SellCount)
                    {
                        info.SellCount++;
                        goodlist[i] = info;
                    }
                    else if (type == GoodsInfoType.RecipeCount)
                    {
                        info.RecipeCount++;
                        goodlist[i] = info;
                    }
                    else if (type == GoodsInfoType.CollectCount)
                    {
                        info.CollectCount++;
                        goodlist[i] = info;
                    }
                    playerinfo.MaterialInfoList.SpecialItems = goodlist;
                    break;
                }
            }
        }
        else
        {
            Debug.Log("Unknow goodstype: " + goodstype);
            return;
        }
        PlayerData.PlayerInfoData.Save(playerinfo);
    }


    //增加事件
    static public void AddEvents(int Event)
    {
        EventInfo info = new EventInfo();
        info.ID = Event;
        info.Type = EventInfo.EventInfoType.Todo;

        playerinfo.CompleteEvents.Add(info);
        PlayerData.PlayerInfoData.Save(playerinfo);
    }

    /// <summary>
    /// 判断是否已经完成事件,true已经完成，false没有完成
    /// </summary>
    /// <param name="Event"></param>
    static public bool CheckEvents(int Event)
    {
        foreach (EventInfo i in playerinfo.CompleteEvents)
        {
            if (i.ID == Event)
                return true;
        }
        return false;
    }

    //设置事件为完成
    static public void SetEventsCompelete(int Event)
    {
        for (int i = 0; i < playerinfo.CompleteEvents.Count; i++)
        {
            EventInfo info =(EventInfo)playerinfo.CompleteEvents[i];
            if (info.ID == Event)
            {
                info.Type = EventInfo.EventInfoType.Complete;
                playerinfo.CompleteEvents[i] = info;
                PlayerData.PlayerInfoData.Save(playerinfo);
                return;
            }
        }
    }

    static public void ClearCompleteEvents(GameObject obj)
    {
        playerinfo.CompleteEvents.Clear();
        Debug.Log("Clear CompleteEvents!\nEvents Count:" + playerinfo.CompleteEvents.Count);
        PlayerData.PlayerInfoData.Save(playerinfo);
    }


    //增加任务
    static public void AddQuest(QuestInfo Quest)
    {
        playerinfo.QuestList.Add(Quest);
        PlayerData.PlayerInfoData.Save(playerinfo);
    }

    //增加完成时间
    static public void AddCompleteQuest(int Quest)
    {
        playerinfo.CompleteQuests.Add(Quest);
        PlayerData.PlayerInfoData.Save(playerinfo);
    }

    //获取任务进度
    static public int GetQuestProgress(int quest)
    {
        for (int i = 0; i < playerinfo.QuestList.Count; i++)
        {
            QuestInfo info = (QuestInfo)playerinfo.QuestList[i];
            if (info.ID == quest)
            {
                return info.Progress;
            }
        }
        return -1;
    }

    //获取任务状态
    static public QuestInfo.QuestInfoType GetQuestStatus(int quest)
    {
        for (int i = 0; i < playerinfo.QuestList.Count; i++)
        {
            QuestInfo info = (QuestInfo)playerinfo.QuestList[i];
            if (info.ID == quest)
            {
                return info.Type;
            }
        }
        return QuestInfo.QuestInfoType.Complete;
    }

    //增加任务状态
    static public void AddQuestProgress(int quest, int num, int max)
    {
        for (int i = 0; i < playerinfo.QuestList.Count; i++)
        {
            QuestInfo info = (QuestInfo)playerinfo.QuestList[i];
            if (info.ID == quest)
            {
                info.Progress = Mathf.Clamp(info.Progress + num, 0, max);
                if (info.Progress >= max)
                    info.Type = QuestInfo.QuestInfoType.WaitingCheck;
                playerinfo.QuestList[i] = info;
                return;
            }
        }
        Debug.Log("Can't find quest:" + quest);
    }

    //获取任务信息
    static public QuestInfo GetQuestInfo(int quest)
    {
        for (int i = 0; i < playerinfo.QuestList.Count; i++)
        {
            QuestInfo info = (QuestInfo)playerinfo.QuestList[i];
            if (info.ID == quest)
            {
                return info;
            }
        }
        Debug.Log("Can't find quest:" + quest);
        return new QuestInfo();
    }

    //清空任务列表
    static public void ClearQuestList()
    {
        playerinfo.QuestList.Clear();
        PlayerData.PlayerInfoData.Save(playerinfo);
    }

    static public int GetNowscene()
    {
        return Nowscene;
    }

    static public void SetNowscene(int NowsceneID)
    {
        Nowscene = NowsceneID;
    }
}
