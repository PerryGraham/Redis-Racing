using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishUI : MonoBehaviour
{
    public CanvasGroup canvasGroup;

    void OnEnable() {
        canvasGroup.alpha = 0;
    }
    public IEnumerator FadeIn() {
        bool isFading = true;
        float time = 0f;
        float duration = 1f;
        while (isFading) {
            if (time >= duration) {
                canvasGroup.alpha = 1;
                isFading = false;
            }
            canvasGroup.alpha = Mathf.Lerp(0, 1, time / duration);
            time += Time.deltaTime;
            yield return 0;
        }
    }
}
