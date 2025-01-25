import chromadb
from chromadb.config import Settings

client = chromadb.PersistentClient(path="../chromadb", settings=Settings(allow_reset=True))
client.reset()
print(f"Reset complete")