using Adesi.Music;

namespace Adesi;

partial class Pawn : AnimatedEntity
{

	public override void Spawn()
	{
		base.Spawn();

		SetModel( "models/sbox_props/watermelon/watermelon.vmdl" );

		EnableDrawing = true;
		EnableHideInFirstPerson = true;
		EnableShadowInFirstPerson = true;
	}

	Sound Metronome;

	public override void Simulate( Client cl )
	{
		base.Simulate( cl );

		Rotation = Input.Rotation;
		EyeRotation = Rotation;

		var movement = new Vector3( Input.Forward, Input.Left, Input.Down( InputButton.Jump ) ? 1 : Input.Down( InputButton.Duck ) ? -1 : 0 ).Normal;

		Velocity = Rotation * movement;

		Velocity *= Input.Down( InputButton.Run ) ? 1000 : 200;

		MoveHelper helper = new MoveHelper( Position, Velocity );
		helper.Trace = helper.Trace.Size( 16 );
		if ( helper.TryMove( Time.Delta ) > 0 )
		{
			Position = helper.Position;
		}

		if ( Input.Pressed( InputButton.Use ) && IsClient )
		{
			SoundtrackManager.PlayTrack( Track.Get( "changingtrack" ), true );

		}
		if ( Input.Pressed( InputButton.Menu ) && IsClient )
		{
			Log.Info( "Playing Next Track" );
			SoundtrackManager.Next();
		}
		if ( Input.Pressed( InputButton.Reload ) && IsClient )
		{
			Metronome = Sound.FromScreen( "metronome_128bpm" );
		}
		if ( Input.Pressed( InputButton.Menu ) && IsClient )
		{
			Metronome.Stop();
		}
	}

	public override void FrameSimulate( Client cl )
	{
		base.FrameSimulate( cl );

		Rotation = Input.Rotation;
		EyeRotation = Rotation;
	}
}
