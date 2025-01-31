function DSDropdownShow(input) {
    var drop = bootstrap.Dropdown.getOrCreateInstance(input);
    setTimeout(() => { drop.show(); }, 150)
    var dropdownmenu = $(input).parent().find('.dropdown-menu');
    if (dropdownmenu.children().length == 0) {
        var p = $('<p class="p-2 text-muted">' + $(input).data('ds-no-items') + '</p>')
        dropdownmenu.append(p);
    }
}

function DSDropdownHide(input) {
    var drop = bootstrap.Dropdown.getOrCreateInstance(input);
    setTimeout(() => {
        drop.hide();
        var dropdownmenu = $(input).parent().find('.dropdown-menu');
        var value = '';
        var text = '';
        if (dropdownmenu.find('button')[0] != undefined) {
            value = $(dropdownmenu.find('button')[0]).val();
            text = $(dropdownmenu.find('button')[0]).text();
        }
        $('#' + $(input).data('ds-value-input')).val(value);
        $(input).val(text);
        $('#' + $(input).data('ds-value-input')).change();
    }, 150)
}

function DSDropdownLoad(input) {
    var action = $(input).data('ds-action');
    var controller = $(input).data('ds-controller');
    var drop = $(input).parent().find('.dropdown-menu');
    var formData = new FormData();
    formData.append('term', $(input).val());
    var filterInputs = $(input).data('ds-filter-inputs');
    for (var i in filterInputs) {
        var filterInput = filterInputs[i];
        formData.append(filterInput[0], $('#' + filterInput[1]).val());
    }
    $.ajax({
        url: window.location.protocol + "//" + window.location.host + '/' + controller + '/' + action,
        type: 'post',
        dataType: "JSON",
        data: formData,
        processData: false,
        contentType: false,
        success: function (data, status) {
            drop.children().remove();
            if (data.length == 0) {
                var p = $('<p class="p-2 text-muted">' + $(input).data('ds-no-items') + '</p>')
                drop.append(p);
            }
            for (var i in data) {
                var d = data[i];
                var li = new $('<li></li>')
                var button = new $('<button></button>')
                li.append(button);
                button.text(d.text);
                button.val(d.value);
                button.addClass('btn btn-light col-12 text-end');
                button.attr('type', 'button');
                button.click((e) => DSDropdownItemSelected(e.currentTarget));
                drop.append(li);
            }
        },
        error: function (xhr, desc, err) {
            drop.children().remove();
            var p = $('<p class="p-2 text-danger">' + $(input).data('ds-error') + '</p>')
            drop.append(p);
        }
    });
}

function DSDropdownItemSelected(button) {
    $(button).parent().parent().prepend($(button).parent());
}

$(document).ready(() => {
    $('.ds-dropdown-input').each((i, e) => {
        $(e).val($(e).data('ds-selected-text'));
    })
})