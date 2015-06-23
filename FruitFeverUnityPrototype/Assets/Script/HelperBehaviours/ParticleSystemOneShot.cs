using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ParticleSystemOneShot : MonoBehaviour
{
    private void Update()
    {
        if (GetComponent<ParticleSystem>().IsAlive())
            return;

        Destroy(gameObject);
    }
}

