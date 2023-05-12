function muestraOpciones(idjugador) {
    console.log(idjugador)
    var divisor = $("#optplayer-" + idjugador).toggle(300);
}

function esconderEquipos() {

    var equipoSelecc = $("#selectorequipo").val()

    var requestEquipo = "/Equipos/_PartialVistaEquipo?idequipo=" + equipoSelecc;
    $("#datosequipo").load(requestEquipo);

    var requestJugadores = "/Jugadores/_PartialJugadoresEquipo?idequipo=" + equipoSelecc;
    $("#datosjugadores").load(requestJugadores);

    $("#equiposeleccionado").val(equipoSelecc);
}
