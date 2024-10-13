using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireFlicker : MonoBehaviour
{
    public Color color;
    [Range(0, .3f)] public float deviation;
    public float timescale = 1f;
    private SpriteRenderer m_SpriteRenderer;
    void Start()
    {
        m_SpriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        m_SpriteRenderer.color = new Color (color.r, color.g, color.b, color.a + deviation * Mathf.PerlinNoise(Time.time * timescale, 0f));
    }
}
