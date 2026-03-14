# DiscordWebhookSender

## Usage
```csharp
Discord.SetToken("your token");

// Send single message
await Discord.SendMessage("Hello!");

// Send multiple messages
await Discord.SendRepeatedMessages(msg: "Hi", amount: 2, delaySecond: 1);

// Send file
await Discord.SendFile(@"C:\Users\user\Pictures\image.png");

// Optional: Customize bot
Discord.SetBotName("Robert");
Discord.SetBotAvatarUrl("https://example.com/avatar.png");
```
