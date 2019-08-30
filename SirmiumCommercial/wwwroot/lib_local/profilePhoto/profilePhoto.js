//New Photo
var capturePhotoWindow = document.getElementById('capturePhoto');
var freezeLayer = document.getElementById('freezeLayer');

var NewPhoto = new function () {
    this.show = function () {
        capturePhotoWindow.style.display = '';
        freezeLayer.style.display = '';
    };
    this.close = function () {
        capturePhotoWindow.style.display = 'none';
        freezeLayer.style.display = 'none';
    }
}

//Upload photo
var uploadPhotoWindow = document.getElementById('uploadPhoto');
var freezeLayer = document.getElementById('freezeLayer');

var UploadPhoto = new function () {
    this.show = function () {
        uploadPhotoWindow.style.display = '';
        freezeLayer.style.display = '';
    };
    this.close = function () {
        uploadPhotoWindow.style.display = 'none';
        freezeLayer.style.display = 'none';
    }
}

var loadFile = function (event) {
    var reader = new FileReader();
    reader.onload = function () {
        var output = document.getElementById('output');
        output.src = reader.result;
    };
    reader.readAsDataURL(event.target.files[0]);
};