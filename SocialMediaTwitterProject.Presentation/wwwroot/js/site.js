


function Follow(isExist) {
    var model = {
        FollowerId: parseInt($("#FollowerId").val()),
        FollowingId: parseInt($("#FollowingId").val()),
        isExist: isExist
    };

    $.ajax({
        data: { FollowerId: model.FollowerId, FollowingId: model.FollowingId, isExist: model.isExist },
        type: "POST",
        url: "/Follow/Follow/",
        dataType: "JSON",
        success: function (result) {
            if (result == "Success") {
                if (!isExist) {
                    $("#Follow").replaceWith('<button onclick="Follow(true)" id="UnFollow" class="btn btn-info btn-sm">Unfollow</button>');
                    FollowersCount = FollowersCount + 1;
                    $("#FollowersCount").replaceWith('<li id="FollowersCount"><strong>' + FollowersCount + '</strong> - Followers</li>')
                }
                else {
                    $("#UnFollow").replaceWith('<button onclick="Follow(false)" id="Follow" class="btn btn-info btn-sm">Follow</button>');
                    FollowersCount = FollowersCount - 1;
                    $("#FollowersCount").replaceWith('<li id="FollowersCount"><strong>' + FollowersCount + '</strong> - Followers</li>')
                }
            }
        }
    });
}

$(document).ready(function () {

    $("#btnSendTweet").click(function (e) {
        var formData = new FormData();
        formData.append("AppUserId", JSON.stringify(parseInt($("#AppUserId").val())));
        formData.append("Text", $("#Text").val());
        //formData.append("Image", $("#Image").val());
        $.ajax({
            data: formData,
            contentType: false,
            processData: false,
            type: "POST",
            url: "/Tweet/AddTweet/",
            success: function (result) {
                if (result == "Success") {
                    document.getElementById("Text").value = "";
                    $("#tweetValidation").addClass("alert alert-success").text("Send Successfully..!");
                    $("#tweetValidation").alert();
                    $("#tweetValidation").fadeOut(2000, 2000).slideDown(800, function () { });
                }
                else {
                    $("#tweetValidation").addClass("alert alert-danger").text("Error Occured..!");
                    $("#tweetValidation").alert();
                    $("#tweetValidation").fadeOut(2000, 2000).slideDown(800, function () { });
                }
            }
            //error: function (result) {
            //    $("#tweetValidation").addClass("alert alert-success").text(result.responseText);
            //    $("#tweetValidation").alert();
            //    $("#tweetValidation").fadeOut(2000, 2000).slideDown(800, function () { });
            //}
        });
    });
});

function loadTweetList(pageIndex, userName) {
    $.ajax({
        url: "/Tweet/GetTweets",
        type: "POST",
        async: true,
        dataType: "Json",
        data: { pageIndex: pageIndex, userName: userName },
        success: function (result) {
            var html = "";
            if (result == "Success") {
                $.each(result, function (key, item) {
                    if (item.ImagePath == null) {
                        html += '<li id="tweet_' + item.Id + '"><img src="' + item.UserImage + '"><a href="/profile/' + item.UserName + '">' + item.Name + '</a></br><p class="text-dark">' + item.Text + '</p></li>';
                        console.log(html);
                    }
                    else {
                        html += '<li id="tweet_' + item.Id + '"><img src="' + item.UserImage + '"><a href="/profile/' + item.UserName + '">' + item.Name + '</a></br><p>' + item.Text + '</p><img src="' + item.ImagePath + '"</li>';
                        console.log(html);
                    }
                });
                if (pageIndex == 1) {
                    $("#TweetList").html(html);
                }
                else {
                    $("#TweetList").append(html);
                }
            }

        },
        error: function (result) {
            console.log(result);
            $("#tweetValidation").addClass("alert alert-danger").text(result.responseText);
            $("#tweetValidation").alert();
            $("#tweetValidation").fadeTo(3000, 3000).slideUp(2000, function () {
            });
        }
    });

}

function keypress(e) {
    if (e.keyCode === 13) {
        document.getElementById('searchform').submit()
    }
}