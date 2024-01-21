document.addEventListener("DOMContentLoaded", function () {
    handleCard3d(3);

    $('#lobby_search').on('input', function () {
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

    $('.lobby-list-item').on('click', function () {
        let id = this.id;
        window.location.href = '/lobby/' + id;
    });

    let navigationLinks = $('.lobby-creation-nav a');

    navigationLinks.click(async function (event) {
        event.preventDefault();
        let url = $(this).attr('href');
        await loadPartialContentAsync(url, $('.lobby-creation-content'));
    });

});

$(document).on('input', '.auto-textarea', function () {
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