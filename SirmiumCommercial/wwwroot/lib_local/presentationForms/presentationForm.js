//show presentations or users
var showPresentations = document.getElementById('showPresentations');
var showPresentationsBtn = document.getElementById('showPresentationsBtn');
var showUsers = document.getElementById('showUsers');
var showUsersBtn = document.getElementById('showUsersBtn');

showPresentationsBtn.style.color = "#fff";
showPresentationsBtn.style.backgroundColor = "cadetblue";

showUsersBtn.addEventListener('click', () => {
    showPresentations.style.display = "none";
    showUsers.style.display = "";
    showPresentationsBtn.style.color = "";
    showPresentationsBtn.style.backgroundColor = "";
    showUsersBtn.style.color = "#fff";
    showUsersBtn.style.backgroundColor = "cadetblue";
});

showPresentationsBtn.addEventListener('click', () => {
    showPresentations.style.display = "";
    showUsers.style.display = "none";
    showPresentationsBtn.style.color = "#fff";
    showPresentationsBtn.style.backgroundColor = "cadetblue";
    showUsersBtn.style.color = "";
    showUsersBtn.style.backgroundColor = "";
});

//New Presentation Form
var presentationFormWindow = document.getElementById('presentationFormContent');
var freezeLayer = document.getElementById('freezeLayer');

var NewPresentation = new function () {
    this.show = function () {
        presentationFormWindow.style.display = '';
        freezeLayer.style.display = '';
    };
    this.close = function () {
        presentationFormWindow.style.display = 'none';
        freezeLayer.style.display = 'none';
    }
}

//Edit Presentation Form
var freezeLayer = document.getElementById('freezeLayer');

var EditPresentation = new function () {
    this.show = function (id) {
        var toShow = document.getElementsByClassName(id);
        toShow[0].style.display = '';
        freezeLayer.style.display = '';
    };
    this.close = function (id) {
        var toClose = document.getElementsByClassName(id);
        toClose[0].style.display = 'none';
        freezeLayer.style.display = 'none';
    }
}