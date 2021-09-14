var connection = new signalR.HubConnectionBuilder().withUrl("/feedhub").build();
var innerChatId = "chat";

connection.on("PedidoAmizade", (idOrigem, idDestino, nomeUsuario) => {
    console.log("3");
    console.log(idOrigem);
    console.log(nomeUsuario);
    var origem = idOrigem;
    var destino = idDestino;
    var modalAmizade = document.createElement("div");
    modalAmizade.className = "modal-amizade";

    var texto = document.createElement("p");
    texto.textContent = nomeUsuario + " mandou um pedido de amizade, aceitar?";

    var areaBotoes = document.createElement("div");
    areaBotoes.className = "area-botoes";

    var botaoAceitar = document.createElement("button");
    botaoAceitar.classList = "btn btn-primary btn-info";
    botaoAceitar.id = "aceitar-pedido";
    botaoAceitar.textContent = "Aceitar";

    var botaoRecusar = document.createElement("button");
    botaoRecusar.classList = "btn btn-primary btn-danger";
    botaoRecusar.id = "recusar-pedido";
    botaoRecusar.textContent = "Recusar";

    console.log(modalAmizade);
    modalAmizade.appendChild(texto);
    areaBotoes.appendChild(botaoAceitar);
    areaBotoes.appendChild(botaoRecusar);
    modalAmizade.appendChild(areaBotoes);
    document.body.appendChild(modalAmizade);
    botaoAceitar.addEventListener("click", function (event) {
        event.preventDefault();
        console.log("2");
        connection.invoke("aceitarPedidoAmizade", origem, destino).catch(function (err) {
            return console.error(err.toString());
        });
        modalAmizade.remove();
    });
    botaoRecusar.addEventListener("click", function (event) {
        event.preventDefault();
        connection.invoke("recusarPedidoAmizade", origem, destino).catch(function (err) {
            return console.error(err.toString());
        });
        modalAmizade.remove();
    });
});

connection.on("AceitarPedido", (destino) => {
    console.log(destino);
    var modalAmizade = document.createElement("div");
    modalAmizade.className = "modal-amizade";

    var texto = document.createElement("p");
    texto.textContent = destino + " aceitou o pedido de amizade!";

    var botaoOk = document.createElement("button");
    botaoOk.classList = "btn btn-primary btn-info";
    botaoOk.id = "fechar-modal";
    botaoOk.textContent = "Ok";

    console.log(modalAmizade);
    modalAmizade.appendChild(texto);
    modalAmizade.appendChild(botaoOk);
    document.body.appendChild(modalAmizade);
    botaoOk.addEventListener("click", function (event) {
        event.preventDefault();
        modalAmizade.remove();
    });
});

connection.on("RecusarPedido", (destino) => {
    console.log(destino);
    var modalAmizade = document.createElement("div");
    modalAmizade.className = "modal-amizade";

    var textoRecusa = document.createElement("p");
    textoRecusa.textContent = destino + " recusou o pedido de amizade!";

    var botaoOk = document.createElement("button");
    botaoOk.classList = "btn btn-primary btn-info";
    botaoOk.id = "fechar-modal";
    botaoOk.textContent = "Ok";

    console.log(modalAmizade);
    modalAmizade.appendChild(textoRecusa);
    modalAmizade.appendChild(botaoOk);
    document.body.appendChild(modalAmizade);
    botaoOk.addEventListener("click", function (event) {
        event.preventDefault();
        modalAmizade.remove();
    });
});

function criarMensagem(mensagem) {
    console.log("teste");
    var position = mensagem.nomeUsuario === nomeUsuario ? "text-right" : "text-left";
    var alert = mensagem.nomeUsuario === nomeUsuario ? "alert-light" : "alert-dark";

    var message = document.createElement("div");
    message.className = "row";

    var messageCol = document.createElement("div");
    messageCol.classList = "col-md-12 " + position;

    var messageLink = document.createElement("a");
    messageLink.href = "/User/Index/" + mensagem.usuarioID;

    var messageImg = document.createElement("img");
    messageImg.src = linkImagemUsuario;
    messageImg.className = "usuario-imagem";

    var messageUser = document.createElement("small");
    messageUser.className = "text-dark";
    messageUser.textContent = mensagem.nomeUsuario;

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
    messageCol.appendChild(messageDt);
    message.appendChild(messageCol);
    console.log(message);
    console.log(innerChatId);
    document.getElementById(innerChatId).appendChild(message);
    objDiv.scrollTop = objDiv.scrollHeight;
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
                LinkImagem: linkImagemUsuario,
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
            console.log("1");
            connection.invoke("enviarPedidoAmizade", origem, destino).catch(function (err) {
                return console.error(err.toString());
            });
        });
    }
});