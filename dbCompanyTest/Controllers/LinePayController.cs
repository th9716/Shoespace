using Microsoft.AspNetCore.Mvc;
using System.Drawing.Drawing2D;
using dbCompanyTest.LineAPI;
using static dbCompanyTest.LineAPI.API;
using static dbCompanyTest.LineAPI.PaymentConfirmResponseDto;
using LinedbCompanyTestDemo.LineAPI;

namespace dbCompanyTest.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class LinePayController : Controller
    {

        private readonly LineAPI.LinePayService _linePayService;
        public LinePayController()
        {
            _linePayService = new LinePayService();
        }

        [HttpPost("Create")]
        public async Task<PaymentResponseDto> CreatePayment(PaymentRequestDto dto)
        {
            return await _linePayService.SendPaymentRequest(dto);
        }
        [HttpPost("Confirm")]
        public async Task<PaymentConfirmResponseDto> ConfirmPayment([FromQuery] string transactionId, [FromQuery] string orderId, PaymentConfirmDto dto)
        {
            return await _linePayService.ConfirmPayment(transactionId, orderId, dto);
        }
    }
}
