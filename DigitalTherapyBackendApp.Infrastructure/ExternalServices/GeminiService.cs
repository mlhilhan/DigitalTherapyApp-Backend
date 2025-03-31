using DigitalTherapyBackendApp.Application.Interfaces;
using DigitalTherapyBackendApp.Domain.Entities;
using DigitalTherapyBackendApp.Domain.Interfaces;
using GenerativeAI.Exceptions;
using GenerativeAI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GenerativeAI.Types;

namespace DigitalTherapyBackendApp.Infrastructure.ExternalServices
{
    public class GeminiService : IAiService
    {
        private readonly GenerativeModel _model;
        private readonly IConfiguration _configuration;
        private readonly ITherapySessionRepository _therapySessionRepository;
        private readonly ISessionMessageRepository _sessionMessageRepository;
        private readonly ILogger<GeminiService> _logger;
        private readonly Guid _systemUserId;

        public GeminiService(
            IConfiguration configuration,
            ITherapySessionRepository therapySessionRepository,
            ISessionMessageRepository sessionMessageRepository,
            ILogger<GeminiService> logger)
        {
            _configuration = configuration;
            _therapySessionRepository = therapySessionRepository;
            _sessionMessageRepository = sessionMessageRepository;
            _logger = logger;

            string apiKey = _configuration["Google:GeminiApiKey"] ??
                throw new InvalidOperationException("Gemini API Key is not configured. Please check your appsettings.");

            string modelName = _configuration["Google:GeminiModel"] ?? "gemini-1.5-pro";
            _logger.LogInformation($"Using Gemini model: {modelName}");

            try
            {
                _model = new GenerativeModel(modelName, apiKey);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initializing GenerativeModel. Please check your API key and model name.");
                throw; // Uygulamanın başlamasını engellemek için hatayı yeniden fırlat
            }

            string systemUserIdStr = _configuration["AI:SystemUserId"];
            if (!Guid.TryParse(systemUserIdStr, out _systemUserId))
            {
                _systemUserId = Guid.Parse("00000000-0000-0000-0000-000000000001");
                _logger.LogWarning($"AI:SystemUserId not set or invalid ('{systemUserIdStr}'). Using default value: {_systemUserId}");
            }
            else
            {
                _logger.LogInformation($"AI System User ID: {_systemUserId}");
            }
        }

        public async Task<string> GetChatResponseAsync(Guid patientId, string message)
        {
            try
            {
                _logger.LogInformation($"Processing chat message for patient {patientId}");

                var session = await GetOrCreateAiSessionAsync(patientId);
                _logger.LogInformation($"Using session {session.Id}");

                // Kullanıcı mesajını kaydet
                await _sessionMessageRepository.AddUserMessageAsync(session.Id, patientId, message);

                // Sohbet geçmişini al (son 10 mesaj)
                var messageHistory = await _sessionMessageRepository.GetRecentBySessionIdAsync(session.Id, 10);

                // Önce sistem talimatı
                var contents = new List<Content>();
                contents.Add(new Content(
                    role: "system",
                    parts: new List<Part> { new Part(GetSystemInstruction()) }
                ));

                // Geçmiş mesajları ekle
                foreach (var historyMessage in messageHistory.OrderBy(m => m.SentAt))
                {
                    string role = historyMessage.IsAiGenerated ? "model" : "user";
                    contents.Add(new Content(
                        role: role,
                        parts: new List<Part> { new Part(historyMessage.Content) }
                    ));
                }

                // Gemini API'ye istek gönder
                var request = new GenerateContentRequest(contents);
                var response = await _model.GenerateContentAsync(request);

                if (response.Candidates != null && response.Candidates.Count() > 0)
                {
                    // Yanıttaki ilk içerik parçasını al
                    var aiResponse = response.Candidates[0].Content.Parts[0].Text;

                    if (!string.IsNullOrEmpty(aiResponse))
                    {
                        _logger.LogInformation($"Gemini response received (session {session.Id}): {aiResponse.Substring(0, Math.Min(aiResponse.Length, 100))}...");
                        await _sessionMessageRepository.AddAiMessageAsync(session.Id, _systemUserId, aiResponse);
                        return aiResponse;
                    }
                    else
                    {
                        _logger.LogWarning($"Empty text content in Gemini response (session {session.Id}).");
                        var defaultResponse = "Üzgünüm, şu anda anlamlı bir yanıt alamadım.";
                        await _sessionMessageRepository.AddAiMessageAsync(session.Id, _systemUserId, defaultResponse);
                        return defaultResponse;
                    }
                }
                else
                {
                    _logger.LogWarning($"No candidates in Gemini response (session {session.Id}).");
                    var defaultResponse = "Üzgünüm, şu anda bir yanıt oluşturulamadı.";
                    await _sessionMessageRepository.AddAiMessageAsync(session.Id, _systemUserId, defaultResponse);
                    return defaultResponse;
                }
            }
            catch (GenerativeAIException ex)
            {
                _logger.LogError(ex, $"Generative AI API error for patient {patientId}: {ex.Message}");
                var errorResponse = "Üzgünüm, Google AI servisi ile iletişim kurarken bir hata oluştu. Lütfen daha sonra tekrar deneyin.";

                // Hata yanıtını da veritabanına kaydet
                try
                {
                    var session = await GetOrCreateAiSessionAsync(patientId);
                    await _sessionMessageRepository.AddAiMessageAsync(session.Id, _systemUserId, errorResponse);
                }
                catch (Exception)
                {
                    // İç içe hata oluşmaması için sessizce geç
                }

                return errorResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An unexpected error occurred while processing chat for patient {patientId}.");
                var errorResponse = "Üzgünüm, beklenmedik bir hata oluştu. Lütfen daha sonra tekrar deneyin.";

                // Hata yanıtını da veritabanına kaydet
                try
                {
                    var session = await GetOrCreateAiSessionAsync(patientId);
                    await _sessionMessageRepository.AddAiMessageAsync(session.Id, _systemUserId, errorResponse);
                }
                catch (Exception)
                {
                    // İç içe hata oluşmaması için sessizce geç
                }

                return errorResponse;
            }
        }

        private async Task<TherapySession> GetOrCreateAiSessionAsync(Guid patientId)
        {
            var activeSession = await _therapySessionRepository.GetActiveAiSessionAsync(patientId);

            if (activeSession != null)
            {
                _logger.LogDebug($"Found active AI session {activeSession.Id} for patient {patientId}");
                return activeSession;
            }

            _logger.LogInformation($"Creating new AI session for patient {patientId}");
            return await _therapySessionRepository.CreateAiSessionAsync(patientId);
        }

        private string GetSystemInstruction()
        {
            return "Sen bir dijital terapi asistanısın. " +
                   "Kullanıcılara empatik, destekleyici ve profesyonel bir şekilde yanıt ver. " +
                   "Kullanıcıların kendilerini ifade etmelerine yardımcı ol ve basit fakat etkili psikolojik teknikler öner. " +
                   "Dinleme, anlama ve rehberlik etmeye odaklan. Tıbbi teşhis koyma veya ilaç önerme. " +
                   "İntihar düşünceleri veya kendine zarar verme durumlarında, acil yardım almanın önemini vurgula ve " +
                   "ulusal yardım hatlarını öner (örn. 112, 182 Sosyal Destek Hattı). " +
                   "Her zaman profesyonel yardım almanın önemini vurgula. " +
                   "Türkçe olarak yanıt ver ve kullanıcıların anlaması için basit dil kullan. " +
                   "Uzmanlığının sınırlarının farkında ol ve gerektiğinde 'bilmiyorum' demeyi tercih et. " +
                   "Görüşmeyi klinik not haline getirmek için zaman zaman özetleme yap.";
        }

        public async Task<bool> EndChatSessionAsync(Guid sessionId)
        {
            _logger.LogInformation($"Ending chat session {sessionId}");
            return await _therapySessionRepository.CloseAiSessionAsync(sessionId);
        }
    }
}