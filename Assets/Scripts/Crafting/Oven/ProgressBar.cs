using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressBar : MyUI
{
    [SerializeField] private Transform barFill;
    float fillScale;

    public override void Start()
    {
        fillScale = barFill.localScale.x;
        base.Start();
    }
    public void SetValue(float value)
    {
        barFill.localScale += new Vector3(value * fillScale - barFill.localScale.x, 0, 0);
    }
}
