using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PssingAn : MonoBehaviour
{
    private static readonly Dictionary<int, Sprite> SpriteCache = new Dictionary<int, Sprite>();
    [Tooltip("多久时间内完整播放一次（秒）")]
    [Min(0.01f)]
    public float loopDurationSeconds = 1f;

    [Header("素材")]
    public Image Back;
    [Tooltip("如已在编辑器里准备好的序列帧，可直接填充此数组")]
    public Sprite[] prebuiltSprites;
    [Tooltip("需要在运行时从纹理生成的帧")]
    public Texture2D[] back1;

    [Header("预热设置")]
    public bool asyncWarmup = true;
    [Min(1)]
    public int texturesPerFrame = 4;

    public PssingOvera _pss;

    private Sprite[] _sprites;
    private int _currentIndex;
    private float _timer;
    private float _frameDuration = 1f / 60f;
    private bool _isReady;

    public int AUDIO = -1;
    public AudioSource _Audio;

    public int g1 = -1;
    public int g2 = -1;
    public int g3;

    private bool _audioTriggered;
    private bool _g1Triggered;
    private bool _g2Triggered;

    private void Awake()
    {
        RecalculateFrameDuration();
    }

    private void OnValidate()
    {
        RecalculateFrameDuration();
        texturesPerFrame = Mathf.Max(1, texturesPerFrame);
    }

    private IEnumerator Start()
    {
        yield return InitializeRoutine();
    }

    private void Update()
    {
        if (!_isReady || _sprites == null || _sprites.Length == 0)
        {
            return;
        }

        _timer += Time.deltaTime;
        while (_timer >= _frameDuration)
        {
            _timer -= _frameDuration;
            AdvanceFrame();
        }
    }

    private void AdvanceFrame()
    {
        var nextIndex = _currentIndex + 1;
        var wrapped = false;

        if (nextIndex >= _sprites.Length)
        {
            nextIndex = 0;
            wrapped = true;
        }

        _currentIndex = nextIndex;
        Back.overrideSprite = _sprites[_currentIndex];

        if (wrapped)
        {
            ResetLoopTriggers();
        }

        HandleFrameEvents();
    }

    private void HandleFrameEvents()
    {
        if (!_audioTriggered && AUDIO == _currentIndex && _Audio != null)
        {
            _Audio.Play();
            _audioTriggered = true;
        }

        if (!_g1Triggered && g1 == _currentIndex && _pss != null)
        {
            _pss.G1();
            _g1Triggered = true;
        }

        if (!_g2Triggered && g2 == _currentIndex && _pss != null)
        {
            _pss.G2();
            _g2Triggered = true;
        }
    }

    private void ResetLoopTriggers()
    {
        _audioTriggered = false;
        _g1Triggered = false;
        _g2Triggered = false;
    }

    private void RecalculateFrameDuration()
    {
        if (loopDurationSeconds <= 0f)
        {
            loopDurationSeconds = 0.01f;
        }

        if (_sprites != null && _sprites.Length > 0)
        {
            _frameDuration = Mathf.Max(loopDurationSeconds / _sprites.Length, Mathf.Epsilon);
        }
        else
        {
            _frameDuration = loopDurationSeconds;
        }
    }

    private IEnumerator InitializeRoutine()
    {
        if (Back == null)
        {
            enabled = false;
            yield break;
        }

        if (!TryUsePrebuiltSprites())
        {
            if (asyncWarmup)
            {
                yield return BuildSpritesAsync();
            }
            else
            {
                BuildSpritesImmediate();
            }
        }

        if (_sprites == null || _sprites.Length == 0)
        {
            enabled = false;
            yield break;
        }

        FinalizeSetup();
    }

    private void FinalizeSetup()
    {
        g3 = _sprites.Length - 1;
        _currentIndex = 0;
        _timer = 0f;
        _audioTriggered = false;
        _g1Triggered = false;
        _g2Triggered = false;

        Back.overrideSprite = _sprites[_currentIndex];
        HandleFrameEvents();
        _isReady = true;
    }

    private bool TryUsePrebuiltSprites()
    {
        if (prebuiltSprites == null || prebuiltSprites.Length == 0)
        {
            return false;
        }

        var frames = new List<Sprite>(prebuiltSprites.Length);
        for (int i = 0; i < prebuiltSprites.Length; i++)
        {
            var sprite = prebuiltSprites[i];
            if (sprite != null)
            {
                frames.Add(sprite);
            }
        }

        if (frames.Count == 0)
        {
            return false;
        }

        _sprites = frames.ToArray();
        RecalculateFrameDuration();
        return true;
    }

    private void BuildSpritesImmediate()
    {
        _sprites = CreateSprites(back1);
        RecalculateFrameDuration();
    }

    private IEnumerator BuildSpritesAsync()
    {
        if (back1 == null || back1.Length == 0)
        {
            _sprites = null;
            yield break;
        }

        var frames = new List<Sprite>(back1.Length);
        int batchCount = Mathf.Max(1, texturesPerFrame);
        int processed = 0;

        for (int i = 0; i < back1.Length; i++)
        {
            var tex = back1[i];
            if (tex != null)
            {
                frames.Add(GetOrCreateSprite(tex));
            }

            processed++;
            if (processed >= batchCount)
            {
                processed = 0;
                yield return null;
            }
        }

        _sprites = frames.ToArray();
        RecalculateFrameDuration();
    }

    private Sprite[] CreateSprites(Texture2D[] textures)
    {
        if (textures == null || textures.Length == 0)
        {
            return null;
        }

        var frames = new List<Sprite>(textures.Length);
        for (int i = 0; i < textures.Length; i++)
        {
            var tex = textures[i];
            if (tex != null)
            {
                frames.Add(GetOrCreateSprite(tex));
            }
        }

        return frames.Count > 0 ? frames.ToArray() : null;
    }

    private static Sprite GetOrCreateSprite(Texture2D tex)
    {
        var key = tex.GetInstanceID();
        if (SpriteCache.TryGetValue(key, out var cachedSprite) && cachedSprite != null)
        {
            return cachedSprite;
        }

        var sprite = Sprite.Create(
            tex,
            new Rect(0, 0, tex.width, tex.height),
            new Vector2(0.5f, 0.5f));
        SpriteCache[key] = sprite;
        return sprite;
    }
}
