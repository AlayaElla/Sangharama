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
    }
    static Info playerinfo = new Info();

	// Use this for initialization
	void Start () {
        LaodPlayerInfo();
	}
	
	// Update is called once per frame
	void Update () {
	
	}


    //读取角色信息
    void LaodPlayerInfo()
    {
        ArrayList _info = PlayerData.PlayerInfoData.Load();
        if (_info != null)
        {
            playerinfo = (Info)_info[0];
        }
        else
        {
            playerinfo = new Info();
            Debug.Log("dont have playerinfo!");
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

}
