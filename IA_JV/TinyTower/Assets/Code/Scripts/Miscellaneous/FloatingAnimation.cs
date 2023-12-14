using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FloatingAnimation : MonoBehaviour
{
    private void FixedUpdate()
    {
        transform.localPosition = new Vector3(transform.localPosition.x, Mathf.Sin(Time.time * 2f) * 0.1f, 0);
    }
}
