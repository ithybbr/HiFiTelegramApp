from pyrogram import Client, filters
from pyrogram.types import InlineKeyboardButton, InlineKeyboardMarkup
import os
from dotenv import load_dotenv
import sys

def start_up(id):
    load_dotenv()
    
    # Define the Pyrogram client
    api_id = os.environ['API_ID']
    api_hash = os.environ['API_HASH']
    bot_token = os.environ['BOT_TOKEN']
    
    app = Client("hifimusic_bot", api_id=api_id, api_hash=api_hash, bot_token=bot_token)
    ids = []
    channel_id = os.environ['CHANNEL_ID']
    return app.run(download_song(app, id) #idk if it would work or how to do it better
async def download_song(app, id):
        file = await app.download_media(message = f"{id}")
        return file.name
    