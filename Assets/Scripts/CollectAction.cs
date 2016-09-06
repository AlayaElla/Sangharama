using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class CollectAction : MonoBehaviour {

    /////////////////
    //UI相关
    GameObject iteminfo;

    //素材按钮
    GameObject bt_materiral;
    //素材按钮框
    RectTransform MateriralList;

    string actionRoot = "Canvas/Scroll View/Viewport/Content/map/";

    //////////////////////
    //逻辑相关
    //道具掉落的基本类型，用CollectionList来保存
    public struct CollectionMap
    {
        public int Map; //地图id
        public int MateriralType;   //type  0:item;1:mind
        public int ID;  //材料id，对应种类
        public int Weight;  //随机权重
        public int PropertyProbability;     //出属性的概率
        public int[] RandomQuality;     //随机品质,Min&Max
        public int[,] RandomProperty;    //随机性质ID,性质id和权重
    }
    static ArrayList CollectionList = new ArrayList();

    void Awake()
    {
        bt_materiral = Resources.Load<GameObject>("Prefab/materiral");
        iteminfo = Resources.Load<GameObject>("Prefab/ItemInfoUI");
        MateriralList = GameObject.Find("Canvas/MateriralList").GetComponent<RectTransform>();
    }

	// Use this for initialization
	void Start () {
        //获取掉落配置表
        XmlTool xt = new XmlTool();
        CollectionList = xt.loadCollectionXmlToArray();

        Transform root = GameObject.Find(actionRoot).transform;
        int childscount = root.childCount;

        for (int index = 0; index < childscount; index++)
        {
            if (root.GetChild(index).name.StartsWith("action"))
            {
                Transform actionroot = root.GetChild(index);
                int actioncount = actionroot.childCount;
                for (int i = 1; i < actioncount; i++)
                {
                    //TODO：判断字符串是否是数字，如果是则添加按钮相应。
                    //actionroot.GetChild(i).name
                    //EventTriggerListener.Get(GameObject.Find(actionRoot + i.ToString())).onClick = CollectionAction;
                }
            }
        }
	}
	
	// Update is called once per frame
	void Update () {

        //if(Input.GetMouseButtonDown(0)) Debug.Log(Input.mousePosition + "    " + Camera.main.ScreenToWorldPoint(Input.mousePosition));
	}

    //通过进入地图的id筛选出道具，然后通过权重选出拾取的东西。
    //如果以后会有专门进入地图的设定的话，则可以吧判断地图id筛选道具列表的方法单独移出。
    void CollectionAction(GameObject go)
    {
        //限制点击事件的时间间隔
        if (!canClick)
            return;
        StartCoroutine(updateTime(0.15f));


        //筛选出可以掉落的道具
        int mapid = int.Parse(go.name);
        ArrayList List = new ArrayList();
        
        int max = 0;
        foreach (CollectionMap materiral in CollectionList)
        {
            if (materiral.Map == mapid)
            {
                List.Add(materiral);
                //获取随机范围
                max += materiral.Weight; 
            }
        }

        //随机权重,选出拾取哪个道具
        CharBag.Goods dropMa = new CharBag.Goods();
        dropMa.Property = new int[4];

        int point = Random.Range(1, max);
        int check = 0;
        foreach (CollectionMap materiral in List)
        {
            check += materiral.Weight;

            if (check >= point)
            {
                //指定掉落的材料
                dropMa.MateriralType = materiral.MateriralType;
                
                dropMa.ID = materiral.ID;
                //指定数量，后期会修改为需要指定数量
                dropMa.Number = 1;

                //随机品质
                dropMa.Quality = Random.Range(materiral.RandomQuality[0], materiral.RandomQuality[1]);

                //随机属性;随机4个属性
                int random_max = 0;
                int propcount = 0;
                for (int i = 0; i < materiral.RandomProperty.Length / 2; i++)
                {
                    random_max += materiral.RandomProperty[i, 1];
                }

                for (int index = 0; index < 4; index++)
                {
                    if (materiral.PropertyProbability < Random.Range(1, 100))
                        continue;
                    
                    int _point = Random.Range(1, random_max);
                    int _check = 0;
                    //指定随机属性
                    for (int i = 0; i < materiral.RandomProperty.Length / 2; i++)
                    {
                        _check += materiral.RandomProperty[i, 1];
                        if (_check >= _point)
                        {
                            dropMa.Property[propcount] = materiral.RandomProperty[i, 0];
                            propcount++;
                            break;
                        }
                    }
                }
                break;
            }//endif
        }//endforech

        //获取名称
        if (dropMa.MateriralType == 0)
        {
            Materiral.Items dorp_item = Materiral.FindItemByID(dropMa.ID);
            dropMa.Name = dorp_item.Name;
            //设定基础价格，之后还需要计算
            dropMa.Price = dorp_item.Price;

            //获取type
            dropMa.Type = Materiral.GetTypeByMaterialID(dropMa.MateriralType, dropMa.ID);

        }   //item
        else
        {
            Materiral.Minds drop_mind = Materiral.FindMindByID(dropMa.ID);
            dropMa.Name = drop_mind.Name;
            //设定基础价格，之后还需要计算
            dropMa.Price = drop_mind.Price;

            //获取type
            dropMa.Type = Materiral.GetTypeByMaterialID(dropMa.MateriralType, dropMa.ID);
        }   //mind

        //计算价格
        dropMa = CharBag.SetPrice(dropMa);
        //添加物品
        dropMa.UID = CharBag.AddGoods(dropMa);

        ShowMateriralInList(dropMa);
    }



    /////////////////
    //UI相关操作
    //UI按钮限制时间点击的方法
    float limit_time = 0.1f;
    bool canClick = true;
    private IEnumerator updateTime(float time)
    {
        while (true)
        {
            limit_time -= Time.deltaTime;

            if (limit_time > 0f)
            {
                canClick = false;
            }
            else
            {
                limit_time = time;
                canClick = true;
                break;
            }
            yield return null;
        }
    }

    void ShowMateriralInList(CharBag.Goods goods)
    {
        float listHeight = MateriralList.FindChild("ListBox").GetComponent<RectTransform>().rect.height;
        GameObject materiral = Instantiate(bt_materiral);

        //获取materiral的颜色
        Color _color_image = materiral.GetComponent<Image>().color;
        Color _color_text = materiral.transform.FindChild("Text").GetComponent<Text>().color;
        Color from = new Color(0, 0, 0, 0);

        materiral.GetComponent<Image>().color = from;
        materiral.transform.FindChild("Text").GetComponent<Text>().color = from;

        //获取materiral的大小
        RectTransform rec_mat = bt_materiral.GetComponent<RectTransform>(); 

        //添加进入列表中
        materiral.transform.SetParent(MateriralList.FindChild("ListBox"));
        //materiral.transform.SetAsFirstSibling();

        //如果显示数目超过顶部，则对齐顶部

        if (MateriralList.FindChild("ListBox").transform.childCount * rec_mat.rect.height > listHeight)
        {
            GameObject materiallist = MateriralList.FindChild("ListBox").gameObject;
            LeanTween.moveLocalY(materiallist, MateriralList.FindChild("ListBox").transform.localPosition.y - rec_mat.rect.height, 0.15f);
        }


        float dis = MateriralList.FindChild("ListBox").GetComponent<RectTransform>().rect.height / 2;    //dis为中心点;抵消local的偏差，使用local0点为中心点
        float button_pos = -dis + (rec_mat.rect.height / 2) + (MateriralList.FindChild("ListBox").transform.childCount - 1) * rec_mat.rect.height;

        materiral.GetComponent<RectTransform>().localPosition = new Vector3(0, dis + button_pos);

        //由上落下的效果
        LeanTween.moveLocalY(materiral, button_pos, 0.5f).setEase(LeanTweenType.easeOutBounce);

        //渐现效果
        LeanTween.value(materiral, from, _color_image, 0.25f).setOnUpdate(
            (Color col) =>
            {
                materiral.GetComponent<Image>().color = col;
            }
            );
        LeanTween.value(materiral, from, _color_text, 0.25f).setOnUpdate(
            (Color col) =>
            {
                materiral.transform.FindChild("Text").GetComponent<Text>().color = col;
            }
            );

        //显示名称
        materiral.transform.FindChild("Text").GetComponent<Text>().text = goods.Name;
        materiral.transform.FindChild("Image").GetComponent<Image>().sprite = Materiral.GetMaterialIcon(goods.MateriralType, goods.ID);

        EventTriggerListener.Get(materiral).parameter = goods;
        EventTriggerListener.Get(materiral).onClickByParameter = ShowMateriralInfo;
    }

    void MaterialListToTop()
    {
        //找到最后一个按钮的rect，用来确定最高的位置，高度需要用世界坐标来确认
        RectTransform MateriralRect = MateriralList.FindChild("ListBox").GetChild(MateriralList.FindChild("ListBox").childCount - 1).transform.GetComponent<RectTransform>();

        //获取目标位置
        float movetarget = MateriralList.position.y + MateriralList.GetComponent<RectTransform>().rect.height / 2 - MateriralRect.rect.height / 2;
        //获取移动距离
        float movedis = movetarget - MateriralRect.position.y;

        //Debug.Log(MateriralRect + "  " + MateriralRect.anchoredPosition.y + "  " + movedis);

        Debug.Log(MateriralList.transform.position + "   material:" + MateriralRect.position + "   movetarget:" + movetarget + "   movedis:" + movedis);

        GameObject material = MateriralList.FindChild("ListBox").gameObject;

        LeanTween.moveY(material, material.transform.position.y + movedis, 0.25f);
        //Debug.Log("Move!");
        //for (int i = 0; i < MateriralList.childCount; i++)
        //{
        //    GameObject material = MateriralList.GetChild(i).gameObject;
        //    LeanTween.moveLocalY(material, material.transform.localPosition.y - movedis, 0.25f);
        //    Debug.Log("Move!");
        //}
        
    }

    void ShowMateriralInfo(GameObject go, object parameter)
    {
        CharBag.Goods goods = (CharBag.Goods)parameter;

        ItemInfoUI info = Instantiate(iteminfo).GetComponent<ItemInfoUI>();
        info.Ini();
        info.OpenItemInfo(goods);
    }

    //判断是字符串是否是数字
    void isNumber(string str)
    {
 
    }
}
