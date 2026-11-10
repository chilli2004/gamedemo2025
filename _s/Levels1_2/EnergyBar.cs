using UnityEngine;
using System.Collections;

public class EnergyBar : MonoBehaviour
{
    private RectTransform rectTransform;
    private Vector2 startPosition;
    private Vector2 targetPosition;
    public float duration = 36f;
    private bool isMoving = false;

    public bool b;
    public bool b1;
    public NThree _n;

    public GameObject win;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            startPosition = rectTransform.anchoredPosition;
            targetPosition = new Vector2(0f, startPosition.y);
            StartCoroutine(MoveToTarget());
        }
    }

    IEnumerator MoveToTarget()
    {
        isMoving = true;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            // 使用线性插值移动
            rectTransform.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, t);
            
            yield return null;
        }
        
        // 确保最终位置准确
        rectTransform.anchoredPosition = targetPosition;
        isMoving = false;
        if (b&&b1==false)
        {
            win.SetActive(true);
        }
        else if (b&&b1)
        {
            _n.PssingOvera();
        }
    }
}
