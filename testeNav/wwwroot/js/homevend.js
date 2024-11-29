// Pegando elementos
var modal = document.getElementById("editModal");
var btn = document.getElementById("openModalButton");
var span = document.getElementsByClassName("close")[0];
var closeButton = document.getElementById("closeModalButton");

// Quando o usuário clicar no botão, abre a modal
btn.onclick = function () {
    modal.style.display = "block";
}

// Quando o usuário clicar no "X" (botão de fechar), fecha a modal
span.onclick = function () {
    modal.style.display = "none";
}

// Quando o usuário clicar no botão de "Fechar", fecha a modal
closeButton.onclick = function () {
    modal.style.display = "none";
}

// Quando o usuário clicar em qualquer lugar fora da modal, fecha a modal
window.onclick = function (event) {
    if (event.target == modal) {
        modal.style.display = "none";
    }
}

$(document).ready(function () {
    // Ao clicar no botão de salvar
    $("#saveButton").click(function () {
        // Captura o formulário de edição
        var formData = new FormData($('#editProfileForm')[0]);

        // Faz uma requisição AJAX para enviar os dados
        $.ajax({
            url: '/HomeVendedor/EditarPerfil', // URL para onde os dados serão enviados
            type: 'POST', // Método HTTP
            data: formData, // Dados do formulário
            contentType: false, // Impede o jQuery de definir o tipo de conteúdo automaticamente
            processData: false, // Não processa os dados (mantém o formato do FormData)
            success: function (response) {
                // Caso a requisição seja bem-sucedida, redireciona para a página inicial
                window.location.href = '/HomeVendedor/Index';
            },
            error: function (xhr, status, error) {
                // Em caso de erro, mostre uma mensagem
                alert('Erro ao salvar as alterações.');
            }
        });
    });
});