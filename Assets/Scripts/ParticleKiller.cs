using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleKiller : MonoBehaviour
{
    public int kilTime;
    private void Start()
    {
        Destroy(this.gameObject, kilTime);
    }
}
