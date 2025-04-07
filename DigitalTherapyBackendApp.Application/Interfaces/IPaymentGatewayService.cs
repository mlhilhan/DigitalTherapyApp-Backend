using DigitalTherapyBackendApp.Application.Dtos.Subscription;
using System;
using System.Threading.Tasks;

namespace DigitalTherapyBackendApp.Application.Interfaces
{
    public interface IPaymentGatewayService
    {
        /// <summary>
        /// Ödeme işlemini başlatır
        /// </summary>
        /// <param name="userId">Kullanıcı Id</param>
        /// <param name="amount">Ödeme tutarı</param>
        /// <param name="currency">Para birimi (TRY, USD, EUR, vb.)</param>
        /// <param name="paymentMethod">Ödeme yöntemi (CreditCard, PayPal, BankTransfer, vb.)</param>
        /// <param name="description">Ödeme açıklaması</param>
        /// <returns>Ödeme sonucu bilgisi</returns>
        Task<PaymentResultDto> ProcessPaymentAsync(Guid userId, decimal amount, string currency, string paymentMethod, string description);

        /// <summary>
        /// İade işlemini başlatır
        /// </summary>
        /// <param name="transactionId">İade edilecek işlem id'si</param>
        /// <param name="amount">İade tutarı</param>
        /// <returns>İşlem başarılı mı?</returns>
        Task<bool> RefundPaymentAsync(string transactionId, decimal amount);

        /// <summary>
        /// Ödeme sağlayıcıdan gelen webhook verilerini işler
        /// </summary>
        /// <param name="webhookData">Webhook veri içeriği</param>
        /// <returns>İşlenen ödeme sonucu</returns>
        PaymentResultDto ParseWebhookData(string webhookData);

        /// <summary>
        /// Ödeme formunu başlatmak için gerekli bilgileri oluşturur
        /// </summary>
        /// <param name="userId">Kullanıcı Id</param>
        /// <param name="subscriptionId">Abonelik Id</param>
        /// <param name="amount">Ödeme tutarı</param>
        /// <param name="currency">Para birimi</param>
        /// <param name="returnUrl">Ödeme sonrası dönüş URL'i</param>
        /// <returns>Ödeme form bilgileri</returns>
        Task<PaymentFormDto> CreatePaymentFormAsync(Guid userId, int subscriptionId, decimal amount, string currency, string returnUrl);
    }
}