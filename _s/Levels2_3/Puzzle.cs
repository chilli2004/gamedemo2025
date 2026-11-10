using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class Puzzle : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler
{
    public Levels2_3 _Levels2;

    private RectTransform _rt;
    private Canvas _canvas;
    public string _name;
    public bool _pbd;
    public bool _isPuzzle;
    void Awake()
    {
        _rt = GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_Levels2 != null)
        {
            _Levels2._puz = gameObject;
            _pbd = false;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_Levels2 != null)
        {
            _pbd = true;
            _Levels2._puz = gameObject;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_rt == null) return;
        float scale = _canvas != null ? _canvas.scaleFactor : 1f;
        _rt.anchoredPosition += eventData.delta / scale;
        _pbd = true;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (_pbd == false)
        {
            if (other.gameObject.name == _name)
            {
                
                var selfRT = GetComponent<RectTransform>();
                var otherRT = other.GetComponent<RectTransform>();

                float selfZ = selfRT != null ? selfRT.localEulerAngles.z : transform.localEulerAngles.z;
                float otherZ = otherRT != null ? otherRT.localEulerAngles.z : other.transform.localEulerAngles.z;

                // 使用 DeltaAngle 处理 0°/360° 等价情况，并给一个小容差
                bool sameZ = Mathf.Abs(Mathf.DeltaAngle(selfZ, otherZ)) < 0.1f;

                if (sameZ)
                {
                    _isPuzzle = true;
                    var canvas = GetComponentInParent<Canvas>();
                    var cam = canvas != null ? canvas.worldCamera : null; // Overlay 模式下 cam 可为 null
                    Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(cam, otherRT.position);

                    RectTransform parentRT = selfRT.parent as RectTransform;
                    Vector2 localPos;
                    if (RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRT, screenPos, cam,
                            out localPos))
                    {
                        selfRT.anchoredPosition = localPos;
                    }
                }

            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.name == _name)
        {
            _isPuzzle = false;
        }
    }
}