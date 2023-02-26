using dbCompanyTest.Controllers;
using dbCompanyTest.Models;
using dbCompanyTest.ViewModels;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System.Text.Json;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace dbCompanyTest.Hubs
{
    public class chatHub : Hub
    {
        // 用戶連線 ID 列表
        public static List<user> userList = new List<user>();
        public static IHubContext<chatHub> Current { get; set; }

        public override async Task OnConnectedAsync()
        {
            if (userList.Where(p => p.connectionId == Context.ConnectionId).FirstOrDefault() == null)
            {
                user newuser = new user();
                newuser.connectionId = Context.ConnectionId;
                newuser.userName = "訪客";
                userList.Add(newuser);
            }

            //await Clients.All.SendAsync("UpdList", jsonString);
            //await Clients.Client(Context.ConnectionId).SendAsync("UpdSelfID", Context.ConnectionId);
            //await Clients.Client(Context.ConnectionId).SendAsync("UpdSystem", "系統", "連線成功");

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception ex)
        {
            user id = userList.Where(p => p.connectionId == Context.ConnectionId).FirstOrDefault();
            if (id != null)
                userList.Remove(id);
            user olduser = userList.FirstOrDefault(x => x.waiter == Context.ConnectionId);
            if (olduser != null)
                olduser.waiter = null;
            Update();
            if (id.waiter != null)
                await Clients.Client(id.waiter).SendAsync("UpdSystem", "系統", id.userName + " 已離線");

            await base.OnDisconnectedAsync(ex);
        }

        public async Task SendMessage(string ClientID, string message/*, string sendToID*/)
        {//客戶傳訊息
            if (message != "")
            {
                user user = userList.FirstOrDefault(x => x.connectionId == Context.ConnectionId);
                if (user.userName.Substring(0, 2) != "ST")
                {
                    if (user.userWords == null)
                        user.userWords = new List<string>();
                    if (user.waiter == null)
                        user.newWords++;
                    else
                        user.newWords = 0;

                    user.userWords.Add(message);
                    string userName = user.userName;
                    await Clients.Client(Context.ConnectionId).SendAsync("UpdContent", message);
                    if (user.waiter == null)
                    {
                        if (user.userWords.Count == 1)
                            await Clients.Client(Context.ConnectionId).SendAsync("UpdSystem", "系統", "客服全在忙線中請稍後");
                    }
                    else
                        await Clients.Client(user.waiter).SendAsync("UpdSystem", userName, message);
                    Update();
                }
                else
                {
                    if (ClientID.Substring(0, 4) != "LINE")
                    {
                        user clients = userList.FirstOrDefault(x => x.connectionId == ClientID);
                        clients.userWords.Add("S" + message);
                        await Clients.Client(ClientID).SendAsync("UpdSystem", "客服人員", message);
                        await Clients.Client(Context.ConnectionId).SendAsync("UpdContent", message);
                    }
                    else
                    {
                        user client = userList.FirstOrDefault(x => x.LineID == ClientID);
                        client.userWords.Add("S" + message);
                        //傳送Line訊息方法
                        new LineBot().SendLineMessage(client.LineID.Substring(4, client.LineID.Count() - 4), message);
                        await Clients.Client(Context.ConnectionId).SendAsync("UpdContent", message);
                    }
                }
            }
        }

        public async Task LineSendMessage(user client, string message)
        {
            if (client.waiter != null)
                await Clients.Client(client.waiter).SendAsync("UpdSystem", client.userName, message);
        }

        public async Task getName(string userName)
        {//設定使用者名稱
            userList.FirstOrDefault(c => c.connectionId == Context.ConnectionId).userName = userName;
            Update();
        }
        public async Task bindWaiterUser(string userId)
        {
            user? olduser = userList.FirstOrDefault(x => x.waiter == Context.ConnectionId);
            if (olduser != null)
                olduser.waiter = null;
            user user;
            if (userId.Substring(0, 4) == "LINE")
                user = userList.FirstOrDefault(x => x.LineID == userId);
            else
                user = userList.FirstOrDefault(x => x.connectionId == userId);
            if (user.waiter == null)
            {
                user.waiter = Context.ConnectionId;
                string userMsgJSON = JsonConvert.SerializeObject(user.userWords);
                user.newWords = 0;
                await Clients.Client(Context.ConnectionId).SendAsync("newClientMsg", userMsgJSON);
                Update();
            }
            else
            {
                await Clients.Client(Context.ConnectionId).SendAsync("rechoose", "此客戶已有人在回應");
            }
        }

        public async Task Update()
        {
            List<user> client = userList.Where(x => x.userName.Substring(0, 2) != "ST" && x.userWords != null).ToList();
            string jsonString = JsonConvert.SerializeObject(client);
            List<string> waiter = userList.Where(x => x.userName.Substring(0, 2) == "ST").Select(x => x.connectionId).ToList();
            foreach (string item in waiter)
                await Clients.Client(item).SendAsync("userList", jsonString);
        }

        public async Task SendNotice(string come_from_num, string Send_To_num, string msg, string listtype, string listnum, string Sta)
        {
            string Send_To_ID = "";
            string come_from_ID = userList.FirstOrDefault(x => x.userName == come_from_num).connectionId;
            if (userList.Where(x => x.userName == Send_To_num).FirstOrDefault() != null)  //todo 如果被通知人不在線上，就把通知存資料庫，登入再來讀
            {
                Send_To_ID = userList.FirstOrDefault(x => x.userName == Send_To_num).connectionId;
                await Clients.Client(Send_To_ID).SendAsync("receive", msg, listtype, listnum, Sta);
            }
        }
        //回復商品
        public async Task SendComment(string response)
        {
            var data = JsonSerializer.Deserialize<ProductDetailViewModels>(response);
            data.客戶編號 = null;
            data.員工編號 = null;
            data.collapseParetid = null;
            string json = JsonSerializer.Serialize(data);
            //抓出發送者
            var user = userList.FirstOrDefault(x => x.connectionId == Context.ConnectionId);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, user.roomName);
            await Clients.Group(user.roomName).SendAsync("UpdMessage",json);
            await Clients.Clients(Context.ConnectionId).SendAsync("UpdMessage", response);
            await Groups.AddToGroupAsync(Context.ConnectionId, user.roomName);
        }
        //JoinGroup
        public async Task JoinRoom(int productid, int colorid)
        {

            if (productid != null && colorid != null)
            {
                string roomName = productid.ToString() + colorid.ToString();
                var user = userList.FirstOrDefault(x => x.connectionId == Context.ConnectionId);
                if (user != null)
                {
                    user.roomName = roomName;
                    await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
                }
            }
           
        }

    }
}