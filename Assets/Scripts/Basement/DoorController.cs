using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DoorController : MonoBehaviour
{
    public GameObject character, mainSound, basementSound, victorySound;
    private Animator animator;
    private List<Animator> childAnimatorList = new List<Animator>();
    public Text alertTxt;
    public EnemyChase boss;
    public DoorController doorCon;
    public static bool victoryFlag = false;

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
        if (Vector3.Distance(character.transform.position, transform.position) <= 1f)
        {
            if (doorCon.name.Equals("OpeningDoor") && ControllerCharacter.coreItems >= 9)
            {
                basementSound.SetActive(true);
                mainSound.SetActive(false);
                animator.SetBool("character_nearby", true);
                foreach (Animator a in childAnimatorList)
                {
                    a.SetBool("isOpening", true);
                }
            }else if (doorCon.name.Equals("BossDoor")){
                animator.SetBool("character_nearby", true);
                foreach (Animator a in childAnimatorList)
                {
                    a.SetBool("isOpening", true);
                }
            }else if (doorCon.name.Equals("SpaceshipDoor"))
            { //&& EnemyChase.isDead
                basementSound.SetActive(false);
                victorySound.SetActive(true);
                animator.SetBool("character_nearby", true);
                foreach (Animator a in childAnimatorList)
                {
                    a.SetBool("isOpening", true);
                }

                victoryFlag = true;
            }
        }
        else
        {
            animator.SetBool("character_nearby", false);
            
            foreach (Animator a in childAnimatorList)
            {
                a.SetBool("isOpening", false);
            }
        }

    }
}
