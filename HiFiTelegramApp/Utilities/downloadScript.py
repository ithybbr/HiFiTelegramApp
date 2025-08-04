from pyrogram import Client
import threading
import os
from dotenv import load_dotenv

load_dotenv()
api_id     = int(os.getenv("API_ID"))
api_hash   = os.getenv("API_HASH")
bot_token  = os.getenv("BOT_TOKEN")
channel_id = os.getenv("CHANNEL_ID")
bot = Client("hifimusic_bot", api_id=api_id, api_hash=api_hash, bot_token=bot_token)

# 2) Start it exactly once, on a background thread
_started = False
def start_bot():
    global _started
    if _started:
        return True
    try:
        t = threading.Thread(target=bot.run, daemon=True, name="PyrogramBot")
        t.start()
        _started = True
        return True
    except Exception:
        return False

def is_started():
    return _started

def download_song(message_id) -> str:
    try:
        await bot.start()
    except ConnectionError e:
        print(f"Connection error: {e}")
    message = bot.get_messages("hifimusicfromtidal", message_id)
    path = bot.download_media(message, file_name = "wwwroot/downloads/")
    return path