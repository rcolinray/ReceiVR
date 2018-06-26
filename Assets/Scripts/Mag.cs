using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mag : MonoBehaviour
{
    int numRounds = 8;
    const int maxRounds = 8;
    Vector3[] roundPos;
    Quaternion[] roundRot;
    Vector3 oldPos;
    public Vector3 holdOffset;
    public Vector3 holdRotation;
    bool collided = false;
    AudioClip[] soundAddRound;
    AudioClip[] soundMagBounce;
    float lifeTime = 0.0f;
    enum MagLoadStage
    {
        None,
        PushingDown,
        AddingRound,
        RemovingRound,
        PushingUp
    };
    MagLoadStage magLoadStage = MagLoadStage.None;
    float magLoadProgress = 0.0f;
    bool disableInterp = true;

    public int NumRounds()
    {
        return numRounds;
    }

    public bool RemoveRound()
    {
        if (numRounds == 0)
        {
            return false;
        }
        Transform roundObj = transform.Find("round_" + numRounds);
        roundObj.GetComponent<Renderer>().enabled = false;
        --numRounds;
        return true;
    }

    public bool RemoveRoundAnimated()
    {
        if (numRounds == 0 || magLoadStage != MagLoadStage.None)
        {
            return false;
        }
        magLoadStage = MagLoadStage.RemovingRound;
        magLoadProgress = 0.0f;
        return true;
    }

    public bool IsFull()
    {
        return numRounds == maxRounds;
    }

    public bool AddRound()
    {
        if (numRounds >= maxRounds || magLoadStage != MagLoadStage.None)
        {
            return false;
        }
        magLoadStage = MagLoadStage.PushingDown;
        magLoadProgress = 0.0f;
        PlaySoundFromGroup(soundAddRound, 0.3f);
        ++numRounds;
        Transform roundObj = transform.Find("round_" + numRounds);
        roundObj.GetComponent<Renderer>().enabled = true;
        return true;
    }

    void Start()
    {
        oldPos = transform.position;
        numRounds = Random.Range(0, maxRounds);
        roundPos = new Vector3[maxRounds];
        roundRot = new Quaternion[maxRounds];
        for (int i = 0; i < maxRounds; i++)
        {
            Transform round = transform.Find("round_" + (i + 1));
            roundPos[i] = round.localPosition;
            roundRot[i] = round.localRotation;
            if (i < numRounds)
            {
                round.GetComponent<Renderer>().enabled = true;
            }
            else
            {
                round.GetComponent<Renderer>().enabled = false;
            }
        }
    }

    void PlaySoundFromGroup(AudioClip[] group, float volume)
    {
        if (group.Length == 0) { return; }
        int whichShot = Random.Range(0, group.Length);
        GetComponent<AudioSource>().PlayOneShot(group[whichShot], volume * PlayerPrefs.GetFloat("sound_volume", 1.0f));
    }

    void CollisionSound()
    {
        if (!collided)
        {
            collided = true;
            PlaySoundFromGroup(soundMagBounce, 0.3f);
        }
    }

    void FixedUpdate()
    {
        if (GetComponent<Rigidbody>() && !GetComponent<Rigidbody>().IsSleeping() && GetComponent<Collider>() && GetComponent<Collider>().enabled)
        {
            lifeTime += Time.deltaTime;
            RaycastHit hit;
            if (Physics.Linecast(oldPos, transform.position, out hit, 1))
            {
                transform.position = hit.point;
                transform.GetComponent<Rigidbody>().velocity *= -0.3f;
            }
            if (lifeTime > 2.0)
            {
                GetComponent<Rigidbody>().Sleep();
            }
        }
        else if (!GetComponent<Rigidbody>())
        {
            lifeTime = 0.0f;
            collided = false;
        }
        oldPos = transform.position;
    }

    void Update()
    {
        switch (magLoadStage)
        {
            case MagLoadStage.PushingDown:
                magLoadProgress += Time.deltaTime * 20.0f;
                if (magLoadProgress >= 1.0)
                {
                    magLoadStage = MagLoadStage.AddingRound;
                    magLoadProgress = 0.0f;
                }
                break;
            case MagLoadStage.AddingRound:
                magLoadProgress += Time.deltaTime * 20.0f;
                if (magLoadProgress >= 1.0)
                {
                    magLoadStage = MagLoadStage.None;
                    magLoadProgress = 0.0f;
                    for (int i = 0; i < numRounds; i++)
                    {
                        Transform obj = transform.Find("round_" + (i + 1));
                        obj.localPosition = roundPos[i];
                        obj.localRotation = roundRot[i];
                    }
                }
                break;
            case MagLoadStage.PushingUp:
                magLoadProgress += Time.deltaTime * 20.0f;
                if (magLoadProgress >= 1.0)
                {
                    magLoadStage = MagLoadStage.None;
                    magLoadProgress = 0.0f;
                    RemoveRound();
                    for (int i = 0; i < numRounds; i++)
                    {
                        Transform obj = transform.Find("round_" + (i + 1));
                        obj.localPosition = roundPos[i];
                        obj.localRotation = roundRot[i];
                    }
                }
                break;
            case MagLoadStage.RemovingRound:
                magLoadProgress += Time.deltaTime * 20.0f;
                if (magLoadProgress >= 1.0)
                {
                    magLoadStage = MagLoadStage.PushingUp;
                    magLoadProgress = 0.0f;
                }
                break;
        }
        float magLoadProgressDisplay = magLoadProgress;
        if (disableInterp)
        {
            magLoadProgressDisplay = Mathf.Floor(magLoadProgress + 0.5f);
        }
        switch (magLoadStage)
        {
            case MagLoadStage.PushingDown:
                Transform obj = transform.Find("round_1");
                obj.localPosition = Vector3.Lerp(transform.Find("point_start_load").localPosition,
                                                transform.Find("point_load").localPosition,
                                                magLoadProgressDisplay);
                obj.localRotation = Quaternion.Slerp(transform.Find("point_start_load").localRotation,
                                                    transform.Find("point_load").localRotation,
                                                    magLoadProgressDisplay);
                for (int i = 1; i < numRounds; i++)
                {
                    obj = transform.Find("round_" + (i + 1));
                    obj.localPosition = Vector3.Lerp(roundPos[i - 1], roundPos[i], magLoadProgressDisplay);
                    obj.localRotation = Quaternion.Slerp(roundRot[i - 1], roundRot[i], magLoadProgressDisplay);
                }
                break;
            case MagLoadStage.AddingRound:
                obj = transform.Find("round_1");
                obj.localPosition = Vector3.Lerp(transform.Find("point_load").localPosition,
                                                roundPos[0],
                                                magLoadProgressDisplay);
                obj.localRotation = Quaternion.Slerp(transform.Find("point_load").localRotation,
                                                    roundRot[0],
                                                    magLoadProgressDisplay);
                for (int i = 1; i < numRounds; i++)
                {
                    obj = transform.Find("round_" + (i + 1));
                    obj.localPosition = roundPos[i];
                }
                break;
            case MagLoadStage.PushingUp:
                obj = transform.Find("round_1");
                obj.localPosition = Vector3.Lerp(transform.Find("point_start_load").localPosition,
                                                transform.Find("point_load").localPosition,
                                                1.0f - magLoadProgressDisplay);
                obj.localRotation = Quaternion.Slerp(transform.Find("point_start_load").localRotation,
                                                    transform.Find("point_load").localRotation,
                                                    1.0f - magLoadProgressDisplay);
                for (int i = 1; i < numRounds; i++)
                {
                    obj = transform.Find("round_" + (i + 1));
                    obj.localPosition = Vector3.Lerp(roundPos[i - 1], roundPos[i], magLoadProgressDisplay);
                    obj.localRotation = Quaternion.Slerp(roundRot[i - 1], roundRot[i], magLoadProgressDisplay);
                }
                break;
            case MagLoadStage.RemovingRound:
                obj = transform.Find("round_1");
                obj.localPosition = Vector3.Lerp(transform.Find("point_load").localPosition,
                                                roundPos[0],
                                                1.0f - magLoadProgressDisplay);
                obj.localRotation = Quaternion.Slerp(transform.Find("point_load").localRotation,
                                                    roundRot[0],
                                                    1.0f - magLoadProgressDisplay);
                for (int i = 1; i < numRounds; i++)
                {
                    obj = transform.Find("round_" + (i + 1));
                    obj.localPosition = roundPos[i];
                    obj.localRotation = roundRot[i];
                }
                break;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        CollisionSound();
    }
}
