console.log(`LoadSheeplist.js`)


var path = $(`#path`).val();
//var data=null;
$.ajax({
    url: `${path}`,
    type: "GET",
    dataType: "json"
})

    .done(data => {
        console.log(`Start_LoadSheeplist_Data`)


        let docFrag = $(document.createDocumentFragment());
        let docFrag_sec = $(document.createDocumentFragment());
        let lastSID = "";
        let rowCound = 0;
        $.each(data, function (i, i_val) {
            let eleT = "";
            let eleT_sec = "";
           
            if (lastSID !== i_val.訂單編號) {
                rowCound++;
                if (rowCound <= 5) {
                    eleT = $(`<tr class="tr-border"></tr>`).append(`<td  class="col-1  tds">${i_val.訂單編號}</td>
                        <td  class="col-1 tds">${i_val.客戶編號}</td>                       
                        <td  class="col-1 tds ">${i_val.商品數量}</td>
                        <td  class="col-3 tdl">${i_val.送貨地址}</td>
                        <td  class="col-1 tds">
                        <button class="btn btn-primary mb-3 btnView" data-bs-toggle="modal" data-bs-target="#addModal">詳細資料</button> 
                        </td>`
                    );
                    docFrag.append(eleT);
                }
                else {
                    eleT_sec = $(`<tr class="tr-border"></tr>`).append(`<td  class="col-1 tds">${i_val.訂單編號}</td>
                        <td  class="col-1 tds">${i_val.客戶編號}</td>                       
                        <td  class="col-1 tds">${i_val.商品數量}</td>
                        <td  class="col-3 tdl">${i_val.送貨地址}</td>
                        <td  class="col-1 tds">
                        <button class="btn btn-primary mb-3 btnView" data-bs-toggle="modal" data-bs-target="#addModal">詳細資料</button> 
                        </td>`
                    );
                    docFrag_sec.append(eleT_sec);
                }

            }
            lastSID = i_val.訂單編號
        });
        $("#Sheeplist_tbody").prepend(docFrag);
        $("#Sheeplist_tbody_sec").append(docFrag_sec);





        $(".btnView").on('click', function () {
            let SID = $(this).parents("td").siblings("td").first().text()
            let CID; let Add; let SIDCount = 0; let SIDIndex;
            lastSID = SID;
            $.each(data, function (k, k_val) {
                if (lastSID == k_val.訂單編號) {
                    SIDCount++;
                    CID = k_val.客戶編號;
                    Add = k_val.送貨地址;
                    SIDIndex = k - SIDCount + 1;
                }
            });
            $(`#SID`).val(SID); $(`#CID`).val(CID); $(`#Add`).val(Add);
            for (j = SIDIndex; j < SIDCount + SIDIndex; j++) {
                const eleODT = $(`<tr></tr>`).append(`
                 <td>${data[j].商品名稱}</td>
                 <td>${data[j].尺寸種類}</td>
                 <td>${data[j].色碼}</td>
                 <td>${data[j].商品數量}</td>`)
                docFrag.append(eleODT);
            }
            $("#SDT_tbody").html(docFrag);
        });
    })


