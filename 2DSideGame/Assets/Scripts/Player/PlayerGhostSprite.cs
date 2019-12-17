using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGhostSprite : MonoBehaviour
{
    private Transform m_player;
    private SpriteRenderer m_renderer;
    private SpriteRenderer m_playerRenderer;
    private Color m_colour;

    [SerializeField]
    private float m_activeTime = 0.1f;
    private float m_timeActivated;
    private float m_alpha;
    private float m_alphaMultiplier = 0.85f;
    [SerializeField]
    private float m_alphaSet = 0.8f;


    private void OnEnable()
    {
        m_renderer = GetComponent<SpriteRenderer>();
        m_player = GameObject.FindGameObjectWithTag("Player").transform;
        m_playerRenderer = m_player.GetComponent<SpriteRenderer>();
        m_alpha = m_alphaSet;
        m_renderer.sprite = m_playerRenderer.sprite;
        transform.position = m_player.position;
        transform.rotation = m_player.rotation;
        m_timeActivated = Time.time;
    }

    public void Update()
    {
        m_alpha *= m_alphaMultiplier;
        m_colour = new Color(1f, 1f, 1f, m_alpha);
        m_renderer.color = m_colour;

        if (Time.time >= (m_timeActivated + m_activeTime)) 
        {
            ObjectPooling.Instance.AddToPool(gameObject);
        }
    }
}
