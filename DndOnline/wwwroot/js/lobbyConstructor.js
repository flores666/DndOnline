$(document).ready(function () {
    $('.lobby-creation-nav a').click(async function (event) {
        event.preventDefault();

        let url = $(this).attr('href');
        await loadPartialContentAsync(url, $('.lobby-creation-content'));
    });
});

<!-- Обработка добавляения нового элемента -->
$(document).on('click', '#add-item', async function () {
    let modal = await getPartialContentAsync('/LobbyConstructor/ItemPartialForm');
    $('body').prepend(modal);
});

$(document).on('click', '#add-creature', async function () {
    let modal = await getPartialContentAsync('/LobbyConstructor/CreaturePartialForm');
    $('body').prepend(modal);
});

$(document).on('click', '#add-character', async function () {
    let modal = await getPartialContentAsync('/LobbyConstructor/CharacterPartialForm');
    $('body').prepend(modal);
});

<!-- Обработка создания нового элемента -->
$(document).on('submit', '.creation-form', async function (event) {
    event.preventDefault();
    let loader = new Loader();
    loader.show();
    
    let form = event.target;
    let validated = validateBaseForm(form);
    if (!validated) return;
    let model = new FormData(form);
    
    let img = $(form).find('[name="File"]')[0].files[0];
    if (img != null) {
        await compressImage(img, 512 * 1024, function (compressedFile) {
            let newFile = new File([compressedFile], img.name, { type: 'image/jpeg', lastModified: Date.now() });
            model.set('File', newFile);
        });
    }
    
    let response;

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
    
    loader.hide();
    
    if (response.isSuccess) {
        $('.modal').remove();
        addItem(response.data);
    } else {
        console.error('Непредвиденная ошибка при отправке формы');
    }
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

    return result;
}

// Убираем error-span при вводе
$(document).on('input', 'input', function () {
    $('.creation-form .error-span').remove();
});

function addItem(model) {
    let name = model.name;
    let desc = model.description;
    let pic = model.relativePath;
    
    let card3d = $('<div></div>');
    card3d.addClass('lobby-creation-item card card3d');
    card3d.title = 'Нажмите чтобы увидеть подробное описание';
    
    let nameWrapper = $('<div></div>');
    nameWrapper.addClass('lobby-creation-item-name d-flex');
    nameWrapper.append(`<p>${name}</p>`);
    card3d.append(nameWrapper);
    
    let file = $('<div></div>');
    file.addClass('lobby-creation-item-file');
    file.append(`<img src="${pic ?? "Content/default.png"}" alt="img" />`);
    card3d.append(file);
    
    if (desc != null) {
        let item = $('<div></div>');
        item.addClass = 'lobby-creation-item-desc';
        item.append(`<p>${desc}</p>`)
        card3d.append(item);
    }
    
    $('.lobby-creation-item-container').append(card3d);
    handleCard3d(2.5);
}
