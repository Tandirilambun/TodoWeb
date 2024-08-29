// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

//// Write your JavaScript code.

function input_change() {
    var txt_title = document.getElementById("input-title");
    var txt_body = document.getElementById("input-body");
    txt_title.style.height = 'auto';
    txt_title.style.height = txt_title.scrollHeight + 'px';
    txt_body.style.height = 'auto';
    txt_body.style.height = txt_body.scrollHeight + 'px';
    if (txt_title.value.length > 0 || txt_body.value.length > 0) {
        button_add.classList.add("button-translate");
        button_add.classList.remove("button-create");
    } else {
        button_add.classList.remove("button-translate");
        button_add.classList.add("button-create");
    }
};

function catChange() {
    button_add.classList.add("button-translate");
    button_add.classList.remove("button-create");
}

function toFormParticipant() {
    const formPage2 = document.getElementById("project-participant");
    formPage2.scrollIntoView();
};
function toFormDetail() {
    const formPage1 = document.getElementById("project-detail");
    formPage1.scrollIntoView();
}

const tooltipTriggerList = document.querySelectorAll('[data-bs-toggle="tooltip"]');
const tooltipList = [...tooltipTriggerList]
    .map(tooltipTriggerEl => new bootstrap.Tooltip(tooltipTriggerEl));

