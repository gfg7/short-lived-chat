const chatWindow = ({ chatId, chatName }) => `
<div id="${chatId}-chat" class="d-none chat-window">
    <p> There have to be at least 2 person to sustain a conversation. You have 15 minutes to find a party. Long live the "${chatName}" chat!</p>
</div>`;

const chatBtn = ({ chatId, chatName }) => `
<div id='${chatId}-panel'>
    <div onclick="OpenChat('${chatId}-chat')">${chatName}</div>
    <div onclick="LeaveChat('${chatId}')">X</div>
</div>`;

const notifyNewChat = ({ chatId, chatName }) => `
<div onclick="JoinIn('${chatId}', '${chatName}')">New chat "${chatName}" has emerged! Waiting for newcomers</div>`;