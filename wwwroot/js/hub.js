const hubConnections = {
    chat: HubConnectionBuilder('chat'),
    msg: HubConnectionBuilder('msg')
}

ChatHubConfigure();
MsgHubConfigure();

function HubConnectionBuilder(hub) {
    return new signalR.HubConnectionBuilder()
        .withUrl(`/${hub}`)
        .build();
}

function ChatHubConfigure() {
    hubConnections.chat.start()
        .then(() => {
            hubConnections.chat.on("notify", function (chat) {
                $("#notify").append([{ chatId: chat.id, chatName: chat.name }].map(notifyNewChat).join(''));
            });

            hubConnections.chat.on("new-chat", function (chat) {
                AddNewChat(chat);
            });

            hubConnections.chat.on("system-msg", function (chatId, resp) {
                $(`#${chatId}-chat`).append(`<p>${resp}</p>`);
            });
        })
        .catch((err) => {
            console.error('no cookie?' + err);

            $.ajax({
                url: `/startup?username=testUser`,
                type: 'POST',
                async: true
            }).done(() => {
                ChatHubConfigure();
            }).error((err) => {
                console.error(err);
            });
        });
}

function MsgHubConfigure() {
    hubConnections.msg.start()
        .then(() => {
            hubConnections.msg.on("message", function (resp) {
                console.info(resp);
            });
        })
        .catch((err) => {
            console.error('no cookie?' + err);
        });
}

function AddNewChat(chat) {
    $("#chats").append([{ chatId: chat.id, chatName: chat.name }].map(chatBtn).join(''));

    $("#chat-window").append([{ chatId: chat.id, chatName: chat.name }].map(chatWindow).join(''));
}

function OpenChat(chatId) {
    $("div[id$='-chat']").each(function () {
        $(this).addClass("d-none");
    });
    $(`#${chatId}`).removeClass("d-none");
}

function CreateChat() {
    let chat = $("#chat-name").val();
    hubConnections.chat.invoke("CreateChat", chat);
}

function LeaveChat(chatId) {
    hubConnections.chat.invoke("Leave", chatId);
    $("div[id$='-chat']").each(function () {
        $(this).addClass("d-none");
    });

    $(`#${chatId}-panel`).remove();
}

function JoinIn(chatId, chatName) {
    hubConnections.chat.invoke("JoinIn", chatId);
    let chat = {
        id: chatId,
        name: chatName
    };
    AddNewChat(chat);
}

function Send() {
    let chatId = $("div[id$='-chat']").attr("id").split('-chat')[0];
    let msg = $("#msg-area").val();

    hubConnections.msg.invoke("Send", chatId, msg);
}
