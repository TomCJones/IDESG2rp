function resizeFooterElementWidth(element) {
    var width = 150;
    var body = window.document.body;
    if (windows.innerWidth) {
        width = min(windows.innerWidth / 3, 150)
    }
    element.style.width = width;
}
