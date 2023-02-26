//=======ProductRelevant========================================
//商品分類
function check_ProType_Edit() {
    let form = $("#Form_ProType_E");
    let errorstr = "";
    //商品分類名稱
    let 商品分類名稱 = form.find("input[name='商品分類名稱']");
    errorstr += check_null_lenth(商品分類名稱, "商品分類名稱")
    return errorstr;
}
function check_ProType_C() {
    let form = $("#Form_ProType_C");
    let errorstr = "";
    //商品分類名稱
    let 商品分類名稱 = form.find("input[name='商品分類名稱']");
    errorstr += check_null_lenth(商品分類名稱, "商品分類名稱")
    return errorstr;
}

//商品尺寸
function check_ProSize() {
    let form = $("form[name='proInfo']");
    let errorstr = "";
    //尺寸種類
    let 尺寸種類 = form.find("input[name='尺寸種類']");
  //  errorstr += check_null_number(尺寸種類, "尺寸種類")
    errorstr += check_null_lenth(尺寸種類, "尺寸種類")
    return errorstr;
}

//商品鞋種
function check_Shoes() {
    let form = $("form[name='proInfo']");
    let errorstr = "";
    //鞋種
    let 鞋種 = form.find("input[name='鞋種']");
    errorstr += check_null_lenth(鞋種, "鞋種")
    return errorstr;
}

//商品顏色
function check_ProColor_E() { 
    let form = $("form[name='proInfo']");
    let errorstr = "";
    //商品分類名稱
    let 商品顏色種類 = form.find("input[name='商品顏色種類']");
    errorstr += check_null_lenth(商品顏色種類, "商品顏色種類");
    //色碼
    let 色碼 = form.find("input[name='色碼']");
    errorstr += check_null_lenth(色碼, "色碼")
   
    return errorstr;
}

//商品顏色
function check_ProColor_C() {
    /*$.ajaxSettings.async = false;*/
    let form = $("form[name='proInfo']");
    let errorstr = "";
    //商品分類名稱
    let 商品顏色種類 = form.find("input[name='商品顏色種類']");
    errorstr += check_null_lenth(商品顏色種類, "商品顏色種類");
    //色碼
    let 色碼 = form.find("input[name='色碼']");
    errorstr += check_null_lenth(色碼, "色碼")
    //商品顏色圖片
    let 商品顏色圖片 = form.find("input[name='商品顏色圖片']");
    errorstr += check_file_null(商品顏色圖片, "商品顏色圖片")
    /*$.ajaxSettings.async = true;*/
    return errorstr;
}



//=======Product==============================================

//產品明細
function check_ProDetail_Edit() {
    let form = $("#Form_ProDe_E");
    let errorstr = "";
    //商品數量
    let 商品數量 = form.find("input[name='商品數量']");
    errorstr += check_null_number(商品數量, "商品數量");
    //先判斷創建圖片是否被勾選
    if (form.find("#create_image").prop("checked")) {
        //photo1
        let photo1 = form.find("input[name='photo1']");
        /*  console.log(photo1);*/
        errorstr += check_file_null(photo1, "照片1")
        //photo2
        let photo2 = form.find("input[name='photo2']");
        errorstr += check_file_null(photo2, "照片2")
        //photo3
        let photo3 = form.find("input[name='photo3']");
        errorstr += check_file_null(photo3, "照片3")
    }  
    return errorstr;
}

//產品明細
function check_ProDetail_Create() {
    let form = $("#Form_ProDe_C");
    let errorstr = "";  
    //商品數量
    let 商品數量 = form.find("input[name='數量']");
    errorstr += check_null_number(商品數量, "商品數量");
    //先判斷創建圖片是否被勾選
    if (form.find("#create_image").prop("checked"))
    {
        //photo1
        let photo1 = form.find("input[name='photo1']");
        /*  console.log(photo1);*/
        errorstr += check_file_null(photo1, "照片1")
        //photo2
        let photo2 = form.find("input[name='photo2']");
        errorstr += check_file_null(photo2, "照片2")
        //photo3
        let photo3 = form.find("input[name='photo3']");
        errorstr += check_file_null(photo3, "照片3")
    }  
    return errorstr;
}


//清空前端警告
function clear_check(obj) {
    
    obj.find("input").removeClass("is-invalid");
    obj.find("span").html("");
}

function check_file_null(obj, mess) {
    let error = "";
    let file = obj.get(0);
    if (file.files.length == 0) {
        error += `請輸入${mess}`;
        obj.prop("class", "form-control is-invalid")
        obj.next(".invalid-feedback").find("span").html(`請輸入${mess}`);
    }
    else {
        obj.prop("class", "form-control is-valid");
        obj.next(".invalid-feedback").html("");
    }
    return error;
}

//產品新增與修改
function check_ProC_Form() {
    let form01 = $("#Moda_P_C");
    let errorstr = "";

    let time = form01.find("input[name='time']");
    errorstr += check_null(time, "上架日期")
   
    //商品名稱
    let name = form01.find("input[name='name']");
    errorstr += check_null_lenth(name, "品名")

    //商品價格
    let price = form01.find("input[name='price']");
    errorstr += check_null_number(price, "價格")
    
    //商品介紹
    let introduce = form01.find("#_introduce");
    errorstr += check_null(introduce, "商品介紹")
    
    //商品材質   
    let material = form01.find("input[name='material']");
    errorstr += check_null_lenth(material, "材質")

    //商品重量
    let weight = form01.find("input[name='weight']");
    errorstr += check_null_number(weight, "重量")

    //商品成本
    let cost = form01.find("input[name='cost']");
    errorstr += check_null_number(cost, "成本")

    return errorstr;
}


function check_null(obj, mess) {
    let error = "";   
    if (obj.val() == "") {
        error += `請輸入${mess}`;   
        obj.prop("class", "form-control is-invalid")
        obj.next(".invalid-feedback ").find("span").html(`請輸入${mess}`);
    }
    else {
        obj.prop("class", "form-control is-valid");
        obj.next(".invalid-feedback").html("");
    }
    return error;
}

function check_null_number(obj, mess) {
    let error = "";
    if (obj.val() == "") {
        error += `請輸入${mess}`;
        obj.prop("class", "form-control is-invalid")
        obj.next(".invalid-feedback ").find("span").html(`請輸入${mess}`);
    }
    
    if (!isNaN(parseInt(obj.val()))) { 
        if (parseInt(obj.val()) < 0) {
            error += `不能為負數`;
            obj.prop("class", "form-control is-invalid")
            obj.next(".invalid-feedback ").find("span").html(`不能為負數`);
        }

    } else {
        error += `請輸入數字`;
        obj.prop("class", "form-control is-invalid")
        obj.next(".invalid-feedback ").find("span").html(`請輸入數字`);
    }


    if (error.length == 0) {
        obj.prop("class", "form-control is-valid");
        obj.next(".invalid-feedback").find("span").html("");
    }
    return error;
}



function check_null_lenth(obj, mess) {
    let error = "";   
    if (obj.val() == "") {
        error += `請輸入${mess}`;
        
        obj.prop("class", "form-control is-invalid")
        obj.next(".invalid-feedback").find("span").html(`請輸入${mess}`);
    }
    if (obj.val().length > 45) {
        error += "字數超出限制!";
      
        obj.prop("class", "form-control is-invalid")
        obj.next(".invalid-feedback").find("span").html("字數超出限制");
    }
 
    if (error.length == 0) {
        obj.prop("class", "form-control is-valid");
        obj.next(".invalid-feedback").find("span").html("");
    }

    return error;
}