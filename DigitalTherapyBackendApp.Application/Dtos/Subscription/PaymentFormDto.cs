using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalTherapyBackendApp.Application.Dtos.Subscription
{
    public class PaymentFormDto
    {
        public bool Success { get; set; }
        public string FormContent { get; set; }
        public string TransactionId { get; set; }
        public string ErrorMessage { get; set; }
    }
}
