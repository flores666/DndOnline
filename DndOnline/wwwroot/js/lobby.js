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
    //
    // document.addEventListener('beforeunload', async function(){
    //     await connection.invoke("LeaveLobby");
    // });

    <!--Подключаемся-->
    connection.start().then(async function () {
        // await connection.invoke("JoinLobby");
        
        console.log("signalR connection started");
    }).catch(function (err) {
        return console.error(err.toString());
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
});