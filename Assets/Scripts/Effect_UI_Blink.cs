using UnityEngine;
using UnityEngine.UI;

public class Effect_UI_Blink : MonoBehaviour
{
    [SerializeField] private Image targetImage;
    
    [Header("Blink Settings")]
    [SerializeField] private float blinkSpeed = 1.5f;
    [SerializeField] private float minAlpha   = 0.1f;
    [SerializeField] private float maxAlpha   = 1.0f;

    private void Awake()
    {
        if (targetImage == null)
            targetImage = GetComponent<Image>();
    }

    private void Update()
    {
        float alpha = Mathf.Lerp(minAlpha, maxAlpha,
                          Mathf.PingPong(Time.time * blinkSpeed, 1f));

        Color c = targetImage.color;
        c.a = alpha;
        targetImage.color = c;
    }
}