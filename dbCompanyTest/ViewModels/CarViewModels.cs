using dbCompanyTest.Models;

namespace dbCompanyTest.ViewModels
{
    public class CarViewModels
    {
        private 會員商品暫存 _car;
        private 圖片位置 _car圖片位置;
        public CarViewModels()
        {
            _car = new 會員商品暫存();
            _car圖片位置=new 圖片位置();
        }
        public 圖片位置 car圖片位置
        {
            get { return _car圖片位置; }
            set { _car圖片位置 = value; }
        }
        public 會員商品暫存 car
        {
            get { return _car; }
            set { _car = value; }
        }
        public string? 商品圖片1
        {
            get { return _car圖片位置.商品圖片1; }
            set { _car圖片位置.商品圖片1 = value; }
        }
        public int Id
        {
            get { return _car.Id; }
            set { _car.Id = value; }
        }
        public string? 客戶編號
        {
            get { return _car.客戶編號; }
            set { _car.客戶編號 = value; }
        }
        public int? 商品編號
        {
            get { return _car.商品編號; }
            set { _car.商品編號 = value; }
        }
        public string? 商品名稱
        {
            get { return _car.商品名稱; }
            set { _car.商品名稱 = value; }
        }
        public string? 尺寸種類
        {
            get { return _car.尺寸種類; }
            set { _car.尺寸種類 = value; }
        }
        public string? 商品顏色種類
        {
            get { return _car.商品顏色種類; }
            set { _car.商品顏色種類 = value; }
        }
        public int? 訂單數量
        {
            get { return _car.訂單數量; }
            set { _car.訂單數量 = value; }
        }
        public decimal? 商品價格
        {
            get { return _car.商品價格; }
            set { _car.商品價格 = value; }
        }


    }
}
