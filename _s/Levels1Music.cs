using System;
using System.Collections;
using UnityEngine;
public class Levels1Music : MonoBehaviour
{
    public AudioSource au;
    public float fl;
    private void Start()
    {
        StartCoroutine(Tem());
    }

    IEnumerator Tem()
    {
        yield return new WaitForSeconds(fl);
        au.Play();
    }
    
}
