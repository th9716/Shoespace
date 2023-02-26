$.each(data, function (i, i_value) { $("#city").append(`<option>${i_value.name}</option>`) });

$("#city").change(function () {
    let Myarea = $(`#city option:selected`).index();
    area(Myarea);
})
$(`#city`).val($(`#q1`).val());
//console.log($(`#q1`).val());
if ($(`#q1`).val()) {
    //console.log($(`#city option:selected`).index());
    area($(`#city option:selected`).index());
}
if ($(`#sexvalue`).val()) {
    $(`#sex`).val($(`#sexvalue`).val().trim());
}

function area(a) {
    $("#town").empty();
    $.each(data[a].districts, function (j, j_value) { $("#town").append(`<option>${j_value.name}</option>`) })
    if ($(`#q2`).val()) {
        $(`#town`).val($(`#q2`).val().trim());
        //console.log($(`#q2`).val());
    }
}
