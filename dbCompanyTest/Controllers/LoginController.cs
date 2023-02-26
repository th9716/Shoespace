using dbCompanyTest.Hubs;
using dbCompanyTest.Models;
using dbCompanyTest.ViewModels;
using Google.Apis.Auth;
using iText.Html2pdf.Attach;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NPOI.SS.Formula.Functions;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text.Json;
using System.Web;

namespace dbCompanyTest.Controllers
{
    public class LoginController : Controller
    {
        private readonly string _appId = "1657826204";
        private readonly string _appSecret = "dfc3f645938564091911eb6782f905bd";
        //private string RedirectUrl = "https://localhost:7100/Login/Line";
        private string RedirectUrl = Environment.Environment.useEnvironment + "/Login/Line";
        private readonly IHttpClientFactory _clientFactory;
        public LoginController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        [TempData]
        public Guid State { get; set; }
        public IActionResult Login()
        {
            if (!HttpContext.Session.Keys.Contains(CDittionary.SK_USE_FOR_LOGIN_USER_SESSION))
            {
                State = Guid.NewGuid();
                ViewData["LineAuth"] = $"https://access.line.me/oauth2/v2.1/authorize?" +
                    $"client_id={_appId}" +
                    $"&response_type=code" +
                    $"&redirect_uri={RedirectUrl}" +
                    $"&scope={HttpUtility.UrlEncode("profile openid email")}" +
                    $"&state={State}";
                ViewData["localUrl"] = Environment.Environment.useEnvironment;
                return View();
            }
            else
                return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        public IActionResult checkLogin(string account, string password)
        {
            dbCompanyTestContext db = new dbCompanyTestContext();
            if (password == null)
            {
                TestClient client = db.TestClients.FirstOrDefault(x=>x.Email== account);
                if (client.客戶編號.Substring(0, 3) == "CLG")
                    return Content("請使用Google登入");
                else if (client.客戶編號.Substring(0, 3) == "CLL")
                    return Content("請使用Line登入");
                else
                    return Content("沒有密碼");
            }
            var a = db.TestClients.FirstOrDefault(c => c.Email == account && c.密碼 == password);
            if (a != null)
            {
                useSession(a);
                return Content("成功"+ Environment.Environment.useEnvironment);
            }
            else
                return Content("失敗");
        }

        public IActionResult loginSussess()
        {
            string? formCredential = Request.Form["credential"]; //回傳憑證
            string? formToken = Request.Form["g_csrf_token"]; //回傳令牌
            string? cookiesToken = Request.Cookies["g_csrf_token"]; //Cookie 令牌

            // 驗證 Google Token
            GoogleJsonWebSignature.Payload? payload = VerifyGoogleToken(formCredential, formToken, cookiesToken).Result;
            if (payload == null)
            {
                // 驗證失敗
                //ViewData["Msg"] = "驗證 Google 授權失敗";
                return RedirectToAction("Login");
            }
            else
            {
                dbCompanyTestContext db = new dbCompanyTestContext();
                TestClient? loggingUser = db.TestClients.FirstOrDefault(c => c.Email == payload.Email);
                if (loggingUser == null)
                {
                    TestClient newClient = new TestClient();
                    newClient.客戶編號 = $"CLG-{payload.JwtId.Substring(0, 7)}";
                    newClient.Email = payload.Email;
                    newClient.客戶姓名 = payload.Name;
                    db.TestClients.Add(newClient);
                    db.SaveChanges();
                    useSession(newClient);
                }
                else
                    useSession(loggingUser);
                //驗證成功，取使用者資訊內容
                //ViewData["Msg"] = "驗證 Google 授權成功" + "<br>";
                //ViewData["Msg"] += "Email:" + payload.Email + "<br>";
                //ViewData["Msg"] += "Name:" + payload.Name + "<br>";
                //ViewData["Msg"] += "Picture:" + payload.Picture;
                return RedirectToAction("index", "Home");
            }
        }
        public async Task<GoogleJsonWebSignature.Payload?> VerifyGoogleToken(string? formCredential, string? formToken, string? cookiesToken)
        {
            // 檢查空值
            if (formCredential == null || formToken == null && cookiesToken == null)
            {
                return null;
            }

            GoogleJsonWebSignature.Payload? payload;
            try
            {
                // 驗證 token
                if (formToken != cookiesToken)
                {
                    return null;
                }

                // 驗證憑證
                IConfiguration Config = new ConfigurationBuilder().AddJsonFile("appSettings.json").Build();
                string GoogleClientId = Config.GetSection("GoogleClientId").Value;
                var settings = new GoogleJsonWebSignature.ValidationSettings()
                {
                    Audience = new List<string>() { GoogleClientId }
                };
                payload = await GoogleJsonWebSignature.ValidateAsync(formCredential, settings);
                if (!payload.Issuer.Equals("accounts.google.com") && !payload.Issuer.Equals("https://accounts.google.com"))
                {
                    return null;
                }
                if (payload.ExpirationTimeSeconds == null)
                {
                    return null;
                }
                else
                {
                    DateTime now = DateTime.Now.ToUniversalTime();
                    DateTime expiration = DateTimeOffset.FromUnixTimeSeconds((long)payload.ExpirationTimeSeconds).DateTime;
                    if (now > expiration)
                    {
                        return null;
                    }
                }
            }
            catch
            {
                return null;
            }
            return payload;
        }

        public void useSession(TestClient x)
        {
            if (!HttpContext.Session.Keys.Contains(CDittionary.SK_USE_FOR_LOGIN_USER_SESSION))
            {
                string json = System.Text.Json.JsonSerializer.Serialize(x);
                HttpContext.Session.SetString(CDittionary.SK_USE_FOR_LOGIN_USER_SESSION, json);
            }
        }

        public IActionResult Logout()
        {
            //Gary
            MyLoveAndSoppingCar(CDittionary.SK_USE_FOR_MYLOVE_SESSION, CDittionary.SK_USE_FOR_LOGIN_USER_SESSION, false);
            //--------------------------------------------------------------
            //---購物車Logout--LU--感謝Gary<3
            MyLoveAndSoppingCar(CDittionary.SK_USE_FOR_SHOPPING_CAR_SESSION, CDittionary.SK_USE_FOR_LOGIN_USER_SESSION, true);
            //---購物車Logout結束--LU
            HttpContext.Session.Remove(CDittionary.SK_USE_FOR_LOGIN_USER_SESSION);
            return RedirectToAction("Index", "Home");
        }

        private void MyLoveAndSoppingCar(string session, string usersession, bool MyloveOrShoppingcar)
        {
            if (HttpContext.Session.Keys.Contains(session))
            {
                dbCompanyTestContext _context = new dbCompanyTestContext();
                string userjson = HttpContext.Session.GetString(usersession);
                var userdata = System.Text.Json.JsonSerializer.Deserialize<TestClient>(userjson);
                var del = _context.會員商品暫存s.Select(x => x).Where(y => y.客戶編號 == userdata.客戶編號 && y.購物車或我的最愛 == MyloveOrShoppingcar);
                _context.會員商品暫存s.RemoveRange(del);
                _context.SaveChanges();
                //----購物車記數規零
                var json = "";
                OnlyForCountCarProductNum? carCount = new OnlyForCountCarProductNum();
                carCount.ProductCountNum = 0;
                json = System.Text.Json.JsonSerializer.Serialize(carCount);
                HttpContext.Session.SetString(CDittionary.SK_ONLY_FOR_CAR_PRODUCT_COUNT_SESSION, json);
                //讀取我的最愛Session
                string lovejson = HttpContext.Session.GetString(session);
                var lovedata = System.Text.Json.JsonSerializer.Deserialize<List<會員商品暫存>>(lovejson).ToArray();
                foreach (var item in lovedata)
                {
                    item.Id = 0;
                }
                _context.會員商品暫存s.AddRange(lovedata);
                _context.SaveChanges();
                HttpContext.Session.Remove(session);
            }
        }

        public IActionResult getUser()
        {
            string? json = HttpContext.Session.GetString(CDittionary.SK_USE_FOR_LOGIN_USER_SESSION);
            string userName = "";
            if (json == null)
                userName = "訪客";
            else
            {
                TestClient? x = System.Text.Json.JsonSerializer.Deserialize<TestClient>(json);
                userName = x.客戶姓名;
            }
            return Content(userName);
        }
        public IActionResult CreateClient()
        {
            return PartialView();
        }

        public IActionResult CheckClient(TestClient x)
        {
            if (x == null)
                return Content("請輸入資料");
            else
            {
                dbCompanyTestContext _context = new dbCompanyTestContext();
                if (!_context.TestClients.Any(c => c.Email == x.Email || c.客戶電話 == x.客戶電話 || c.身分證字號 == x.身分證字號))
                {
                    int count = _context.TestClients.Count() + 1;
                    x.客戶編號 = $"CL{x.身分證字號.Substring(1, 1)}-{count.ToString("0000")}{x.身分證字號.Substring(7, 3)}";
                    _context.TestClients.Add(x);
                    _context.SaveChanges();
                    useSession(x);
                    return Content("OK");
                }
                else
                    return Content("Email,電話或身分證字號已被使用");
            }
        }

        public IActionResult forgetPassword(string Email)
        {
            dbCompanyTestContext _context = new dbCompanyTestContext();
            if (_context.TestClients.Any(c => c.Email == Email))
            {//zlazqafpmuwxkxvo
                TestClient client = _context.TestClients.FirstOrDefault(x => x.Email == Email);
                if (client.客戶編號.Substring(0, 3) == "CLG")
                    return Content("請使用Google登入");
                else if (client.客戶編號.Substring(0, 3) == "CLL")
                    return Content("請使用Line登入");
                var mail = new MailMessage();
                mail.To.Add(Email);
                mail.Subject = "Shoespace忘記密碼變更";
                mail.Body = $"<h1>Shoespace密碼變更</h1><a href=`{Environment.Environment.useEnvironment}/Login/RePassword?Email={Email}`><h2>點選這裡變更密碼</h2></a><hr/><h6>此訊息為系統自動寄出請勿直接回覆</h6>";
                //mail.Body = $"<form action = 'https://localhost:7100/Login/ResetPassword'><input type='hidden' value='{Email}' id='account' name='Email' /><input type='text' id='newPassword' name='Password'/><br/><input type='text' id='dblnewPassword' /><br/><input type='submit' value='確認'/></form>";
                mail.IsBodyHtml = true;
                mail.Priority = MailPriority.Normal;
                mail.From = new MailAddress("msit145finalpj@gmail.com", "Shoespace");
                var smtp = new SmtpClient("smtp.gmail.com", 587)
                {
                    Credentials = new System.Net.NetworkCredential("msit145finalpj@gmail.com", "zlazqafpmuwxkxvo"),
                    EnableSsl = true
                };
                smtp.Send(mail);
                mail.Dispose();
                return Content("請至信箱接收密碼更改信件");
            }
            else
                return Content("沒有這個帳號");
        }
        [HttpGet]
        public IActionResult RePassword(string Email)
        {
            ViewData["Email"] = Email;
            ViewData["url"] = Environment.Environment.useEnvironment + "/Login/Login";
            return View();
        }

        public IActionResult ResetPassword(string Email, string Password)
        {
            dbCompanyTestContext _context = new dbCompanyTestContext();
            TestClient? client = _context.TestClients.FirstOrDefault(c => c.Email == Email);
            client.密碼 = Password;
            _context.SaveChanges();
            return Content("");
        }

        public async Task<IActionResult> Line(string code, Guid state, string error, string error_description)
        {
            // 有錯誤訊息(未授權等)、State遺失、State不相同、沒有code
            if (!string.IsNullOrEmpty(error) || state == null || State != state || string.IsNullOrEmpty(code))
                return RedirectToAction(nameof(Login));

            // 使用代碼交換存取權杖 與Facebook 和 Google不同，是使用 application/x-www-form-urlencoded
            var url = "https://api.line.me/oauth2/v2.1/token";
            var postData = new Dictionary<string, string>()
        {
            {"client_id",_appId},
            {"client_secret",_appSecret},
            {"code",code},
            {"grant_type","authorization_code"},
            {"redirect_uri",RedirectUrl}
        };

            var contentPost = new FormUrlEncodedContent(postData);

            var client = _clientFactory.CreateClient();
            var response = await client.PostAsync(url, contentPost);

            string responseContent;
            if (response.IsSuccessStatusCode)
                responseContent = await response.Content.ReadAsStringAsync();
            else
                return RedirectToAction(nameof(Login));

            var lineLoginResource = JsonConvert.DeserializeObject<LINELoginResource>(responseContent);

            // 因為Line API 可以同時取得ClientId Token 和 Access Token，所以這邊有兩種選擇
            // 1. 使用JWT解析Id Token, Nuget > System.IdentityModel.Tokens.Jwt
            var userInfo = new JwtSecurityToken(lineLoginResource.IDToken).Payload;
            dbCompanyTestContext _context = new dbCompanyTestContext();
            TestClient? dbclient = _context.TestClients.FirstOrDefault(x => x.Email == userInfo["email"]);
            if (dbclient == null)
            {
                TestClient newclient = new TestClient();
                newclient.客戶編號 = "CLL-"+userInfo["sub"].ToString().Substring(0,7);
                newclient.客戶姓名 = userInfo["name"].ToString();
                newclient.Email = userInfo["email"].ToString();
                _context.TestClients.Add(newclient);
                _context.SaveChanges();
                useSession(newclient);
            }
            else
                useSession(dbclient);
            // 2. https://developers.line.biz/en/reference/social-api/#profile
            //url = $"https://api.line.me/v2/profile";
            //client.DefaultRequestHeaders.Add("authorization", $"Bearer {lineLoginResource.AccessToken}");
            //response = await client.GetAsync(url);
            //if (response.IsSuccessStatusCode)
            //{
            //    responseContent = await response.Content.ReadAsStringAsync();
            //    var user = JsonConvert.DeserializeObject<LINEUser>(responseContent);
            //}
            return RedirectToAction("index", "Home");
        }
        public class LINELoginResource
        {
            [JsonProperty("access_token")]
            public string AccessToken { get; set; }

            [JsonProperty("token_type")]
            public string TokenType { get; set; }

            [JsonProperty("expires_in")]
            public string ExpiresIn { get; set; }

            [JsonProperty("scope")]
            public string Scope { get; set; }

            [JsonProperty("refresh_token")]
            public string RefreshToken { get; set; }

            // 這邊跟一般的TokenResponse不同，多了使用者的Id Token
            [JsonProperty("id_token")]
            public string IDToken { get; set; }
        }

        //public class LINEUser
        //{
        //    [JsonProperty("userId")]
        //    public string Id { get; set; }

        //    [JsonProperty("displayName")]
        //    public string Name { get; set; }

        //    [JsonProperty("pictureUrl")]
        //    public string PictureUrl { get; set; }

        //    [JsonProperty("statusMessage")]
        //    public string StatusMessage { get; set; }
        //}
    }
}
