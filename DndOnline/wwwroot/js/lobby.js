$(document).ready(function () {
    let connection = new signalR.HubConnectionBuilder().withUrl("/lobbyHub").build();

    <!--Подключение к лобби-->
    connection.on("JoinLobby", function (userName) {
        let elem = getElementFromUl('player_list', userName);
        console.log('player ' + userName + ' connected');

        if (elem) return;

        let li = document.createElement("li");
        document.getElementById("player_list").appendChild(li);
        li.textContent = `${userName}`;
    });

    <!--Отключение от лобби-->
    connection.on("LeaveLobby", function (userName) {
        console.log('player ' + userName + ' disconnected');

        let elem = getElementFromUl('player_list', userName);
        if (elem) elem.remove();
    });

    connection.on('TokenAdd', function (item) {
        let child = document.createElement('li');
        child.className = 'list-item-child list-item-child_transportable';
        child.draggable = true;
        child.dataset.src = item.filePath;
        child.innerText = item.name;
        document.querySelector('#tokens_list > .list-body').appendChild(child);
    });

    connection.on('MapAdd', function (item) {
        let child = document.createElement('li');
        child.className = 'list-item-child list-item-child_transportable';
        child.draggable = true;
        child.dataset.src = item.filePath;
        child.innerText = item.name;
        document.querySelector('#maps_list > .list-body').appendChild(child);
    });
    
    <!--Подключаемся-->
    connection.start().then(async function () {
        console.log("signalR connection started");
    }).catch(function (err) {
        return console.error(err.toString());
    });
});

$(document).on('click', '.list-header', function () {
    let listBody = $(this).parent().find('.list-body');
    
    if (listBody.hasClass('hidden')) {
        listBody.removeClass('hidden');
    }
    else listBody.addClass('hidden');
    
    let span = $(this).find('span');
    if (span.hasClass('plus')) span.attr('class', 'minus');
    else span.attr('class', 'plus');
});

$(document).on('change input', '[required]', function (e) {
    let span = $(this).siblings('.error-span');
    if (span.length === 0) span = $(this).parent().siblings('.error-span');
    
    span.remove();
});

function getElementFromUl(ulId, targetValue) {
    let myList = document.getElementById(ulId);
    let listItems = myList.getElementsByTagName("li");
    let foundItem = null;

    for (let i = 0; i < listItems.length; i++) {
        if (listItems[i].innerHTML === targetValue) {
            foundItem = listItems[i];
            break;
        }
    }
    return foundItem;
}