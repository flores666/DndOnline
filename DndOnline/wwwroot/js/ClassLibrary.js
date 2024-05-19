class Loader {
    show() {
        let container = document.createElement('div');
        container.className = 'loader-container';
        let loader = document.createElement('span');
        loader.className = 'loader';
        container.append(loader);
        document.body.appendChild(container);
    }

    hide() {
        $('.loader-container').remove();
    }
}

(function ($) {
    $.fn.extend({
        donetyping: function (callback, timeout) {
            timeout = timeout || 500; // 500 ms default timeout
            let timeoutReference,
                doneTyping = function (el) {
                    if (!timeoutReference) return;
                    timeoutReference = null;
                    callback.call(el);
                };
            return this.each(function (i, el) {
                let $el = $(el);
                // Chrome Fix (Use keyup over keypress to detect backspace)
                // thank you @palerdot
                $el.is(':input') && $el.on('keyup keypress paste', function (e) {
                    // This catches the backspace button in chrome, but also prevents
                    // the event from triggering too preemptively. Without this line,
                    // using tab/shift+tab will make the focused element fire the callback.
                    if (e.type == 'keyup' && e.keyCode != 8) return;

                    // Check if timeout has been set. If it has, "reset" the clock and
                    // start over again.
                    if (timeoutReference) clearTimeout(timeoutReference);
                    timeoutReference = setTimeout(function () {
                        // if we made it here, our timeout has elapsed. Fire the
                        // callback
                        doneTyping(el);
                    }, timeout);
                }).on('blur', function () {
                    // If we can, fire the event since we're leaving the field
                    doneTyping(el);
                });
            });
        }
    });
})(jQuery);

//callback вызывается при перемещении
(function ($) {
    $.fn.imageCropper = function (callback) {
        let thumbnailSize;
        let imgWidth;
        let imgHeight;
        
        return this.each(function () {
            let $this = $(this);
            let $img = $this.find('img');
            imgWidth = Math.round($img.width());
            imgHeight = Math.round($img.height());
            
            let $overlay = $('<div class="overlay"></div>');
            let $cropFrame = $('<div class="crop-frame"></div>');
            thumbnailSize = Math.min(imgWidth, imgHeight);
            
            $overlay.css({
                'position': 'absolute',
                'top': $img.position().top,
                'left': $img.position().left,
                'width': imgWidth + 'px',
                'height': imgHeight + 'px',
                'pointer-events': 'none',
                'box-sizing': 'border-box'
            });

            $cropFrame.css({
                'position': 'absolute',
                'top': (imgHeight / 2) - (thumbnailSize / 2) + 'px',
                'left': (imgWidth / 2) - (thumbnailSize / 2) + 'px',
                'border': '1px solid rgba(100, 100, 100, 0.5)',
                'border-radius': '50%',
                'width': thumbnailSize + 'px',
                'height': thumbnailSize + 'px',
                'pointer-events': 'auto',
                'box-shadow': '0 0 0 9999px rgba(0, 0, 0, 0.5)',
                'cursor': 'pointer'
            });

            $overlay.append($cropFrame);
            $this.append($overlay);

            $cropFrame.draggable({
                containment: $img,
                drag: function (event, ui) {
                    if (ui.position.left < 0) {
                        ui.position.left = 0;
                    }
                    if (ui.position.top < 0) {
                        ui.position.top = 0;
                    }
                    
                    let imgNaturalWidth = $img.get(0).naturalWidth;
                    let imgNaturalHeight = $img.get(0).naturalHeight;
                    let cropFramePosition = $cropFrame.position();
                    let scaleX = imgNaturalWidth / imgWidth;
                    let scaleY = imgNaturalHeight / imgHeight;
                    let cropData = {
                        x: cropFramePosition.left * scaleX,
                        y: cropFramePosition.top * scaleY,
                        width: thumbnailSize * scaleX,
                        height: thumbnailSize * scaleY
                    };

                    callback(cropData);
                }
            });
        });
    };
})(jQuery);
