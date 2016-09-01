using UnityEngine;
using System.Collections;

public class pathTest : MonoBehaviour {



    public Vector3 piont1;
    public Vector3 piont2;
    public Vector3 piont3;
    public Vector3 piont4;


	// Use this for initialization
	void Start () {
        LeanTween.drawBezierPath(piont1, piont2, piont3, piont4);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
