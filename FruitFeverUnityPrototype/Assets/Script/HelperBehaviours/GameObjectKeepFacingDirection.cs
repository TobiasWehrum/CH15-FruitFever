using UnityEngine;
using System.Collections;

public class GameObjectKeepFacingDirection : MonoBehaviour
{
    private Quaternion desiredLocalRotation;

    private void Awake()
    {
        desiredLocalRotation = transform.localRotation;
    }

    private void LateUpdate()
    {
        var lossyScale = transform.lossyScale;

        if (lossyScale.x < 0)
        {
            var localScale = transform.localScale;
            localScale.x = -localScale.x;
            transform.localScale = localScale;
        }

        if (lossyScale.y < 0)
        {
            var localScale = transform.localScale;
            localScale.y = -localScale.y;
            transform.localScale = localScale;
        }

        transform.rotation = desiredLocalRotation;
    }
}