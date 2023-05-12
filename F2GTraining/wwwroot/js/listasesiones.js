function muestraOpciones(idjugador) {
    console.log(idjugador)
    var divisor = $("#sessionopt-" + idjugador).slideToggle(250);
}

function vuelveInicio() {
    window.location.href = "/equipos/MenuEquipo";
}

function crearSesion(idequipo) {
    document.getElementsByClassName("view-sessions-zone")[0].style.display = "none";
    Swal.fire({
        title: "Nombre de la Sesion",
        input: "text",
        background: '#111111',
        color: "#CFC0FF",
        showCancelButton: true,
        confirmButtonText: "Crear Sesion",
        cancelButtonText: "Cancelar",
        allowOutsideClick: () => {
            return false
        }
    }).then(resultado => {
        console.log(resultado)
        if (resultado.isConfirmed) {
            $('#inputnombre').val(resultado.value);
            $('#form-new-team').submit();
        }
        else {
            document.getElementsByClassName("view-sessions-zone")[0].style.display = "block"
        }
    });
}

    