﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity;
using System;
using UnityEngine.Windows.Speech;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.CSharp;

public class VoiceCallback : MonoBehaviour, IMyDictationHandler
{
    /*[SerializeField]
	private GameObject charactor;
	[SerializeField]
	private GameObject hololensCamera;*/
    [SerializeField]
    private GameObject motionController;
    /*[SerializeField]
    private MicStream.StreamCategory streamType = MicStream.StreamCategory.HIGH_QUALITY_VOICE;
    [SerializeField]
    private float inputGain = 1f;
    [SerializeField]
    private bool keepAllData;*/
    [SerializeField]
    private GameObject speakor;
    [SerializeField]
    private string luisUrl;

    private bool isResording;

    struct IntentAndEntity
    {
        public string intent;
        public string[] entities;
    }

    public void StartRecord()
    {
        Debug.Log("Recording");
        isResording = true;
    }

    public void FollowMe()
    {
        motionController.GetComponent<BotMotionController>().Follow();
    }

    public void StandStill()
    {
        motionController.GetComponent<BotMotionController>().Stand();
    }

    public void Test(string r = "")
    {
        Debug.Log("测试: " + r);
    }

    private void CheckForErrorOnCall(int returnCode)
    {
        MicStream.CheckForErrorOnCall(returnCode);
    }

    public void OnDictationHypothesis(string text)
    {
        Debug.Log("Hypothesis");
        speakor.GetComponent<BotSpeak>().Show(text + "...");
    }

    public async void OnDictationResult(string text, ConfidenceLevel confidence)
    {
        Debug.Log(text);
        speakor.GetComponent<BotSpeak>().Say(text);
        if (text.ToLower() == "跟随")
        {
            FollowMe();
        }
        else if (text.ToLower() == "原地等待")
        {
            StandStill();
        }
        else
        {
            // LUIS后端
            HttpClient http = new HttpClient();
            var res = await http.GetAsync(luisUrl + text);
            var obj = JsonUtility.FromJson<IntentAndEntity>(await res.Content.ReadAsStringAsync());
            //obj.intent;
            // 自定义
        }
    }

    public void OnDictationComplete(DictationCompletionCause cause)
    {
        Debug.Log("Complete" + cause.ToString());
    }

    public void OnDictationError(string error, int hresult)
    {
        Debug.Log("Dictation error!");
    }
}
