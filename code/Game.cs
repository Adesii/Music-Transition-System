using Equivalent.Player;

namespace Adesi;

public partial class Game : Sandbox.Game
{
	public static Game Instance => Sandbox.Game.Current as Game;

	public Game()
	{
	}

	public override void ClientJoined( Client client )
	{
		base.ClientJoined( client );

		var player = new EquivalentPlayer();
		client.Pawn = player;

		player.InitialRespawn();
	}
}
