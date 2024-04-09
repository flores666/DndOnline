let globalDraggingItem;
let sceneObjectsContainer;
let ctrlPressed;

const maxFPS = 30;

let app = new PIXI.Application({
    antialias: true,
    maxFPS: maxFPS,
    height: 5000,
    width: 5000,
    transparent: false,
    resolution: 1
});

$(document).ready(function () {
    let game = $('.game');

    sceneObjectsContainer = createObjectsContainer();
    game.append(app.view);

    processInterface(game);
    processGame(game);
    app.renderer.render(app.stage);
});

function createObjectsContainer() {
    let container = new PIXI.Container();
    return container;
}

function processGame() {
    window.addEventListener('keydown', function (event) {
        if (event.key === 'Control') {
            ctrlPressed = true;
            $('.game').addClass('pointer');
        }
    });

    window.addEventListener('keyup', function (event) {
        if (event.key === 'Control') {
            ctrlPressed = false;
            $('.game').removeClass('pointer');
        }
    });

    app.renderer.view.addEventListener('mousedown', function (event) {
        if (ctrlPressed && event.button === 0) { // Ctrl и левая кнопка мыши
            let startX = event.clientX;
            let startY = event.clientY;

            let isDragging = true;

            const mouseMoveHandler = function (event) {
                if (isDragging) {
                    const dx = event.clientX - startX;
                    const dy = event.clientY - startY;

                    app.stage.x += dx;
                    app.stage.y += dy;

                    startX = event.clientX;
                    startY = event.clientY;
                }
            };

            const mouseUpHandler = function () {
                isDragging = false;
                window.removeEventListener('mousemove', mouseMoveHandler);
                window.removeEventListener('mouseup', mouseUpHandler);
            };

            window.addEventListener('mousemove', mouseMoveHandler);
            window.addEventListener('mouseup', mouseUpHandler);
        }
    });

    // Событие прокрутки колеса мыши
    app.renderer.view.addEventListener('wheel', function (e) {
        if (ctrlPressed) { // Проверяем, что нажата клавиша Ctrl
            e.preventDefault();
            let s = app.stage.scale.x;
            let tx = (e.x - app.stage.x) / s;
            let ty = (e.y - app.stage.y) / s;
            s += -1 * Math.max(-1, Math.min(1, e.deltaY)) * 0.1 * s;
            app.stage.setTransform(-tx * s + e.x, -ty * s + e.y, s, s);
        }
    });

    // Создаем контейнер для хранения изображений
    app.stage.addChild(sceneObjectsContainer);

    // Добавляем обработчики событий для перетаскивания изображений
    app.view.addEventListener('dragover', (event) => {
        event.preventDefault();
    });

    app.view.addEventListener('drop', (event) => handlePasting(event));

    // вставка спрайта извне путем drag and drop
    function handlePasting(event) {
        event.preventDefault();

        let s = app.stage.scale.x;
        let x = (event.x - app.stage.x) / s;
        let y = (event.y - app.stage.y) / s;
        
        switch (typeof globalDraggingItem) {
            case "undefined" :
                // Получаем информацию о перетаскиваемых элементах
                const file = event.dataTransfer.files[0];
                let fileType = file.type.split('/')[0];
                if (fileType !== "image") return;

                let reader = new FileReader();
                let loader = new Loader();

                reader.onload = function(event) {
                    loader.hide();
                    let img = new Image();
                    img.src = event.target.result;

                    let modal = createBaseModal(70, 30);
                    modal.style.display = 'flex';

                    modal.innerHTML += '<div class="modal-main-pic" style="overflow: hidden"><img src="' + img.src + '"></div>';
                    modal.innerHTML += '<div class="modal-main"></div>';
                    let modalMain = $('.modal-main')[0];
                    
                    modalMain.innerHTML += '<input id="importing_name" class="input-field modal-header-input" placeholder="Нажмите для ввода названия"/>';
                    
                    modalMain.innerHTML += '<div class="select-wrapper"></div>';
                    let selectWrapper = $('.select-wrapper')[0];
                    selectWrapper.innerHTML += '<span>Тип импортируемого элемента</span>';
                    selectWrapper.innerHTML += '<select id="importing_type">' + 
                        '<option value="none">Выберите тип</option>' +
                        '<option value="token">Токен</option>' +
                        '<option value="map">Локация</option>' +
                        '</select>';

                    $('.select-wrapper').on('change', '#importing_type', function () {
                        let picWrapper = $('.modal-main-pic')[0];
                        // let picker = $('<div class="picker">')[0];
                        
                        if (this.value === 'none') {
                            picWrapper.style.overflow = 'hidden';
                            modal.querySelector('.btn').remove();
                            // $('.picker')?.remove();
                            return;
                        }
                        
                        if (this.value === 'token') {
                            picWrapper.style.overflow = 'scroll';
                            // picWrapper.appendChild(picker);
                        }

                        if (this.value === 'map') {
                            picWrapper.style.overflow = 'hidden';
                            // $('.picker')?.remove();
                        }
                        
                        if (modal.querySelector('.btn') == null) {
                            let btn = '<button class="btn">Готово</button>';
                            modalMain.append($(btn)[0]);
                        }
                    });
                    
                    $(modal).on('click', '.btn', async function () {
                        let name = $('#importing_name').val();
                        let type = $('#importing_type').val();
                        let url = '/lobby';
                        
                        let fd = new FormData();
                        fd.append('file', file);
                        fd.append('name', name);
                        
                        if (type === 'token') url += '/saveToken';
                        if (type === 'map') url += '/saveMap';
                        
                        let response = await fetch(url, {method:'POST', body: fd})
                        let result = await response.json();
                        
                        if (response.ok && result.isSuccess) {
                            createSprite(x, y, result.data.filePath);
                            $('.modal').remove();
                        }
                    })
                };
            
                loader.show();
                reader.readAsDataURL(file);
                break;
            default:
                createSprite(x, y, globalDraggingItem.dataset.src);
                break;
        }
        
        globalDraggingItem = undefined;
    }

    // Функция для создания спрайта на сцене PIXI.js
    function createSprite(x, y, src) {
        const sprite = PIXI.Sprite.from("/" + src);
        sprite.anchor.set(0.5);
        sprite.position.set(x, y);
        sceneObjectsContainer.addChild(sprite);

        return sprite;
    }
}

function processInterface() {
    //Переключение между сценами
    $(document).on('click', '.nav-scene', async function (event) {
        if ($(this).hasClass('selected') || !$(this).hasClass('nav-scene')) return;

        let loader = new Loader();
        loader.show();

        let selected = Array.from($('.tabs > .tab')).find(item => item.classList.contains('selected'));
        $(selected).removeClass('selected');

        $(this).addClass('selected');

        await changeScene(selected?.dataset?.id, this.dataset.id);
        loader.hide();
    });

    document.querySelectorAll('.nav-scene').forEach(async (item) => {
        if ($(item).hasClass('selected')) {
            await changeScene(null, item.dataset.id);
        }
    });

    //Сохранение сцены
    $('.export-scene').on('click', async () => await saveScene($('.selected')[0].dataset.id));

    //Добавление сцены
    $('#add-scene').on('click', function () {
        let modal = createBaseModal(35, 5);
        $(modal).append('<div id="scene-name-label">');
        $('#scene-name-label').css('text-align', 'center');
        $('#scene-name-label').css('margin-bottom', '1em');
        $('#scene-name-label').text('Введите название сцены');

        $(modal).append('<div id="scene-name-input-div">');
        $('#scene-name-input-div').append('<input id="scene-name-input" type="text" name="sceneName" class="form-control-lg input-field"/>');
        $('#scene-name-input-div').css('display', 'flex');
        $('#scene-name-input-div').css('gap', '1em');

        $('#scene-name-input').css('font-size', '16px');
        $('#scene-name-input').css('width', '80%');

        $('#scene-name-input-div').append('<button id="save-btn" class="btn">');
        $('#save-btn').css('width', '20%');
        $('#save-btn').css('font-size', '16px');
        $('#save-btn').text("Сохранить");

        $('#save-btn').on('click', async function () {
            let value = $('#scene-name-input').val();
            let fd = new FormData();

            fd.append('name', value);
            let response = await fetch('/lobby/createScene', {method: 'POST', body: fd});
            let result = await response.json();

            if (response.ok && result.isSuccess) {
                let tab = $('<div class="tab nav-scene" data-id="' + result.data.id + '">');
                let p = $('<p>');
                p.text(result.data.name);
                tab.append(p);
                $('.modal').remove();

                let lastNav = $('.left.tabs').find('.tab.nav-scene').last();
                if (lastNav.length == 0) {
                    $('#add-scene').before(tab);
                } else lastNav.after(tab);

                tab.click();
            }
        });
    });

    $(document).on('dragstart', '.list-item-child_transportable', function (event) {
        globalDraggingItem = this;
        console.log('drag started');
    });

    async function saveScene(id) {
        if (id == null) return;
        let raw = getSceneData();
        let json = JSON.stringify(raw);
        let sceneId = id ?? $('.selected')[0].dataset.id;
        let fd = new FormData();
        fd.append('json', json);
        fd.append('sceneId', sceneId);

        let response = await fetch('/lobby/saveScene', {method: 'POST', body: fd});
        let result = await response.json();

        if (response.ok && result.isSuccess) {
            console.log('Scene saved!');
        }
    }

    // Собирает и возвращает все объекты сцены
    function getSceneData() {
        let sceneData = {};

        sceneData.graphics = [];
        let children = sceneObjectsContainer.children;

        for (let i = 0; i < children.length; i++) {
            if (children[i] instanceof PIXI.Sprite) {
                let sprite = {};
                sprite.texture = children[i].texture.textureCacheIds[0];
                sprite.position = {x: children[i].x, y: children[i].y};
                sceneData.graphics.push(sprite);
            }
        }

        return sceneData;
    }

    // уничтожает все объекты сцены
    function destroyScene() {
        app.stage.removeChild(sceneObjectsContainer);
        sceneObjectsContainer.destroy(true);
        sceneObjectsContainer = createObjectsContainer();
        app.stage.addChild(sceneObjectsContainer);
    }

    function restoreScene(sceneDataJson) {
        if (sceneDataJson === "") return;
        let sceneData = JSON.parse(sceneDataJson);
        // спрайты
        if (sceneData.graphics) {
            for (let i = 0; i < sceneData.graphics.length; i++) {
                let texture = PIXI.Texture.from(sceneData.graphics[i].texture);
                let sprite = new PIXI.Sprite(texture);
                sprite.position.set(sceneData.graphics[i].position.x, sceneData.graphics[i].position.y);
                sprite.anchor.set(0.5);
                sceneObjectsContainer.addChild(sprite);
            }
        }
        
        console.log('Scene restored');
    }

    // переключить текущую сцену на новую
    // from - guid сцены с которой переключаемся
    // to - guid сцены на которую переключаемся
    async function changeScene(from, to) {
        let fd = new FormData();
        fd.append("id", to);

        let response = await fetch('/lobby/getScene', {method: 'POST', body: fd});
        let result = await response.json();

        if (response.ok && result.isSuccess) {
            let scene = result.data;
            if (from != null) {
                await saveScene(from);
                destroyScene();
            }

            restoreScene(scene.data);
        }
    }
}

// разметка
function createGrid(num) {
    let grid = new PIXI.Graphics();
    grid.lineStyle(1, 0x808080, 1);

    // Рисуем вертикальные линии
    for (let i = 0; i <= 2000; i += num) {
        grid.moveTo(i, 0);
        grid.lineTo(i, 2000);
    }

    // Рисуем горизонтальные линии
    for (let j = 0; j <= 2000; j += num) {
        grid.moveTo(0, j);
        grid.lineTo(2000, j);
    }

    return grid;
}

// Базовое пустое модальное окно.
// Родитель - .modal, контент - .modal-content. 
// Изначально по центру. 
// width и height измеряются в em
function createBaseModal(width, height) {
    let modal = document.createElement('div');
    let modalContent = document.createElement('div');
    modalContent.classList.add('modal-content');
    modal.classList.add('modal');
    modalContent.style.width = width == null ? '70em' : width + 'em';
    modalContent.style.height = height == null ? '5em' : height + 'em';

    modal.appendChild(modalContent);

    document.body.appendChild(modal);

    window.onclick = function (event) {
        if (event.target === modal) {
            modal.style.display = 'none';
            document.body.removeChild(modal);
        }
    };

    modal.style.display = 'block';
    return modalContent;
}

$(document).on('change', '.input-file input[type=file]', function () {
    let dt = new DataTransfer();
    let $files_list = $(this).closest('.input-file').next();
    $files_list.empty();

    for (let i = 0; i < this.files.length; i++) {
        let file = this.files.item(i);
        dt.items.add(file);

        let reader = new FileReader();
        reader.readAsDataURL(file);

        reader.onloadend = function () {
            let new_file_input = '<div class="input-file-list-item">' +
                '<img class="input-file-list-img" src="' + reader.result + '">' +
                '<span class="input-file-list-name">' + file.name + '</span>' +
                '<a href="#" onclick="removeFilesItem(this); return false;" class="input-file-list-remove">x</a>' +
                '</div>';
            $files_list.append(new_file_input);
        }
    }

    this.files = dt.files;
});

function removeFilesItem(target) {
    let name = $(target).prev().text();
    let input = $(target).closest('.input-file-row').find('input[type=file]');
    $(target).closest('.input-file-list-item').remove();
    for (let i = 0; i < dt.items.length; i++) {
        if (name === dt.items[i].getAsFile().name) {
            dt.items.remove(i);
        }
    }
    input[0].files = dt.files;
}

// Функция для сжатия размера изображения
async function compressImage(file, maxSizeInBytes, callback) {
    return new Promise((resolve) => {
        const reader = new FileReader();

        reader.onload = function (event) {
            const img = new Image();

            img.onload = function () {
                const canvas = document.createElement('canvas');
                const ctx = canvas.getContext('2d');
                let width = img.width;
                let height = img.height;

                // Если размер файла меньше или равен maxSizeInBytes, вызываем callback без изменений
                if (file.size <= maxSizeInBytes) {
                    resolve(callback(file));
                    return;
                }

                // Рассчитываем новые размеры изображения для сжатия
                const maxDimension = Math.max(width, height);
                if (maxDimension > 1024) {
                    const ratio = 1024 / maxDimension;
                    width *= ratio;
                    height *= ratio;
                }

                canvas.width = width;
                canvas.height = height;

                // Рисуем изображение на канвасе
                ctx.drawImage(img, 0, 0, width, height);

                // Преобразуем изображение на канвасе обратно в Blob (файл)
                canvas.toBlob(function (blob) {
                    const compressedFile = new File([blob], file.name, {type: 'image/jpeg', lastModified: Date.now()});

                    // Вызываем callback с сжатым файлом
                    resolve(callback(compressedFile));
                }, 'image/png', 0.8);
            };

            img.src = event.target.result;
        };

        reader.readAsDataURL(file);
    });
}