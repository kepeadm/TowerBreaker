using UnityEngine;
using UnityEngine.UI;

public class UI_SpriteAnimator : MonoBehaviour
{
    [Header("스프라이트 프레임")]
    [SerializeField] private Sprite[] frames;

    [Header("재생 설정")]
    [SerializeField] private float fps = 12f;
    [SerializeField] private bool loop = true;
    [SerializeField] private bool playOnAwake = true;

    private Image _image;
    private float _timer;
    private int _currentFrame;
    private bool _isPlaying;

    void Awake()
    {
        _image = GetComponent<Image>();

        if (playOnAwake) Play();
    }

    void Update()
    {
        if (!_isPlaying || frames == null || frames.Length == 0) return;

        _timer += Time.deltaTime;

        if (_timer >= 1f / fps)
        {
            _timer = 0f;
            _currentFrame++;

            if (_currentFrame >= frames.Length)
            {
                if (loop)
                    _currentFrame = 0;
                else
                {
                    _currentFrame = frames.Length - 1;
                    Stop();
                    return;
                }
            }

            _image.sprite = frames[_currentFrame];
        }
    }

    public void Play()
    {
        if (frames == null || frames.Length == 0) return;
        _currentFrame = 0;
        _timer = 0f;
        _isPlaying = true;
        _image.sprite = frames[0];
    }

    public void Stop()
    {
        _isPlaying = false;
    }

    public void Pause()
    {
        _isPlaying = false;
    }

    public void Resume()
    {
        _isPlaying = true;
    }
}