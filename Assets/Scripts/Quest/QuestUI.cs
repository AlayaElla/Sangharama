using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestUI : MonoBehaviour {

    GameObject QuestButton;
    GameObject QuestBoard;

	// Use this for initialization
	void Start () {
        QuestButton = GameObject.Find("Canvas");
        QuestBoard = GameObject.Find("Canvas");
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
