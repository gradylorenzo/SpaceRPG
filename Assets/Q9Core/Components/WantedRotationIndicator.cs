using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Q9Core;

public class WantedRotationIndicator : MonoBehaviour
{
    public Vector3 newRotation;
    private void FixedUpdate()
    {
        newRotation = GameManager.Control.wantedRotation;
        transform.rotation = Quaternion.Euler(newRotation);
    }
}
