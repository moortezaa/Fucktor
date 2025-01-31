function Toast(title, text, style) {
    // style: success, danger, warning, info
    var textStyle = "text-white";
    switch (style) {
        case "success":
            textStyle = "text-white";
            break;
        case "danger":
            textStyle = "text-white";
            break;
        case "warning":
            textStyle = "text-dark";
            break;
        case "info":
            textStyle = "text-dark";
            break;
        default:
            textStyle = "text-white";
            break;
    }

    var toast = $("<div></div>");
    toast.addClass("toast");
    toast.addClass("bg-" + style);
    toast.addClass("fade");
    toast.attr("role", "alert");
    toast.attr("aria-live", "assertive");
    toast.attr("aria-atomic", "true");
    toast.attr("data-bs-delay", "5000");
    toast.attr("data-bs-autohide", "true");
    toast.css("position", "fixed");
    toast.css("top", "100vh");
    toast.css("right", "50%");
    toast.css("transform", "translate(50%,-150%)");
    toast.css("margin", "10px");
    toast.css("z-index", "1000");
    var toastHeader = $("<div></div>");
    toastHeader.addClass("toast-header");
    var toastTitle = $("<strong></strong>");
    toastTitle.addClass("me-auto");
    toastTitle.text(title);
    var toastCloseButton = $("<button></button>");
    toastCloseButton.addClass("btn-close");
    toastCloseButton.attr("type", "button");
    toastCloseButton.attr("data-bs-dismiss", "toast");
    toastCloseButton.attr("aria-label", "Close");
    var toastBody = $("<div></div>");
    toastBody.addClass("toast-body");
    toastBody.addClass(textStyle);
    toastBody.text(text);
    toastHeader.append(toastTitle);
    toastHeader.append(toastCloseButton);
    toast.append(toastHeader);
    toast.append(toastBody);
    toast.appendTo("body");
    var bsToast = new bootstrap.Toast(toast);
    bsToast.show();
    //remove toast on the bootstrap event hidden.bs.toast
    toast.on('hidden.bs.toast', function () {
        toast.remove();
    });
}

let inputTypes = ["text", "password", "email", "number", "tel", "date", "time", "month", "week", "range", "select"];

function getCookie(cname) {
    let name = cname + "=";
    let decodedCookie = decodeURIComponent(document.cookie);
    let ca = decodedCookie.split(';');
    for (let i = 0; i < ca.length; i++) {
        let c = ca[i];
        while (c.charAt(0) == ' ') {
            c = c.substring(1);
        }
        if (c.indexOf(name) == 0) {
            return c.substring(name.length, c.length);
        }
    }
    return "";
}

function createCookie(name, value, days) {
    if (days) {
        var date = new Date();
        date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
        var expires = "; expires=" + date.toGMTString();
    }
    else var expires = "";
    document.cookie = name + "=" + value + expires + "; path=/";
}

function readCookie(name) {
    var nameEQ = name + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') c = c.substring(1, c.length);
        if (c.indexOf(nameEQ) == 0) return c.substring(nameEQ.length, c.length);
    }
    return null;
}

function eraseCookie(name) {
    createCookie(name, "", -1);
}

