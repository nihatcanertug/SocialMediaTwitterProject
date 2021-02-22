$(document).ready(function () {
    var pageIndex = 1;
    loadUserResults(pageIndex, userName, controllerName, actionName);

    $(window).scroll(function () {
        if ($(window).scrollTop() + $(window).height() > $(document).height() - 50) {
            pageIndex = pageIndex + 1;
            loadUserResults(pageIndex, userName, controllerName, actionName);
        }
    });
});

function loadUserResults(pageIndex, userName, controllerName, actionName) {
    $.ajax({
        url: "/" + controllerName + "/" + actionName,
        type: "POST",
        async: true,
        dataType: "JSON",
        data: { userName: userName, pageIndex: pageIndex },
        success: function (result) {
            console.log(result);
            var html = "";
            if (result.length != 0) {
                $.each(result, function (key, item) {
                    html += '<li class="list-group-item"><img src="' + item.ImagePath + '" alt="" width="25" height="25"><a href="/profile/' + item.UserName + '">' + item.UserName + '</li>'
                });
                if (pageIndex == 1) {
                    $('#UserResult').html(html);
                }
                else {
                    $('#UserResult').append(html);
                }
            }
            else {
                $(window).unbind('scroll');
            }
        },
        error: function (errormessage) {
            $('#UsersResult').html('<li><p class="text-center">There were no results found</p></li>');
            $(window).unbind('scroll');
        }
    });
}