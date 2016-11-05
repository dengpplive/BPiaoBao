




Date.prototype.format = function (format) {
    var o = {
        "M+": this.getMonth() + 1, //month
        "d+": this.getDate(), //day
        "h+": this.getHours(), //hour
        "m+": this.getMinutes(), //minute
        "s+": this.getSeconds(), //second
        "q+": Math.floor((this.getMonth() + 3) / 3), //quarter
        "S": this.getMilliseconds() //millisecond
    }
    if (/(y+)/.test(format)) format = format.replace(RegExp.$1,
     (this.getFullYear() + "").substr(4 - RegExp.$1.length));
    for (var k in o) if (new RegExp("(" + k + ")").test(format))
        format = format.replace(RegExp.$1,
        RegExp.$1.length == 1 ? o[k] :
        ("00" + o[k]).substr(("" + o[k]).length));
    return format;
};
 
function JsonTimeStrFormater(value) {
    if (value == null || value == "") {
        return "";
    }
    var jsonDate = new Date(parseInt(value.substring(6, value.length - 2)));
    var result = jsonDate.format("yyyy-MM-dd h:mm:ss");
    return result;
}

function getDate(strDate) {

        str = strDate.replace(/-/g, "/");
        var date = new Date(str);
        return date;

}