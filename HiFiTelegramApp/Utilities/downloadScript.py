import asyncio
from pyrogram import Client
import os
from dotenv import load_dotenv
import sys

def start_up(message_id: int) -> str:
    load_dotenv()
    api_id     = int(os.getenv("API_ID"))
    api_hash   = os.getenv("API_HASH")
    bot_token  = os.getenv("BOT_TOKEN")
    channel_id = os.getenv("CHANNEL_ID")

    # ensure there is a loop for this thread
    try:
        loop = asyncio.get_event_loop()
    except RuntimeError:
        loop = asyncio.new_event_loop()
        asyncio.set_event_loop(loop)

    with Client("hifimusic_bot", api_id=api_id, api_hash=api_hash, bot_token=bot_token) as app:
        # Run your async download_song directly, passing both args
        return download_song(app, message_id)

def download_song(app, message_id) -> str:
    message = app.get_messages("hifimusicfromtidal", message_id)
    path = app.download_media(message, file_name = "wwwroot/downloads/")
    return path