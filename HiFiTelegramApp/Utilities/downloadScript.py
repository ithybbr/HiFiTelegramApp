from pyrogram import Client
import os
import sys
import asyncio
from dotenv import load_dotenv

load_dotenv()
api_id     = int(os.getenv("API_ID"))
api_hash   = os.getenv("API_HASH")
bot_token  = os.getenv("BOT_TOKEN")
channel_id = os.getenv("CHANNEL_ID")

def start_bot(message_id: int) -> str:
    return asyncio.run(download(message_id))

async def download(message_id: int) -> str:
    # Use the async context manager
    async with Client(
        "hifimusic_bot",
        api_id=api_id,
        api_hash=api_hash,
        bot_token=bot_token
    ) as app:
        return await download_song(app, message_id)

async def download_song(app: Client, message_id: int) -> str:
    print("Starting to download song…")
    # get_messages is async, so await it
    message = await app.get_messages("hifimusicfromtidal", message_id)
    print(f"Message ID: {message_id}")
    # download_media is also async
    path = await app.download_media(message, file_name="wwwroot/downloads/")
    print(f"Downloaded to: {path}")
    return path
