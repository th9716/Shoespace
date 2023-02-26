using dbCompanyTest.Models;

namespace dbCompanyTest.ViewModels
{
    public class CTestStaffViewModels
    {
        private TestStaff _testStaff;

        public CTestStaffViewModels()
        {
            _testStaff= new TestStaff();
        }

        public TestStaff? testStaff 
        {
            get { return _testStaff; }
            set { _testStaff = value; }
        }

        public string? 員工編號
        {
            get { return _testStaff.員工編號; }
            set { _testStaff.員工編號 = value; }
        }
        public string? 員工姓名 
        {
            get { return _testStaff.員工姓名; }
            set { _testStaff.員工姓名 = value; }
        }
        public string? 員工電話 
        {
            get { return _testStaff.員工電話; }
            set { _testStaff.員工電話 = value; }
        }
        public string? 身分證字號 
        {
            get { return _testStaff.身分證字號; }
            set { _testStaff.身分證字號 = value; }
        }
        public string? 縣市 
        {
            get { return _testStaff.縣市; }
            set { _testStaff.縣市 = value; }
        }
        public string? 區 
        {
            get { return _testStaff.區; }
            set { _testStaff.區 = value; }
        }
        public string? 地址 {
            get { return _testStaff.地址; }
            set { _testStaff.地址 = value; }
        }
        public string? Email {
            get { return _testStaff.Email; }
            set { _testStaff.Email = value; }
        }
        public string? 緊急聯絡人 {
            get { return _testStaff.緊急聯絡人; }
            set { _testStaff.緊急聯絡人 = value; }
        }
        public string? 聯絡人關係 {
            get { return _testStaff.聯絡人關係; }
            set { _testStaff.聯絡人關係 = value; }
        }
        public string? 聯絡人電話 {
            get { return _testStaff.聯絡人電話; }
            set { _testStaff.聯絡人電話 = value; }
        }
        public string? 部門 {
            get { return _testStaff.部門; }
            set { _testStaff.部門 = value; }
        }
        public string? 主管 {
            get { return _testStaff.主管; }
            set { _testStaff.主管 = value; }
        }
        public string? 職稱 {
            get { return _testStaff.職稱; }
            set { _testStaff.職稱 = value; }
        }
        public string? 密碼 {
            get { return _testStaff.密碼; }
            set { _testStaff.密碼 = value; }
        }
        public string? 薪資 {
            get { return _testStaff.薪資; }
            set { _testStaff.薪資 = value; }
        }
        public string? 權限 {
            get { return _testStaff.權限; }
            set { _testStaff.權限 = value; }
        }
        public string? 在職 {
            get { return _testStaff.在職; }
            set { _testStaff.在職 = value; }
        }

        public IFormFile? File { get; set; }

    }
}
