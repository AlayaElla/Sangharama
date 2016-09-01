using UnityEngine;
using System.Collections;

public class ani : MonoBehaviour {

	// Use this for initialization
	void Start () {

        //Vector3 _nv = this.transform.position;
        //Vector3[] _path = new Vector3[] { new Vector3(_nv.x, _nv.y, _nv.z), new Vector3(_nv.x + 1, _nv.y + 1, _nv.z) };
        //LeanTween.moveSpline(this.gameObject, _path, 1.5f).setEase(LeanTweenType.easeOutQuad);
	}
	
	// Update is called once per frame
	void Update () {

	}

    public Vector3 startpos;

    public void creatIcon()
    {

        int count = Random.Range(19, 20);

        for (int i = 1; i <= count; i++)
        {
            GameObject obj = new GameObject();
            SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
            sr.sprite = Materiral.GetIconByName("actionIcon_6");

            Vector3 endpos = new Vector3(startpos.x + Random.Range(-0.5f, 0.5f), startpos.y + Random.Range(-0.05f, 0.05f), 0);
            
            float randomheight = Random.Range(0.3f, 0.6f);
            float time = Mathf.Abs(randomheight);
            Debug.Log(randomheight);
            Vector3[] _path = new Vector3[] { startpos, startpos, new Vector3(endpos.x / 2, randomheight + startpos.y, startpos.z), endpos, endpos };
            LeanTween.moveSpline(obj, _path, time).setOnComplete(() => {
                LeanTween.moveY(obj, randomheight / 5 + startpos.y, time / 5).setOnComplete(() => {
                    LeanTween.moveY(obj, endpos.y, time / 5).setEase(LeanTweenType.easeOutBounce);
                });
            });
        }
    }
}
