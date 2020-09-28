using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverManager : AdditiveSceneManager
{	

	[SerializeField] Animator GameOverAnimator;

	[SerializeField] AudioClip LostAudioClip;
	[SerializeField] AudioClip WonAudioClip;
	[SerializeField] AudioClip MoveAudioClip;

	[SerializeField] PlayerProfileStat ProfileStatPrefab;
	[SerializeField] Transform ProfileStatHolder;
	List<PlayerProfileStat> StatsList = new List<PlayerProfileStat>();

	[SerializeField] NumberRender TotalScoreNumber;
	

	bool IntroDone = false;
	bool triggered = false;

	int SelectedIndex = 0;

	void Awake()
	{
		//Setup(true);
	}

	public void Setup(bool wonTheGame)
	{
		MMLogger.Log($"GameOver wonTheGame: {wonTheGame}");
		GameOverAnimator.SetBool("Won", wonTheGame);

		var audioClip = wonTheGame ? WonAudioClip : LostAudioClip;
		AudioSource.PlayClipAtPoint(audioClip, transform.position);
		int score = 1234;

		var profileStats = new Dictionary<eStatType, PlayerProfile.StatValue>();

		if (OverworldMemory.PlayerProfile?.ProfileStats != null)
		{
			profileStats = OverworldMemory.PlayerProfile.ProfileStats;
			score = OverworldMemory.PlayerProfile.GetScore();
		}

		TotalScoreNumber.UseLargeNumbers = true;
		TotalScoreNumber.SetNumber(score);

		//this is just for debug
		if (profileStats.Count == 0)
		{
			for (int i = 0; i < 10; i++)
			{
				var profileStat = new PlayerProfile.StatValue("temp", i*2);
				profileStats[(eStatType)i] = profileStat;
			}
		}

		StatsList.Clear();
		int index = 0;
		foreach (var statInfo in profileStats.Values)
		{
			var stat = Instantiate(ProfileStatPrefab, ProfileStatHolder);
			stat.transform.localPosition = Vector3.zero;
			stat.Setup(statInfo, index);
			index += 1;
			StatsList.Add(stat);
		}

		ProfileStatPrefab.gameObject.SetActive(false);
		SelectedIndex = 0;
		SetHolderPos(SelectedIndex);
		StartCoroutine(Intro());
	}

	IEnumerator Intro()
	{
		yield return new WaitForSeconds(2f);

		int loop = 0;
		foreach (var stat in StatsList)
		{
			stat.Show();
			yield return new WaitForSeconds(0.3f);
			if (loop >= 3)
			{
				IntroDone = true;
			}
			loop += 1;
		}
		IntroDone = true;
	}

	float HoldCoolDown = 0;
	void Update()
	{
		HoldCoolDown += Time.deltaTime;

		if (!IntroDone)
		{
			return;
		}

		int newIndex = SelectedIndex;
		if(!triggered && SimpleInput.GetInputState(EInput.A) == EButtonState.Released)
		{
			FlowManager.Instance.TransToTitle(Settings.SceneGameOver);
			triggered = true;
		}
		else 
		{
			if (SimpleInput.GetInputState(EInput.dpadUp) == EButtonState.Pressed ||
				(SimpleInput.GetInputState(EInput.dpadUp) == EButtonState.Held && 
				SimpleInput.GetTimeInState(EInput.dpadUp) > 0.1f &&
				HoldCoolDown >= 0.25f))
			{
				newIndex -= 1;
				HoldCoolDown = 0;
			}
			if (SimpleInput.GetInputState(EInput.dpadDown) == EButtonState.Pressed ||
				(SimpleInput.GetInputState(EInput.dpadDown) == EButtonState.Held && 
				SimpleInput.GetTimeInState(EInput.dpadDown) > 0.1f &&
				HoldCoolDown >= 0.25f))
			{
				newIndex += 1;
				HoldCoolDown = 0;
			}
		}
		
		SetHolderPos(newIndex);
	}

	void SetHolderPos(int index)
	{
		StatsList[SelectedIndex].SetSelected(false);
		index = Mathf.Max(index, 0);
		index = Mathf.Min(index, StatsList.Count-1);

		if (SelectedIndex != index)
		{
			AudioSource.PlayClipAtPoint(MoveAudioClip, Vector3.zero);
		}
		SelectedIndex = index;
		StatsList[SelectedIndex].SetSelected(true);

		var yPos = ProfileStatPrefab.Hight * SelectedIndex;
		yPos = Mathf.Max(yPos, 0);
		yPos = Mathf.Min(yPos, (StatsList.Count-2)*ProfileStatPrefab.Hight);

		ProfileStatHolder.transform.localPosition = new Vector3(0, yPos, 0);
	}
}
