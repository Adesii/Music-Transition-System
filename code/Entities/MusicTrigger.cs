using Adesi.Music;

namespace Adesi.Entities;

[AutoApplyMaterial( "materials/tools/toolstrigger.vmat" )]
[Solid, VisGroup( VisGroup.Trigger ), HideProperty( "enable_shadows" )]
[Title( "Music Trigger" ), Icon( "select_all" ), HammerEntity]
public partial class MusicTrigger : BaseTrigger
{
	[Property, ResourceType( "track" )]
	public string SelectedTrack { get; set; }

	[Property]
	public bool TriggerOnce { get; set; } = true;

	private bool Triggered;

	public override void Spawn()
	{
		base.Spawn();
		Transmit = TransmitType.Always;
	}

	public override void OnTouchStart( Entity toucher )
	{
		base.OnTouchStart( toucher );
		if ( Host.IsClient )
			return;
		var track = Track.GetFromPath( SelectedTrack );
		PlayTrackClient( To.Single( toucher ), track.ResourceName );
	}

	[ClientRpc]
	public void PlayTrackClient( string trackName )
	{
		if ( Triggered ) return;
		Triggered = true;
		var track = Track.Get( trackName );
		if ( track == null )
		{
			Log.Warning( $"Track {trackName} not found" );
			return;
		}
		SoundtrackManager.PlayTrack( track );
	}


}
