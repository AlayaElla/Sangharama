using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestUI : MonoBehaviour {

    GameObject QuestListUI;
    GameObject QuestButton;
    GameObject QuestBoard;

	// Use this for initialization
	void Start () {
        QuestListUI = GameObject.Find("ToolsKit/Canvas/QuestlLst/Viewport/Content");
        QuestButton = Resources.Load<GameObject>("Prefab/Quest/quest");
        QuestBoard = Resources.Load<GameObject>("Prefab/Quest/QuestBoardLayer");
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void AddQustUI(QuestManager.QuestBase quest)
    {
        GameObject questButton = Instantiate(QuestButton);
        questButton.transform.SetParent(QuestListUI.transform, false);
    }
}
