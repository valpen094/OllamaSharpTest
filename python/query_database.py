import ollama
import chromadb
from chromadb.config import Settings

src_directory = '../convert/'
database_directory = '../chromadb'

client = chromadb.PersistentClient(path = database_directory, settings = Settings(allow_reset = True))
collection = client.get_or_create_collection(name = 'docs')

user_text = "valpen"
response = ollama.embeddings(model = 'mxbai-embed-large', prompt = user_text)
results = collection.query(
    query_embeddings = [response["embedding"]],
    n_results=3
)

documents = results['documents'][0]
distances = results['distances'][0]

sorted_results = sorted(zip(distances, documents))

for distance, doc in sorted_results:
    print(f"\nVector distance: {distance:.2f}")
    print(f"Document: {doc}")