var videoWindow = document.getElementById('video');
var freezeLayer = document.getElementById('videoFreezeLayer');

var NewVideo = new function () {
    this.show = function () {
        videoWindow.style.display = '';
        freezeLayer.style.display = '';
    };
    this.close = function () {
        videoWindow.style.display = 'none';
        freezeLayer.style.display = 'none';
    }
}

var Camera = new function () {
    this.active = function () {
        let constraintObj = {
            audio: true,
            video: true
        };

        if (navigator.mediaDevices == undefined) {
            navigator.mediaDevices = {};
            navigator.mediaDevices.getUserMedia = function (constraintObj) {
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

        navigator.mediaDevices.getUserMedia(constraintObj)
            .then(function (mediaStreamObj) {
                let video = document.getElementById('recorder');
                if ("srcObject" in video) {
                    video.srcObject = mediaStreamObj;
                } else {
                    video.src = windo.Url.createObjectUrl(mediaStreamObj);
                }

                video.onloadedmetadata = function (ev) {
                    video.play();
                };

                let start = document.getElementById('recBtn');
                let stop = document.getElementById('stopBtn');
                let vidSave = document.getElementById('player');
                let save = document.getElementById('saveBtn');
                let videoSave = document.getElementById('videoUrl');
                let mediaRecorder = new MediaRecorder(mediaStreamObj);
                let chunks = [];

                start.addEventListener('click', (ev) => {
                    mediaRecorder.start();
                    start.style.display = 'none';
                    stop.style.display = '';
                    video.style.display = '';
                    vidSave.style.display = 'none';
                    console.log(mediaRecorder.state);
                })

                stop.addEventListener('click', (ev) => {
                    mediaRecorder.stop();
                    stop.style.display = 'none';
                    start.style.display = 'inline';
                    console.log(mediaRecorder.state);
                });

                mediaRecorder.ondataavailable = function (ev) {
                    chunks.push(ev.data);
                }

                var reader = new FileReader();
                let controls = document.getElementById('controls');
                let playerControls = document.getElementById('controls2');
                mediaRecorder.onstop = (ev) => {
                    let blob = new Blob(chunks, { 'type': 'video/mp4' });
                    chunks = [];
                    let videoURL = window.URL.createObjectURL(blob);
                    vidSave.src = videoURL;
                    videoSave.value = videoURL;

                    reader.readAsDataURL(blob);
                    console.log(reader.result.toString());


                    video.style.display = 'none';
                    vidSave.style.display = 'inline';
                    controls.style.display = 'none';
                    vidSave.controls = true;
                }

                vidSave.onended = (ev) => {
                    console.log('video stop');
                    videoSave.value = reader.result.toString();
                    console.log(reader.result.toString());
                    vidSave.controls = false;
                    playerControls.style.display = 'inline';
                }

                let replay = document.getElementById('replayBtn');
                replay.addEventListener('click', (ev) => {
                    playerControls.style.display = 'none';
                    vidSave.controls = true;
                    vidSave.play();
                });

                let newVideo = document.getElementById('newVideoBtn');
                newVideo.addEventListener('click', (ev) => {
                    vidSave.style.display = 'none';
                    playerControls.style.display = 'none';
                    video.style.display = 'inline';
                    controls.style.display = 'inline';
                    document.rel
                });
            })
            .catch(function (err) {
                console.log(err.name, err.message);
            });
    }
}