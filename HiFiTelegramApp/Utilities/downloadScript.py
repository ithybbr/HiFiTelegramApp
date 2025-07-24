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

    return Client(
        "hifimusic_bot",
        api_id=api_id,
        api_hash=api_hash,
        bot_token=bot_token
    ).run(download_song, channel_id, message_id)

async def download_song(app: Client, channel_id: str, message_id: int):
    file = await app.download_media(f"{channel_id}/{message_id}")
    return file.name