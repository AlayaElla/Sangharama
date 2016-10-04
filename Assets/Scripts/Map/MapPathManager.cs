using UnityEngine;
using System.Collections;

public class MapPathManager : MonoBehaviour {

    public struct Path
    {
        public int Map; //地图id
        public int[] Next;   //之后的路点集合
        public int[] Pre;  //之前的路点集合
        public int[] Points;
        public int EndType; //终点类型，0是末尾是路点，1是起点是路点
    }
    //角色状态
    enum MoveState
    {
        Stay,
        Moving,
    }
    MoveState state;
    static Path[] PathList;
    public Transform MovePlayer;

    struct Points {
        public int Nowpoint;
        public int Nextpoint;
        public int Targetpoint;
        public enum PointsVecter
        {
            UP,
            DOWN
        }
        public PointsVecter Vecter; 
    }
    Points playerPoints;

    int startpoint = 9;
    ArrayList pathBox = new ArrayList();  //用于保存路点
    ArrayList Bestpaths = new ArrayList();

    CharacterModle charaModle;

	// Use this for initialization
	void Start () {
        //获取路径配置表
        GetPathConfig();
        state = MoveState.Stay;
        playerPoints.Nowpoint = startpoint;

        //读取角色配置表
        charaModle = GameObject.Find("/CollectionTools/CharacterModle").GetComponent<CharacterModle>();

        InstPlayer(playerPoints.Nowpoint);
        AddPathPointListener();

	}
	
	// Update is called once per frame
	void Update () {

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

        AniController.Get(MovePlayer).AddSprite(charaModle.GetSkinSprite("boy1"));
        CharacterModle.Skin skin = charaModle.GetSkin("boy1");
        AniController.Get(MovePlayer).PlayAniCanBreak(skin.ActionList["down"][0], skin.ActionList["down"][1], AniController.AniType.LoopBack, 10);
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
                    }
                }
            }
        }
    }


    void SetTarget(GameObject go)
    {
        //如果正在移动则跳过
        if (state != MoveState.Stay)
        {
            SmallNoticeUI sNotice = new SmallNoticeUI();
            sNotice = sNotice.INIT();
            sNotice.SetMaxNotice(2, MovePlayer);
            sNotice.SetAlignType(SmallNoticeList.Align.UP, MovePlayer);
            sNotice.SetFirstPosition(new Vector3(0, Screen.height / 2, 0), MovePlayer);

            string str = "移动中...";
            sNotice.OpenNotice(str, 2f, MovePlayer);
            return;
        }

        playerPoints.Targetpoint = int.Parse(go.name);
        pathBox.Clear();
        pathBox.Add("," + playerPoints.Nowpoint.ToString() + ",");
        FindPath(playerPoints.Nowpoint);
        Bestpaths = GetBestPath(pathBox);

        MovePath();
        //InstPlayer(playerPoints.Targetpoint);
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
        string root = "/Canvas/Scroll View/Viewport/Content/map/pathList/";
        string pathline = "";
        Transform[] _ts = new Transform[0];

        //如果到达终点则打断
        if (playerPoints.Nowpoint == playerPoints.Targetpoint)
        {
            state = MoveState.Stay;
            return;
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
            float time = cr.distance / 300;
            LeanTween.moveLocal(MovePlayer.gameObject, cr, time).setOnComplete(() =>
                {
                    playerPoints.Nowpoint = playerPoints.Nextpoint;
                    MovePath();
                });
        }
    }


    //镜头跟随主角
    void CmameraFollowPlayer()
    {
 
    }




}
