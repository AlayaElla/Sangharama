using UnityEngine;
using System.Collections;

public class Hint : MonoBehaviour {

    SpriteRenderer hintsp;
    bool ishint = false;

	// Use this for initialization
	void Start () {
        hintsp = transform.FindChild("hint").GetComponent<SpriteRenderer>();
        hintsp.color = new Color(hintsp.color.r, hintsp.color.g, hintsp.color.b, 0);
	}

	// Update is called once per frame
	void Update () {
        if (ishint)
        {
            float alpha = Mathf.Lerp(hintsp.color.a, 0, 3 * Time.deltaTime);
            hintsp.color = new Color(hintsp.color.r, hintsp.color.g, hintsp.color.b, alpha);

            if (alpha <= 0.01f)
            {
                ishint = false;
            }
        }
	}
    
    void OnMouseOver()
    {
        ishint = true;
        hintsp.color = new Color(hintsp.color.r, hintsp.color.g, hintsp.color.b, 150/255f);
    }

}
