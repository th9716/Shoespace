//Product index.cs ProductDetail js控制
$("#btn_ProDetailControl").on('click', function () {
    $("#collapse_ProDetaol_Search").toggle(500)  
   setTimeout(function () {
       change_ProDetailtable();

    }, 500);
   
});

//用TBProDetal 代表整個TBProDetal表單
//TBProDetal Load Start===============
let TBProDetal = null;

const ProDetalLoad = (url, Proimg_url) => {
    let _keywork = $("#keywork_Pro").val();          
    TBProDetal = $('#tableProDetal').DataTable(
        {
            "language": lang,
            "searching": false,
            "ajax": url + `/?id=${_keywork}`,
            "type": 'get',
            "lengthMenu": [[5, 10, 25, 50, -1], [5, 10, 25, 50, "All"]],
            "columns":
                [ 
                    {  //master detail button
                        className: 'details-control',
                        orderable: false,
                        data: null,
                        defaultContent: '',
                        //用渲染 新checkbox
                        render: function (data, type, row) {
                            return `<input type="checkbox" name="Pro_check_delete"  value="${data.id}" >`;
                        },
                        width: '6%'
                    },
                    /* { "data": "id"},*/
                    {
                        "data": null,
                        "render": function (data, type, full, meta) {
                            return meta.row + 1 + meta.settings._iDisplayStart;
                        }
                    }, // 序號
                    { "data": "商品名稱", width: '10%' },
                    { "data": "分類", width: '5%' },
                    { "data": "鞋種", width: '5%' },
                    { "data": "明細尺寸" },
                    { "data": "顏色" },
                    { "data": "數量" },
                    { "data":"圖片位置id"},
                    {
                        data: null,
                        className: 'img1',
                        render: function (data, type, row) {
                            if (data.商品圖片1 == null)
                                return `<img src="${ProDeimg_url}/404.jpg" width="50" height="50" '/>`
                            else
                                return `<img src="${ProDeimg_url}/${data.商品圖片1}" width="50" height="50" '/>`

                        },
                        orderable: false,
                        width: '10%'
                    },
                    {
                        data: null,
                        className: 'img2',
                        render: function (data, type, row) {
                            if (data.商品圖片2 == null)
                                return `<img src="${ProDeimg_url}/404.jpg" width="50" height="50" '/>`
                            else
                                return `<img src="${ProDeimg_url}/${data.商品圖片2}" width="50" height="50" '/>`

                        },
                        orderable: false,
                        width: '10%'
                    },
                    {
                        data: null,
                        className: 'img3',
                        render: function (data, type, row) {
                            if (data.商品圖片3 == null)
                                return `<img src="${ProDeimg_url}/404.jpg" width="50" height="50" '/>`
                            else
                                return `<img src="${ProDeimg_url}/${data.商品圖片3}" width="50" height="50" '/>`

                        },
                        orderable: false,
                        width: '10%'
                    },
                    { "data": "商品是否有貨" },
                    { "data": "商品是否上架" },
                    {
                        data: null,
                        className: 'edit',
                        render: function (data, type, row) {
                            return `<button type="submit" class="btn btn-primary"  id="Prolist_btnEdit" name="DE_btnEdit" title="修改"   data-bs-toggle="modal" data-bs-target="#Moda_D_C" data-value="${data.id}"><i class="fas fa-edit"></i>修改</button>`;

                        },
                        orderable: false,
                        width: '7%'
                    },
                    {
                        data: null,
                        className: 'del',
                        render: function (data, type, row) {
                            return `<button type="submit" class="btn btn-primary" id="Prolist_btnDel" name="DE_btnDel"  title="刪除" data-bs-toggle="modal" data-bs-target="#Moda_D_D" data-value="${data.id}"><i class="fa fa-minus-square"></i>刪除</button>`;
                        },
                        orderable: false,
                        width: '7%'
                    }
                ],
           
        });
}
//  TBProDetal End===================

//重新加載類別
function change_ProDetailtable() {
    if (($("#tableProDetal tbody tr").length) > 0) {
        TBProDetal.destroy();
    }
    ProDetalLoad(urlDe, ProDeimg_url);  
}

//產品預覽圖事件
$('#tableProDetal').on('click', 'img', function () {
    const url = $(this).prop("src");
    console.log(url);
    $('#Moda_Img_View').modal('show');
    $("#Moda_Img_View").find("img").prop("src", url);
});

//加入按鈕事件(新增明細)_collapse_ProDetaol_Search
$('#collapse_ProDetaol_Search').on('click', 'button[name="Detail_list_Create"]', function () {
   // console.log(Pro_Create_url);
    $("#Moda_D_C #myModal-label").text("新增細項商品");
    $("#Moda_D_C .modal-body").load(ProDe_Create_url);
})

//查詢事件綁定
$("#collapse_ProDetaol_Search button[name='btn-Search_Pro']").on('click', function () {
    //重新載入
    change_ProDetailtable();

});

//加入按鈕事件(明細刪除) P_btnDel
$('#tableProDetal').on('click', 'button[name="DE_btnDel"]', function () {
    //const row = $(this).parents("tr[name='tr_de']");
    //const id = row.find('td:eq(0)').text().trim();
    const id = $(this).attr("data-value")
    Send_DatatoDel_Moda(ProDe_Delet_url, id)
});

//加入按鈕事件(明細修改) P_btnEdit
$('#tableProDetal').on('click', 'button[name="DE_btnEdit"]', function () {
    const id = $(this).attr("data-value")
    console.log(id);
    $("#Moda_D_C #myModal-label").text("修改細項商品");
    //顯示Moda_D_C相應資料load _EditDetial
    $("#Moda_D_C .modal-body").load(`${ProDe_Edit_url}/?id=${id}`);
});


//加入批量全選 name=check_All_delete
$("#tableProDetal").on('click', 'input[name="TBcheck_AllDetail"]', function () {
   // const color = $(this).val();
   
    if ($(this).prop('checked') == true) {
        $("#tableProDetal").find(`input[name="Pro_check_delete"]`).each(function () {
            $(this).prop("checked", true);
        });
    } else {
        $("#tableProDetal").find(`input[name="Pro_check_delete"]`).each(function () {
            $(this).prop("checked", false);
        });
    }


});

//加入批量刪除事件
$('#collapse_ProDetaol_Search').on('click', 'button[name="batch_Delete"]', function () {
    console.log("123");
    const thisgroup = $("#collapse_ProDetaol_Search")
    //取得組數
    let checkedId = new Array();
    $("#tableProDetal").find(`input[name="Pro_check_delete"]:checked`).each(function () {
        if (!isNaN($(this).val())) {
            checkedId.push($(this).val());
        } else {
            console.log("lost value");
        }
    });
    
    console.log(checkedId);
    console.log(checkedId.length);
    if (checkedId.length == 0) {
        //alert要改
        alert("請選擇要刪除的資料!");
        return false;
    }
    //  console.log("陣列：" + checkedId);
    //  console.log("id串：" + checkedId.toString());   Moda_D_D
    const id = checkedId.toString();
    Send_DatatoDel_Moda(ProDe_Delet_url, id);
    $('#Moda_D_D').modal('toggle');

});