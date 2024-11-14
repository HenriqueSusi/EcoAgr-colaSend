function loadFile(event) {
    var iconPlaceholder = document.getElementById('iconPlaceholder');
    var image = document.createElement('img');
    image.src = URL.createObjectURL(event.target.files[0]);
    iconPlaceholder.innerHTML = '';
    iconPlaceholder.appendChild(image);
}

let count = 0;
function increment(counterId) {
    const counterElement = document.getElementById(counterId);
    let count = parseInt(counterElement.innerText);
    count++;
    counterElement.innerText = count;
}

function decrement(counterId) {
    const counterElement = document.getElementById(counterId);
    let count = parseInt(counterElement.innerText);
    if (count > 0) {
        count--;
    }
    counterElement.innerText = count;
}