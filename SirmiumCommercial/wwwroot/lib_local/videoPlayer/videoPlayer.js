var freezeLayer = document.getElementById('videoFreezeLayer');
var player = document.getElementById('player');
var videoSrc = document.getElementById('videoSrc');

var videoPlay = new function () {
    this.show = function (src) {
        videoSrc.src = src;
        player.style.display = '';
        freezeLayer.style.display = '';
    };
    this.close = function () {
        player.style.display = 'none';
        freezeLayer.style.display = 'none';
    }
}