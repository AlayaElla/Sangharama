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
    }

    static Info playerinfo;

    [Serializable]
    public struct ItemsInfo
    {
        public int ID;     //ID

        //事件判断
        public int PutCount;    //上架次数
        public int RecipeCount;    //合成次数
        public int CollectCount;    //采集次数
    }

    [Serializable]
    public struct MindsInfo
    {
        public int ID;     //ID

        //事件判断
        public int PutCount;    //上架次数
        public int RecipeCount;    //合成次数
        public int CollectCount;    //采集次数
    }

    [Serializable]
    public struct SpecialItemsInfo
    {
        public int ID;     //ID 0:shop；1:map

        //事件判断
        public int PutCount;    //上架次数
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


	// Use this for initialization
	void Start () {
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
            PlayerData.PlayerInfoData.SaveData("PlayerInfoData.sav");
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

        //初始材料
        foreach (Materiral.Items m in Materiral.GetItemList())
        {
            ItemsInfo _m = new ItemsInfo();
            _m.ID = m.ID;
            _m.PutCount = 0;
            _m.RecipeCount = 0;
            _m.CollectCount = 0;
            playerinfo.MaterialInfoList.Items.Add(_m);
        }
        foreach (Materiral.Minds m in Materiral.GetMindList())
        {
            MindsInfo _m = new MindsInfo();
            _m.ID = m.ID;
            _m.PutCount = 0;
            _m.RecipeCount = 0;
            _m.CollectCount = 0;
            playerinfo.MaterialInfoList.Minds.Add(_m);
        }
        foreach (Materiral.SpecialItem m in Materiral.GetSpecialItemList())
        {
            SpecialItemsInfo _m = new SpecialItemsInfo();
            _m.ID = m.ID;
            _m.PutCount = 0;
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
        //foreach (MapPathManager.Path p in MapPathManager.GetPathList())
        //{
        //    MapInfo m = new MapInfo();
        //    m.ID = p.Map;
        //    m.InCount = 0;
        //    playerinfo.MapInfoList.Add(m);
        //}
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
}
