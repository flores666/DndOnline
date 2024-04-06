let globalDraggingItem;
let sceneObjectsContainer;

$(document).ready(function () {
    let game = $('.game');
    const maxFPS = 30;
    let app = new PIXI.Application({
        antialias: true,
        maxFPS: maxFPS,
        resizeTo: window
    });

    sceneObjectsContainer = createObjectsContainer();
    game.append(app.view);

    processInterface(game, app);
    processGame(game, app);
    app.renderer.render(app.stage);
});

function createObjectsContainer() {
    let container = new PIXI.Container();
    return container;
}

function processGame(container, app) {
    // Задаем начальный масштаб
    let startScale = {x: 1, y: 1};

    let isSceneDragging = false;
    let isSpriteDragging = false;
    let prevX = 0;
    let prevY = 0;

    container.on('resize', function () {
        app.renderer.resize(container.innerWidth(), container.innerHeight());
        app.view.resizeTo(container.innerWidth(), container.innerHeight());
    })

    let grid = createGrid(50);

    // app.stage.addChild(grid);
    
    app.view.addEventListener('mousedown', (event) => {
        isSceneDragging = true;
        prevX = event.clientX;
        prevY = event.clientY;
        document.body.classList.add('pointer');
    });

    app.view.addEventListener('mouseup', () => {
        isSceneDragging = false;
        document.body.classList.remove('pointer');
    });

    app.view.addEventListener('mousemove', (event) => dragScene(event));

    // Добавляем обработчик события колеса мыши для зума
    app.view.addEventListener('wheel', (event) => zoomScene(event));

    // Создаем контейнер для хранения изображений
    app.stage.addChild(sceneObjectsContainer);

    // Добавляем обработчики событий для перетаскивания изображений
    app.view.addEventListener('dragover', (event) => {
        event.preventDefault();
    });

    app.view.addEventListener('drop', (event) => handlePasting(event));

    //перемещение по сцене курсором
    function dragScene(event) {
        if (isSceneDragging) {
            const deltaX = event.clientX - prevX;
            const deltaY = event.clientY - prevY;

            // Изменяем позицию сцены на основе движения мыши
            // Ограничиваем перемещение по X
            app.stage.x = Math.min(0, Math.max(app.stage.x + deltaX, app.renderer.width - app.stage.width));
            // Ограничиваем перемещение по Y
            app.stage.y = Math.min(0, Math.max(app.stage.y + deltaY, app.renderer.height - app.stage.height));

            // Обновляем предыдущие координаты
            prevX = event.clientX;
            prevY = event.clientY;
        }
    }

    // обработка зума
    function zoomScene(event) {
        // Определяем направление вращения колеса мыши (вверх или вниз)
        const delta = event.deltaY > 0 ? -0.1 : 0.1;

        // Увеличиваем или уменьшаем масштаб
        startScale.x += delta;
        startScale.y += delta;

        // Ограничиваем масштаб, чтобы избежать слишком большого или маленького значения
        startScale.x = Math.max(0.1, Math.min(3, startScale.x));
        startScale.y = Math.max(0.1, Math.min(3, startScale.y));

        // Проверяем, чтобы не отдалить сцену за пределы исходных размеров
        if (app.stage.width * startScale.x >= app.renderer.width) {
            app.stage.scale.x = startScale.x;
        }

        if (app.stage.height * startScale.y >= app.renderer.height) {
            app.stage.scale.y = startScale.y;
        }

    }

    // вставка спрайта извне путем drag and drop
    function handlePasting(event) {
        event.preventDefault();

        // Получаем координаты события
        const x = event.clientX - app.view.getBoundingClientRect().left;
        const y = event.clientY - app.view.getBoundingClientRect().top;

        switch (typeof globalDraggingItem) {
            case "undefined" :
                // Получаем информацию о перетаскиваемых элементах
                const file = event.dataTransfer.files[0];
                let modal = createBaseModal();

                $(modal).on('click', '.btn', function () {
                    // Добавляем изображения на сцену
                    if (file.type.startsWith('image/')) {
                        // Создаем спрайт для изображения
                        const imageSprite = PIXI.Sprite.from(file.path);
                        // Устанавливаем позицию спрайта в место, где был сделан drop
                        imageSprite.position.set(x, y);
                        // Добавляем спрайт в контейнер
                        sceneObjectsContainer.addChild(imageSprite);
                    }
                })
                break;
            default:
                createSprite(x, y, globalDraggingItem.dataset.src);
                globalDraggingItem = undefined;
                break;
        }
    }

    // Функция для создания спрайта на сцене PIXI.js
    function createSprite(x, y, src) {
        const sprite = PIXI.Sprite.from("/" + src);
        sprite.position.set(x, y);
        sceneObjectsContainer.addChild(sprite);
        return sprite;
    }
}

function processInterface(container, app) {
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

    // Собирает и возвращает все объекты сцены а затем уничтожает сцену.
    function getSceneDataAndDestroy() {
        let sceneData = {};

        sceneData.graphics = [];
        app.stage.children.forEach(child => {
            if (child instanceof PIXI.Sprite) {
                let sprite = {};
                sprite.texture = child.texture.baseTexture.cacheId;
                sprite.position = {x: child.x, y: child.y};
                sceneData.graphics.push(sprite);
                child.destroy();
            }
        });

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

// method - POST/GET
// url - адрес до точки
// data - отправляемые данные. Объект либо FormData
async function sendRequestAsync(method, url, data) {
    return new Promise((resolve, reject) => {
        let ajaxOptions = {
            url: url,
            method: method,
            async: true,
            success: function (response) {
                resolve(response);
            },
            error: function (error) {
                reject(error);
            }
        };

        // Проверяем, есть ли данные в formData
        if (data) {
            if (data.getAll && data.getAll.length > 0) {
                ajaxOptions.processData = false;
                ajaxOptions.contentType = false;
            } else {
                ajaxOptions.contentType = "application/json";
            }

            ajaxOptions.data = data;
        }

        $.ajax(ajaxOptions);
    });
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
                }, 'image/jpeg', 0.8);
            };

            img.src = event.target.result;
        };

        reader.readAsDataURL(file);
    });
}