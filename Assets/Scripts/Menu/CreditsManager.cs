using System.Collections;
using TMPro;
using UnityEngine;

public class CreditsManager : MonoBehaviour
{
    public TMP_Text creditText;

    public float blackScreenTime = 6f;

    public GameObject logoImage;

    public float fadeInTime = 2f;
    public float visibleTime = 5f;
    public float fadeOutTime = 2f;

    public GameObject scrollCreditsPanel;

    private void Start()
    {
        scrollCreditsPanel.SetActive(false);

        StartCoroutine(CreditsSequence());
    }

    private IEnumerator CreditsSequence()
    {
        // Pantalla negra
        yield return new WaitForSeconds(blackScreenTime);

        // Créditos estáticos van haciendo fade in y out
        logoImage.SetActive(true);
        yield return new WaitForSeconds(12f);
        logoImage.SetActive(false);
        yield return new WaitForSeconds(2f);

        yield return ShowCredit("Julia Ibáñez   Y   Ale Navarro");
        yield return new WaitForSeconds(2f);
        yield return ShowCredit("Dirección técnica\n\nJulia Ibáñez Montero");
        yield return new WaitForSeconds(2f);
        yield return ShowCredit("Sonido y composición\n\nAle Navarro García");
        yield return new WaitForSeconds(2f);
        // Activar scroll
        scrollCreditsPanel.SetActive(true);
        scrollCreditsPanel.GetComponent<CreditsScroll>().StartScroll();
        creditText.gameObject.SetActive(false);
    }

    private IEnumerator ShowCredit(string text)
    {
        creditText.text = text;

        Color c = creditText.color;

        // Fade In
        float t = 0f;
        while (t < fadeInTime)
        {
            t += Time.deltaTime;

            c.a = Mathf.Lerp(0f, 1f, t / fadeInTime);
            creditText.color = c;

            yield return null;
        }

        // Visible
        yield return new WaitForSeconds(visibleTime);

        // Fade Out
        t = 0f;

        while (t < fadeOutTime)
        {
            t += Time.deltaTime;

            c.a = Mathf.Lerp(1f, 0f, t / fadeOutTime);
            creditText.color = c;

            yield return null;
        }
    }
}