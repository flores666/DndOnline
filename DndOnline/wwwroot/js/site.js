document.addEventListener("DOMContentLoaded", function () {
    handleCard3d(3);

    $('#lobby_search').donetyping(function () {
        let input = $('#lobby_search').val();
        $.ajax({
            url: "/Home/SearchLobby",
            type: "GET",
            data: {input: input},
            success: function (data) {
                $(".lobby-list-container").html(data);
            },
            error: function () {
                console.error("Error during search");
            }
        });
    });

    $('#new_lobby').on('click', function () {
        window.location.href = '/lobby-constructor';
    });
});

$(document).on('click', '.lobby-list-item', function () {
    let id = this.id;
    window.location.href = '/lobby/' + id;
});

$(document).on('input', 'textarea', function () {
    this.style.height = 'auto';
    this.style.height = (this.scrollHeight + 2) + 'px';
});

//скрытие модального окна
$(document).on('click', '.modal', function (event) {
    if (event.target === this) {
        this.style.display = 'none';
        document.body.removeChild(this);
    }
});

async function loadPartialContentAsync(url, container) {
    $.ajax({
        url: url,
        method: 'GET',
        success: function (html) {
            container.html(html);
        },
        error: function (error) {
            console.error('Error fetching content:', error);
        }
    });
}

async function getPartialContentAsync(url) {
    try {
        const html = await $.ajax({
            url: url,
            method: 'GET',
            async: true
        });

        return html;
    } catch (error) {
        console.error('Error fetching content:', error);
        return null;
    }
}

function handleCard3d(THRESHOLD) {
    const cards = document.querySelectorAll(".card3d");
    const motionMatchMedia = window.matchMedia("(prefers-reduced-motion)");

    function handleHover(e) {
        const {clientX, clientY, currentTarget} = e;
        const {clientWidth, clientHeight, offsetLeft, offsetTop} = currentTarget;
        const horizontal = (clientX - offsetLeft) / clientWidth;
        const vertical = (clientY - offsetTop) / clientHeight;
        const rotateX = (THRESHOLD / 2 - horizontal * THRESHOLD).toFixed(2);
        const rotateY = (vertical * THRESHOLD - THRESHOLD / 2).toFixed(2);

        currentTarget.style.transform = `perspective(${clientWidth}px) rotateX(${rotateY}deg) rotateY(${rotateX}deg) scale3d(1, 1, 1)`;
    }

    function resetStyles(e) {
        e.currentTarget.style.transform = `perspective(${e.currentTarget.clientWidth}px) rotateX(0deg) rotateY(0deg)`;
    }

    if (!motionMatchMedia.matches) {
        cards.forEach(function (card) {
            card.addEventListener("mousemove", handleHover);
            card.addEventListener("mouseleave", resetStyles);
        });
    }
}

// Базовое пустое модальное окно.
// Родитель - .modal, конент - .modal-content. 
// Изначально по центре
function createBaseModal() {
    let modal = document.createElement('div');
    let modalContent = document.createElement('div');
    modalContent.classList.add('modal-content');
    modal.classList.add('modal');
    modalContent.style.width = '70em';

    // let closeBtn = document.createElement('span');
    // closeBtn.classList.add('close-modal');
    // closeBtn.innerHTML = '&times;';
    //
    // modalContent.appendChild(closeBtn);
    modal.appendChild(modalContent);

    document.body.appendChild(modal);

    // closeBtn.onclick = function () {
    //     modal.style.display = 'none';
    //     document.body.removeChild(modal);
    // };

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
    ;
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

$.fn.getLabelValue = function (fieldName) {
    let label = this.find(`[for="${fieldName}"]`);

    return label.length ? label.text() : null;
};

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
