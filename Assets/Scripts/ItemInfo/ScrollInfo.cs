using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class ScrollInfo : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    public ScrollRect scrollrect;

    float[] pages;
    int currentPageIndex = 0;

    //滑动速度
    public float smooting = 4;

    //滑动的起始坐标
    float targethorizontal = 0;

    //是否拖拽结束
    bool isDrag = false;

    /// <summary>
    /// 用于返回一个页码，-1说明page的数据为0
    /// </summary>
    public System.Action<int, int> OnPageChanged;

    //float startime = 0f;
    //float delay = 0.1f;

    // Use this for initialization
    void Start()
    {
        //适配rect的大小
        float fixwidth = scrollrect.GetComponent<RectTransform>().rect.width;

        foreach (Transform element in scrollrect.transform.FindChild("PageList"))
        {
            element.GetComponent<LayoutElement>().preferredWidth = fixwidth;
        }
        
        //添加页面位置进入page列表中
        int count = scrollrect.content.childCount;
        pages = new float[count];
        for (int i = 0; i < count; i++)
        {
            float page = 0;
            if (count != 1)
                page = i / ((float)(count - 1));
            pages[i] = page;
        }

        OnPageChanged(pages.Length, currentPageIndex);
    }

    void Update()
    {
        if (!isDrag && pages.Length > 0)
        {
            scrollrect.horizontalNormalizedPosition = Mathf.Lerp(scrollrect.horizontalNormalizedPosition, targethorizontal, Time.deltaTime * smooting);
        }
    }

    private Vector2 pressPoint = new Vector3();
    public void OnBeginDrag(PointerEventData eventData)
    {
        isDrag = true;
        pressPoint = eventData.position;

        StartCoroutine(TimeTool.StartTiming());
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDrag = false;
        float _st = TimeTool.EndTiming();

        float posX = scrollrect.horizontalNormalizedPosition;
        int index = 0;
        //当滑动时间长时，使用该方法
        if (_st > 0.25f)
        {
            float offset = Mathf.Abs(pages[index] - posX);
            for (int i = 0; i < pages.Length; i++)
            {
                float temp = Mathf.Abs(pages[i] - posX);
                if (temp < offset)
                {
                    index = i;
                    offset = temp;
                }
            }
        }
        //当滑动时间短时，使用该方法
        else
        {
            float offset = pressPoint.x - eventData.position.x;

            //往左滑
            if (offset < 0)
            {
                if (currentPageIndex == 0)
                    index = currentPageIndex;
                else
                    index = currentPageIndex - 1;
            }
            //往右滑
            else
            {
                if (currentPageIndex == pages.Length - 1)
                    index = currentPageIndex;
                else
                    index = currentPageIndex + 1;
            }
        }

        if (index != currentPageIndex)
        {
            currentPageIndex = index;
            OnPageChanged(pages.Length, currentPageIndex);
        }

        targethorizontal = pages[index];
    }
}
