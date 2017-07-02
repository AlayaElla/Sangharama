using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MapUI : MonoBehaviour {

    //金钱的面板
    public RectTransform MoneyBoard;
    RectTransform moneyIcon;
    Text moneyText;

    //挖矿的面板
    public RectTransform mineBoard;
    RectTransform mineIcon;
    Text mineText;

    //保存角色信息
    PlayerInfo.Info playerInfo = new PlayerInfo.Info();
    ChatEventManager eventmanager;


	// Use this for initialization
	void Start () {
        eventmanager = transform.Find("/ToolsKit/EventManager").GetComponent<ChatEventManager>();

        moneyIcon = MoneyBoard.FindChild("icon").GetComponent<RectTransform>();
        moneyText = MoneyBoard.FindChild("Text").GetComponent<Text>();

        mineIcon = mineBoard.FindChild("icon").GetComponent<RectTransform>();
        mineText = mineBoard.FindChild("Text").GetComponent<Text>();

        //更新显示
        UpdateMapUI();
        
        //临时增加采集次数
        AddMineCount(10 - playerInfo.MineCount,null);

        //临时增加金币数
        if (playerInfo.Money < 100)
        {
            AddMoney(100 - playerInfo.Money, null);
        }
        eventmanager.CheckEventList(ChatEventManager.ChatEvent.EventTypeList.Mines, true);
        if (eventmanager.CheckEventList(ChatEventManager.ChatEvent.EventTypeList.Golds, false))
        {
            eventmanager.StartStory();
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    //更新显示金币
    void UpdateMapUI()
    {
        playerInfo = PlayerInfo.GetPlayerInfo();

        mineText.text = playerInfo.MineCount.ToString();
        moneyText.text = playerInfo.Money.ToString();
    }

    float actiontime = 0.25f;

    //增加金币
    public bool AddMoney(int num, RectTransform positon)
    {
        //如果大于一定值则返回false
        //if (playerInfo.Money + num > 999999)
        //    return false;

        PlayerInfo.ChangeMoney(num);
        UpdateMapUI();
        //Debug.Log(ishit);

        LeanTween.cancel(moneyIcon.gameObject);
        LeanTween.scale(moneyIcon, new Vector3(1.1f, 1.1f, 1.1f), actiontime).setLoopPingPong(1);

        if (positon != null)
            FlyToIcon(moneyIcon.GetComponent<Image>(), positon, num);

        return true;
    }

    //减少金币
    public bool DownMoney(int num, RectTransform positon)
    {
        //如果不足则返回false
        if (playerInfo.Money - num < 0)
            return false;

        PlayerInfo.ChangeMoney(-num);
        UpdateMapUI();

        LeanTween.cancel(moneyIcon.gameObject);
        LeanTween.scale(moneyIcon, new Vector3(1.1f, 1.1f, 1.1f), actiontime).setLoopPingPong(1);

        FlyToPositon(moneyIcon.GetComponent<Image>(), positon, num);
        
        return true;
    }

    //增加采集次数
    public bool AddMineCount(int num, RectTransform positon)
    {
        //如果大于一定值则返回false
        //if (playerInfo.MineCount + num > 999999)
        //    return false;

        PlayerInfo.ChangeMineCount(num);
        UpdateMapUI();

        LeanTween.cancel(mineIcon.gameObject);
        LeanTween.scale(mineIcon, new Vector3(1.1f, 1.1f, 1.1f), actiontime).setLoopPingPong(1);

        if (positon!=null)
            FlyToIcon(mineIcon.GetComponent<Image>(), positon, num);

        return true;
    }

    //减少采集次数
    public bool DownMineCount(int num, RectTransform positon)
    {
        //如果不足则返回false
        if (playerInfo.MineCount - num < 0)
            return false;

        PlayerInfo.ChangeMineCount(-num);
        UpdateMapUI();

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
