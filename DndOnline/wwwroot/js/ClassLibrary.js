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