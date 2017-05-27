$(document).ready(function () {
    $("#email").focus()
})
//确认资料是否完整
function ensure() {
    var username = $("#username").val()
    var password = $("#password").val()
    var password_again = $("#password_again").val()
    var email = $("#email").val()
    var code = $("#code").val()
    if (username != "" && password != "" && password_again != "" && email != "" && code != "" && $(".form-group.has-error").length == 0) {
        $("#submit").removeAttr("style")
        $("#submit").attr("onclick", "regist()")
    }
    else {
        $("#submit").removeAttr("onclick")
        $("#submit").attr("style", "cursor:not-allowed")
    }
}
//警告提示框
function warning(warnname, alert, flag) {
    if (flag == true) {
        $("#" + warnname).parent().parent().attr("class", "form-group has-success")
        ensure()
    }
    else {
        console.log(11111)
        $("#" + warnname).parent().parent().attr("class", "form-group has-error")
    }
    $("#" + warnname + "_tip").html(alert)
    $("#" + warnname + "_tip").show()
}
//验证邮箱格式是否合法
function verifyemail(email) {
    var regexEL = /^(\w)+(\.\w+)*@(\w)+((\.\w{2,3}){1,3})$/
    return regexEL.test(email)
}
function verifyemailexist() {
    var email = $("#email").val()
    var emailaddress = {
        email : email
    }
    if (verifyemail(email) == false) {
        console.log(verifyemail(email))
        warning("email", "邮箱格式错误", false)
        $("#sendemail").removeAttr("onclick")
        $("#sendemail").attr("style", "cursor:not-allowed")
    }
    else {
        $.ajax({
            url: "/api/MembershipAPI/VerifyEmailExist/",
            type: "Post",
            dataType: "json",
            data: emailaddress,
            beforeSend: function () {
                $("#email").parent().parent().attr("class", "form-group has-warn")
                $("#email_tip").html("正在验证邮箱")
                $("#username_tip").show()
            },
            success: function (result) {
                if (result) {
                    warning("email", "电子邮箱不存在，注册账号？<a href='/membership/regist'>注册</a>", false)
                    $("#sendmail").removeAttr("onclick")
                    $("#sendmail").attr("style", "cursor:not-allowed")
                    return
                }
                else {
                    warning("email", "电子邮件正确", true)
                    $("#code").removeAttr("disabled")
                    $("#sendemail").removeAttr("style")
                    $("#sendemail").attr("onclick", "sendemail()")
                    ensure()
                    
                }
            }
        })
    }

}
function sendemail() {
    var emailaddress = {
        email: $("#email").val()
    }
    $.ajax({
        url: "/api/MembershipAPI/SendEmail/",
        type: "Post",
        dataType: "json",
        data: emailaddress,
        beforeSend: function () {
            $("#sendemail").text("正在发送")
            $("#sendemail").removeAttr("onclick")
            $("#sendemail").attr("style", "cursor:not-allowed")
        },
        success: function (result) {
            if (result.result == true) {
                alert("发送成功")
            }
            else {
                alert(result.error + "请检查网络")
            }
        },
        error: function () {
            alert("无法连接远程服务器")
        },
        complete: function (XMLHttpRequest, textStatus) {

            $("#sendemail").text("没有收到？再次发送")
            $("#sendemail").removeAttr("style")
            $("#sendemail").attr("onclick", "sendemail()")
        }
    })
}
//验证验证码
function verifycode() {
    var code = {
        token: $("#code").val()
    }
    if ($("#code").val() == "") {
        warning("code", "验证码不能为空", false)
        return
    }
    $.ajax({
        url: "/api/MembershipAPI/VerifyCode/",
        type: "Post",
        dataType: "json",
        data: code,
        success: function (result) {
            if (result == false) {
                warning("code", "验证码错误", false)
            }
            else {
                warning("code", "验证成功", true)
                ensure()
            }
        },
        error: function (textStatus) {
            console.log(textStatus.responseJSON)
        }
    })
}
//验证密码是否合法
function verifypassword(password) {
    var regexEL = /^[0-9A-Za-z]{6,}$/
    return regexEL.test(password)
}

//验证密码
function verifypasswordtext() {
    var password = $("#new_password").val()
    if (password == "") {
        warning("new_password", "密码不能为空", false)
        return
    }
    if (!verifypassword(password)) {
        warning("new_password", "密码至少6位数，支持英文大写、小写或数字", false)
        return
    }
    else {
        if ($("#new_password_again").val() != password) {
            warning("new_password_again", "两次输入的密码不相同", false)
        }
        else {
            warning("new_password_again", "密码正确", true)
        }
        warning("new_password", "密码可以使用", true)
        ensure()
    }
}
//验证再次输入密码
function verifypasswordagain() {
    var password = $("#new_password").val()
    var password_again = $("#new_password_again").val()
    if (password_again != "") {
        if (password != password_again) {
            warning("new_password_again", "两次输入的密码不相同", false)
        }
        else {
            warning("new_password_again", "密码正确", true)
            ensure()
        }
    }
    else {
        warning("new_password_again", "密码不能为空", false)
    }
}