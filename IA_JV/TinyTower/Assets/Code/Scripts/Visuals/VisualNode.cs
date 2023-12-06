using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class VisualNode : MonoBehaviour
{
    [SerializeField] private GameObject line;

    public enum State
    {
        START,
        INACTIVE,
        END,
        PATH
    }

    private SpriteRenderer spriteRenderer;

    private Color inactiveColor = Color.grey;
    private Color startColor = Color.green;
    private Color endColor = Color.red;
    private Color pathColor = Color.blue;


    public LineRenderer AddLine(float length, Vector2 dir, float scale)
    {
        LineRenderer lineRenderer = Instantiate(line, Vector3.zero, Quaternion.identity).GetComponent<LineRenderer>();
        lineRenderer.startColor = inactiveColor;
        lineRenderer.endColor = inactiveColor;
        lineRenderer.transform.SetParent(transform);
        lineRenderer.transform.localPosition = Vector3.zero;
        lineRenderer.SetPosition(0, Vector2.zero);
        lineRenderer.SetPosition(1, dir * length / scale);

        transform.localScale = (scale * 0.5f) * Vector3.one;
        lineRenderer.startWidth *= scale;
        lineRenderer.transform.localScale = Vector3.one;

        if(spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        spriteRenderer.color = inactiveColor;
        return lineRenderer;
    }

    public void SetNode(State state)
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        spriteRenderer.color = state switch
        {
            State.START => startColor,
            State.INACTIVE => inactiveColor,
            State.END => endColor,
            State.PATH => pathColor,
            _ => inactiveColor,
        };
    }

    internal void SetLineColor(State state, LineRenderer line)
    {
        var col = state switch
        {
            State.START => startColor,
            State.INACTIVE => inactiveColor,
            State.END => endColor,
            State.PATH => pathColor,
            _ => inactiveColor,
        };

        line.startColor = col;
        line.endColor = col;
    }
}
