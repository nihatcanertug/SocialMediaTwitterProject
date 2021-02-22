
$(document).ready(function () {
	var pageIndex = 1;
	if (typeof userName === 'undefined') {
		userName = null;
	}
	console.log(userName)
	loadTweetList(pageIndex, userName);
	$(window).scroll(function () {
		if ($(window).scrollTop() + $(window).height() > $(document).height() - 50) {
			pageIndex = pageIndex + 1;
			loadTweetList(pageIndex, userName);
		}
	});
})


function loadTweetList(pageIndex, userName) {
	$.ajax({
		url: "/Tweet/GetTweets",
		type: "POST",
		async: true,
		dataType: "Json",
		data: { pageIndex: pageIndex, userName: userName },
		success: function (result) {
			var html = "";
			if (result.length != 0) {
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
			else {
				$(window).unbind('scroll');
			}

		},
		error: function (errormessage) {
			$('#TweetsList').html('<li><p class="text-center">There were no results found</p></li>');
			$(window).unbind('scroll');
		}
	});
}