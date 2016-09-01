using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;
using System;

public class Grid : MonoBehaviour {

    public float width = 3.20f;
    public float height = 3.20f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnDrawGizmos()
    {
        Vector3 pos = this.transform.localPosition;

        RectTransform rect = this.transform as RectTransform;

        int count = 0;
        for (float y = pos.y + rect.sizeDelta.y / 2; y > pos.y - rect.sizeDelta.y / 2 || count > 100; y -= height)
        {

            Gizmos.DrawLine(new Vector3(-100, y, 0.0f),

            new Vector3(100, y, 0.0f));

            count++;
        }

        //for (float x = pos.x - 1200.0f; x < pos.x + 1200.0f; x += width)
        //{

        //    Gizmos.DrawLine(new Vector3(Mathf.Floor(x / width) * width, -1000, 0.0f),

        //    new Vector3(Mathf.Floor(x / width) * width, 1000000.0f, 0.0f));

        //}

    }
}
