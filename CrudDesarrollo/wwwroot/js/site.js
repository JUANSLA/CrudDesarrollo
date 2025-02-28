

let sliderInner = document.querySelector(".slider--inner");
let images = sliderInner.querySelectorAll("img");
let index = 0;

// Función para actualizar la posición del slider
function updateSliderPosition() {
    let percentage = index * -100;
    sliderInner.style.transform = "translateX(" + percentage + "%)";
}

// Avance automático cada 3 segundos
setInterval(function () {
    index++;
    if (index >= images.length) {
        index = 0;
    }
    updateSliderPosition();
}, 3000);

// Función para mostrar la imagen siguiente
function nextImage() {
    index++;
    if (index >= images.length) {
        index = 0;
    }
    updateSliderPosition();
}

// Función para mostrar la imagen anterior
function prevImage() {
    index--;
    if (index < 0) {
        index = images.length - 1;
    }
    updateSliderPosition();
}

// Agregar eventos a los botones de navegación
document.getElementById('next').addEventListener('click', nextImage);
document.getElementById('prev').addEventListener('click', prevImage);
