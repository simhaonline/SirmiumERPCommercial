﻿//show presentations, users or representations
var showPresentations = document.getElementById('showPresentations');
var showPresentationsBtn = document.getElementById('showPresentationsBtn');
var showUsers = document.getElementById('showUsers');
var showUsersBtn = document.getElementById('showUsersBtn');
var showRepresentations = document.getElementById('showRepresentations');
var showRepresentationsBtn = document.getElementById('showRepresentationsBtn');
var showComments = document.getElementById('showComments');
var showCommentsBtn = document.getElementById('showCommentsBtn');

showPresentationsBtn.style.color = "#fff";
showPresentationsBtn.style.backgroundColor = "cadetblue";

showUsersBtn.addEventListener('click', () => {
    showPresentations.style.display = "none";
    showRepresentations.style.display = "none";
    showComments.style.display = "none";
    showUsers.style.display = "";
    showPresentationsBtn.style.color = "";
    showPresentationsBtn.style.backgroundColor = "";
    showUsersBtn.style.color = "#fff";
    showUsersBtn.style.backgroundColor = "cadetblue";
    showRepresentationsBtn.style.color = "";
    showRepresentationsBtn.style.backgroundColor = "";
    showCommentsBtn.style.color = "";
    showCommentsBtn.style.backgroundColor = "";
});

showPresentationsBtn.addEventListener('click', () => {
    showPresentations.style.display = "";
    showRepresentations.style.display = "none";
    showUsers.style.display = "none";
    showComments.style.display = "none";
    showPresentationsBtn.style.color = "#fff";
    showPresentationsBtn.style.backgroundColor = "cadetblue";
    showUsersBtn.style.color = "";
    showUsersBtn.style.backgroundColor = "";
    showRepresentationsBtn.style.color = "";
    showRepresentationsBtn.style.backgroundColor = "";
    showCommentsBtn.style.color = "";
    showCommentsBtn.style.backgroundColor = "";
});

showRepresentationsBtn.addEventListener('click', () => {
    showRepresentations.style.display = "";
    showPresentations.style.display = "none";
    showUsers.style.display = "none";
    showComments.style.display = "none";
    showRepresentationsBtn.style.color = "#fff";
    showRepresentationsBtn.style.backgroundColor = "cadetblue";
    showUsersBtn.style.color = "";
    showUsersBtn.style.backgroundColor = "";
    showPresentationsBtn.style.color = "";
    showPresentationsBtn.style.backgroundColor = "";
    showCommentsBtn.style.color = "";
    showCommentsBtn.style.backgroundColor = "";
});

showCommentsBtn.addEventListener('click', () => {
    showComments.style.display = "";
    showRepresentations.style.display = "none";
    showPresentations.style.display = "none";
    showUsers.style.display = "none";
    showCommentsBtn.style.color = "#fff";
    showCommentsBtn.style.backgroundColor = "cadetblue";
    showRepresentationsBtn.style.color = "";
    showRepresentationsBtn.style.backgroundColor = "";
    showUsersBtn.style.color = "";
    showUsersBtn.style.backgroundColor = "";
    showPresentationsBtn.style.color = "";
    showPresentationsBtn.style.backgroundColor = "";
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