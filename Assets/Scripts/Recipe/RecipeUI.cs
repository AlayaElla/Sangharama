using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RecipeUI : MonoBehaviour{

    public GameObject Plane;
    public GameObject recipeUI;
    public GameObject MenuUI;
    GameObject iteminfo;

    private ContentSizeFitter _fitter;
    private GameObject scrollList;
    private RectTransform recipeSlotsList;
    private Vector3 recipeSlotsListPos;
    //private Vector3 recipeSlotsListSize;
    
    //button
    public GameObject recipeButton;
    public Button btn_Close;
    public GameObject btn_recipeSlot;
    public Button btn_startRecipe;

    //BagUI
    GameObject _bagUI;

    BagUI _bagInstance;

    //recipeBase
    Recipe recipeMap;
    Recipe.RecipeMap recipe;

    //propertyUI
    public RectTransform PropertyListPlane;
    public GridLayoutGroup PropertyListGrid;
    public Scrollbar QualityBar;
    public Button btn_Compose;
    public GameObject ComposeMask;
    public Text QualityValue;
    public Button QualityEffect;
    public Slider QualitySkillPoint1;
    public Slider QualitySkillPoint2;
    public Slider QualitySkillPoint3;
    public GameObject PropertyListElement;
    private Vector3 PropertyListPlaneRect;

    //事件&任务管理器
    ChatEventManager eventmanager;
    QuestManager questManager;

    public struct SlotBox
    {
        public GameObject button;
        public CharBag.Goods slot;
    }
    Dictionary<int, SlotBox> SlotList;

    void Awake()
    {

    }

	// Use this for initialization
	void Start () {
        //获取事件控制器
        eventmanager = transform.Find("/ToolsKit/EventManager").GetComponent<ChatEventManager>();
        questManager = transform.Find("/ToolsKit/QuestManager").GetComponent<QuestManager>();

        iteminfo = Resources.Load<GameObject>("Prefab/ItemInfoUI");
        _bagUI = Resources.Load<GameObject>("Prefab/BagUI");
        scrollList = MenuUI.transform.Find("ScrollList/RecipeList").gameObject;
        _fitter = scrollList.transform.Find("GridLayoutPanel").GetComponent<ContentSizeFitter>();

        recipeSlotsList = recipeUI.transform.Find("SlotsList").GetComponent<RectTransform>();
        recipeSlotsListPos = recipeSlotsList.position;
        //recipeSlotsListSize = recipeSlotsList.sizeDelta;

        PropertyListPlaneRect = PropertyListPlane.position;

        EventTriggerListener.Get(GameObject.Find("Canvas/RecipeButton")).onClick = OpenRecipeMenu;

        //get recipe
        recipeMap = Recipe.Intance;
	}
	
	// Update is called once per frame
	void Update () {
	}

    void OpenRecipeMenu(GameObject go)
    {
        Plane.SetActive(true);
        MenuUI.SetActive(true);
        recipeUI.SetActive(false);

        ShopUI.ChangeRecipeUiState();

        EventTriggerListener.Get(btn_Close.gameObject).onClick = CloseRecipeMenu;
        GetRecipeList();
    }

    void CloseRecipeMenu(GameObject go)
    {
        Plane.SetActive(false);
        _fitter.enabled = false;

        ShopUI.ChangeRecipeUiState();

        scrollList.GetComponent<ScrollRect>().enabled = false;

        SlotList = null;

        foreach (Transform obj in _fitter.transform)
        {
            Destroy(obj.gameObject);
        }
    }

    void GetRecipeList()
    {

        SetRecipeName();

        if (recipeMap.ReicipeList.Count > 7)
        {
            setGird();
        }
    }


    void SetRecipeName()
    {
        for (int i = 0; i < recipeMap.ReicipeList.Count; i++)
        {
            GameObject button = Instantiate(recipeButton);
            button.transform.SetParent(_fitter.transform);

            Recipe.RecipeMap _map = (Recipe.RecipeMap)recipeMap.ReicipeList[i];

            button.name = _map.ID.ToString();
            button.transform.Find("Text").GetComponent<Text>().text = _map.Name;
            int _t = _map.Target[0].ToString() == "0" ? 0 : 1;
            int _i = int.Parse(_map.Target.Substring(_map.Target.IndexOf(",") + 1));

            button.transform.Find("Image").GetComponent<Image>().sprite = Materiral.GetMaterialIcon(_t, _i);

            //设置点击事件
            //button.GetComponent<Button>().onClick.AddListener(OpenRecipe);
            OnClickInScorll.Get(button.transform).onClick = OpenRecipe;
        }
    }

    void setGird()
    {
        _fitter.enabled = true;
        scrollList.GetComponent<ScrollRect>().enabled = true;
        scrollList.GetComponent<ScrollRect>().verticalScrollbar.value = 1;
    }


    //打开合成界面
    void OpenRecipe(GameObject go)
    {
        //获取合成配方
        recipe = recipeMap.GetRecipeByID(int.Parse(go.name));
        if (recipe.Name == null)
        {
            Debug.Log("Can't GetRecipe!");
            return;
        }

    #if _Debug  
        Debug.Log(recipe.Name);
    #endif

        //打开合成界面，隐藏合成菜单界面
        MenuUI.SetActive(false);
        recipeUI.SetActive(true);
        btn_Compose.gameObject.SetActive(false);

        SetSlot(recipe);

        btn_Close.transform.Find("Text").GetComponent<Text>().text = "返回";
        btn_startRecipe.transform.Find("Text").GetComponent<Text>().text = "合成";
        EventTriggerListener.Get(btn_Close.gameObject).onClick = CloseRecipe;

        EventTriggerListener.Get(btn_startRecipe.gameObject).onClick = StartRecipe;
    }

    //关闭合成界面
    void CloseRecipe(GameObject go)
    {

        btn_Close.transform.Find("Text").GetComponent<Text>().text = "关闭";
        EventTriggerListener.Get(btn_Close.gameObject).onClick = CloseRecipeMenu;

        LeanTween.moveY(PropertyListPlane.gameObject, PropertyListPlaneRect.y, 0.5f).setEase(LeanTweenType.easeInBack);
        LeanTween.moveY(recipeSlotsList.gameObject, recipeSlotsListPos.y, 0.5f).setEase(LeanTweenType.easeInBack);
        LeanTween.scale(recipeSlotsList.gameObject, new Vector3(1f, 1f, 1f), 0.5f).setEase(LeanTweenType.easeInBack);

        Color c = recipeSlotsList.GetComponent<Image>().color;
        recipeSlotsList.GetComponent<Image>().color = new Color(c.r, c.g,c.b, 1);

        Color m = ComposeMask.GetComponent<Image>().color;
        ComposeMask.GetComponent<Image>().color = new Color(m.r, m.g, m.b, 0.7f);

        MenuUI.SetActive(true);
        recipeUI.SetActive(false);
        ComposeMask.SetActive(false);
        btn_startRecipe.gameObject.SetActive(true);
        recipeSlotsList.gameObject.SetActive(true);

        ClearRecipeData();

        //触发合成事件
        bool ishit = false;
        ishit = eventmanager.CheckEventList(ChatEventManager.ChatEvent.EventTypeList.ComposeGoods, true) || ishit;
        if (ishit)
        {
            eventmanager.StartStory();
        }
    }

    void ClearRecipeData()
    {
        //清除数据
        InputBox = new ArrayList();
        SlotList = new Dictionary<int, SlotBox>();
        PropertyBox = new ArrayList();
        ChangeRecipeStat(RecipeStat.WaitInput);


        if (targetgoods != null)
            Destroy(targetgoods);

        for (int i = 0; i < recipeSlotsList.transform.childCount; i++)
        {
            if (recipeSlotsList.transform.GetChild(i).gameObject.name == "Compose")
                continue;
            GameObject slot = recipeSlotsList.transform.GetChild(i).gameObject;
            Destroy(slot);
        }

        foreach (Transform _p in PropertyListGrid.transform)
        {
            Destroy(_p.gameObject);
        }
    }

    //设置合成界面槽的信息和位置
    void SetSlot(Recipe.RecipeMap map)
    {
        int num = map.Slots.Length;

        //TODO:游戏道具的名称和ID，以及按钮的表现效果
        Vector3 base_point = new Vector3(0, -100, 0);
        int targetAngel = 0;

        SlotList = new Dictionary<int, SlotBox>();

        for (int i = 0; i < num; i++)
        {
            GameObject recipeSlot = Instantiate(btn_recipeSlot);
            recipeSlot.transform.SetParent(recipeSlotsList.transform);

            //设置按钮位置
            targetAngel = 360 / num * i;

            Vector3 target_pos = Quaternion.Euler(0, 0, targetAngel) * base_point;
            recipeSlot.transform.localPosition = target_pos;

            //判断slot中材料的类型，0为固定材料，1为材料种类
            if (map.Slots[i].SlotType == Recipe.SlotTypeList.Material)
            {
                //固定材料
                if (map.Slots[i].MatType == 0)  //Item
                {
                    Materiral.Items _item = Materiral.FindItemByID(map.Slots[i].MatId);

                    recipeSlot.name = i.ToString();
                    recipeSlot.transform.Find("Text").GetComponent<Text>().text = "[" + _item.Name + "]";
                    recipeSlot.transform.Find("Bottom/Image").GetComponent<Image>().sprite = Materiral.GetIconByName(_item.IMG);
                    recipeSlot.transform.Find("Bottom/Image").GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
                }
                else if (map.Slots[i].MatType == 1)  //Mind
                {
                    Materiral.Minds _mind = Materiral.FindMindByID(map.Slots[i].MatId);
                    recipeSlot.name = i.ToString();
                    recipeSlot.transform.Find("Text").GetComponent<Text>().text = "[" + _mind.Name + "]";
                    recipeSlot.transform.Find("Bottom/Image").GetComponent<Image>().sprite = Materiral.GetIconByName(_mind.IMG);
                    recipeSlot.transform.Find("Bottom/Image").GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
                }
                
            }
            else if (map.Slots[i].SlotType == Recipe.SlotTypeList.MaterialType)
            {
                //材料类型
                recipeSlot.name = i.ToString();

                int typeID = map.Slots[i].MatType;
                Materiral.MaterialType type = Materiral.FindTypeNameByID(typeID);
                recipeSlot.transform.Find("Text").GetComponent<Text>().text = "<color=red>[" + type.Name + "]\n(类型)</color>";
                recipeSlot.transform.Find("Bottom/Image").GetComponent<Image>().sprite = Materiral.GetIconByName(type.IMG);
                recipeSlot.transform.Find("Bottom/Image").GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
            }
            else
                Debug.Log("Can't find recipe slots!");

            //设置参数容器中的参数
            Parameter.Box parameter = new Parameter.Box();
            parameter.ID = i;   //ID为slot的序号
            parameter.callback = ClickInBag;
            parameter.obj = map.Slots[i];
            parameter.subobj = SlotList;

            //添加记录到合成系统用容器中
            SlotBox slot = new SlotBox();
            slot.button = recipeSlot;
            SlotList.Add(i, slot);

            //设置点击事件
            GameObject _slot = recipeSlot.transform.Find("Bottom/Image").gameObject;
            EventTriggerListener.Get(_slot).parameter = parameter;
            EventTriggerListener.Get(_slot).onClickByParameter = OpenBag;
        }
    }

    void OpenBag(GameObject go,object parameter)
    {
        _bagInstance = Instantiate(_bagUI).GetComponent<BagUI>();
        ////初始化
        _bagInstance.Initialize(1);
        _bagInstance.OpenBagInRecipeSlots(parameter);
    }

    void ClickInBag(GameObject go,object parameter)
    {
        Parameter.Box p = (Parameter.Box)parameter;
        CharBag.Goods goods = (CharBag.Goods)p.obj;

    #if _Debug  
        Debug.Log("ok!!" + "  Name:" + goods.Name + "  ID:" + p.ID);
    #endif

        SlotBox slotbox =new SlotBox();

        if (SlotList.ContainsKey(p.ID))
        {
            slotbox = SlotList[p.ID];
            slotbox.button.transform.Find("Text").GetComponent<Text>().text = goods.Name;
            slotbox.button.transform.Find("Bottom/Image").GetComponent<Image>().sprite = Materiral.GetMaterialIcon(goods.MateriralType, goods.ID);
            slotbox.button.transform.Find("Bottom/Image").GetComponent<Image>().color = new Color(1, 1, 1, 1f);

            //把物品添加到合成系统的slotlist中
            slotbox.slot = goods;
            SlotList[p.ID] = slotbox;

            //CloseBag
            _bagInstance.CloseBagMenu(gameObject);
        }
    }

    void StartRecipe(GameObject go)
    {
        //如果没有选择材料,则提示
        foreach (SlotBox slot in SlotList.Values)
        {
            if (slot.slot.Name == null)
            {
                SmallNoticeUI sNotice = gameObject.AddComponent<SmallNoticeUI>();
                sNotice = sNotice.INIT();

                string str = "请放入材料！";
                sNotice.OpenNotice(str, 2f, Plane.transform);                
                return;
            }
        }

        btn_startRecipe.gameObject.SetActive(false);

         //下拉属性界面和合成槽
        float targetPos = Screen.height - PropertyListPlane.rect.height * 0.5f;
        LeanTween.moveY(PropertyListPlane.gameObject,targetPos, 0.5f).setEase(LeanTweenType.easeOutQuad);
        targetPos = recipeSlotsList.rect.height * 0.5f;
        LeanTween.moveY(recipeSlotsList.gameObject, targetPos, 0.5f).setEase(LeanTweenType.easeOutQuad);
        LeanTween.scale(recipeSlotsList.gameObject, new Vector3(0.8f, 0.8f, 0.8f), 0.5f).setEase(LeanTweenType.easeOutQuad);

        AddFristPropertyUIEft();
        SetPropertyPlane();
    }


    void SetPropertyPlane()
    {
        Quality = 0;
        QualityBar.size = 0;
        QualityValue.text = "0";
        QualitySkillPoint1.value = (float)recipe.EffetBox.Eft1.NeedQuality / 100;
        QualitySkillPoint1.transform.Find("Handle Slide Area/Handle/Text").GetComponent<Text>().text = recipe.EffetBox.Eft1.NeedQuality.ToString();
        QualitySkillPoint2.value = (float)recipe.EffetBox.Eft2.NeedQuality / 100;
        QualitySkillPoint2.transform.Find("Handle Slide Area/Handle/Text").GetComponent<Text>().text = recipe.EffetBox.Eft2.NeedQuality.ToString();
        QualitySkillPoint3.value = (float)recipe.EffetBox.Eft3.NeedQuality / 100;
        QualitySkillPoint3.transform.Find("Handle Slide Area/Handle/Text").GetComponent<Text>().text = recipe.EffetBox.Eft3.NeedQuality.ToString();

        QualityEffect.transform.GetChild(0).GetComponent<Text>().text = "";

        foreach (SlotBox slot in SlotList.Values)
        {
            Parameter.Box parameter = new Parameter.Box();
            //parameter.ID = slot.slot.UID;
            //parameter.callback = ClickInBag;
            
            parameter.obj = slot;
            GameObject _slotButton = slot.button.transform.Find("Bottom/Image").gameObject;
            EventTriggerListener.Get(_slotButton).parameter = parameter;
            EventTriggerListener.Get(_slotButton).onClickByParameter = InputMaterial;
        }
    }

    //TODO:历史记录
    private ArrayList InputBox = new ArrayList();
    private ArrayList PropertyBox = new ArrayList();
    private int Quality = 0;
    private int QualityEffectID = 0;

    public struct PropertyElementBase
    {
        public int ID;
        public Materiral.Property Property;
        public RectTransform Button;
    }

    //表示合成状态
    enum RecipeStat
    {
        WaitInput,
        Compose,
    }
    RecipeStat ripeState = RecipeStat.WaitInput;
    //改变状态方法
    void ChangeRecipeStat(RecipeStat stat)
    {
        ripeState = stat;
    }
    IEnumerator ChangeRecipeStatByWait(RecipeStat stat,float time)
    {
        yield return new WaitForSeconds(time);
        ripeState = stat;
    }


    void InputMaterial(GameObject go, object parameter)
    {
        //如果已经放入则不能再放入
        if (InputBox.Contains(parameter))
            return;

        //判断状态
        if (ripeState != RecipeStat.WaitInput)
            return;

        //改变状态
        ChangeRecipeStat(RecipeStat.Compose);
 
        InputBox.Add(parameter);
        Parameter.Box pro = (Parameter.Box)parameter;
        SlotBox _slot = (SlotBox)pro.obj;
        GameObject _slotButton = _slot.button.transform.Find("Bottom/Image").gameObject;

        //合成槽效果
        _slotButton.GetComponent<Image>().color = Color.gray;
        GameObject count = _slot.button.transform.Find("Count").gameObject;
        count.SetActive(true);
        count.transform.GetChild(0).GetComponent<Text>().text = InputBox.Count.ToString();

        //飞向属性栏
        GameObject _btn = Instantiate(btn_recipeSlot);
        _btn.transform.Find("Text").GetComponent<Text>().text = _slot.slot.Name;
        _btn.transform.Find("Bottom/Image").GetComponent<Image>().sprite = Materiral.GetMaterialIcon(_slot.slot.MateriralType, _slot.slot.ID);
        _btn.transform.Find("Bottom").GetComponent<Image>().enabled = false;
        _btn.transform.position = _slot.button.transform.position;
        _btn.transform.localScale = _slot.button.transform.localScale;
        _btn.transform.SetParent(Plane.transform);

        Vector3 _movepos=new Vector3();
        Vector3 tmp = GetPropertyPos(PropertyBox.Count + 1);
        RectTransform _box = PropertyListGrid.GetComponent<RectTransform>();
        _movepos = new Vector3(tmp.x + _box.position.x, tmp.y + _box.position.y);

        float actionTime = 0.65f;
        LeanTween.move(_btn.gameObject, _movepos, actionTime).setEase(LeanTweenType.easeOutQuart);
        LeanTween.scale(_btn.gameObject, new Vector3(0.1f, 0.1f, 1), actionTime).setDestroyOnComplete(true);

        //属性栏UI效果
        if (_slot.slot.Property != null)
        {
            AddQualityUIEft(_slot);
            AddPropertyUIEft(_slot.slot);
            ComposeProperty();
        }
        else
        {
            AddQualityUIEft(_slot);
            ShowComposeButton();
            //改变状态
            ChangeRecipeStatByWait(RecipeStat.WaitInput, 0.5f);
            return;
        }
    }

    void ShowComposeButton()
    {
        if (SlotList.Count == InputBox.Count)
        {
            btn_Compose.gameObject.SetActive(true);
            EventTriggerListener.Get(btn_Compose.gameObject).onClick = StartCompose;
            btn_Compose.transform.localScale=new Vector3(0.1f,0.1f,0.1f);
            LeanTween.scale(btn_Compose.gameObject, new Vector3(1, 1, 1), 1f).setEase(LeanTweenType.easeOutBack);
        }
    }

    void ShowMateriralInfo(GameObject go, object parameter)
    {
        CharBag.Goods goods = (CharBag.Goods)parameter;

        ItemInfoUI info = Instantiate(iteminfo).GetComponent<ItemInfoUI>();
        info.Ini();
        info.OpenItemInfo(goods);
    }

    //合成道具
    private GameObject targetgoods;

    void StartCompose(GameObject go)
    {
        for (int i = 0; i < recipeSlotsList.transform.childCount; i++)
        {
            if (recipeSlotsList.transform.GetChild(i).gameObject.name == "Compose")
                continue;
            GameObject slot = recipeSlotsList.transform.GetChild(i).gameObject;
            slot.transform.Find("Bottom/Image").GetComponent<Image>().color = Color.white;
            slot.transform.Find("Bottom").GetComponent<Image>().enabled = false;
            slot.transform.Find("Text").GetComponent<Text>().text = "";
            slot.transform.Find("Count").gameObject.SetActive(false);
        }

        float actionTime = 3f;

        LeanTween.scale(btn_Compose.gameObject, new Vector3(0.1f, 0.1f, 0.1f), 0.1f).setEase(LeanTweenType.easeInBack).setOnComplete(
            () =>
            {
                btn_Compose.gameObject.SetActive(false);
            });
       
        ComposeMask.SetActive(true);
        Color c = recipeSlotsList.GetComponent<Image>().color;
        recipeSlotsList.GetComponent<Image>().color = new Color(c.r, c.g, c.b, 0);
        Color m = ComposeMask.GetComponent<Image>().color;
        //渐现效果
        LeanTween.value(ComposeMask.gameObject, c, new Color(m.r, m.g, m.b, 0.7f), actionTime * 0.1f).setOnUpdate(
            (Color col) =>
            {
                ComposeMask.GetComponent<Image>().color = col;
            }
            );
        LeanTween.value(recipeSlotsList.gameObject, c, new Color(c.r, c.g, c.b, 0), actionTime * 0.1f).setOnUpdate(
            (Color col) =>
            {
                recipeSlotsList.GetComponent<Image>().color = col;
            }
            );
        //上升效果
        LeanTween.moveY(recipeSlotsList.gameObject, Screen.height / 2, actionTime * 0.5f).setEase(LeanTweenType.easeOutQuad);
        //旋转效果
        LeanTween.rotateZ(recipeSlotsList.gameObject, 360 * 10, actionTime).setEase(LeanTweenType.easeInOutQuad);
        LeanTween.scale(recipeSlotsList.gameObject, new Vector3(1.3f, 1.3f, 1.3f), actionTime * 0.7f).setEase(LeanTweenType.easeOutQuad).setOnComplete(
            () =>
            {
                LeanTween.scale(recipeSlotsList.gameObject, new Vector3(0.1f, 0.1f, 0.1f), actionTime * 0.3f).setEase(LeanTweenType.easeInBack).setOnComplete(
                    () =>
                    {
                        CharBag.Goods goods = CreateGoods();
                        btn_startRecipe.gameObject.SetActive(true);
                        btn_startRecipe.transform.Find("Text").GetComponent<Text>().text = "收取";
                        recipeSlotsList.gameObject.SetActive(false);
                        EventTriggerListener.Get(btn_startRecipe.gameObject).onClick = CloseRecipe;

                        targetgoods = Instantiate(btn_recipeSlot);
                        targetgoods.transform.SetParent(recipeUI.transform);
                        targetgoods.transform.SetAsLastSibling();
                        targetgoods.transform.position = recipeSlotsList.transform.position;
                        targetgoods.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                        LeanTween.scale(targetgoods, new Vector3(1.5f, 1.5f,1.5f), 1f).setEase(LeanTweenType.easeOutBack);

                        targetgoods.transform.Find("Text").GetComponent<Text>().text = goods.Name;
                        targetgoods.transform.Find("Bottom/Image").GetComponent<Image>().sprite = Materiral.GetMaterialIcon(goods.MateriralType, goods.ID);

                        GameObject _targetButton = targetgoods.transform.Find("Bottom/Image").gameObject;
                        EventTriggerListener.Get(_targetButton).parameter = goods;
                        EventTriggerListener.Get(_targetButton).onClickByParameter = ShowMateriralInfo;
                    });
            }
            );
    }

    CharBag.Goods CreateGoods()
    {
        CharBag.Goods goods = new CharBag.Goods();
        goods.Property = new int[4];

        for (int i = 0; i < PropertyBox.Count; i++)
        {
            //如果大于最大个数则忽略
            if (i >= 4)
                break;
            PropertyElementBase p = (PropertyElementBase)PropertyBox[i];
            goods.Property[i] = p.Property.ID;
        }

        goods.Quality = Quality;
        goods.Number = 1;
        goods.Name = recipe.Name;

        string target = recipe.Target;
        if (target[0] == char.Parse("0"))
        {
            string mat_str = target.Substring(target.IndexOf(",") + 1);
            goods.MateriralType = 0;
            goods.ID = int.Parse(mat_str);
        }
        else if(target[0] == char.Parse("1"))
        {
            string mat_str = target.Substring(target.IndexOf(",") + 1);
            goods.MateriralType = 1;
            goods.ID = int.Parse(mat_str);
        }

        goods.Type = Materiral.GetTypeByMaterialID(goods.MateriralType, goods.ID);
        goods.MaterialEffet = QualityEffectID;

        //计算价格
        goods.Price = Materiral.GetMaterialPrice(goods.MateriralType, goods.ID);
        goods = CharBag.SetPrice(goods);

        //添加道具
        goods.UID = CharBag.AddGoods(goods);

        questManager.CheckQuestListWithGoods(QuestManager.QuestTypeList.ComposeGoods, goods, 0);
        //更新物品信息
        PlayerInfo.AddGoodsInfo(goods.MateriralType, goods.ID, PlayerInfo.GoodsInfoType.RecipeCount);

        //删除道具
        foreach (SlotBox slot in SlotList.Values)
        {
            CharBag.RemoveGoods(slot.slot.UID);
        }

        return goods;
    }


    //品质UI效果
    void AddQualityUIEft(SlotBox _slot)
    {
        int oldQuality = Quality;
        if (InputBox.Count > 1)
            Quality = (Quality + _slot.slot.Quality) / 2;
        else
            Quality = _slot.slot.Quality;

        LeanTween.value(QualityValue.gameObject, oldQuality, Quality, 0.5f).setOnUpdate(
            (float q) =>
            {
                int text = (int)q;
                QualityValue.text = text.ToString();
            }
            );
        LeanTween.value(QualityBar.gameObject, oldQuality, Quality, 0.5f).setOnUpdate(
                    (float q) =>
                    {
                        QualityBar.size = q / 100;
                    }
                    );
    }

    void AddFristPropertyUIEft()
    {
        CharBag.Goods _fristGoods = new CharBag.Goods();
        string[] mt = recipe.Target.Split(',');
        _fristGoods.Property = Materiral.GetMaterialProperty(int.Parse(mt[0]), int.Parse(mt[1]));
        AddPropertyUIEft(_fristGoods);
    }

    //属性效果UI效果
    void AddPropertyUIEft(CharBag.Goods _slot)
    {
        float actionTime = 0.5f;

        for (int i = 0; i < _slot.Property.Length; i++)
        {
            if (_slot.Property[i] == 0)
                continue;

            int proID = _slot.Property[i];
            RectTransform _property = Instantiate(PropertyListElement).GetComponent<RectTransform>();
            _property.transform.SetParent(PropertyListGrid.transform);

            _property.localPosition = GetPropertyPos(PropertyBox.Count + 1);
            _property.sizeDelta = GetPropertySize(PropertyBox.Count + 1);

            //获取materiral的颜色
            Color _color_image = _property.GetComponent<Image>().color;
            Color _color_text = _property.transform.Find("Text").GetComponent<Text>().color;
            Color from = new Color(_color_image.r, _color_image.g, _color_image.b, 0);
            _property.GetComponent<Image>().color = from;
            _property.transform.Find("Image").GetComponent<Image>().color = from;
            _property.transform.Find("Text").GetComponent<Text>().color = from;

            //渐现效果
            LeanTween.value(_property.gameObject, from, _color_image, 0.25f).setDelay(actionTime).setOnUpdate(
                (Color col) =>
                {
                    _property.GetComponent<Image>().color = col;
                }
                );
            LeanTween.value(_property.gameObject, from, _color_text, 0.25f).setDelay(actionTime).setOnUpdate(
                (Color col) =>
                {
                    _property.transform.Find("Text").GetComponent<Text>().color = col;
                }
                );
            LeanTween.value(_property.gameObject, from, new Color(1,1,1,1), 0.25f).setDelay(actionTime).setOnUpdate(
                (Color col) =>
                {
                    _property.transform.Find("Image").GetComponent<Image>().color = col;
                }
                );

            //TODO：点击效果

            //EndTODO

            Materiral.Property _p = Materiral.GetProNameByProID(proID);
            _property.transform.Find("Text").GetComponent<Text>().text = _p.Name;
            _property.transform.Find("Image").GetComponent<Image>().sprite = Materiral.GetIconByName(_p.IMG);

            PropertyElementBase _proBase = new PropertyElementBase();
            _proBase.ID = PropertyBox.Count + 1;
            _proBase.Property = _p;
            _proBase.Button = _property;
            PropertyBox.Add(_proBase);

        }
    }

    int _composCount = 0;
    void ComposeProperty()
    {
        int listCount = PropertyBox.Count;
        float actionTime = 0.5f;
        float DelayTime = 1f;

        Recipe.Result composID = new Recipe.Result();
        ArrayList compose = recipeMap.ComposeProperty(PropertyBox, out composID);

        if (compose.Count != listCount)
        {
            if (_composCount > 0)
                DelayTime = 0;

            _composCount++;
            LeanTween.move(composID.Base2.Button.gameObject, composID.Base1.Button.position, actionTime).setEase(LeanTweenType.easeInOutQuad).setDelay(DelayTime).setOnComplete(
                () =>
                {
                    composID.Base1.Button.Find("Text").GetComponent<Text>().text = composID.Base1.Property.Name;
                    composID.Base1.Button.Find("Image").GetComponent<Image>().sprite = Materiral.GetIconByName(composID.Base1.Property.IMG);
                    Destroy(composID.Base2.Button.gameObject);
                    //更新任务
                    //questManager.CheckQuestListWithGoods(QuestManager.QuestTypeList.ComposeProperty, composID.Base1.ID);
                    LeanTween.scale(composID.Base1.Button.gameObject, new Vector3(1.1f, 1.1f, 1.1f), 0.125f).setLoopPingPong(1).setOnComplete(
                        () =>
                        {
                            ComposeProperty();
                        });
                });
        }
        else
        {
            //如果合成，则等待oncomplete改变状态
            if (_composCount > 0)
            {
                //改变状态
                _composCount = 0;
                ShowComposeButton();
                //对齐位置
                SetPropertyPos();
                return;
            }
            //如果合成没有合成时
            else
            {
                _composCount = 0;
                ShowComposeButton();
                //改变状态
                ChangeRecipeStat(RecipeStat.WaitInput);
                return;
            }
        }
    }

    void SetPropertyPos()
    {
        float actionTime = 0.5f;

        for (int i = 0; i < PropertyBox.Count; i++)
        {
            PropertyElementBase _p = (PropertyElementBase)PropertyBox[i];

            Vector3 pos = GetPropertyPos(_p.ID);

            if (i < PropertyBox.Count - 1)
            {
                LeanTween.moveLocal(_p.Button.gameObject, pos, actionTime);
            }
            else
            {
                LeanTween.moveLocal(_p.Button.gameObject, pos, actionTime).setEase(LeanTweenType.easeInOutQuad).setOnComplete(
                                       () =>
                                       {
                                           //改变状态
                                           ChangeRecipeStat(RecipeStat.WaitInput);
                                       });
            }
        }
    }

    Vector3 GetPropertyPos(int count)
    {
        //获取容器大小
        float boxWidth = PropertyListGrid.GetComponent<RectTransform>().rect.width;
        float boxheight = PropertyListGrid.GetComponent<RectTransform>().rect.height;
        //设定间距大小
        float spacingX = boxWidth * 0.015f;
        float spacingY = boxheight * 0.015f;
        //设定按钮大小
        float btn_width = boxWidth / 3.2f;
        float btn_height = btn_width / 3;
        
        //设定按钮位置
        int perrow = 3;
        int colcount = ((count - 1) / perrow) + 1;
        int rowcount = count - (colcount - 1) * perrow;

        float posX = btn_width * (rowcount - 1) + spacingX * rowcount + btn_width / 2;
        float posY = -(btn_height * (colcount - 1) + spacingY * colcount + btn_height / 2);

        return new Vector3(posX, posY);
    }

    Vector2 GetPropertySize(int count)
    {
        //获取容器大小
        float boxWidth = PropertyListGrid.GetComponent<RectTransform>().rect.width;
        //float boxheight = PropertyListGrid.GetComponent<RectTransform>().rect.height;
        ////设定间距大小
        //float spacingX = boxWidth * 0.015f;
        //float spacingY = boxheight * 0.015f;
        //设定按钮大小
        float btn_width = boxWidth / 3.2f;
        float btn_height = btn_width / 3;

        return new Vector2(btn_width, btn_height);
    }

}
