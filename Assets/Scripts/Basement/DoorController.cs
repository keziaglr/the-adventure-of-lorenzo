using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DoorController : MonoBehaviour
{
    public GameObject character;
    private Animator animator;
    private List<Animator> childAnimatorList = new List<Animator>();
    public Text alertTxt;

    // Start is called before the first frame update
    private void Start()
    {
        animator = GetComponent<Animator>();
        GameObject child = transform.GetChild(0).gameObject;
        for (int i = 0; i < child.transform.childCount; i++)
        {
            childAnimatorList.Add(child.transform.GetChild(i).GetComponent<Animator>());
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(character.transform.position, transform.position) <= 1f && ControllerCharacter.coreItems >= 9)
        {
            animator.SetBool("character_nearby", true);
            foreach (Animator a in childAnimatorList)
            {
                a.SetBool("isOpening", true);
            }
        }
        else
        {
            //if(ControllerCharacter.coreItems < 9)
            //{
            //    alertTxt.text = "Not Enough Core Items";
            //}
            animator.SetBool("character_nearby", false);
            SoundManager.PlaySound("DoorSFX");
            foreach (Animator a in childAnimatorList)
            {
                a.SetBool("isOpening", false);
            }
        }

    }
}
