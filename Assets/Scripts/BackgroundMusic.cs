using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundMusic : MonoBehaviour {
	public AudioSource AudioSource;
	public Slider volumeSlider;
	public Toggle MusicToggle;
	// Use this for initialization
	void Start () {
		//Add Slider listener
		volumeSlider.onValueChanged.AddListener(delegate {volumeSliderChanged(); });

		//Toggle Listeners
		MusicToggle.onValueChanged.AddListener(delegate {musicToggleChanged(); });
	}
	
	

	void volumeSliderChanged()
	{
		AudioSource.volume = volumeSlider.value;
	}

	void musicToggleChanged()
	{
		if (! MusicToggle.isOn) {
			AudioSource.volume = 0;
		} else {
			AudioSource.volume = volumeSlider.value;
		}
			
	}
}
