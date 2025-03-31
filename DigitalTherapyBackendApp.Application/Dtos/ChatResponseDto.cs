using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalTherapyBackendApp.Application.Dtos
{
    public class ChatResponseDto
    {
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }
        public Guid? SessionId { get; set; }
    }
}
