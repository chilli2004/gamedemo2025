using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class Levels1_1 : MonoBehaviour
{
    public Image Back1;
    public Texture2D[] back11;

    public Image People;
    public Texture2D[] people1;

    public Image Convers;
    public Texture2D[] convers1;
    
    public string[] _st;
    
    public TextMeshProUGUI _tex;
    
    // 配置
    public float charsPerSecond = 30f;

    // 状态
    private int _index = 0;
    private bool _isTyping = false;
    private Coroutine _typingCo;
    private bool b;
    public bool _b;
    void Start()
    {
        ShowLine(0);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (_isTyping)
            {
                SkipTyping();
            }
            else
            {
                NextLine();
            }
        }
    }

    private void ShowLine(int idx)
    {
        if (_st == null || _st.Length == 0) return;
        _index = Mathf.Clamp(idx, 0, _st.Length - 1);

        // 切图（若有）
        TrySetImage(Back1, back11, _index);
        TrySetImage(People, people1, _index);
        TrySetImage(Convers, convers1, _index);

        // 开始打字
        if (_typingCo != null) StopCoroutine(_typingCo);
        _typingCo = StartCoroutine(TypeLine(_st[_index]));
    }

    private void NextLine()
    {
        if (_st == null || _st.Length == 0) return;

        int next = _index + 1;
        if (next >= _st.Length)
        {
            if (b==false)
            {
                if (_b)
                {
#if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
#else
Application.Quit();
#endif
                }
                PssingOvera();
                b = true;
            }
            
            // 已到最后一条，可在这里触发结束逻辑
            return;
        }
        ShowLine(next);
    }

    private void SkipTyping()
    {
        _isTyping = false;
        if (_typingCo != null)
        {
            StopCoroutine(_typingCo);
            _typingCo = null;
        }
        _tex.text = _st[_index]; // 直接补全
    }

    private IEnumerator TypeLine(string fullText)
    {
        _isTyping = true;
        _tex.text = "";
        if (string.IsNullOrEmpty(fullText))
        {
            _isTyping = false;
            yield break;
        }

        float tPerChar = charsPerSecond > 0f ? 1f / charsPerSecond : 0f;
        for (int i = 0; i < fullText.Length; i++)
        {
            if (!_isTyping) yield break;
            _tex.text = fullText.Substring(0, i + 1);
            if (tPerChar > 0f)
                yield return new WaitForSeconds(tPerChar);
            else
                yield return null;
        }

        _isTyping = false;
        _typingCo = null;
    }

    private void TrySetImage(Image img, Texture2D[] texArr, int idx)
    {
        if (img == null || texArr == null || texArr.Length == 0) return;
        if (idx < 0 || idx >= texArr.Length) return;
        var tex = texArr[idx];
        if (tex == null)
        {
            img.gameObject.SetActive(false);
            return;
        }
        else
        {
            img.gameObject.SetActive(true);
        }

        var rt = img.rectTransform;
        var pos = rt.anchoredPosition;
        var size = rt.sizeDelta;
        var scale = rt.localScale;

        var spr = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100f);
        img.sprite = spr;
        //img.preserveAspect = true; // 可选：保持比例，仍不改位置

        // 还原位置/大小/缩放
        //rt.anchoredPosition = pos;
        //rt.sizeDelta = size;
        //rt.localScale = scale;
    }
    public GameObject canvas;
    public GameObject pssingOvera;
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
        
            pssingInstance.SetActive(true);
            pssingInstance.transform.GetChild(1).GetComponent<PssingOvera>()._ga1 = gameObject;
        }
    }
}