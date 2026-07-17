// Calculadora en vivo de la cotización (HU-COT-001).
//
// El cálculo autoritativo vive en el servidor (QuoteService): esto es solo un
// espejo para que el asesor vea la tarifa mientras digita. Al guardar, el
// servidor recalcula y sus montos son los que mandan.
(function () {
    'use strict';

    const form = document.getElementById('formCotizacion');
    if (!form) return;

    const campos = {
        origen: document.getElementById('CostoOrigen'),
        aduana: document.getElementById('CostoTramitesAduana'),
        flete: document.getElementById('CostoFlete'),
        destino: document.getElementById('CostoDestino'),
        seguro: document.getElementById('chkSeguro'),
        valorDeclarado: document.getElementById('ValorDeclarado'),
        porcentaje: document.getElementById('PorcentajeSeguro')
    };

    const salidas = {
        subtotal: document.getElementById('outSubtotal'),
        seguro: document.getElementById('outSeguro'),
        total: document.getElementById('outTotal'),
        letras: document.getElementById('outLetras'),
        lineaSeguro: document.getElementById('lineaSeguro'),
        seguroCampos: document.getElementById('seguroCampos')
    };

    const formatoUSD = new Intl.NumberFormat('en-US', {
        minimumFractionDigits: 2,
        maximumFractionDigits: 2
    });

    function moneda(valor) {
        return 'US$' + formatoUSD.format(valor);
    }

    function num(input) {
        if (!input) return 0;
        const v = parseFloat(input.value);
        return isNaN(v) || v < 0 ? 0 : v;
    }

    // Mismo redondeo comercial que QuoteService.Redondear.
    function redondear(v) {
        return Math.round((v + Number.EPSILON) * 100) / 100;
    }

    function calcular() {
        const subtotal = redondear(
            num(campos.origen) + num(campos.aduana) + num(campos.flete) + num(campos.destino)
        );

        const incluyeSeguro = campos.seguro && campos.seguro.checked;
        const valorDeclarado = num(campos.valorDeclarado);
        const porcentaje = num(campos.porcentaje);

        let montoSeguro = 0;
        if (incluyeSeguro && valorDeclarado > 0) {
            montoSeguro = redondear(valorDeclarado * (porcentaje / 100));
        }

        const total = redondear(subtotal + montoSeguro);

        salidas.subtotal.textContent = moneda(subtotal);
        salidas.seguro.textContent = moneda(montoSeguro);
        salidas.total.textContent = moneda(total);

        // Los campos del seguro solo estorban si no se va a incluir.
        if (salidas.seguroCampos) {
            salidas.seguroCampos.classList.toggle('oculto', !incluyeSeguro);
        }
        if (salidas.lineaSeguro) {
            salidas.lineaSeguro.classList.toggle('oculto', !incluyeSeguro);
        }

        pedirLetras(total);
    }

    // El monto en letras lo arma el servidor para que coincida exactamente con
    // el que va a salir en el machote: no lo duplicamos en JS.
    let temporizador = null;
    let ultimoSolicitado = null;

    function pedirLetras(total) {
        if (!salidas.letras) return;

        if (total <= 0) {
            salidas.letras.textContent = '';
            return;
        }

        clearTimeout(temporizador);

        // Antirrebote: no golpeamos el servidor en cada tecla.
        temporizador = setTimeout(function () {
            if (ultimoSolicitado === total) return;
            ultimoSolicitado = total;

            const token = form.querySelector('input[name="__RequestVerificationToken"]');

            fetch('/Cotizacion/Calcular', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': token ? token.value : ''
                },
                body: JSON.stringify({
                    costoOrigen: num(campos.origen),
                    costoTramitesAduana: num(campos.aduana),
                    costoFlete: num(campos.flete),
                    costoDestino: num(campos.destino),
                    incluyeSeguro: campos.seguro ? campos.seguro.checked : false,
                    valorDeclarado: num(campos.valorDeclarado),
                    porcentajeSeguro: num(campos.porcentaje),
                    // El binder exige los campos requeridos de la entidad aunque
                    // el cálculo no los use.
                    numeroCotizacion: 'CALC',
                    nombreCliente: 'CALC'
                })
            })
                .then(function (r) { return r.ok ? r.json() : null; })
                .then(function (data) {
                    if (data && data.enLetras) {
                        salidas.letras.textContent = '(' + data.enLetras + ')';
                    }
                })
                .catch(function () {
                    // Si falla, el monto numérico ya se ve: no vale la pena
                    // molestar al usuario por el texto en letras.
                    salidas.letras.textContent = '';
                });
        }, 400);
    }

    // Autocompletar datos al elegir un cliente registrado.
    const selCliente = document.getElementById('selCliente');
    const txtNombre = document.getElementById('txtNombreCliente');

    if (selCliente && txtNombre) {
        selCliente.addEventListener('change', function () {
            const opcion = selCliente.options[selCliente.selectedIndex];
            if (selCliente.value) {
                txtNombre.value = opcion.text.trim();
            }
        });
    }

    Object.values(campos).forEach(function (campo) {
        if (!campo) return;
        campo.addEventListener('input', calcular);
        campo.addEventListener('change', calcular);
    });

    calcular();
})();
