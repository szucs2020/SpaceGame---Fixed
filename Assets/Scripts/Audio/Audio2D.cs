/*
 * Audio2D.cs
 * Authors: Christian
 * Description: This script allows us to manage and play non spatial sounds
 */
using UnityEngine;
using System.Collections;

[System.Serializable]
public class Sound {

    public string name;
    public AudioClip clip;

    [Range(0f, 1f)]
    public float volume = 0.7f;
    [Range(0.5f, 1.5f)]
    public float pitch = 1.0f;

    private AudioSource source;

    public void SetSource(AudioSource src) {
        source = src;
        source.clip = clip;
    }

    public void Play() {
        source.volume = volume;
        source.pitch = pitch;
        source.Play();
    }

    public void Stop() {
        source.Stop();
    }

}

public class Audio2D : MonoBehaviour {

    [SerializeField]
    Sound[] sounds;

    public static Audio2D singleton = null;

    void Awake() {
        if (singleton != null && singleton != this) {
            Destroy(this.gameObject);
            return;
        } else {
            singleton = this;
        }
        DontDestroyOnLoad(this);
    }

    void Start() {
        for (int i = 0; i < sounds.Length; i++) {
            GameObject g = new GameObject("Sound_" + i + "_" + sounds[i].name);
            g.transform.SetParent(this.transform);
            sounds[i].SetSource(g.AddComponent<AudioSource>());
        }
    }

    public void PlaySound(string n) {
        for (int i = 0; i < sounds.Length; i++) {
            if (sounds[i].name == n) {
                sounds[i].Play();
                return;
            }
        }
        print("ERROR PlaySound(): No sound with name: " + n);
    }

    public void StopSound(string n) {
        for (int i = 0; i < sounds.Length; i++) {
            if (sounds[i].name == n) {
                sounds[i].Stop();
                return;
            }
        }
        print("ERROR PauseSound(): No sound with name: " + n);
    }
}
