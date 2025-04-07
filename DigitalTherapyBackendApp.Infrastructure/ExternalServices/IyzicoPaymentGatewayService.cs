using DigitalTherapyBackendApp.Application.Dtos.Subscription;
using DigitalTherapyBackendApp.Application.Interfaces;
using DigitalTherapyBackendApp.Domain.Entities;
using DigitalTherapyBackendApp.Domain.Interfaces;
using DigitalTherapyBackendApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DigitalTherapyBackendApp.Infrastructure.ExternalServices
{
    public class IyzicoPaymentGatewayService : IPaymentGatewayService
    {
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<IyzicoPaymentGatewayService> _logger;

        private readonly string _apiKey;
        private readonly string _secretKey;
        private readonly string _baseUrl;
        private readonly bool _testMode;

        public IyzicoPaymentGatewayService(
            IConfiguration configuration,
            AppDbContext context,
            IHttpClientFactory httpClientFactory,
            ILogger<IyzicoPaymentGatewayService> logger)
        {
            _configuration = configuration;
            _context = context;
            _httpClientFactory = httpClientFactory;
            _logger = logger;

            // İyzico konfigürasyonlarını al
            _apiKey = _configuration["Payment:Iyzico:ApiKey"];
            _secretKey = _configuration["Payment:Iyzico:SecretKey"];
            _baseUrl = _configuration["Payment:Iyzico:BaseUrl"];
            _testMode = bool.Parse(_configuration["Payment:Iyzico:TestMode"] ?? "false");
        }


        public async Task<PaymentResultDto> ProcessPaymentAsync(Guid userId, decimal amount, string currency, string paymentMethod, string description)
        {
            try
            {
                // Kullanıcı bilgilerini getir
                var user = await _context.Users
                    .Include(u => u.PatientProfile)
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null)
                {
                    return new PaymentResultDto
                    {
                        Success = false,
                        ErrorMessage = "Kullanıcı bulunamadı"
                    };
                }

                // Bu metot doğrudan ödeme yapmaz, sadece kullanıcının form doldurması için gereken bilgileri döner
                // Gerçek ödeme işlemi CreatePaymentFormAsync ve webhook üzerinden gerçekleşir

                return new PaymentResultDto
                {
                    Success = true,
                    TransactionId = Guid.NewGuid().ToString("N"), // Geçici bir transaction ID
                    Status = "pending",
                    ErrorMessage = null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ödeme işlemi başlatılırken hata oluştu");
                return new PaymentResultDto
                {
                    Success = false,
                    ErrorMessage = "Ödeme işlemi başlatılırken bir hata oluştu: " + ex.Message
                };
            }
        }

        public async Task<bool> RefundPaymentAsync(string transactionId, decimal amount)
        {
            try
            {
                // İyzico API endpointi
                string refundUrl = $"{_baseUrl}/payment/refund";

                // İade isteği için gerekli parametreler
                var requestData = new
                {
                    locale = "tr",
                    conversationId = Guid.NewGuid().ToString(),
                    paymentTransactionId = transactionId,
                    price = amount.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture),
                    ip = "127.0.0.1" // Sunucu IP adresi
                };

                // API isteği için gerekli imzayı oluştur
                string requestJson = JsonConvert.SerializeObject(requestData);
                var headers = CreateIyzicoHeaders(requestJson);

                // API isteğini gönder
                using var request = new HttpRequestMessage(HttpMethod.Post, refundUrl);
                request.Content = new StringContent(requestJson, Encoding.UTF8, "application/json");

                foreach (var header in headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }

                var client = _httpClientFactory.CreateClient();
                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<dynamic>(responseContent);

                    return result.status == "success";
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "İade işlemi sırasında hata oluştu");
                return false;
            }
        }

        public PaymentResultDto ParseWebhookData(string webhookData)
        {
            try
            {
                // Webhook verilerini deserialize et
                var webhookObj = JsonConvert.DeserializeObject<dynamic>(webhookData);

                string status = webhookObj.status;
                string paymentId = webhookObj.paymentId;

                // Başarılı ödeme kontrolü
                bool isSuccess = status == "success";

                return new PaymentResultDto
                {
                    Success = isSuccess,
                    TransactionId = paymentId,
                    Status = isSuccess ? "completed" : "failed",
                    ErrorMessage = isSuccess ? null : webhookObj.errorMessage
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Webhook verisi işlenirken hata oluştu");
                return new PaymentResultDto
                {
                    Success = false,
                    ErrorMessage = "Webhook verisi işlenirken bir hata oluştu: " + ex.Message
                };
            }
        }

        public async Task<PaymentFormDto> CreatePaymentFormAsync(Guid userId, int subscriptionId, decimal amount, string currency, string returnUrl)
        {
            try
            {
                // Kullanıcı ve abonelik bilgilerini getir
                var user = await _context.Users
                    .Include(u => u.PatientProfile)
                    .FirstOrDefaultAsync(u => u.Id == userId);

                var subscription = await _context.Subscriptions
                    .FindAsync(subscriptionId);

                if (user == null || subscription == null)
                {
                    return new PaymentFormDto
                    {
                        Success = false,
                        ErrorMessage = "Kullanıcı veya abonelik planı bulunamadı"
                    };
                }

                // İyzico için ödeme formu oluştur
                string conversationId = Guid.NewGuid().ToString();
                string paymentId = Guid.NewGuid().ToString("N");

                // İyzico API endpointi
                string checkoutFormUrl = $"{_baseUrl}/payment/initialize3ds";

                // Ödeme formu için gerekli parametreler
                var buyerInfo = new
                {
                    id = user.Id.ToString(),
                    name = user.PatientProfile.FirstName,
                    surname = user.PatientProfile.LastName,
                    email = user.Email,
                    identityNumber = "11111111111", // TC kimlik no veya benzeri tanımlayıcı (test için sabit değer)
                    //registrationAddress = user.PatientProfile.Address ?? "Test Adres",
                    //city = user.PatientProfile.City ?? "İstanbul",
                    //country = user.PatientProfile.Country ?? "Türkiye",
                    ip = "127.0.0.1" // Client IP adresi
                };

                var basketItems = new[]
                {
                    new
                    {
                        id = subscription.Id.ToString(),
                        name = subscription.Name,
                        category1 = "Abonelik",
                        itemType = "VIRTUAL",
                        price = amount.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture)
                    }
                };

                var requestData = new
                {
                    locale = "tr",
                    conversationId,
                    price = amount.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture),
                    paidPrice = amount.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture),
                    currency,
                    installment = 1,
                    basketId = subscriptionId.ToString(),
                    paymentChannel = "WEB",
                    paymentGroup = "SUBSCRIPTION",
                    callbackUrl = returnUrl,
                    buyer = buyerInfo,
                    shippingAddress = new
                    {
                        contactName = $"{user.PatientProfile.FirstName} {user.PatientProfile.LastName}",
                        //city = user.PatientProfile.City ?? "İstanbul",
                        //country = user.PatientProfile.Country ?? "Türkiye",
                        //address = user.PatientProfile.Address ?? "Test Adres"
                    },
                    billingAddress = new
                    {
                        contactName = $"{user.PatientProfile.FirstName} {user.PatientProfile.LastName}",
                        //city = user.PatientProfile.City ?? "İstanbul",
                        //country = user.PatientProfile.Country ?? "Türkiye",
                        //address = user.PatientProfile.Address ?? "Test Adres"
                    },
                    basketItems
                };

                // API isteği için gerekli imzayı oluştur
                string requestJson = JsonConvert.SerializeObject(requestData);
                var headers = CreateIyzicoHeaders(requestJson);

                // API isteğini gönder
                using var request = new HttpRequestMessage(HttpMethod.Post, checkoutFormUrl);
                request.Content = new StringContent(requestJson, Encoding.UTF8, "application/json");

                foreach (var header in headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }

                var client = _httpClientFactory.CreateClient();
                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<dynamic>(responseContent);

                    if (result.status == "success")
                    {
                        return new PaymentFormDto
                        {
                            Success = true,
                            FormContent = result.threeDSHtmlContent,
                            TransactionId = paymentId
                        };
                    }
                    else
                    {
                        return new PaymentFormDto
                        {
                            Success = false,
                            ErrorMessage = result.errorMessage
                        };
                    }
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return new PaymentFormDto
                    {
                        Success = false,
                        ErrorMessage = $"API isteği başarısız: {response.StatusCode} - {errorContent}"
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ödeme formu oluşturulurken hata oluştu");
                return new PaymentFormDto
                {
                    Success = false,
                    ErrorMessage = "Ödeme formu oluşturulurken bir hata oluştu: " + ex.Message
                };
            }
        }

        #region Helper Methods

        private Dictionary<string, string> CreateIyzicoHeaders(string requestBody)
        {
            string randomString = GenerateRandomString();
            string timestamp = GetTimestamp();

            string signature = GenerateSignature(_apiKey, randomString, _secretKey, timestamp, requestBody);

            return new Dictionary<string, string>
            {
                { "Authorization", $"IYZWS {_apiKey}:{signature}" },
                { "x-iyzi-rnd", randomString },
                { "x-iyzi-timestamp", timestamp }
            };
        }

        private string GenerateRandomString()
        {
            return Guid.NewGuid().ToString("N");
        }

        private string GetTimestamp()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
        }

        private string GenerateSignature(string apiKey, string randomString, string secretKey, string timestamp, string requestBody)
        {
            string stringToHash = apiKey + randomString + secretKey + timestamp + requestBody;
            using var sha1 = SHA1.Create();
            byte[] bytes = Encoding.UTF8.GetBytes(stringToHash);
            byte[] hashBytes = sha1.ComputeHash(bytes);

            return Convert.ToBase64String(hashBytes);
        }

        #endregion
    }
}