using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollBackground : MonoBehaviour
{
    public float HorizontalSpeed = 0.2f;
    public float VerticalSpeed = 0.2f;
    public bool ScrollAutomatically = false;

    private Renderer _renderer;

    // Start is called before the first frame update
    void Start()
    {
        _renderer = GetComponent<Renderer>();
        if (!ScrollAutomatically) enabled = false;
    }

    private void FixedUpdate()
    {
        _renderer.material.mainTextureOffset = new Vector2(Time.time * HorizontalSpeed, Time.time * VerticalSpeed);
    }

    public void Move(Vector2 offset)
    {
        _renderer.material.mainTextureOffset += offset;
    }
}
