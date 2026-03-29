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

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        lastPos = rb.position;
    }

    void FixedUpdate()
    {
        Transform target = points[index];

        Vector2 newPos = Vector2.MoveTowards(
            rb.position,
            target.position,
            speed * Time.fixedDeltaTime
        );

        rb.MovePosition(newPos);

        PlatformVelocity = (newPos - lastPos) / Time.fixedDeltaTime;
        lastPos = newPos;

        if (Vector2.Distance(newPos, target.position) < 0.01f)
        {
            index = (index + 1) % points.Length;
        }
    }
}