using UnityEngine;
using System.Collections;

public class MapPathManager : MonoBehaviour {

    public struct Path
    {
        public int Map; //地图id
        public int[] Next;   //之后的路点集合
        public int[] Pre;  //之前的路点集合
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

    CharacterModle charaModle;

	// Use this for initialization
	void Start () {
        //获取路径配置表
        GetPathConfig();
        state = MoveState.Stay;
        playerPoints.Nowpoint = startpoint;

        //读取角色配置表
        charaModle = GameObject.Find("/CollectionTools/CharacterModle").GetComponent<CharacterModle>();
       
        InstPlayer();
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

    void InstPlayer()
    {
        string actionRoot = "Canvas/Scroll View/Viewport/Content/map/action" + startpoint + "/" + startpoint;
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
                for (int i = 1; i < actioncount; i++)
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
        playerPoints.Targetpoint = int.Parse(go.name);
        pathBox.Clear();
        pathBox.Add(playerPoints.Targetpoint);
        FindPath(playerPoints.Targetpoint);

        string str = "";
        foreach (int i in pathBox)
        {
            str += i.ToString() + ",";
        }
        Debug.Log("tagert:" + go.name + "   path:" + str);
    }

    void FindPath(int path)
    {
        //从起始点开始查找路点。
        //双向查找——建立两个str[3]，第一个保存方向：0向上；1向下；第二个保存分支的下一个路点；第三个保存信息。
        //如果遇到分叉路点，则新建立一个分叉，分支需要保存下一个查找的点
        //如果遇到重复的点，则判断miss
        //如果遇到到达的点不知目标点则miss
        //如果计算完成则判断所有得出点的长度找出最短的，然后计入路点。

        int checkindex = GetCheckPoint();
        //如果点击的路点就是当前路点，则没反应
        if (playerPoints.Targetpoint == playerPoints.Nowpoint)
            return;
        Path _p = PathList[checkindex];

        int nextcount=0;

        if (_p.Next != null)
        {
            for (int i = 0; i < _p.Next.Length; i++)
            {

            }
        }





        //////////////////////////////向上查找
        if (playerPoints.Vecter == Points.PointsVecter.UP)
        {

        }
        //////////////////////////////向下查找
        else if (playerPoints.Vecter == Points.PointsVecter.DOWN)
        {
            if (_p.Next != null)
            {
                for (int i = 0; i < _p.Next.Length; i++)
                {
                    if (_p.Next[i] == playerPoints.Targetpoint)
                    {
                        pathBox.Add(_p.Map);
                        playerPoints.Targetpoint = _p.Map;
                        FindPath(playerPoints.Targetpoint);
                        break;
                    }
                }
            }
            //如果不能快速找到上一个则便利所有路点
            else
            {
                for (int i = 0; i < PathList.Length; i++)
                {
                    Path __p = PathList[i];
                    //跳过终点路点
                    if (__p.Next == null)
                        continue;

                    for (int j = 0; j < __p.Next.Length; j++)
                    {
                        if (__p.Next[j] == playerPoints.Targetpoint)
                        {
                            pathBox.Add(__p.Map);
                            playerPoints.Targetpoint = __p.Map;
                            FindPath(playerPoints.Targetpoint);
                            return;
                        }
                    }
                }
            }
        }
    }

    int GetCheckPoint()
    {
        int check = 0;
        //如果点击的路点大于当前路点则快速检查上一个路点是否是正确路径点
        if (playerPoints.Targetpoint > playerPoints.Nowpoint)
        {
            playerPoints.Vecter = Points.PointsVecter.DOWN;
            check = playerPoints.Targetpoint - 2;
        }
        else if (playerPoints.Targetpoint < playerPoints.Nowpoint)
        {
            playerPoints.Vecter = Points.PointsVecter.UP;
            check = playerPoints.Targetpoint;
        }
        return check;
    }

    //镜头跟随主角
    void CmameraFollowPlayer()
    {
 
    }




}
