
    var ReplyComment = new function () {
        this.show = function (replyId) {
            var commentReply = document.getElementById(replyId);
            commentReply.style.display = "";
        }
        this.close = function (replyId) {
            var commentReply = document.getElementById(replyId);
            commentReply.style.display = "none";
        }
    };

    //SignalR connection
    var connectionC = new signalR.HubConnectionBuilder().withUrl("/commentsHub").build();

    //post new comment
    connectionC.on("PostNewComment", function (commentfor, commentForID, commentDateAdded, commentContent, commentId,
        commentCreatedBy, userFirstName, userLastName, userProfilePhoto) {
        if (commentfor == commentFor && commentForID == commentForId) {
            var profilePhoto = (userProfilePhoto == null) ?
                `<img src="/defaultAvatar.png" class="img-circle m-b" alt="logo" />` :
                `<img src="/UsersData/` + userProfilePhoto + `" class="img-circle m-b" alt="logo" />`;
            var currentUserPhoto = document.getElementById('current-user-photo').value;
            var currentUserProfilePhoto = (currentUserPhoto == null) ?
                `<img src="/defaultAvatar.png" class="img-circle m-b" alt="logo" />` :
                `<img src="/UsersData/` + currentUserPhoto + `" class="img-circle m-b" alt="logo" />`;
            $('#all-comments').append(`
            <div class="content p-xxs m-l-lg m-r-lg" id="` + commentId + `">
                <div class="row">
                    <div class="col-lg-12">
                        <div class="hpanel forum-box m-b-n-sm">
                            <div class="panel-body">
                                <div class="media">
                                    <div class="media-image pull-left">` + profilePhoto + `</div>
                                    <div class="media-body">
                                        <a href="/User/UserProfile/` + currentUser + `?userId=` + commentCreatedBy + `" style="font-size: 15px">
                                            <span class="font-bold">` + userFirstName + ` ` + userLastName + `</span>
                                        </a>
                                        <div id="remove-for-currentUser-` + commentId + `" class="dropdown pull-right"></div>                    
                                        <br />
                                        <span>Just now</span>
                                        <br /><br />
                                        <strong>` + commentContent + `</strong>
                                        <br /><br />

                                        <!--like and dislike btn-->
                                        <span class="btn fa fa-thumbs-up m-l-n m-r-n" style="font-weight:300; color: #777;"
                                             id="comment-likes-count-` + commentId + `"
                                             onclick="AddRemoveCommentLikeDislike.like('` + commentId + `', 'comment-likes-count-` + commentId + `', 'comment-dislikes-count-` + commentId + `');">
                                          0
                                        </span>
                                        <span>&emsp;</span>
                                        <span class="btn fa fa-thumbs-down m-l-n m-r-n" style="font-weight:300; color: #777;"
                                            id="comment-dislikes-count-` + commentId + `"
                                            onclick="AddRemoveCommentLikeDislike.dislike('` + commentId + `', 'comment-likes-count-` + commentId + `', 'comment-dislikes-count-` + commentId + `');">
                                          0
                                        </span>
                                        <span>&emsp;</span>
                                        <a class="text-muted" onclick="ReplyComment.show('commentReply`+ commentId + `')">Reply</a>
                                    </div>
                                    
                                    <!--Subcomment-->
                                    <div id="subocmm` + commentId + `"></div>

                                    <!--Comment reply-->
                                    <div class="forum-comments" id="commentReply`+ commentId + `" style="display: none">
                                        <div class="media">
                                            <div class="media-image pull-left">` + currentUserProfilePhoto + `</div>
                                            <div class="media-body">
                                                <!--Close reply-->
                                                <a onclick="ReplyComment.close('commentReply`+ commentId + `');"
                                                        class="pull-right">
                                                    <span style="font-size: 20px"
                                                        class="text-muted pe-7s-angle-up"></span>
                                                </a>
                                                <br />
                                                <div class="social-content">
                                                    <input id="parent-commentId" value="` + commentId + `" hidden />
                                                    <div class="m-r-sm m-b-sm">
                                                        <textarea class="form-control form-input form-group" placeholder="Add a comment..."
                                                            id="subcomment-content` + commentId + `" style="width: 100%;"
                                                            rows="3"></textarea>
                                                        <button class="btn btn-primary m-b-sm m-l-sm"
                                                                onclick="newSubcomment.add('subcomment-content` + commentId + `', '` + commentId + `');">
                                                            <span class="fa fa-paper-plane"></span>
                                                        </button>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>`);

            if (currentUser == commentCreatedBy) {
                var div = "#remove-for-currentUser-" + commentId;
                $(div).append(`
                <a class="dropdown-toggle" href="#" data-toggle="dropdown">
                    <i class="pe-7s-more"></i>
                </a>
                <div class="dropdown-menu hdropdown bigmenu animated fadeInDown"
                  style="padding: 10px 10px 0 10px;">
                    <table><tbody><tr><td>
                        <a onclick="DeleteComment.delete('` + commentId + `');">
                            <i class="pe pe-7s-trash text-error"></i>
                            Remove
                        </a>
                    </td></tr></tbody></table>
                </div>
                `);
            }
        }
    });

    //post new subcomment
    connectionC.on("PostNewSubComment", function (commentfor, commentForID, commentDateAdded, commentContent,
        parentCommentId, commentCreatedBy, userFirstName, userLastName, userProfilePhoto, subcommentId) {
        if (commentfor == commentFor && commentForID == commentForId) {
            var profilePhoto = (userProfilePhoto == null) ?
                `<img src="/defaultAvatar.png" class="img-circle m-b" alt="logo" />` :
                `<img src="/UsersData/` + userProfilePhoto + `" class="img-circle m-b" alt="logo" />`;
            var subcomdiv = "#subocmm" + parentCommentId;
            console.log(subcomdiv);
            $(subcomdiv).append(`
            <div class="forum-comments m-t-n m-b-n" id="` + subcommentId + `">
                <div class="media">
                    <div class="pull-left">` + profilePhoto + `</div>
                    <div class="media-body">
                        <a href="/User/UserProfile/` + currentUser + `?userId=` + commentCreatedBy + `" style="font-size: 15px">
                            <span class="font-bold">` + userFirstName + ` ` + userLastName + `</span>
                        </a>
                        <div id="remove-for-currentUser-` + subcommentId + `" class="dropdown pull-right"></div>
                        <br />
                        <span>Just now</span>
                        <br /><br />
                        <strong>` + commentContent + `</strong>
                        <br /><br />

                        <!--like and dislike btn-->
                        <span class="btn fa fa-thumbs-up m-l-n m-r-n" style="font-weight:300; color: #777;"
                              id="subcomment-likes-count-` + subcommentId + `"
                              onclick="AddRemoveCommentLikeDislike.like('` + subcommentId + `', 'subcomment-likes-count-` + subcommentId + `', 'subcomment-dislikes-count-` + subcommentId + `');">
                          0
                        </span>
                        <span>&emsp;</span>
                        <span class="btn fa fa-thumbs-down m-l-n m-r-n" style="font-weight:300; color: #777;"
                              id="subcomment-dislikes-count-` + subcommentId + `"
                              onclick="AddRemoveCommentLikeDislike.dislike('` + subcommentId + `', 'subcomment-likes-count-` + subcommentId + `', 'subcomment-dislikes-count-` + subcommentId + `');">
                          0
                        </span>
                        <span>&emsp;</span>
                        <a class="text-muted" onclick="ReplyComment.show('commentReply` + parentCommentId + `')">Reply</a>
                    </div>
                </div>
            </div>`);

            if (currentUser == commentCreatedBy) {
                var div = "#remove-for-currentUser-" + subcommentId;
                $(div).append(`
                <a class="dropdown-toggle" href="#" data-toggle="dropdown">
                   <i class="pe-7s-more"></i>
                </a>
                <div class="dropdown-menu hdropdown bigmenu animated fadeInDown"
                         style="padding: 10px 10px 0 10px;">
                   <table><tbody><tr><td>
                       <a onclick="DeleteComment.delete('` + subcommentId + `');">
                          <i class="pe pe-7s-trash text-error"></i>
                          Remove
                       </a>
                   </td></tr></tbody></table>
                </div>
                `);
            }
        }
    });

    //post comment likes and dislikes
    connectionC.on("InitialLikesDislikes", function (cfor, cforId, userLikeInd,
        userDislikeInd, totalLikes, totalDislikes, likeSpan, dislikeSpan) {
        if (cfor == commentFor && cforId == commentForId) {
            var lspan = document.getElementById(likeSpan);
            var dspan = document.getElementById(dislikeSpan);
            lspan.style.color = (userLikeInd) ? "#62cb31" : "#777";
            dspan.style.color = (userDislikeInd) ? "#e74c3c" : "#777";

            var span = "#" + likeSpan;
            $(span).html(totalLikes);
            span = "#" + dislikeSpan;
            $(span).html(totalDislikes);
        }
    });

    //remove comment
    connectionC.on("CommentRemove", function (cId, cfor, cforId) {
        if (cfor == commentFor && cforId == commentForId) {
            var div = "#" + cId;
            $(div).remove();
        }
    });

    //start connection
    connectionC.start().then(function () {
        console.log("Connected");
        initialLikesDislikesCount.run();
    }).catch(function (err) {
        return console.error(err.toString());
    });

    //add new comment
    document.getElementById('comment-add').addEventListener("click", function (event) {
        console.log(currentUser, commentFor, commentForId,
            commentContent);
        var commentContent = document.getElementById('comment-content').value;
        $('#comment-content').val("");
        connectionC.invoke("NewComment", currentUser, commentFor, commentForId,
            commentContent).catch(function (err) {
                return console.error(err.toString());
            });
        event.preventDefault();
    });

    //add new subcomment
    var newSubcomment = new function () {
        this.add = function (subcommContent, parentCommentId) {
            var content = document.getElementById(subcommContent).value;
            var contentId = "#" + subcommContent;
            $(contentId).val("");
            connectionC.invoke("NewSubComment", currentUser, commentFor, commentForId,
                parentCommentId, content).catch(function (err) {
                    return console.error(err.toString());
                });
        };
    };

    //comment likes and dislikes
    //initial
    var CommentLikesDislikes = new function () {
        this.initial = function (commentId, likeSpanId, dislikeSpanId) {
            connectionC.invoke("CommentLikesDislikesInitial", currentUser, commentId,
                likeSpanId, dislikeSpanId).catch(function (err) {
                    return console.error(err.toString());
                });
        };
    };

    //run initial likes and dislikes cont
    var initialLikesDislikesCount = new function () {
        this.run = function () {
            var totalComments = document.getElementById('total-comments').value;
            for (var i = 0; i < totalComments; i++) {
                var comId = "current-commentId-" + i;
                var currentCommentId = document.getElementById(comId).value;
                var comLikeSpan = "comment-likes-count-" + currentCommentId;
                var comDislikeSpan = "comment-dislikes-count-" + currentCommentId;
                CommentLikesDislikes.initial(currentCommentId, comLikeSpan, comDislikeSpan);

                var tSubcome = "total-subcoments-" + i;
                var totalSubcoments = document.getElementById(tSubcome).value;
                for (var j = 0; j < totalSubcoments; j++) {
                    var subcomId = "current-subcommentId-" + i + "-" + j;
                    var currentSubcommentId = document.getElementById(subcomId).value;
                    var subcomLikeSpan = "subcomment-likes-count-" + currentSubcommentId;
                    var subcomDislikeSpan = "subcomment-dislikes-count-" + currentSubcommentId;
                    CommentLikesDislikes.initial(currentSubcommentId, subcomLikeSpan, subcomDislikeSpan);
                }
            }
        };
    };

    //add/remove comment like and dislike
    var AddRemoveCommentLikeDislike = new function () {
        this.like = function (commentId, likeSpanId, dislikeSpanId) {
            connectionC.invoke("CommentLike", currentUser, commentId,
                likeSpanId, dislikeSpanId).catch(function (err) {
                    return console.error(err.toString());
                });
        };
        this.dislike = function (commentId, likeSpanId, dislikeSpanId) {
            connectionC.invoke("CommentDislike", currentUser, commentId,
                likeSpanId, dislikeSpanId).catch(function (err) {
                    return console.error(err.toString());
                });
        };
    };

    //delete comment
    var DeleteComment = new function () {
        this.delete = function (commentId) {
            connectionC.invoke("RemoveComment", commentId)
                .catch(function (err) {
                    return console.error(err.toString());
                });
        };
    };
