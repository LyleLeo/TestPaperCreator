$(document).ready(function () {
    $("#username").focus()
})
function init() {
    $("#alert").text("")
    $("#alert").hide()
}
function isEmail(str) {
    var reg = /^(\w)+(\.\w+)*@@(\w)+((\.\w{2,3}){1,3})$/
    return reg.test(str);
}
function login() {
    var username = $("#username").val()
    var password = $("#password").val()
    if (username != "" && password != "") {
        var logintype = isEmail(username) ? 1 : 0
        var rememberme = 0
        if ($("#rememberme").prop("checked")) {
            rememberme = 1
        }
        var user = {
            username: username,
            password: hex_md5(password),
            logintype: logintype,
            rememberme: rememberme,
        }
        $.ajax({
            url: "/api/MembershipAPI/Login/",
            type: "POST",
            dataType: "JSON",
            data: user,
            beforeSend: function () {
                $("#submit").val("正在登录中")
                $("#submit").attr("disabled", "disabled")
            },
            success: function (result) {
                if (result == true) {
                    window.location.href = "/Home/Index/"
                }
                else {
                    $("#alert").text("对不起，账号或密码有误")
                    $("#alert").show()
                    $("#submit").val("登录")
                    $("#submit").removeAttr("disabled")
                    return
                }
            }
        })
    }
    else {
        $("#alert").text("用户名或密码不能为空")
        $("#alert").show()
    }
}