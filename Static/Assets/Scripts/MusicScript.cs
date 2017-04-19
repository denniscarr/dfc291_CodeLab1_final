using UnityEngine;
using System.Collections;

public class MusicScript : MonoBehaviour {

	public AudioClip[] tracks;
    AudioSource audioSource;
    int lastIndex;

	void Start () {
		audioSource = GetComponent<AudioSource> ();
		int clipIndex = Random.Range(0, tracks.Length);
        lastIndex = clipIndex;
		audioSource.clip = tracks [clipIndex];
	}

	void ChooseNewClip()
    {
        // Make sure the same track does not play twice in a row.
        int clipIndex = lastIndex;
        while (clipIndex == lastIndex)
        {
            clipIndex = Random.Range(0, tracks.Length);
        }

        lastIndex = clipIndex;
        audioSource.clip = tracks [clipIndex];
		audioSource.Play ();
	}
	
	void Update () {
		if (!audioSource.isPlaying) {
			ChooseNewClip ();
		}
	}
}
