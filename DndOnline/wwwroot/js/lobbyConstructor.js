document.addEventListener("DOMContentLoaded", function () {
    handleCard3d();
});

function addItem() {
    let itemsContainer = document.getElementById('lobby-creation-item-container');
    
    let newItem = document.createElement('div');
    newItem.className = 'lobby-creation-item card3d';
    itemsContainer.appendChild(newItem);
}