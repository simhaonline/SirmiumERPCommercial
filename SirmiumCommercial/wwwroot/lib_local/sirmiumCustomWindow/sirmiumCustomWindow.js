//load CSS file
var head = document.getElementsByTagName('HEAD')[0];
var link = document.createElement('link');
link.rel = 'stylesheet';
link.href = "/lib_local/sirmiumCustomWindow/sirmiumCustomWindow.css";
head.appendChild(link);

//Success window

 /*   <button onclick="CustomSuccessWindow.show('msg')">
        btnName
      </button>
      <div id="customSuccessWindow"></div>*/

var customSuccessWindow = document.getElementById('customSuccessWindow');
if (customSuccessWindow != null) {
    customSuccessWindow.innerHTML = `
        <div id="successFreezeLayer" class="freeze-layer" style="display: none"></div>
        <div id="successWindowCont" class="dlg-container" style="display: none">
            <div class="dlg-success-header">
                <i id="successIcon" class="fas fa-check"></i>
            </div>
            <div class="dlg-body">
                <h2 id="successTxt">Success!</h2>
                <h4>
                    <span id="successDialogText"></span>
                </h4>
            </div>
            <div class="dlg-footer">
                <a onclick="CustomSuccessWindow.close()" class="">Ok</a>
            </div>
        </div>
    `;
}
var successWindow = document.getElementById('successWindowCont');
var successFreezeLayer = document.getElementById('successFreezeLayer');

var CustomSuccessWindow = new function () {
    this.show = function (msg) {
        var successDialogText = document.getElementById('successDialogText');
        successDialogText.textContent = msg;
        successWindow.style.display = '';
        successFreezeLayer.style.display = '';
    };

    this.close = function () {
        successWindow.style.display = 'none';
        successFreezeLayer.style.display = 'none';
    };
}

//warning dialog

/*    <button onclick = "CustomWarningWindow.show('msg', 'btnMsg', '@Url.Action(" action", "controler")')">
        btnName
      </button >
     <div id="customWarningDialog"></div>*/


var customWarningDialog = document.getElementById('customWarningDialog');
if (customWarningDialog != null) {
    customWarningDialog.innerHTML = `
        <div id="freezeLayer" class="freeze-layer" style="display: none"></div>
        <div id="warningWindowCont" class="dlg-container" style="display: none">
        <div class="dlg-warning-header blinking">
            <i class="pe pe-7s-attention"></i>
        </div>
        <div class="dlg-body">
            <h2 id="warningTxt">Warning!</h2>
            <h4>
                <span id="dialogText"></span>
            </h4>
        </div>
        <div class="dlg-footer">
            <a onclick="CustomWarningWindow.close()" class="">Cancel</a>
            <a id="positive" onclick="CustomWarningWindow.yes()" style="background: #D30000; color: #fff"><span id="positiveTxt"></span></a>
        </div>
    `;
}
var warningWindow = document.getElementById('warningWindowCont');
var freezeLayer = document.getElementById('freezeLayer');
var positiveBtn = document.getElementById('positiveTxt');

var CustomWarningWindow = new function () {
    this.show = function (msg, btnMsg, link) {
        var dialogText = document.getElementById('dialogText');
        dialogText.textContent = msg;
        positiveBtn.textContent = btnMsg;
        warningWindow.style.display = '';
        this.link = link;
        freezeLayer.style.display = '';
    };

    this.yes = function () {

        var positive = document.getElementById('positive');
        positive.href = this.link;
        this.close();
    };

    this.close = function () {
        warningWindow.style.display = 'none';
        freezeLayer.style.display = 'none';
    };
}

//blinking text
function blinker() {
    if ($('.blinking') != null) {
        $('.blinking').fadeOut(500);
        $('.blinking').fadeIn(500);
    }
};
setInterval(blinker, 100);