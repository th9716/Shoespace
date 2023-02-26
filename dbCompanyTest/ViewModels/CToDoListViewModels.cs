using dbCompanyTest.Models;
using NPOI.SS.Formula.Functions;

namespace dbCompanyTest.ViewModels
{
    public class CToDoListViewModels
    {
        private ToDoList _toDoList;

        public CToDoListViewModels()
        {
            _toDoList = new ToDoList();
        }

        public ToDoList? toDoList
        {
            get { return _toDoList; }
            set { _toDoList = value; }
        }


        public int 交辦事項id
        {
            get { return _toDoList.交辦事項id; }
            set { _toDoList.交辦事項id = value; }
        }
        public string 員工編號
        {
            get { return _toDoList.員工編號; }
            set { _toDoList.員工編號 = value; }
        }
        public string 表單類型
        {
            get { return _toDoList.表單類型; }
            set { _toDoList.表單類型 = value; }
        }
        public string 表單內容
        {
            get { return _toDoList.表單內容; }
            set { _toDoList.表單內容 = value; }
        }
        public string? 回覆
        {
            get { return _toDoList.回覆; }
            set { _toDoList.回覆 = value; }
        }
        public string 表單狀態
        {
            get { return _toDoList.表單狀態; }
            set { _toDoList.表單狀態 = value; }
        }
        public string? 起單時間
        {
            get { return _toDoList.起單時間; }
            set { _toDoList.起單時間 = value; }
        }
        public string? 起單人
        {
            get { return _toDoList.起單人; }
            set { _toDoList.起單人 = value; }
        }
        public string? 部門主管
        {
            get { return _toDoList.部門主管; }
            set { _toDoList.部門主管 = value; }
        }
        public string? 部門主管簽核
        {
            get { return _toDoList.部門主管簽核; }
            set { _toDoList.部門主管簽核 = value; }
        }
        public string? 部門主管簽核意見
        {
            get { return _toDoList.部門主管簽核意見; }
            set { _toDoList.部門主管簽核意見 = value; }
        }
        public string? 部門主管簽核時間
        {
            get { return _toDoList.部門主管簽核時間; }
            set { _toDoList.部門主管簽核時間 = value; }
        }
        public string? 協辦部門
        {
            get { return _toDoList.協辦部門; }
            set { _toDoList.協辦部門 = value; }
        }
        public string? 協辦部門簽核
        {
            get { return _toDoList.協辦部門簽核; }
            set { _toDoList.協辦部門簽核 = value; }
        }
        public string? 協辦部門簽核人員
        {
            get { return _toDoList.協辦部門簽核人員; }
            set { _toDoList.協辦部門簽核人員 = value; }
        }
        public string? 協辦部門簽核意見
        {
            get { return _toDoList.協辦部門簽核意見; }
            set { _toDoList.協辦部門簽核意見 = value; }
        }
        public string? 協辦部門簽核時間
        {
            get { return _toDoList.協辦部門簽核時間; }
            set { _toDoList.協辦部門簽核時間 = value; }
        }
        public string? 老闆簽核
        {
            get { return _toDoList.老闆簽核; }
            set { _toDoList.老闆簽核 = value; }
        }
        public string? 老闆簽核意見
        {
            get { return _toDoList.老闆簽核意見; }
            set { _toDoList.老闆簽核意見 = value; }
        }
        public string? 老闆簽核時間
        {
            get { return _toDoList.老闆簽核時間; }
            set { _toDoList.老闆簽核時間 = value; }
        }
        public string? 執行人
        {
            get { return _toDoList.執行人; }
            set { _toDoList.執行人 = value; }
        }
        public string? 執行時間
        {
            get { return _toDoList.執行時間; }
            set { _toDoList.執行時間 = value; }
        }
        public string? 執行人簽核
        {
            get { return _toDoList.執行人簽核; }
            set { _toDoList.執行人簽核 = value; }
        }
        public string? 附件
        {
            get { return _toDoList.附件; }
            set { _toDoList.附件 = value; }
        }
        public string? 附件path
        {
            get { return _toDoList.附件path; }
            set { _toDoList.附件path = value; }
        }

        public IFormFile? File{ get; set; }


        public string? 起單部門 { get; set; }
        public string? 老闆部門 { get; set; }
        public string? 執行部門 { get; set; }
        public string? 老闆 { get; set; }
    }
}
