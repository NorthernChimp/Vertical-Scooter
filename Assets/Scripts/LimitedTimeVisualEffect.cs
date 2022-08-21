using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimitedTimeVisualEffect :MonoBehaviour
{
    public Counter endCounter;
    public void Setup(float timeAlive)
    {
        endCounter = new Counter(timeAlive);
    }
    public void Destroy() { Destroy(gameObject); }
}
