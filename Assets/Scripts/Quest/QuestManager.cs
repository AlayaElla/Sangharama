using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour {


    ArrayList QuestList = new ArrayList();
    ArrayList QuesGrouptList = new ArrayList();

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
        QuestList = xt.loadRecipeXmlToArray();
        QuesGrouptList = xt.loadPropertyRecipeXmlToArray();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
