$(document).ready(function () {
    $('.lobby-creation-nav a').click(async function (event) {
        event.preventDefault();

        let url = $(this).attr('href');
        await loadPartialContentAsync(url, $('.lobby-creation-content'));
    });
});

<!-- Обработка добавляения нового элемента -->
$(document).on('click', '#new-item', async function () {
    let modal = await getPartialContentAsync('/LobbyConstructor/ItemPartialForm');
    $('body').prepend(modal);
});

$(document).on('click', '#new-creature', async function () {
    let modal = await getPartialContentAsync('/LobbyConstructor/CreaturePartialForm');
    $('body').prepend(modal);
});

$(document).on('click', '#new-character', async function () {
    let modal = await getPartialContentAsync('/LobbyConstructor/CharacterPartialForm');
    $('body').prepend(modal);
});

<!-- Обработка создания нового элемента -->
$(document).on('submit', '.creation-form', async function (event) {
    event.preventDefault();
    let form = event.target;
    let validated = validateBaseForm(form);
    if (!validated) return;
    let response;

    let model = new FormData(form);
debugger
    switch (form.id) {
        case "character-form":
            response = await sendRequestAsync('POST', '/LobbyConstructor/NewCharacter', model);
            break;
        case "creature-form":
            response = await sendRequestAsync('POST', '/LobbyConstructor/NewCreature', model);
            break;
        case "item-form":
            response = await sendRequestAsync('POST', '/LobbyConstructor/NewItem', model);
            break;
        default:
            response = null;
            break;
    }
    console.log(response);
});

// Валидация основных общих полей формы создания объекта
function validateBaseForm(form) {
    let alert = $('<span>');
    alert.addClass('error-span');
    let result = true;

    let name = $(form).find('[name="Name"]');
    let nameLabel = $(form).getLabelValue('Name');
    let spans = $(form).find('.error-span');

    if (name.val() == '') {
        if (spans.length == 0) {
            alert.text(`Поле \"${nameLabel}\" должно быть заполнено`);
            name.parent().parent().append(alert);
        }
        result = false;
    }

    if (name.val().length > 50) {
        if (spans.length == 0) {
            alert.text(`Поле \"${nameLabel}\" имеет недопустимое количество символов`);
            name.parent().parent().append(alert);
        }
        result = false;
    }

    let description = $(form).find('[name="Description"]');
    let descLabel = $(form).getLabelValue('Description');

    if (description.val().length > 5000) {
        if (spans.length == 0) {
            alert.text(`Поле \"${descLabel}\" имеет недопустимое количество символов`);
            description.parent().after(alert);
        }
        result = false;
    }

    let img = $(form).find('[name="File"]')[0].files[0];
    if (img != null) {
        compressImage(img, 512 * 1024, function (result) {
            $('.input-file-list-img').src = "";
        });
    }

    return result;
}

// Убираем error-span при вводе
$(document).on('input', 'input', function () {
    $('.creation-form .error-span').remove();
});

function addItem() {
    let itemsContainer = document.getElementById('lobby-creation-item-container');

    let newItem = document.createElement('div');
    newItem.className = 'lobby-creation-item card3d';
    itemsContainer.appendChild(newItem);
}
