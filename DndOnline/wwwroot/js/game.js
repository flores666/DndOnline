$(document).ready(function () {
    let game = $('.game');
    const maxFPS = 30;
    let app = new PIXI.Application({
        width: game.innerWidth(),
        height: game.innerHeight(),
        antialias: true,
        maxFPS: maxFPS
    });

    game.append(app.view);

    processInterface(game, app);
    processGame(game, app);
});

function processGame(container, app) {
    // Задаем начальный масштаб
    let startScale = {x: 1, y: 1};

    let isSceneDragging = false;
    let isSpriteDragging = false;
    let prevX = 0;
    let prevY = 0;

    container.on('resize', function () {
        app.renderer.resize(container.innerWidth(), container.innerHeight());
    })

    let grid = createGrid(50);

    app.stage.addChild(grid);

    app.view.addEventListener('mousedown', (event) => {
        isSceneDragging = true;
        prevX = event.clientX;
        prevY = event.clientY;
    });

    app.view.addEventListener('mouseup', () => {
        isSceneDragging = false;
    });

    app.view.addEventListener('mousemove', (event) => dragScene(event));

    // Добавляем обработчик события колеса мыши для зума
    app.view.addEventListener('wheel', (event) => zoomScene(event));

    // Создаем контейнер для хранения изображений
    // let imagesContainer = new PIXI.Container();
    // app.stage.addChild(imagesContainer);

    // Добавляем обработчики событий для перетаскивания изображений
    app.view.addEventListener('dragover', (event) => {
        event.preventDefault();
    });

    app.view.addEventListener('drop', (event) => processFilePasting(event));

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
    function processFilePasting(event) {
        event.preventDefault();
        // Получаем координаты события
        const x = event.clientX - app.view.getBoundingClientRect().left;
        const y = event.clientY - app.view.getBoundingClientRect().top;

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
                imagesContainer.addChild(imageSprite);
            }
        })
    }
}

function processInterface(container, app) {
    $(document).on('click', '.tab', function (event) {
        if ($(this).hasClass('selected') || !$(this).hasClass('nav-scene')) return;

        let selected = Array.from($('.tabs > .tab')).find(item => item.classList.contains('selected'));
        $(selected).removeClass('selected');

        $(this).addClass('selected');

        let bcg = PIXI.Sprite.from(this.dataset.src);
        app.stage.addChild(bcg);

    });
    
    $('.export-scene').on('click', async function () {
        let raw = getSceneData();
        let json = JSON.stringify(raw);
        let sceneId = $('.selected')[0].dataset.id;
        let fd = new FormData();
        fd.append('json', json);
        fd.append('sceneId', sceneId);

        let res = await fetch('/lobby/saveScene', {method: 'POST', body: fd});

        app.stage.children.forEach(child => child instanceof PIXI.Sprite ? child.destroy() : child);
        let data = JSON.parse(json);
        
        //restoreScene(data);
    });

    function getSceneData() {
        let sceneData = {};
        let sprite = {};

        sceneData.graphics = [];
        app.stage.children.forEach(child => {
            if (child instanceof PIXI.Sprite) {
                sprite.texture = child.texture.baseTexture.cacheId;
                sprite.position = { x: child.x, y: child.y };
                sceneData.graphics.push(sprite);
            }
        });

        return sceneData;
    }

    function restoreScene(sceneData) {
        // спрайты
        sceneData.graphics.forEach(spriteData => {
            let texture = PIXI.Texture.from(spriteData.texture.baseTexture.cacheId);
            let sprite = new PIXI.Sprite(texture);
            sprite.position.set(spriteData.position.x, spriteData.position.y);
            app.stage.addChild(sprite);
        });
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
// Изначально по центру
function createBaseModal() {
    let modal = document.createElement('div');
    let modalContent = document.createElement('div');
    modalContent.classList.add('modal-content');
    modal.classList.add('modal');
    modalContent.style.width = '70em';

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
    };

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
            }
            ajaxOptions.contentType = "application/json";
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

const replacerFunc = () => {
    const visited = new WeakSet();
    return (key, value) => {
        if (typeof value === "object" && value !== null) {
            if (visited.has(value)) {
                return;
            }
            visited.add(value);
        }
        return value;
    };
};