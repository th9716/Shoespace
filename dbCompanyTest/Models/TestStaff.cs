using System;
using System.Collections.Generic;

namespace dbCompanyTest.Models
{
    public partial class TestStaff
    {
        public TestStaff()
        {
            ToDoLists = new HashSet<ToDoList>();
        }

        public string 員工編號 { get; set; } = null!;
        public string? 員工姓名 { get; set; }
        public string? 員工電話 { get; set; }
        public string? 身分證字號 { get; set; }
        public string? 縣市 { get; set; }
        public string? 區 { get; set; }
        public string? 地址 { get; set; }
        public string? Email { get; set; }
        public string? 緊急聯絡人 { get; set; }
        public string? 聯絡人關係 { get; set; }
        public string? 聯絡人電話 { get; set; }
        public string? 部門 { get; set; }
        public string? 主管 { get; set; }
        public string? 職稱 { get; set; }
        public string? 密碼 { get; set; }
        public string? 薪資 { get; set; }
        public string? 權限 { get; set; }
        public string? 在職 { get; set; }

        public virtual ICollection<ToDoList> ToDoLists { get; set; }
    }
}
