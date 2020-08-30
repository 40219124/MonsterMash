using UnityEngine;

public class Agent : MonoBehaviour
{
	protected bool IsOurTurn { get; private set;}
	public Body Body;

	public void OnTurnStart(bool isOurTurn)
	{
		IsOurTurn = isOurTurn;
	}
}