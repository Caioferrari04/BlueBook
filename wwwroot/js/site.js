var connection = new signalR.HubConnectionBuilder().withUrl("/feedhub").build();
var innerChatId = "chat";

connection.on("PedidoAmizade", (idOrigem, idDestino, nomeUsuario) => {
    console.log("3");
    console.log(idOrigem);
    console.log(nomeUsuario);
    var origem = idOrigem;
    var destino = idDestino;

    var modal = document.createElement("div");
    modal.classList = "modal fade";
    modal.id = "pedidoAmizade";
    modal.setAttribute("data-bs-backdrop", "static");
    modal.setAttribute("data-bs-keyboard", "false");
    modal.tabIndex = "-1";
    modal.setAttribute("aria-labelledby", "staticBackdropLabel");
    modal.setAttribute("aria-hidden", "true");

    var modalAmizade = document.createElement("div");
    modalAmizade.className = "modal-dialog";

    var modalContent = document.createElement("div");
    modalContent.className = "modal-content"

    var modalBody = document.createElement("div");
    modalBody.className = "modal-body";
    modalBody.textContent = nomeUsuario + " mandou um pedido de amizade, aceitar?";

    var areaBotoes = document.createElement("div");
    areaBotoes.className = "modal-footer";

    var botaoAceitar = document.createElement("button");
    botaoAceitar.classList = "btn btn-primary btn-info";
    botaoAceitar.id = "aceitar-pedido";
    botaoAceitar.textContent = "Aceitar";
    botaoAceitar.setAttribute("data-bs-dismiss", "modal");
    

    var botaoRecusar = document.createElement("button");
    botaoRecusar.classList = "btn btn-primary btn-danger";
    botaoRecusar.id = "recusar-pedido";
    botaoRecusar.textContent = "Recusar";
    botaoRecusar.setAttribute("data-bs-dismiss", "modal");

    modalContent.appendChild(modalBody);
    areaBotoes.appendChild(botaoAceitar);
    areaBotoes.appendChild(botaoRecusar);
    modalContent.appendChild(areaBotoes);
    modalAmizade.appendChild(modalContent);
    modal.appendChild(modalAmizade);
    document.body.appendChild(modal);
    $('#pedidoAmizade').modal('toggle');
    $('#pedidoAmizade').modal('show');
    botaoAceitar.addEventListener("click", function (event) {
        event.preventDefault();
        console.log("2");
        $('.modal').modal('hide').data('bs.modal', null);
        connection.invoke("aceitarPedidoAmizade", origem, destino).catch(function (err) {
            return console.error(err.toString());
        });
    });
    botaoRecusar.addEventListener("click", function (event) {
        event.preventDefault();
        $('.modal').modal('hide').data('bs.modal', null);
        connection.invoke("recusarPedidoAmizade", origem, destino).catch(function (err) {
            return console.error(err.toString());
        });
    });
});

connection.on("AceitarPedido", (destino) => {

    var modal = document.createElement("div");
    modal.classList = "modal fade";
    modal.id = "pedidoAmizadeAceite";
    modal.setAttribute("data-bs-backdrop", "static");
    modal.setAttribute("data-bs-keyboard", "false");
    modal.tabIndex = "-1";
    modal.setAttribute("aria-labelledby", "staticBackdropLabel");
    modal.setAttribute("aria-hidden", "true");

    var modalAmizade = document.createElement("div");
    modalAmizade.className = "modal-dialog";

    var modalContent = document.createElement("div");
    modalContent.className = "modal-content"

    var modalBody = document.createElement("div");
    modalBody.className = "modal-body";
    modalBody.textContent = destino + " aceitou o pedido de amizade!";

    var areaBotoes = document.createElement("div");
    areaBotoes.className = "modal-footer";

    var botaoOk = document.createElement("button");
    botaoOk.classList = "btn btn-primary btn-info";
    botaoOk.textContent = "Ok";
    botaoOk.setAttribute("data-bs-dismiss", "modal");
    console.log("chegou aqui");
    modalContent.appendChild(modalBody);
    areaBotoes.appendChild(botaoOk);
    modalContent.appendChild(areaBotoes);
    modalAmizade.appendChild(modalContent);
    modal.appendChild(modalAmizade);
    document.body.appendChild(modal);
    $('#pedidoAmizadeAceite').modal('toggle');
    $('#pedidoAmizadeAceite').modal('show');
    botaoOk.addEventListener("click", function (event) {
        event.preventDefault();
        $('.modal').modal('hide').data('bs.modal', null);
    });
});

connection.on("RecusarPedido", (destino) => {
    console.log(destino);
    var modal = document.createElement("div");
    modal.classList = "modal fade";
    modal.id = "pedidoAmizadeRecusado";
    modal.setAttribute("data-bs-backdrop", "static");
    modal.setAttribute("data-bs-keyboard", "false");
    modal.tabIndex = "-1";
    modal.setAttribute("aria-labelledby", "staticBackdropLabel");
    modal.setAttribute("aria-hidden", "true");

    var modalAmizade = document.createElement("div");
    modalAmizade.className = "modal-dialog";

    var modalContent = document.createElement("div");
    modalContent.className = "modal-content"

    var modalBody = document.createElement("div");
    modalBody.className = "modal-body";
    modalBody.textContent = destino + " recusou o pedido de amizade!";
    
    var areaBotoes = document.createElement("div");
    areaBotoes.className = "modal-footer";
    console.log("chegou aqui");
    var botaoOk = document.createElement("button");
    botaoOk.classList = "btn btn-primary btn-info";
    botaoOk.textContent = "Ok";
    botaoOk.setAttribute("data-bs-dismiss", "modal");
    
    modalContent.appendChild(modalBody);
    areaBotoes.appendChild(botaoOk);
    modalContent.appendChild(areaBotoes);
    modalAmizade.appendChild(modalContent);
    modal.appendChild(modalAmizade);
    document.body.appendChild(modal);
    $('#pedidoAmizadeRecusado').modal('toggle');
    $('#pedidoAmizadeRecusado').modal('show');
    botaoOk.addEventListener("click", function (event) {
        event.preventDefault();
        $('.modal').modal('hide').data('bs.modal', null);
    });
});

function criarMensagem(mensagem) {
    var position = mensagem.nomeUsuario === nomeUsuario ? "text-right" : "text-left";
    var alert = mensagem.nomeUsuario === nomeUsuario ? "alert-light" : "alert-dark";

    var message = document.createElement("div");
    message.className = "row";

    var messageCol = document.createElement("div");
    messageCol.classList = "col-md-12 " + position;

    var messageLink = document.createElement("a");
    messageLink.href = "/User/Index/" + mensagem.usuarioID;

    var messageImg = document.createElement("img");
    messageImg.src = mensagem.usuario.linkImagem;
    messageImg.className = "usuario-imagem";

    var messageUser = document.createElement("small");
    messageUser.className = "text-dark";
    messageUser.textContent = " " + mensagem.nomeUsuario + " ";

    var messageAlert = document.createElement("div");
    messageAlert.classList = "alert mb-0 " + alert;

    var messageAlertMessage = document.createElement("div");
    messageAlertMessage.className = "d-block";
    messageAlertMessage.innerHTML = mensagem.texto;

    var messageDt = document.createElement("small")
    messageDt.className = "text-info"
    messageDt.textContent = formatDate(mensagem.dataEnvio)
    
    messageAlert.appendChild(messageAlertMessage);
    messageCol.appendChild(messageLink);
    messageLink.appendChild(messageImg);
    messageLink.appendChild(messageUser);
    messageCol.appendChild(messageAlert);
    messageLink.appendChild(messageDt);
    message.appendChild(messageCol);
    document.getElementById(innerChatId).appendChild(message);
    objDiv.scrollTop = objDiv.scrollHeight;
    console.log("Criou mensagem com sucesso!");
}

connection.start().then(function () {
    console.log("Conectado")
    var mensagens = document.getElementById("SendMessageForm");
    var pedido = document.getElementById("enviarPedido");
    if (mensagens != null) {
        mensagens.addEventListener("submit", function (event) {
            event.preventDefault();

            var data = {
                Texto: document.getElementById("MessageText").value,
                NomeUsuario: nomeUsuario,
                UsuarioID: idAtual
            }

            document.getElementById("MessageText").value = ""
            connection.invoke("enviarMensagem", data).catch(function (err) {
                return console.error(err.toString());
            });
        });
    }
    if (pedido != null) {
        pedido.addEventListener("submit", function (event) {
            event.preventDefault();
            connection.invoke("enviarPedidoAmizade", origem, destino).catch(function (err) {
                return console.error(err.toString());
            });
        });
    }
});