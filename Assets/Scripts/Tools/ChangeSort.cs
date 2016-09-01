using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class ChangeSort : MonoBehaviour {


    public string sortingLayerName = "FrontInfo";
    public int sortingOrder = 2;

    Renderer[] _renderers;
	// Use this for initialization
	void Start () {
	    _renderers = GetComponentsInChildren<Renderer>();

	}

    void Update()
    {
        foreach (Renderer r in _renderers)
        {
            r.sortingLayerName = sortingLayerName;
            r.sortingOrder = sortingOrder;
        }
    }
}
