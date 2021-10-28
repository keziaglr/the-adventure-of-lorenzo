using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{

	public Text nameText;
	public Text dialogueText;
	public GameObject gameUI, dialogueUI, thirdpersoncam, maincam;
	public static bool dialogueActive = true;
	public Text alertText;

	public Animator animator;

	private Queue<string> sentences;

	// Use this for initialization
	void Start()
	{
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;
		sentences = new Queue<string>();
	}

	public void StartDialogue(Dialogue dialogue)
	{
		//Debug.Log("STart Dialogue " + PauseMenu.IsRestart);
        if (!PauseMenu.IsRestart) 
		{ 		
			animator.SetBool("IsOpen", true);

			nameText.text = dialogue.name;

			sentences.Clear();

			foreach (string sentence in dialogue.sentences)
			{
				sentences.Enqueue(sentence);
			}

			DisplayNextSentence();
        }
        else
        {
			GameInit();
        }
	}

	public void DisplayNextSentence()
	{
		if (sentences.Count == 0)
		{
			EndDialogue();
			return;
		}

		string sentence = sentences.Dequeue();
		StopAllCoroutines();
		StartCoroutine(TypeSentence(sentence));
	}

	IEnumerator TypeSentence(string sentence)
	{
		dialogueText.text = "";
		foreach (char letter in sentence.ToCharArray())
		{
			dialogueText.text += letter;
			yield return null;
		}
	}

	void EndDialogue()
	{
		animator.SetBool("IsOpen", false);
		GameInit();
	}

	void GameInit()
    {
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
		maincam.SetActive(false);
		thirdpersoncam.SetActive(true);
		dialogueUI.SetActive(false);
		gameUI.SetActive(true);
		dialogueActive = false;
		alertText.text = "";
	}

}
