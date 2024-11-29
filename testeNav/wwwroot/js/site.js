function showSidebar() {
    const sidebar = document.querySelector('#sidebar-mobile');
    sidebar.style.display = 'flex';
}

function hideSidebar() {
    const sidebar = document.querySelector('#sidebar-mobile');
    sidebar.style.display = 'none';
}

var slideIndex = 0;
showSlides();

function showSlides() {
    let i;
    let slides = document.getElementsByClassName("mySlides");
    for (i = 0; i < slides.length; i++) {
        slides[i].style.display = "none";
    }
    slideIndex++;
    if (slideIndex > slides.length) { slideIndex = 1; }
    slides[slideIndex - 1].style.display = "block";
    setTimeout(showSlides, 5000);
}

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
