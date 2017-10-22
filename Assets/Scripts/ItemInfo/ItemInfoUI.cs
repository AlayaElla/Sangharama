using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ItemInfoUI : MonoBehaviour {

    public GameObject infoPrefab;

    //按钮
    public GameObject btn_close;
    public GameObject btn_discard;

    //属性相关UI
    public GameObject InfoPlane;
    public GameObject ScrollPages;
    
    private struct GoodsBase
    {
        //Page1
        public Text nameText;
        public Image goodsIMG;
        public Text MaterialtypeText;
        public Text numText;
        public Text qualityText;
        public Text priceText;

        public Text effect;
        public GameObject[] propertys;

        //Page2
        public Text typeText;
        public Text des;
    }
    GoodsBase goodsUIBase;

    //数据相关
    Materiral.Effect effect;
    ArrayList Propertys;


    // Use this for initialization
    void Start()
    {
        RectTransform infoRect = this.GetComponent<RectTransform>();

        infoRect.SetParent(GameObject.Find("Canvas").transform, false);
        infoRect.SetAsLastSibling();

        infoRect.sizeDelta = new Vector2(0, 0);
        infoRect.localPosition = new Vector3(0, 0, 0);
        LeanTween.scale(infoRect.gameObject, new Vector3(1.05f, 1.05f, 1.05f), 0.125f).setEase(LeanTweenType.easeOutQuad).setLoopPingPong(1);
    }

    // Update is called once per frame
    void Update()
    {
    }

    //public ItemInfoUI()
    //{
    //    GameObject info = Instantiate(infoPrefab);

    //}

    public void Ini()
    {
        EventTriggerListener.Get(btn_close).onClick = CloseItemInfo;

        //基本属性
        goodsUIBase = new GoodsBase();
        goodsUIBase.nameText = InfoPlane.transform.Find("Title/Text").GetComponent<Text>();
        goodsUIBase.goodsIMG = InfoPlane.transform.Find("IMG/Image").GetComponent<Image>();
        goodsUIBase.MaterialtypeText = InfoPlane.transform.Find("TextInfo/MaterialType").GetComponent<Text>();
        goodsUIBase.numText = InfoPlane.transform.Find("TextInfo/Num").GetComponent<Text>();
        goodsUIBase.qualityText = InfoPlane.transform.Find("TextInfo/Quality").GetComponent<Text>();
        goodsUIBase.priceText = InfoPlane.transform.Find("TextInfo/Price").GetComponent<Text>();

        //物品效果
        goodsUIBase.effect = ScrollPages.transform.Find("PageList/Page1/Effect/EffectName/Text").GetComponent<Text>();

        //物品属性
        goodsUIBase.propertys = new GameObject[4];
        for (int i = 0; i < 4; i++)
        {
            goodsUIBase.propertys[i] = ScrollPages.transform.Find("PageList/Page1/Property/List").GetChild(i).gameObject;
        }

        //物品类型
        goodsUIBase.typeText = ScrollPages.transform.Find("PageList/Page2/Type/TypeList/Text").GetComponent<Text>();

        //物品描述
        goodsUIBase.des = ScrollPages.transform.Find("PageList/Page2/Info/TextBox/Text").GetComponent<Text>();

        //初始化
        effect = new Materiral.Effect();
        Propertys = new ArrayList();


        //按钮效果
        EventTriggerListener.Get(btn_close).onClick = CloseItemInfo;
    }

    public void OpenItemInfo(CharBag.Goods good)
    {
        //名称
        goodsUIBase.nameText.text = good.Name;

        //图片icon
        goodsUIBase.goodsIMG.sprite = Materiral.GetMaterialIcon(good.MateriralType, good.ID);

        //物品类型
        if (good.MateriralType == 0)
        {
            goodsUIBase.MaterialtypeText.text = "类型:物品";
        }
        else if (good.MateriralType == 1)
        {
            goodsUIBase.MaterialtypeText.text = "类型:概念";
        }
        else
        {
            goodsUIBase.MaterialtypeText.text = "类型:未知";
        }

        //数量
        goodsUIBase.numText.text = "数量:" + good.Number.ToString();

        //品质
        goodsUIBase.qualityText.text = "品质:" + good.Quality.ToString();

        //价格
        goodsUIBase.priceText.text = "价格:" + good.Price.ToString();

        //物品效果
        if (good.MaterialEffet != 0)
        {
            Materiral.Effect _effect = Materiral.FindMaterialEffectByID(good.MaterialEffet);
            goodsUIBase.effect.text = _effect.Name;

            effect = _effect;
        }

        //物品属性
        if (good.Property != null)
        {
            for (int i = 0; i < good.Property.Length; i++)
            {
                if (good.Property[i] == 0)
                    break;

                Materiral.Property _p = Materiral.GetProNameByProID(good.Property[i]);
                GameObject p_obj = goodsUIBase.propertys[i];
                p_obj.SetActive(true);
                p_obj.transform.GetChild(0).GetComponent<Text>().text = _p.Name;
                p_obj.transform.GetChild(1).GetComponent<Image>().sprite = Materiral.GetPropertyIcon(_p.ID);

                Propertys.Add(_p);
            }
        }

        //类型
        goodsUIBase.typeText.text = Materiral.FindTypeNameByID(good.Type).Name;

        string str = Materiral.GetDesByMaterialID(good.MateriralType, good.ID);
        
        //描述
        goodsUIBase.des.text = System.Text.RegularExpressions.Regex.Unescape(str);
    }

    public void CloseItemInfo(GameObject obj)
    {
        Destroy(gameObject);
    }
}
