using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScreenFader : MonoBehaviour
{
    [Header("Screen Fade Settings")]
    [SerializeField] private Image fadeImage; // CanvasGroup을 사용하여 투명도를 제어
    [SerializeField] private float fadeSpeed = 1.5f; // 페이드 속도
    [Tooltip("페이드 인 딜레이")]
    [SerializeField] private float fadeInDelay = 0.5f; // 페이드 인 딜레이

    private Coroutine currentFadeCoroutine;

    public void Start()
    {
        if (fadeImage == null)
        {
            Debug.LogError("ScreenFader: fadeImage가 할당되지 않았습니다. Inspector에서 Image를 연결하세요.");
            enabled = false;
            return;
        }

        // 시작 시점에 투명(Alpha=0) 상태로 세팅
        Color c = fadeImage.color;
        c.a = 0f;
        fadeImage.color = c;
        fadeImage.gameObject.SetActive(true);
    }

    /// <summary>
    /// 페이드 아웃 후 사다리 이동을 실행하는 코루틴
    /// </summary>
    public IEnumerator FadeOut_Move_FadeIn(System.Action moveAction)
    {

        // 1) 페이드 아웃 실행
        yield return StartCoroutine(FadeOutCoroutine());

        // 2) 사다리 이동 (LaddersGimmick에서 호출한 Action)
        moveAction?.Invoke();

        // 3) 페이드 아웃 전에 잠시 대기
        yield return new WaitForSeconds(fadeInDelay);

        // 4) 페이드 인 실행
        yield return StartCoroutine(FadeInCoroutine());
    }

    /// <summary>
    /// 화면을 완전 검정(a=1)까지 페이드 아웃
    /// </summary>
    public IEnumerator FadeOutCoroutine()
    {
        yield return StartCoroutine(FadeToAlpha(1f));
    }

    /// <summary>
    /// 화면을 완전 투명(a=0)까지 페이드 인
    /// </summary>
    public IEnumerator FadeInCoroutine()
    {
        yield return StartCoroutine(FadeToAlpha(0f));
    }

    /// <summary>
    /// fadeImage.color.a 값을 targetAlpha(0~1)로 서서히 변경
    /// </summary>
    private IEnumerator FadeToAlpha(float targetAlpha)
    {
        // 현재 알파 값
        float startAlpha = fadeImage.color.a;
        float timeElapsed = 0f;

        while (Mathf.Abs(fadeImage.color.a - targetAlpha) > 0.01f)
        {
            timeElapsed += Time.deltaTime * fadeSpeed;
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, timeElapsed);
            newAlpha = Mathf.Clamp01(newAlpha);

            Color c = fadeImage.color;
            c.a = newAlpha;
            fadeImage.color = c;

            yield return null;
        }

        // 정확하게 목표 알파로 설정
        Color finalColor = fadeImage.color;
        finalColor.a = targetAlpha;
        fadeImage.color = finalColor;
    }
}