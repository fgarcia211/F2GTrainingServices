function muestraArchivoSubido() {

    var rutaimagen = document.getElementById('inputimagen').value
    var texto = document.getElementById("image-info");

    texto.style.color = "white";

    if (rutaimagen != "") {
        texto.innerText = "Nombre de imagen seleccionada: " + rutaimagen.split("\\")[rutaimagen.split("\\").length - 1];
    } else {
        texto.innerText = "Para registrar el equipo, selecciona una imagen (Solo formato .png)"
    }
}

function vuelveInicio() {
    window.location.href = "/equipos/MenuEquipo";
}
