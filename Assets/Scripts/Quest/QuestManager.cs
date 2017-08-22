using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour {

    ArrayList QuestList = new ArrayList();
    ArrayList QuesGrouptList = new ArrayList();

    ArrayList NowQuestInfoList = new ArrayList();
    ArrayList NowQuestList = new ArrayList();
    ArrayList NewQuests = new ArrayList();

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
        PutGoods = 0,       //上架物品
        SellGoods,          //售出物品
        ComposeGoods,       //合成物品
        CollectGoods,       //采集物品
        ComposeProperty,    //合成属性
        Arrive,             //到达路点
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
        UIInstance = gameObject.GetComponent<QuestUI>();

        AddShowQuest(1);
        AddShowQuest(2);
    }
	
	// Update is called once per frame
	void Update () {
		
	}


    public void AddShowQuest(int questID)
    {
        AddQuestToList(questID);
        UIInstance.AddQustUI(GetQuestInfoByID(questID));
    }

    //添加任务到显示new的列表中
    void AddQuestToNew(int questID)
    {
        NewQuests.Add(questID);
    }
    //去除任务显示new
    void RemoveQuestToNew(int questID)
    {
        if(NewQuests.Count>0)
            NewQuests.Remove(questID);
    }

    public ArrayList GetNewQuests()
    {
        return NewQuests;
    }

    //添加任务信息到列表
    void AddQuestToList(int questID)
    {
        //添加任务信息记录，方便以后查找
        QuestBase Quest = new QuestBase();
        Quest = GetQuestInfoByID(questID);
        NowQuestInfoList.Add(Quest);

        //初始化任务信息到角色信息
        PlayerInfo.QuestInfo newQuest = new PlayerInfo.QuestInfo();
        newQuest.ID = Quest.ID;
        newQuest.Goal = Quest.QuestComplete.Num;
        newQuest.Progress = 0;
        newQuest.Type = PlayerInfo.QuestInfo.QuestInfoType.Todo;
        newQuest.TaskPoint = Quest.Award.TaskPoint;
        PlayerInfo.GetPlayerInfo().QuestList.Add(newQuest);

        AddQuestToNew(questID);
    }

    //查找任务信息
    public QuestBase GetQuestInfoByID(int questID)
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

    public int GetQuestProgress(int questID)
    {
        foreach (PlayerInfo.QuestInfo q in NowQuestList)
        {
            if (q.ID == questID)
            {
                return q.Progress;
            }
        }

        Debug.LogWarning("Can't find questID: " + questID);
        return 0;
    }

    public void OpenQuestBoardInUI(int questID,float delyTime)
    {
        RemoveQuestToNew(questID);
        UIInstance.OpenQuestBoard(questID, delyTime);
    }
}
