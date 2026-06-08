using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlataformaRompible : MonoBehaviour
{
   //Tiempo para romperse
    public float breakDelay = 1.5f;
    //Tiempo para reaparecer
    public float respawnTime = 3f;

    // Plataforma Sprite
    public Transform visual;
    // Collider de la plataforma
    public Collider2D col;

    // Partículas cayendo
    public ParticleSystem stepLeaves;
    //Partículas explosión
    public ParticleSystem breakLeaves;

    //Posición original
    private Vector3 originalPos;
    //Buleano para saber si se ha subido el pj
    private bool isTriggered = false;

    void Start()
    {
        originalPos = visual.localPosition;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isTriggered && collision.gameObject.tag == "Player")
        {
            AnimSound("ArbustoCaer");
            StartCoroutine(BreakPlatform());
        }
    }

    IEnumerator BreakPlatform()
    {
        isTriggered = true;

        //Primeras hojas
        stepLeaves.Play();

        //Tiembla un poco
        yield return StartCoroutine(Bounce());

        //hojas de aviso
        for (int i = 0; i < 2; i++)
        {
            stepLeaves.Play();
            AnimSound("HojasCaer");
            yield return new WaitForSeconds(0.3f);
        }

        //temblor segundo
        yield return StartCoroutine(Shake(0.4f, 0.05f));

        //Delay antes de romperse
        yield return new WaitForSeconds(0.2f);

        // explosión y desaparece
        breakLeaves.Play();
        AnimSound("ArbustoRomperse");
        visual.gameObject.SetActive(false);
        col.enabled = false;

        //Reaparece
       
        yield return new WaitForSeconds(respawnTime);
         AnimSound("ArbustoRecompon");
        visual.localPosition = originalPos;
        breakLeaves.Play();
        visual.gameObject.SetActive(true);
        
        col.enabled = true;

        isTriggered = false;

    }
IEnumerator Bounce()
    {
        //posicion e inicio
        Vector3 down = originalPos + Vector3.down * 0.05f;
        float t = 0;

        //Baja
        while (t < 1)
        {
            visual.localPosition = Vector3.Lerp(originalPos, down, t);
            //velocidad
            t += Time.deltaTime * 10f;
            yield return null;
        }
        //vuelve a 0
        t = 0;

        //Sube
        while (t < 1)
        {
            visual.localPosition = Vector3.Lerp(down, originalPos, t);
            //velocidad
            t += Time.deltaTime * 8f;
            yield return null;
        }

        visual.localPosition = originalPos;
    }


    IEnumerator Shake(float duration, float magnitude)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            //temblor aleatorio en x
            float x = Random.Range(-0.2f, 0.2f) * magnitude;
            visual.localPosition = originalPos + new Vector3(x, 0, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }

        visual.localPosition = originalPos;

    }

    public void AnimSound(string soundName)
    {
        SoundEffectManager.Instance.PlayAtPosition(soundName, transform.position, true);
    }

}
