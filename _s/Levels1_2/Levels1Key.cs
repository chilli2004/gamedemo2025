using TMPro;
using UnityEngine;
using System.Collections;

public class Levels1Key : MonoBehaviour
{
    public float[] AppTime;

    public float[] JudTime;

    public float[] DurTime;

    public float[] Type;

    public GameObject _tr;
    
    public GameObject _notes;
    
    public GameObject[] ShortNote;

    public GameObject LongNote;
    
    public TextMeshProUGUI _L;

    public TextMeshProUGUI _R;

    private int b = Animator.StringToHash("b");

    private float targetY = -419f;
    private float targetY1 = -110.48f;
    private float longNoteEndY = -780f;
    private float noteScale = 1f;

    public bool isShortNote; // 是否是短音符
    public bool isLongNote; // 是否是短音符
    public float longNotePressDuration = 0f; // 长按累计时间
    private bool isBeingHeld = false; // 是否正在被按住
    private GameObject _ga;
    Animator animator;
     // 可配置的按键
    public KeyCode targetKey = KeyCode.A; // 默认为A键，可在Inspector中修改
    void Start()
    {
        StartCoroutine(SpawnNotes());
    }

    void Update()
    {
        // 检测按键输入
        if (isShortNote)
        {
            // 短音符：按 A 键立即触发
            if (Input.GetKeyDown(targetKey))
            {
                // 这里可以触发得分或其他逻辑
                Debug.Log("短音符被按下！");
                _ga.GetComponent<ShortNote>().Ga();
                string l1 = _L.text;
                int l11 = int.Parse(l1);
                l11 += 100;
                _L.text = l11.ToString();
                string r1 = _R.text;
                int r11 = int.Parse(r1);
                r11 += 100;
                _R.text = r11.ToString();
            }
        }
        if (isLongNote)
        {
            // 长音符：长按 A 键
            if (Input.GetKey(targetKey))
            {
                Debug.Log("开始长按");
                animator.SetBool("b", true);
                string l1 = _L.text;
                int ll11 = int.Parse(l1);
                ll11 += 5;
                _L.text = ll11.ToString();
                string rr1 = _R.text;
                int rr11 = int.Parse(rr1);
                rr11 += 5;
                _R.text = rr11.ToString();
            }
        }
    }
    
    void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag == "Short"&&_ga==null)
        {
            isShortNote = true;
            _ga = other.gameObject;
            //animator = other.gameObject.GetComponent<Animator>();
        }

        if (other.tag == "Long"&&_ga==null)
        {
            isLongNote = true;
            _ga = other.gameObject;
            animator = other.gameObject.GetComponent<Animator>();
        }
    }
    
    void OnTriggerExit2D(Collider2D other)
    {
        isShortNote = false;
        isLongNote = false;
    }

    IEnumerator SpawnNotes()
    {
        // 确保数组长度一致
        int count = AppTime.Length;
        
        float timer = 0f;
        int currentIndex = 0;
        
        while (currentIndex < count)
        {
            // 每帧累加时间
            timer += Time.deltaTime;
            
            // 检查是否到达下一个生成时间
            if (timer >= AppTime[currentIndex])
            {
                // 生成音符
                GameObject noteToSpawn;
                if (Type[currentIndex] == 0f)
                {
                    // 短音符 - 随机选择一个ShortNote
                    int randomIndex = Random.Range(0, ShortNote.Length);
                    noteToSpawn = ShortNote[randomIndex];
                }
                else
                {
                    // 长音符
                    noteToSpawn = LongNote;
                }
                
                // 实例化音符
                GameObject note = Instantiate(noteToSpawn, _tr.transform.position, _tr.transform.rotation, _notes.transform);
                note.transform.localScale = Vector3.one * noteScale;
                note.SetActive(true);
                // 启动下落协程
                StartCoroutine(MoveNoteToTarget(note, currentIndex));
                
                currentIndex++;
            }
            
            yield return null; // 等待下一帧
        }
    }

    IEnumerator MoveNoteToTarget(GameObject note, int index)
    {
        RectTransform rectTransform = note.GetComponent<RectTransform>();
        if (rectTransform == null)
            yield break;
            
        Vector2 startPos = rectTransform.anchoredPosition;
        
        
        // 短音符：直接移动到目标位置
        if (Type[index] == 0f)
        {
            Vector2 targetPos = new Vector2(startPos.x, targetY);
            float elapsed = 0f;
            while (elapsed < JudTime[index])
            {
                // 检查对象是否已销毁
                if (note == null || rectTransform == null)
                    yield break;
                    
                elapsed += Time.deltaTime;
                float t = elapsed / JudTime[index];
                rectTransform.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
                yield return null;
            }
            if (note != null && rectTransform != null)
                rectTransform.anchoredPosition = targetPos;
            // 第二阶段：继续移动到 -196
            Vector2 endPos = new Vector2(startPos.x, longNoteEndY);
            elapsed = 0f;
            while (elapsed < 1.15f)
            {
                // 检查对象是否已销毁
                if (note == null || rectTransform == null)
                    yield break;
                    
                elapsed += Time.deltaTime;
                float t = elapsed / 1.15f;
                t = Mathf.Clamp01(t); // 确保t在0到1之间
                rectTransform.anchoredPosition = Vector2.Lerp(targetPos, endPos, t);
                yield return null;
            }
            if (note != null && rectTransform != null)
                rectTransform.anchoredPosition = endPos;
        }
        // 长音符：先到-97，再继续到-196
        else
        {
            Vector2 targetPos = new Vector2(startPos.x, targetY1);
            // 第一阶段：移动到 -97
            float elapsed = 0f;
            while (elapsed < JudTime[index])
            {
                // 检查对象是否已销毁
                if (note == null || rectTransform == null)
                    yield break;
                    
                elapsed += Time.deltaTime;
                float t = elapsed / JudTime[index];
                rectTransform.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
                yield return null;
            }
            if (note != null && rectTransform != null)
                rectTransform.anchoredPosition = targetPos;
            
            // 第二阶段：继续移动到 -196
            Vector2 endPos = new Vector2(startPos.x, longNoteEndY);
            elapsed = 0f;
            while (elapsed < DurTime[index])
            {
                // 检查对象是否已销毁
                if (note == null || rectTransform == null)
                    yield break;
                    
                elapsed += Time.deltaTime;
                float t = elapsed / DurTime[index];
                t = Mathf.Clamp01(t); // 确保t在0到1之间
                rectTransform.anchoredPosition = Vector2.Lerp(targetPos, endPos, t);
                yield return null;
            }
            if (note != null && rectTransform != null)
                rectTransform.anchoredPosition = endPos;
        }
    }
}
