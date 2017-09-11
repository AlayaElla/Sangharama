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
        Text questnameText = Board.transform.FindChild("QuestBoard/questName/questText").GetComponent<Text>();
        Slider prograssBar = Board.transform.FindChild("QuestBoard/questName/progress").GetComponent<Slider>();
        Text prograssBarText = Board.transform.FindChild("QuestBoard/questName/progress/progressText").GetComponent<Text>();
        GameObject closebutton = Board.transform.FindChild("QuestBoard/Close").gameObject;
        Text closebuttonText = closebutton.transform.FindChild("Text").GetComponent<Text>();
        GameObject Mask = Board.transform.FindChild("Mask").gameObject;
        GameObject questBoardBG = closebutton.transform.parent.gameObject;
        //Award
        Text awardGoldNum = Board.transform.FindChild("QuestBoard/award/Gold/Num").GetComponent<Text>();
        Text awardExpNum = Board.transform.FindChild("QuestBoard/award/Exp/Num").GetComponent<Text>();
        Image AwardGoodsImage = Board.transform.FindChild("QuestBoard/award/Goods").GetComponent<Image>();
        Text AwardGoodsNum = Board.transform.FindChild("QuestBoard/award/Goods/Num").GetComponent<Text>();

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
            //设置领取
            closebutton.GetComponent<Image>().color = Color.green;
            closebuttonText.text = "领取";
            EventTriggerListener.Get(closebutton).onClick = CloseQuestBoard;
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

    public void UpdateQuestUI(int quest)
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
            AniController.Get(hint.gameObject).PlayAni(0, 3, AniController.AniType.Loop, 5);
        }
    }

    public void ShowEventHint(int sceneID, ArrayList preCheckQuest)
    {
        Transform root;
        if (sceneID == 0)
            root = GameObject.Find("Canvas/Button/").transform;
        else if (sceneID == 1)
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
            else if (sceneID == 1)
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
            else if (sceneID == 1)
                obj.transform.localPosition = new Vector2(0, 0);
            Image img = obj.AddComponent<Image>();
            obj.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0);
            img.color = Color.green;

            AniController.Get(obj.gameObject).AddSprite(hintsprites);
            AniController.Get(obj.gameObject).PlayAni(0, 3, AniController.AniType.Loop, 5);
        }
        else
        {
            Image img = tt.GetComponent<Image>();
            tt.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0);
            img.color = Color.green;

            AniController.Get(tt.gameObject).AddSprite(hintsprites);
            AniController.Get(tt.gameObject).PlayAni(0, 3, AniController.AniType.Loop, 5);
        }
    }
}
