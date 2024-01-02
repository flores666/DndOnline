﻿document.addEventListener("DOMContentLoaded", function () {
    const cards = document.querySelectorAll(".lobby-item");
    const motionMatchMedia = window.matchMedia("(prefers-reduced-motion)");
    const THRESHOLD = 10;

    function handleHover(e) {
        const { clientX, clientY, currentTarget } = e;
        const { clientWidth, clientHeight, offsetLeft, offsetTop } = currentTarget;

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
});
