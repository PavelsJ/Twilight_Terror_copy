using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Shake_Camera_Manager : MonoBehaviour
{
    private CinemachineVirtualCamera cinemachine;
    private CinemachineBasicMultiChannelPerlin perlin;
   
    private void Awake()
    {
        cinemachine = GameObject.Find("Virtual Camera").GetComponent<CinemachineVirtualCamera>();
    }

    private void Start()
    {
        if (cinemachine != null)
        {
            perlin = cinemachine.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        }
    }

    public void ShakeCamera(float intensity)
    {
        perlin.m_AmplitudeGain = intensity;
    }

    public void ShakeCamera(float intensity, float duration)
    {
        perlin.m_AmplitudeGain = intensity;
        StartCoroutine(Shake(duration));
    }

    private IEnumerator Shake(float duration)
    {
        yield return new WaitForSeconds(duration);
        perlin.m_AmplitudeGain = 0;
    }
}
