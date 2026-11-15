
using UnityEngine;
using TMPro;

public class DialogueUI : MonoBehaviour
{
    [Header("References")]
    public GameObject dialoguePanel;
    public PlayerController playerController;
    public TMP_InputField messageInput;

    public bool IsOpen { get; private set; }

    void Start()
    {
        IsOpen = false;
        dialoguePanel.SetActive(false);
    }

    void Update()
    {
        // Временно открываем текстовый режим по E
        if (!IsOpen && Input.GetKeyDown(KeyCode.E))
        {
            OpenDialogue();
        }

        // Закрываем диалог по Escape
        if (IsOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseDialogue();
        }
    }

    public void OpenDialogue()
    {
        if (IsOpen)
            return;

        IsOpen = true;

        dialoguePanel.SetActive(true);

        playerController.SetControlsEnabled(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        messageInput.ActivateInputField();
    }

    public void CloseDialogue()
    {
        if (!IsOpen)
            return;

        IsOpen = false;

        dialoguePanel.SetActive(false);

        playerController.SetControlsEnabled(true);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
