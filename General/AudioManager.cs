using Godot;
using System;
using System.Linq;

public partial class AudioManager : Node
{
	// Called when the node enters the scene tree for the first time.
	AudioStream[] streams = new AudioStream[] {
		ResourceLoader.Load<AudioStream>("res://Audio/Music/Dream Sakura-remix.mp3"),
		ResourceLoader.Load<AudioStream>("res://Audio/Music/Honobono Teahouse.mp3"),
		ResourceLoader.Load<AudioStream>("res://Audio/Music/Moonlight Japanese Harp.mp3"),
		ResourceLoader.Load<AudioStream>("res://Audio/Music/Mountain God's Shrine.mp3"),
		ResourceLoader.Load<AudioStream>("res://Audio/Music/Mysterious Kyoto.mp3"),
		ResourceLoader.Load<AudioStream>("res://Audio/Music/Shamisen Samurai Rock.mp3"),
		ResourceLoader.Load<AudioStream>("res://Audio/Music/Voice of Evening Calm.mp3")
	};
	private Random rnd = new Random();
	public AudioStream GetSong(){
		return streams[rnd.Next(0, streams.Count())];
	}
}
