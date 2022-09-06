namespace Adesi.Music;

[GameResource( "Music Track", "track", "A compilation of music files that can be played as a single track. And transitioned between", Icon = "queue_music" )]
public class Track : GameResource
{
	public static Dictionary<string, Track> AllTracks { get; set; } = new Dictionary<string, Track>();
	public static Dictionary<string, Track> AllTracksByPath { get; set; } = new Dictionary<string, Track>();

	protected override void PostLoad()
	{
		base.PostLoad();
		AllTracks[ResourceName] = this;
		AllTracksByPath[ResourcePath] = this;
	}
	protected override void PostReload()
	{
		base.PostReload();
		AllTracks[ResourceName] = this;
		AllTracksByPath[ResourcePath] = this;
	}
	public List<SoundTrack> Tracks { get; set; }

	public bool TransitionOnBeat { get; set; } = true;

	[Editor( "class" )]
	public class SoundTrack
	{
		public SoundTrack()
		{
			Volume = Music?.Volume ?? 1f;
		}

		public SoundEvent Music { get; set; }
		[Title( "Beats Per Minute" )]
		public float BPM { get; set; } = 90;
		[Title( "Beats Per Measure" )]
		public int BeatsPerMeasure { get; set; } = 4;
		public float Volume = 1f;

		public float TransitionTime { get; set; }

		public Sound CurrentSound;

		override public string ToString()
		{
			return Music?.ResourceName ?? "No music";
		}

		public Sound Play()
		{
			return Play( Music.Volume );
		}
		public Sound Play( float volume )
		{
			var handle = Sound.FromScreen( Music.ResourceName );
			handle.SetVolume( volume );
			Volume = volume;
			CurrentSound.Stop();
			CurrentSound = handle;
			return handle;
		}

		public void Stop()
		{
			CurrentSound.Stop();
		}

		public void SetVolume( float v )
		{
			CurrentSound.SetVolume( v );
			Volume = v;
		}

		public RealTimeUntil GetTimeTillNextBeat()
		{
			if ( BeatsPerMeasure <= 0 )
			{
				BeatsPerMeasure = 4;
			}
			var time = RealTime.Now;
			var beat = 60f / (BPM / BeatsPerMeasure);
			var timeTillNextBeat = beat - (time % beat);
			return timeTillNextBeat;
		}
		public RealTimeUntil GetTimeTillNextBeat( float offset )
		{
			if ( BeatsPerMeasure <= 0 )
			{
				BeatsPerMeasure = 4;
			}
			var time = RealTime.Now;
			var beat = 60f / (BPM / BeatsPerMeasure);
			var timeTillNextBeat = beat - (time % beat) + offset;
			return timeTillNextBeat;
		}
	}

	public static Track GetFromPath( string selectedTrack )
	{
		if ( AllTracksByPath.ContainsKey( selectedTrack ) )
		{
			return AllTracksByPath[selectedTrack];
		}
		return null;
	}

	public static Track Get( string v )
	{
		if ( AllTracks.ContainsKey( v ) )
			return AllTracks[v];
		return null;
	}
}
