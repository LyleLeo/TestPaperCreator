$(document).ready(function () {
    var regist_flag = false
    //用户名输入框设置焦点
    $("#username").focus()
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
        $("#" + warnname).parent().parent().attr("class", "form-group has-error")
    }
    $("#" + warnname + "_tip").html(alert)
    $("#" + warnname + "_tip").show()
}

//验证密码是否合法
function verifypassword(password) {
    var regexEL = /^[0-9A-Za-z]{6,}$/
    return regexEL.test(password)
}

//验证邮箱格式是否合法
function verifyemail(email) {
    var regexEL = /^(\w)+(\.\w+)*@(\w)+((\.\w{2,3}){1,3})$/
    return regexEL.test(email)
}

//验证用户名是否合法
function verifyusername(username) {
    var regexEL = /^[\u4E00-\u9FA5\uF900-\uFA2D\da-zA-Z]+$/
    return regexEL.test(username)
}

//验证用户是否存在
function verifyusernameexist() {
    var username = $("#username").val() //用户名
    //检测用户名是否为空
    if (username == "") {
        warning("username", "请填写用户名", false)
        return
    }
    //检测用户名是否合法
    if (!verifyusername(username)) {
        warning("username", "用户名只能是汉字、字母或数字", false)
        return
    }
    //检测用户名是否存在
    var user = {
        username: $("#username").val()
    }
    $.ajax({
        url: "/api/MembershipAPI/VerifyUsernameExist/",
        type: "Post",
        dataType: "json",
        data: user,
        beforeSend: function () {
            $("#username").parent().parent().attr("class", "form-group has-success")
            $("#username_tip").html("正在验证用户名")
            $("#username_tip").show()
        },
        success: function (result) {
            //用户名已存在
            if (result == false) {
                warning("username", "用户名已存在，已有账户？<a href='/membership/login/'>登录</a>或<a href='#'>忘记密码？</a>", false)
                return
            }
            else {
                warning("username", "用户名可以使用", true)
                ensure()
            }
        }
    })

}

//验证密码
function verifypasswordtext() {
    var password = $("#password").val()
    if (password == "") {
        warning("password", "密码不能为空", false)
        return
    }
    if (!verifypassword(password)) {
        warning("password", "密码至少6位数，支持英文大写、小写或数字", false)
        return
    }
    else {
        if ($("#password_again").val() != password) {
            warning("password_again", "两次输入的密码不相同", false)
        }
        else {
            warning("password_again", "密码正确", true)
        }
        warning("password", "密码可以使用", true)
        ensure()
    }
}

//验证邮箱是否合法
function verityemailaddress() {
    var email = $("#email").val()
    if (email == "") {
        warning("email", "电子邮箱不能为空", false)
        return
    }
    if (!verifyemail(email)) {
        warning("email", "电子邮件格式不正确", false)
        return
    }
    var email = $("#email").val()
    var emailaddress = {
        email: email,
    }
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
                warning("email", "电子邮箱可以使用", true)
                $("#code").removeAttr("disabled")
                $("#sendemail").removeAttr("style")
                $("#sendemail").attr("onclick", "sendemail()")
                ensure()
            }
            else {
                warning("email", "电子邮箱已存在，已注册账号？<a href='/membership/login'>登录</a>或<a href='#'>忘记密码？</a>", false)
                $("#sendmail").removeAttr("onclick")
                $("#sendmail").attr("style", "cursor:not-allowed")
                return
            }
        }
    })
}

//验证再次输入密码
function verifypasswordagain() {
    var password = $("#password").val()
    var password_again = $("#password_again").val()
    if (password_again != "") {
        if (password != password_again) {
            warning("password_again", "两次输入的密码不相同", false)
        }
        else {
            warning("password_again", "密码正确", true)
            ensure()
        }
    }
   else{
        warning("password_again", "密码不能为空", false)
        }
}

//发送邮件
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
                alert(result.error+"请检查网络")
            }
        },
        error: function(){
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

//注册
function regist() {
    var logintype = 0
    var rememberme = 1
    var user = {
        username: $("#username").val(),
        password: hex_md5($("#password").val()),
        logintype: logintype,
        rememberme: rememberme,
        email: $("#email").val()
    }
    $.ajax({
        url: "/api/MembershipAPI/Register",
        type: "post",
        dataType: "json",
        data: user,
        beforeSend: function () {
            $("#submit").val("正在注册")
            $("#submit").attr("disabled", "disabled")
        },
        success: function (result) {
            if (result.result == true) {
                $.ajax({
                    url: "/api/MembershipAPI/Login/",
                    type: "POST",
                    dataType: "JSON",
                    data: user,
                    beforeSend: function () {
                        $("#submit").val("正在登录中")
                        $("#submit").attr("disabled", "disabled")
                    },
                    success: function (data) {
                        if (data == true) {
                            window.location.href = "/Home/Index/"
                        }
                    }
                })
            }
            else {
                alert("注册失败，"+result.error)
            }
        }
    })
}