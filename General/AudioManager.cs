using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class AudioManager : Node
{
    public AudioStreamPlayer2D Music = new AudioStreamPlayer2D();
    public Dictionary<string, List<AudioStream>> musicStreams = new();
    GameManager gameManager;
    private static AudioManager _instance;
    public static AudioManager Instance
    {
        get
        {
            return _instance;
        }
    }
    //ensures PlayerManager is always in every scene
    public override void _EnterTree()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            QueueFree();
        }
    }
    public override void _Ready()
    {
        gameManager = GameManager.Instance;
        AddChild(Music);
        Music.Finished += PlayNextSong; // Correct
        LoadMusic("res://Audio/Music/FightMusic", "Fight");
        LoadMusic("res://Audio/Music/MenuMusic", "Menu");
        Music.Stream = GetSong("Menu");
        Music.Play();
    }
    public void LoadMusic(string folderPath, string category)
    {
        var dir = DirAccess.Open(folderPath);
        if (dir == null)
        {
            GD.PrintErr($"Could not open folder: {folderPath}");
            return;
        }

        if (!musicStreams.ContainsKey(category))
            musicStreams[category] = new List<AudioStream>();

        dir.SetIncludeNavigational(false);
        dir.SetIncludeHidden(false);
        dir.ListDirBegin();

        string fileName = dir.GetNext();
        while (fileName != string.Empty)
        {
            if (!dir.CurrentIsDir())
            {
                var ext = System.IO.Path.GetExtension(fileName).ToLowerInvariant();
                if (ext == ".ogg" || ext == ".wav" || ext == ".mp3")
                {
                    var fullPath = $"{folderPath}/{fileName}";
                    var stream = ResourceLoader.Load<AudioStream>(fullPath);
                    if (stream != null)
                        musicStreams[category].Add(stream);
                    else
                        GD.PrintErr($"Failed to load audio at {fullPath}");
                }
            }
            fileName = dir.GetNext();
        }

        dir.ListDirEnd();
    }

    public AudioStream GetSong(string category)
    {
        if (!musicStreams.ContainsKey(category) || musicStreams[category].Count == 0)
        {
            GD.PrintErr($"No songs found for category: {category}");
            return null;
        }

        Random rnd = new Random();
        var list = musicStreams[category];
        return list[rnd.Next(0, list.Count)];
    }

    
    public void PlayNextSong()
    {
        string category = gameManager.isInMenu ? "Menu" : "Fight";
        AudioStream next = GetSong(category);
        if (next != null)
        {
            Music.Stream = next;
            Music.Play();
        }
    }
}
