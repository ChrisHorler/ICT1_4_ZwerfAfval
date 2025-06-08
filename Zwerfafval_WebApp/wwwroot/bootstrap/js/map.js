window.initMap = () => {
    var map = L.map('map').setView([51.571915, 4.768323], 13); // Breda
    L.tileLayer('https://api.maptiler.com/maps/nl-cartiqo-topo/{z}/{x}/{y}.png?key=mvRA2CGG1T8gvSWGDugz', {
        attribution: '<a href="https://cartiqo.nl/" target="_blank">© Cartiqo</a> <a href="https://www.maptiler.com/copyright/" target="_blank">© MapTiler</a> <a href="https://www.openstreetmap.org/copyright" target="_blank">© OpenStreetMap contributors</a>',
    }).addTo(map);
}
