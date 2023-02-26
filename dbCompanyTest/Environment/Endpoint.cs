namespace dbCompanyTest.Environment
{
    public class Endpoint
    {
        public string id { get; set; }
        public string region { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public string public_url { get; set; }
        public string proto { get; set; }
        public string hostport { get; set; }
        public string type { get; set; }
        public Tunnel tunnel { get; set; }
    }
}
