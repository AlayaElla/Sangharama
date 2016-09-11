using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterController : MonoBehaviour {

    public enum Turn
    {
        Up,
        Down,
        Left,
        Right
    }

    public float speed = 1f;
    float actiontimer = 0;

    public enum MoveType
    {
        Horizontal,
        Vertical
    }
    MoveType moveType = MoveType.Horizontal;

    Dictionary<string, Vector2> movePoint = new Dictionary<string, Vector2>();
    string targetPoint;
    Vector2 targetVertor = new Vector2();
    Vector2 startVertor = new Vector2();


    public enum CharaState
    {
        Think,
        In,
        Buy,
        Buyed,
        Check,
        Out
    }
    CharaState characterstate = CharaState.Think;
    /// <summary>
    /// 是否进入碰撞区域
    /// </summary>
    bool istrigger = false;

    /// <summary>
    /// 0:没有移动状态；1:转向前移动；2:转向后移动；3:移动完成
    /// </summary>
    int actionStep = 1;

    CharacterModle.CharacterInfo info = new CharacterModle.CharacterInfo();
    CharacterModle.Skin skin = new CharacterModle.Skin();

    ShopGood selectgoods = new ShopGood();
    Vector2 goodsVertor = new Vector2();

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

        if (targetPoint != null)
        {
            if (moveType == MoveType.Horizontal)
            {
                MoveHorizontal();
                                    
            }
            else
            {
                MoveVertical();
            }
        }

        if (characterstate == CharaState.Buy && actionStep == 3)
        {
            GetingGoods();
        }
	}

    //转换角色状态
    public void ChangeCharaState(CharaState _state)
    {
        characterstate = _state;
        Debug.Log("change characterstate:" + _state.ToString());
    }

    void MoveVertical()
    {
        if (actionStep == 3)
            return;

        //竖向
        if (Mathf.Abs(transform.position.y - targetVertor.y) > 0.01f)
        {
            float _m = transform.position.y - targetVertor.y;

            //设置状态
            if (actionStep == 0)
            {
                //设置动作
                if (_m > 0)
                    AniController.Get(transform).PlayAniCanBreak(skin.ActionList["down"][0], skin.ActionList["down"][1], AniController.AniType.LoopBack, 10);
                else if (_m < 0)
                    AniController.Get(transform).PlayAniCanBreak(skin.ActionList["up"][0], skin.ActionList["up"][1], AniController.AniType.LoopBack, 10);

                //重置动画播放百分比
                actiontimer = 0;
                //改变状态
                actionStep = 1;
            }

            //移动
            actiontimer += Time.deltaTime;
            float _totaltime = Mathf.Abs((startVertor.y - targetVertor.y)) / speed;
            float movedis = Mathf.Lerp(startVertor.y, targetVertor.y, actiontimer / _totaltime);
            transform.position = new Vector3(transform.position.x, movedis, transform.position.z);
            /*
            if (_m > 0)
            {
                transform.Translate(new Vector3(0, -speed * Time.deltaTime, 0));
            }
            else
            {
                transform.Translate(new Vector3(0, speed * Time.deltaTime, 0));
            }
             */
        }
        //横向
        else
        {
            float _m = transform.position.x - targetVertor.x;

            //设置状态
            if (actionStep == 1)
            {
                //设置x轴位置
                if (transform.position.y != targetVertor.y)
                    transform.position = new Vector2(transform.position.x, targetVertor.y);
                //设置动作
                if (_m > 0)
                    AniController.Get(transform).PlayAniCanBreak(skin.ActionList["left"][0], skin.ActionList["left"][1], AniController.AniType.LoopBack, 10);
                else if(_m<0)
                    AniController.Get(transform).PlayAniCanBreak(skin.ActionList["right"][0], skin.ActionList["right"][1], AniController.AniType.LoopBack, 10);

                //重置动画播放百分比
                actiontimer = 0;
                //改变状态
                actionStep = 2;
            }

            //移动
            actiontimer += Time.deltaTime;
            float _totaltime = Mathf.Abs((startVertor.x - targetVertor.x)) / speed;
            float movedis = Mathf.Lerp(startVertor.x, targetVertor.x, actiontimer / _totaltime);
            transform.position = new Vector3(movedis, transform.position.y, transform.position.z);
            /*
            if (_m > 0)
            {
                transform.Translate(new Vector3(-speed * Time.deltaTime, 0, 0));
            }
            else
            {
                transform.Translate(new Vector3(speed * Time.deltaTime, 0, 0));
            }
             */
        }

        //如果到达终点
        if (Mathf.Abs(transform.position.x - targetVertor.x) < 0.01f && actionStep == 2)
        {
            //设置y轴位置
            if (transform.position.y != targetVertor.y)
                transform.position = new Vector2(transform.position.x, targetVertor.y);

            actionStep = 3;
        }
    }

    void MoveHorizontal()
    {
        if (actionStep == 3)
            return;

        //横向
        if (Mathf.Abs(transform.position.x - targetVertor.x) > 0.01f)
        {
            float _m = transform.position.x - targetVertor.x;

            //设置状态
            if (actionStep != 1)
            {
                //设置动作
                if (_m > 0)
                    AniController.Get(transform).PlayAniCanBreak(skin.ActionList["left"][0], skin.ActionList["left"][1], AniController.AniType.LoopBack, 10);
                else if (_m < 0)
                    AniController.Get(transform).PlayAniCanBreak(skin.ActionList["right"][0], skin.ActionList["right"][1], AniController.AniType.LoopBack, 10);

                //重置动画播放百分比
                actiontimer = 0;
                //改变状态
                actionStep = 1;
            }

            //移动
            actiontimer += Time.deltaTime;
            float _totaltime = Mathf.Abs((startVertor.x - targetVertor.x)) / speed;
            float movedis = Mathf.Lerp(startVertor.x, targetVertor.x, actiontimer / _totaltime);
            transform.position = new Vector3(movedis, transform.position.y, transform.position.z);
            /*
            if (_m > 0)
            {
                transform.Translate(new Vector3(-speed * Time.deltaTime, 0, 0));
            }
            else
            {
                transform.Translate(new Vector3(speed * Time.deltaTime, 0, 0));
            }
             */
        }
        //竖向
        else
        {
            float _m = transform.position.y - targetVertor.y;

            //设置状态
            if (actionStep != 2)
            {
                //设置x轴位置
                if (transform.position.x != targetVertor.x)
                    transform.position = new Vector2(targetVertor.x, transform.position.y);
                //设置动作
                if (_m > 0)
                    AniController.Get(transform).PlayAniCanBreak(skin.ActionList["down"][0], skin.ActionList["down"][1], AniController.AniType.LoopBack, 10);
                else if (_m < 0)
                    AniController.Get(transform).PlayAniCanBreak(skin.ActionList["up"][0], skin.ActionList["up"][1], AniController.AniType.LoopBack, 10);
                
                //重置动画播放百分比
                actiontimer = 0;
                //改变状态
                actionStep = 2;
            }

            //移动
            actiontimer += Time.deltaTime;
            float _totaltime = Mathf.Abs((startVertor.y - targetVertor.y)) / speed;
            float movedis = Mathf.Lerp(startVertor.y, targetVertor.y, actiontimer / _totaltime);
            transform.position = new Vector3(transform.position.x, movedis, transform.position.z);

            /*
            if (_m > 0)
            {
                transform.Translate(new Vector3(0, -speed * Time.deltaTime, 0));
            }
            else
            {
                transform.Translate(new Vector3(0, speed * Time.deltaTime, 0));
            }
             */
        }

        //如果到达终点
        if (Mathf.Abs(transform.position.y - targetVertor.y) < 0.01f && actionStep == 2)
        {
            //设置y轴位置
            if (transform.position.y != targetVertor.y)
                transform.position = new Vector2(transform.position.x, targetVertor.y);

            actionStep = 3;
        }
    }

    public void SetTarget(string id)
    {
        targetPoint = id;
        startVertor = transform.position;
        if (movePoint.ContainsKey(targetPoint))
            targetVertor = movePoint[targetPoint];
        else
            return;

        actionStep = 0;
    }

    public void SetTargetByVector(Vector2 vec)
    {
        targetVertor = vec;
        startVertor = transform.position;
        actionStep = 0;
    }


    public void Init(CharacterModle.CharacterInfo _info, CharacterModle.Skin _skin)
    {
        targetPoint = null;

        actionStep = 0;
        info = _info;
        skin = _skin;

        Transform root = transform.Find("/Scene/road");
        foreach (Transform t in root)
        {
            movePoint[t.name] = t.position;
        }
    }

    static public CharacterController Get(GameObject go)
    {
        CharacterController controller = go.GetComponent<CharacterController>();
        if (controller == null)
        {
            controller = go.AddComponent<CharacterController>();
        }
        return controller;
    }

    Vector2 SetNext(string target, string now)
    {
        int _t = int.Parse(target.Substring(4));
        int _n = int.Parse(now);

        if (_n > _t)
        {
            return movePoint["move" + (_n - 1).ToString()];
        }
        else if (_n < _t)
        {
            return movePoint["move" + (_n + 1).ToString()];
        }
        else
        {
            return movePoint[_n.ToString()];
        }
    }

    void OnTriggerStay2D(Collider2D coll)
    {
        if (coll.transform.name == "move3" && characterstate == CharaState.Think)
        {
            if (istrigger != true)
            {
                SellectGoods();
                istrigger = true;
            }
        }
        else if ((coll.transform.name == "move4" || coll.transform.name == "move5" || coll.transform.name == "move6") && actionStep == 3)
        {
            if (istrigger != true)
            {
                //selectgoods
                Vector2 _tver = new Vector2(goodsVertor.x, goodsVertor.y + 0.3f);

                //改变寻路方式，然后设定终点
                //moveType = MoveType.Vertical;
                SetTargetByVector(_tver);

                ChangeCharaState(CharaState.Buy);
                istrigger = true;
            }
        }
        else if (coll.transform.name == "move7" && actionStep == 3 && characterstate == CharaState.Check)
        {
            BuyingGoods();
        }
        else if (coll.transform.name == "finish1" || coll.transform.name == "finish2")
        {
            Destroy(this.gameObject);
        }

        //设定层级
        if (coll.transform.name == "move3")
        {
            transform.GetComponent<SpriteRenderer>().sortingOrder = 0;
        }
        else if (coll.transform.name == "move4" && characterstate != CharaState.Out)
        {
            transform.GetComponent<SpriteRenderer>().sortingOrder = 1;
        }
        else if (coll.transform.name == "move5" && characterstate != CharaState.Out)
        {
            transform.GetComponent<SpriteRenderer>().sortingOrder = 2;
        }
        else if (coll.transform.name == "move6" && characterstate != CharaState.Out)
        {
            transform.GetComponent<SpriteRenderer>().sortingOrder = 4;
        }
    }

    void OnTriggerExit2D(Collider2D coll)
    {
        istrigger = false;
    }


    public class ShopGood
    {
        public CharBag.Goods goods = new CharBag.Goods();
        public int slotID = -1;
    }

    void SellectGoods()
    {
        CharBag.Goods[] goodsList = transform.Find("/goodBox").GetComponent<ShopUI>().GetGoods();

        ShopGood[] selectList = new ShopGood[18];
        int count = 0;

        //筛选出有商品的货架
        for (int i = 0; i < goodsList.Length; i++)
        {
            if (goodsList[i].Name != null)
            {
                selectList[count] = new ShopGood();
                selectList[count].slotID = i + 1;
                selectList[count].goods = goodsList[i];
                count++;
            }
        }

        //挑选商品
        if (count != 0)
        {
            //TODO：选择商品
            int slot = Random.Range(1, count + 1);
            selectgoods = selectList[slot - 1];
            if (selectgoods.slotID >= 1 && selectgoods.slotID <= 6)
            {
                SetTarget("move6");
            }
            else if (selectgoods.slotID >= 7 && selectgoods.slotID <= 12)
            {
                SetTarget("move5");
            }
            else
            {
                SetTarget("move4");
            }
            ChangeCharaState(CharaState.In);
            goodsVertor = transform.Find("/goodBox/" + selectgoods.slotID.ToString()).transform.position;
        }
        else
        {
            //if (info.startPiont == 0)
            //    SetTarget("finish2");
            //else
            //    SetTarget("finish1");

            ChangeCharaState(CharaState.Out);
        }
    }

    bool isonhead = false;
    void GetingGoods()
    {
        if (!isonhead)
        {
            AddGoodsToHead();
            isonhead = !isonhead;
        }

        TimeTool.SetWaitTime(0.5f, gameObject, () =>
        {
            moveType = MoveType.Horizontal;
            SetTarget("move7");
            ChangeCharaState(CharaState.Check);
        });

    }

    void AddGoodsToHead()
    {
        GameObject goods = new GameObject();
        SpriteRenderer _sprite = goods.AddComponent<SpriteRenderer>();

        ShopUI _shop = transform.Find("/goodBox").GetComponent<ShopUI>();
        CharBag.Goods[] goodslist = _shop.GetGoods();

        //如果货架上有商品，则添加道具到背包
        if (goodslist[selectgoods.slotID - 1].Name != null && goodslist[selectgoods.slotID - 1].ID != 0)
        {
            //移除道具
            _shop.BuyedGoods(selectgoods);
        }

        _sprite.sprite = Materiral.GetMaterialIcon(selectgoods.goods.MateriralType, selectgoods.goods.ID);
        _sprite.sortingLayerName = "Character";

        //设定道具的为角色的子物件
        goods.transform.SetParent(this.transform);
        goods.transform.position = goodsVertor;

        //把物体移动到头上
        LeanTween.move(goods.gameObject, new Vector2(goodsVertor.x, goodsVertor.y + 0.5f), 0.5f);
    }

    bool isoutcoin = false;
    void BuyingGoods()
    {
        if (!isoutcoin)
        {
            FlyCoin();

            //增加钱
            ShopUI _shop = transform.Find("/goodBox").GetComponent<ShopUI>();
            _shop.AddMoney(selectgoods.goods.Price);

            isoutcoin = !isoutcoin;
        }

        TimeTool.SetWaitTime(0.5f, gameObject, () =>
        {
            //买东西
            moveType = MoveType.Vertical;
            if (info.startPiont == 0)
                SetTarget("finish2");
            else
                SetTarget("finish1");

            ChangeCharaState(CharaState.Out);
        });
    }


    void FlyCoin()
    {
        GameObject obj = new GameObject();
        SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
        sr.sprite = Materiral.GetIconByName("actionIcon_6");
        sr.sortingLayerName = "Character";

        Vector3 startpos = transform.position;
        float randomlength = Random.Range(-0.4f, 0.4f);
        Vector3 endpos = new Vector3(startpos.x + randomlength, Random.Range(-4.3f, -4.47f), 0);

        float randomheight = Random.Range(0.3f, 0.6f);
        float time = Mathf.Abs(randomheight);
        Debug.Log(randomheight);
        Vector3[] _path = new Vector3[] { startpos, startpos, new Vector3(startpos.x + randomlength / 2, randomheight + startpos.y, startpos.z), endpos, endpos };
        LeanTween.moveSpline(obj, _path, time).setOnComplete(() =>
        {
            LeanTween.moveY(obj, randomheight / 5 + startpos.y, time / 5).setOnComplete(() =>
            {
                LeanTween.moveY(obj, endpos.y, time / 5).setEase(LeanTweenType.easeOutBounce);
            });
        });

        TimeTool.SetWaitTime(5f, obj, () =>
        {
            LeanTween.alpha(obj, 0, 1).setDestroyOnComplete(true);
        });
    }
}
