
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class ChatManager : MonoBehaviour
{
    public TMP_InputField messageInput;
    public TMP_Text chatLog;
    public Button sendButton;
    public ScrollRect chatScrollRect;

    private string modelName = "qwen2.5:3b";
    private string ollamaUrl = "http://localhost:11434/api/chat";

    private List<ChatMessage> history = new List<ChatMessage>();

    void Start()
    {
        history.Add(new ChatMessage(
            "system",
            "Ты — загадочный персонаж игры «Амнезия». " +
            "Отвечай по-русски, естественно и не слишком длинно. " +
            "Запоминай информацию, которую игрок сообщает о себе, " +
            "и учитывай предыдущие сообщения в текущем разговоре."
        ));

        sendButton.onClick.AddListener(SendMessageToNPC);

        messageInput.onSubmit.AddListener(delegate
        {
            SendMessageToNPC();
        });
    }

    void SendMessageToNPC()
    {
        string message = messageInput.text.Trim();

        if (message == "")
            return;

        chatLog.text += "\n\nТы: " + message;
        ScrollToBottom();

        history.Add(new ChatMessage("user", message));

        messageInput.text = "";
        sendButton.interactable = false;

        StartCoroutine(GetNPCResponse());
    }

    IEnumerator GetNPCResponse()
    {
        string json = JsonUtility.ToJson(
            new OllamaChatRequest(modelName, history.ToArray(), false)
        );

        byte[] body = Encoding.UTF8.GetBytes(json);

        using (UnityWebRequest request = new UnityWebRequest(
            ollamaUrl, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(body);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.timeout = 120;

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                OllamaChatResponse result =
                    JsonUtility.FromJson<OllamaChatResponse>(
                        request.downloadHandler.text
                    );

                string answer = result.message.content;

                history.Add(new ChatMessage("assistant", answer));

                chatLog.text += "\nNPC: " + answer;
                ScrollToBottom();
            }
            else
            {
                chatLog.text += "\nNPC: Не получилось ответить. " +
                    request.error;

                Debug.LogError(request.downloadHandler.text);

                // Удаляем неотвеченное сообщение из истории,
                // чтобы не отправлять его повторно при следующем запросе.
                if (history.Count > 1 &&
                    history[history.Count - 1].role == "user")
                {
                    history.RemoveAt(history.Count - 1);
                }
            }
        }

        sendButton.interactable = true;
        messageInput.ActivateInputField();
    }

    void ScrollToBottom()
    {
        if (chatScrollRect == null)
            return;

        Canvas.ForceUpdateCanvases();
        chatScrollRect.verticalNormalizedPosition = 0f;
        Canvas.ForceUpdateCanvases();
    }

    [System.Serializable]
    public class ChatMessage
    {
        public string role;
        public string content;

        public ChatMessage(string role, string content)
        {
            this.role = role;
            this.content = content;
        }
    }

    [System.Serializable]
    public class OllamaChatRequest
    {
        public string model;
        public ChatMessage[] messages;
        public bool stream;

        public OllamaChatRequest(
            string model,
            ChatMessage[] messages,
            bool stream)
        {
            this.model = model;
            this.messages = messages;
            this.stream = stream;
        }
    }

    [System.Serializable]
    public class OllamaChatResponse
    {
        public ChatMessage message;
    }
}
