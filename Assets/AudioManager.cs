using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Networking;

public class AudioManager : MonoBehaviour
{
    public AudioClip Bgm;
    public AudioClip Answer;
    public AudioClip Judge;
    public AudioClip Slide;
    public AudioClip Break;
    public AudioClip BreakEffect;
    public AudioClip BreakSlide;
    public AudioClip BreakSlideEffect;
    public AudioClip Ex;
    public AudioClip Touch;
    public AudioClip Hanabi;

    public AudioMixer AudioMixer;

    public AudioMixerGroup mBgm;
    public AudioMixerGroup mAnswer;
    public AudioMixerGroup mJudge;
    public AudioMixerGroup mSlide;
    public AudioMixerGroup mBreak;
    public AudioMixerGroup mBreakSlide;
    public AudioMixerGroup mEx;
    public AudioMixerGroup mTouch;
    public AudioMixerGroup mHanabi;

    public float lastPlayAnswer = -1;
    public float lastPlayJudge = -1;
    public float lastPlaySlide = -1;
    public float lastPlayBreak = -1;
    public float lastPlayBreakEffect = -1;
    public float lastPlayBreakSlide = -1;
    public float lastPlayBreakSlideEffect = -1;
    public float lastPlayEx = -1;
    public float lastPlayTouch = -1;
    public float lastPlayHanabi = -1;

    bool isPlaying = false;
    public enum Audio
    {
        BGM,
        ANSWER,
        JUDGE,
        SLIDE,
        BREAK,
        BREAK_EFFECT,
        BREAKSLIDE,
        BREAKSLIDE_EFFECT,
        EX,
        TOUCH,
        HANABI
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        var source = transform.Find("BGM").gameObject.GetComponent<AudioSource>();
        var audioTime = GameObject.Find("AudioTimeProvider").GetComponent<AudioTimeProvider>().AudioTime;
        //var audioTimeSamples = GameObject.Find("AudioTimeProvider").GetComponent<AudioSource>().timeSamples;
        
        if(source.isPlaying &&  Math.Abs(audioTime - source.time) > 0.03)
            source.time = audioTime;
    }
    private void OnEnable()
    {
        
    }
    void PlayAudio(AudioSource source,int startAt = 0)
    {
        var script = source.gameObject.GetComponent<AudioDestroySelf>();
        if (source is null)
            return;
        //source.time = startAt;
        source.Play();
        source.time = startAt;
        if(script is not null)
            script.enabled = true;
    }
    public void Play()
    {
        AudioSource source = transform.Find("BGM").gameObject.GetComponent<AudioSource>();
        PlayAudio(source);
    }
    public void PlayEffect(Audio audio,float time)
    {
        AudioSource source = null;
        switch (audio)
        {
            case Audio.ANSWER:
                if (Math.Abs(lastPlayAnswer - time) < 0.002)
                    return;
                else
                    lastPlayAnswer = time;
                source = Instantiate(transform.Find("Answer").gameObject, transform).GetComponent<AudioSource>();
                break;
            case Audio.JUDGE:
                if (Math.Abs(lastPlayJudge - time) < 0.002)
                    return;
                else
                    lastPlayJudge = time;
                source = Instantiate(transform.Find("Judge").gameObject, transform).GetComponent<AudioSource>();
                break;
            case Audio.SLIDE:
                if (Math.Abs(lastPlaySlide - time) < 0.002)
                    return;
                else
                    lastPlaySlide = time;
                source = Instantiate(transform.Find("Slide").gameObject, transform).GetComponent<AudioSource>();
                break;
            case Audio.BREAK:
                if (Math.Abs(lastPlayBreak - time) < 0.002)
                    return;
                else
                    lastPlayBreak = time;
                source = Instantiate(transform.Find("Break").gameObject, transform).GetComponent<AudioSource>();
                break;
            case Audio.BREAK_EFFECT:
                if (Math.Abs(lastPlayBreakEffect - time) < 0.002)
                    return;
                else
                    lastPlayBreakEffect = time;
                source = Instantiate(transform.Find("BreakEffect").gameObject, transform).GetComponent<AudioSource>();
                break;
            case Audio.BREAKSLIDE:
                if (Math.Abs(lastPlayBreakSlide - time) < 0.002)
                    return;
                else
                    lastPlayBreakSlide = time;
                source = Instantiate(transform.Find("BreakSlide").gameObject, transform).GetComponent<AudioSource>();
                break;
            case Audio.BREAKSLIDE_EFFECT:
                if (Math.Abs(lastPlayBreakSlideEffect - time) < 0.002)
                    return;
                else
                    lastPlayBreakSlideEffect = time;
                source = Instantiate(transform.Find("BreakSlideEffect").gameObject, transform).GetComponent<AudioSource>();
                break;
            case Audio.EX:
                if (Math.Abs(lastPlayEx - time) < 0.002)
                    return;
                else
                    lastPlayEx = time;
                source = Instantiate(transform.Find("Ex").gameObject, transform).GetComponent<AudioSource>();
                break;
            case Audio.TOUCH:
                if (Math.Abs(lastPlayTouch - time) < 0.002)
                    return;
                else
                    lastPlayTouch = time;
                source = Instantiate(transform.Find("Touch").gameObject, transform).GetComponent<AudioSource>();
                break;
            case Audio.HANABI:
                //source = Instantiate(transform.Find("Hanabi").gameObject, transform).GetComponent<AudioSource>();
                source = transform.Find("Hanabi").gameObject.GetComponent<AudioSource>();
                if (source.isPlaying)
                    source.Stop();
                break;
        }
        PlayAudio(source);

    }
    public void SetVolume(double[] volume)
    {
        AudioMixer.SetFloat("bgm",(1 - (float)volume[0]) * (-80));
        AudioMixer.SetFloat("answer", (1 - (float)volume[1]) * (-80));
        AudioMixer.SetFloat("judge",(1 - (float)volume[2]) * (-80));
        AudioMixer.SetFloat("slide",(1 - (float)volume[3]) * (-80));
        AudioMixer.SetFloat("break",(1 - (float)volume[4]) * (-80));
        AudioMixer.SetFloat("breakslide",(1 - (float)volume[5]) * (-80));
        AudioMixer.SetFloat("ex",(1 - (float)volume[6]) * (-80));
        AudioMixer.SetFloat("touch",(1 - (float)volume[7]) * (-80));
        AudioMixer.SetFloat("hanabi",(1 - (float)volume[8]) * (-80));
    }
    public void Stop()
    {
        transform.Find("BGM").gameObject.GetComponent<AudioSource>().Stop();
    }
    public void LoadEffect()
    {
        var notes = GameObject.Find("Notes");
        var noteCount = notes.transform.childCount;

        List<float> answerTime = new();// 正解
        List<float> judgeTime = new();// 判定
        List<float> slideTime = new();// Slide开头音效
        List<float> breakTime = new();// Break与BreakEffect共用
        List<float> exTime = new();// Ex音效
        List<float> breakSlideTime = new();// BreakSlide开头音效
        List<float> breakSlideEndTime = new();// BreakSlide判定音效
        List<float> hanabiTime = new();// 烟花音效

        for(int i = 0;i < noteCount;i++)
        {
            var obj = notes.transform.GetChild(i).gameObject;

            NoteDrop note = obj.GetComponent<TapDrop>();
            note = note != null ? note : obj.GetComponent<StarDrop>();
            note = note != null ? note : obj.GetComponent<TouchDrop>();
            note = note != null ? note : obj.GetComponent<TouchHoldDrop>();
            note = note != null ? note : obj.GetComponent<HoldDrop>();
            note = note != null ? note : obj.GetComponent<SlideDrop>();
            note = note != null ? note : obj.GetComponent<WifiDrop>();

            if (note is null)
                continue;
            else if(note is TapDrop tap)
            {
                answerTime.Add(tap.time);
                if(tap.isBreak)
                    breakTime.Add(tap.time);
                else if(tap.isEX)
                    exTime.Add(tap.time);
                else
                    judgeTime.Add(tap.time);                    
            }
            else if (note is StarDrop star)
            {
                answerTime.Add(star.time);
                if (star.isBreak)
                    breakTime.Add(star.time);
                else if (star.isEX)
                    exTime.Add(star.time);
                else
                    judgeTime.Add(star.time);
            }
            else if (note is TouchDrop touch)
            {
                answerTime.Add(touch.time);
                //TODO: Touch SFX
                if (touch.isFirework)
                    hanabiTime.Add(touch.time);
            }
            else if (note is HoldDrop hold)
            {
                answerTime.Add(hold.time);
                if(hold.LastFor != 0)
                {
                    answerTime.Add(hold.time + hold.LastFor);
                    if (hold.isBreak)
                        breakTime.Add(hold.time + hold.LastFor);
                    else
                        judgeTime.Add(hold.time + hold.LastFor);
                }
                else if (hold.isBreak)
                    breakTime.Add(hold.time);
                else if (hold.isEX)
                    exTime.Add(hold.time);
                else
                    judgeTime.Add(hold.time);
            }
            else if (note is TouchHoldDrop touchHold)
            {
                answerTime.Add(touchHold.time);
                if (touchHold.LastFor != 0)
                {
                    answerTime.Add(touchHold.time + touchHold.LastFor);
                    if (touchHold.isFirework)
                        hanabiTime.Add(touchHold.time + touchHold.LastFor);
                    // TODO: Touch SFX
                }
                else if (touchHold.isFirework)
                    hanabiTime.Add(touchHold.time + touchHold.LastFor);
                //TODO : Touch SFX
            }
            else if(note is SlideDrop slide)
            {
                if (slide.isBreak && !slide.isGroupPart)
                    breakSlideTime.Add(slide.time);
                else if(!slide.isGroupPart)
                    slideTime.Add(slide.time);

                if (slide.isBreak && slide.isGroupPartEnd)
                    breakSlideEndTime.Add(slide.time + slide.LastFor);                    
            }
            else if(note is WifiDrop wifi)
            {
                if (wifi.isBreak && !wifi.isGroupPart)
                    breakSlideTime.Add(wifi.time);
                else if (!wifi.isGroupPart)
                    slideTime.Add(wifi.time);

                if (wifi.isBreak && wifi.isGroupPartEnd)
                    breakSlideEndTime.Add(wifi.time + wifi.LastFor);
            }
        }
        var output = InsertAudioClip(answerTime.ToArray(), Answer);
        var source = transform.Find("Answer").gameObject.GetComponent<AudioSource>();
        source.clip = output;
        source.Play();
        return;
    }
    public void LoadAudio(string path)
    {
        //音效文件目录
        var sfx = new DirectoryInfo(Application.dataPath).Parent.FullName + "/SFX";

        var bgm = LoadAudioFromFile(Path.Combine(path,"track.mp3"), AudioType.MPEG);
        var answer = LoadAudioFromFile(Path.Combine(sfx, "answer.wav"), AudioType.WAV);
        var judge = LoadAudioFromFile(Path.Combine(sfx, "judge.wav"), AudioType.WAV);
        var slide = LoadAudioFromFile(Path.Combine(sfx, "slide.wav"), AudioType.WAV);
        var _break = LoadAudioFromFile(Path.Combine(sfx, "judge_break.wav"), AudioType.WAV);
        var breakEffect = LoadAudioFromFile(Path.Combine(sfx, "break.wav"), AudioType.WAV);
        var breakSlide = LoadAudioFromFile(Path.Combine(sfx, "break_slide_start.wav"), AudioType.WAV);
        var breakSlideEffect = LoadAudioFromFile(Path.Combine(sfx, "break_slide.wav"), AudioType.WAV);
        var ex = LoadAudioFromFile(Path.Combine(sfx, "judge_ex.wav"), AudioType.WAV);
        var touch = LoadAudioFromFile(Path.Combine(sfx, "touch.wav"), AudioType.WAV);
        var hanabi = LoadAudioFromFile(Path.Combine(sfx, "hanabi.wav"), AudioType.WAV);

        if(bgm is null)
            bgm = LoadAudioFromFile(Path.Combine(path, "track.ogg"), AudioType.OGGVORBIS);

        Bgm = bgm;
        Answer = answer;
        Judge = judge;
        Slide = slide;
        Break = _break; 
        BreakEffect = breakEffect;
        BreakSlide = breakSlide;
        BreakSlideEffect = breakSlideEffect;
        Ex = ex;
        Touch = touch;
        Hanabi = hanabi;
        Bgm.LoadAudioData();
        var source = transform.Find("BGM").gameObject.GetComponent<AudioSource>();
        source.clip = Bgm;
        source = transform.Find("Answer").gameObject.GetComponent<AudioSource>();
        source.clip = Answer;
        source = transform.Find("Judge").gameObject.GetComponent<AudioSource>();
        source.clip = Judge;
        source = transform.Find("Slide").gameObject.GetComponent<AudioSource>();
        source.clip = Slide;
        source = transform.Find("Break").gameObject.GetComponent<AudioSource>();
        source.clip = Break;
        source = transform.Find("BreakEffect").gameObject.GetComponent<AudioSource>();
        source.clip = BreakEffect;
        source = transform.Find("BreakSlide").gameObject.GetComponent<AudioSource>();
        source.clip = BreakSlide;
        source = transform.Find("BreakSlideEffect").gameObject.GetComponent<AudioSource>();
        source.clip = BreakSlideEffect;
        source = transform.Find("Ex").gameObject.GetComponent<AudioSource>();
        source.clip = Ex;
        source = transform.Find("Touch").gameObject.GetComponent<AudioSource>();
        source.clip = Touch;
        source = transform.Find("Hanabi").gameObject.GetComponent<AudioSource>();
        source.clip = Hanabi;
    }
    AudioClip InsertAudioClip(float[] time, AudioClip clip)
    {
        if (time.Length == 0)
            return null;
        var data = new float[clip.samples * clip.channels];
        clip.GetData(data, 0);

        var newClipLength = (int)(clip.frequency * time.OrderBy(x => x).Last());
        var newData = new List<float>();//插入0
        var newClip = AudioClip.Create("Track", newClipLength, clip.channels, clip.frequency, false);     
        
        List<long> samples = new();
        foreach (var t in time) samples.Add(clip.frequency * clip.channels * (long)t);

        for(int i =0;i<samples.Last();i++)
        {
            if (samples.Contains(i))
            {
                newData.RemoveRange(i, newData.Count - i);
                newData.InsertRange(i, data);
            }
            else
                newData.Add(0);                
        }

        newClip.SetData(newData.ToArray(), 0);

        return newClip;
    }
    AudioClip RemoveSilence(AudioClip clip, float value)
    {
        var data = new float[clip.samples * clip.channels];
        clip.GetData(data, 0);
        var _data = new List<float>();

        foreach (var b in data)
            if (Math.Abs(b) > value)
                _data.Add(b);

        if (_data.Count == 0) return clip;

        var _clip = AudioClip.Create(clip.name, _data.Count, clip.channels, clip.frequency, false);
        _clip.SetData(_data.ToArray(), 0);

        return _clip;
    }
#nullable enable
    public static AudioClip? LoadAudioFromFile(string path,AudioType type)
    {
        try
        {
            if (!File.Exists(path))
                return null;

            var uwr = UnityWebRequestMultimedia.GetAudioClip($"file:///{path}", type);
            var response = uwr.SendWebRequest();
            while (!response.isDone) ;

            return DownloadHandlerAudioClip.GetContent(uwr);
        }
        catch { return null; }
    }
}
