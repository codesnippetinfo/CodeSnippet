//防止多次提交
$(document).on('invalid-form.validate', 'form', function () {
    var button = $(this).find('input[type="submit"]');
    setTimeout(function () {
        button.removeAttr('disabled');
    }, 1);
});

$(document).on('submit', 'form', function () {
    var button = $(this).find('input[type="submit"]');
    if (button[0].defaultValue == "本地上传") {
        //这个逻辑是有问题的，最佳做法是在插件里面完成重复提交控制
        //但是现在的问题是无法将代码注入到插件中！！！所以作为临时方法，将按钮文字为本地上传的按钮暂时不做禁用处理
        //这个时候页面应该是无法触摸的状态
        return;
    }
    setTimeout(function () {
        button.attr('disabled', 'disabled');
    }, 0);
});


localData = {
    hname: location.hostname ? location.hostname : 'localStatus',
    isLocalStorage: window.localStorage ? true : false,
    dataDom: null,

    initDom: function () { //初始化userData
        if (!this.dataDom) {
            try {
                this.dataDom = document.createElement('input');//这里使用hidden的input元素
                this.dataDom.type = 'hidden';
                this.dataDom.style.display = "none";
                this.dataDom.addBehavior('#default#userData');//这是userData的语法
                document.body.appendChild(this.dataDom);
                var exDate = new Date();
                exDate = exDate.getDate() + 30;
                this.dataDom.expires = exDate.toUTCString();//设定过期时间
            } catch (ex) {
                return false;
            }
        }
        return true;
    },
    set: function (key, value) {
        if (this.isLocalStorage) {
            window.localStorage.setItem(key, value);
        } else {
            if (this.initDom()) {
                this.dataDom.load(this.hname);
                this.dataDom.setAttribute(key, value);
                this.dataDom.save(this.hname)
            }
        }
    },
    get: function (key) {
        if (this.isLocalStorage) {
            return window.localStorage.getItem(key);
        } else {
            if (this.initDom()) {
                this.dataDom.load(this.hname);
                return this.dataDom.getAttribute(key);
            }
        }
    },
    remove: function (key) {
        if (this.isLocalStorage) {
            localStorage.removeItem(key);
        } else {
            if (this.initDom()) {
                this.dataDom.load(this.hname);
                this.dataDom.removeAttribute(key);
                this.dataDom.save(this.hname)
            }
        }
    }
}


//模态对话框
$(function () {
    // dom加载完毕
    var $mBtn = $('#modalBtn');
    var $modal = $('#myModal');
    $mBtn.on('click', function () {
        $modal.modal({ backdrop: 'static' });
    });

    // 测试 bootstrap 居中
    $modal.on('show.bs.modal', function () {
        var $this = $(this);
        var $modalDialog = $this.find('.modal-dialog');
        // 关键代码，如没将modal设置为 block，则$modala_dialog.height() 为零
        $this.css('display', 'block');
        $modalDialog.css({ 'margin-top': Math.max(0, ($(window).height() - $modalDialog.height()) / 2) });
    });
});

//将模态窗体的确定按钮变成一个执行GET的链接，
//链接按下之后会自动关闭该模态窗口
function OpenModal(href, title, message) {
    if (href == "") {
        $('#LinkRef')[0].style.display = "none";
    } else {
        $('#LinkRef')[0].href = href;
    }
    $('#DialogTitle')[0].innerText = title;
    $('#DialogMessage')[0].innerText = message;
    $('#myModal').modal('show');
    return false;
}

//收藏
function Stock(articleid) {
    $.get("/API/StockArticle.ashx",
    { ArticleID: articleid },
    function (data, textStatus) {
        OpenModal("", "收藏", data.message);
        if (data.success = 1) {
            document.getElementById("btnStock").innerText = " 已收藏";
        }
    }, "json");
}

//关注
function FocusUser(accountid) {
    $.get("/API/FocusUser.ashx",
    { AccountID: accountid },
    function (data, textStatus) {
        OpenModal("","关注用户",data.message);
        if (data.success = 1) {
            document.getElementById("btnFocus_" + accountid).innerText = " 已关注";
        }
    }, "json");
}


function FocusTopic(topicid) {
    $.get("/API/FocusTopic.ashx",
    { TopicID: topicid },
    function (data, textStatus) {
        OpenModal("","关注专题",data.message);
        if (data.success = 1) {
            document.getElementById("btnFocus_" + topicid).innerText = " 已关注";
        }
    }, "json");
}

//将文章加入专题
function Topic(articleid) {
    $.get("/API/PutToTopic.ashx",
    { ArticleID: articleid },
    function (data, textStatus) {
        OpenModal("","收录专题",data.message);
        if (data.success = 1) {
            document.getElementById("btnTopic").innerText = " 已收录";
        }
    }, "json");
}

//加入标签圈子
function Join(tagname) {
    $.get("/API/JoinCircle.ashx",
    { TagName: tagname },
    function (data, textStatus) {
        OpenModal("", "加入标签圈子", data.message);
        if (data.success = 1) {
            document.getElementById("btnCircle").innerText = " 已加入";
        }
    }, "json");
}
//读取站内短消息
function ReadMessage(msgid) {
    $.get("/API/ReadMessage.ashx",
    { MessageId: msgid },
    function (data, textStatus) {
        OpenModal("", "消息", data.message);
        if (data.success = 1) {
            document.getElementById("btnMsg_" + msgid).innerText = " 已读";
            document.getElementById("btnMsg_" + msgid).disabled = true;
        }
    }, "json");
}