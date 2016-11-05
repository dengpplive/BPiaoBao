function sendPid() {
    var url = "/BlackQuery/GetBlackData";
    var Cmd = $.trim($("#txtCmd").val());
    var ipport = $("input[type='radio'][name='rdip']:checked").val();//$("#ShowIP").text();
    if (ipport == null) {
        alert("请选择运营商！");
        return;
    }
    if (ipport.split(":").length != 2) {
        //$.messager.alert("提示", "IP端口号不对或者未设置！", "info");
        alert("IP端口号不对或者未设置！");
    } else {
        if (Cmd != "") {
            var IP = ipport.split(":")[0];
            var Port = ipport.split(":")[1]
            var Office = $.trim($("#txtOffice").val());
            var showOffice = $.trim($("#showOffice").text()).split('|');
            if (Office == "" && showOffice.length > 0 && showOffice[0] != "") {
                Office = showOffice[0];
            }
            var data = { Cmd: Cmd, IP: IP, Port: Port, Office: Office, num: Math.random() };
            $("#btnSend").attr("enabled", false);
            $.post(url, data, function (result) {
                $("#btnSend").attr("enabled", true);
                var res = $("#txtAcceptData").val();
                if (res != "") {
                    res += "\r\n" + result.msg.Message;
                } else {
                    res += result.msg.Message;
                }
                $("#txtAcceptData").val(res);
            }, "json");
        }
    }
}
function setChange(sel) {
    try {
        var top = $(sel).offset().top;      
        $(".AP_div").attr("top", top + "px");
        var valArr = $(sel).val().split('#');
        if (valArr.length == 2) {
            var ipArr = valArr[0].split('^');
            var office = valArr[1];
            var htmlInput = [];
            var isel = "checked='checked'";
            for (var i = 0; i < ipArr.length; i++) {
                if (i > 0) { isel = ""; }
                htmlInput.push("<label for='rdip_" + i.toString() + "'><input type='radio' " + isel + " id='rdip_" + i.toString() + "' name='rdip' value='" + ipArr[i] + "' />" + ipArr[i] + "</label><br/>");
            }
            $("#ShowIP").html(htmlInput.join(""));
            $("#showOffice").text(office);
        } else {
            $("#ShowIP").html("");
            $("#showOffice").text("");
        }
    } catch (e) {

    }
}
$(document).keyup(function (event) {
    if (event.keyCode == 13) {
        var obj = event.target ? event.target : event.srcElement;
        if (obj != null && obj.tagName != "TEXTAREA") {
            sendPid();
        }
    }
});
$(function () {
    $("#btnSend").click(sendPid);
    $("#selGY option:eq(0)").attr("selected", true);
});
