public class Settings
{
	public const bool AlwaysGoToPickerPostBattle = false;
	public const bool ShowStatsForAi = false;
	public const bool DpadOnlyCombat = true;
	public const float ActionTimeForgiveness = 0.2f;
	public const float TurnTime = 6.0f;
	public const float BattleIntroMaxTime = 30.0f;
	public const float TurnTransitionTime = 1.5f;
	public const float PreShowBattleUiTime = 0.5f;
	public const float PreQueueActionTimePercentage = 0.4f; //this is max 0.5
	public const float PostPickHangTime = 0.25f;

	public const float NoActionAvailableSpeedMultiplier = 5f;
	public const float AiTurnTimeSpeedMultiplier = 1.5f;

	public const float AiPickAttackerMinTime = 0f;
	public const float AiPickAttackerMaxTime = 0.5f;

	public const float AiPickTargetMinTime = 0f;
	public const float AiPickTargetMaxTime = 0.5f;


	public const int MapSize = 5;
	public const int MaxRooms = 8;

	public const float ChanceOfHealingPotion = 33f;
	public const float ChanceOfALimbDrop = 33f;

	public const float UpgradeBodyPartHealthMultiplier = 1.1f;
	public const float UpgradeBodyPartDamageMultiplier = 1.1f;
	public const float UpgradeBodyPartArmorMultiplier = 1.1f;

	public const string SceneMain = "MainScene";
	public const string SceneOverworld = "Overworld";
	public const string SceneOverworldMemory = "OverworldMemory";
	public const string SceneBattle = "Louie";
	public const string SceneBodyPartPicker = "BodyPartPicker";
	public const string SceneGameOver = "GameOver";
	public const string SceneTitle = "TitleScreen";
	public const string TransitionOverlay = "ScreenTransitions";

}