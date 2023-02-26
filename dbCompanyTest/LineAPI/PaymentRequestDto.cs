namespace dbCompanyTest.LineAPI
{
        public class PaymentRequestDto
        {
            public int amount { get; set; }
            public string currency { get; set; }
            public string orderId { get; set; }
            public List<PackageDto> packages { get; set; }
            public RedirectUrlsDto redirectUrls { get; set; }
            //public RequestOptionDto? Options { get; set; }
        }
        public class PackageDto
        {
            public string id { get; set; }
            public int amount { get; set; }
            public string name { get; set; }
            public List<LinePayProductDto> products { get; set; }
            //public int? UserFee { get; set; }

        }
        public class LinePayProductDto
        {
            public string name { get; set; }
            public int quantity { get; set; }
            public int price { get; set; }
            //public string? Id { get; set; }
            //public string? ImageUrl { get; set; }
            //public int? OriginalPrice { get; set; }
        }

    public class RedirectUrlsDto
    {
        public string confirmUrl { get; set; }
        public string cancelUrl { get; set; }
        //public string? AppPackageName { get; set; }
        //public string? ConfirmUrlType { get; set; }
    }

    //public class RequestOptionDto
    //{
    //    public PaymentOptionDto? Payment { get; set; }
    //    public DisplpyOptionDto? Displpy { get; set; }
    //    public ShippingOptionDto? Shipping { get; set; }
    //    public ExtraOptionsDto? Extra { get; set; }
    //}
    //public class PaymentOptionDto
    //{
    //    public bool? Capture { get; set; }
    //    public string? PayType { get; set; }
    //}
    //public class DisplpyOptionDto
    //{
    //    public string? Local { get; set; }
    //    public bool? CheckConfirmUrlBrowser { get; set; }
    //}
    //public class ShippingOptionDto
    //{
    //    public string? Type { get; set; }
    //    public int FeeAmount { get; set; }
    //    public string? FeeInquiryUrl { get; set; }
    //    public string? FeeInquiryType { get; set; }
    //    public ShippingAddressDto? Address { get; set; }
    //}

    //public class ShippingAddressDto
    //{
    //    public string? Country { get; set; }
    //    public string? PostalCode { get; set; }
    //    public string? State { get; set; }
    //    public string? City { get; set; }
    //    public string? Detail { get; set; }
    //    public string? Optional { get; set; }
    //    public ShippingAddressRecipientDto Recipient { get; set; }
    //}

    //public class ShippingAddressRecipientDto
    //{
    //    public string? FirstName { get; set; }
    //    public string? LastName { get; set; }
    //    public string? FirstNameOptional { get; set; }
    //    public string? LastNameOptional { get; set; }
    //    public string? Email { get; set; }
    //    public string? PhoneNo { get; set; }
    //    public string? Type { get; set; }
    //}

    //public class ExtraOptionsDto
    //{
    //    public string? BranchName { get; set; }
    //    public string? BranchId { get; set; }
    //}
}
