namespace dbCompanyTest.LineAPI
{
    public class PaymentRequestResponseDto
    {
        public string returnCode { get; set; }
        public string returnMessage { get; set; }
        public ConfirmResponseInfoDto info { get; set; }
    }

    public class ConfirmResponseInfoDto
    {
        public string orderId { get; set; }
        public long transactionId { get; set; }
        public string authorizationExpireDate { get; set; }
        public string regKey { get; set; }
        public ConfirmResponsePayInfoDto[] payInfo { get; set; }
    }

    public class ConfirmResponsePayInfoDto
    {
        public string method { get; set; }
        public int amount { get; set; }
        public string creditCardNickname { get; set; }
        public string creditCardBrand { get; set; }
        public string maskedCreditCardNumber { get; set; }
        public ConfirmResponsePackageDto[] packages { get; set; }
        public ConfirmResponseShippingOptionsDto shipping { get; set; }
    }
    public class ConfirmResponsePackageDto
    {
        public string id { get; set; }
        public int amount { get; set; }
        public int userFeeAmount { get; set; }
    }
    public class ConfirmResponseShippingOptionsDto
    {
        public string methodId { get; set; }
        public int feeAmount { get; set; }
        //public ShippingAddressDto Address { get; set; }
    }

}

