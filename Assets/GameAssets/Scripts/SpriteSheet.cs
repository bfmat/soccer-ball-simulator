﻿using UnityEngine;
using System.Collections;

class SpriteSheet : MonoBehaviour
{
    public int Columns = 5;
    public int Rows = 5;
    public float FramesPerSecond = 10f;
    public bool RunOnce = true;

    private Renderer myRenderer;

    public float RunTimeInSeconds
    {
        get
        {
            return ((1f / FramesPerSecond) * (Columns * Rows));
        }
    }

    private Material materialCopy = null;

    void Start()
    {
        myRenderer = GetComponent<Renderer>();
        // Copy its material to itself in order to create an instance not connected to any other
        materialCopy = new Material(myRenderer.material);
        myRenderer.material = materialCopy;

        Vector2 size = new Vector2(1f / Columns, 1f / Rows);
        myRenderer.material.SetTextureScale("_MainTex", size);
        StartCoroutine(UpdateTiling());
    }

    private IEnumerator UpdateTiling()
    {
        float x = 0f;
        float y = 0f;
        Vector2 offset = Vector2.zero;

        while (true)
        {
            for (int i = Rows - 1; i >= 0; i--) // y
            {
                y = (float)i / Rows;

                for (int j = 0; j <= Columns - 1; j++) // x
                {
                    x = (float)j / Columns;

                    offset.Set(x, y);

                    myRenderer.material.SetTextureOffset("_MainTex", offset);
                    yield return new WaitForSeconds(1f / FramesPerSecond);
                }
            }

            if (RunOnce)
            {
                yield break;
            }
        }
    }
}