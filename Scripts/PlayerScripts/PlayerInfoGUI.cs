using Godot;
using System;
using System.Collections.Generic;

public partial class PlayerInfoGUI : CanvasLayer
{
	PlayerManager playerManager;
	GameManager gameManager;
	[Export] public PackedScene playerCard {get; private set;}
	[Export] public HBoxContainer hBox {get; private set;}
	public Dictionary<int, PlayerCard> playerCards = new Dictionary<int, PlayerCard>();
	public override void _Ready()
	{
		playerManager = PlayerManager.Instance;
		gameManager = GameManager.Instance;
	}
	public void AddCard(int playerIndex)
	{
		PlayerCard currentPlayer = (PlayerCard)playerCard.Instantiate();
		hBox.AddChild(currentPlayer);
		hBox.MoveChild(currentPlayer, playerIndex);
		playerCards.Add(playerIndex, currentPlayer);
		if (gameManager.gameMode is StockBattle)
		{
			playerCards[playerIndex].SetAll(
				"Lives: " + playerManager.playerList[playerIndex].GetLives().ToString(),
				playerManager.playerList[playerIndex].GetDamageTaken().ToString(),
				playerManager.playerList[playerIndex].GetColor()
			);
		}
		else if (gameManager.gameMode is Elimination)
		{
			playerCards[playerIndex].SetAll(
				"Kills: " + playerManager.playerList[playerIndex].GetKills().ToString(),
				playerManager.playerList[playerIndex].GetDamageTaken().ToString(),
				playerManager.playerList[playerIndex].GetColor()
			);
		}
	}
	public void ResetCardInfo(int playerIndex){
		if (gameManager.gameMode is StockBattle)
		{
			playerCards[playerIndex].SetAll(
				"Lives: " + playerManager.playerList[playerIndex].GetLives().ToString(),
				playerManager.playerList[playerIndex].GetDamageTaken().ToString(),
				playerManager.playerList[playerIndex].GetColor()
			);
		}
		else if (gameManager.gameMode is Elimination)
		{
			playerCards[playerIndex].SetAll(
				"Kills: " + playerManager.playerList[playerIndex].GetKills().ToString(),
				playerManager.playerList[playerIndex].GetDamageTaken().ToString(),
				playerManager.playerList[playerIndex].GetColor()
			);
		}
	}
	public void RemoveCard(int playerIndex){
		hBox.RemoveChild(playerCards[playerIndex]);
		playerCards.Remove(playerIndex);
	}
	public void RemoveAll(){
		foreach(KeyValuePair<int,PlayerCard> p in playerCards){
			hBox.RemoveChild(playerCards[p.Key]);
			playerCards.Remove(p.Key);
		}
	}
}
