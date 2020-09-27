using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProfileStat : MonoBehaviour
{
	public float Hight;
	[SerializeField] TextMesh Label;
	[SerializeField] NumberRender Value;
	[SerializeField] Animator Animator;

	public void Setup(PlayerProfile.StatValue statInfo, int index)
	{
		Label.text = statInfo.StatName;
		Value.SetNumber((int)statInfo.Value);
		transform.position = new Vector3(0, -Hight * index, 0);
	}

	public void SetSelected(bool selected)
	{
		Animator.SetBool("Selected", selected);
	}

	public void Show()
	{
		Animator.SetTrigger("Intro");
	}

}
