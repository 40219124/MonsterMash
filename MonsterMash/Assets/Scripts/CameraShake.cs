using UnityEngine;
using System;


//code from anthony
public class CameraShake : MonoBehaviour
{
	public float Duration = 0.5f;
	public float Speed = 1.0f;
	public float Magnitude = 0.1f;

	public AnimationCurve Falloff;

	float Elapsed;
	float Intensity;
	Vector3 OriginalCamPos;

	void Start()
	{
		Intensity = 0.0f;
		Elapsed = 1000000;
		OriginalCamPos = transform.position;
	}

	public void PlayShake(float intensity)
	{
		Intensity = intensity;
		Elapsed = 0;
	}

	void Update()
	{
		Elapsed += Time.deltaTime;

		float percentComplete = Elapsed / Duration;

		float x = OriginalCamPos.x;
		float y = OriginalCamPos.y;
		if (percentComplete <= 1)
		{
			percentComplete = Falloff.Evaluate(percentComplete);

			float damper = 1.0f - Mathf.Clamp(2.0f * percentComplete - 1.0f, 0.0f, 1.0f);

			float alpha = Speed * percentComplete * Intensity;

			x = Mathf.PerlinNoise(alpha, 0) * 2.0f - 1.0f;
			y = Mathf.PerlinNoise(0, alpha) * 2.0f - 1.0f;

			x *= Magnitude * damper * Intensity;
			y *= Magnitude * damper * Intensity;

		}

		transform.position = new Vector3(x, y, OriginalCamPos.z);
	}
}