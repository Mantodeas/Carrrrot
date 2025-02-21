using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioClip bgm;
    public AudioClip seShoot;
    public AudioClip seDamage;
    public AudioClip seJump;
    public AudioClip seHurt;
    public AudioClip seThump;
    public AudioClip seBubble;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 播放音乐
    public void Play(string name, bool isLoop, float volume){   
        var clip = getAudioClip(name);
        if(clip != null){
            var audio = Pool.instance.Get("audio", transform).GetComponent<AudioSource>();
            audio.clip = clip;
            audio.loop = isLoop;
            audio.volume = volume;
            audio.Play();
            StartCoroutine(WaitingRelease(clip.length, audio));
        }
    }

    AudioClip getAudioClip(string name){
        switch(name){
            case "bgm":
                return bgm;
            case "shoot":
                return seShoot;
            case "damage":
                return seDamage;
            case "jump":
                return seJump;
            case "hurt":
                return seHurt;
            case "thump":
                return seThump;
            case "bubble":
                return seBubble;

            default:
                return null;
        }
    }

    // 对象池回收audio
    IEnumerator WaitingRelease(float time, AudioSource audio){  
        yield return new WaitForSeconds(time);
        Pool.instance.Release(audio.gameObject);
    }


}
