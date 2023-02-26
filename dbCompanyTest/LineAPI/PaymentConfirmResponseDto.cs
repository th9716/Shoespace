namespace dbCompanyTest.LineAPI
{
    public class PaymentConfirmResponseDto
    {
        public class PaymentResponseDto
        {
            public string returnCode { get; set; }
            public string returnMessage { get; set; }
            public ResponseInfoDto info { get; set; }
        }

        public class ResponseInfoDto
        {
            public ResponsePaymentUrlDto paymentUrl { get; set; }
            public long transactionId { get; set; }
            public string paymentAccessToken { get; set; }
        }

        public class ResponsePaymentUrlDto
        {
            public string web { get; set; }
            public string app { get; set; }
        }
    }
}
