using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MapUI : MonoBehaviour {

    //金钱的面板
    RectTransform MoneyBoard;
    RectTransform moneyIcon;
    Text moneyText;

    //挖矿的面板
    RectTransform mineBoard;
    RectTransform mineIcon;
    Text mineText;

    //保存角色信息
    PlayerInfo.Info playerInfo = new PlayerInfo.Info();

    //事件管理器
    ChatEventManager eventmanager;
    QuestManager questManager;

    // Use this for initialization
    void Start () {
        MoneyBoard = transform.Find("/ToolsKit/Canvas/PlayerInfo/glodBoard").GetComponent<RectTransform>();
        mineBoard = transform.Find("/ToolsKit/Canvas/PlayerInfo/mineBoard").GetComponent<RectTransform>();
        moneyIcon = MoneyBoard.FindChild("icon").GetComponent<RectTransform>();
        moneyText = MoneyBoard.FindChild("Text").GetComponent<Text>();

        mineIcon = mineBoard.FindChild("icon").GetComponent<RectTransform>();
        mineText = mineBoard.FindChild("Text").GetComponent<Text>();

        //获取事件控制器
        eventmanager = transform.Find("/ToolsKit/EventManager").GetComponent<ChatEventManager>();
        questManager = transform.Find("/ToolsKit/QuestManager").GetComponent<QuestManager>();

        //临时增加采集次数
        AddMineCount(10 - playerInfo.MineCount,null,false);
        AddMoney(100, null, false);
        ////临时增加金币数
        //if (playerInfo.Money < 100)
        //{
        //    AddMoney(100 - playerInfo.Money, null, false);
        //}

        //增加进入地图次数
        PlayerInfo.AddSenceInfo(1);

        //更新显示
        InstMapUI();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    //初始化显示地图ui
    void InstMapUI()
    {
        playerInfo = PlayerInfo.GetPlayerInfo();

        mineText.text = playerInfo.MineCount.ToString();
        moneyText.text = playerInfo.Money.ToString();

        bool ishit = false;
        ishit = eventmanager.CheckUnCompleteEvent() || ishit;
        ishit = eventmanager.CheckEventList(ChatEventManager.ChatEvent.EventTypeList.InMap, false) || ishit;
        ishit = eventmanager.CheckEventList(ChatEventManager.ChatEvent.EventTypeList.Mines, false) || ishit;
        ishit = eventmanager.CheckEventList(ChatEventManager.ChatEvent.EventTypeList.Golds, false) || ishit;
        if (ishit)
        {
            eventmanager.StartStory();
        }
    }

    //更新显示金币
    void UpdateMapUI(bool checkEvent)
    {
        playerInfo = PlayerInfo.GetPlayerInfo();

        mineText.text = playerInfo.MineCount.ToString();
        moneyText.text = playerInfo.Money.ToString();

        if (checkEvent)
        {
            bool ishit = false;
            ishit = eventmanager.CheckEventList(ChatEventManager.ChatEvent.EventTypeList.Mines, true) || ishit;
            ishit = eventmanager.CheckEventList(ChatEventManager.ChatEvent.EventTypeList.Golds, false) || ishit;
            if (ishit)
            {
                eventmanager.StartStory();
            }
        }
    }

    float actiontime = 0.25f;

    //增加金币
    public bool AddMoney(int num, RectTransform positon,bool checkEvent)
    {
        //如果大于一定值则返回false
        //if (playerInfo.Money + num > 999999)
        //    return false;

        PlayerInfo.ChangeMoney(num);
        UpdateMapUI(checkEvent);
        //Debug.Log(ishit);

        LeanTween.cancel(moneyIcon.gameObject);
        LeanTween.scale(moneyIcon, new Vector3(1.1f, 1.1f, 1.1f), actiontime).setLoopPingPong(1);

        if (positon != null)
            FlyToIcon(moneyIcon.GetComponent<Image>(), positon, num);

        questManager.CheckQuestListWithGold(num, -1);
        return true;
    }

    //减少金币
    public bool DownMoney(int num, RectTransform positon,bool checkEvent)
    {
        //如果不足则返回false
        if (playerInfo.Money - num < 0)
            return false;

        PlayerInfo.ChangeMoney(-num);
        UpdateMapUI(checkEvent);

        LeanTween.cancel(moneyIcon.gameObject);
        LeanTween.scale(moneyIcon, new Vector3(1.1f, 1.1f, 1.1f), actiontime).setLoopPingPong(1);

        FlyToPositon(moneyIcon.GetComponent<Image>(), positon, num);
        
        return true;
    }

    //增加采集次数
    public bool AddMineCount(int num, RectTransform positon,bool checkEvent)
    {
        //如果大于一定值则返回false
        //if (playerInfo.MineCount + num > 999999)
        //    return false;

        PlayerInfo.ChangeMineCount(num);
        UpdateMapUI(checkEvent);

        LeanTween.cancel(mineIcon.gameObject);
        LeanTween.scale(mineIcon, new Vector3(1.1f, 1.1f, 1.1f), actiontime).setLoopPingPong(1);

        if (positon!=null)
            FlyToIcon(mineIcon.GetComponent<Image>(), positon, num);

        return true;
    }

    //减少采集次数
    public bool DownMineCount(int num, RectTransform positon,bool checkEvent)
    {
        //如果不足则返回false
        if (playerInfo.MineCount - num < 0)
            return false;

        PlayerInfo.ChangeMineCount(-num);
        UpdateMapUI(checkEvent);

        LeanTween.cancel(mineIcon.gameObject);
        LeanTween.scale(mineIcon, new Vector3(1.1f, 1.1f, 1.1f), actiontime).setLoopPingPong(1);

        FlyToPositon(mineIcon.GetComponent<Image>(), positon, num);

        return true;
    }

    void FlyToPositon(Image img, RectTransform rect,int num)
    {
        if (num > 50)
            num = 50;

        for (int i = 0; i < num; i++)
        {
            GameObject obj = new GameObject();
            Image sr = obj.AddComponent<Image>();
            sr.sprite = img.sprite;

            RectTransform imgrect = img.GetComponent<RectTransform>();
            obj.GetComponent<RectTransform>().sizeDelta = imgrect.sizeDelta;

            obj.transform.SetParent(MoneyBoard.parent);

            obj.transform.position = imgrect.position;
            float randomlength = Random.Range(-100f, 100f);

            float time = 0.5f;
            float dely = i * 0.05f;
            LeanTween.moveY(obj, rect.position.y, time).setDelay(dely);
            LeanTween.moveX(obj, rect.position.x + randomlength, time / 2).setEase(LeanTweenType.easeOutQuad).setDelay(dely).setOnComplete(() =>
            {
                LeanTween.moveX(obj, rect.position.x, time / 2);
            });

            LeanTween.scale(obj, new Vector3(0.8f, 0.8f, 0.8f), time).setEase(LeanTweenType.easeOutQuad).setDelay(dely).setOnComplete(() =>
            {
                Destroy(obj);
                LeanTween.cancel(rect.gameObject);
                rect.localScale = new Vector3(1, 1, 1);
                LeanTween.scale(rect, new Vector3(1.05f, 1.05f, 1f), time / 2).setLoopPingPong(1);
            });
        }
    }


    void FlyToIcon(Image img, RectTransform rect, int num)
    {
        if (num > 50)
            num = 50;

        for (int i = 0; i < num; i++)
        {
            GameObject obj = new GameObject();
            Image sr = obj.AddComponent<Image>();
            sr.sprite = img.sprite;

            RectTransform imgrect = img.GetComponent<RectTransform>();
            obj.GetComponent<RectTransform>().sizeDelta = imgrect.sizeDelta;

            obj.transform.SetParent(MoneyBoard.parent);

            obj.transform.position = rect.position;
            float randomlength = Random.Range(-100f, 100f);

            float time = 0.5f;
            float dely = i * 0.05f;
            LeanTween.moveY(obj, imgrect.position.y, time).setDelay(dely);
            LeanTween.moveX(obj, imgrect.position.x + randomlength, time / 2).setEase(LeanTweenType.easeOutQuad).setDelay(dely).setOnComplete(() =>
            {
                LeanTween.moveX(obj, imgrect.position.x, time / 2);
            });

            LeanTween.scale(obj, new Vector3(0.8f, 0.8f, 0.8f), time).setEase(LeanTweenType.easeOutQuad).setDelay(dely).setOnComplete(() =>
            {
                Destroy(obj);
                LeanTween.cancel(imgrect.gameObject);
                imgrect.localScale = new Vector3(1, 1, 1);
                LeanTween.scale(imgrect, new Vector3(1.05f, 1.05f, 1f), time / 2).setLoopPingPong(1);
            });
        }
    }
}
