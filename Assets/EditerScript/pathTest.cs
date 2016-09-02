using UnityEngine;
using System.Collections;
using UnityEditor;

public class pathTest : MonoBehaviour
{

    public Transform pathpoint;
	public Transform root;
    Transform[] trans;
    LTSpline cr;
    public int Accuracy = 100;

    public bool showspline = true;

	void OnEnable(){
		// create the path
        for (int i = 0; i < root.childCount; i++)
        {
            trans[i] = root.GetChild(i);
        }

		Vector3[] points=new Vector3[trans.Length];
		for(int i=0;i<trans.Length;i++)
		{
	        points[i]=trans[i].position;
		}

        cr = new LTSpline(points);
	}

	void OnDrawGizmos(){
        trans = new Transform[root.childCount];
		if(trans!=null)
			OnEnable();
		Gizmos.color = Color.red;
        if (cr != null && showspline)
        {
            cr.gizmoDraw(); // To Visualize the path, use this method
            DrawLine();
        }
    }

    void DrawLine()
    {
        if (transform.Find("/pathLine") != null)
        {
            Tools.DestroyImmediate(transform.Find("/pathLine").gameObject);
        }

        GameObject pathLine = new GameObject();
        pathLine.name = "pathLine";

        for (int i = 0; i < cr.ptsAdjLength; i += Accuracy)
        {
            Transform t = Tools.Instantiate(pathpoint);
            t.position = cr.ptsAdj[i];
            t.SetParent(pathLine.transform);
        }
    }

    [ContextMenu("CleanLine")]
    void CleanLine()
    {
        Tools.DestroyImmediate(transform.Find("/pathLine").gameObject);
    }

}
