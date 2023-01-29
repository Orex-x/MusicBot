using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;
using Yandex.Music.Client;
using Yandex.Music.Client.Extensions;

var bot = new TelegramBotClient("{API KEY}");

var client = new YandexMusicClient();
client.Authorize("{login}", "{password}");

bot.StartReceiving(Update, Error);

async Task Update(ITelegramBotClient bot, Update update, CancellationToken cancellationToken)
{
    try
    {
        switch (update.Type)
        {
            case Telegram.Bot.Types.Enums.UpdateType.Message:
                
                var message = update.Message?.Text;
                var chatId = update.Message!.Chat.Id;
                await Console.Out.WriteLineAsync($"id: {chatId} message: {message}");

                if (message!.Contains("start"))
                {
                    await bot.SendTextMessageAsync(chatId, "Привет, я питаюсь ссылками яндекс музыки. Покорми меня, а я тебя отблагадарю :>");
                    return;
                }

                if (message!.Contains("https://music.yandex"))
                {
                    await SendAudio(chatId, ParseLink(message), cancellationToken);
                    return;
                }


                await bot.SendTextMessageAsync(chatId, "нет, я такое не ем");
                break;

        }
    }
    catch (Exception ex)
    {
        await Console.Out.WriteLineAsync($"Exception {ex}");
    }
}


async Task Error(ITelegramBotClient arg1, Exception ex, CancellationToken arg3)
{
    await Console.Out.WriteLineAsync($"Exception {ex}");
}


async Task SendAudio(long chatId, string trackId, CancellationToken cancellationToken)
{
    var yTrack = client.GetTrack(trackId);
    var path = $@"music/{yTrack.Title}.mp3";
    YTrackExtensions.Save(yTrack, path);

    await using var stream = System.IO.File.OpenRead(path);
    await bot.SendDocumentAsync(chatId, new InputOnlineFile(stream, $"{yTrack.Title}.mp3"));
    stream.Close();
}

string ParseLink(string message)
{
    if (message.Contains("album"))
    {
        return message.Split("/")[6];
    }
    else
    {
        return message.Split("/")[4];
    }
}

Console.ReadLine();
