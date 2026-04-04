using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlataformaMovil : MonoBehaviour
{
    public Transform[] points;
    public float speed = 2f;

    private int index = 0;
    private Rigidbody2D rb;

    public Vector2 PlatformVelocity { get; private set; }
    private Vector2 lastPos;

    //variables para el tiempo parada en cada punto
    private float waitTime;
    private float waitCounter = 0f;
    private bool isWaiting = false;

    private bool hasArrived = false; //ha llegado mejor?



    void Awake()
    {
        waitTime = 0.3f;
    }
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        lastPos = rb.position;
    }

    void FixedUpdate()
    {
        if (isWaiting)
        {
            waitCounter -= Time.fixedDeltaTime;
            if (waitCounter <= 0f)
            {
                isWaiting = false;
                index = (index + 1) % points.Length;

            }
            PlatformVelocity = Vector2.zero;
            return;
        }
        Transform target = points[index];

        Vector2 newPos = Vector2.MoveTowards(
            rb.position,
            target.position,
            speed * Time.fixedDeltaTime
        );

        rb.MovePosition(newPos);

        PlatformVelocity = (newPos - lastPos) / Time.fixedDeltaTime;
        lastPos = newPos;

        if (!isWaiting && Vector2.Distance(newPos, target.position) < 0.01f)
        {
            isWaiting = true;
            waitCounter = waitTime;
        }
    }
}