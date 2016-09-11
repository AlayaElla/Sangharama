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
    string Nowpoint;
    string Targetpoint;
    string startpoint = "1";

    CharacterModle charaModle;

	// Use this for initialization
	void Start () {
        //获取路径配置表
        GetPathConfig();
        state = MoveState.Stay;
        Nowpoint = startpoint;

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
                        EventTriggerListener.Get(actionroot.GetChild(i)).onClick = FindPath;
                    }
                }
            }
        }
    }

    void FindPath(GameObject go)
    {
        Debug.Log(go.name);
    }


    //镜头跟随主角
    void CmameraFollowPlayer()
    {
 
    }




}
