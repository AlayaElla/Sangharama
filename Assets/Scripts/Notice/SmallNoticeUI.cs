using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class SmallNoticeUI : MonoBehaviour {

    private GameObject smallNoticeObject;
    private GameObject smallNoticeList;
    float duratrionTime = 2f;
    float ActionTime = 0.25f;

	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
	}

    public SmallNoticeUI()
    {
        smallNoticeObject = Resources.Load<GameObject>("Prefab/Notice/SmallNotice");
        smallNoticeList = Resources.Load<GameObject>("Prefab/Notice/NoticeList");
    }

    public SmallNoticeUI INIT()
    {
        return this;
    }

    public void OpenNotice(string str, float durationTime,Transform t)
    {
        GameObject _notice = Instantiate(smallNoticeObject);
        Text info = _notice.transform.FindChild("Text").GetComponent<Text>();
        info.text = str;
        int line = Mathf.CeilToInt(info.preferredWidth / Screen.width);

        //查找UI画布的最顶层
        Canvas c = _notice.GetComponent<Canvas>();
        while (t != null && c == null)
        {
            c = t.gameObject.GetComponent<Canvas>();
            t = t.parent;
        }
        Transform list = c.transform.FindChild("NoticeList");
        if (list == null)
        {
            list = Instantiate(smallNoticeList).transform;
            list.name = "NoticeList";
            list.transform.SetParent(c.transform);
            list.transform.SetAsLastSibling();
        }

        _notice.transform.SetParent(list.transform);
        _notice.transform.SetAsLastSibling();

        RectTransform rect = _notice.transform as RectTransform;

        rect.sizeDelta = new Vector2(0, Screen.height / 15 * line);
        rect.localScale = new Vector3(rect.localScale.x, 0, rect.localScale.z);
        rect.localPosition = new Vector3(0, 0, 0);

        ShowNoticeAction(_notice);
    }

    void ShowNoticeAction(GameObject notice)
    {
        LeanTween.scaleY(notice, 1, ActionTime).setOnComplete(
            () => {
                LeanTween.scaleY(notice, 0, ActionTime).setDelay(duratrionTime).setDestroyOnComplete(true);
            }
            );
    }

}
