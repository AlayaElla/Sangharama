using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestUI : MonoBehaviour {

    GameObject QuestListUI;
    GameObject QuestButton;
    GameObject QuestBoard;
    GameObject QuestCanvas;

    QuestManager questManager;
    public Sprite[] hintsprites;
    // Use this for initialization
    void Start () {
        questManager = gameObject.GetComponent<QuestManager>();

        QuestListUI = GameObject.Find("ToolsKit/Canvas/QuestlList/Viewport/Content");
        QuestCanvas= GameObject.Find("ToolsKit/Canvas");
        QuestButton = Resources.Load<GameObject>("Prefab/Quest/quest");
        QuestBoard = Resources.Load<GameObject>("Prefab/Quest/QuestBoardLayer");
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void AddQustUI(QuestManager.QuestBase quest)
    {
        GameObject questButton = Instantiate(QuestButton);
        questButton.name = quest.ID.ToString();
        questButton.transform.SetParent(QuestListUI.transform, false);

        Image iconImage = questButton.transform.FindChild("icon").GetComponent<Image>();
        Slider prograssBar = questButton.transform.FindChild("progress").GetComponent<Slider>();
        Transform hint = questButton.transform.FindChild("hint");

        iconImage.sprite = Materiral.GetIconByName(quest.Smallicon);
        prograssBar.value = (float)PlayerInfo.GetQuestProgress(quest.ID) / quest.QuestComplete.Num;
        //如果已经完成
        if (prograssBar.value >= 1)
        {
            hint.gameObject.SetActive(true);
            AniController.Get(hint.gameObject).AddSprite(hintsprites);
            hint.GetComponent<Image>().color = Color.green;
            AniController.Get(hint.gameObject).PlayAni(0, 3, AniController.AniType.Loop, 5);
        }
        EventTriggerListener.Get(questButton).onClick = OpenQuestBoard;
    }

    void OpenQuestBoard(GameObject go)
    {
        OpenQuestBoard(int.Parse(go.name), 0);
    }

    public void OpenQuestBoard(int questID,float delyTime)
    {
        QuestManager.QuestBase quest = questManager.GetQuestInfoByID(questID);

        GameObject Board = Instantiate(QuestBoard);
        Board.name = quest.ID.ToString();
        Board.transform.SetParent(QuestCanvas.transform, false);

        //获取面板组件
        Image iconImage = Board.transform.FindChild("QuestBoard/icon").GetComponent<Image>();
        Text questinfoText = Board.transform.FindChild("QuestBoard/questinfo/Text").GetComponent<Text>();
        Text CompleteinfoText = Board.transform.FindChild("QuestBoard/questinfo/CompleteText").GetComponent<Text>();
        Text questnameText = Board.transform.FindChild("QuestBoard/questName/questText").GetComponent<Text>();
        Slider prograssBar = Board.transform.FindChild("QuestBoard/questName/progress").GetComponent<Slider>();
        Text prograssBarText = Board.transform.FindChild("QuestBoard/questName/progress/progressText").GetComponent<Text>();
        GameObject closebutton = Board.transform.FindChild("QuestBoard/Close").gameObject;
        Text closebuttonText = closebutton.transform.FindChild("Text").GetComponent<Text>();
        GameObject Mask = Board.transform.FindChild("Mask").gameObject;
        GameObject questBoardBG = closebutton.transform.parent.gameObject;
        //Award
        Text awardGoldNum = Board.transform.FindChild("QuestBoard/award/GoldNum").GetComponent<Text>();
        Text awardExpNum = Board.transform.FindChild("QuestBoard/award/ExpNum").GetComponent<Text>();
        Image AwardGoodsImage = Board.transform.FindChild("QuestBoard/award/Goods").GetComponent<Image>();
        Text AwardGoodsNum = Board.transform.FindChild("QuestBoard/award/GoodsNum").GetComponent<Text>();

        //更新参数
        iconImage.sprite = Materiral.GetIconByName(quest.Bigicon);
        questinfoText.text = quest.des;
        questnameText.text = quest.name;
        awardGoldNum.text = "x" + quest.Award.Gold;
        awardExpNum.text= "x" + quest.Award.Exp;
        AwardGoodsImage.sprite = Materiral.GetMaterialIcon(quest.Award.Goods[0], quest.Award.Goods[1]);
        AwardGoodsNum.text= "x" + quest.Award.GoodsNum;

        prograssBar.value = (float)questManager.GetQuestProgress(quest.ID) / quest.QuestComplete.Num;
        prograssBarText.text = questManager.GetQuestProgress(quest.ID) + "/" + quest.QuestComplete.Num;

        //设置按钮响应
        if (prograssBar.value >= 1)
        {
            if (PlayerInfo.GetNowscene() == quest.Award.TaskPoint)
            {
                //如果在任务地点则设置领取
                closebutton.GetComponent<Image>().color = Color.green;
                closebuttonText.text = "领取";
                EventTriggerListener.Get(closebutton).onClick = GetQuestAward;
            }
            else
            {
                //关闭按钮
                closebutton.GetComponent<Image>().color = Color.white;
                closebuttonText.text = "关闭";
                CompleteinfoText.text = quest.completedes;
                EventTriggerListener.Get(closebutton).onClick = CloseQuestBoard;
            }
        }
        else
        {
            //关闭按钮
            closebutton.GetComponent<Image>().color = Color.white;
            closebuttonText.text = "关闭";
            EventTriggerListener.Get(closebutton).onClick = CloseQuestBoard;
        }

        questBoardBG.transform.localPosition = new Vector2(0, Screen.height / 2 + 200);
        LeanTween.moveLocalY(closebutton.transform.parent.gameObject, 300, 0.5f).setEaseOutBack().setDelay(delyTime);
    }

    void CloseQuestBoard(GameObject go)
    {
        Transform board = go.transform.parent.parent;
        LeanTween.moveLocalY(go.transform.parent.gameObject, Screen.height / 2 + 200, 0.3f).setEaseInBack().setOnComplete(()=>
        {
            Destroy(board.gameObject);

            if (questManager.GetNewQuests().Count > 0)
            {
                questManager.OpenQuestBoardInUI((int)questManager.GetNewQuests()[0], 0);
            }
        });
    }

    public void UpdateQuestUI(int quest,int point)
    {
        PlayerInfo.QuestInfo info = PlayerInfo.GetQuestInfo(quest);
        Transform questButton = QuestListUI.transform.FindChild(info.ID.ToString());
        Slider prograssBar = questButton.transform.FindChild("progress").GetComponent<Slider>();
        Transform hint = questButton.transform.FindChild("hint");

        prograssBar.value = (float)info.Progress / info.Goal;

        //如果已经完成
        if (prograssBar.value >= 1)
        {
            hint.gameObject.SetActive(true);
            AniController.Get(hint.gameObject).AddSprite(hintsprites);
            hint.GetComponent<Image>().color = Color.green;
            AniController.Get(hint.gameObject).PlayAni(0, 3, AniController.AniType.Loop, 5);
            questManager.PreCheckQuest(PlayerInfo.GetNowscene());
        }
    }

    public void ShowEventHint(int sceneID, ArrayList preCheckQuest)
    {
        Transform root;
        if (sceneID == 0)
            root = GameObject.Find("Canvas/Button/").transform;
        else if (sceneID >= 1)
            root = GameObject.Find("Canvas/Scroll View/Viewport/Content/map/").transform;
        else
            root = null;
        //事件判断
        foreach (PlayerInfo.QuestInfo _quest in preCheckQuest)
        {
            Transform p;
            if (sceneID == 0)
            {
                if (_quest.TaskPoint > 1)
                {
                    CreateHintSprite(sceneID, root);
                }
                break;
            }
            else if (sceneID >= 1)
            {
                p = root.FindChild("action" + _quest.TaskPoint + "/" + _quest.TaskPoint);
                CreateHintSprite(sceneID, p);
            }
            else
            {
                Debug.Log("Unknow sceneID: " + sceneID);
            }
        }
    }

    void CreateHintSprite(int sceneID, Transform t)
    {
        Transform tt = t.FindChild("hint");
        if (tt == null)
        {
            GameObject obj = new GameObject();
            obj.name = "hint";
            obj.transform.SetParent(t, false);
            if (sceneID == 0)
                obj.transform.localPosition = new Vector2(-50, 0);
            else if (sceneID >= 1)
                obj.transform.localPosition = new Vector2(0, 0);
            Image img = obj.AddComponent<Image>();
            RectTransform rt = obj.GetComponent<RectTransform>();
            rt.pivot = new Vector2(0.5f, 0);
            rt.sizeDelta = new Vector2(70, 70);
            img.color = Color.green;

            AniController.Get(obj.gameObject).AddSprite(hintsprites);
            AniController.Get(obj.gameObject).PlayAni(0, 3, AniController.AniType.Loop, 5);
        }
        else
        {
            Image img = tt.GetComponent<Image>();
            RectTransform rt = tt.GetComponent<RectTransform>();
            rt.pivot = new Vector2(0.5f, 0);
            rt.sizeDelta = new Vector2(70, 70);
            img.color = Color.green;

            AniController.Get(tt.gameObject).AddSprite(hintsprites);
            AniController.Get(tt.gameObject).PlayAni(0, 3, AniController.AniType.Loop, 5);
        }
    }

    void GetQuestAward(GameObject go)
    {
        Transform Board = go.transform.parent.parent;
        Transform root = Board.parent;
        Transform MoneyBoard = transform.Find("/ToolsKit/Canvas/PlayerInfo/glodBoard");
        Transform mineBoard = transform.Find("/ToolsKit/Canvas/PlayerInfo/mineBoard");
        //Award
        Transform awardGold = Board.transform.FindChild("QuestBoard/award/Gold");
        Transform awardExp = Board.transform.FindChild("QuestBoard/award/Exp");
        Transform AwardGoods = Board.transform.FindChild("QuestBoard/award/Goods");

        QuestManager.QuestBase info = questManager.GetQuestInfoByID(int.Parse(Board.name));

        //增加奖励
        PlayerInfo.ChangeMoney(info.Award.Gold);
        for (int i = 0; i < info.Award.GoodsNum; i++)
        {
            CharBag.AddGoodsByID(info.Award.Goods[0], info.Award.Goods[1]);
        }

        //完成任务
        PlayerInfo.AddCompleteQuest(info.ID);
        questManager.RemoveQuestToList(info.ID);
        //销毁任务面板的任务
        Transform questbutton = QuestListUI.transform.FindChild(info.ID.ToString());
        if (questbutton != null) Destroy(questbutton.gameObject);

        //生成金币
        for (int i = 0; i < info.Award.Gold; i = i + 50)
        {
            Transform gold = Instantiate(awardGold, root);
            gold.position = awardGold.position;
            float randomrange = 100f;
            float time = 0.5f;
            LeanTween.moveY(gold.gameObject, gold.position.y+Random.Range(-randomrange,randomrange), time).setEase(LeanTweenType.easeOutSine);
            LeanTween.moveX(gold.gameObject, gold.position.x + Random.Range(-randomrange, randomrange), time).setEase(LeanTweenType.easeOutSine);

            LeanTween.moveX(gold.gameObject, MoneyBoard.position.x, time).setDelay(time * Random.Range(1.5f, 2f));
            LeanTween.moveY(gold.gameObject, MoneyBoard.position.y, time).setDelay(time * Random.Range(1.5f, 2f)).setOnComplete(
                ()=>
                {
                    Destroy(gold.gameObject);
                });
            if (i > 500) break;
        }

        //生成经验
        for (int i = 0; i < info.Award.Exp; i = i + 50)
        {
            Transform exp = Instantiate(awardExp, root);
            exp.position = awardExp.position;
            float randomrange = 100f;
            float time = 0.5f;
            LeanTween.moveY(exp.gameObject, exp.position.y + Random.Range(-randomrange, randomrange), time).setEase(LeanTweenType.easeOutSine);
            LeanTween.moveX(exp.gameObject, exp.position.x + Random.Range(-randomrange, randomrange), time).setEase(LeanTweenType.easeOutSine);

            LeanTween.moveX(exp.gameObject, mineBoard.position.x, time).setDelay(time * Random.Range(1.5f, 2f));
            LeanTween.moveY(exp.gameObject, mineBoard.position.y, time).setDelay(time * Random.Range(1.5f, 2f)).setOnComplete(
                () =>
                {
                    Destroy(exp.gameObject);
                });

            if (i > 500) break;
        }

        //生成物品
        for (int i = 0; i < info.Award.GoodsNum; i++)
        {
            Transform goods = Instantiate(AwardGoods, root);
            goods.position = AwardGoods.position;
            float randomrange = 100f;
            float time = 0.5f;
            LeanTween.moveY(goods.gameObject, AwardGoods.position.y + Random.Range(-randomrange, randomrange), time).setEase(LeanTweenType.easeOutSine);
            LeanTween.moveX(goods.gameObject, AwardGoods.position.x + Random.Range(-randomrange, randomrange), time).setEase(LeanTweenType.easeOutSine);

            LeanTween.moveX(goods.gameObject, mineBoard.position.x + 100, time).setDelay(time * Random.Range(1.5f, 2f));
            LeanTween.moveY(goods.gameObject, mineBoard.position.y, time).setDelay(time * Random.Range(1.5f, 2f)).setOnComplete(
                () =>
                {
                    Destroy(goods.gameObject);
                });

            if (i > 10) break;
        }

        LeanTween.moveLocalY(go.transform.parent.gameObject, Screen.height / 2 + 200, 0.3f).setEaseInBack().setOnComplete(() =>
        {
            Destroy(Board.gameObject);
        });

        //延迟更新显示
        LeanTween.delayedCall(0.5f* 2f, ()=>{

            PlayerInfo.Info playerInfo = PlayerInfo.GetPlayerInfo();
            MoneyBoard.FindChild("Text").GetComponent<Text>().text = playerInfo.Money.ToString();
        });
    }
}
