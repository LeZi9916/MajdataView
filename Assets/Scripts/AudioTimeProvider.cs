﻿using System;
using UnityEngine;

public class AudioTimeProvider : MonoBehaviour
{
    public float AudioTime; //notes get this value
    public bool isStart;
    public bool isRecord;
    public float offset;
    private float speed;

    private float startTime;
    private long ticks;

    public float CurrentSpeed => isRecord ? Time.timeScale : speed;

    // Update is called once per frame
    private void Update()
    {
        if (isStart)
        {
            if (isRecord)
                AudioTime = Time.time - startTime + offset;
            else
            {
                AudioTime = (Time.realtimeSinceStartup - startTime) * speed + offset;
                var source = GameObject.Find("AudioManager").transform.Find("BGM").gameObject.GetComponent<AudioSource>();
                //AudioTime = (float)source.timeSamples / source.clip.frequency;
                print(source.time);
            }
        }
    }

    public void SetStartTime(long _ticks, float _offset, float _speed, bool _isRecord = false)
    {
        var source = GameObject.Find("AudioManager").transform.Find("BGM").gameObject.GetComponent<AudioSource>();
        source.Play();
        ticks = _ticks;
        offset = _offset;
        AudioTime = offset;
        var dateTime = new DateTime(ticks);
        var seconds = (dateTime - DateTime.Now).TotalSeconds;
        isRecord = _isRecord;
        if (_isRecord)
        {
            startTime = Time.time + 5;
            Time.timeScale = _speed;
            Time.captureFramerate = 60;
        }
        else
        {
            startTime = Time.realtimeSinceStartup + (float)seconds;
            speed = _speed;
            Time.captureFramerate = 0;
        }
        isStart = true;
    }

    public void ResetStartTime()
    {
        offset = 0f;
        isStart = false;
    }
}