var videoImgA = document.getElementById("videoImgActivity");
var videoImgPlayA = document.getElementById("videoImgPlayActivity");

videoImgA.onmouseenter = function () {
    videoImgPlayA.hidden = false;
}
videoImgA.onmouseleave = function () {
    videoImgPlayA.hidden = true;
}