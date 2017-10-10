$(function () {    //页面初始化      Page initialization
    $.ajax({
        type: "POST",
        url: "CheckLogin.ashx",
        data: {
            "Check": "ListIndex",            
        },
        success: function (result) {
            if (result == "error") {
                window.location = "Login.html";
            } else {
                $("#domainuser").text = result;
                listtreeview();     //加载主页菜单树   Load home menu treeview
            } 
        }
    });
    path = "";      //置空原OU路径   Empty the original OU path
    newpath = "";   //置空新OU路径   Empty the new OU path    
})


//全局变量    Global variable
var username = "";              //原用户名     Original user name
var groupname = "";             //原组名      Original group name
var ouname = "";                //原OU名      Original organization units name
var path = "";                  //OU=***,拼接路径       Splicing path
var newpath = "";           //移动对象时选择的新路径   New path selected when moving objects
var oldobject = "";             //移动的对象名称    The name of object which you want to move
var moveurl = "";               //移动动作请求Url         Request Url of move
var deleteflag = "";          //删除动作请求Url           Request Url of delete



//添加用户按钮事件      Add user
function AddUser() {
    addtree();
}

//添加OU按钮事件  Add  OU
function AddOU() {
    addoutree();
}

//添加组按钮事件  Add group
function AddGP() {
    addgptree();
}



//刷新按钮事件   Refresh
function Refresh() {
    listuser(path);
    listtreeview();
}

//修改按钮绑定信息事件        Update button  information binding
function Update(name, type) {
    if (type == "user") {
        //用户信息绑定     User information binding
        $("#myModal").modal('show');        
        username = name;
        $.ajax({
            type: "POST",
            url: "/UserOperate/GetUserInfo.ashx",
            data: {
                "name": name,
                "url": path
            },
            success: function (result) {
                var obj = jQuery.parseJSON(result);
                //$("#newname").val(username);          //预留重命名用户功能（获取新用户名）     Reserved rename user function(get new user name)
                $("#lastname").val(obj.lastName);
                $("#firstname").val(obj.firstName);
                $("#displayname").val(obj.displayName);
                $("#loginname").val(obj.sAMAccountname);
                $("#description").val(obj.Description);
                $("#department").val(obj.Department);
                $("#telphone").val(obj.Telephone);
            }
        });
    } else if (type == "group") {
        //组信息绑定   Group information bingding
        $("#renameGPModal").modal('show');
        groupname = name;
        $("#gpname").val(groupname);
        
    }
    else if (type == "organizationalUnit") {
        //OU信息绑定 OU information binding
        $("#renameOUModal").modal('show');
        ouname = name;
        $("#ouname").val(ouname);
        
    }
    else {        
    }
}


//提交修改后的用户信息   Submit update user information 
function ChangeUserInfo() {
    $.ajax({
        type: "Post",
        async: false,
        url: "/UserOperate/ChangeInfo.ashx",
        data: {
            //"newname":$("#newname").val(),            //预留重命名用户功能（提交新用户名）         Reserved rename user function(submit new user name)
            "lastname": $("#lastname").val(),
            "firstname": $("#firstname").val(),
            "displayname": $("#displayname").val(),
            "loginname": $("#loginname").val(),
            "description": $("#description").val(),
            "department": $("#department").val(),
            "telphone": $("#telphone").val(),
            "olduser": username,
            "url": path
        },
        success: function (data) {
            alert(data);
            $("#myModal").modal('hide');
            Refresh();
        }
    });
}


//删除所选行对应的对象   Delete object of your selected
function Delete(name, type) {
    if (type == "group") {
        deleteflag = "/GroupOperate/DeleteGP.ashx";                    //删除组url     Delete group url
    }
    else if (type == "user") {
        deleteflag = "/UserOperate/DeleteUser.ashx";                   //删除用户url  Delete user url
    }
    else if (type == "organizationalUnit") {
        deleteflag = "/OUOperate/DeleteOU.ashx";                    //删除OUurl   Delete OU url
    } else { }
    if (confirm("Confirm Delete？")) {
        $.ajax({
            type: "POST",
            url: deleteflag,
            data: {
                "name": name,
                "url": path
            },
            success: function (result) {
                alert(result);
                Refresh();
            }
        });
        Refresh();
    } else {

    }
}

//添加用户提交按钮事件    Add user 
function adduser() {
    var lastname_add = $("#lastname_add").val();
    var firstname_add = $("#firstname_add").val();
    var loginname_add = $("#loginname_add").val();
    if (lastname_add == "" && firstname_add == "" && loginname_add == "") {
        alert("Firstname,Lastname,Loginname can not be empty!");
    }
    else {
        $.ajax({
            type: "Post",
            url: "/UserOperate/AddUser.ashx",         //添加用户url         Add user url
            async: false,
            data: {
                "lastname_add": $("#lastname_add").val(),
                "firstname_add": $("#firstname_add").val(),
                "displayname_add": $("#displayname_add").val(),
                "loginname_add": $("#loginname_add").val(),
                "description_add": $("#description_add").val(),
                "department_add": $("#department_add").val(),
                "telphone_add": $("#telphone_add").val(),
                "password_add": $("#password_add").val(),
                "confirmpwd_add": $("#confirmpwd_add").val(),
                "url": path
            },
            success: function (data) {
                alert(data);
                $("#addModal").modal('hide');
                Refresh();
            }
        });
    }
}

//添加OU提交按钮事件    Add OU
function addOU() {
    var ouname_add = $("#ouname_add").val();
    if (ouname_add == "") {
        alert("OU name can not be empty！");
    }
    else {
        $.ajax({
            type: "Post",
            url: "/OUOperate/AddOU.ashx",        //添加OU url         Add OU url
            async: false,
            data: {
                "ouname_add": $("#ouname_add").val(),
                "url": path
            },
            success: function (data) {
                alert(data);
                $("#addOUModal").modal('hide');
                Refresh();
            }
        });
    }
}

//添加组提交按钮事件     Add group
function addGP() {
    var gpname_add = $("#gpname_add").val();
    if (gpname_add == "") {
        alert("Group name can not be empty！");
    }
    else {
        $.ajax({
            type: "Post",
            url: "/GroupOperate/AddGroup.ashx",         //添加组url    Add group url
            async: false,
            data: {
                "gpname_add": $("#gpname_add").val(),
                "url": path
            },
            success: function (data) {
                alert(data);
                $("#addGPModal").modal('hide');
                Refresh();
            }
        });
    }
}



//主页列表展示            Show main page list 
function listuser(url) {

    $("#listbody").html("");
    $.ajax({
        type: "POST",
        url: "/UserOperate/GetAllUser.ashx",          //获取列表数据url       Get list data
        data: { "url": url },
        success: function (table) {
            if (table != "") {
                $("#listbody").html(table);
            }
            else {
                $("#listbody").html("Current domain have no information!");
            }
        }
    });
}


//添加用户treeview数据绑定      Add user TreeView information binding
function addtree() {    
    var para = "";
    
    //请求treeview菜单数据        Request treeview menu data
    $.ajax({
        type: "Post",
        url: "GetTreeView.ashx",        //获取树列表内容url       Get tree list contents url
        data: { "name": para },
        success: function (data) {
            var json = eval(data);
            $("#tree").html("");        //清空树列表         Clear tree list
            $("#tree").treeview({
                animated: "fast",
                collapsed: true,
                color: "#428bca",
                data: json,
            });
            //绑定treeview点击选择动作        Bind treeview  click and select action
            $("#tree").on('nodeSelected', function (event, data) {
                path = "";
                if (data["nodeId"] == '0') {
                    path = "";
                } else {
                    addpath(data["text"]);
                    getpath(data["nodeId"]);
                }
            });
        },
        error: function (err) {
            $("#tree").html("Can not get information！");
        }
    });
}

//添加OU树绑定           Add OU TreeView information binding
function addoutree() {
    var para = "";
    
    $.ajax({
        type: "Post",
        url: "GetTreeView.ashx",         //获取树列表内容url       Get tree list contents url
        data: { "name": para },
        success: function (data) {
            var json = eval(data);
            $("#OUtree").html("");
            $("#OUtree").treeview({
                animated: "fast",
                collapsed: true,
                color: "#428bca",
                data: json,
            });
            //绑定treeview点击选择动作        Bind treeview  click and select action
            $("#OUtree").on('nodeSelected', function (event, data) {
                path = "";
                if (data["nodeId"] == '0') {
                    path = "";
                } else {
                    addpath(data["text"]);
                    getpath(data["nodeId"]);
                }
            });
        },
        error: function (err) {
            $("#OUtree").html("Can not get information！");
        }
    });
}

//添加组树绑定            Add group TreeView information binding
function addgptree() {
    var para = "";
    
    $.ajax({
        type: "Post",
        url: "GetTreeView.ashx",    //获取树列表内容url       Get tree list contents url
        data: { "name": para },
        success: function (data) {
            var json = eval(data);
            $("#GPtree").html("");
            $("#GPtree").treeview({
                animated: "fast",
                collapsed: true,
                color: "#428bca",
                data: json,
            });
            //绑定treeview点击选择动作        Bind treeview  click and select action
            $("#GPtree").on('nodeSelected', function (event, data) {
                path = "";
                if (data["nodeId"] == '0') {
                    path = "";
                } else {
                    addpath(data["text"]);
                    getpath(data["nodeId"]);
                }
            });
        },
        error: function (err) {
            $("#GPtree").html("Can not get information！");
        }
    });
}

//移动对象treeview数据绑定          Move object TreeView information binding
function addmovetree() {

    var para = "";
    
    $.ajax({
        type: "Post",
        url: "GetTreeView.ashx",        //获取树列表内容 url      Get tree list contents url
        data: { "name": para },
        success: function (data) {
            var json = eval(data);
            $("#movetree").html("");
            $("#movetree").treeview({
                animated: "fast",
                collapsed: true,
                color: "#428bca",
                data: json,
            });
            //绑定treeview点击选择动作        Bind treeview  click and select action
            $("#movetree").on('nodeSelected', function (event, data) {
                newpath = "";
                if (data["nodeId"] == '0') {
                    newpath = "";
                } else {
                    addnewpath(data["text"]);
                    getnewpath(data["nodeId"]);
                }
            });
        },
        error: function (err) {
            $("#movetree").html("Can not get information！");
        }
    });
}


//主页树绑定         Main page TreeView information binding
function listtreeview() { 
    var para = "";
    
    $.ajax({
        type: "Post",
        url: "GetTreeView.ashx",            //获取树列表内容url       Get tree list contents url
        data: { "name": para },
        success: function (data) {
            var json = eval(data);
            $("#treeview").html("");
            $("#treeview").treeview({
                animated: "fast",
                collapsed: true,
                color: "#428bca",
                data: json
            });
            //绑定treeview点击选择动作        Bind treeview  click and select action
            $("#treeview").on('nodeSelected', function (event, data) {
                path = "";
                if (data["nodeId"] == '0') {
                    listuser(path);
                } else {
                    addpath(data["text"]);
                    getpath(data["nodeId"]);
                }
                listuser(path);
            });
        },
        error: function (err) {
            $("#treeview").html("Can not get information！");
        }
    });
}





//账户禁用和启用           Account disabled and enabled
function Check(name) {
    
    if (confirm("Confirm to do this？")) {
        $.ajax({
            type: "POST",
            url: "/UserOperate/ChangeState.ashx",           //账户禁用和启用url           Account disabled and enabled url
            data: {
                "name": name,
                "url": path
            },
            success: function (result) {
                alert(result);
                Refresh();
            }
        });
        Refresh();
    } else {

    }
}

//递归查找OU路径   Select path of OU 
function getpath(node) {
    var parent = $('#treeview').treeview('getParent', node);    
    if (parent["nodeId"] != '0') {        
        addpath(parent["text"]);
        getpath(parent["nodeId"]);
    }
    else {
    }
}
//构造路径字符串           Create path string
function addpath(nodepath) {
    path += "OU=" + nodepath + ",";
}

//递归查找新路径           Select new path
function getnewpath(node) {
    var parent = $('#movetree').treeview('getParent', node);    
    if (parent["nodeId"] != '0') {       
        addnewpath(parent["text"]);
        getnewpath(parent["nodeId"]);
    }
    else {
    }
}
//构造新路径字符串      Create new path string 
function addnewpath(nodepath) {
    newpath += "OU=" + nodepath + ",";
}

//扩展功能重命名和删除组和组织单位     Rename and Delete group or organization units

//重命名组          Rename Group
function renameGP() {
    if ($("#gpname").val() != "") {
        $.ajax({
            type: "Post",
            async: false,
            url: "/GroupOperate/RenameGP.ashx",                   //重命名组url           Rename group url
            data: {
                "gpname": $("#gpname").val(),
                "oldgroup": groupname,
                "url": path
            },
            success: function (data) {
                alert(data);
                $("#renameGPModal").modal('hide');
                Refresh();
            }
        });
    }
    else {
        alert("Group name can not be empty！");
    }

}


//重命名OU   Rename OU 
function renameOU() {
    if ($("#ouname").val() != "") {
        $.ajax({
            type: "Post",
            async: false,
            url: "/OUOperate/RenameOU.ashx",               //重命名OU名称url   Rename OU name url
            data: {
                "ouname": $("#ouname").val(),
                "oldou": ouname,
                "url": path
            },
            success: function (data) {
                alert(data);
                $("#renameOUModal").modal('hide');
                Refresh();
            }
        });
    }
    else {
        alert("OU name can not be empty！");
    }
}


//根据请求类型重置请求url         Reset request URL according to request type
function Moveto(name, type) {    
    if (type == "user" || type == "group") {
        $("#movetoModal").modal('show');
        oldobject = name;
        addmovetree();
        moveurl = "/UserOperate/MoveObjTo.ashx";        //移动用户和组url     Move user and group url
    }
    else if (type == "organizationalUnit") {
        $("#movetoModal").modal('show');
        oldobject = name;
        addmovetree();
        moveurl = "/OUOperate/MoveOUTo.ashx";           //移动OU　url      Move OU url
    }
    else {

    }
}

//移动请求动作            Move object
function movetosubmit() {
    
    if (path != newpath) {       
            $.ajax({
                type: "Post",
                async: false,
                url: moveurl,               //移动对象url,由Moveto(name,type)函数提供           Move object url，provided by the Moveto(name, type) function
                data: {
                    "oldurl": path,
                    "newurl": newpath,
                    "name": oldobject
                },
                success: function (data) {
                    alert(data);
                    $("#movetoModal").modal('hide');
                    Refresh();
                }
            });
    }
}