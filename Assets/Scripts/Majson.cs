using System.Collections.Generic;
#nullable enable
internal class Majson
{
    public string artist { get; init; } = "default";
    public string designer { get; init; } = "default";
    public string difficulty { get; init; } = "EZ";
    public int diffNum { get; init; } = 0;
    public string level { get; init; } = "1";
    public List<SimaiTimingPoint> timingList { get; init; } = new();
    public string title { get; init; } = "default";
}

internal class SimaiTimingPoint
{
    public float currentBpm { get; set; } = -1;
    public bool havePlayed { get; set; }
    public float HSpeed { get; set; } = 1f;
    public List<SimaiNote> noteList { get; set; } = new(); //only used for json serialize
    public string notesContent { get; set; }
    public int rawTextPositionX { get; set; }
    public int rawTextPositionY { get; set; }
    public double time { get; set; }
}

internal enum SimaiNoteType
{
    Tap,
    Slide,
    Hold,
    Touch,
    TouchHold
}

internal class SimaiNote
{
    public double holdTime { get; set; } = 0d;
    public bool isBreak { get; set; } = false;
    public bool isEx { get; set; } = false;
    public bool isFakeRotate { get; set; } = false;
    public bool isForceStar { get; set; } = false;
    public bool isHanabi { get; set; } = false;
    public bool isSlideBreak { get; set; } = false;
    public bool isSlideNoHead { get; set; } = false;

    public string noteContent { get; set; } //used for star explain
    public SimaiNoteType noteType { get; set; }

    public double slideStartTime { get; set; } = 0d;
    public double slideTime { get; set; } = 0d;

    public int startPosition { get; set; } = 1; //键位（1-8）
    public char touchArea { get; set; } = ' ';
}

internal readonly struct EditRequest
{
    public float? AudioSpeed { get; init; }
    public float? BackgroundCover { get; init; }
    public EditorComboIndicator? ComboStatusType { get; init; }
    public EditorPlayMethod? EditorPlayMethod { get; init; }
    public EditorControlMethod Control { get; init; }
    public string? JsonPath { get; init; }
    public float? NoteSpeed { get; init; }
    public long? StartAt { get; init; }
    public float? StartTime { get; init; }
    public float? TouchSpeed { get; init; }
    public bool? SmoothSlideAnime { get; init; }
}

public enum EditorComboIndicator
{
    None,

    // List of viable indicators that won't be a static content.
    // ScoreBorder, AchievementMaxDown, ScoreDownDeluxe are static.
    Combo,
    ScoreClassic,
    AchievementClassic,
    AchievementDownClassic,
    AchievementDeluxe = 11,
    AchievementDownDeluxe,
    ScoreDeluxe,

    // Please prefix custom indicator with C
    CScoreDedeluxe = 101,
    CScoreDownDedeluxe,
    MAX
}

internal enum EditorControlMethod
{
    Start,
    Stop,
    OpStart,
    Pause,
    Continue,
    Record
}

public enum EditorPlayMethod
{
    Classic, DJAuto, Random, Disabled
}