// Variável para armazenar o mapa e o geocoder
let map;
let geocoder;

function initMap() {
    // Inicializa o mapa com um centro padrão
    map = new google.maps.Map(document.getElementById('map'), {
        center: { lat: -23.55052, lng: -46.633308 }, // São Paulo
        zoom: 8
    });

    // Inicializa o geocoder para converter o endereço em coordenadas
    geocoder = new google.maps.Geocoder();
}

function showLocation() {
    const address = document.getElementById('location-input').value;

    geocoder.geocode({ 'address': address }, function(results, status) {
        if (status === 'OK') {
            // Ajusta o centro do mapa com base nos resultados
            map.setCenter(results[0].geometry.location);
            const marker = new google.maps.Marker({
                map: map,
                position: results[0].geometry.location
            });
        } else {
            alert('Não foi possível localizar o endereço: ' + status);
        }
    });
}

window.onload = initMap;