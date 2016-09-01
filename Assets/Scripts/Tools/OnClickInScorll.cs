using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class OnClickInScorll : MonoBehaviour, IPointerClickHandler,IPointerDownHandler, IPointerUpHandler
{
    public delegate void VoidDelegate(GameObject go);
    public VoidDelegate onClick;
    public VoidDelegate onDown;
    public VoidDelegate onUp;
    public VoidDelegate onHold;


    public delegate void ParameterDelegate(GameObject go, object obj);
    public ParameterDelegate onClickByParameter;
    public ParameterDelegate onDownByParameter;
    public ParameterDelegate onUpByParameter;
    public ParameterDelegate onHoldByParameter;

    public object parameter;

    bool ishold = false;
    bool isholded = false;      //用于解决长按和点击时的冲突
    float holdTime = 0;
    float checkTime = 1.0f;


    void Update()
    {
        if (ishold)
        {
            holdTime += Time.deltaTime;
        }

        if (holdTime >= checkTime)
        {
            OnPointerHold();
        }
    }



    static public OnClickInScorll Get(Transform transform)
    {
        OnClickInScorll listener = transform.GetComponent<OnClickInScorll>();
        if (listener == null) listener = transform.gameObject.AddComponent<OnClickInScorll>();
        return listener;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isholded == true)
            return;

        if (onClick != null || onClickByParameter != null)
        {
            if (parameter == null)
                onClick(gameObject);
            else
            {
                onClickByParameter(gameObject, parameter);
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        ishold = true;
        if (onDown != null || onDownByParameter != null)
        {
            if (parameter == null)
                onDown(gameObject);
            else
            {
                onDownByParameter(gameObject, parameter);
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        ishold = false;
        holdTime = 0;
        if (isholded == true)
            StartCoroutine(WaitToReset());

        if (onUp != null || onUpByParameter != null)
        {
            if (parameter == null)
                onUp(gameObject);
            else
            {
                onUpByParameter(gameObject, parameter);
            }
        }
    }

    public void OnPointerHold()
    {
        ResetHoldTime();

        if (onHold != null || onHoldByParameter != null)
        {
            if (parameter == null)
                onHold(gameObject);
            else
            {
                onHoldByParameter(gameObject, parameter);
            }
        }
    }

    void ResetHoldTime()
    {
        ishold = false;
        isholded = true;
        holdTime = 0;
    }

    IEnumerator WaitToReset()
    {
        float startime = 0;
        while (true)
        {
            startime += Time.deltaTime;

            if (startime > 0.05f)
            {
                isholded = false;
                break;
            }

            yield return null;
        }
    }

}
