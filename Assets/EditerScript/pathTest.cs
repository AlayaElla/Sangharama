using UnityEngine;
using System.Collections;
using UnityEditor;

public class pathTest : MonoBehaviour
{

    public Transform pathpoint;
	public Transform root;
    public Transform Listparent;
    public string idstr = "1_";
    Transform[] trans;
    LTSpline cr;
    public int Accuracy = 100;

    public bool showspline = true;
    public bool isDraw = true;

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
        }
        if (isDraw)
            DrawLine();
    }

    void DrawLine()
    {
        CleanLine();

        GameObject pathLine = new GameObject();
        pathLine.name = "pathLine" + idstr;
        pathLine.transform.SetParent(Listparent);

        for (int i = 0; i < cr.ptsAdjLength; i += Accuracy)
        {
            Transform t = Tools.Instantiate(pathpoint);
            t.position = cr.ptsAdj[i];
            t.SetParent(pathLine.transform);
            t.name = idstr + (pathLine.transform.childCount).ToString();

            if (i + Accuracy < cr.ptsAdjLength)
            {
                Vector3 v3Dir = cr.ptsAdj[i + Accuracy] - t.position;
                float angle = Mathf.Atan2(v3Dir.y, v3Dir.x) * Mathf.Rad2Deg;
                t.eulerAngles = new Vector3(0, 0, angle);
            }
            else
            {
                Vector3 v3Dir = cr.ptsAdj[cr.ptsAdjLength - 1] - t.position;
                float angle = Mathf.Atan2(v3Dir.y, v3Dir.x) * Mathf.Rad2Deg;
                t.eulerAngles = new Vector3(0, 0, angle);
            }
        }
    }

    [ContextMenu("CleanLine")]
    void CleanLine()
    {
        if (transform.Find("/Canvas/Scroll View/Viewport/Content/map/pathLine" + idstr) != null)
            Tools.DestroyImmediate(transform.Find("/Canvas/Scroll View/Viewport/Content/map/pathLine" + idstr).gameObject);
    }

    [ContextMenu("ShowEditorPoint")]
    void ShowEditorPoint()
    {
        for (int i = 0; i < root.childCount; i++)
        {
            string name = root.GetChild(i).name;
            if (name == "start" || name == "end" || name.Contains("point"))
            {

                root.GetChild(i).gameObject.SetActive(!root.GetChild(i).gameObject.activeSelf);
            }
        }
    }
}
