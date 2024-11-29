var menuopen = document.getElementById('menuopen');
var menuclose = document.getElementById('menuclose');
var sidebarResp = document.getElementById('sidebarResponsive');

function menuOpen() {
    sidebarResp.style.display = 'flex'
}

function menuclose() {
    sidebarResp.style.display = 'none'
}
function confirmarDelecao() {
    return confirm("Tem certeza que deseja deletar este produto?");
}
