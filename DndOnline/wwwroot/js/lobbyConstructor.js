$(document).ready(function () {
    $('#new-enemy').on('click', async function () {
        let modal = await getPartialContentAsync('/LobbyConstructor/EnemyPartialForm');
        
        $('body').prepend(modal);
    });

    $('#new-item').on('click', async function () {
        let modal = await getPartialContentAsync('/LobbyConstructor/ItemPartialForm');

        $('body').prepend(modal);
    });

    $('#new-character').on('click', async function () {
        let modal = await getPartialContentAsync('/LobbyConstructor/CharacterPartialForm');

        $('body').prepend(modal);
    });
});

//запрос на создание модели
async function sendCreationRequest(url, formData) {
    return new Promise((resolve, reject) => {
        $.ajax({
            url: url,
            data: formData,
            method: 'POST',
            success: function (response) {
                resolve(response);
            },
            error: function (error) {
                reject(error);
            }
        });
    });
}

function addItem() {
    let itemsContainer = document.getElementById('lobby-creation-item-container');

    let newItem = document.createElement('div');
    newItem.className = 'lobby-creation-item card3d';
    itemsContainer.appendChild(newItem);
}

function createEnemyModal() {
    createBaseModal();
    // Создание первого блока
    let row1 = $('<div>').addClass('row');
    let btn = $('<button>').addClass('col-1 btn float-end');
    btn.text('Создать');
    
    let label1 = $('<label>').attr('for', 'Name').text('Имя персонажа');
    let inputWrapper1 = $('<div>').addClass('form-group');
    let input1 = $('<input>').attr({
        'autocomplete': 'off',
        'class': 'input-field col-10',
        'id': 'Name'
    });
    let validationSpan1 = $('<span>').attr('id', 'Name').hide();
    
    inputWrapper1.append(input1, validationSpan1, btn);
    row1.append(label1, inputWrapper1);

    // Создание второго блока
    let row2 = $('<div>').addClass('row');

    let label2 = $('<label>').attr('for', 'File').text('Изображение');
    let inputWrapper2 = $('<div>').addClass('form-group col-4');
    let fileInput = $('<input>').attr({
        'type': 'file',
        'class': 'input-field form-control',
        'id': 'File'
    });

    inputWrapper2.append(fileInput);
    row2.append(label2, inputWrapper2);

    // Создание третьего блока
    let row3 = $('<div>').addClass('row');

    let label3 = $('<label>').attr('for', 'Description').text('Описание');
    let inputWrapper3 = $('<div>').addClass('form-group mb-0');
    let textarea = $('<textarea>').attr({
        'id': 'Description',
        'class': 'auto-textarea input-field col-12 description-textarea'
    });

    inputWrapper3.append(textarea);
    row3.append(label3, inputWrapper3);
    
    $('.modal-content').append(row1, row3, row2);
}