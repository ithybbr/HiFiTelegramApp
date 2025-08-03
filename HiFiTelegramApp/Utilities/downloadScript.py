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
def start_up():
    global _started
    if _started:
        return
    _started = True

    def run():
        bot.run()   # this blocks inside Python but in our own thread

    t = threading.Thread(target=run, daemon=True, name="PyrogramBot")
    t.start()

def download_song(message_id) -> str:
    message = bot.get_messages("hifimusicfromtidal", message_id)
    path = bot.download_media(message, file_name = "wwwroot/downloads/")
    return path