/* Quellcode zum iX-Artikel: ChatGPT in eigenen Apps
 * Autor: Daniel Basler
 * Erschienen im iX Magazin 04/2023 der Heise Medien GmbH & Co. KG
 * Quelle: iX-GitHub: https://github.com/ix-magazin
 */

// Listing 2: Implementiern der Klasse RequestGPT

using System.Text.Json.Serialization;

namespace ExampleChatGPTApplication
{
    public class RequestGPT
    {
        public RequestGPT() { }        
       
        [JsonPropertyName("model")]
        public string? Model { get; set; }       

        [JsonPropertyName("prompt")]
        public string? Prompt { get; set; }         
       
        [JsonPropertyName("max_tokens")]
        public int MaxTokens { get; set; }        
       
        [JsonPropertyName("temperature")]
        public float Temperature { get; set; }
                
        [JsonPropertyName("top_p")]
        public float TopP { get; set; }        
        
        [JsonPropertyName("presence_penalty")]
        public float PresencePenalty { get; set; }        
        
        [JsonPropertyName("frequency_penalty")]
        public float FrequencyPenalty { get; set; }
        
    }
}

// Listing 3: Implementieren der Klasse ResponseGPT

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ExampleChatGPTApplication
{
    public class ResponseGPT
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("object")]        
        public string? @Object { get; set; }

        [JsonPropertyName("created")]
        public int Created { get; set; }

        [JsonPropertyName("model")]
        public string? Model { get; set; }

        [JsonPropertyName("choices")]
        public List<ChatGPTChoice>? Choices { get; set; }

        [JsonPropertyName("usage")]
        public ChatGPTUsage? Usage { get; set; }
    }
}

public class ChatGPTUsage
{
    [JsonPropertyName("prompt_tokens")]
    public int PromptTokens { get; set; }

    [JsonPropertyName("completion_tokens")]
    public int CompletionTokens { get; set; }

    [JsonPropertyName("total_tokens")]
    public int TotalTokens { get; set; }
}

public class ChatGPTChoice
{    
    [JsonPropertyName("text")]
    public string? Text { get; set; }

    [JsonPropertyName("index")]
    public int Index { get; set; }

    [JsonPropertyName("logprobs")]
    public object? LogProbabilities { get; set; }

    [JsonPropertyName("finish_reason")]
    public string? FinishReason { get; set; }
}

// Listing 4: Implementieren der Klasse RequestChatGPT

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ExampleChatGPTApplication
{
    public class RequestChatGPT
    {
        [JsonPropertyName("model")]
        public string Model { get; set; } = null!;
                
        [JsonPropertyName("messages")]
        public IList<ChatCompletionMessage> Messages { get; set; } = new List<ChatCompletionMessage>();
        
        [JsonPropertyName("temperature")]
        public float? Temperature { get; set; }
        
        [JsonPropertyName("top_p")]
        public float? TopP { get; set; }        
        
        [JsonPropertyName("stream")]
        public bool Stream { get; set; }       
        
        [JsonPropertyName("max_tokens")]
        public int? MaxTokens { get; set; }
        
        [JsonPropertyName("presence_penalty")]
        public float? PresencePenalty { get; set; }
        
        [JsonPropertyName("frequency_penalty")]
        public float? FrequencyPenalty { get; set; }        
    }
}

// Listing 5: Implementieren der Klasse ResponseChatGPT

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ExampleChatGPTApplication
{
    public class CreateChatResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = null!;

        [JsonPropertyName("object")]
        public string Object { get; set; } = null!;

        [JsonPropertyName("created")]
        public int Created { get; set; }

        [JsonPropertyName("model")]
        public string Model { get; set; } = null!;

        [JsonPropertyName("choices")]
        public List<ChatCompletionChoice> Choices { get; set; } = new();

        [JsonPropertyName("usage")]
        public ChatCompletionUsage? Usage { get; set; }
    }
    public class ChatCompletionChoice
    {
        [JsonPropertyName("delta")]
        public ChatCompletionMessage? Delta
        {
            get => Message;
            set => Message = value;
        }

        [JsonPropertyName("message")]
        public ChatCompletionMessage? Message { get; set; }

        [JsonPropertyName("index")]
        public int Index { get; set; }

        [JsonPropertyName("finish_reason")]
        public string FinishReason { get; set; } = null!;
    }
    public class ChatCompletionUsage
    {
        [JsonPropertyName("prompt_tokens")]
        public int PromptTokens { get; set; }

        [JsonPropertyName("completion_tokens")]
        public int CompletionTokens { get; set; }

        [JsonPropertyName("total_tokens")]
        public int TotalTokens { get; set; }
    }
}

// Listing 6: Implementieren der Klasse ChatCompletionMessage

using System.Text.Json.Serialization;

namespace ExampleChatGPTApplication
{
    public class ChatCompletionMessage
    {
        public ChatCompletionMessage(string role, string content)
        {
            Role = role;
            Content = content;
        }

        [JsonPropertyName("role")]
        public string Role { get; set; }

        [JsonPropertyName("content")]
        public string Content { get; set; }
    }
}

// Listing 7: Implementieren der Zugriffslogik auf die API

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ExampleChatGPTApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string OpenAI_ApiKey = "Your API-KEY";    
        string modelName = string.Empty;
        int maxTokens = 2048;
        string user = "Anfrage > ";
        string textGPT = "Antwort GPT > ";
        bool isChatGPTModel = false;
        
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if(OpenAI_ApiKey == string.Empty)
            {
                MessageBox.Show("Bitte OpenAI API Key eintragen!");                
            }
        }

        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int selectedIndex = comboBox.SelectedIndex;
            switch (selectedIndex)
            {
                case 0:
                    modelName = "text-davinci-002";
                    maxTokens = 2048;
                    isChatGPTModel = false;
                    break;
                case 1:
                    modelName = "text-davinci-003";
                    maxTokens = 4000;
                    isChatGPTModel = false;
                    break;
                case 2:
                    modelName = "code-davinci-002";
                    maxTokens = 2048;
                    isChatGPTModel = false;
                    break;
                case 3:
                    modelName = "gpt-3.5-turbo";
                    maxTokens = 4096;
                    isChatGPTModel = true;
                    break;
                default:
                    modelName = "text-davinci-003";
                    maxTokens = 4000;
                    isChatGPTModel = false;
                    break;
            }
        }

        private void sendMessageButton_Click(object sender, RoutedEventArgs e)
        {
           if(!isChatGPTModel)
            {
                CallGPTRequest();
            }
           else
            {
                CallChatGPTModel();
            }
        }

        private async void CallGPTRequest()
        {

            RequestGPT completionReqGTP = new RequestGPT
            {
                Model = modelName,
                Temperature = float.Parse(textBox.Text, CultureInfo.InvariantCulture.NumberFormat),
                MaxTokens = maxTokens,
                TopP = float.Parse(textBox3.Text, CultureInfo.InvariantCulture.NumberFormat),
                FrequencyPenalty = float.Parse(textBox1.Text, CultureInfo.InvariantCulture.NumberFormat),
                PresencePenalty = float.Parse(textBox2.Text, CultureInfo.InvariantCulture.NumberFormat)
            };            

            using (HttpClient httpClient = new HttpClient())
            {
                try
                {
                    string? userResponse = messageText.Text;
                    chatBox.Foreground = new SolidColorBrush(Colors.Red);
                    chatBox.Text = user + userResponse;
                    completionReqGTP.Prompt = userResponse;
                    ResponseGPT? responseGPT = null;

                    using (HttpRequestMessage httpReq = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/completions"))
                    {
                        httpReq.Headers.Add("Authorization", $"Bearer {OpenAI_ApiKey}");
                        string requestString = JsonSerializer.Serialize(completionReqGTP);
                        httpReq.Content = new StringContent(requestString, Encoding.UTF8, "application/json");

                        using (HttpResponseMessage? httpResponse = await httpClient.SendAsync(httpReq))
                        {
                            if (httpResponse is not null)
                            {
                                string responseString = await httpResponse.Content.ReadAsStringAsync();
                                if (httpResponse.IsSuccessStatusCode && !string.IsNullOrWhiteSpace(responseString))
                                {
                                    responseGPT = JsonSerializer.Deserialize<ResponseGPT>(responseString);
                                }
                            }
                        }
                    }

                    if (responseGPT != null)
                    {
                        string? responseText = responseGPT.Choices?[0]?.Text;
                        chatBox.Foreground = new SolidColorBrush(Colors.DarkBlue);
                        chatBox.Text = textGPT+responseText;
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private async void CallChatGPTModel()
        {
            var request = new RequestChatGPT
            {
                Model = modelName,
                Stream = true,
                MaxTokens = maxTokens,
                Messages = new List<ChatCompletionMessage>
                {                    
                    new("user", messageText.Text)                    
                }
            };

            HttpClient httpClient = new HttpClient();
            CreateChatResponse? responseChatGPT = null;

            using (HttpRequestMessage httpReq = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions"))
            {
                httpReq.Headers.Add("Authorization", $"Bearer {OpenAI_ApiKey}");
                string requestString = JsonSerializer.Serialize(request);
                httpReq.Content = new StringContent(requestString, Encoding.UTF8, "application/json");

                using (HttpResponseMessage? httpResponse = await httpClient.SendAsync(httpReq))
                {
                    if (httpResponse is not null)
                    {
                        string responseString = await httpResponse.Content.ReadAsStringAsync();
                        if (httpResponse.IsSuccessStatusCode && !string.IsNullOrWhiteSpace(responseString))
                        {
                            responseChatGPT = JsonSerializer.Deserialize<CreateChatResponse>(responseString);
                        }
                    }
                }
            }

            if (responseChatGPT != null)
            {
                string? responseText = responseChatGPT.Choices?[0]?.Message?.ToString();
                chatBox.Foreground = new SolidColorBrush(Colors.DarkBlue);
                chatBox.Text = textGPT + responseText;
            }            
        }
    }
}
