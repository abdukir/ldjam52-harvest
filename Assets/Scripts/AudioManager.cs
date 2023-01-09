using UnityEngine.Audio;
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{

	public Sound[] sounds;
	public static AudioManager Instance { set; get; }


	private void OnEnable()
	{
		Play("bgmusic");
	}
	// Use this for initialization
	void Awake()
	{
		Instance = this;
		foreach (Sound s in sounds)
		{
			s.source = gameObject.AddComponent<AudioSource>();
			s.source.clip = s.clip;

			s.source.volume = s.volume;
			s.source.pitch = s.pitch;
			s.source.loop = s.loop;
			s.source.spatialBlend = s.spatialBlend;
		}
		DontDestroyOnLoad(this.gameObject);
	}

	// Update is called once per frame
	public void Play(string name)
	{
		Sound s = Array.Find(sounds, sound => sound.name == name);
		if (s == null)
		{
			Debug.LogWarning(name + "Adlı Ses Bulunamadı!");
			return;
		}
		s.source.Play();
	}
	public void Stop(string name)
	{
		Sound s = Array.Find(sounds, sound => sound.name == name);
		if (s == null)
		{
			Debug.LogWarning(name + "Adlı Ses Bulunamadı!");
			return;
		}
		s.source.Stop();
	}
}
