using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class PlayerActionAnimator : MonoBehaviour
{
    public SpriteRenderer Scyte;
    public SpriteRenderer WaterCan;
    public Transform RightHand;
    private Quaternion initalRotation;
    private Animator animator;


    private void Start()
    {
        animator = GetComponent<Animator>();
        Scyte.DOFade(0, 0);
        WaterCan.DOFade(0, 0);
        initalRotation = Scyte.transform.rotation;
    }
    public void SetSpeed(float speed)
    {
        //Debug.Log("Setting speed to " + speed);
        animator.SetFloat("Speed", speed);
    }

    public void SetAwait(float time)
    {
        animator.SetBool("Await", true);
        StartCoroutine(StopAwait(time));
    }

    IEnumerator StopAwait(float time)
    {
        yield return new WaitForSeconds(time);
        animator.SetBool("Await", false);
    }

    public void ScyteAction()
    {
        SetAwait(0.3f);
        Scyte.DOFade(1, 0);
        //rotates arouns RightHand 45 degrees to left
        Scyte.transform.DORotate(new Vector3(0, 0, 60), 0.3f, RotateMode.LocalAxisAdd).onComplete += () =>
        {
            Scyte.DOFade(0, 0).onComplete += () =>
            {
                Scyte.transform.DORotate(new Vector3(0, 0, -60), 0f, RotateMode.LocalAxisAdd);
            };
           
        };

    }

    public void WaterAction()
    {
        SetAwait(0.3f);
        WaterCan.DOFade(1, 0.3f).onComplete += () =>
        {
            WaterCan.DOFade(0, 0.01f);
        };
    }
}
