using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ConfirmationUI : MonoBehaviour
{
    private TextMeshProUGUI textMeshProUgui;

    // Use this for initialization
    void Start()
    {
        textMeshProUgui = gameObject.GetComponent<TextMeshProUGUI>();

        FadeOutAndDestroy();
    }

    public void FadeOutAndDestroy()
    {
        Color color = textMeshProUgui.color;

        StartCoroutine(FadeTo(textMeshProUgui, color, 0, 1f));
    }

    private IEnumerator FadeTo(TextMeshProUGUI textMeshProUgui, Color color, float aValue, float aTime)
    {
        yield return new WaitForSeconds(1f);

        float alpha = color.a;
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
        {
            Color newColor = new Color(1, 1, 1, Mathf.Lerp(alpha, aValue, t));
            textMeshProUgui.color = newColor;
            yield return null;
        }
        Destroy(gameObject);
    }
}
