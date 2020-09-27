using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverManager : AdditiveSceneManager
{	

	[SerializeField] Animator GameOverAnimator;

	[SerializeField] AudioClip LostAudioClip;
	[SerializeField] AudioClip WonAudioClip;

	[SerializeField] PlayerProfileStat ProfileStatPrefab;
	[SerializeField] Transform ProfileStatHolder;
	List<PlayerProfileStat> StatsList = new List<PlayerProfileStat>();
	

	bool IntroDone = false;
	bool triggered = false;

	int SelectedIndex = 0;

	public void Setup(bool wonTheGame)
	{
		MMLogger.Log($"GameOver wonTheGame: {wonTheGame}");
		GameOverAnimator.SetBool("Won", wonTheGame);

		var audioClip = wonTheGame ? WonAudioClip : LostAudioClip;
		AudioSource.PlayClipAtPoint(audioClip, transform.position);

		StatsList.Clear();
		int index = 0;
		foreach (var statInfo in OverworldMemory.PlayerProfile.ProfileStats.Values)
		{
			var stat = Instantiate(ProfileStatPrefab, ProfileStatHolder);
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
		yield return new WaitForSeconds(1f);

		foreach (var stat in StatsList)
		{
			stat.Show();
			yield return new WaitForSeconds(0.5f);
		}
		IntroDone = true;
	}

	void Update()
	{
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
		else if (SimpleInput.GetInputState(EInput.dpadUp) == EButtonState.Pressed)
		{
			newIndex -= 1;
		}
		else if (SimpleInput.GetInputState(EInput.dpadDown) == EButtonState.Pressed)
		{
			newIndex += 1;
		}
		
		SetHolderPos(newIndex);
	}

	void SetHolderPos(int selectedIndex)
	{
		StatsList[SelectedIndex].SetSelected(false);
		selectedIndex = Mathf.Max(selectedIndex, 0);
		SelectedIndex = Mathf.Min(selectedIndex, StatsList.Count-1);

		StatsList[SelectedIndex].SetSelected(true);

		var yPos = ProfileStatPrefab.Hight * SelectedIndex;
		yPos = Mathf.Max(yPos, ProfileStatPrefab.Hight*2);
		yPos = Mathf.Min(yPos, (StatsList.Count-2)*ProfileStatPrefab.Hight);

		ProfileStatHolder.transform.position = new Vector3(0, yPos, 0);
	}
}
