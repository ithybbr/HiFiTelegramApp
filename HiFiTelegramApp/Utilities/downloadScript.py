from pyrogram import Client
import os
from dotenv import load_dotenv
import sys

def start_up(message_id: int) -> str:
    # 1) load our .env
    load_dotenv()

    # 2) grab vars (cast API_ID to int)
    api_id    = int(os.getenv("API_ID"))
    api_hash  = os.getenv("API_HASH")
    bot_token = os.getenv("BOT_TOKEN")
    channel_id = os.getenv("CHANNEL_ID")  # e.g. "-1001234567890"

    # 3) run the client synchronously
    with Client(
        "hifimusic_bot",
        api_id=api_id,
        api_hash=api_hash,
        bot_token=bot_token
    ) as app:
        # if you want to download a message from a channel:
        # add the chat_id: f"{channel_id}/{message_id}"
        message = app.get_messages("hifimusicfromtidal", message_id)
        path = app.download_media(message, file_name = "wwwroot/downloads/")
        return path
    