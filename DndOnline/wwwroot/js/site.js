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

    navigationLinks.click(function (event) {
        event.preventDefault();
        let url = $(this).attr('href');
        loadPageContent(url);
    });

    function loadPageContent(url) {
        $.ajax({
            url: url,
            method: 'GET',
            success: function (html) {
                $('.lobby-creation-content').html(html);
            },
            error: function (error) {
                console.error('Error fetching content:', error);
            }
        });
    }
});

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