using UnityEngine;
using System;
using System.Collections;

public class CharBag : MonoBehaviour {

    //背包储存玩家获得的物品，物品拥有uid的唯一标示，可以堆叠的物品公用一个uid且用数量作出区分。
    //背包物品包含可以改变的属性，固定属性读取素材xml
    //储存、读取

    [Serializable]
    public struct Goods
    {
        public int UID;    //物品的标示
        public string Name; //名字
        public int Number;  //物品的数量
        public int Price;   //价格

        //物品的属性
        public int MateriralType; //背包物品ID；区分是item(1)还是mind(2)
        public int ID;     //子ID;
     
        public int Quality; //品质
        public int[] Property;    //属性
        public int MaterialEffet;    //材料特性
        public int Type;        //类型ID
    }

    static ArrayList GoodsList = new ArrayList();

	// Use this for initialization
	void Start () {


	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public static ArrayList GetGoodsList()
    {
        if (GoodsList.Count != 0)
            return GoodsList;
        else
            return new ArrayList();
    }

    static public int AddGoods(Goods goods)
    {
        int uid = 0;
        //uid从1开始
        if (GoodsList.Count > 0)
        {
            Goods last = (Goods)GoodsList[GoodsList.Count - 1];
            uid = last.UID + 1;
        }
        else
            uid = GoodsList.Count + 1;

        goods.UID = uid;

        GoodsList.Add(goods);

        PlayerData.BagData.UpdateBag(GoodsList);

        return uid;
    }

    //计算物品价格
    static public Goods SetPrice(Goods goods)
    {
        //通过特性来计算物品价格；TODO：在制作技能/特性属性之后添加
        goods.Price = (int)(goods.Price * 1.1f);
        return goods;
    }

    static public void RemoveGoods(int Uid)
    {
        foreach (Goods g in GoodsList)
        {
            if (g.UID == Uid)
            {
                GoodsList.Remove(g);
                return;
            }
        }
        PlayerData.BagData.UpdateBag(GoodsList);
    }


    static public void UpdateGoods(ArrayList bags)
    {
        GoodsList = bags;
    }
}
