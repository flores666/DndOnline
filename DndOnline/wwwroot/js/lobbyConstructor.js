$(document).ready(function () {
    $('#new-enemy').on('click', async function () {
        let modal = await getPartialContentAsync('/LobbyConstructor/CreaturePartialForm');
        
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
