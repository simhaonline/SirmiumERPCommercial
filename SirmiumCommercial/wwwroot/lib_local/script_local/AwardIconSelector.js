var awardInput = document.getElementById("awardInput");
var iconPreview = document.getElementById("iconPreview");
var awardIcon = "";
var awardColor = "";

//----Create/edit course selected award icon
//row 1
var awardTd11 = document.getElementById("award11");
var awardTd12 = document.getElementById("award12");
var awardTd13 = document.getElementById("award13");
var awardTd14 = document.getElementById("award14");
var awardTd15 = document.getElementById("award15");
var awardTd16 = document.getElementById("award16");
//row 2
var awardTd21 = document.getElementById("award21");
var awardTd22 = document.getElementById("award22");
var awardTd23 = document.getElementById("award23");
var awardTd24 = document.getElementById("award24");
var awardTd25 = document.getElementById("award25");
var awardTd26 = document.getElementById("award26");
//row 3
var awardTd31 = document.getElementById("award31");
var awardTd32 = document.getElementById("award32");
var awardTd33 = document.getElementById("award33");
var awardTd34 = document.getElementById("award34");
var awardTd35 = document.getElementById("award35");
var awardTd36 = document.getElementById("award36");
//row 4
var awardTd41 = document.getElementById("award41");
var awardTd42 = document.getElementById("award42");
var awardTd43 = document.getElementById("award43");
var awardTd44 = document.getElementById("award44");
var awardTd45 = document.getElementById("award45");
var awardTd46 = document.getElementById("award46");

var select = 0;
var selectBefore;

//row 1
awardTd11.addEventListener("click", () => {
    awardTd11.classList += " bg-info";
    if (select != 0) {
        selectBefore.classList.remove("bg-info");
    }
    selectBefore = awardTd11;
    awardIcon = "fa fa-fire-alt";
    select = 1;
    awardInput.value = awardIcon + " " + awardColor;
    iconPreview.className = awardIcon + " " + awardColor;
});

awardTd12.addEventListener("click", () => {
    awardTd12.classList += " bg-info";
    newSelect = awardTd21;

    if (select != 0) {
        selectBefore.classList.remove("bg-info");
    }
    selectBefore = awardTd12;
    awardIcon = "fa fa-shield";
    select = 1;
    awardInput.value = awardIcon + " " + awardColor;
    iconPreview.className = awardIcon + " " + awardColor;
});

awardTd13.addEventListener("click", () => {
    awardTd13.classList += " bg-info";

    if (select != 0) {
        selectBefore.classList.remove("bg-info");
    }
    selectBefore = awardTd13;
    awardIcon = "fa fa-star";
    select = 1;
    awardInput.value = awardIcon + " " + awardColor;
    iconPreview.className = awardIcon + " " + awardColor;
});

awardTd14.addEventListener("click", () => {
    awardTd14.classList += " bg-info";

    if (select != 0) {
        selectBefore.classList.remove("bg-info");
    }
    selectBefore = awardTd14;
    awardIcon = "fa fa-star-o";
    select = 1;
    awardInput.value = awardIcon + " " + awardColor;
    iconPreview.className = awardIcon + " " + awardColor;
});

awardTd15.addEventListener("click", () => {
    awardTd15.classList += " bg-info";

    if (select != 0) {
        selectBefore.classList.remove("bg-info");
    }
    selectBefore = awardTd15;
    awardIcon = "fa fa-star-half-alt";
    select = 1;
    awardInput.value = awardIcon + " " + awardColor;
    iconPreview.className = awardIcon + " " + awardColor;
});

awardTd16.addEventListener("click", () => {
    awardTd16.classList += " bg-info";

    if (select != 0) {
        selectBefore.classList.remove("bg-info");
    }
    selectBefore = awardTd16;
    awardIcon = "pe pe-7s-star";
    select = 1;
    awardInput.value = awardIcon + " " + awardColor;
    iconPreview.className = awardIcon + " " + awardColor;
});
//row 2
awardTd21.addEventListener("click", () => {
    awardTd21.classList += " bg-info";

    if (select != 0) {
        selectBefore.classList.remove("bg-info");
    }
    selectBefore = awardTd21;
    awardIcon = "fa fa-award";
    select = 1;
    awardInput.value = awardIcon + " " + awardColor;
    iconPreview.className = awardIcon + " " + awardColor;
});

awardTd22.addEventListener("click", () => {
    awardTd22.classList += " bg-info";

    if (select != 0) {
        selectBefore.classList.remove("bg-info");
    }
    selectBefore = awardTd22;
    awardIcon = "fa fa-medal";
    select = 1;
    awardInput.value = awardIcon + " " + awardColor;
    iconPreview.className = awardIcon + " " + awardColor;
});

awardTd23.addEventListener("click", () => {
    awardTd23.classList += " bg-info";

    if (select != 0) {
        selectBefore.classList.remove("bg-info");
    }
    selectBefore = awardTd23;
    awardIcon = "pe pe-7s-medal";
    select = 1;
    awardInput.value = awardIcon + " " + awardColor;
    iconPreview.className = awardIcon + " " + awardColor;
});

awardTd24.addEventListener("click", () => {
    awardTd24.classList += " bg-info";

    if (select != 0) {
        selectBefore.classList.remove("bg-info");
    }
    selectBefore = awardTd24;
    awardIcon = "fa fa-trophy";
    select = 1;
    awardInput.value = awardIcon + " " + awardColor;
    iconPreview.className = awardIcon + " " + awardColor;
});

awardTd25.addEventListener("click", () => {
    awardTd25.classList += " bg-info";

    if (select != 0) {
        selectBefore.classList.remove("bg-info");
    }
    selectBefore = awardTd25;
    awardIcon = "pe pe-7s-cup";
    select = 1;
    awardInput.value = awardIcon + " " + awardColor;
    iconPreview.className = awardIcon + " " + awardColor;
});

awardTd26.addEventListener("click", () => {
    awardTd26.classList += " bg-info";

    if (select != 0) {
        selectBefore.classList.remove("bg-info");
    }
    selectBefore = awardTd26;
    awardIcon = "fa fa-crown";
    select = 1;
    awardInput.value = awardIcon + " " + awardColor;
    iconPreview.className = awardIcon + " " + awardColor;
});
//row 3
awardTd31.addEventListener("click", () => {
    awardTd31.classList += " bg-info";

    if (select != 0) {
        selectBefore.classList.remove("bg-info");
    }
    selectBefore = awardTd31;
    awardIcon = "fa fa-chess-rook";
    select = 1;
    awardInput.value = awardIcon + " " + awardColor;
    iconPreview.className = awardIcon + " " + awardColor;
});

awardTd32.addEventListener("click", () => {
    awardTd32.classList += " bg-info";

    if (select != 0) {
        selectBefore.classList.remove("bg-info");
    }
    selectBefore = awardTd32;
    awardIcon = "fa fa-chess-queen";
    select = 1;
    awardInput.value = awardIcon + " " + awardColor;
    iconPreview.className = awardIcon + " " + awardColor;
});

awardTd33.addEventListener("click", () => {
    awardTd33.classList += " bg-info";

    if (select != 0) {
        selectBefore.classList.remove("bg-info");
    }
    selectBefore = awardTd33;
    awardIcon = "fa fa-chess-pawn";
    select = 1;
    awardInput.value = awardIcon + " " + awardColor;
    iconPreview.className = awardIcon + " " + awardColor;
});

awardTd34.addEventListener("click", () => {
    awardTd34.classList += " bg-info";

    if (select != 0) {
        selectBefore.classList.remove("bg-info");
    }
    selectBefore = awardTd34;
    awardIcon = "fa fa-chess-king";
    select = 1;
    awardInput.value = awardIcon + " " + awardColor;
    iconPreview.className = awardIcon + " " + awardColor;
});

awardTd35.addEventListener("click", () => {
    awardTd35.classList += " bg-info";

    if (select != 0) {
        selectBefore.classList.remove("bg-info");
    }
    selectBefore = awardTd35;
    awardIcon = "fa fa-chess-bishop";
    select = 1;
    awardInput.value = awardIcon + " " + awardColor;
    iconPreview.className = awardIcon + " " + awardColor;
});

awardTd36.addEventListener("click", () => {
    awardTd36.classList += " bg-info";

    if (select != 0) {
        selectBefore.classList.remove("bg-info");
    }
    selectBefore = awardTd36;
    awardIcon = "fa fa-chess";
    select = 1;
    awardInput.value = awardIcon + " " + awardColor;
    iconPreview.className = awardIcon + " " + awardColor;
});
//row 4
awardTd41.addEventListener("click", () => {
    awardTd41.classList += " bg-info";

    if (select != 0) {
        selectBefore.classList.remove("bg-info");
    }
    selectBefore = awardTd41;
    awardIcon = "fa fa-graduation-cap";
    select = 1;
    awardInput.value = awardIcon + " " + awardColor;
    iconPreview.className = awardIcon + " " + awardColor;
});

awardTd42.addEventListener("click", () => {
    awardTd42.classList += " bg-info";

    if (select != 0) {
        selectBefore.classList.remove("bg-info");
    }
    selectBefore = awardTd42;
    awardIcon = "fas fa-heart";
    select = 1;
    awardInput.value = awardIcon + " " + awardColor;
    iconPreview.className = awardIcon + " " + awardColor;
});

awardTd43.addEventListener("click", () => {
    awardTd43.classList += " bg-info";

    if (select != 0) {
        selectBefore.classList.remove("bg-info");
    }
    selectBefore = awardTd43;
    awardIcon = "fa fa-atom";
    select = 1;
    awardInput.value = awardIcon + " " + awardColor;
    iconPreview.className = awardIcon + " " + awardColor;
});

awardTd44.addEventListener("click", () => {
    awardTd44.classList += " bg-info";

    if (select != 0) {
        selectBefore.classList.remove("bg-info");
    }
    selectBefore = awardTd44;
    awardIcon = "pe pe-7s-light";
    select = 1;
    awardInput.value = awardIcon + " " + awardColor;
    iconPreview.className = awardIcon + " " + awardColor;
});

awardTd45.addEventListener("click", () => {
    awardTd45.classList += " bg-info";

    if (select != 0) {
        selectBefore.classList.remove("bg-info");
    }
    selectBefore = awardTd45;
    awardIcon = "pe pe-7s-gleam";
    select = 1;
    awardInput.value = awardIcon + " " + awardColor;
    iconPreview.className = awardIcon + " " + awardColor;
});

awardTd46.addEventListener("click", () => {
    awardTd46.classList += " bg-info";

    if (select != 0) {
        selectBefore.classList.remove("bg-info");
    }
    selectBefore = awardTd46;
    awardIcon = "pe pe-7s-sun";
    select = 1;
    awardInput.value = awardIcon + " " + awardColor;
    iconPreview.className = awardIcon + " " + awardColor;
});

//----Create/edit course selected award icon color
var awardError = document.getElementById("awardError");
var awardPrimary = document.getElementById("awardPrimary");
var awardPrimary2 = document.getElementById("awardPrimary2");
var awardInfo = document.getElementById("awardInfo");
var awardSuccess = document.getElementById("awardSuccess");
var awardWarning = document.getElementById("awardWarning");
var awardDanger = document.getElementById("awardDanger");

var selectColor;
var selectColorInd = 0;

awardError.addEventListener("click", () => {
    awardError.style.border = "5px solid #b3ffb3";

    if (selectColorInd != 0) {
        selectColor.style.border = "none";
    }
    selectColor = awardError;
    awardColor = "text-error";
    selectColorInd = 1;
    awardInput.value = awardIcon + " " + awardColor;
    iconPreview.className = awardIcon + " " + awardColor;
});

awardPrimary.addEventListener("click", () => {
    awardPrimary.style.border = "5px solid #b3ffb3";

    if (selectColorInd != 0) {
        selectColor.style.border = "none";
    }
    selectColor = awardPrimary;
    awardColor = "text-primary";
    selectColorInd = 1;
    awardInput.value = awardIcon + " " + awardColor;
    iconPreview.className = awardIcon + " " + awardColor;
});

awardPrimary2.addEventListener("click", () => {
    awardPrimary2.style.border = "5px solid #b3ffb3";

    if (selectColorInd != 0) {
        selectColor.style.border = "none";
    }
    selectColor = awardPrimary2;
    awardColor = "text-primary-2";
    selectColorInd = 1;
    awardInput.value = awardIcon + " " + awardColor;
    iconPreview.className = awardIcon + " " + awardColor;
});

awardInfo.addEventListener("click", () => {
    awardInfo.style.border = "5px solid #b3ffb3";

    if (selectColorInd != 0) {
        selectColor.style.border = "none";
    }
    selectColor = awardInfo;
    awardColor = "text-info";
    selectColorInd = 1;
    awardInput.value = awardIcon + " " + awardColor;
    iconPreview.className = awardIcon + " " + awardColor;
});

awardSuccess.addEventListener("click", () => {
    awardSuccess.style.border = "5px solid #b3ffb3";

    if (selectColorInd != 0) {
        selectColor.style.border = "none";
    }
    selectColor = awardSuccess;
    awardColor = "text-success";
    selectColorInd = 1;
    awardInput.value = awardIcon + " " + awardColor;
    iconPreview.className = awardIcon + " " + awardColor;
});

awardWarning.addEventListener("click", () => {
    awardWarning.style.border = "5px solid #b3ffb3";

    if (selectColorInd != 0) {
        selectColor.style.border = "none";
    }
    selectColor = awardWarning;
    awardColor = "text-warning";
    selectColorInd = 1;
    awardInput.value = awardIcon + " " + awardColor;
    iconPreview.className = awardIcon + " " + awardColor;
});

awardDanger.addEventListener("click", () => {
    awardDanger.style.border = "5px solid #b3ffb3";

    if (selectColorInd != 0) {
        selectColor.style.border = "none";
    }
    selectColor = awardDanger;
    awardColor = "text-danger";
    selectColorInd = 1;
    awardInput.value = awardIcon + " " + awardColor;
    iconPreview.className = awardIcon + " " + awardColor;
});