using Adesi.Music;

namespace Adesi.Entities;

[HammerEntity, EditorSprite( "materials/editor/ai_sound.vmat" )]
public partial class MusicManager : Entity
{
	[Property, ResourceType( "track" )]
	public string SelectedTrack { get; set; }

	[Property]
	public bool PlayOnStart { get; set; }

	[Property]
	public bool PlayOnlyOnLocalPlayer { get; set; } = true;

	public override void Spawn()
	{
		base.Spawn();
		Transmit = TransmitType.Always;
	}

	[Input]
	public void PlaySelectedTrack()
	{
		var track = Track.GetFromPath( SelectedTrack );
		PlayTrackClient( track.ResourceName );
	}

	[Input]
	public void PlayTrack( Entity activator, string trackName )
	{
		if ( PlayOnlyOnLocalPlayer )
			PlayTrackClient( To.Single( activator ), trackName );
		else
			PlayTrackClient( trackName );
	}

	[Input]
	public void PlayTrackIndexNumber( Entity activator, int number )
	{
		if ( PlayOnlyOnLocalPlayer )
			PlayTrackIndexClient( To.Single( activator ), number );
		else
			PlayTrackIndexClient( number );
	}

	[Input]
	public void NextTrack( Entity activator )
	{
		if ( PlayOnlyOnLocalPlayer )
			NextTrackClient( To.Single( activator ) );
		else
			NextTrackClient();
	}

	[Input]
	public void PreviousTrack( Entity activator )
	{
		if ( PlayOnlyOnLocalPlayer )
			PreviousTrackClient( To.Single( activator ) );
		else
			PreviousTrackClient();
	}

	[Input]
	public void StopTrack( Entity activator )
	{
		if ( PlayOnlyOnLocalPlayer )
			StopTrackClient( To.Single( activator ) );
		else
			StopTrackClient();
	}




	[ClientRpc]
	public void PlayTrackClient( string trackName )
	{

		var track = Track.Get( trackName );
		if ( track == null )
		{
			Log.Warning( $"Track {trackName} not found" );
			return;
		}
		SoundtrackManager.PlayTrack( track );
	}

	[ClientRpc]
	public void PlayTrackIndexClient( int tracknumber )
	{
		SoundtrackManager.PlayTrackIndex( tracknumber );
	}
	[ClientRpc]
	public void NextTrackClient()
	{
		SoundtrackManager.Next();
	}

	[ClientRpc]
	public void PreviousTrackClient()
	{
		SoundtrackManager.Previous();
	}

	[ClientRpc]
	public void StopTrackClient()
	{
		SoundtrackManager.Stop();
	}

	public override void ClientSpawn()
	{
		base.ClientSpawn();
		if ( PlayOnStart )
		{
			WaitForStart();
		}
	}

	public async void WaitForStart()
	{
		await GameTask.DelayRealtime( 200 );
		var track = Track.GetFromPath( SelectedTrack );
		PlayTrackClient( track.ResourceName );
	}




}
