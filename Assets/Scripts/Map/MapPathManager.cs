using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MapPathManager : MonoBehaviour {

    public struct Path
    {
        public int Map; //地图id
        public string Name; //地图名字
        public int[] Next;   //之后的路点集合
        public int[] Pre;  //之前的路点集合
        public int[] Points;
        public int Price; //从这个路点经过所需的费用
    }
    //角色状态
    enum MoveState
    {
        Stay,
        Moving,
        Mining,
    }
    MoveState state;
    static Path[] PathList;     //路点配置列表
    public Transform MovePlayer;
    public RectTransform pathPoint;
    //价格面板
    public RectTransform priceBoard;
    Text priceText;
    RectTransform btn_priceOK;
    RectTransform btn_priceCancle;
    RectTransform btn_okMask;
    //回家面板
    public RectTransform homeBoard;
    Text homeText;
    //采集面板
    public RectTransform mineActionBoard;
    Text MineText;
    RectTransform btn_mine;
    RectTransform btn_mineMask;

    struct Points {
        public int Nowpoint;
        public int Nextpoint;
        public int Targetpoint;
        public enum PointsVecter
        {
            UP,
            DOWN
        }
        public int Price;
        public RectTransform Targetposition; 
    }
    Points playerPoints;

    int startpoint = 1;
    ArrayList pathBox = new ArrayList();  //用于保存路点
    ArrayList Bestpaths = new ArrayList();
    ArrayList movePathLine = new ArrayList();   //用于保存移动路径的点

    CharacterModle charaModle;

    //获取地图的ui
    MapUI _mapUI;

    //获取采集控制脚本
    CollectAction collectAction;

    //事件管理器
    ChatEventManager eventmanager;
    QuestManager questManager;

    // Use this for initialization
    void Start () {
        //获取路径配置表
        GetPathConfig();
        state = MoveState.Stay;
        playerPoints.Nowpoint = startpoint;

        //获取价格文本，确定和取消按钮
        priceText = priceBoard.FindChild("price/pricetext").GetComponent<Text>();
        btn_priceOK = priceBoard.FindChild("ok").GetComponent<RectTransform>();
        btn_priceCancle = priceBoard.FindChild("cancle").GetComponent<RectTransform>();
        btn_okMask = priceBoard.FindChild("okMask").GetComponent<RectTransform>();
        //获取采集文本和采集按钮
        MineText = mineActionBoard.FindChild("Text").GetComponent<Text>();
        btn_mine = mineActionBoard.FindChild("mine").GetComponent<RectTransform>();
        btn_mineMask = mineActionBoard.FindChild("mineMask").GetComponent<RectTransform>();
        //获取回家文版
        homeText = homeBoard.FindChild("Text").GetComponent<Text>();

        //读取角色配置表
        charaModle = GameObject.Find("/CollectionTools/CharacterModle").GetComponent<CharacterModle>();
        //获取地图的ui
        _mapUI = GameObject.Find("/CollectionTools/Colection").GetComponent<MapUI>();
        //读取采集控制脚本
        collectAction = GameObject.Find("/CollectionTools/Colection").GetComponent<CollectAction>();
        //获取事件控制器
        eventmanager = transform.Find("/ToolsKit/EventManager").GetComponent<ChatEventManager>();
        questManager = transform.Find("/ToolsKit/QuestManager").GetComponent<QuestManager>();

        InstPlayer(playerPoints.Nowpoint);
        AddPathPointListener();

        eventmanager.PreCheckEventList(1);
        questManager.PreCheckQuest(1);

        questManager.IsArriveWaitingCheckPoint(1);
    }
	
	// Update is called once per frame
	void Update () {

	}

    public static Path[] GetPathList()
    {
        return PathList;
    }

    void GetPathConfig()
    {
        XmlTool xt = new XmlTool();
        ArrayList _list = xt.loadPathXmlToArray();
        PathList = new Path[_list.Count];
        _list.CopyTo(PathList);
        GetPrePath(PathList);
        SetPoints();
    }

    void GetPrePath(Path[] list)
    {
        //用来记录pre的信息
        string[] str_prelist = new string[list.Length];

        //获取Pre的信息
        for (int i = 0; i < list.Length; i++)
        {
            Path _p = list[i];
            if (_p.Next == null)
                continue;

            for (int j = 0; j < _p.Next.Length; j++)
            {
                int _pre = _p.Next[j]-1;
                if (str_prelist[_pre] == null)
                {
                    str_prelist[_pre] += _p.Map;
                }
                else
                {
                    str_prelist[_pre] += "," + _p.Map;
                }
            }
        }
        //把Pre的信息赋值到列表中
        for (int i = 0; i < list.Length; i++)
        {
            Path _p = list[i];

            string _str = str_prelist[i];
            if (_str == null)
                continue;

            string[] str_temp = _str.Split(',');
            _p.Pre = new int[str_temp.Length];
            for (int j = 0; j < _p.Pre.Length; j++)
            {
                _p.Pre[j] = int.Parse(str_temp[j]);
            }

            list[i] = _p;
        }
    }

    void SetPoints()
    {
        for (int i = 0; i < PathList.Length; i++)
        {
            Path _p = PathList[i];
            int precount = (_p.Pre == null) ? 0 : _p.Pre.Length;
            int nextcount = (_p.Next == null) ? 0 : _p.Next.Length;
            int len = precount + nextcount;

            _p.Points = new int[len];
            for (int j = 0; j < nextcount; j++)
            {
                _p.Points[j] = _p.Next[j];
            }
            for (int x = 0; x < precount; x++)
            {
                _p.Points[nextcount + x] = _p.Pre[x];
            }

            PathList[i] = _p;
        }
    }


    void InstPlayer(int point)
    {
        string actionRoot = "Canvas/Scroll View/Viewport/Content/map/action" + point + "/" + point;
        Transform root = GameObject.Find(actionRoot).transform;
        MovePlayer.position = root.position;

        AniController.Get(MovePlayer).AddSprite(charaModle.GetSkinSprite("boy1"), charaModle.GetSkin("boy1"));
        AniController.Get(MovePlayer).PlayAniBySkin("down", AniController.AniType.LoopBack, 5);

        ShowHomeBoard(1);
    }


    void AddPathPointListener()
    {
        string actionRoot = "Canvas/Scroll View/Viewport/Content/map/";
        Transform root = GameObject.Find(actionRoot).transform;
        int childscount = root.childCount;

        for (int index = 0; index < childscount; index++)
        {
            if (root.GetChild(index).name.StartsWith("action"))
            {
                Transform actionroot = root.GetChild(index);
                int actioncount = actionroot.childCount;
                for (int i = 0; i < actioncount; i++)
                {
                    //如果是数字则就是采集点
                    if (MathTool.isNumber(actionroot.GetChild(i).name))
                    {
                        EventTriggerListener.Get(actionroot.GetChild(i)).onClick = SetTarget;

                        //现在设置名字的方法是根据child的在scene中的排序来设置的，所以尽量不要动排序。（第一个为pathList路点列表）
                        actionroot.GetChild(i).GetChild(0).GetComponent<Text>().text = PathList[index - 1].Name;
                    }
                }
            }
        }
    }


    void SetTarget(GameObject go)
    {
        //如果正在移动/采集则跳过
        if (state != MoveState.Stay)
        {
            SmallNoticeUI sNotice = gameObject.AddComponent<SmallNoticeUI>();
            sNotice = sNotice.INIT();
            sNotice.SetMaxNotice(2, MovePlayer);
            sNotice.SetAlignType(SmallNoticeList.Align.UP, MovePlayer);
            sNotice.SetFirstPosition(new Vector3(0, Screen.height / 2 - 50, 0), MovePlayer);

            string str = "移动中...";
            if (state == MoveState.Mining)
                str = "采集中...";

            sNotice.OpenNotice(str, 0.5f, MovePlayer);
            return;
        }

        playerPoints.Targetpoint = int.Parse(go.name);
        playerPoints.Targetposition = go.GetComponent<RectTransform>();
        pathBox.Clear();
        pathBox.Add("," + playerPoints.Nowpoint.ToString() + ",");
        FindPath(playerPoints.Nowpoint);
        Bestpaths = GetBestPath(pathBox);

        ShowPriceBoard(playerPoints.Targetpoint);
        
        //获取路径中的点，然后显示出来
        GetMovePathLine();
        ShowMovePathLine();

        //ShowPathPoint(playerPoints.Targetpoint);
    }

    void FindPath(int path)
    {
        //从起始点开始查找路点。
        //双向查找——建立两个str[3]，第一个保存方向：0向上；1向下；第二个保存分支的下一个路点；第三个保存信息。
        //如果遇到分叉路点，则新建立一个分叉，分支需要保存下一个查找的点
        //如果遇到重复的点，则判断miss
        //如果遇到到达的点不知目标点则miss
        //如果计算完成则判断所有得出点的长度找出最短的，然后计入路点。

        //int checkindex = GetCheckPoint();
        //如果点击的路点就是当前路点，则没反应
        if (playerPoints.Targetpoint == playerPoints.Nowpoint)
            return;
        Path _p = PathList[path - 1];

        //向下查找
        if (_p.Points != null)
        {
            int pointsindex = pathBox.Count - 1;
            string _str = (string)pathBox[pointsindex];
            for (int i = 0; i < _p.Points.Length; i++)
            {
                //如果遇到和之前相同的路点，则表示绕了一圈。废弃当前路点。
                if (_str.Contains("," + _p.Points[i].ToString() + ","))
                    continue;
                
                if (i > 0 && _p.Points.Length >= 2)
                {
                    pathBox.Add(_str);
                }

                string pointstr = _str + _p.Points[i] + ",";
                pathBox[pathBox.Count - 1] = pointstr;

                //如果到达终点则结束当前路点的循环。
                if (_p.Points[i] == playerPoints.Targetpoint)
                    break;

                FindPath(_p.Points[i]);
            }
        }
    }

    ArrayList GetBestPath(ArrayList list)
    {
        int ckeckLen = 99999;
        ArrayList path = new ArrayList();
        foreach (string _str in list)
        {
            if (_str.Contains("," + playerPoints.Targetpoint.ToString() + ","))
            {
                //筛选出最短路径
                if (ckeckLen > _str.Length)
                {
                    string[] str_temp = _str.Split(',');
                    int offest = 1; //跳过第n个和最后n个循环
                    for (int j = offest; j < str_temp.Length - offest; j++)
                    {
                        path.Add(int.Parse(str_temp[j]));
                    }

                    ckeckLen = _str.Length;
                }
            }
        }
        return path;
    }


    void MovePath()
    {
        //如果有采集、回家图标，则关闭采集，回家图标
        CloseMineIcon();
        CloseHomeIcon();

        string root = "/Canvas/Scroll View/Viewport/Content/map/pathList/";
        string pathline = "";
        Transform[] _ts = new Transform[0];

        //如果到达终点则打断
        if (playerPoints.Nowpoint == playerPoints.Targetpoint)
        {
            //增加路点次数信息
            PlayerInfo.AddMapInfo(playerPoints.Nowpoint);

            //检测触发事件
            if (eventmanager.CheckEventList(ChatEventManager.ChatEvent.EventTypeList.Arrive, true))
            {
                eventmanager.StartStory();
            }
            questManager.CheckQuestListWithArrive(playerPoints.Targetpoint);
            questManager.IsArriveWaitingCheckPoint(playerPoints.Targetpoint);
            state = MoveState.Stay;
            AniController.Get(MovePlayer).PlayAniBySkin("down", AniController.AniType.LoopBack, 5);

            //关闭路点提示
            ClosePathPoint();
            CloseMovePathLine();
            movePathLine.Clear();

            if (playerPoints.Targetpoint != 1)
            {
                //显示采集图标
                ShowMineIcon(playerPoints.Targetpoint);
                return;
            }
            else
            {
                ShowHomeBoard(1);
                return;
            }
        }

        //找到移动的路径
        for (int i = 0; i <= Bestpaths.Count - 2; i++)
        {
            //根据方向排序
            if ((int)Bestpaths[i] == playerPoints.Nowpoint)
            {
                if ((int)Bestpaths[i + 1] > (int)Bestpaths[i])
                {
                    pathline = "pathLine_" + Bestpaths[i].ToString() + "_" + Bestpaths[i + 1].ToString();
                    Transform t = transform.Find(root + pathline);
                    if (t != null)
                    {
                        _ts = new Transform[t.childCount];
                        for (int j = 0; j < t.childCount; j++)
                        {
                            _ts[j] = t.GetChild(j);
                        }
                    }
                    else
                    {
                        Debug.Log("can't find pathline:" + root + pathline);
                    }

                }
                else
                {
                    pathline = "pathLine_" + Bestpaths[i+1].ToString() + "_" + Bestpaths[i].ToString();
                    Transform t = transform.Find(root + pathline);
                    if (t != null)
                    {
                        _ts = new Transform[t.childCount];
                        for (int j = 0; j < t.childCount; j++)
                        {
                            _ts[j] = t.GetChild(t.childCount - 1 - j);
                        }
                    }
                    else
                    {
                        Debug.Log("can't find pathline:" + root + pathline);
                    }
                }
                playerPoints.Nextpoint = (int)Bestpaths[i + 1];
                break;
            }
        }

        //移动方法
        if (_ts.Length != 0)
        {
            playerPoints.Nowpoint = playerPoints.Nextpoint;
            Transform _map = transform.Find("/Canvas/Scroll View/Viewport/Content/map");
            Vector3[] vecs = new Vector3[_ts.Length + 2];

            for (int i = 0; i < _ts.Length; i++)
            {
                vecs[i + 1] = _ts[i].position - _map.position;
            }
            vecs[0] = vecs[1];
            vecs[vecs.Length - 1] = vecs[vecs.Length - 2];

            state = MoveState.Moving;
            LTSpline cr = new LTSpline(vecs);
            float time = cr.distance / 400;
            LeanTween.moveLocal(MovePlayer.gameObject, cr, time).setOnComplete(() =>
                {
                    playerPoints.Nowpoint = playerPoints.Nextpoint;
                    MovePath();
                });

            SetCharacterVecter(vecs);
        }
    }

    
    void GetMovePathLine()
    {
        string root = "/Canvas/Scroll View/Viewport/Content/map/pathList/";
        string pathline = "";
        int index = playerPoints.Nowpoint;
        movePathLine.Clear();

        while (index != playerPoints.Targetpoint)
        {
            //找到移动的路径
            for (int i = 0; i <= Bestpaths.Count - 2; i++)
            {
                //根据方向排序
                if ((int)Bestpaths[i] == index)
                {
                    if ((int)Bestpaths[i + 1] > (int)Bestpaths[i])
                    {
                        pathline = "pathLine_" + Bestpaths[i].ToString() + "_" + Bestpaths[i + 1].ToString();
                        Transform t = transform.Find(root + pathline);
                        if (t != null)
                        {

                            for (int j = 0; j < t.childCount; j++)
                            {
                                movePathLine.Add(t.GetChild(j));
                            }
                        }
                        else
                        {
                            Debug.Log("can't find pathline:" + root + pathline);
                        }

                    }
                    else
                    {
                        pathline = "pathLine_" + Bestpaths[i + 1].ToString() + "_" + Bestpaths[i].ToString();
                        Transform t = transform.Find(root + pathline);
                        if (t != null)
                        {
                            for (int j = 0; j < t.childCount; j++)
                            {
                                movePathLine.Add(t.GetChild(t.childCount - 1 - j));
                            }
                        }
                        else
                        {
                            Debug.Log("can't find pathline:" + root + pathline);
                        }
                    }
                    index = (int)Bestpaths[i + 1];
                    break;
                }
            }
        }
    }


    //设定角色方向
    void SetCharacterVecter(Vector3[] vecs)
    {
        //Vector3 target = vecs[vecs.Length - 1] - vecs[0];
        float angle = Vector3.Angle(Vector3.right, vecs[vecs.Length - 1] - vecs[0]);

        if (angle < 45)
        {
            AniController.Get(MovePlayer).PlayAniBySkin("right", AniController.AniType.LoopBack, 10);
        }
        else if (angle < 135)
        {
            if (vecs[vecs.Length - 1].y > vecs[0].y)
            {
                AniController.Get(MovePlayer).PlayAniBySkin("up", AniController.AniType.LoopBack, 10);
            }
            else
            {
                AniController.Get(MovePlayer).PlayAniBySkin("down", AniController.AniType.LoopBack, 10);
            }
        }
        else
        {
            AniController.Get(MovePlayer).PlayAniBySkin("left", AniController.AniType.LoopBack, 10);
        }

        //Debug.Log("angle: " + angle);
    }


    //显示路点提示
    void ShowPathPoint(int point)
    {
        pathPoint.gameObject.SetActive(true);

        string actionRoot = "Canvas/Scroll View/Viewport/Content/map/action" + point + "/" + point;
        RectTransform root = GameObject.Find(actionRoot).GetComponent<RectTransform>();
        pathPoint.position = new Vector3(root.position.x, root.position.y + 20, root.position.z);

        //LeanTween.cancel(pathPoint.gameObject);

        pathPoint.localScale = new Vector3(0, 0, 0);

        float at = 0.3f;
        LeanTween.scale(pathPoint.gameObject, new Vector3(1, 1, 1), 0.25f).setOnComplete(() =>
            {
                LeanTween.scaleX(pathPoint.gameObject, 1.2f, at).setEase(LeanTweenType.easeInOutSine).setLoopPingPong();
                LeanTween.scaleY(pathPoint.gameObject, 0.8f, at).setEase(LeanTweenType.easeInOutSine).setLoopPingPong();
            });
        LeanTween.moveY(pathPoint, pathPoint.localPosition.y + 20, at).setLoopPingPong();
    }

    //关闭路点提示
    void ClosePathPoint()
    {
        LeanTween.cancel(pathPoint.gameObject);
        LeanTween.scale(pathPoint.gameObject, new Vector3(0, 0, 0), 0.15f).setOnComplete(() =>
        {
            pathPoint.gameObject.SetActive(false);
        });
    }


    //镜头跟随主角
    void CmameraFollowPlayer()
    {

    }

    //显示价格面板
    void ShowPriceBoard(int point)
    {
        CloseMovePathLine();
        priceBoard.gameObject.SetActive(true);

        string actionRoot = "Canvas/Scroll View/Viewport/Content/map/action" + point + "/" + point;
        RectTransform root = GameObject.Find(actionRoot).GetComponent<RectTransform>();
        priceBoard.position = new Vector3(root.position.x, root.position.y + 80, root.position.z);

        priceText.text = setPrice(Bestpaths);

        LeanTween.cancel(priceBoard.gameObject);
        priceBoard.localScale = new Vector3(0, 0, 0);
        float at = 0.5f;
        LeanTween.scale(priceBoard.gameObject, new Vector3(1, 1, 1), 0.25f).setEase(LeanTweenType.easeOutBack);
        LeanTween.moveY(priceBoard, priceBoard.localPosition.y + 5, at).setLoopPingPong();

        //添加按钮响应
        EventTriggerListener.Get(btn_priceOK).onClick = OkToMove;
        EventTriggerListener.Get(btn_priceCancle).onClick = ClosePriceBoard;
        EventTriggerListener.Get(btn_okMask).onClick = OkToMove;
    }

    //关闭价格面板
    void ClosePriceBoard(GameObject go)
    {
        LeanTween.cancel(priceBoard.gameObject);
        LeanTween.scale(priceBoard.gameObject, new Vector3(0, 0, 0), 0.15f).setOnComplete(() =>
        {
            priceBoard.gameObject.SetActive(false);
        });
        CloseMovePathLine();
        movePathLine.Clear();
    }

    //获取经过路点的价格，计算总价格
    string setPrice(ArrayList pathlist)
    {
        int price = 0;

        //跳过第一个点，因为起点不算价格
        for (int i = 1; i < Bestpaths.Count; i++)
        {
            foreach (Path _p in PathList)
            {
                //获取价格
                if (_p.Map == (int)Bestpaths[i])
                {
                    price += _p.Price;
                    break;
                }
            }
        }
        //到第一个路点不要钱
        if (playerPoints.Targetpoint == 1)
            price = 0;
        playerPoints.Price = price;

        return price.ToString();
    }

    //在加个板上点击确认按钮
    void OkToMove(GameObject go)
    {
        LeanTween.cancel(priceBoard.gameObject);
        LeanTween.scale(priceBoard.gameObject, new Vector3(0, 0, 0), 0.15f).setOnComplete(() =>
        {
            priceBoard.gameObject.SetActive(false);
        });

        //扣除货币如果不足则弹出货币不足提示
        if (!_mapUI.DownMoney(playerPoints.Price, playerPoints.Targetposition, false))
        {
            SmallNoticeUI sNotice = gameObject.AddComponent<SmallNoticeUI>();
            sNotice = sNotice.INIT();
            string str = "金币不足...";
            sNotice.OpenNotice(str, 0.5f, MovePlayer);

            CloseMovePathLine();
            return;
        }

        MovePath();
        ShowPathPoint(playerPoints.Targetpoint);
    }

    Vector3[,] TempPathList;    //临时用于保存RectTranform的数据
    //显示路点路线变化
    void ShowMovePathLine()
    {
        TempPathList = new Vector3[movePathLine.Count, 2];
        for (int i = 0; i < movePathLine.Count; i++)
        {
            RectTransform lt = (RectTransform)movePathLine[i];
            
            //保存路点的数据
            TempPathList[i, 0] = lt.localPosition;
            TempPathList[i, 1] = lt.localRotation.eulerAngles;

            LeanTween.alpha(lt, 1, 0.4f);
            if (i < movePathLine.Count - 1)
            {
                RectTransform lt2 = (RectTransform)movePathLine[i + 1];
                LeanTween.moveLocal(lt.gameObject, lt2.localPosition, 0.4f).setLoopClamp();
                LeanTween.rotateLocal(lt.gameObject, lt2.localRotation.eulerAngles, 0.4f).setLoopClamp();
            }
        }
    }

    //关闭显示路点变化
    void CloseMovePathLine()
    {
        for (int i = 0; i < movePathLine.Count; i++)
        {
            RectTransform lt = (RectTransform)movePathLine[i];
            LeanTween.cancel(lt.gameObject);
            LeanTween.alpha(lt, (float)150 / 255, 0.2f);

            if (TempPathList.Length != 0)
            {
                //LeanTween.moveLocal(lt.gameObject, TempPathList[i, 0], 0.05f);
                //LeanTween.rotateLocal(lt.gameObject, TempPathList[i, 1], 0.05f);
                lt.localPosition = TempPathList[i, 0];
                lt.localRotation = Quaternion.Euler(TempPathList[i, 1]);
            }
        }
    }

    //显示采集图标
    void ShowMineIcon(int point)
    {
        mineActionBoard.gameObject.SetActive(true);

        string actionRoot = "Canvas/Scroll View/Viewport/Content/map/action" + point + "/" + point;
        RectTransform root = GameObject.Find(actionRoot).GetComponent<RectTransform>();
        mineActionBoard.position = new Vector3(root.position.x, root.position.y + 80, root.position.z);

        LeanTween.cancel(mineActionBoard.gameObject);
        mineActionBoard.localScale = new Vector3(0, 0, 0);
        float at = 0.5f;
        LeanTween.scale(mineActionBoard.gameObject, new Vector3(1, 1, 1), 0.25f).setEase(LeanTweenType.easeOutBack);
        LeanTween.moveY(mineActionBoard, mineActionBoard.localPosition.y + 5, at).setLoopPingPong();
        LeanTween.moveY(MineText.rectTransform, MineText.rectTransform.localPosition.y + 5, at).setLoopPingPong();

        //添加按钮响应
        EventTriggerListener.Get(btn_mine).onClick = CollectAction;
        EventTriggerListener.Get(btn_mineMask).onClick = CollectAction;

        CloseHintSprite(root);
    }

    //关闭采集图标
    void CloseMineIcon()
    {
        if (mineActionBoard.gameObject.activeSelf)
        {
            mineActionBoard.gameObject.SetActive(false);
        }
    }

    //采集方法，采集需要等待采集动作完成后才能继续采集
    bool iscanClick = true;
    void CollectAction(GameObject go)
    {
        if (iscanClick)
        {
            //如果次数不足则弹出次数不足提示
            if (!_mapUI.DownMineCount(1, playerPoints.Targetposition, false))
            {
                SmallNoticeUI sNotice = gameObject.AddComponent<SmallNoticeUI>();
                sNotice = sNotice.INIT();
                string str = "采集次数不足...";
                sNotice.OpenNotice(str, 0.5f, MovePlayer);
                return;
            }

            iscanClick = false;
            state = MoveState.Mining;
            LeanTween.rotateLocal(btn_mine.gameObject, new Vector3(0, 0, 90), 0.25f).setLoopPingPong(2).setOnComplete(() =>
                {
                    collectAction.CollectionAction(playerPoints.Targetpoint, playerPoints.Targetposition);
                    ChangeCanClick();
                    state = MoveState.Stay;
                });
        }
    }

    void ChangeCanClick()
    {
        Debug.Log("change click!!");
        iscanClick = true;
    }


    //显示回家面板
    void ShowHomeBoard(int point)
    {
        homeBoard.gameObject.SetActive(true);

        string actionRoot = "Canvas/Scroll View/Viewport/Content/map/action" + point + "/" + point;
        RectTransform root = GameObject.Find(actionRoot).GetComponent<RectTransform>();
        homeBoard.position = new Vector3(root.position.x, root.position.y + 80, root.position.z);

        LeanTween.cancel(homeBoard.gameObject);
        homeBoard.localScale = new Vector3(0, 0, 0);
        float at = 0.5f;
        LeanTween.scale(homeBoard.gameObject, new Vector3(1, 1, 1), 0.25f).setEase(LeanTweenType.easeOutBack);
        LeanTween.moveY(homeBoard, homeBoard.localPosition.y + 5, at).setLoopPingPong();
        LeanTween.moveY(homeText.rectTransform, homeText.rectTransform.localPosition.y + 5, at).setLoopPingPong();

        CloseHintSprite(root);
    }

    //关闭回家面板
    void CloseHomeIcon()
    {
        if (homeBoard.gameObject.activeSelf)
        {
            LeanTween.cancel(homeBoard.gameObject);
            LeanTween.scale(homeBoard.gameObject, new Vector3(0, 0, 0), 0.15f).setOnComplete(() =>
            {
                homeBoard.gameObject.SetActive(false);
            });
        }
       
    }

    //关闭事件提示
    void CloseHintSprite(RectTransform root)
    {
        Transform hintsprite = root.FindChild("hint");
        if (hintsprite != null)
        {
            LeanTween.scale(hintsprite.gameObject, new Vector3(0, 0, 0), 0.25f).setOnComplete(()=>
            {
                Destroy(hintsprite.gameObject);
            });
        }
    }
}
