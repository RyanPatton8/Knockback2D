using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class AudioManager : Node
{
    public AudioStreamPlayer2D Music = new AudioStreamPlayer2D();
    public List<AudioStream> streams = new();

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
        AddChild(Music);
        Music.Finished += PlayNextSong;
        LoadAllAudio("res://Audio//Music");
        Music.Stream = GetSong();
        Music.Play();
    }

    public void LoadAllAudio(string folderPath)
    {
        // Open the directory (res:// is read-only inside the PCK)
        var dir = DirAccess.Open(folderPath);
        if (dir == null)
        {
            GD.PrintErr($"Could not open folder: {folderPath}");
            return;
        }
        // tell it whether to include “.”/“..” and hidden files
        dir.SetIncludeNavigational(false);  // skip “.” and “..”
        dir.SetIncludeHidden(false);        // skip hidden files

        // now begin the listing
        dir.ListDirBegin();
        string fileName = dir.GetNext();
        while (fileName != string.Empty)
        {
            // Only files (not sub-folders)
            if (!dir.CurrentIsDir())
            {
                // filter by audio extension
                var ext = System.IO.Path.GetExtension(fileName).ToLowerInvariant();
                if (ext == ".ogg" || ext == ".wav" || ext == ".mp3")
                {
                    var fullPath = $"{folderPath}/{fileName}";
                    // load the AudioStream resource
                    var stream = ResourceLoader.Load<AudioStream>(fullPath);
                    if (stream != null)
                        streams.Add(stream);
                    else
                        GD.PrintErr($"Failed to load audio at {fullPath}");
                }
            }
            fileName = dir.GetNext();
        }
        dir.ListDirEnd();
    }
    public AudioStream GetSong()
    {
        Random rnd = new Random();
        return streams[rnd.Next(0, streams.Count)];
    }
    
    public void PlayNextSong(){
        AudioStream next = GetSong();
        while (next == Music.Stream)
        {
            next = GetSong();
        }
        Music.Stream = next;
        Music.Play();
    }
}
