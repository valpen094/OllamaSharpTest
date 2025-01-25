import ollama
import chromadb
import pandas as pd
import os
from chromadb.config import Settings

client = chromadb.PersistentClient(path="../chromadb", settings=Settings(allow_reset=True))
collection = client.get_or_create_collection(name="docs")
csv_file=os.listdir('../convert')

i=0

for i, d in enumerate(csv_file):
    d=pd.read_csv('../convert/'+str(csv_file[i]),engine="python", encoding="utf-8-sig")
    text = d.to_string(index=False)
    response = ollama.embeddings(model="mxbai-embed-large", prompt=text)
    embedding = response["embedding"]
    collection.add(
        ids=[str(i)],
        embeddings=[embedding],
        documents=[text]
    )
    print(f"Add [{csv_file[i]}]")