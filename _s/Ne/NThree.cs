using UnityEngine;

public class NThree : MonoBehaviour
{
    public GameObject canvas;
    public GameObject pssingOvera;
    public GameObject pssingOvera1;
    public bool l2;
    public bool l2_1;
    private void Start()
    {
        //PssingOvera();
    }

    public void PssingOvera(){
        if (pssingOvera != null && canvas != null)
        {
            // 获取 pssingOvera 的世界位置
            RectTransform pssingRect = pssingOvera.GetComponent<RectTransform>();
            Vector3 worldPosition = Vector3.zero;
            if (pssingRect != null)
            {
                worldPosition = pssingRect.position;
            }
        
            // 生成 pssingOvera，先作为 canvas 的子物品
            GameObject pssingInstance = Instantiate(pssingOvera, canvas.transform);
        
            // 获取实例的 RectTransform
            RectTransform instanceRect = pssingInstance.GetComponent<RectTransform>();
            if (instanceRect != null && canvas != null)
            {
                // 将世界坐标转换为 canvas 的本地坐标
                instanceRect.position = worldPosition;
            }
            if (l2_1)
            {
                pssingInstance.transform.GetChild(1).GetComponent<PssingOvera>()._ga1 = pssingOvera1;
            }
            pssingInstance.SetActive(true);
            
            if (l2)
            {
                Destroy(transform.parent.gameObject);
            }

            
           
        }
    }
}
