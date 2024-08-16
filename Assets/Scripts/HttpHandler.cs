using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Assets.Scripts.Types;
using System.Text.Json;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Security.Cryptography;

public class HttpHandler : MonoBehaviour
{
    public static bool IsReloding { get; set; } = false;
    private readonly HttpListener http = new();
    private Task listen;
    private string request = "";


    private void Start()
    {
        SceneManager.LoadScene(1);
        http.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
        http.Prefixes.Add("http://localhost:8013/");
        http.Prefixes.Add("http://localhost:8013/Ping/");
        http.Start();
        listen = new Task(httpListen);
        listen.Start();
        print("server started");
    }
    void InitJsonLoader(in EditRequest data,bool isRecordOrPreview = false)
    {
        var loader = GameObject.Find("DataLoader").GetComponent<JsonDataLoader>();
        var timeProvider = GameObject.Find("AudioTimeProvider").GetComponent<AudioTimeProvider>();
        var objectCounter = GameObject.Find("ObjectCounter").GetComponent<ObjectCounter>();
        var multTouchHandler = GameObject.Find("MultTouchHandler").GetComponent<MultTouchHandler>();
        var bgManager = GameObject.Find("Background").GetComponent<BGManager>();
        var bgCover = GameObject.Find("BackgroundCover").GetComponent<SpriteRenderer>();

        InputManager.Mode = (AutoPlayMode)(int)data.EditorPlayMethod;

        timeProvider.SetStartTime((long)data.StartAt, (float)data.StartTime, (float)data.AudioSpeed);
        loader.noteSpeed = (float)(107.25 / (71.4184491 * Mathf.Pow((float)data.NoteSpeed + 0.9975f, -0.985558604f)));
        loader.touchSpeed = (float)data.TouchSpeed;
        loader.smoothSlideAnime = (bool)data.SmoothSlideAnime;
        objectCounter.ComboSetActive((EditorComboIndicator)data.ComboStatusType);
        loader.LoadJson(File.ReadAllText(data.JsonPath), (float)data.StartTime);
        multTouchHandler.clearSlots();
        bgManager.LoadBGFromPath(new FileInfo(data.JsonPath).DirectoryName, (float)data.AudioSpeed);
        bgCover.color = new Color(0f, 0f, 0f, (float)data.BackgroundCover);

        if(isRecordOrPreview)
            bgManager.PlaySongDetail();
    }
    private void Update()
    {
        if (string.IsNullOrEmpty(request)) 
            return;

        IsReloding = false;
        var data = JsonSerializer.Deserialize<EditRequest>(request);
        request = string.Empty;

        var timeProvider = GameObject.Find("AudioTimeProvider").GetComponent<AudioTimeProvider>();
        var bgManager = GameObject.Find("Background").GetComponent<BGManager>();
        var screenRecorder = GameObject.Find("ScreenRecorder").GetComponent<ScreenRecorder>();

        

        switch(data.Control)
        {
            case EditorControlMethod.Start:
                {
                    InitJsonLoader(data);
                    GameObject.Find("Notes").GetComponent<PlayAllPerfect>().enabled = false;
                }
                break;
            case EditorControlMethod.OpStart:
                {
                    InitJsonLoader(data,true);
                }
                break;
            case EditorControlMethod.Record:
                {
                    var maidataPath = new FileInfo(data.JsonPath).DirectoryName;
                    InitJsonLoader(data, true);

                    screenRecorder.CutoffTime = getChartLength();
                    screenRecorder.CutoffTime += 10f;
                    screenRecorder.StartRecording(maidataPath);

                    GameObject.Find("CanvasButtons").SetActive(false);
                    //GameObject.Find("Notes").GetComponent<NoteManager>().Refresh();
                }
                break;
            case EditorControlMethod.Pause:
                timeProvider.isStart = false;
                bgManager.PauseVideo();
                break;
            case EditorControlMethod.Stop:
                screenRecorder.StopRecording();
                timeProvider.ResetStartTime();
                IsReloding = true;
                SceneManager.LoadScene(1);
                break;
            case EditorControlMethod.Continue:
                timeProvider.SetStartTime((long)data.StartAt, (float)data.StartTime, (float)data.AudioSpeed);
                bgManager.ContinueVideo((float)data.AudioSpeed);
                break;
        }
    }

    private void OnDestroy()
    {
        http.Stop();
        print("server stoped");
    }

    private void httpListen()
    {
        while (http.IsListening)
        {
            var context = http.GetContext();
            
            switch(context.Request.RawUrl)
            {
                case "/Ping/":
                    {
                        context.Response.StatusCode = 200;
                        var stream = new StreamWriter(context.Response.OutputStream);
                        stream.WriteLine("Pong");
                        stream.Close();
                        context.Response.Close();
                    }
                    break;
                default:
                    {
                        print(context.Request.HttpMethod);
                        var reader = new StreamReader(context.Request.InputStream);
                        var data = reader.ReadToEnd();
                        print(data);
                        request = data;
                        while (request != "") ;
                        context.Response.StatusCode = 200;
                        var stream = new StreamWriter(context.Response.OutputStream);
                        stream.WriteLine("Hello!!!");
                        stream.Close();
                        context.Response.Close();
                    }
                    break;
            }

        }

        print("exit listen");
    }

    private float getChartLength()
    {
        var length = 0f;
        foreach (var noteData in GameObject.Find("Notes").GetComponentsInChildren<NoteDrop>(true))
        {
            length = Math.Max(length, noteData.time);

            var longData = noteData as NoteLongDrop;
            if (longData != null) length = Math.Max(length, noteData.time + longData.LastFor);
        }

        return length;
    }
}