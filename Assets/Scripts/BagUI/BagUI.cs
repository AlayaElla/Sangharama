using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class BagUI : MonoBehaviour {

    public GameObject BagMenuUI;
    public GameObject shopGoods;

    //物品详情prefab
    GameObject iteminfo;


    //自动调整大小
    private ContentSizeFitter _fitter;
    //背包列表
    private GameObject scrollList;

    //button
    public GameObject msk_Close;
    public GameObject btn_menu;


    //baglistBase
    ArrayList _bagList;
    ArrayList f_BagList;

    void Start()
    {
        this.transform.SetParent(GameObject.Find("Canvas").transform);
        this.transform.SetAsLastSibling();

        RectTransform infoRect = this.GetComponent<RectTransform>();

        infoRect.sizeDelta = new Vector2(0, 0);
        infoRect.localPosition = new Vector3(0, 0, 0);
    }


	// Use this for initialization
    public void Initialize(int type)
    {
        scrollList = BagMenuUI.transform.Find("ScrollList/BagList").gameObject;
        _fitter = scrollList.transform.Find("GridLayoutPanel").GetComponent<ContentSizeFitter>();
        iteminfo = Resources.Load<GameObject>("Prefab/ItemInfoUI");
        shopGoods = transform.Find("ShopGoods").gameObject;

        //获取背包数据
        _bagList = CharBag.GetGoodsList();
        f_BagList = new ArrayList();
        //设置关闭按钮
        if (type == 1)
        {
            EventTriggerListener.Get(msk_Close).onClick = CloseBagMenu;
        }
        else
        {
            EventTriggerListener.Get(msk_Close).onClick = CloseBagMenuInShop;
            shopGoods.SetActive(true);
        }

        this.transform.SetParent(GameObject.Find("Canvas").transform);
        this.transform.SetAsLastSibling();
	}
	
	// Update is called once per frame
	void Update () {
	
	}


    public void OpenBagInRecipeSlots(object parameter)
    {
        SetRecipeNameFilter(parameter);
    }

    public void OpenBagMenuInShop(object parameter)
    {
        SetRecipeNameInShop(parameter);
    }


    public void CloseBagMenu(GameObject go)
    {

        Destroy(this.gameObject);
    }

    public void CloseBagMenuInShop(GameObject go)
    {

        Destroy(this.gameObject);
        
        ShopUI.ChangeRecipeUiState();
    }

    void SetRecipeNameInShop(object parameter)
    {
        Parameter.Box p = (Parameter.Box)parameter;
        //显示当前上架的物品
        if (p.obj!=null)
        {
            CharBag.Goods currentGoods = (CharBag.Goods)p.obj;

            Image _image = shopGoods.transform.Find("Image").GetComponent<Image>();
            Text _text = shopGoods.transform.Find("Text").GetComponent<Text>();
            Button _button = shopGoods.transform.Find("Button").GetComponent<Button>();

            _text.text = "当前上架的商品:";
            _text.transform.localPosition = new Vector3(_text.transform.position.x, 50, _text.transform.position.z);
            _image.gameObject.SetActive(true);
            _image.sprite = Materiral.GetMaterialIcon(currentGoods.MateriralType, currentGoods.ID);
            EventTriggerListener.Get(_image.gameObject).parameter = p;
            EventTriggerListener.Get(_image.gameObject).onClickByParameter = ShowMateriralInfo;

            _button.gameObject.SetActive(true);
            EventTriggerListener.Get(_button.gameObject).parameter = p;
            EventTriggerListener.Get(_button.gameObject).onClickByParameter = p.callbackByEvent;
        }

        //创建物品列表
        for (int i = 0; i < _bagList.Count; i++)
        {
            CharBag.Goods _map = (CharBag.Goods)_bagList[i];
            
            //筛选物品，特殊物品不显示
            if (_map.MateriralType > 1)
                continue;

            GameObject button = Instantiate(btn_menu);
            button.transform.SetParent(_fitter.transform);

            button.name = _map.ID.ToString();
            button.transform.Find("Text").GetComponent<Text>().text = _map.Name;
            button.transform.Find("Image").GetComponent<Image>().sprite = Materiral.GetMaterialIcon(_map.MateriralType, _map.ID);

            //显示数量
            button.transform.Find("num").gameObject.SetActive(true);
            button.transform.Find("num/Text").GetComponent<Text>().text = _map.Number.ToString();

            //显示价格
            button.transform.Find("PriceBoard").gameObject.SetActive(true);
            button.transform.Find("PriceBoard/Text").GetComponent<Text>().text = _map.Price.ToString();

            //设置参数容器中的参数
            Parameter.Box _p = new Parameter.Box();
            _p.ID = p.ID;
            _p.obj = _map;

            //设置点击事件
            OnClickInScorll.Get(button.transform).parameter = _p;
            OnClickInScorll.Get(button.transform).onClickByParameter = p.callback;
            OnClickInScorll.Get(button.transform).onHoldByParameter = ShowMateriralInfo;

            //添加背包进入筛选背包列表
            f_BagList.Add(_map);

        }

        //调整列表为置顶
        if (f_BagList.Count > 5)
        {
            setGird();
        }

        StartCoroutine(SetListToTop());
    }

    //TODO:add type mode
    void SetRecipeNameFilter(object parameter)
    {
        Parameter.Box p = (Parameter.Box)parameter;
        Recipe.Slot slot = (Recipe.Slot)p.obj;
        Dictionary<int, RecipeUI.SlotBox> slotList = (Dictionary<int, RecipeUI.SlotBox>)p.subobj;

        for (int i = 0; i < _bagList.Count; i++)
        {
            CharBag.Goods _map = new CharBag.Goods();
            CharBag.Goods m_map = (CharBag.Goods)_bagList[i];

            ////////筛选
            if (slot.SlotType == Recipe.SlotTypeList.Material)
            {
                //固定材料
                if (slot.MatType == 0 && slot.MatType == m_map.MateriralType)  //Item
                {
                    if (m_map.ID == slot.MatId)
                        _map = m_map;
                }
                else if (slot.MatType == 1 && slot.MatType == m_map.MateriralType)  //Mind
                {
                    if (m_map.ID == slot.MatId)
                        _map = m_map;
                }
            }
            else if (slot.SlotType == Recipe.SlotTypeList.MaterialType)
            {
                if (m_map.Type == slot.MatType)
                    _map = m_map;
            }

            //添加进入背包列表，并且创建按钮
            if (_map.Name != null)
            {
                //添加背包进入筛选背包列表
                f_BagList.Add(_map);

                //创建按钮
                GameObject button = Instantiate(btn_menu);
                button.transform.SetParent(_fitter.transform);

                button.name = _map.ID.ToString();
                button.transform.Find("Text").GetComponent<Text>().text = _map.Name;
                button.transform.Find("Image").GetComponent<Image>().sprite = Materiral.GetMaterialIcon(_map.MateriralType, _map.ID);
                
                //显示数量
                button.transform.Find("num").gameObject.SetActive(true);
;               button.transform.Find("num/Text").GetComponent<Text>().text = _map.Number.ToString();

                //设置参数容器中的参数
                Parameter.Box _p = new Parameter.Box();
                _p.ID = p.ID;
                _p.obj = _map;

                //设置点击事件
                OnClickInScorll.Get(button.transform).parameter = _p;
                OnClickInScorll.Get(button.transform).onClickByParameter = p.callback;
                OnClickInScorll.Get(button.transform).onHoldByParameter = ShowMateriralInfo;


                //如果是已经的选中的则不能点击
                foreach (RecipeUI.SlotBox _slot in slotList.Values)
                {
                    if (_map.UID == _slot.slot.UID)
                    {
                        button.name = _map.ID.ToString();
                        button.transform.Find("Text").GetComponent<Text>().text = _map.Name + " <color=red>(E)</color>";
                        button.transform.GetComponent<Image>().color = Color.gray;

                        OnClickInScorll.Get(button.transform).onClickByParameter = null;
                    }
                }
            }
        }
        //调整列表为置顶
        if (f_BagList.Count > 5)
        {
            setGird();
        }

        StartCoroutine(SetListToTop());
    }

    public void SetGoodsName(int MaterialType)
    {
        //创建物品列表
        for (int i = 0; i < _bagList.Count; i++)
        {
            CharBag.Goods _map = (CharBag.Goods)_bagList[i];

            //筛选物品，特殊物品不显示
            if (MaterialType != -1)
            {
                if (_map.MateriralType != MaterialType)
                    continue;
                if (_map.MateriralType == 2 && _map.Type == 0)
                    continue;
            }

            GameObject button = Instantiate(btn_menu);
            button.transform.SetParent(_fitter.transform);

            button.name = _map.ID.ToString();
            button.transform.Find("Text").GetComponent<Text>().text = _map.Name;
            button.transform.Find("Image").GetComponent<Image>().sprite = Materiral.GetMaterialIcon(_map.MateriralType, _map.ID);

            //显示数量
            button.transform.Find("num").gameObject.SetActive(true);
            button.transform.Find("num/Text").GetComponent<Text>().text = _map.Number.ToString();

            //显示价格
            button.transform.Find("PriceBoard").gameObject.SetActive(true);
            button.transform.Find("PriceBoard/Text").GetComponent<Text>().text = _map.Price.ToString();

            //设置参数容器中的参数
            Parameter.Box _p = new Parameter.Box();
            _p.obj = _map;

            //设置点击事件
            OnClickInScorll.Get(button.transform).parameter = _p;
            OnClickInScorll.Get(button.transform).onHoldByParameter = ShowMateriralInfo;

            //添加背包进入筛选背包列表
            f_BagList.Add(_map);
        }

        //调整列表为置顶
        if (f_BagList.Count > 5)
        {
            setGird();
        }

        StartCoroutine(SetListToTop());
    }


    IEnumerator SetListToTop()
    {
        yield return null;
        RectTransform rect = scrollList.transform.Find("GridLayoutPanel").GetComponent<RectTransform>();

        rect.localPosition = new Vector3(rect.localPosition.x, -(rect.sizeDelta.y / 2));

    }

    void setGird()
    {
        _fitter.enabled = true;
        scrollList.GetComponent<ScrollRect>().enabled = true;
    }

    void ShowMateriralInfo(GameObject go, object parameter)
    {
        Parameter.Box _p = (Parameter.Box)parameter;
        CharBag.Goods goods = (CharBag.Goods)_p.obj;

        ItemInfoUI info = Instantiate(iteminfo).GetComponent<ItemInfoUI>();
        info.Ini();
        info.OpenItemInfo(goods);
    }

}
