using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickStory : MonoBehaviour {


    ChatEventManager chatEventManager;
    // Use this for initialization
    void Start () {
        chatEventManager = transform.Find("/ToolsKit/EventManager").GetComponent<ChatEventManager>();
        chatEventManager.StartStory("Start");
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
