using UnityEngine;

public class Footsteps : MonoBehaviour
{
	[Header("Player")]
	public Yandere player;

	[Header("Audio")]
	private AudioSource audioSource;
	public AudioClip[] footsteps;

	[Header("Threshold")]
	public float downThreshold = 0.02f;
	public float upThreshold = 0.025f;
	bool footUp;

	private void Start()
	{
		audioSource = GetComponent<AudioSource>();
	}

	private void Update()
	{
		if (!footUp)
		{
			if (transform.position.y > player.transform.position.y + upThreshold)
			{
				footUp = true;
			}
		}
		else if (transform.position.y < player.transform.position.y + downThreshold)
		{
			if (footUp)
			{
				audioSource.clip = footsteps[UnityEngine.Random.Range(0, footsteps.Length)];
				audioSource.volume = player.Running ? 0.5f : 0.25f;
				audioSource.Play();
			}
			footUp = false;
		}
	}
}