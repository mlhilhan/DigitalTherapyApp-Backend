using DigitalTherapyBackendApp.Application.Interfaces;
using DigitalTherapyBackendApp.Domain.Entities;
using DigitalTherapyBackendApp.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenAI_API;
using OpenAI_API.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalTherapyBackendApp.Infrastructure.Services
{
    public class GptService : IAiService
    {
        private readonly OpenAIAPI _openAIApi;
        private readonly IConfiguration _configuration;
        private readonly ILogger<GptService> _logger;
        private readonly ITherapySessionRepository _sessionRepository;
        private readonly ISessionMessageRepository _messageRepository;
        private readonly string _model;
        private readonly int _messageHistoryCount;
        private readonly int _maxRetries;
        private readonly int _timeout;

        private const string DEFAULT_SYSTEM_PROMPT = @"Your name is Derman. You are a digital therapy assistant designed to provide emotional and psychological support to users. 
            You use an empathetic, understanding, and supportive tone. You try to understand users' mental health conditions and provide helpful responses. 
            When necessary, you encourage users to seek professional help. Also, if a user expresses an emergency situation or suicidal thoughts, you immediately 
            advise them to get professional help. Always respond in the same language that the user is using to communicate with you. 
            When introducing yourself, refer to yourself as Derman, the therapy assistant.";

        public GptService(
            IConfiguration configuration,
            ILogger<GptService> logger,
            ITherapySessionRepository sessionRepository,
            ISessionMessageRepository messageRepository)
        {
            _configuration = configuration;
            _logger = logger;
            _sessionRepository = sessionRepository;
            _messageRepository = messageRepository;

            var apiKey = _configuration["OpenAI:ApiKey"]
                ?? throw new ArgumentNullException("OpenAI:ApiKey", "API key not configured");

            _model = _configuration["OpenAI:ModelName"] ?? "gpt-3.5-turbo";

            if (!int.TryParse(_configuration["OpenAI:MessageHistoryCount"], out _messageHistoryCount))
            {
                _messageHistoryCount = 10;
            }

            if (!int.TryParse(_configuration["OpenAI:MaxRetries"], out _maxRetries))
            {
                _maxRetries = 3;
            }

            if (!int.TryParse(_configuration["OpenAI:TimeoutSeconds"], out _timeout))
            {
                _timeout = 30;
            }

            _openAIApi = new OpenAIAPI(new APIAuthentication(apiKey));
        }


        public async Task<string> GetChatResponseAsync(Guid patientId, string message, Guid? sessionId = null)
        {
            var retryCount = 0;

            while (retryCount <= _maxRetries)
            {
                try
                {
                    TherapySession session;

                    // Eğer session ID belirtilmişse, doğrudan o session'ı kullan
                    if (sessionId.HasValue)
                    {
                        session = await _sessionRepository.GetByIdAsync(sessionId.Value);

                        // Session bulunamadıysa veya bu kullanıcıya ait değilse hata döndür
                        if (session == null || session.PatientId != patientId)
                        {
                            throw new Exception("Session not found or does not belong to this user");
                        }

                        // Session aktif değilse aktif yap
                        if (!session.IsActive)
                        {
                            // Diğer aktif sessionları devre dışı bırak
                            var activeSessions = await _sessionRepository.GetActiveSessionsAsync(patientId);
                            foreach (var activeSession in activeSessions)
                            {
                                if (activeSession.Id != session.Id)
                                {
                                    activeSession.IsActive = false;
                                    activeSession.EndTime = DateTime.UtcNow;
                                    await _sessionRepository.UpdateAsync(activeSession);
                                }
                            }

                            session.IsActive = true;
                            if (session.EndTime.HasValue)
                            {
                                session.EndTime = null;
                            }
                            await _sessionRepository.UpdateAsync(session);
                            _logger.LogInformation($"Session {session.Id} activated in AI service");
                        }
                    }
                    else
                    {
                        // Session ID belirtilmemişse, aktif session'ı al veya yenisini oluştur
                        session = await _sessionRepository.GetActiveAiSessionAsync(patientId);
                        if (session == null)
                        {
                            _logger.LogWarning($"No active AI sessions found for patient {patientId}. A new session is being created.");
                            session = await _sessionRepository.CreateAiSessionAsync(patientId);
                        }
                    }

                    await _messageRepository.AddUserMessageAsync(session.Id, patientId, message);

                    var chatHistory = (await _messageRepository.GetRecentBySessionIdAsync(session.Id, _messageHistoryCount)).ToList();

                    var conversation = new List<ChatMessage>
            {
                new ChatMessage(ChatMessageRole.System, DEFAULT_SYSTEM_PROMPT)
            };

                    foreach (var chatMessage in chatHistory)
                    {
                        var role = chatMessage.IsAiGenerated ? ChatMessageRole.Assistant : ChatMessageRole.User;
                        conversation.Add(new ChatMessage(role, chatMessage.Content));
                    }

                    var chatRequest = new ChatRequest()
                    {
                        Model = _model,
                        Temperature = 0.7,
                        MaxTokens = 800,
                        Messages = conversation,
                        NumChoicesPerMessage = 1
                    };

                    _logger.LogInformation($"Sending GPT API request. Model: {_model}, PatientID: {patientId}, SessionID: {session.Id}");

                    var timeoutTask = Task.Delay(TimeSpan.FromSeconds(_timeout));
                    var apiTask = _openAIApi.Chat.CreateChatCompletionAsync(chatRequest);
                    var completedTask = await Task.WhenAny(apiTask, timeoutTask);

                    if (completedTask == timeoutTask)
                    {
                        throw new TimeoutException($"The GPT API request timed out. ({_timeout} seconds)");
                    }

                    var result = await apiTask;

                    if (result == null || result.Choices == null || result.Choices.Count == 0)
                    {
                        throw new Exception("API response is empty or invalid format.");
                    }

                    string aiResponse = result.Choices[0].Message.Content;
                    _logger.LogInformation($"GPT API response received. Session: {session.Id}, Response length: {aiResponse?.Length ?? 0}");

                    await _messageRepository.AddAiMessageAsync(session.Id, patientId, aiResponse);

                    return aiResponse;
                }
                catch (TimeoutException ex)
                {
                    _logger.LogWarning(ex, $"GPT API timed out: (Test {retryCount + 1}/{_maxRetries + 1})");
                    retryCount++;

                    if (retryCount > _maxRetries)
                    {
                        return "Sorry it took so long to reply. Please try again later.";
                    }

                    await Task.Delay(1000 * retryCount);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred during the GPT API call: (Test {retryCount + 1}/{_maxRetries + 1}): {ex.Message}");

                    if (ex.Message.Contains("insufficient_quota") || ex.Message.Contains("TooManyRequests"))
                    {
                        return "Sorry, the AI ​​service is currently unavailable. Please try again later or contact your system administrator.";
                    }

                    if (ex.Message.Contains("model_not_found") || ex.Message.Contains("does not exist"))
                    {
                        _logger.LogCritical("OpenAI model error. Configuration needs to be checked: {Error}", ex.Message);
                        return "Sorry, an AI service configuration error occurred. Please contact your system administrator.";
                    }

                    retryCount++;

                    if (retryCount > _maxRetries)
                    {
                        return "Sorry, there was a problem processing your message. Please try again later.";
                    }

                    await Task.Delay(1000 * retryCount);
                }
            }

            return "Sorry, we are experiencing technical difficulties. Please try again later.";
        }


        public async Task<bool> EndChatSessionAsync(Guid sessionId)
        {
            try
            {
                return await _sessionRepository.CloseAiSessionAsync(sessionId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while terminating the session: {sessionId}");
                return false;
            }
        }
    }
}