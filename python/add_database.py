import ollama
import chromadb
import pandas as pd
import chardet
import os
from chromadb.config import Settings

src_directory = '../convert/'
database_directory = '../chromadb'

client = chromadb.PersistentClient(path = database_directory, settings = Settings(allow_reset = True))
collection = client.get_or_create_collection(name = 'docs')
files = os.listdir(src_directory)

i = 0

for i, d in enumerate(files):
    src_file = os.path.join(src_directory, files[i])

    # Skip if it's a folder
    if os.path.isdir(src_file):
        continue

    with open(src_file, 'rb') as f:
        # Check the encoding of the opened file
        raw_data = f.read()
        detected = chardet.detect(raw_data)
        detected_encoding = detected['encoding']

    # Skip if it's not a 'utf-8'
    if detected_encoding != 'utf-8':
        continue

    d = pd.read_csv(src_file, engine = 'python', encoding = 'utf-8')
    text = d.to_string(index = False)
    response = ollama.embeddings(model = 'mxbai-embed-large', prompt = text)
    embedding = response['embedding']
    collection.add(
        ids = [str(i)],
        embeddings = [embedding],
        documents = [text]
    )