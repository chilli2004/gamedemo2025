using UnityEngine;

public class JudgmentBit : MonoBehaviour
{
    public GameObject _ga;

    public void An()
    {
        if (_ga==null)
        {
            // 获取当前物体的父物体
            Transform parentTransform = transform.parent;
            _ga = parentTransform.GetChild(0).gameObject;
        }
        _ga.SetActive(true);
        gameObject.SetActive(false);

    }
}
