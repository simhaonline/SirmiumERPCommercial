const player = document.getElementById("player");
const canvas = document.getElementById("canvas");
const context = canvas.getContext("2d");
const captureBtn = document.getElementById("capture");
const captureDiv = document.getElementById("captureDiv");
const saveBtn = document.getElementById("save");
const saveDiv = document.getElementById("saveDiv");
const newBtn = document.getElementById("new");
const img = document.getElementById("img");

const constraints = {
    video: true,
};

if (navigator.mediaDevices == undefined) {
    navigator.mediaDevices = {};
    navigator.mediaDevices.getUserMedia = function (constraints) {
        let getUserMedia = navigator.webkitGetUserMedia || navigator.mozGetUserMedia;
        if (!getUserMedia) {
            return Promise.reject(new Error('getUserMedia is not implemented in this browser'));
        }
        return new Promise(function (resolve, reject) {
            getUserMedia.call(navigator, cons, resolve, reject);
        });
    }
} else {
    navigator.mediaDevices.enumerateDevices()
        .then(devices => {
            devices.forEach(device => {
                console.log(device.kind.toUpperCase(), device.label);
            })
        })
        .catch(err => {
            console.log(err.name, err.message);
        })
}

navigator.mediaDevices.getUserMedia(constraints)
    .then((stream) => {
        if ("srcObject" in player) {
            player.srcObject = stream;
        } else {
            player.src = window.Url.createObjectUrl(stream);
        }
    });

player.onloadedmetadata = function (ev) {
    player.play();
};

captureBtn.addEventListener("click", () => {
    context.drawImage(player, 0, 0, canvas.width, canvas.height);
    img.value = canvas.toDataURL();
    canvas.hidden = false;
    player.hidden = true;
    captureBtn.hidden = true;
    captureDiv.hidden = true;
    saveBtn.hidden = false;
    saveDiv.hidden = false;
    newBtn.hidden = false;
});

newBtn.addEventListener("click", () => {
    canvas.hidden = true;
    player.hidden = false;
    captureBtn.hidden = false;
    captureDiv.hidden = false;
    saveBtn.hidden = true;
    saveDiv.hidden = true;
    newBtn.hidden = true;
});