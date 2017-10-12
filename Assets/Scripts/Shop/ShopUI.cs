using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ShopUI : MonoBehaviour {

    static bool isopenRecipe = true;
    static bool isInStory = false;
    static bool isCheckEvent = true;

    BagUI _bagInstance;
    GameObject _bagUI;
    Text Money;
    Text Mine;

    //商店的商品列表
    CharBag.Goods[] goodslist = new CharBag.Goods[18];

    //事件管理器
    ChatEventManager eventmanager;
    QuestManager questManager;

    // Use this for initialization
    void Start () {
        _bagUI = Resources.Load<GameObject>("Prefab/BagUI");
        Money = transform.Find("/ToolsKit/Canvas/PlayerInfo/glodBoard/Text").GetComponent<Text>();
        Mine = transform.Find("/ToolsKit/Canvas/PlayerInfo/mineBoard/Text").GetComponent<Text>();

        //获取事件控制器
        eventmanager = transform.Find("/ToolsKit/EventManager").GetComponent<ChatEventManager>();
        questManager = transform.Find("/ToolsKit/QuestManager").GetComponent<QuestManager>();

        UpdateShopGoodsData();
        InitShopGoods();
        UpdateShopMoney();

        EventTriggerListener.Get(GameObject.Find("Canvas/Button (2)")).onClick = OpenSpecialItemBag;
        EventTriggerListener.Get(GameObject.Find("Canvas/Button (3)")).onClick = OpenSpecialItemBagAll;
        EventTriggerListener.Get(GameObject.Find("Canvas/Button (4)")).onClick = PlayerInfo.ClearCompleteEvents;

        //增加进入地图次数
        PlayerInfo.AddSenceInfo(0);

        bool ishit = false;
        ishit = eventmanager.CheckUnCompleteEvent() || ishit;
        ishit = eventmanager.CheckEventList(ChatEventManager.ChatEvent.EventTypeList.InShop, false) || ishit;
        ishit = eventmanager.CheckEventList(ChatEventManager.ChatEvent.EventTypeList.SellGoods, false) || ishit;
        if (ishit)
        {
            eventmanager.StartStory();
        }
        else
        {
            eventmanager.PreCheckEventList(0);
            questManager.PreCheckQuest(0);
            questManager.IsArriveWaitingCheckPoint(0);
        }
    }
	
	// Update is called once per frame
	void Update () {

        if (isInStory)
            return;

        //检查事件
        if (!isInStory && isCheckEvent == false)
        {
            isCheckEvent = true;
            CheckEvent();
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            //当前检测到的是否是UI层   
            if (EventSystem.current.IsPointerOverGameObject())
            {
                //是UI的时候，执行相关的UI操作
                Debug.Log("点击到了UI");
                return;
            }

            Vector2 mousepos = Input.mousePosition;
            Vector2 mouseToWorld = Camera.main.ScreenToWorldPoint(mousepos);
            //设定相应动画的按钮                
            RaycastHit2D hit;
            hit = Physics2D.Raycast(mouseToWorld, Vector2.zero, 100, 1 << 5);
            if (hit && isopenRecipe)
            {
                int ID = int.Parse(hit.transform.name.ToString());
                Debug.Log(ID);
                OpenBag(ID);
            }
        }
	}

    //打开特殊物品背包
    void OpenSpecialItemBag(GameObject obj)
    {
        _bagInstance = Instantiate(_bagUI).GetComponent<BagUI>();
        ////初始化
        _bagInstance.Initialize(2);

        Parameter.Box p = new Parameter.Box();
        p.callback = ClickInBag;
        _bagInstance.SetGoodsName(2);

        ChangeRecipeUiState();
    }

    //打开特殊物品背包
    void OpenSpecialItemBagAll(GameObject obj)
    {
        _bagInstance = Instantiate(_bagUI).GetComponent<BagUI>();
        ////初始化
        _bagInstance.Initialize(2);

        Parameter.Box p = new Parameter.Box();
        p.callback = ClickInBag;
        _bagInstance.SetGoodsName(-1);

        ChangeRecipeUiState();
    }


    public static void ChangeStoryState()
    {
        isInStory = !isInStory;
        if (isInStory == false)
            isCheckEvent = false;
    }

    void CheckEvent()
    {
        eventmanager.PreCheckEventList(0);
        questManager.PreCheckQuest(0);
        questManager.IsArriveWaitingCheckPoint(0);
    }

    public static void ChangeRecipeUiState()
    {
        isopenRecipe = !isopenRecipe;
    }

    //打开背包
    void OpenBag(int slotID)
    {
        _bagInstance = Instantiate(_bagUI).GetComponent<BagUI>();
        ////初始化
        _bagInstance.Initialize(2);
        
        Parameter.Box p = new Parameter.Box();
        p.ID = slotID;
        p.callback = ClickInBag;
        p.callbackByEvent = RemoveGoods;
        //如果货架上有商品，则添加道具到背包
        if (goodslist[slotID - 1].Name != null && goodslist[slotID - 1].ID != 0)
        {
            p.obj = goodslist[slotID - 1];
        }
        _bagInstance.OpenBagMenuInShop(p);

        ChangeRecipeUiState();
    }

    //上架商品
    void ClickInBag(GameObject go, object parameter)
    {
        Parameter.Box p = (Parameter.Box)parameter;
        CharBag.Goods goods = (CharBag.Goods)p.obj;

        //如果货架上有商品，则添加道具到背包
        if (goodslist[p.ID - 1].Name != null && goodslist[p.ID - 1].ID != 0)
        {
            CharBag.AddGoods(goodslist[p.ID - 1]);
        }

        //添加选中的商品到商品列
        goodslist[p.ID - 1] = goods;
        string path=p.ID.ToString() + "/itemIcon";
        transform.Find(path).GetComponent<SpriteRenderer>().sprite = Materiral.GetMaterialIcon(goods.MateriralType, goods.ID);

        //显示商品价格
        string priceiconpath = p.ID.ToString() + "/price/icon";
        string pricepath = p.ID.ToString() + "/price/text";

        transform.Find(priceiconpath).gameObject.SetActive(true);
        transform.Find(pricepath).gameObject.SetActive(true);
        transform.Find(pricepath).GetComponent<TextMesh>().text = goods.Price.ToString();


        //移除背包中的商品
        CharBag.RemoveGoods(goods.UID);

        _bagInstance.CloseBagMenu(gameObject);
        ChangeRecipeUiState();

        PlayerData.ShopGoodsData.SaveShopGoods(goodslist);
        CharBag.SaveBagGoods();

        questManager.CheckQuestListWithGoods(QuestManager.QuestTypeList.PutGoods, goods, 0);

        //更新物品信息
        PlayerInfo.AddGoodsInfo(goods.MateriralType, goods.ID, PlayerInfo.GoodsInfoType.PutCount);
    }

    void RemoveGoods(GameObject go, object parameter)
    {
        Parameter.Box p = (Parameter.Box)parameter;
        //CharBag.Goods goods = (CharBag.Goods)p.obj;

        //如果货架上有商品，则添加道具到背包
        if (goodslist[p.ID - 1].Name != null && goodslist[p.ID - 1].ID != 0)
        {
            CharBag.AddGoods(goodslist[p.ID - 1]);
            goodslist[p.ID - 1] = new CharBag.Goods();
        }

        string path = p.ID.ToString() + "/itemIcon";
        transform.Find(path).GetComponent<SpriteRenderer>().sprite = null;

        //取消显示价格
        string priceiconpath = p.ID.ToString() + "/price/icon";
        string pricepath = p.ID.ToString() + "/price/text";
        transform.Find(priceiconpath).gameObject.SetActive(false);
        transform.Find(pricepath).gameObject.SetActive(false);

        _bagInstance.CloseBagMenu(gameObject);
        ChangeRecipeUiState();

        PlayerData.ShopGoodsData.SaveShopGoods(goodslist);
    }

    //购买物品
    public void BuyedGoods(MyCharacterController.ShopGood goods)
    {
        //如果货架上有商品则移除
        if (goodslist[goods.slotID - 1].Name != null && goodslist[goods.slotID - 1].ID != 0)
        {
            goodslist[goods.slotID - 1] = new CharBag.Goods();

            string path = goods.slotID.ToString() + "/itemIcon";
            transform.Find(path).GetComponent<SpriteRenderer>().sprite = null;

            //取消显示价格
            string priceiconpath = goods.slotID.ToString() + "/price/icon";
            string pricepath = goods.slotID.ToString() + "/price/text";
            transform.Find(priceiconpath).gameObject.SetActive(false);
            transform.Find(pricepath).gameObject.SetActive(false);

            questManager.CheckQuestListWithGoods(QuestManager.QuestTypeList.SellGoods, goods.goods, 0);

            PlayerData.ShopGoodsData.SaveShopGoods(goodslist);
            //更新物品信息
            PlayerInfo.AddGoodsInfo(goods.goods.MateriralType, goods.goods.ID, PlayerInfo.GoodsInfoType.SellCount);
        }
        else
        {
            Debug.Log("can't remove good:" + goods.goods.Name + " ,because don't have!");
        }
    }

    public CharBag.Goods[] GetGoods()
    {
        return goodslist;
    }

    //保存上架的商品
    void UpdateShopGoodsData()
    {
#if _Debug
        Debug.Log("读取商店物品存档..");
#endif
        ArrayList _shopgoods = PlayerData.ShopGoodsData.LoadData();
        if (_shopgoods != null)
        {
            object[] _s = _shopgoods.ToArray();
            for (int i = 0; i < _s.Length; i++)
            {
                goodslist[i] = (CharBag.Goods)_shopgoods[i];
            }
        }
    }

    //进入商店场景时显示上架的商品
    void InitShopGoods()
    {
#if _Debug
        Debug.Log("配置商店物品..");
#endif

        for (int i = 0; i < goodslist.Length; i++)
        {
            string priceiconpath = (i + 1).ToString() + "/price/icon";
            string pricepath = (i + 1).ToString() + "/price/text";
            transform.Find(priceiconpath).gameObject.SetActive(false);
            transform.Find(pricepath).gameObject.SetActive(false);

            if (goodslist[i].Name != null && goodslist[i].ID != 0)
            {
                transform.Find(priceiconpath).gameObject.SetActive(true);
                transform.Find(pricepath).gameObject.SetActive(true);

                string path = (i+1).ToString() + "/itemIcon";
                transform.Find(path).GetComponent<SpriteRenderer>().sprite = Materiral.GetMaterialIcon(goodslist[i].MateriralType, goodslist[i].ID);
            }
        }
    }

    //进入商店时，显示金币
    void UpdateShopMoney()
    {
#if _Debug
        Debug.Log("读取角色存档..");
#endif

        PlayerInfo.Info info = PlayerInfo.GetPlayerInfo();

        Money.text = info.Money.ToString();
        Mine.text = info.MineCount.ToString();
    }

    //增加金币
    public void AddMoney(int num)
    {
        PlayerInfo.ChangeMoney(num);
        UpdateShopMoney();
        questManager.CheckQuestListWithGold(num, -1);
    }

}
