var Staff_Home_StaffNum = $("#StaffNumpath").val();
//console.log(Staff_Home_StaffNum)
//var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

//connection.start().then(async function () {
//    const data = await fetch(`${Staff_Home_StaffNum}`);
//    const StaffNum = await data.text();
//    if (StaffNum == "fales")
//        alert("連線逾時請重新登入");
//    else
//        connection.invoke("getName", StaffNum).catch(function (err) {
//            alert('傳送錯誤: ' + err.toString());
//        });
//});

$("#inp_start").on("click", function () {
    var split_name_num = $(`#stf_info`).text().split(' ');
    var name_num = `${split_name_num[0]}${split_name_num[1]}`
    
    let listtype = $("#list_type").val(); 
    let listnum = $("#listnumber").val();  //create要去controller linq
    let come_from_num = split_name_num[1];
    console.log(come_from_num);
    let msg = `您有來自${name_num}的新表單待簽`;
    console.log(come_from_num);
    console.log(Send_To_num);
    console.log(msg);
    console.log(listtype);
    console.log(listnum)
    console.log(`************************`)
    connection.invoke("SendNotice", come_from_num, Send_To_num, msg, listtype, listnum, Sta).catch(function (err) {
        alert('傳送錯誤: ' + err.toString());
    });
});




connection.on("receive", function (msg, listtype, listnum, Sta) {
    console.log(msg);
    console.log(listtype);
    console.log(listnum);
    if (Sta == `next`) {
        note_newlist(msg);
    }
    else if (Sta == `back`) {
        note_listBack(listnum)
    }
    let TDL_DTpath = $(`#TDL_DTpath1`).val();
    $("#online").addClass("avatar-online");
    $("#null_note").css("display", "none");
    //$("#layout_note_List").append(`  <li class="menu-item">
    //                                    <a href="${TDL_DTpath}/?listNum=${listnum}" class="menu-link">
    //                                        <div data-i18n="Blank">${listtype}${listnum}</div>
    //                                    </a>
    //                                </li>`)
    $("#layout_note_List").append(`<li>
                                        <div class="dropdown-divider"></div>
                                    </li>
                                    <li>
                                        <a class="dropdown-item" href="${TDL_DTpath}/?listNum=${listnum}&listType=${listtype}">
                                            <span class="align-middle">${listtype}${listnum}</span>
                                        </a>
                                    </li>`)
});



function note_newlist(msg) {
    new Noty({
        //alert,success,warning,error,info
        type: 'success',
        text: `${msg}`,
        timeout: 5000,
        closeWith: ['click', 'button']
    }).show();
}
function note_listBack(listnum) {
    new Noty({
        //alert,success,warning,error,info
        type: 'error',
        text: `表單${listnum}被退回`,
        timeout: 5000,
        closeWith: ['click', 'button']
    }).show();
}