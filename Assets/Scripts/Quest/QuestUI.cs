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

        iconImage.sprite = Materiral.GetIconByName(quest.Smallicon);
        prograssBar.value = 0;

        EventTriggerListener.Get(questButton).onClick = OpenQuestBoard;
    }

    void OpenQuestBoard(GameObject go)
    {
        QuestManager.QuestBase quest = questManager.GetQuestInfoByID(int.Parse(go.name));

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
        GameObject Mask = Board.transform.FindChild("Mask").gameObject;

        //更新参数
        iconImage.sprite = Materiral.GetIconByName(quest.Bigicon);
        questinfoText.text = quest.des;
        questnameText.text = quest.name;
        prograssBar.value = questManager.GetQuestProgress(quest.ID) / quest.QuestComplete.Num;
        prograssBarText.text = questManager.GetQuestProgress(quest.ID) + "/" + quest.QuestComplete.Num;

        //关闭按钮
        EventTriggerListener.Get(closebutton).onClick = CloseQuestBoard;
    }

    void CloseQuestBoard(GameObject go)
    {
        Transform board = go.transform.parent.parent;
        Destroy(board.gameObject);
    }

}
