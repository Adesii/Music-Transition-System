using Adesi.Util;

namespace Adesi.Music;

public static class SoundtrackManager
{

	public static Track LastTrack { get; set; }
	public static Track CurrentTrack { get; set; }
	public static int LastTrackIndex { get; set; } = 0;
	public static int CurrentTrackIndex { get; set; } = 0;
	public static Track.SoundTrack LastSoundTrack => LastTrack?.Tracks[LastTrackIndex];
	public static Track.SoundTrack CurrentSoundTrack => CurrentTrack?.Tracks[CurrentTrackIndex];

	public static Sound CurrentSound => CurrentSoundTrack?.CurrentSound ?? new();

	public static void PlayTrack( Track track, bool ForceStop = false )
	{
		Host.AssertClient( "PlayTrack must be called on the client" );
		if ( track == null )
		{
			if ( CurrentSoundTrack != null )
				_ = FadeVolumeTo( CurrentSoundTrack, 0, CurrentSoundTrack.TransitionTime, Stop: true );
			else
				CurrentSound.Stop();
			CurrentTrack = null;
			return;
		}
		if ( ForceStop )
		{
			CurrentSound.Stop();
		}
		LastTrack = CurrentTrack;
		LastTrackIndex = CurrentTrackIndex;
		CurrentTrack = track;
		Log.Debug( "Playing track " + CurrentTrack.ResourceName );
		CurrentTrackIndex = 0;
		PlayTrackIndex( CurrentTrackIndex );
	}

	public static void Stop()
	{
		PlayTrack( null );
	}

	public static void PlayTrackIndex( int currentTrackIndex )
	{
		if ( CurrentTrack == null )
			return;

		if ( currentTrackIndex >= CurrentTrack.Tracks.Count )
			return;

		var track = CurrentTrack.Tracks[currentTrackIndex];
		if ( track == CurrentSoundTrack && !CurrentSound.Finished )
		{
			return;
		}

		if ( CurrentTrack.TransitionOnBeat )
		{
			TransitionOnBeat( LastSoundTrack, track );
		}
		else
		{
			Transition( LastSoundTrack, track );
		}
	}



	public static void Transition( Track.SoundTrack oldtrack, Track.SoundTrack newtrack )
	{
		if ( oldtrack != null && oldtrack != newtrack )
		{
			_ = FadeVolumeTo( oldtrack, 0, oldtrack.TransitionTime, Stop: true );
		}
		newtrack.Play( 0 );
		_ = FadeVolumeTo( newtrack, newtrack.Music.Volume, newtrack.TransitionTime );
	}
	private static async void TransitionOnBeat( Track.SoundTrack oldtrack, Track.SoundTrack newtrack )
	{


		if ( oldtrack == null )
		{
			Transition( oldtrack, newtrack );
			return;
		}
		var timetill = oldtrack.GetTimeTillNextBeat( newtrack.TransitionTime );
		Log.Info( "Transitioning in " + timetill );
		await GameTask.DelayRealtimeSeconds( timetill );
		_ = FadeVolumeTo( oldtrack, 0, newtrack.TransitionTime, Stop: true );
		newtrack.Play( 0 );
		_ = FadeVolumeTo( newtrack, newtrack.Music.Volume, newtrack.TransitionTime );

		//lastSoundTrack.Stop();
		//track.Play();

	}

	public static void Next()
	{
		LastTrack = CurrentTrack;
		LastTrackIndex = CurrentTrackIndex;
		CurrentTrackIndex = (CurrentTrackIndex + 1) % CurrentTrack.Tracks.Count;

		PlayTrackIndex( CurrentTrackIndex );
	}
	public static void Previous()
	{
		LastTrack = CurrentTrack;
		LastTrackIndex = CurrentTrackIndex;
		CurrentTrackIndex = (CurrentTrackIndex - 1) % CurrentTrack.Tracks.Count;

		PlayTrackIndex( CurrentTrackIndex );
	}

	public static async Task FadeVolumeTo( Track.SoundTrack track, float volume, float seconds = 1, int steps = 100, bool Stop = false )
	{
		var initialVolume = track.Volume;
		var volumeMod = initialVolume - volume;
		Log.Info( "Fading volume from Track " + track.Music.ResourceName + " v:" + initialVolume + " to " + volume + " in " + seconds + " seconds" );
		for ( int i = 0; i < steps; i++ )
		{
			track.SetVolume( initialVolume - (i * volumeMod / steps) );
			//Log.Info( $"Curently fading track: {track.Music.ResourceName} to {track.Volume}" );
			await GameTask.DelaySeconds( seconds / steps );
		}
		if ( Stop )
			track.Stop();
		Log.Info( "Done fading" );
	}
}
