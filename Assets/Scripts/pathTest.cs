using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class pathTest : MonoBehaviour {

    public Transform pathpoint;

	public Transform[] trans;

    LTBezierPath bcr;
    LTSpline cr;

    public bool showBezier = true;
    public bool showspline = true;

	void OnEnable(){
		// create the path

		Vector3[] points=new Vector3[trans.Length];
		for(int i=0;i<trans.Length;i++)
		{
	        points[i]=trans[i].position;
		}
        bcr = new LTBezierPath(points);
        cr = new LTSpline(points);
	}

	void Start () {

	}
	
	void Update () {

	}


	void OnDrawGizmos(){

		if(trans!=null)
			OnEnable();
		Gizmos.color = Color.red;
        if (cr != null && showspline)
        {
            cr.gizmoDraw(); // To Visualize the path, use this method
        }

        if (bcr != null && showBezier)
        {
            bcr.gizmoDraw(); // To Visualize the path, use this method
        }
        //DrawLine();
    }

    void DrawLine()
    {
        if (cr == null || bcr == null)
        {
            Debug.Log("dont have points");
            return;
        }

        if (showspline)
        {
            if (pathpoint.childCount <= cr.ptsAdj.Length / 100)
            {
                for (int i = 0; i < cr.ptsAdj.Length; i += 100)
                {
                    Transform t = Instantiate(pathpoint);
                    t.position = cr.ptsAdj[i];
                    t.SetParent(pathpoint);
                }
            }
        }
        else
        {
            if (pathpoint.childCount != 0)
            {
                foreach (Transform _t in pathpoint)
                {
                    Destroy(_t.gameObject);
                }
            }
        }

        if (showBezier)
        {

        }
        else
        {

        }

    }
}
