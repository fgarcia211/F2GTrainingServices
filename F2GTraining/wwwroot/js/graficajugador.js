function vuelveInicio() {
    window.location.href = "/equipos/MenuEquipo";
}

function cargaGrafica(etiquetas, valores) {

    var texto = "GRAFICA DEL JUGADOR";
    var statsamostrar = 6;
    var data = [];

    for (var i = 0; i < etiquetas.length; i++) {
        if (valores[i] != 0) {
            data.push([etiquetas[i], valores[i]]);
        } else {
            statsamostrar--;
        }
    }

    if (statsamostrar == 0) {
        texto = "NO HAY DATOS DISPONIBLES"
    }

    Highcharts.chart("graf-player", {
        chart: {
            backgroundColor: "transparent",
            plotBorderWidth: 0,
            plotShadow: false,
        },
        title: {
            useHTML: true,
            text: texto,
            y: 60,
            style: {
                fontWeight: 'bold',
                color: 'white',
                fontSize: '25px',
                'padding': '10px 50% 10px 50%',
                'background-color': '#000000',
                'text-align': "center"
            }
        },
        plotOptions: {
            pie: {
                dataLabels: {
                    enabled: true,
                    distance: -50,
                    style: {
                        fontWeight: 'bold',
                        color: 'white',
                        fontSize: '16px'
                    }
                },
                startAngle: -90,
                endAngle: 90,
                center: ['50%', '80%'],
                size: '110%'
            }
        },
        series: [{
            type: 'pie',
            name: 'Valoracion',
            innerSize: '40%',
            data: data
        }],
    });

    $("text.highcharts-credits").hide();
}

function descargaInforme(idjugador) {
    window.location.href = "/Jugadores/GraficaComoPDF?idjugador=" + idjugador
}

