window.jsInterop = {
    focusElement : function (element) {
        element.focus();
        console.log(element);
    },
    closeWindow : function () {
        window.close();
    }
}